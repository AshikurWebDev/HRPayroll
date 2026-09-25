using System.Collections.Generic;

namespace HR_PayRoll_Management.ViewModels
{
    public class MonthlyPayrollReportVM
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetAmount { get; set; }
    }

    public class DepartmentSalaryReportVM
    {
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal TotalNetAmount { get; set; }
    }

    public class EmployeeSalaryHistoryVM
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PhotoPath { get; set; }
        public string DepartmentName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
    }

    public class TaxReportVM
    {
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TaxDeduction { get; set; }
    }

    public class DeductionReportVM
    {
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AttendanceDeduction { get; set; }
        public decimal LeaveDeduction { get; set; }
        public decimal TotalDeduction { get; set; }
    }

    public class PayrollReportDashboardVM
    {
        public IEnumerable<MonthlyPayrollReportVM> MonthlyReports { get; set; }
        public IEnumerable<DepartmentSalaryReportVM> DepartmentReports { get; set; }
        public IEnumerable<TaxReportVM> TaxReports { get; set; }
        public IEnumerable<DeductionReportVM> DeductionReports { get; set; }
    }
}
