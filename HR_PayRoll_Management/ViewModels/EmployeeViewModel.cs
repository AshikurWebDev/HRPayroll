using HR_PayRoll_Management.ViewModels.AttendanceVM;

namespace HR_PayRoll_Management.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public System.DateTime DateOfBirth { get; set; }
        public decimal BasicSalary { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string ShiftName { get; set; }
        public string PhotoPath { get; set; }
        public bool IsActive { get; set; }

        public EmployeeAttendanceHistoryVM AttendanceHistory { get; set; }
        public System.Collections.Generic.List<HR_PayRoll_Management.ViewModels.Dashboard.DashboardPayrollVM> PayrollHistory { get; set; }
        public System.Collections.Generic.List<HR_PayRoll_Management.Models.EmployeeFile> Documents { get; set; }
    }
}