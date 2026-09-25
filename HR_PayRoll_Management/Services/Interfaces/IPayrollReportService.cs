using HR_PayRoll_Management.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface IPayrollReportService
    {
        Task<IEnumerable<MonthlyPayrollReportVM>> GetMonthlyPayrollReportsAsync();
        Task<IEnumerable<DepartmentSalaryReportVM>> GetDepartmentSalaryReportsAsync(int month, int year);
        Task<IEnumerable<EmployeeSalaryHistoryVM>> GetEmployeeSalaryHistoryAsync(int employeeId);
        Task<IEnumerable<TaxReportVM>> GetTaxReportsAsync(int month, int year);
        Task<IEnumerable<DeductionReportVM>> GetDeductionReportsAsync(int month, int year);
        Task<PayrollReportDashboardVM> GetPayrollReportDashboardAsync(int month, int year);
    }
}
