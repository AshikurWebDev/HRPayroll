using HR_PayRoll_Management.Models;
using System.Data.Entity;
using System.Linq;

namespace HR_PayRoll_Management.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("HRConnection") { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<EmployeeFile> EmployeeFiles { get; set; }
        public DbSet<PayrollCycle> PayrollCycles { get; set; }
        public DbSet<SalarySlip> SalarySlips { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<EmployeeSalaryStructure> EmployeeSalaryStructures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public override int SaveChanges()
        {
            var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified || p.State == EntityState.Added || p.State == EntityState.Deleted).ToList();

            foreach (var change in modifiedEntities)
            {
                var entityName = change.Entity.GetType().Name;
                if (entityName == "AuditLog") continue;

                var audit = new AuditLog
                {
                    ActionDate = System.DateTime.Now,
                    Module = entityName,
                    Username = System.Web.HttpContext.Current?.User?.Identity?.Name ?? "System"
                };

                if (change.State == EntityState.Added)
                {
                    audit.Action = "Create";
                    audit.NewValue = GetEntityProperties(change);
                }
                else if (change.State == EntityState.Modified)
                {
                    audit.Action = "Update";
                    audit.OldValue = GetOriginalProperties(change);
                    audit.NewValue = GetEntityProperties(change);
                }
                else if (change.State == EntityState.Deleted)
                {
                    audit.Action = "Delete";
                    audit.OldValue = GetOriginalProperties(change);
                }
                AuditLogs.Add(audit);
            }
            return base.SaveChanges();
        }

        public override System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Modified || p.State == EntityState.Added || p.State == EntityState.Deleted).ToList();

            foreach (var change in modifiedEntities)
            {
                var entityName = change.Entity.GetType().Name;
                if (entityName == "AuditLog" || entityName.Contains("Proxy")) continue;

                var audit = new AuditLog
                {
                    ActionDate = System.DateTime.Now,
                    Module = entityName,
                    Username = System.Web.HttpContext.Current?.User?.Identity?.Name ?? "System"
                };

                if (change.State == EntityState.Added)
                {
                    audit.Action = "Create";
                    audit.NewValue = GetEntityProperties(change);
                }
                else if (change.State == EntityState.Modified)
                {
                    audit.Action = "Update";
                    audit.OldValue = GetOriginalProperties(change);
                    audit.NewValue = GetEntityProperties(change);
                }
                else if (change.State == EntityState.Deleted)
                {
                    audit.Action = "Delete";
                    audit.OldValue = GetOriginalProperties(change);
                }
                AuditLogs.Add(audit);
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        private string GetEntityProperties(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var values = new System.Text.StringBuilder();
            foreach (var prop in entry.CurrentValues.PropertyNames)
            {
                values.Append($"{prop}: {entry.CurrentValues[prop]}; ");
            }
            return values.ToString();
        }

        private string GetOriginalProperties(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var values = new System.Text.StringBuilder();
            foreach (var prop in entry.OriginalValues.PropertyNames)
            {
                values.Append($"{prop}: {entry.OriginalValues[prop]}; ");
            }
            return values.ToString();
        }
    }
}
