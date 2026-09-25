using System;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class AttendanceReportViewModel
    {
        public string EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }
        public bool IsLate { get; set; }
        public double? WorkingHour { get; set; }
    }
}