using System;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class EmployeeSalaryStructure
    {
        [Key]
        public int SalaryStructureId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRent { get; set; }

        public decimal MedicalAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal TaxPercentage { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public virtual Employee Employee { get; set; }
    }
}