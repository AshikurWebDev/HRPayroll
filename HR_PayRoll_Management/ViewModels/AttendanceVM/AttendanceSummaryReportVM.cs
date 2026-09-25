using System;
using System.Collections.Generic;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class AttendanceSummaryReportVM
    {
        public int Month { get; set; }

        public int Year { get; set; }
        public string DepartmentName { get; set; }

        public DateTime GeneratedDate { get; set; }

        // Summary Cards

        public int TotalEmployees { get; set; }

        public int TotalAttendance { get; set; }

        public int PresentCount { get; set; }

        public int AbsentCount { get; set; }

        public int LeaveCount { get; set; }

        public int LateCount { get; set; }

        public double PresentPercentage { get; set; }

        public double AbsentPercentage { get; set; }

        public double LeavePercentage { get; set; }

        public List<DepartmentAttendanceReportVM> DepartmentReports { get; set; }
        public List<EmployeeAttendanceReportVM> EmployeeReports { get; set; }
    }

    public class EmployeeAttendanceReportVM
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }


        public int PresentDays { get; set; }

        public int AbsentDays { get; set; }

        public int LeaveDays { get; set; }

        public int LateDays { get; set; }


        public double AverageWorkingHours { get; set; }

        public double AttendancePercentage { get; set; }
    }

    public class DepartmentAttendanceReportVM
    {
        public string DepartmentName { get; set; }

        public int TotalAttendance { get; set; }

        public int PresentDays { get; set; }

        public double AttendancePercentage { get; set; }
    }
}