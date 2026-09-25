using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        public virtual System.Collections.Generic.ICollection<RolePermission> RolePermissions { get; set; } = new System.Collections.Generic.List<RolePermission>();
    }
}
