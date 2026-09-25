using HR_PayRoll_Management.ViewModels.AttendanceVM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Service_Attandance
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceViewModel>> GetEmployeeAttendanceAsync(int employeeId);

        Task<AttendanceViewModel> GetByIdAsync(int id);

        Task CreateAsync(AttendanceCreateViewModel model);

        Task UpdateAsync(AttendanceEditViewModel model);

        Task DeleteAsync(int id);

        Task<IEnumerable<AttendanceReportViewModel>> GetDailyAttendanceAsync(DateTime date);

        Task<IEnumerable<AttendanceReportViewModel>> GetMonthlyAttendanceAsync(
            int month,
            int year);
        Task<int> GetAbsentCountAsync(int employeeId, int month, int year);
        Task<DailyAttendancePageViewModel> GetDailyAttendancePageAsync(DateTime date, int? departmentId);
        Task<IEnumerable<SelectListItem>> GetDepartmentsAsync();

        Task SaveAttendanceAsync(SaveAttendanceViewModel model);
        Task<EmployeeAttendanceHistoryVM> GetEmployeeAttendanceHistoryAsync(int employeeId);

        Task<EmployeeAttendanceHistoryVM> GetEmployeeAttendanceHistoryByMonthAsync(int employeeId, int month, int year);

        Task<AttendaneManagementVM> GetAttendanceManagementAsync(DateTime? date, int? departmentId, string status);

        Task<AttendaneManagementVM> GetAttendanceManagementByMonthAsync(int month, int year, int? departmentId, string status);

        //Reporting 
        Task<AttendanceSummaryReportVM> GetAttendanceSummaryReportAsync(int month, int year, int? departmentId);
    }
}