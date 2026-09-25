using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.ViewModels.AttendanceVM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Service_Attandance
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AttendanceViewModel>> GetEmployeeAttendanceAsync(int employeeId)
        {
            var attendances = await _unitOfWork.Attendances.GetEmployeeAttendanceAsync(employeeId);

            return attendances.Select(x => new AttendanceViewModel
            {
                AttendanceId = x.AttendanceId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.FullName,
                AttendanceDate = x.AttendanceDate,
                EntryTime = x.EntryTime,
                ExitTime = x.ExitTime,
                Status = x.Status,
                IsLate = x.IsLate,
                WorkingHours = x.WorkingHours,
                Remarks = x.Remarks
            });
        }

        public async Task<AttendanceViewModel> GetByIdAsync(int id)
        {
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                return null;

            return new AttendanceViewModel
            {
                AttendanceId = attendance.AttendanceId,
                EmployeeId = attendance.EmployeeId,
                EmployeeName = attendance.Employee.FullName,
                AttendanceDate = attendance.AttendanceDate,
                EntryTime = attendance.EntryTime,
                ExitTime = attendance.ExitTime,
                Status = attendance.Status,
                IsLate = attendance.IsLate,
                WorkingHours = attendance.WorkingHours,
                Remarks = attendance.Remarks
            };
        }

        public async Task CreateAsync(AttendanceCreateViewModel model)
        {
            var employee = await _unitOfWork.Employees.GetEmployeeWithShiftAsync(model.EmployeeId);
            if (employee == null)
                return;

            bool isLate = false;
            double? workingHours = null;

            if (model.EntryTime.HasValue && employee != null && employee.Shift != null)
            {
                var allowTime = employee.Shift.StartTime.Add(TimeSpan.FromMinutes(employee.Shift.LateAllowanceMinutes));
                if (model.EntryTime.Value > allowTime)
                {
                    isLate = true;
                }
            }

            if (model.EntryTime.HasValue && model.ExitTime.HasValue)
            {
                var duration = model.ExitTime.Value - model.EntryTime.Value;
                workingHours = duration.TotalHours;
            }

            var attendance = new Attendance
            {
                EmployeeId = model.EmployeeId,
                AttendanceDate = model.AttendanceDate,
                EntryTime = model.EntryTime,
                ExitTime = model.ExitTime,
                Status = model.Status,
                IsLate = isLate,
                WorkingHours = workingHours,
                Remarks = model.Remarks
            };

            var existingAttendance = await _unitOfWork.Attendances.FindAsync(x =>
                x.EmployeeId == model.EmployeeId &&
                DbFunctions.TruncateTime(x.AttendanceDate) == DbFunctions.TruncateTime(model.AttendanceDate));

            if (existingAttendance.Any())
            {
                throw new Exception("Attendance Already exists for this employee on this date");
            }

            _unitOfWork.Attendances.Add(attendance);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(AttendanceEditViewModel model)
        {
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(model.AttendanceId);
            if (attendance == null)
                return;

            var employee = await _unitOfWork.Employees.GetEmployeeWithShiftAsync(model.EmployeeId);

            bool isLate = false;
            double? workingHours = null;

            if (model.EntryTime.HasValue && employee.Shift != null)
            {
                var allowTime = employee.Shift.StartTime.Add(TimeSpan.FromMinutes(employee.Shift.LateAllowanceMinutes));
                if (model.EntryTime.Value > allowTime)
                    isLate = true;
            }

            if (model.EntryTime.HasValue && model.ExitTime.HasValue)
            {
                var duration = model.ExitTime.Value - model.EntryTime.Value;
                workingHours = duration.TotalHours;
            }

            attendance.AttendanceDate = model.AttendanceDate;
            attendance.EntryTime = model.EntryTime;
            attendance.ExitTime = model.ExitTime;
            attendance.Status = model.Status;
            attendance.IsLate = isLate;
            attendance.WorkingHours = workingHours;
            attendance.Remarks = model.Remarks;

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                return;

            _unitOfWork.Attendances.Delete(attendance);
            await _unitOfWork.SaveAsync();
        }

        // Retrieving the daily attendance 
        public async Task<IEnumerable<AttendanceReportViewModel>> GetDailyAttendanceAsync(DateTime date)
        {
            var data = await _unitOfWork.Attendances.GetDailyAttendanceAsync(date);

            return data.Select(x => new AttendanceReportViewModel
            {
                EmployeeName = x.Employee.FullName,
                AttendanceDate = x.AttendanceDate,
                Status = x.Status,
                IsLate = x.IsLate,
                WorkingHour = x.WorkingHours
            });
        }

        // Retrieving the monthly report 
        public async Task<IEnumerable<AttendanceReportViewModel>> GetMonthlyAttendanceAsync(int month, int year)
        {
            var data = await _unitOfWork.Attendances.GetMonthlyAttendanceAsync(month, year);

            return data.Select(x => new AttendanceReportViewModel
            {
                EmployeeName = x.Employee.FullName,
                AttendanceDate = x.AttendanceDate,
                Status = x.Status,
                IsLate = x.IsLate,
                WorkingHour = x.WorkingHours
            });
        }

        // Absent count for payroll 
        public async Task<int> GetAbsentCountAsync(int employeeId, int month, int year)
        {
            var data = await _unitOfWork.Attendances.GetMonthlyAttendanceAsync(month, year);
            return data.Count(x => x.EmployeeId == employeeId && x.Status == "Absent");
        }

        public async Task<DailyAttendancePageViewModel> GetDailyAttendancePageAsync(DateTime date, int? departmentId)
        {
            var departments = await _unitOfWork.Departments.GetActiveDepartmentsAsync();
            var employees = await _unitOfWork.Employees.GetAllAsync(x => x.Department);

            if (departmentId.HasValue)
            {
                employees = employees.Where(x => x.DepartmentId == departmentId.Value);
            }

            return new DailyAttendancePageViewModel
            {
                AttendanceDate = date,
                Departments = departments.Select(x => new SelectListItem
                {
                    Value = x.DepartmentId.ToString(),
                    Text = x.DepartmentName
                }),
                Employees = employees.Select(x => new DailyAttendanceViewModel
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.FullName,
                    DepartmentName = x.Department != null ? x.Department.DepartmentName : "-",
                    Status = "Present",
                    PhotoPath = x.PhotoPath
                }).ToList()
            };
        }

        public async Task<IEnumerable<SelectListItem>> GetDepartmentsAsync()
        {
            var departments = await _unitOfWork.Departments.GetActiveDepartmentsAsync();

            return departments.Select(x => new SelectListItem
            {
                Value = x.DepartmentId.ToString(),
                Text = x.DepartmentName
            });
        }

        public async Task SaveAttendanceAsync(SaveAttendanceViewModel model)
        {
            foreach (var item in model.AttendanceEntries)
            {
                var existing = await _unitOfWork.Attendances.FindAsync(x =>
                    x.EmployeeId == item.EmployeeId &&
                    DbFunctions.TruncateTime(x.AttendanceDate) == DbFunctions.TruncateTime(model.AttendanceDate));

                if (existing.Any())
                {
                    throw new InvalidOperationException($"Attendance for one or more employees already exists on this date.");
                }

                var employee = await _unitOfWork.Employees.GetEmployeeWithShiftAsync(item.EmployeeId);

                bool isLate = false;
                if (item.EntryTime.HasValue && employee != null && employee.Shift != null)
                {
                    var allowedTime = employee.Shift.StartTime.Add(TimeSpan.FromMinutes(employee.Shift.LateAllowanceMinutes));
                    if (item.EntryTime.Value > allowedTime)
                        isLate = true;
                }

                double? workingHours = null;
                if (item.EntryTime.HasValue && item.ExitTime.HasValue)
                {
                    workingHours = (item.ExitTime.Value - item.EntryTime.Value).TotalHours;
                }

                var attendance = new Attendance
                {
                    EmployeeId = item.EmployeeId,
                    AttendanceDate = model.AttendanceDate,
                    Status = item.Status,
                    EntryTime = item.EntryTime,
                    ExitTime = item.ExitTime,
                    IsLate = isLate,
                    WorkingHours = workingHours,
                    Remarks = item.Remarks
                };

                _unitOfWork.Attendances.Add(attendance);
            }

            await _unitOfWork.SaveAsync();
        }

        // Individual model 
        public async Task<EmployeeAttendanceHistoryVM> GetEmployeeAttendanceHistoryAsync(int employeeId)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return null;
            }

            var attendance = await _unitOfWork.Attendances.FindAsync(x => x.EmployeeId == employeeId);
            var records = attendance.ToList();

            var totalDays = records.Count;
            var presentDays = records.Count(x => x.Status == "Present");
            var absentDays = records.Count(x => x.Status == "Absent");
            var leaveDays = records.Count(x => x.Status == "Leave");
            var lateDays = records.Count(x => x.IsLate);

            var availableYears = records
                .Select(x => x.AttendanceDate.Year)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            var averageWorkingHour = records
                .Where(x => x.WorkingHours.HasValue)
                .Select(x => x.WorkingHours.Value)
                .DefaultIfEmpty(0)
                .Average();

            var attendancePercentage = totalDays == 0
                ? 0
                : ((double)presentDays / totalDays) * 100;

            return new EmployeeAttendanceHistoryVM
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                AvailableYears = availableYears,
                TotalDays = totalDays,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                LeaveDays = leaveDays,
                LateDays = lateDays,
                AverageWorkingHours = averageWorkingHour,
                AttendancePercentage = attendancePercentage,
                AttendanceRecords = records
                    .OrderByDescending(x => x.AttendanceDate)
                    .Select(x => new AttendanceRecordVM
                    {
                        AttendanceDate = x.AttendanceDate,
                        Status = x.Status,
                        EntryTime = x.EntryTime,
                        ExitTime = x.ExitTime,
                        IsLate = x.IsLate,
                        WorkingHour = x.WorkingHours,
                        Remarks = x.Remarks
                    })
                    .ToList()
            };
        }

        public async Task<EmployeeAttendanceHistoryVM> GetEmployeeAttendanceHistoryByMonthAsync(int employeeId, int month, int year)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return null;
            }

            var attendance = await _unitOfWork.Attendances.FindAsync(x =>
                x.EmployeeId == employeeId &&
                (month == 0 || x.AttendanceDate.Month == month) &&
                x.AttendanceDate.Year == year);

            var records = attendance.ToList();

            return new EmployeeAttendanceHistoryVM
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                TotalDays = records.Count(),
                PresentDays = records.Count(x => x.Status == "Present"),
                AbsentDays = records.Count(x => x.Status == "Absent"),
                LeaveDays = records.Count(x => x.Status == "Leave"),
                LateDays = records.Count(x => x.IsLate),
                AverageWorkingHours = records.Where(x => x.WorkingHours.HasValue)
                                             .Select(x => x.WorkingHours.Value)
                                             .DefaultIfEmpty(0)
                                             .Average(),
                AttendancePercentage = records.Count == 0 ? 0 : ((double)records.Count(x => x.Status == "Present") / records.Count) * 100,
                AttendanceRecords = records.OrderByDescending(x => x.AttendanceDate)
                                           .Select(x => new AttendanceRecordVM
                                           {
                                               AttendanceDate = x.AttendanceDate,
                                               Status = x.Status,
                                               EntryTime = x.EntryTime,
                                               ExitTime = x.ExitTime,
                                               IsLate = x.IsLate,
                                               WorkingHour = x.WorkingHours,
                                               Remarks = x.Remarks
                                           }).ToList()
            };
        }

        // Attendance Management 
        public async Task<AttendaneManagementVM> GetAttendanceManagementAsync(DateTime? date, int? departmentId, string status)
        {
            var attendance = await _unitOfWork.Attendances.GetAllAsync(x => x.Employee, x => x.Employee.Department);

            if (date.HasValue)
            {
                attendance = attendance.Where(x => x.AttendanceDate.Date == date.Value.Date);
            }

            if (departmentId.HasValue)
            {
                attendance = attendance.Where(x => x.Employee.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                attendance = attendance.Where(x => x.Status == status);
            }

            return new AttendaneManagementVM
            {
                AttendanceDate = date ?? DateTime.Now,
                DepartmentId = departmentId,
                Status = status,
                Departments = await GetDepartmentsAsync(),
                AttendanceRecords = attendance.OrderByDescending(x => x.AttendanceDate)
                    .Select(x => new AttendanceViewModel
                    {
                        AttendanceId = x.AttendanceId,
                        EmployeeId = x.EmployeeId,
                        EmployeeName = x.Employee.FullName,
                        DepartmentName = x.Employee.Department.DepartmentName,
                        AttendanceDate = x.AttendanceDate,
                        EntryTime = x.EntryTime,
                        ExitTime = x.ExitTime,
                        Status = x.Status,
                        IsLate = x.IsLate,
                        WorkingHours = x.WorkingHours,
                        Remarks = x.Remarks
                    }).ToList()
            };
        }

        // Report method
        public async Task<AttendanceSummaryReportVM> GetAttendanceSummaryReportAsync(int month, int year, int? departmentId)
        {
            var attendance = await _unitOfWork.Attendances.GetAllAsync(
                x => x.Employee,
                x => x.Employee.Department
            );

            attendance = attendance.Where(x =>
                x.AttendanceDate.Month == month &&
                x.AttendanceDate.Year == year);

            if (departmentId.HasValue)
            {
                attendance = attendance.Where(x => x.Employee.DepartmentId == departmentId.Value);
            }

            var records = attendance.ToList();


            //For showing the chart in chart.js part 

            var totalDays = records.Count;
            var present = records.Count(x => x.Status == "Present");
            var absent = records.Count(x => x.Status == "Absent");
            var leave = records.Count(x => x.Status == "Leave");


            return new AttendanceSummaryReportVM
            {
                Month = month,
                Year = year,
                GeneratedDate = DateTime.Now,
                DepartmentName = departmentId.HasValue ? records.FirstOrDefault()?.Employee?.Department?.DepartmentName : "All Departments",
                TotalAttendance = records.Count,
                PresentCount = records.Count(x => x.Status == "Present"),
                AbsentCount = records.Count(x => x.Status == "Absent"),
                LeaveCount = records.Count(x => x.Status == "Leave"),
                LateCount = records.Count(x => x.IsLate),
                TotalEmployees = records.Select(x => x.EmployeeId).Distinct().Count(),
                // this part is the char int the view 
                PresentPercentage = totalDays == 0 ? 0 : ((double)present / totalDays) * 100,
                AbsentPercentage = totalDays == 0 ? 0 : ((double)absent / totalDays) * 100,
                LeavePercentage = totalDays == 0 ? 0 : ((double)leave / totalDays) * 100,

                DepartmentReports = records.GroupBy(x => x.Employee.Department.DepartmentName).Select(x => new DepartmentAttendanceReportVM
                {
                    DepartmentName = x.Key,
                    TotalAttendance = x.Count(),
                    PresentDays = x.Count(a => a.Status == "Present"),
                    AttendancePercentage = x.Count() == 0 ? 0 : ((double)x.Count(a => a.Status == "Present") / x.Count()) * 100
                }).ToList(),

                EmployeeReports = records
                    .GroupBy(x => x.EmployeeId)
                    .Select(x => new EmployeeAttendanceReportVM
                    {
                        EmployeeId = x.Key,
                        EmployeeName = x.First().Employee.FullName,
                        DepartmentName = x.First().Employee.Department.DepartmentName,
                        PresentDays = x.Count(a => a.Status == "Present"),
                        AbsentDays = x.Count(a => a.Status == "Absent"),
                        LeaveDays = x.Count(a => a.Status == "Leave"),
                        LateDays = x.Count(a => a.IsLate),
                        AverageWorkingHours = x.Where(a => a.WorkingHours.HasValue)
                                               .Select(a => a.WorkingHours.Value)
                                               .DefaultIfEmpty(0)
                                               .Average(),
                        AttendancePercentage = x.Count() == 0
                            ? 0
                            : ((double)x.Count(a => a.Status == "Present") / x.Count()) * 100
                    })
                    .ToList()
            };
        }

        // Attendance Management filtered by Month/Year
        public async Task<AttendaneManagementVM> GetAttendanceManagementByMonthAsync(int month, int year, int? departmentId, string status)
        {
            var attendance = await _unitOfWork.Attendances.GetAllAsync(x => x.Employee, x => x.Employee.Department);

            attendance = attendance.Where(x => x.AttendanceDate.Month == month && x.AttendanceDate.Year == year);

            if (departmentId.HasValue)
            {
                attendance = attendance.Where(x => x.Employee.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                attendance = attendance.Where(x => x.Status == status);
            }

            return new AttendaneManagementVM
            {
                AttendanceDate = new DateTime(year, month, 1),
                DepartmentId = departmentId,
                Status = status,
                Departments = await GetDepartmentsAsync(),
                AttendanceRecords = attendance.OrderByDescending(x => x.AttendanceDate)
                    .ThenBy(x => x.Employee.FullName)
                    .Select(x => new AttendanceViewModel
                    {
                        AttendanceId = x.AttendanceId,
                        EmployeeId = x.EmployeeId,
                        EmployeeName = x.Employee.FullName,
                        DepartmentName = x.Employee.Department.DepartmentName,
                        AttendanceDate = x.AttendanceDate,
                        EntryTime = x.EntryTime,
                        ExitTime = x.ExitTime,
                        Status = x.Status,
                        IsLate = x.IsLate,
                        WorkingHours = x.WorkingHours,
                        Remarks = x.Remarks,
                        PhotoPath = x.Employee.PhotoPath
                    }).ToList()
            };
        }
    }
}