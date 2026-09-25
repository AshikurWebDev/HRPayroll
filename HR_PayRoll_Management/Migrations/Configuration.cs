namespace HR_PayRoll_Management.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<HR_PayRoll_Management.DAL.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HR_PayRoll_Management.DAL.AppDbContext context)
        {
            // Seed Roles
            context.Roles.AddOrUpdate(
                r => r.RoleName,
                new Models.Role { RoleName = "Admin" },
                new Models.Role { RoleName = "HR Manager" },
                new Models.Role { RoleName = "HR Officer" },
                new Models.Role { RoleName = "Manager" },
                new Models.Role { RoleName = "Employee" }
            );
            context.SaveChanges();

            // Seed Permissions
            var permissions = new string[]
            {
                "Leave.ViewOwn", "Leave.Create", "Leave.CancelOwn",
                "Leave.ViewTeam", "Leave.ViewAll", "Leave.Approve", "Leave.Reject",
                "Leave.Override", "Leave.CreateType", "Leave.EditPolicy",
                "Leave.ManageEntitlement", "Leave.AdjustBalance"
            };

            foreach (var perm in permissions)
            {
                context.Permissions.AddOrUpdate(p => p.PermissionName, new Models.Permission { PermissionName = perm });
            }
            context.SaveChanges();

            // Assign Permissions to Roles
            var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
            var hrManagerRole = context.Roles.FirstOrDefault(r => r.RoleName == "HR Manager");
            var hrOfficerRole = context.Roles.FirstOrDefault(r => r.RoleName == "HR Officer");
            var managerRole = context.Roles.FirstOrDefault(r => r.RoleName == "Manager");
            var employeeRole = context.Roles.FirstOrDefault(r => r.RoleName == "Employee");

            void AssignPermissions(Models.Role role, params string[] perms)
            {
                if (role == null) return;
                foreach (var permName in perms)
                {
                    var permission = context.Permissions.FirstOrDefault(p => p.PermissionName == permName);
                    if (permission != null && !context.RolePermissions.Any(rp => rp.RoleId == role.RoleId && rp.PermissionId == permission.PermissionId))
                    {
                        context.RolePermissions.Add(new Models.RolePermission { RoleId = role.RoleId, PermissionId = permission.PermissionId });
                    }
                }
            }

            // Employee Permissions
            AssignPermissions(employeeRole, "Leave.ViewOwn", "Leave.Create", "Leave.CancelOwn");
            
            // Manager Permissions
            AssignPermissions(managerRole, "Leave.ViewOwn", "Leave.Create", "Leave.CancelOwn", "Leave.ViewTeam", "Leave.Approve", "Leave.Reject");

            // HR Officer Permissions
            AssignPermissions(hrOfficerRole, "Leave.ViewOwn", "Leave.Create", "Leave.CancelOwn", "Leave.ViewAll", "Leave.ManageEntitlement", "Leave.AdjustBalance");

            // HR Manager Permissions
            AssignPermissions(hrManagerRole, "Leave.ViewOwn", "Leave.Create", "Leave.CancelOwn", "Leave.ViewAll", "Leave.ManageEntitlement", "Leave.AdjustBalance", "Leave.Approve", "Leave.Reject", "Leave.Override", "Leave.CreateType", "Leave.EditPolicy");

            // Admin gets everything (handled via code or seed)
            AssignPermissions(adminRole, permissions);

            context.SaveChanges();

            // Create admin user if doesn't exist
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                context.Users.AddOrUpdate(
                    u => u.Username,
                    new Models.User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), RoleId = adminRole?.RoleId ?? 1 }
                );
                context.SaveChanges();
            }

            // Seed Leave Types
            context.LeaveTypes.AddOrUpdate(
                lt => lt.Name,
                new Models.LeaveType { Name = "Annual Leave", AllowedDays = 17, IsPaid = true, IsActive = true },
                new Models.LeaveType { Name = "Casual Leave", AllowedDays = 10, IsPaid = true, IsActive = true },
                new Models.LeaveType { Name = "Sick Leave", AllowedDays = 14, IsPaid = true, IsActive = true },
                new Models.LeaveType { Name = "Maternity Leave", AllowedDays = 180, IsPaid = true, GenderSpecific = "Female", IsActive = true },
                new Models.LeaveType { Name = "Paternity Leave", AllowedDays = 15, IsPaid = true, GenderSpecific = "Male", IsActive = true },
                new Models.LeaveType { Name = "Unpaid Leave", AllowedDays = 0, IsPaid = false, IsActive = true }
            );
            context.SaveChanges();
        }
    }
}
