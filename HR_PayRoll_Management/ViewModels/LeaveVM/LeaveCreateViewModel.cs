using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HR_PayRoll_Management.ViewModels.LeaveVM
{
    public class LeaveCreateViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        public string Reason { get; set; }


        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
    }

    public class LeaveBalanceCardVM
    {
        public string Name { get; set; }
        public int Remaining { get; set; }
        public int Allocated { get; set; }
    }
}