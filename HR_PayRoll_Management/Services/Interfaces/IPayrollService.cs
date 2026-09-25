using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface IPayrollService
    {
        Task GeneratePayrollAsync(int month, int year);
        Task<IEnumerable<PayrollViewModel>> GetPayrollHistoryAsync();
        Task<IEnumerable<SalarySlipListViewModel>>GetSalarySlipsByCycleAsync(int payrollCycleId);
        Task<PayslipViewModel> GetPayslipAsync(int salarySlipId);
        Task ApprovePayrollAsync(int payrollCycleId);
        Task PayPayrollAsync(int payrollCycleId);
        Task DeletePayrollAsync(int payrollCycleId);
        Task<PayrollCycle> GetPayrollCycleByIdAsync(int payrollCycleId);
    }
}
