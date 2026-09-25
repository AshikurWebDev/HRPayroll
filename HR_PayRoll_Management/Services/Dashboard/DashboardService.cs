using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Services.Dashboard;
using HR_PayRoll_Management.ViewModels.Dashboard;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardVM> GetDashboardAsync()
        {
            var model = new DashboardVM();
            bool isManagerOnly = false;
            int? managerDeptId = null;

            var httpContext = System.Web.HttpContext.Current;
            if (httpContext != null && httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                if (httpContext.User.IsInRole("Manager") && !httpContext.User.IsInRole("Admin") && !httpContext.User.IsInRole("HR Manager") && !httpContext.User.IsInRole("HR Officer"))
                {
                    isManagerOnly = true;
                    using (var db = new HR_PayRoll_Management.DAL.AppDbContext())
                    {
                        var u = db.Users.Include("Employee").FirstOrDefault(x => x.Username == httpContext.User.Identity.Name);
                        if (u != null && u.Employee != null)
                        {
                            managerDeptId = u.Employee.DepartmentId;
                        }
                    }
                }
            }

            // EMPLOYEE INFORMATION
            var employees = await _unitOfWork.Employees.GetAllAsync();
            var employeeList = employees.ToList();
            if (isManagerOnly && managerDeptId.HasValue) {
                employeeList = employeeList.Where(e => e.DepartmentId == managerDeptId.Value).ToList();
            }

            model.TotalEmployees = employeeList.Count;
            model.ActiveEmployees = employeeList.Count(x => x.IsActive);

            // DEPARTMENT INFORMATION
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var departmentList = departments.ToList();
            model.TotalDepartments = isManagerOnly ? 1 : departmentList.Count;

            var designations = await _unitOfWork.Designations.GetAllAsync();
            model.TotalDesignations = designations.Count();

            // PENDING LEAVES
            var leaves = await _unitOfWork.Leaves.GetAllAsync();
            var leaveList = leaves.ToList();
            if (isManagerOnly && managerDeptId.HasValue) {
                // Fetch employee IDs in this department to filter leaves
                var empIds = employeeList.Select(e => e.EmployeeId).ToList();
                leaveList = leaveList.Where(l => empIds.Contains(l.EmployeeId)).ToList();
            }
            model.PendingLeaves = leaveList.Count(x => x.Status == "Pending");

            // PAYROLL INFORMATION (Hide for managers)
            var payrollCycles = await _unitOfWork.PayrollCycles.GetAllAsync();
            var payrollList = payrollCycles.ToList();
            
            if (!isManagerOnly) 
            {
                var latestPayroll = payrollList.OrderByDescending(x => x.CycleYear).ThenByDescending(x => x.CycleMonth).FirstOrDefault();
                if (latestPayroll != null) model.MonthlyPayroll = latestPayroll.TotalNetAmount;

                var payrollChartData = payrollList.OrderByDescending(x => x.CycleYear).ThenByDescending(x => x.CycleMonth).Take(6).OrderBy(x => x.CycleYear).ThenBy(x => x.CycleMonth).ToList();
                foreach (var payroll in payrollChartData)
                {
                    string label = $"{payroll.CycleMonth}/{payroll.CycleYear}";
                    model.PayrollMonths.Add(label);
                    model.PayrollAmounts.Add(payroll.TotalNetAmount);
                    model.PayrollLabels.Add(label);
                    model.PayrollGross.Add(payroll.TotalGrossAmount);
                    model.PayrollDeduction.Add(payroll.TotalDeductions);
                    model.PayrollNet.Add(payroll.TotalNetAmount);
                }
            }

            // EMPTY DB PROTECTION
            if (!model.PayrollMonths.Any())
            {
                model.PayrollMonths.Add("No Data");
                model.PayrollAmounts.Add(0);
                model.PayrollLabels.Add("No Data");
                model.PayrollGross.Add(0);
                model.PayrollDeduction.Add(0);
                model.PayrollNet.Add(0);
            }

            // DEPARTMENT CHART
            if (isManagerOnly && managerDeptId.HasValue) {
                var d = departmentList.FirstOrDefault(x => x.DepartmentId == managerDeptId.Value);
                if (d != null) {
                    model.DepartmentNames.Add(d.DepartmentName);
                    model.DepartmentEmployeeCount.Add(employeeList.Count);
                }
            }
            else {
                foreach (var department in departmentList)
                {
                    model.DepartmentNames.Add(department.DepartmentName);
                    model.DepartmentEmployeeCount.Add(employeeList.Count(x => x.DepartmentId == department.DepartmentId));
                }
            }

            if (!model.DepartmentNames.Any())
            {
                model.DepartmentNames.Add("No Data");
                model.DepartmentEmployeeCount.Add(0);
            }

            // ATTENDANCE INFORMATION
            var today = DateTime.Today;
            var attendance = await _unitOfWork.Attendances.FindAsync(x => x.AttendanceDate == today);
            var attendanceList = attendance.ToList();

            if (isManagerOnly && managerDeptId.HasValue) {
                var empIds = employeeList.Select(e => e.EmployeeId).ToList();
                attendanceList = attendanceList.Where(a => empIds.Contains(a.EmployeeId)).ToList();
            }

            int present = 0;
            int absent = 0;

            if (attendanceList.Any())
            {
                present = attendanceList.Count(x => x.Status == "Present");
                absent = attendanceList.Count(x => x.Status == "Absent");
                model.AttendancePercentage = ((double)present / attendanceList.Count) * 100;
            }

            model.PresentCount = present;
            model.AbsentCount = absent;

            // EMPLOYEE GROWTH
            var growth = employeeList.Where(x => x.JoiningDate.Year > 2000).GroupBy(x => x.JoiningDate.Year).OrderBy(x => x.Key).ToList();
            foreach (var item in growth)
            {
                model.EmployeeGrowthLabels.Add(item.Key.ToString());
                model.EmployeeGrowthCount.Add(item.Count());
            }

            if (!model.EmployeeGrowthLabels.Any())
            {
                model.EmployeeGrowthLabels.Add(DateTime.Now.Year.ToString());
                model.EmployeeGrowthCount.Add(0);
            }

            // RECENT PAYROLL TABLE (Hide for managers)
            if (!isManagerOnly)
            {
                var salarySlips = await _unitOfWork.SalarySlips.GetAllAsync(x => x.Employee, x => x.Employee.Department, x => x.PayrollCycle);
                var recentSlips = salarySlips.OrderByDescending(x => x.PayrollCycle.CycleYear).ThenByDescending(x => x.PayrollCycle.CycleMonth).Take(10).ToList();
                foreach (var slip in recentSlips)
                {
                    model.RecentPayrolls.Add(new DashboardPayrollVM
                    {
                        SalarySlipId = slip.SalarySlipId,
                        EmployeeName = slip.Employee.FullName,
                        PhotoPath = slip.Employee.PhotoPath,
                        DepartmentName = slip.Employee.Department != null ? slip.Employee.Department.DepartmentName : "-",
                        GrossSalary = slip.GrossSalary,
                        TotalDeduction = slip.TotalDeduction,
                        NetSalary = slip.NetSalary,
                        Status = slip.PayrollCycle.Status
                    });
                }
            }

            return model;
        }
    }
}



