using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class SalarySlip
    {
        [Key]
        public int SalarySlipId { get; set; }

        // Foreign Key
        public int PayrollCycleId { get; set; }

        // Foreign Key
        public int EmployeeId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRent { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal BonusAmount { get; set; }
        public int WorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public decimal AbsentDeduction { get; set; }
        public decimal LeaveDeduction { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal NetSalary { get; set; }
        public decimal OvertimeAmount { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalDeduction { get; set; }
        // Navigation Properties
        public virtual PayrollCycle PayrollCycle { get; set; }
        public virtual Employee Employee { get; set; }
    }
}