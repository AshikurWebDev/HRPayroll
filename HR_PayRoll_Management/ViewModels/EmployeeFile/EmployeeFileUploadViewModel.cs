using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR_PayRoll_Management.ViewModels.EmployeeFile
{
    public class EmployeeFileUploadViewModel
    {
        public int EmployeeId { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}