using HR_PayRoll_Management.Services.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }
        private bool CanAccessPayslip(int targetEmployeeId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("HR Manager") || User.IsInRole("HR Officer")) return true;
            using (var db = new HR_PayRoll_Management.DAL.AppDbContext())
            {
                var user = System.Linq.Queryable.FirstOrDefault(db.Users, u => u.Username == User.Identity.Name);
                if (user != null && user.EmployeeId == targetEmployeeId) return true;
            }
            return false;
        }

        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Index()
        {
            var data = await _payrollService.GetPayrollHistoryAsync();
            return View(data);
        }

        [HttpGet]
        public ActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Generate(int month, int year)
        {
            await _payrollService.GeneratePayrollAsync(month, year);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Payslip(int id)
        {
            var payslip = await _payrollService.GetPayslipAsync(id);
            if (payslip == null) return HttpNotFound();
            if (!CanAccessPayslip(payslip.EmployeeId)) return new HttpUnauthorizedResult("You can only view your own payslips.");


            return View(payslip);
        }

        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Details(int id)
        {
            var slips =
                await _payrollService
                .GetSalarySlipsByCycleAsync(id);

            ViewBag.PayrollCycleId = id;

            return View(slips);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Approve(int id)
        {
            await _payrollService.ApprovePayrollAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Pay(int id)
        {
            await _payrollService.PayPayrollAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Delete(int id)
        {
            var cycle = await _payrollService.GetPayrollCycleByIdAsync(id);
            if (cycle != null)
            {
                if (cycle.Status == "Paid") { TempData["Error"] = "This payroll cycle is already paid and finalized. No one can delete it.";
                    return RedirectToAction("Index");
                }
                await _payrollService.DeletePayrollAsync(id);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ExportPayslipPdf(int id)
        {
            var payslip = await _payrollService.GetPayslipAsync(id);
            if (payslip == null) return HttpNotFound();
            if (!CanAccessPayslip(payslip.EmployeeId)) return new HttpUnauthorizedResult("You can only export your own payslips.");

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var netFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);

                var title = new Paragraph("HR PAYROLL SYSTEM", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);

                var subtitle = new Paragraph("Salary Slip", headingFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(subtitle);

                var monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(payslip.PayrollMonth);
                var period = new Paragraph($"Period: {monthName} {payslip.PayrollYear}", normalFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(period);

                document.Add(new Paragraph("\n"));

                // Employee Information
                var employeeTable = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };

                AddPdfCell(employeeTable, "Employee Name", boldFont);
                AddPdfCell(employeeTable, payslip.EmployeeName, normalFont);
                AddPdfCell(employeeTable, "Employee ID", boldFont);
                AddPdfCell(employeeTable, payslip.EmployeeId.ToString(), normalFont);
                AddPdfCell(employeeTable, "Department", boldFont);
                AddPdfCell(employeeTable, payslip.DepartmentName, normalFont);
                AddPdfCell(employeeTable, "Designation", boldFont);
                AddPdfCell(employeeTable, payslip.DesignationName, normalFont);
                AddPdfCell(employeeTable, "Generated Date", boldFont);
                AddPdfCell(employeeTable, payslip.GeneratedDate.ToString("dd MMM yyyy"), normalFont);

                document.Add(employeeTable);
                document.Add(new Paragraph("\n"));

                // Earnings and Deductions side-by-side
                var mainTable = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };

                var earningTable = CreateSalaryTable("Earnings");
                AddSalaryRow(earningTable, "Basic Salary", payslip.BasicSalary);
                AddSalaryRow(earningTable, "House Rent", payslip.HouseRent);
                AddSalaryRow(earningTable, "Medical Allowance", payslip.MedicalAllowance);
                AddSalaryRow(earningTable, "Transport Allowance", payslip.TransportAllowance);
                AddSalaryRow(earningTable, "Bonus", payslip.BonusAmount);
                AddSalaryRow(earningTable, "Overtime", payslip.OvertimeAmount);
                AddSalaryRow(earningTable, "Gross Salary", payslip.GrossSalary);

                var deductionTable = CreateSalaryTable("Deductions");
                AddSalaryRow(deductionTable, "Absent Deduction", payslip.AbsentDeduction);
                AddSalaryRow(deductionTable, "Leave Deduction", payslip.LeaveDeduction);
                AddSalaryRow(deductionTable, "Tax Deduction", payslip.TaxDeduction);
                AddSalaryRow(deductionTable, "Total Deduction", payslip.TotalDeduction);

                var earningCell = new PdfPCell(earningTable)
                {
                    Border = Rectangle.NO_BORDER,
                    PaddingRight = 10
                };

                var deductionCell = new PdfPCell(deductionTable)
                {
                    Border = Rectangle.NO_BORDER,
                    PaddingLeft = 10
                };

                mainTable.AddCell(earningCell);
                mainTable.AddCell(deductionCell);

                document.Add(mainTable);
                document.Add(new Paragraph("\n"));

                // Net Salary Box
                var netTable = new PdfPTable(1)
                {
                    WidthPercentage = 100
                };

                var netCell = new PdfPCell(new Phrase($"Net Salary\n{payslip.NetSalary:N2}", netFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 15,
                    BorderWidth = 2,
                    BackgroundColor = new BaseColor(248, 249, 250)
                };

                netTable.AddCell(netCell);
                document.Add(netTable);

                document.Add(new Paragraph("\n\n"));

                // Signatures
                var signatureTable = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };

                AddSignatureCell(signatureTable, "Prepared By");
                AddSignatureCell(signatureTable, "Employee Signature");

                document.Add(signatureTable);

                document.Close();

                return File(
                    stream.ToArray(),
                    "application/pdf",
                    $"Payslip_{payslip.EmployeeName}_{payslip.PayrollMonth}_{payslip.PayrollYear}.pdf"
                );
            }
        }

        public async Task<ActionResult> ExportPayslipExcel(int id)
        {
            var payslip = await _payrollService.GetPayslipAsync(id);
            if (payslip == null) return HttpNotFound();
            if (!CanAccessPayslip(payslip.EmployeeId)) return new HttpUnauthorizedResult("You can only export your own payslips.");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Payslip");
                
                worksheet.Cells[1, 1].Value = "HR PAYROLL SYSTEM";
                worksheet.Cells[2, 1].Value = "Salary Slip";
                worksheet.Cells[3, 1].Value = $"Period: {System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(payslip.PayrollMonth)} {payslip.PayrollYear}";
                
                worksheet.Cells[5, 1].Value = "Employee Name:";
                worksheet.Cells[5, 2].Value = payslip.EmployeeName;
                worksheet.Cells[6, 1].Value = "Employee ID:";
                worksheet.Cells[6, 2].Value = payslip.EmployeeId;
                worksheet.Cells[7, 1].Value = "Department:";
                worksheet.Cells[7, 2].Value = payslip.DepartmentName;
                worksheet.Cells[8, 1].Value = "Designation:";
                worksheet.Cells[8, 2].Value = payslip.DesignationName;
                
                worksheet.Cells[10, 1].Value = "EARNINGS";
                worksheet.Cells[11, 1].Value = "Basic Salary";
                worksheet.Cells[11, 2].Value = payslip.BasicSalary;
                worksheet.Cells[12, 1].Value = "House Rent";
                worksheet.Cells[12, 2].Value = payslip.HouseRent;
                worksheet.Cells[13, 1].Value = "Medical Allowance";
                worksheet.Cells[13, 2].Value = payslip.MedicalAllowance;
                worksheet.Cells[14, 1].Value = "Transport Allowance";
                worksheet.Cells[14, 2].Value = payslip.TransportAllowance;
                worksheet.Cells[15, 1].Value = "Bonus";
                worksheet.Cells[15, 2].Value = payslip.BonusAmount;
                worksheet.Cells[16, 1].Value = "Overtime";
                worksheet.Cells[16, 2].Value = payslip.OvertimeAmount;
                worksheet.Cells[17, 1].Value = "Gross Salary";
                worksheet.Cells[17, 2].Value = payslip.GrossSalary;
                
                worksheet.Cells[19, 1].Value = "DEDUCTIONS";
                worksheet.Cells[20, 1].Value = "Absent Deduction";
                worksheet.Cells[20, 2].Value = payslip.AbsentDeduction;
                worksheet.Cells[21, 1].Value = "Leave Deduction";
                worksheet.Cells[21, 2].Value = payslip.LeaveDeduction;
                worksheet.Cells[22, 1].Value = "Tax Deduction";
                worksheet.Cells[22, 2].Value = payslip.TaxDeduction;
                worksheet.Cells[23, 1].Value = "Total Deduction";
                worksheet.Cells[23, 2].Value = payslip.TotalDeduction;
                
                worksheet.Cells[25, 1].Value = "NET SALARY";
                worksheet.Cells[25, 2].Value = payslip.NetSalary;
                
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Payslip_{payslip.EmployeeName}_{payslip.PayrollMonth}_{payslip.PayrollYear}.xlsx");
            }
        }

        private PdfPTable CreateSalaryTable(string title)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            AddPdfHeader(table, title);
            AddPdfHeader(table, "Amount");

            return table;
        }

        private void AddPdfCell(PdfPTable table, string text, Font font)
        {
            var cell = new PdfPCell(new Phrase(text ?? string.Empty, font));
            table.AddCell(cell);
        }

        private void AddPdfHeader(PdfPTable table, string text)
        {
            var cell = new PdfPCell(new Phrase(text ?? string.Empty))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Padding = 5
            };

            table.AddCell(cell);
        }

        private void AddSalaryRow(PdfPTable table, string name, decimal amount)
        {
            table.AddCell(name);

            var amountCell = new PdfPCell(new Phrase(amount.ToString("N2")))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT
            };

            table.AddCell(amountCell);
        }

        private void AddSignatureCell(PdfPTable table, string text)
        {
            var cell = new PdfPCell(new Phrase($"____________________\n\n{text}"))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = Rectangle.NO_BORDER
            };

            table.AddCell(cell);
        }
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> ExportCycleCsv(int id)
        {
            var slips = await _payrollService.GetSalarySlipsByCycleAsync(id);
            var cycle = await _payrollService.GetPayrollCycleByIdAsync(id);
            if(cycle == null) return HttpNotFound();

            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendLine("Employee ID,Employee Name,Department,Gross Salary,Total Deduction,Net Salary");

            foreach (var slip in slips)
            {
                stringBuilder.AppendLine($"{slip.EmployeeId},{slip.EmployeeName},{slip.DepartmentName},{slip.GrossSalary},{slip.TotalDeduction},{slip.NetSalary}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", $"Payroll_Cycle_{cycle.CycleMonth}_{cycle.CycleYear}.csv");
        }

        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> ExportCycleExcel(int id)
        {
            var slips = await _payrollService.GetSalarySlipsByCycleAsync(id);
            var cycle = await _payrollService.GetPayrollCycleByIdAsync(id);
            if(cycle == null) return HttpNotFound();

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Payroll");
                
                // Headers
                worksheet.Cells[1, 1].Value = "Employee ID";
                worksheet.Cells[1, 2].Value = "Employee Name";
                worksheet.Cells[1, 3].Value = "Department";
                worksheet.Cells[1, 4].Value = "Gross Salary";
                worksheet.Cells[1, 5].Value = "Total Deduction";
                worksheet.Cells[1, 6].Value = "Net Salary";

                int row = 2;
                foreach (var slip in slips)
                {
                    worksheet.Cells[row, 1].Value = slip.EmployeeId;
                    worksheet.Cells[row, 2].Value = slip.EmployeeName;
                    worksheet.Cells[row, 3].Value = slip.DepartmentName;
                    worksheet.Cells[row, 4].Value = slip.GrossSalary;
                    worksheet.Cells[row, 5].Value = slip.TotalDeduction;
                    worksheet.Cells[row, 6].Value = slip.NetSalary;
                    row++;
                }

                // AutoFitColumns requires EPPlus.System.Drawing which may be missing
                // worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Payroll_Cycle_{cycle.CycleMonth}_{cycle.CycleYear}.xlsx");
            }
        }
    }
}




