using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        public bool IsActive { get; set; }


        // Navigation Property
        public virtual ICollection<Employee> Employees { get; set; }

        public Department()
        {
            Employees = new HashSet<Employee>();
        }
    }
}