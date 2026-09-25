using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        public int RoleId { get; set; }
        
        public int? EmployeeId { get; set; }
        
        public virtual Role Role { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
