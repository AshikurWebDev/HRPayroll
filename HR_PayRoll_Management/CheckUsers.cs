using HR_PayRoll_Management.DAL;
using System;
using System.Linq;
using System.Data.Entity;

namespace HR_PayRoll_Management {

    public class CheckUsers
    {
        public static void Run()
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users.Include(u => u.Role).Include(u => u.Employee.Department).ToList();
                foreach (var u in users)
                {
                    string dept = u.Employee?.Department?.DepartmentName ?? "None";
                    Console.WriteLine("User: " + u.Username + " | Role: " + u.Role?.RoleName + " | Dept: " + dept);
                }
            }
        }
    }
}
