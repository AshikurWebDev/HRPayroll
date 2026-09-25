using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class AttendaneManagementVM
    {
        public DateTime AttendanceDate { get; set; }
        public int? DepartmentId { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<AttendanceViewModel> AttendanceRecords { get; set; }
    }
}