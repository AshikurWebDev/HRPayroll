using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_PayRoll_Management.Models
{
    public class Leave
    {
        [Key]
        public int LeaveId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        public int TotalDays { get; set; }

        [StringLength(200)]
        public string Reason { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Draft, Pending, Approved, Rejected, Applied, Cancelled

        public int? ApprovedById { get; set; }
        public bool PayrollImpactCalculated { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        [ForeignKey("ApprovedById")]
        public virtual User ApprovedBy { get; set; }
    }
}