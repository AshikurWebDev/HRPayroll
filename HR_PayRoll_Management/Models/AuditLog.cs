using System;
using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }
        
        [StringLength(100)]
        public string Username { get; set; }
        
        [StringLength(50)]
        public string Action { get; set; } // Create, Update, Delete
        
        [StringLength(100)]
        public string Module { get; set; } // Employee, SalaryStructure, etc.
        
        public DateTime ActionDate { get; set; }
        
        public string OldValue { get; set; }
        
        public string NewValue { get; set; }
    }
}
