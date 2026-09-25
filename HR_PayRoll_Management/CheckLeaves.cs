using HR_PayRoll_Management.DAL;
using System.Linq;

namespace CheckDB {
    class Program {
        static void Main() {
            using (var db = new AppDbContext()) {
                var leaves = db.Leaves.ToList();
                System.Console.WriteLine("Total Leaves: " + leaves.Count);
                foreach(var l in leaves) {
                    System.Console.WriteLine("Leave ID: " + l.LeaveId + ", EmpID: " + l.EmployeeId + ", Status: " + l.Status);
                }
            }
        }
    }
}
