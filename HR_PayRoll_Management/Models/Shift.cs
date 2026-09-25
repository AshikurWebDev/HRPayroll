using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class Shift
    {
        [Key]
        public int ShiftId { get; set; }

        [Required]
        [StringLength(50)]
        public string ShiftName { get; set; }


        [Required]
        public TimeSpan StartTime { get; set; }


        [Required]
        public TimeSpan EndTime { get; set; }


        // Allowed late time in minutes
        public int LateAllowanceMinutes { get; set; }

        // Navigation Property
        public virtual ICollection<Employee> Employees { get; set; }
    }
}