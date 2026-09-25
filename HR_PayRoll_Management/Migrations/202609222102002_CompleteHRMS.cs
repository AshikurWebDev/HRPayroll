namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompleteHRMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        AuditLogId = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 100),
                        Action = c.String(maxLength: 50),
                        Module = c.String(maxLength: 100),
                        ActionDate = c.DateTime(nullable: false),
                        OldValue = c.String(),
                        NewValue = c.String(),
                    })
                .PrimaryKey(t => t.AuditLogId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 100),
                        PasswordHash = c.String(nullable: false),
                        RoleId = c.Int(nullable: false),
                        EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.EmployeeId);
            
            AddColumn("dbo.SalarySlips", "WorkingDays", c => c.Int(nullable: false));
            AddColumn("dbo.SalarySlips", "PresentDays", c => c.Int(nullable: false));
            AddColumn("dbo.SalarySlips", "LeaveDeduction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Users", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Users", new[] { "EmployeeId" });
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropColumn("dbo.SalarySlips", "LeaveDeduction");
            DropColumn("dbo.SalarySlips", "PresentDays");
            DropColumn("dbo.SalarySlips", "WorkingDays");
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.AuditLogs");
        }
    }
}
