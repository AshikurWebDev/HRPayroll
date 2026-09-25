using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HR_PayRoll_Management.ViewModels.DepartmentVM
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }


        [Required]
        [Display(Name = "Department Name")]
        [StringLength(100)]
        public string DepartmentName { get; set; }



        [Display(Name = "Description")]
        [StringLength(250)]
        public string Description { get; set; }



        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }


        public int EmployeeCount { get; set; }

    }
}