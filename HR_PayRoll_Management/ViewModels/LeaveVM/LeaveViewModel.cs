using System;

namespace HR_PayRoll_Management.ViewModels.LeaveVM
{
    public class LeaveViewModel
    {
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string LeaveType { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int TotalDays { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }
    }
}