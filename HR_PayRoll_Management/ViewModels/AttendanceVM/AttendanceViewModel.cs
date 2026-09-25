using System;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class AttendanceViewModel
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? EntryTime { get; set; }
        public TimeSpan? ExitTime { get; set; }
        public string Status { get; set; }
        public bool IsLate { get; set; }
        public double? WorkingHours { get; set; }
        public string Remarks { get; set; }
        public string PhotoPath { get; set; }
    }
}