using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HR_PayRoll_Management.ViewModels.LeaveVM
{
    public class LeaveEditViewModel
    {
        public int LeaveId { get; set; }


        [Required]
        public int EmployeeId { get; set; }


        [Required]
        public int LeaveTypeId { get; set; }


        [Required]
        public DateTime FromDate { get; set; }


        [Required]
        public DateTime ToDate { get; set; }


        public string Reason { get; set; }


        public string Status { get; set; }


        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
    }
}