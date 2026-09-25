using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services
{
    public class PayrollReportService : IPayrollReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PayrollReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MonthlyPayrollReportVM>> GetMonthlyPayrollReportsAsync()
        {
            var cycles = await _unitOfWork.PayrollCycles.GetAllAsync();
            return cycles.Select(c => new MonthlyPayrollReportVM
            {
                Month = c.CycleMonth,
                Year = c.CycleYear,
                TotalEmployees = c.TotalEmployees,
                TotalGrossAmount = c.TotalGrossAmount,
                TotalDeductions = c.TotalDeductions,
                TotalNetAmount = c.TotalNetAmount
            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToList();
        }

        public async Task<IEnumerable<DepartmentSalaryReportVM>> GetDepartmentSalaryReportsAsync(int month, int year)
        {
            var slips = await _unitOfWork.SalarySlips.GetAllAsync(x => x.Employee, x => x.Employee.Department, x => x.PayrollCycle);
            var filteredSlips = slips.Where(x => x.PayrollCycle.CycleMonth == month && x.PayrollCycle.CycleYear == year);

            return filteredSlips.GroupBy(x => (x.Employee != null && x.Employee.Department != null) ? x.Employee.Department.DepartmentName : "-")
                .Select(g => new DepartmentSalaryReportVM
                {
                    DepartmentName = g.Key,
                    EmployeeCount = g.Count(),
                    TotalGrossAmount = g.Sum(s => s.GrossSalary),
                    TotalNetAmount = g.Sum(s => s.NetSalary)
                }).ToList();
        }

        public async Task<IEnumerable<EmployeeSalaryHistoryVM>> GetEmployeeSalaryHistoryAsync(int employeeId)
        {
            var slips = await _unitOfWork.SalarySlips.GetAllAsync(x => x.Employee, x => x.Employee.Department, x => x.PayrollCycle);
            var filteredSlips = slips.Where(x => x.EmployeeId == employeeId);

            return filteredSlips.Select(x => new EmployeeSalaryHistoryVM
            {
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee != null ? x.Employee.FullName : "-",
                PhotoPath = x.Employee != null ? x.Employee.PhotoPath : null,
                DepartmentName = (x.Employee != null && x.Employee.Department != null) ? x.Employee.Department.DepartmentName : "-",
                Month = x.PayrollCycle.CycleMonth,
                Year = x.PayrollCycle.CycleYear,
                GrossSalary = x.GrossSalary,
                NetSalary = x.NetSalary
            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToList();
        }

        public async Task<IEnumerable<TaxReportVM>> GetTaxReportsAsync(int month, int year)
        {
            var slips = await _unitOfWork.SalarySlips.GetAllAsync(x => x.Employee, x => x.Employee.Department, x => x.PayrollCycle);
            var filteredSlips = slips.Where(x => x.PayrollCycle.CycleMonth == month && x.PayrollCycle.CycleYear == year);

            return filteredSlips.Select(x => new TaxReportVM
            {
                EmployeeName = x.Employee != null ? x.Employee.FullName : "-",
                DepartmentName = (x.Employee != null && x.Employee.Department != null) ? x.Employee.Department.DepartmentName : "-",
                Month = x.PayrollCycle.CycleMonth,
                Year = x.PayrollCycle.CycleYear,
                GrossSalary = x.GrossSalary,
                TaxDeduction = x.TaxDeduction
            }).OrderBy(x => x.DepartmentName).ThenBy(x => x.EmployeeName).ToList();
        }

        public async Task<IEnumerable<DeductionReportVM>> GetDeductionReportsAsync(int month, int year)
        {
            var slips = await _unitOfWork.SalarySlips.GetAllAsync(x => x.Employee, x => x.Employee.Department, x => x.PayrollCycle);
            var filteredSlips = slips.Where(x => x.PayrollCycle.CycleMonth == month && x.PayrollCycle.CycleYear == year);

            return filteredSlips.Select(x => new DeductionReportVM
            {
                EmployeeName = x.Employee != null ? x.Employee.FullName : "-",
                DepartmentName = (x.Employee != null && x.Employee.Department != null) ? x.Employee.Department.DepartmentName : "-",
                Month = x.PayrollCycle.CycleMonth,
                Year = x.PayrollCycle.CycleYear,
                AttendanceDeduction = x.AbsentDeduction,
                LeaveDeduction = x.LeaveDeduction,
                TotalDeduction = x.TotalDeduction
            }).OrderBy(x => x.DepartmentName).ThenBy(x => x.EmployeeName).ToList();
        }

        public async Task<PayrollReportDashboardVM> GetPayrollReportDashboardAsync(int month, int year)
        {
            return new PayrollReportDashboardVM
            {
                MonthlyReports = await GetMonthlyPayrollReportsAsync(),
                DepartmentReports = await GetDepartmentSalaryReportsAsync(month, year),
                TaxReports = await GetTaxReportsAsync(month, year),
                DeductionReports = await GetDeductionReportsAsync(month, year)
            };
        }
    }
}
