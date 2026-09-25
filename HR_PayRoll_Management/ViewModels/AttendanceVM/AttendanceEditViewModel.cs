using System;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class AttendanceEditViewModel
    {
        public int AttendanceId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? EntryTime { get; set; }
        public TimeSpan? ExitTime { get; set; }
        [Required]
        [StringLength(20)]
        public string Status { get; set; }
        [StringLength(200)]
        public string Remarks { get; set; }
    }
}