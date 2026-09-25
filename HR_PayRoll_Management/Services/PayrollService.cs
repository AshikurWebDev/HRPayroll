using HR_PayRoll_Management.DAL;
using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PayrollService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task GeneratePayrollAsync(int month, int year)
        {
            var employees = await _unitOfWork.Employees.FindAsync(x => x.IsActive);

            var existingCycle = await _unitOfWork.PayrollCycles.FindAsync(x =>
                x.CycleMonth == month &&
                x.CycleYear == year);

            if (existingCycle.Any())
            {
                throw new Exception($"Payroll for {month}/{year} has already been generated.");
            }

            var payrollCycle = new PayrollCycle
            {
                CycleMonth = month,
                CycleYear = year,
                ProcessedDate = DateTime.Now,
                Status = "Generated",
                TotalEmployees = employees.Count()
            };

            _unitOfWork.PayrollCycles.Add(payrollCycle);

            decimal totalGross = 0;
            decimal totalNet = 0;
            decimal totalDeduction = 0;

            foreach (var employee in employees)
            {
                var salaryStructures = await _unitOfWork.EmployeeSalaryStructures.FindAsync(x =>
                    x.EmployeeId == employee.EmployeeId &&
                    x.IsActive);

                var salary = salaryStructures
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefault();

                if (salary == null)
                {
                    continue;
                }

                var attendances = await _unitOfWork.Attendances.FindAsync(a =>
                    a.EmployeeId == employee.EmployeeId &&
                    a.AttendanceDate.Month == month &&
                    a.AttendanceDate.Year == year);

                // 1. Calculate actual absences
                int baseAbsentDays = attendances.Count(a => a.Status == "Absent");

                // 2. Calculate late penalty (3 days late = 1 absent)
                int lateDays = attendances.Count(a => a.IsLate || a.Status == "Late");
                int penaltyAbsentDays = lateDays / 3;

                int totalAbsentDays = baseAbsentDays + penaltyAbsentDays;

                // 3. Approved Leaves = No Salary Deduction for Paid Leaves, Deduction for Unpaid
                var allLeaves = await _unitOfWork.Leaves.GetAllAsync(x => x.LeaveType);
                var employeeLeaves = allLeaves.Where(l =>
                    l.EmployeeId == employee.EmployeeId &&
                    l.Status == "Approved" &&
                    ((l.FromDate.Month == month && l.FromDate.Year == year) || (l.ToDate.Month == month && l.ToDate.Year == year)));

                int approvedPaidLeaveDays = 0;
                int approvedUnpaidLeaveDays = 0;

                foreach (var leave in employeeLeaves)
                {
                    DateTime start = leave.FromDate > new DateTime(year, month, 1) ? leave.FromDate : new DateTime(year, month, 1);
                    DateTime end = leave.ToDate < new DateTime(year, month, DateTime.DaysInMonth(year, month)) ? leave.ToDate : new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    int daysInMonth = (end - start).Days + 1;

                    if (leave.LeaveType != null && !leave.LeaveType.IsPaid)
                    {
                        approvedUnpaidLeaveDays += daysInMonth;
                    }
                    else
                    {
                        approvedPaidLeaveDays += daysInMonth;
                    }
                }

                // 4. Overtime Calculation (Professional HR Improvement)
                double totalOvertimeHours = 0;
                foreach (var att in attendances.Where(a => a.Status == "Present" && a.WorkingHours.HasValue))
                {
                    // Assuming a standard 8-hour shift. Anything above 8 is overtime.
                    if (att.WorkingHours.Value > 8)
                    {
                        totalOvertimeHours += (att.WorkingHours.Value - 8);
                    }
                }

                int workingDays = DateTime.DaysInMonth(year, month);
                int presentDays = workingDays - totalAbsentDays - approvedPaidLeaveDays - approvedUnpaidLeaveDays;
                if (presentDays < 0) presentDays = 0;

                decimal grossSalary = salary.BasicSalary
                    + salary.HouseRent
                    + salary.MedicalAllowance
                    + salary.BonusAmount
                    + salary.TransportAllowance;

                decimal dailySalary = grossSalary / workingDays;
                decimal hourlySalary = dailySalary / 8m; // Assuming 8 hour work day

                decimal absentDeduction = dailySalary * totalAbsentDays;
                decimal leaveDeduction = dailySalary * approvedUnpaidLeaveDays; // Professional Rule: Unpaid leave = deduction

                decimal overtimeAmount = (decimal)totalOvertimeHours * (hourlySalary * 1.5m); // OT = 1.5x hourly rate

                decimal taxDeduction = grossSalary * (salary.TaxPercentage / 100);
                decimal totalEmployeeDeduction = taxDeduction + absentDeduction + leaveDeduction;
                decimal netSalary = (grossSalary + overtimeAmount) - totalEmployeeDeduction;

                var salarySlip = new SalarySlip
                {
                    EmployeeId = employee.EmployeeId,
                    PayrollCycle = payrollCycle,
                    BasicSalary = salary.BasicSalary,
                    HouseRent = salary.HouseRent,
                    MedicalAllowance = salary.MedicalAllowance,
                    TransportAllowance = salary.TransportAllowance,
                    BonusAmount = salary.BonusAmount,
                    OvertimeAmount = overtimeAmount,
                    GrossSalary = grossSalary,
                    WorkingDays = workingDays,
                    PresentDays = presentDays,
                    AbsentDays = totalAbsentDays,
                    AbsentDeduction = absentDeduction,
                    LeaveDeduction = leaveDeduction,
                    TaxDeduction = taxDeduction,
                    TotalDeduction = totalEmployeeDeduction,
                    NetSalary = netSalary
                };

                totalGross += grossSalary;
                totalDeduction += totalEmployeeDeduction;
                totalNet += netSalary;

                _unitOfWork.SalarySlips.Add(salarySlip);
            }

            payrollCycle.TotalGrossAmount = totalGross;
            payrollCycle.TotalDeductions = totalDeduction;
            payrollCycle.TotalNetAmount = totalNet;

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<PayrollViewModel>> GetPayrollHistoryAsync()
        {
            var cycles = await _unitOfWork.PayrollCycles.GetAllAsync();

            return cycles
                .OrderByDescending(x => x.CycleYear)
                .ThenByDescending(x => x.CycleMonth)
                .Select(x => new PayrollViewModel
                {
                    PayrollCycleId = x.PayrollCycleId,
                    CycleMonth = x.CycleMonth,
                    CycleYear = x.CycleYear,
                    ProcessedDate = x.ProcessedDate,
                    TotalEmployees = x.TotalEmployees,
                    TotalGrossAmount = x.TotalGrossAmount,
                    TotalDeductions = x.TotalDeductions,
                    TotalNetAmount = x.TotalNetAmount,
                    Status = x.Status
                });
        }

        public async Task<IEnumerable<SalarySlipListViewModel>> GetSalarySlipsByCycleAsync(int payrollCycleId)
        {
            var slips = await _unitOfWork.SalarySlips.GetAllAsync(
                x => x.Employee,
                x => x.Employee.Department,
                x => x.PayrollCycle
            );

            return slips
                .Where(x => x.PayrollCycleId == payrollCycleId)
                .Select(x => new SalarySlipListViewModel
                {
                    SalarySlipId = x.SalarySlipId,
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee != null ? x.Employee.FullName : "-",
                    DepartmentName = (x.Employee != null && x.Employee.Department != null)
                        ? x.Employee.Department.DepartmentName
                        : "-",
                    GrossSalary = x.GrossSalary,
                    TotalDeduction = x.TotalDeduction,
                    NetSalary = x.NetSalary,
                    Month = x.PayrollCycle.CycleMonth,
                    Year = x.PayrollCycle.CycleYear
                });
        }

        public async Task<PayslipViewModel> GetPayslipAsync(int salarySlipId)
        {
            var salarySlips = await _unitOfWork.SalarySlips.GetAllAsync(
                x => x.Employee,
                x => x.Employee.Department,
                x => x.Employee.Designation,
                x => x.PayrollCycle
            );

            var salarySlip = salarySlips.FirstOrDefault(x => x.SalarySlipId == salarySlipId);
            if (salarySlip == null)
            {
                return null;
            }

            return new PayslipViewModel
            {
                SalarySlipId = salarySlip.SalarySlipId,
                EmployeeId = salarySlip.EmployeeId,
                EmployeeName = salarySlip.Employee != null ? salarySlip.Employee.FullName : "-",
                PhotoPath = salarySlip.Employee != null ? salarySlip.Employee.PhotoPath : null,
                DepartmentName = (salarySlip.Employee != null && salarySlip.Employee.Department != null)
                    ? salarySlip.Employee.Department.DepartmentName
                    : "-",
                DesignationName = (salarySlip.Employee != null && salarySlip.Employee.Designation != null)
                    ? salarySlip.Employee.Designation.DesignationName
                    : "-",
                PayrollMonth = salarySlip.PayrollCycle.CycleMonth,
                PayrollYear = salarySlip.PayrollCycle.CycleYear,
                BasicSalary = salarySlip.BasicSalary,
                HouseRent = salarySlip.HouseRent,
                MedicalAllowance = salarySlip.MedicalAllowance,
                TransportAllowance = salarySlip.TransportAllowance,
                BonusAmount = salarySlip.BonusAmount,
                OvertimeAmount = salarySlip.OvertimeAmount,
                GrossSalary = salarySlip.GrossSalary,
                AbsentDays = salarySlip.AbsentDays,
                AbsentDeduction = salarySlip.AbsentDeduction,
                LeaveDeduction = salarySlip.LeaveDeduction,
                TaxDeduction = salarySlip.TaxDeduction,
                TotalDeduction = salarySlip.TotalDeduction,
                NetSalary = salarySlip.NetSalary,
                GeneratedDate = salarySlip.PayrollCycle.ProcessedDate
            };
        }

        public async Task ApprovePayrollAsync(int payrollCycleId)
        {
            var cycle = await _unitOfWork.PayrollCycles.GetByIdAsync(payrollCycleId);
            if (cycle != null && cycle.Status == "Generated")
            {
                cycle.Status = "Approved";
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task PayPayrollAsync(int payrollCycleId)
        {
            var cycle = await _unitOfWork.PayrollCycles.GetByIdAsync(payrollCycleId);
            if (cycle != null && cycle.Status == "Approved")
            {
                cycle.Status = "Paid";
                await _unitOfWork.SaveAsync();

                // Generate Notifications for Employees
                using (var db = new AppDbContext())
                {
                    var slips = db.SalarySlips.Where(s => s.PayrollCycleId == payrollCycleId).ToList();
                    foreach (var slip in slips)
                    {
                        var notif = new Notification
                        {
                            EmployeeId = slip.EmployeeId,
                            Title = "Salary Paid",
                            Message = $"Your salary for {System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(cycle.CycleMonth)} {cycle.CycleYear} has been deposited to your account. Net Amount: ৳{slip.NetSalary:N0}.",
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };
                        db.Notifications.Add(notif);
                    }
                    db.SaveChanges();
                }
            }
        }

        public async Task<PayrollCycle> GetPayrollCycleByIdAsync(int payrollCycleId)
        {
            return await _unitOfWork.PayrollCycles.GetByIdAsync(payrollCycleId);
        }

        public async Task DeletePayrollAsync(int payrollCycleId)
        {
            var cycle = await _unitOfWork.PayrollCycles.GetByIdAsync(payrollCycleId);
            if (cycle != null && cycle.Status != "Paid")
            {
                // Cascade delete salary slips
                var slips = await _unitOfWork.SalarySlips.FindAsync(s => s.PayrollCycleId == payrollCycleId);
                foreach (var slip in slips)
                {
                    _unitOfWork.SalarySlips.Delete(slip);
                }

                _unitOfWork.PayrollCycles.Delete(cycle);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}