using HR_PayRoll_Management.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin,HR Manager,HR Officer")]
    public class PayrollReportsController : Controller
    {
        private readonly IPayrollReportService _payrollReportService;
        private readonly IEmployeeService _employeeService;

        public PayrollReportsController(IPayrollReportService payrollReportService, IEmployeeService employeeService)
        {
            _payrollReportService = payrollReportService;
            _employeeService = employeeService;
        }

        public async Task<ActionResult> Index(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;

            var dashboard = await _payrollReportService.GetPayrollReportDashboardAsync(selectedMonth, selectedYear);
            return View(dashboard);
        }

        public async Task<ActionResult> EmployeeHistory(int? employeeId)
        {
            var employees = await _employeeService.GetEmployeesAsync();
            ViewBag.Employees = employees;

            if (employeeId.HasValue)
            {
                var history = await _payrollReportService.GetEmployeeSalaryHistoryAsync(employeeId.Value);
                return View(history);
            }

            return View();
        }
    }
}
