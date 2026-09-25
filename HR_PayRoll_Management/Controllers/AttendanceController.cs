using HR_PayRoll_Management.Helpers;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.Services.Service_Attandance;
using HR_PayRoll_Management.ViewModels.AttendanceVM;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin,HR Manager,HR Officer,Manager")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;

        public AttendanceController(
            IAttendanceService attendanceService,
            IEmployeeService employeeService)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
        }
        private int? GetManagerDepartmentId()
        {
            if (User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("HR Manager") && !User.IsInRole("HR Officer"))
            {
                using (var db = new DAL.AppDbContext())
                {
                    var user = System.Linq.Queryable.FirstOrDefault(db.Users.Include("Employee"), u => u.Username == User.Identity.Name);
                    if (user != null && user.Employee != null)
                    {
                        return user.Employee.DepartmentId;
                    }
                }
            }
            return null;
        }

        // ============================================
        // DAILY ATTENDANCE PAGE
        // URL: /Attendance
        // ============================================
        public async Task<ActionResult> Index(int? departmentId)
        {
            int? mgrDept = GetManagerDepartmentId();
            if (mgrDept.HasValue) departmentId = mgrDept;
            var model = await _attendanceService.GetDailyAttendancePageAsync(DateTime.Now, departmentId);
            return View(model);
        }

        // ============================================
        // CREATE SINGLE ATTENDANCE
        // ============================================
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new AttendanceCreateViewModel
            {
                AttendanceDate = DateTime.Now
            };

            ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AttendanceCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
                return View(model);
            }

            await _attendanceService.CreateAsync(model);
            return RedirectToAction("Index");
        }

        // ============================================
        // EMPLOYEE ATTENDANCE HISTORY
        // URL: /Attendance/EmployeeHistory?id=1
        // ============================================
        public async Task<ActionResult> EmployeeHistory(int id)
        {
            var data = await _attendanceService.GetEmployeeAttendanceAsync(id);
            return View(data);
        }

        // ============================================
        // EDIT ATTENDANCE
        // ============================================
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var attendance = await _attendanceService.GetByIdAsync(id);
            if (attendance == null)
            {
                return HttpNotFound();
            }

            var model = new AttendanceEditViewModel
            {
                AttendanceId = attendance.AttendanceId,
                EmployeeId = attendance.EmployeeId,
                AttendanceDate = attendance.AttendanceDate,
                EntryTime = attendance.EntryTime,
                ExitTime = attendance.ExitTime,
                Status = attendance.Status,
                Remarks = attendance.Remarks
            };

            ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AttendanceEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
                return View(model);
            }

            await _attendanceService.UpdateAsync(model);
            return RedirectToAction("Management");
        }

        // ============================================
        // DELETE
        // ============================================
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var attendance = await _attendanceService.GetByIdAsync(id);
            if (attendance == null)
            {
                return HttpNotFound();
            }

            return View(attendance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            await _attendanceService.DeleteAsync(id);
            return RedirectToAction("Management");
        }

        // ============================================
        // MONTHLY REPORT
        // ============================================
        public async Task<ActionResult> MonthlyReport(int month = 0, int year = 0)
        {
            if (month == 0)
                month = DateTime.Now.Month;

            if (year == 0)
                year = DateTime.Now.Year;

            var report = await _attendanceService.GetMonthlyAttendanceAsync(month, year);

            ViewBag.Month = month;
            ViewBag.Year = year;

            return View(report);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartments()
        {
            var departments = await _attendanceService.GetDepartmentsAsync();
            var result = departments.Select(x => new
            {
                id = x.Value,
                name = x.Text
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployeesByDepartment(int? departmentId)
        {
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
            var result = employees.Select(x => new DailyAttendanceViewModel
            {
                EmployeeId = x.EmployeeId,
                EmployeeName = x.FullName,
                DepartmentName = x.DepartmentName,
                PhotoPath = x.PhotoPath
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveAttendance(SaveAttendanceViewModel model)
        {
            try
            {
                await _attendanceService.SaveAttendanceAsync(model);
                TempData["SuccessMessage"] = "Attendance Saved Successfully";
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Management", "Attendance")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployeeAttendanceByMonth(int employeeId, int month, int year)
        {
            var data = await _attendanceService.GetEmployeeAttendanceHistoryByMonthAsync(employeeId, month, year);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Management(int? month, int? year, int? departmentId, string status)
        {
            int? mgrDept = GetManagerDepartmentId();
            if (mgrDept.HasValue) departmentId = mgrDept;
            int m = month ?? DateTime.Now.Month;
            int y = year ?? DateTime.Now.Year;

            var model = await _attendanceService.GetAttendanceManagementByMonthAsync(m, y, departmentId, status);
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> FilterManagement(int month, int year, int? departmentId, string status)
        {
            int? mgrDept = GetManagerDepartmentId();
            if (mgrDept.HasValue) departmentId = mgrDept;
            var model = await _attendanceService.GetAttendanceManagementByMonthAsync(month, year, departmentId, status);

            // Render the partial view to a string
            string html = RenderPartialViewToString("_AttendanceManagementTable", model);

            var records = model.AttendanceRecords?.ToList() ?? new System.Collections.Generic.List<HR_PayRoll_Management.ViewModels.AttendanceVM.AttendanceViewModel>();

            return Json(new
            {
                html = html,
                total = records.Count,
                present = records.Count(x => x.Status == "Present"),
                absent = records.Count(x => x.Status == "Absent"),
                leave = records.Count(x => x.Status == "Leave"),
                late = records.Count(x => x.IsLate)
            }, JsonRequestBehavior.AllowGet);
        }

        // Helper to render a partial view to string for AJAX responses
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Report(int month = 0, int year = 0, int? departmentId = null)
        {
            int? mgrDept = GetManagerDepartmentId();
            if (mgrDept.HasValue) departmentId = mgrDept;
            if (month == 0)
                month = DateTime.Now.Month;

            if (year == 0)
                year = DateTime.Now.Year;

            var report = await _attendanceService.GetAttendanceSummaryReportAsync(month, year, departmentId);

            ViewBag.Departments = await _attendanceService.GetDepartmentsAsync();
            ViewBag.Month = month;
            ViewBag.Years = Enumerable.Range(DateTime.Now.Year - 5, 6).ToList();

            return View(report);
        }

        [HttpGet]
        public async Task<PartialViewResult> LoadAttendanceReport(int month, int year, int? departmentId)
        {
            var report = await _attendanceService.GetAttendanceSummaryReportAsync(month, year, departmentId);
            return PartialView("_AttendanceReportResult", report);
        }

        public async Task<ActionResult> ExportExcel(int month, int year, int? departmentId)
        {
            int? mgrDept = GetManagerDepartmentId();
            if (mgrDept.HasValue) departmentId = mgrDept;
            ExcelPackage.License.SetNonCommercialPersonal("HR Payroll System");

            var report = await _attendanceService.GetAttendanceSummaryReportAsync(month, year, departmentId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Attendance Report");

                worksheet.Cells[1, 1].Value = "Employee";
                worksheet.Cells[1, 2].Value = "Department";
                worksheet.Cells[1, 3].Value = "Present Days";
                worksheet.Cells[1, 4].Value = "Absent Days";
                worksheet.Cells[1, 5].Value = "Leave Days";
                worksheet.Cells[1, 6].Value = "Late Days";
                worksheet.Cells[1, 7].Value = "Attendance Percentage";

                int row = 2;
                foreach (var item in report.EmployeeReports)
                {
                    worksheet.Cells[row, 1].Value = item.EmployeeName;
                    worksheet.Cells[row, 2].Value = item.DepartmentName;
                    worksheet.Cells[row, 3].Value = item.PresentDays;
                    worksheet.Cells[row, 4].Value = item.AbsentDays;
                    worksheet.Cells[row, 5].Value = item.LeaveDays;
                    worksheet.Cells[row, 6].Value = item.LateDays;
                    worksheet.Cells[row, 7].Value = Math.Round(item.AttendancePercentage, 0) + "%";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var fileBytes = package.GetAsByteArray();

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Attendance_Report_{month}_{year}.xlsx"
                );
            }
        }

        [HttpGet]
        public async Task<ActionResult> ExportPdf(int month, int year, int? departmentId)
        {
            int? mgrDept = GetManagerDepartmentId();
            if (mgrDept.HasValue) departmentId = mgrDept;
            var report = await _attendanceService.GetAttendanceSummaryReportAsync(month, year, departmentId);

            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                writer.PageEvent = new PdfPageEvent();

                document.Open();

                // ==========================
                // REPORT HEADER
                // ==========================
                Paragraph company = new Paragraph("HR PAYROLL SYSTEM", PdfHelper.TitleFont())
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(company);

                Paragraph title = new Paragraph("Monthly Attendance Report", PdfHelper.HeadingFont())
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // ==========================
                // REPORT INFORMATION
                // ==========================
                string monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(report.Month);
                string department = string.IsNullOrEmpty(report.DepartmentName) ? "All Departments" : report.DepartmentName;

                document.Add(new Paragraph($"Period: {monthName} {report.Year}"));
                document.Add(new Paragraph($"Department: {department}"));
                document.Add(new Paragraph($"Generated: {report.GeneratedDate:dd MMM yyyy}"));
                document.Add(new Paragraph("\n"));

                // ==========================
                // SUMMARY TABLE
                // ==========================
                PdfPTable summaryTable = new PdfPTable(2) { WidthPercentage = 100 };

                PdfHelper.AddHeaderCell(summaryTable, "Summary");
                PdfHelper.AddHeaderCell(summaryTable, "Value");

                PdfHelper.AddCell(summaryTable, "Total Employees");
                PdfHelper.AddCell(summaryTable, report.TotalEmployees.ToString());

                PdfHelper.AddCell(summaryTable, "Total Attendance");
                PdfHelper.AddCell(summaryTable, report.TotalAttendance.ToString());

                PdfHelper.AddCell(summaryTable, "Present");
                PdfHelper.AddCell(summaryTable, report.PresentCount.ToString());

                PdfHelper.AddCell(summaryTable, "Absent");
                PdfHelper.AddCell(summaryTable, report.AbsentCount.ToString());

                PdfHelper.AddCell(summaryTable, "Leave");
                PdfHelper.AddCell(summaryTable, report.LeaveCount.ToString());

                PdfHelper.AddCell(summaryTable, "Late");
                PdfHelper.AddCell(summaryTable, report.LateCount.ToString());

                document.Add(summaryTable);
                document.Add(new Paragraph("\n"));

                // ==========================
                // CHARTS / ANALYTICS
                // ==========================
                document.Add(new Paragraph("Attendance Analytics", PdfHelper.HeadingFont()));

                byte[] statusChart = PdfChartHelper.CreateAttendanceStatusChart(
                    report.PresentPercentage,
                    report.AbsentPercentage,
                    report.LeavePercentage);

                Image statusImage = Image.GetInstance(statusChart);
                statusImage.ScaleToFit(350, 200);
                document.Add(statusImage);
                document.Add(new Paragraph("\n"));

                byte[] departmentChart = PdfChartHelper.CreateDepartmentChart(report.DepartmentReports);
                Image departmentImage = Image.GetInstance(departmentChart);
                departmentImage.ScaleToFit(400, 250);
                document.Add(departmentImage);
                document.Add(new Paragraph("\n"));

                // ==========================
                // EMPLOYEE REPORT TABLE
                // ==========================
                PdfPTable table = new PdfPTable(7) { WidthPercentage = 100 };

                string[] headers =
                {
                    "Employee",
                    "Department",
                    "Present",
                    "Absent",
                    "Leave",
                    "Late",
                    "Attendance %"
                };

                foreach (var header in headers)
                {
                    PdfHelper.AddHeaderCell(table, header);
                }

                foreach (var item in report.EmployeeReports)
                {
                    PdfHelper.AddCell(table, item.EmployeeName);
                    PdfHelper.AddCell(table, item.DepartmentName);
                    PdfHelper.AddCell(table, item.PresentDays.ToString());
                    PdfHelper.AddCell(table, item.AbsentDays.ToString());
                    PdfHelper.AddCell(table, item.LeaveDays.ToString());
                    PdfHelper.AddCell(table, item.LateDays.ToString());
                    PdfHelper.AddCell(table, Math.Round(item.AttendancePercentage, 0) + "%");
                }

                document.Add(table);
                document.Close();

                return File(
                    stream.ToArray(),
                    "application/pdf",
                    $"Attendance_Report_{month}_{year}.pdf"
                );
            }
        }
    }
}



