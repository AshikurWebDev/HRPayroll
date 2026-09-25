using System;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class EmployeeFile
    {
        [Key]
        public int FileId { get; set; }

        // Foreign Key
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(200)]
        public string FileName { get; set; }

        [Required]
        [StringLength(300)]
        public string FilePath { get; set; }

        [Required]
        [StringLength(50)]
        public string FileType { get; set; }

        public DateTime UploadDate { get; set; }

        // Navigation Property [just for the references]
        public virtual Employee Employee { get; set; }
    }
}