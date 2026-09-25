using System;

namespace HR_PayRoll_Management.ViewModels.EmployeeFile
{
    public class EmployeeFileViewModel
    {
        public int FileId { get; set; }
        public int EmployeeId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
    }
}