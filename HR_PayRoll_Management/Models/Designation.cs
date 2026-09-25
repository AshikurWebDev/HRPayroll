using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class Designation
    {
        [Key]
        public int DesignationId { get; set; }


        [Required]
        [StringLength(100)]
        public string DesignationName { get; set; }


        [StringLength(250)]
        public string Description { get; set; }


        public bool IsActive { get; set; }


        public DateTime CreatedDate { get; set; }


        public virtual ICollection<Employee> Employees { get; set; }


        public Designation()
        {
            Employees = new HashSet<Employee>();

            IsActive = true;

            CreatedDate = DateTime.Now;
        }
    }
}