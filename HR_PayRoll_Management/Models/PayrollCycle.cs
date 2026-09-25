using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_PayRoll_Management.Models
{
    public class PayrollCycle
    {
        [Key]
        public int PayrollCycleId { get; set; }

        [Required]
        [Index("IX_PayrollCycle_MonthYear", 1, IsUnique = true)]
        public int CycleMonth { get; set; }

        [Required]
        [Index("IX_PayrollCycle_MonthYear", 2, IsUnique = true)]
        public int CycleYear { get; set; }

        public DateTime ProcessedDate { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetAmount { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public virtual ICollection<SalarySlip> SalarySlips { get; set; }

        public PayrollCycle()
        {
            SalarySlips = new HashSet<SalarySlip>();
        }

    }
}