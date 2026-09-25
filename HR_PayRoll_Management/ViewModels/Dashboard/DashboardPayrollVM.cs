namespace HR_PayRoll_Management.ViewModels.Dashboard
{
    public class DashboardPayrollVM
    {
        public int SalarySlipId { get; set; }

        public string EmployeeName { get; set; }
        public string PhotoPath { get; set; }

        public string DepartmentName { get; set; }


        public decimal GrossSalary { get; set; }

        public decimal TotalDeduction { get; set; }

        public decimal NetSalary { get; set; }


        public string Status { get; set; }
    }
}