using System;

namespace HR_PayRoll_Management.ViewModels
{
    public class PayslipViewModel
    {
        public int SalarySlipId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PhotoPath { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public DateTime GeneratedDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRent { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal OvertimeAmount { get; set; }
        public decimal GrossSalary { get; set; }
        public int AbsentDays { get; set; }
        public decimal AbsentDeduction { get; set; }
        public decimal LeaveDeduction { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetSalary { get; set; }
    }
}