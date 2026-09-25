using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR_PayRoll_Management.ViewModels
{
    public class EmployeeDeleteViewModel
    {
        public int EmployeeId { get; set; } 
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhotoPath { get; set; }
    }
}