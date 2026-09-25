using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Mapping;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetEmployeesAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync(
                x => x.Department,
                x => x.Designation,
                x => x.Shift
            );

            return employees.Select(EmployeeMapper.ToViewModel).ToList();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _unitOfWork.Employees.GetByIdAsync(id);
        }

        public async Task<EmployeeViewModel> GetEmployeeDetailsAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetEmployeeDetailsAsync(id);
            if (employee == null)
            {
                return null;
            }

            var slips = await _unitOfWork.SalarySlips.FindAsync(
                s => s.EmployeeId == id,
                s => s.PayrollCycle,
                s => s.Employee,
                s => s.Employee.Department
            );

            var payrollHistory = slips.OrderByDescending(s => s.PayrollCycle.CycleYear)
                .ThenByDescending(s => s.PayrollCycle.CycleMonth)
                .Select(s => new HR_PayRoll_Management.ViewModels.Dashboard.DashboardPayrollVM
                {
                    SalarySlipId = s.SalarySlipId,
                    EmployeeName = s.Employee.FullName,
                    PhotoPath = s.Employee.PhotoPath,
                    DepartmentName = s.Employee.Department?.DepartmentName ?? "-",
                    GrossSalary = s.GrossSalary,
                    TotalDeduction = s.TotalDeduction,
                    NetSalary = s.NetSalary,
                    Status = s.PayrollCycle.Status
                }).ToList();

            var docs = (await _unitOfWork.EmployeeFiles.FindAsync(f => f.EmployeeId == id)).ToList();

            return new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                EmailAddress = employee.EmailAddress,
                Phone = employee.Phone,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                DepartmentName = employee.Department != null
                    ? employee.Department.DepartmentName
                    : "-",
                DesignationName = employee.Designation != null
                    ? employee.Designation.DesignationName
                    : "-",
                ShiftName = employee.Shift != null
                    ? employee.Shift.ShiftName
                    : "-",
                PhotoPath = employee.PhotoPath,
                PayrollHistory = payrollHistory,
                Documents = docs
            };
        }

        public async Task<EmployeeEditViewModel> GetEmployeeForEditAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetEmployeeDetailsAsync(id);
            if (employee == null)
            {
                return null;
            }

            return new EmployeeEditViewModel
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                EmailAddress = employee.EmailAddress,
                Phone = employee.Phone,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                DepartmentId = employee.DepartmentId,
                ShiftId = employee.ShiftId,
                DesignationId = employee.DesignationId,
                ExistingPhotoPath = employee.PhotoPath
            };
        }

        public async Task<EmployeeDeleteViewModel> GetEmployeeForDeleteAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return null;
            }

            return new EmployeeDeleteViewModel
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                EmailAddress = employee.EmailAddress,
                PhotoPath = employee.PhotoPath
            };
        }

                public async Task CreateAsync(Employee employee)
        {
            _unitOfWork.Employees.Add(employee);
            await _unitOfWork.SaveAsync();

            // Auto-provision User Account for this new employee
            using (var db = new HR_PayRoll_Management.DAL.AppDbContext())
            {
                var employeeRole = System.Linq.Queryable.FirstOrDefault(db.Roles, r => r.RoleName == "Employee");
                if (employeeRole != null && employee.EmployeeId > 0)
                {
                    string baseUsername = employee.FullName.Replace(" ", "").ToLower();
                    if (string.IsNullOrEmpty(baseUsername)) baseUsername = "user";
                    
                    string username = baseUsername;
                    int count = 1;
                    while (System.Linq.Queryable.Any(db.Users, u => u.Username == username))
                    {
                        username = baseUsername + count.ToString();
                        count++;
                    }

                    var newUser = new HR_PayRoll_Management.Models.User
                    {
                        Username = username,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                        RoleId = employeeRole.RoleId,
                        EmployeeId = employee.EmployeeId
                    };
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }
            }
        }

        public async Task UpdateAsync(Employee employee)
        {
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveAsync();
        }

                        public async Task DeleteAsync(Employee employee)
        {
            using (var db = new HR_PayRoll_Management.DAL.AppDbContext())
            {
                int empId = employee.EmployeeId;

                // PHASE 1: Nullify all foreign key references first
                var subordinates = System.Linq.Queryable.Where(db.Employees, e => e.ManagerId == empId).ToList();
                foreach(var sub in subordinates) { sub.ManagerId = null; }

                var userId = System.Linq.Queryable
                    .Where(db.Users, u => u.EmployeeId == empId)
                    .Select(u => (int?)u.UserId)
                    .FirstOrDefault();

                if (userId.HasValue)
                {
                    var approvedLeaves = System.Linq.Queryable.Where(db.Leaves, l => l.ApprovedById == userId.Value).ToList();
                    foreach (var lv in approvedLeaves) { lv.ApprovedById = null; }
                }

                db.SaveChanges(); // Save nullifications first

                // PHASE 2: Delete all dependent records
                if (userId.HasValue)
                {
                    var user = db.Users.Find(userId.Value);
                    if (user != null) db.Users.Remove(user);
                }

                db.Attendances.RemoveRange(System.Linq.Queryable.Where(db.Attendances, a => a.EmployeeId == empId).ToList());
                db.SalarySlips.RemoveRange(System.Linq.Queryable.Where(db.SalarySlips, s => s.EmployeeId == empId).ToList());
                db.EmployeeSalaryStructures.RemoveRange(System.Linq.Queryable.Where(db.EmployeeSalaryStructures, s => s.EmployeeId == empId).ToList());
                db.Leaves.RemoveRange(System.Linq.Queryable.Where(db.Leaves, l => l.EmployeeId == empId).ToList());
                db.EmployeeLeaveBalances.RemoveRange(System.Linq.Queryable.Where(db.EmployeeLeaveBalances, b => b.EmployeeId == empId).ToList());
                db.EmployeeFiles.RemoveRange(System.Linq.Queryable.Where(db.EmployeeFiles, f => f.EmployeeId == empId).ToList());
                db.Notifications.RemoveRange(System.Linq.Queryable.Where(db.Notifications, n => n.EmployeeId == empId).ToList());

                db.SaveChanges(); // Save deletions
            }

            _unitOfWork.Employees.Delete(employee);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetEmployeeDropdownAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();

            return employees.Select(x => new SelectListItem
            {
                Text = x.FullName,
                Value = x.EmployeeId.ToString()
            });
        }

        public async Task<IEnumerable<EmployeeDropdownViewModel>> GetEmployeesByDepartmentAsync(int? departmentId)
        {
            var employees = departmentId.HasValue 
                ? await _unitOfWork.Employees.FindAsync(x => x.DepartmentId == departmentId.Value, x => x.Department)
                : await _unitOfWork.Employees.GetAllAsync(x => x.Department);

            return employees.Select(x => new EmployeeDropdownViewModel
            {
                EmployeeId = x.EmployeeId,
                FullName = x.FullName,
                DepartmentName = x.Department != null
                    ? x.Department.DepartmentName
                    : "-",
                PhotoPath = x.PhotoPath
            });
        }

        public async Task<IEnumerable<EmployeeDropdownViewModel>> GetEmployeesByDepartmentDesignationAsync(int departmentId, int designationId)
        {
            var employees = await _unitOfWork.Employees.FindAsync(x =>
                x.DepartmentId == departmentId &&
                x.DesignationId == designationId &&
                x.IsActive
            );

            return employees.Select(x => new EmployeeDropdownViewModel
            {
                EmployeeId = x.EmployeeId,
                FullName = x.FullName,
                PhotoPath = x.PhotoPath
            });
        }
    }
}


