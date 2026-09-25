using System;
using System.Collections.Generic;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class EmployeeAttendanceHistoryVM
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public List<int> AvailableYears { get; set; }
        //summary 
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int LeaveDays { get; set; }
        public double AverageWorkingHours { get; set; }
        public double AttendancePercentage { get; set; }
        public List<AttendanceRecordVM> AttendanceRecords { get; set; }
    }

    public class AttendanceRecordVM
    {
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }
        public TimeSpan? EntryTime { get; set; }
        public TimeSpan? ExitTime { get; set; }
        public bool IsLate { get; set; }
        public double? WorkingHour { get; set; }
        public string Remarks { get; set; }
    }
}