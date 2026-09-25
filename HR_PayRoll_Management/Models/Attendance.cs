using System;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }
        [Required]
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? EntryTime { get; set; }
        public TimeSpan? ExitTime { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }
        public bool IsLate { get; set; }
        public double? WorkingHours { get; set; }

        [StringLength(200)]
        public string Remarks { get; set; }

        // Foreign Key
        public int EmployeeId { get; set; }
        // Navigation Property
        public virtual Employee Employee { get; set; }
    }
}