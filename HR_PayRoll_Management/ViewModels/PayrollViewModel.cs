using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR_PayRoll_Management.ViewModels
{
    public class PayrollViewModel
    {
        public int PayrollCycleId { get; set; }

        public int CycleMonth { get; set; }

        public int CycleYear { get; set; }

        public DateTime ProcessedDate { get; set; }

        public int TotalEmployees { get; set; }

        public decimal TotalGrossAmount { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal TotalNetAmount { get; set; }

        public string Status { get; set; }
    }
}