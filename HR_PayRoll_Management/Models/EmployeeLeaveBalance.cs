using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_PayRoll_Management.Models
{
    public class EmployeeLeaveBalance
    {
        [Key, Column(Order = 0)]
        public int EmployeeId { get; set; }

        [Key, Column(Order = 1)]
        public int LeaveTypeId { get; set; }

        [Required]
        public int Year { get; set; }

        public int AllocatedDays { get; set; }
        public int UsedDays { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual LeaveType LeaveType { get; set; }
    }
}
