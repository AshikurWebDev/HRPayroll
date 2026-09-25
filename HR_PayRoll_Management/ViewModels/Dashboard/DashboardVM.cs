using System.Collections.Generic;

namespace HR_PayRoll_Management.ViewModels.Dashboard
{
    public class DashboardVM
    {
        public bool IsManagerView { get; set; }
        // ============================
        // KPI CARDS
        // ============================

        public int TotalEmployees { get; set; }

        public int ActiveEmployees { get; set; }

        public int TotalDepartments { get; set; }

        public int TotalDesignations { get; set; }


        public decimal MonthlyPayroll { get; set; }

        public double AttendancePercentage { get; set; }



        // ============================
        // PENDING INFORMATION
        // ============================

        public int PendingAttendance { get; set; }

        public int PendingLeaves { get; set; }

        public int PendingPayroll { get; set; }



        // ============================
        // PAYROLL CHART
        // ============================

        public List<string> PayrollMonths { get; set; }

        public List<decimal> PayrollAmounts { get; set; }
        public List<string> PayrollLabels { get; set; }

        public List<decimal> PayrollGross { get; set; }

        public List<decimal> PayrollDeduction { get; set; }

        public List<decimal> PayrollNet { get; set; }



        // ============================
        // DEPARTMENT CHART
        // ============================

        public List<string> DepartmentNames { get; set; }

        public List<int> DepartmentEmployeeCount { get; set; }




        // ============================
        // ATTENDANCE CHART
        // ============================

        public int PresentCount { get; set; }

        public int AbsentCount { get; set; }




        // ============================
        // EMPLOYEE GROWTH CHART
        // ============================

        public List<string> EmployeeGrowthLabels { get; set; }

        public List<int> EmployeeGrowthCount { get; set; }

        // Payroll Overview Chart

        public List<string> PayrollOverviewLabels { get; set; }

        public List<decimal> PayrollGrossAmounts { get; set; }

        public List<decimal> PayrollDeductionAmounts { get; set; }

        public List<decimal> PayrollNetAmounts { get; set; }

        public List<DashboardPayrollVM> RecentPayrolls { get; set; }
        public DashboardVM()
        {

            PayrollMonths =
                new List<string>();


            PayrollAmounts =
                new List<decimal>();



            DepartmentNames =
                new List<string>();


            DepartmentEmployeeCount =
                new List<int>();



            EmployeeGrowthLabels =
                new List<string>();


            EmployeeGrowthCount =
                new List<int>();
            PayrollLabels = new List<string>();

            PayrollGross = new List<decimal>();

            PayrollDeduction = new List<decimal>();

            PayrollNet = new List<decimal>();

            PayrollOverviewLabels =
    new List<string>();


            PayrollGrossAmounts =
                new List<decimal>();


            PayrollDeductionAmounts =
                new List<decimal>();


            PayrollNetAmounts =
                new List<decimal>();
            RecentPayrolls =
    new List<DashboardPayrollVM>();
        }
    }
}
