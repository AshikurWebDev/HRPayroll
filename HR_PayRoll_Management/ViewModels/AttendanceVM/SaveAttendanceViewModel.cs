using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR_PayRoll_Management.ViewModels.AttendanceVM
{
    public class SaveAttendanceViewModel
    {
        public DateTime AttendanceDate { get; set;  }
        public List<AttenDanceEntryVM> AttendanceEntries { get; set;  }

    }

    public class AttenDanceEntryVM
    {
        public int EmployeeId { get; set; } 
        public string Status { get; set; }
        public TimeSpan? EntryTime { get; set;  }
        public TimeSpan? ExitTime { get; set; }
        public string Remarks { get; set;  }
    }
}