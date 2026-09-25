using HR_PayRoll_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public class AttendanceRepository
        : Repository<Attendance>, IAttendanceRepository
    {


        public AttendanceRepository(AppDbContext context) : base(context) { }

        // Employee Attendance History
        public async Task<IEnumerable<Attendance>> GetEmployeeAttendanceAsync(int employeeId)
        {

            return await _context.Attendances
                .Include(x => x.Employee)
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.AttendanceDate)
                .ToListAsync();

        }

        // Daily Attendance
        public async Task<IEnumerable<Attendance>>
            GetDailyAttendanceAsync(DateTime date)
        {

            return await _context.Attendances
                .Include(x => x.Employee)
                .Where(x =>
                    DbFunctions.TruncateTime(x.AttendanceDate)
                    ==
                    DbFunctions.TruncateTime(date))
                .OrderBy(x => x.Employee.FullName)
                .ToListAsync();
        }

        // Monthly Attendance Report
        public async Task<IEnumerable<Attendance>> GetMonthlyAttendanceAsync(
            int month,
            int year)
        {

            return await _context.Attendances
                .Include(x => x.Employee)
                .Where(x =>
                    x.AttendanceDate.Month == month
                    &&
                    x.AttendanceDate.Year == year)
                .OrderBy(x => x.AttendanceDate)
                .ToListAsync();

        }

        // Duplicate Checking
        public async Task<bool>
            AttendanceExistsAsync(
            int employeeId,
            DateTime attendanceDate)
        {

            return await _context.Attendances
                .AnyAsync(x =>
                    x.EmployeeId == employeeId
                    &&
                    DbFunctions.TruncateTime(
                        x.AttendanceDate)
                    ==
                    DbFunctions.TruncateTime(
                        attendanceDate));

        }

    }
}