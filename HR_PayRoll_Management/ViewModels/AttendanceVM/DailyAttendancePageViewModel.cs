using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class DailyAttendancePageViewModel
    {
        public DateTime AttendanceDate { get; set; }
        public int? DepartmentId { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public List<DailyAttendanceViewModel> Employees { get; set; }
    }
}