using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class LeaveType
    {
        [Key]
        public int LeaveTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int AllowedDays { get; set; }

        public bool IsPaid { get; set; }

        [StringLength(20)]
        public string GenderSpecific { get; set; } // "Male", "Female", or null for any

        public bool IsActive { get; set; }
    }
}