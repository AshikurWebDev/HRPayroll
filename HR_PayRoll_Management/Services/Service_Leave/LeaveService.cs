using HR_PayRoll_Management.DAL;
using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.ViewModels.LeaveVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Service_Leave
{
    public class LeaveService : ILeaveService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LeaveViewModel>> GetAllAsync(int? employeeId = null, bool viewTeam = false, bool viewAll = false)
        {
            var leaves = await _unitOfWork.Leaves.GetAllAsync(
                x => x.Employee,
                x => x.LeaveType
            );

            if (!viewAll)
            {
                                if (viewTeam && employeeId.HasValue)
                {
                    int mgrDeptId = 0;
                    using (var db = new DAL.AppDbContext())
                    {
                        var e = db.Employees.Find(employeeId.Value);
                        if (e != null) mgrDeptId = e.DepartmentId;
                    }
                    leaves = leaves.Where(x => x.Employee.DepartmentId == mgrDeptId);
                }
                else if (employeeId.HasValue)
                {
                    leaves = leaves.Where(x => x.EmployeeId == employeeId.Value);
                }
            }

            return leaves.Select(x => new LeaveViewModel
            {
                LeaveId = x.LeaveId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.FullName,
                LeaveType = x.LeaveType.Name,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                TotalDays = x.TotalDays,
                Reason = x.Reason,
                Status = x.Status
            });
        }

                public async Task CreateAsync(LeaveCreateViewModel model)
        {
            int totalDays = (model.ToDate - model.FromDate).Days + 1;
            int year = model.FromDate.Year;

            // Retrieve Employee and Leave Type
            var employee = await _unitOfWork.Employees.GetByIdAsync(model.EmployeeId);
            var leaveType = await _unitOfWork.LeaveTypes.GetByIdAsync(model.LeaveTypeId);

            if (employee == null || leaveType == null)
                throw new System.Exception("Employee or Leave Type not found.");

            // Gender Specific Check
            if (!string.IsNullOrEmpty(leaveType.GenderSpecific) && employee.Gender != leaveType.GenderSpecific && leaveType.GenderSpecific != "A")
            {
                throw new System.Exception($"This leave type is restricted to {leaveType.GenderSpecific} employees only.");
            }

            // Unpaid leave check bypasses balance
            if (leaveType.IsPaid)
            {
                using (var db = new AppDbContext())
                {
                    var balance = System.Linq.Queryable.FirstOrDefault(db.EmployeeLeaveBalances, b => b.EmployeeId == model.EmployeeId && b.LeaveTypeId == model.LeaveTypeId);

                    if (balance == null)
                    {
                        balance = new EmployeeLeaveBalance
                        {
                            EmployeeId = model.EmployeeId,
                            LeaveTypeId = model.LeaveTypeId,
                            Year = year,
                            AllocatedDays = leaveType.AllowedDays,
                            UsedDays = 0
                        };
                        db.EmployeeLeaveBalances.Add(balance);
                        db.SaveChanges();
                    }
                    else if (balance.Year != year)
                    {
                        // Reset balance for the new year since Year is not in the PK
                        balance.Year = year;
                        balance.AllocatedDays = leaveType.AllowedDays;
                        balance.UsedDays = 0;
                        db.SaveChanges();
                    }

                    int remaining = balance.AllocatedDays - balance.UsedDays;
                    if (remaining < 0) remaining = 0;

                    if (totalDays > remaining)
                    {
                        int paidDays = remaining;
                        int unpaidDays = totalDays - paidDays;

                        // Create paid portion if they have some balance left
                        if (paidDays > 0)
                        {
                            string pReason = model.Reason ?? "";
                            if (pReason.Length > 200) pReason = pReason.Substring(0, 197) + "...";

                            var paidLeave = new Leave
                            {
                                EmployeeId = model.EmployeeId,
                                LeaveTypeId = model.LeaveTypeId,
                                FromDate = model.FromDate,
                                ToDate = model.FromDate.AddDays(paidDays - 1),
                                TotalDays = paidDays,
                                Reason = pReason,
                                Status = "Pending",
                                PayrollImpactCalculated = false
                            };
                            _unitOfWork.Leaves.Add(paidLeave);
                        }

                        // Create unpaid portion for the overflow
                        var unpaidType = System.Linq.Queryable.FirstOrDefault(db.LeaveTypes, lt => !lt.IsPaid);
                        int unpaidTypeId = unpaidType != null ? unpaidType.LeaveTypeId : model.LeaveTypeId;

                        string uReason = (model.Reason ?? "") + $" (Auto-converted {unpaidDays} days to unpaid)";
                        if (uReason.Length > 200) uReason = uReason.Substring(0, 197) + "...";

                        var unpaidLeave = new Leave
                        {
                            EmployeeId = model.EmployeeId,
                            LeaveTypeId = unpaidTypeId,
                            FromDate = model.FromDate.AddDays(paidDays),
                            ToDate = model.ToDate,
                            TotalDays = unpaidDays,
                            Reason = uReason,
                            Status = "Pending",
                            PayrollImpactCalculated = false
                        };
                        _unitOfWork.Leaves.Add(unpaidLeave);

                        await _unitOfWork.SaveAsync();
                        return; // Done
                    }
                }
            }

            string safeReason = model.Reason ?? "";
            if (safeReason.Length > 200) safeReason = safeReason.Substring(0, 200);

            var leave = new Leave
            {
                EmployeeId = model.EmployeeId,
                LeaveTypeId = model.LeaveTypeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TotalDays = totalDays,
                Reason = safeReason,
                Status = "Pending",
                PayrollImpactCalculated = false
            };

            _unitOfWork.Leaves.Add(leave);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var leave = await _unitOfWork.Leaves.GetByIdAsync(id);
            if (leave == null)
                return;

            _unitOfWork.Leaves.Delete(leave);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetLeaveTypesAsync()
        {
            var types = await _unitOfWork.LeaveTypes.GetAllAsync();


            return types
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.LeaveTypeId.ToString(),
                    Text = x.Name
                });
        }

        public async Task<LeaveEditViewModel> GetByIdAsync(int id)
        {
            var leave =
                await _unitOfWork.Leaves.GetByIdAsync(id);


            if (leave == null)
                return null;


            return new LeaveEditViewModel
            {
                LeaveId = leave.LeaveId,

                EmployeeId = leave.EmployeeId,

                LeaveTypeId = leave.LeaveTypeId,

                FromDate = leave.FromDate,

                ToDate = leave.ToDate,

                Reason = leave.Reason,

                Status = leave.Status
            };
        }

        public async Task UpdateAsync(
    LeaveEditViewModel model)
        {

            var leave =
                await _unitOfWork.Leaves
                .GetByIdAsync(model.LeaveId);


            if (leave == null)
                return;


            leave.LeaveTypeId =
                model.LeaveTypeId;


            leave.FromDate =
                model.FromDate;


            leave.ToDate =
                model.ToDate;


            leave.TotalDays =
                (model.ToDate -
                model.FromDate)
                .Days + 1;


            leave.Reason =
                model.Reason;


            leave.Status =
                model.Status;


            await _unitOfWork.SaveAsync();

        }

        public async Task ApproveLeaveAsync(int leaveId, int approvedByUserId)
        {
            var leave = await _unitOfWork.Leaves.GetByIdAsync(leaveId);
            if (leave == null || leave.Status != "Pending") return;

            leave.Status = "Approved";
            leave.ApprovedById = approvedByUserId;

            // Consume balance if it's paid leave
            var leaveType = await _unitOfWork.LeaveTypes.GetByIdAsync(leave.LeaveTypeId);
            if (leaveType != null && leaveType.IsPaid)
            {
                using (var db = new AppDbContext())
                {
                    var balance = db.EmployeeLeaveBalances.FirstOrDefault(b => b.EmployeeId == leave.EmployeeId && b.LeaveTypeId == leave.LeaveTypeId && b.Year == leave.FromDate.Year);
                    if (balance != null)
                    {
                        balance.UsedDays += leave.TotalDays;
                        db.SaveChanges();
                    }
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task RejectLeaveAsync(int leaveId, int rejectedByUserId)
        {
            var leave = await _unitOfWork.Leaves.GetByIdAsync(leaveId);
            if (leave == null || leave.Status != "Pending") return;

            leave.Status = "Rejected";
            leave.ApprovedById = rejectedByUserId; // Tracing who rejected it
            await _unitOfWork.SaveAsync();
        }
    }
}

