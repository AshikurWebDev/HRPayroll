using HR_PayRoll_Management.DAL.Interfaces;
using HR_PayRoll_Management.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {

        Task<IEnumerable<Attendance>> GetEmployeeAttendanceAsync(int employeeId);
        Task<IEnumerable<Attendance>> GetDailyAttendanceAsync(DateTime date);
        Task<IEnumerable<Attendance>> GetMonthlyAttendanceAsync( int month,int year);
        Task<bool> AttendanceExistsAsync(int employeeId,DateTime attendanceDate);

    }
}