using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HR_PayRoll_Management.ViewModels
{
    public class SalaryViewModel
    {
        public int SalaryStructureId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRent { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal TaxPercentage { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<SelectListItem> Employees { get; set; }
    }
}