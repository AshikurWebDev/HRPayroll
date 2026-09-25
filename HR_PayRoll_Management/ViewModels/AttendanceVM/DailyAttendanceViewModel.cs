using System;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class DailyAttendanceViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        [Required]
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }
        public TimeSpan? EntryTime { get; set; }
        public TimeSpan? ExitTime { get; set; }
        public string Remarks { get; set; }
        public string PhotoPath { get; set; }
    }
}