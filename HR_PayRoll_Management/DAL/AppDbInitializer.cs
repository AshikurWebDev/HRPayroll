using HR_PayRoll_Management.DAL;
using HR_PayRoll_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace HR_PayRoll_Management.Web.DAL
{
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {

        protected override void Seed(AppDbContext context)
        {

            var departments = new List<Department>
            {
                new Department
                {
                    DepartmentName = "IT",
                    IsActive = true
                },

                new Department
                {
                    DepartmentName = "Human Resource",
                    IsActive = true
                },

                new Department
                {
                    DepartmentName = "Finance",
                    IsActive = true
                }
            };


            departments.ForEach(d =>
            context.Departments.Add(d));


            context.SaveChanges();



            var employees = new List<Employee>
            {

                new Employee
                {
                    FullName = "Rahim Ahmed",
                    EmailAddress = "rahim@test.com",
                    JoiningDate = DateTime.Now,
                    IsActive = true,

                    DepartmentId = 1
                },


                new Employee
                {
                    FullName = "Karim Hasan",
                    EmailAddress = "karim@test.com",
                    JoiningDate = DateTime.Now,

                    IsActive = true,

                    DepartmentId = 2
                }

            };


            employees.ForEach(e =>
            context.Employees.Add(e));


            context.SaveChanges();

            var roles = new List<Role>
            {
                new Role { RoleName = "HR" },
                new Role { RoleName = "Manager" },
                new Role { RoleName = "Payroll Officer" },
                new Role { RoleName = "Employee" }
            };

            roles.ForEach(r => context.Roles.Add(r));
            context.SaveChanges();

            var users = new List<User>
            {
                new User { Username = "admin", PasswordHash = "admin", RoleId = 1 },
                new User { Username = "manager", PasswordHash = "manager", RoleId = 2 },
                new User { Username = "payroll", PasswordHash = "payroll", RoleId = 3 },
                new User { Username = "rahim", PasswordHash = "12345", RoleId = 4, EmployeeId = 1 },
                new User { Username = "karim", PasswordHash = "12345", RoleId = 4, EmployeeId = 2 }
            };

            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();


            base.Seed(context);
        }
    }
}