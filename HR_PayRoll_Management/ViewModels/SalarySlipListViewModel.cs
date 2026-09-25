using System;

namespace HR_PayRoll_Management.ViewModels
{
    public class SalarySlipListViewModel
    {
        public int SalarySlipId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public decimal GrossSalary { get; set; }

        public decimal TotalDeduction { get; set; }

        public decimal NetSalary { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}