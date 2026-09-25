using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace HR_PayRoll_Management.ViewModels
{
    public class EmployeeEditViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public System.DateTime DateOfBirth { get; set; }
        public int DepartmentId { get; set; }
        public int ShiftId { get; set; }
        public int DesignationId { get; set; }
        public IEnumerable<SelectListItem> Designations { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Shifts { get; set; }
        public string ExistingPhotoPath { get; set; }
        public HttpPostedFileBase Photo { get; set; }

    }
}