using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_PayRoll_Management.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(150)]
        public string EmailAddress { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhotoPath { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool IsActive { get; set; }

        // Foreign Key
        public int ShiftId { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("Designation")]
        public int DesignationId { get; set; }

        // Navigation Property
        public virtual Department Department { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual Designation Designation { get; set; }

        // Future relationships
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<EmployeeFile> EmployeeFiles { get; set; }
        public virtual ICollection<SalarySlip> SalarySlips { get; set; }
        public Employee()
        {
            Attendances = new HashSet<Attendance>();

            EmployeeFiles = new HashSet<EmployeeFile>();

            SalarySlips = new HashSet<SalarySlip>();
        }

        // Self-referencing Foreign Key for Manager
        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; }

        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    }
}