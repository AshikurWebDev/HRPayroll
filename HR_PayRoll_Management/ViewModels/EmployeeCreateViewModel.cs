using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace HR_PayRoll_Management.ViewModels
{
    public class EmployeeCreateViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        
        [StringLength(20)]
        public string Phone { get; set; }
        
        [Required]
        public string Gender { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public System.DateTime DateOfBirth { get; set; }

        [Required]
        public int DepartmentId { get; set; }
        public int ShiftId { get; set; }
        public int DesignationId { get; set; }

        //DropDown Data 
        public IEnumerable<SelectListItem> Designations { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Shifts { get; set; }

        //Image Upload 
        public HttpPostedFileBase Photo { get; set; }
    }
}