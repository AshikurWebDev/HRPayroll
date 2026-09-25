namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRBACLeaveAndEmployeeDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeLeaveBalances",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false),
                        LeaveTypeId = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        AllocatedDays = c.Int(nullable: false),
                        UsedDays = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EmployeeId, t.LeaveTypeId })
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.LeaveTypes", t => t.LeaveTypeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LeaveTypeId);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.PermissionId })
                .ForeignKey("dbo.Permissions", t => t.PermissionId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.PermissionId);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        PermissionId = c.Int(nullable: false, identity: true),
                        PermissionName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.PermissionId);
            
            AddColumn("dbo.Employees", "Phone", c => c.String(maxLength: 20));
            AddColumn("dbo.Employees", "Gender", c => c.String(maxLength: 20));
            AddColumn("dbo.Employees", "DateOfBirth", c => c.DateTime(nullable: false));
            AddColumn("dbo.Employees", "ManagerId", c => c.Int());
            AddColumn("dbo.Leaves", "ApprovedById", c => c.Int());
            AddColumn("dbo.Leaves", "PayrollImpactCalculated", c => c.Boolean(nullable: false));
            AddColumn("dbo.LeaveTypes", "GenderSpecific", c => c.String(maxLength: 20));
            CreateIndex("dbo.Employees", "ManagerId");
            CreateIndex("dbo.Leaves", "ApprovedById");
            AddForeignKey("dbo.Employees", "ManagerId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.Leaves", "ApprovedById", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Leaves", "ApprovedById", "dbo.Users");
            DropForeignKey("dbo.RolePermissions", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.RolePermissions", "PermissionId", "dbo.Permissions");
            DropForeignKey("dbo.EmployeeLeaveBalances", "LeaveTypeId", "dbo.LeaveTypes");
            DropForeignKey("dbo.EmployeeLeaveBalances", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "ManagerId", "dbo.Employees");
            DropIndex("dbo.RolePermissions", new[] { "PermissionId" });
            DropIndex("dbo.RolePermissions", new[] { "RoleId" });
            DropIndex("dbo.Leaves", new[] { "ApprovedById" });
            DropIndex("dbo.EmployeeLeaveBalances", new[] { "LeaveTypeId" });
            DropIndex("dbo.EmployeeLeaveBalances", new[] { "EmployeeId" });
            DropIndex("dbo.Employees", new[] { "ManagerId" });
            DropColumn("dbo.LeaveTypes", "GenderSpecific");
            DropColumn("dbo.Leaves", "PayrollImpactCalculated");
            DropColumn("dbo.Leaves", "ApprovedById");
            DropColumn("dbo.Employees", "ManagerId");
            DropColumn("dbo.Employees", "DateOfBirth");
            DropColumn("dbo.Employees", "Gender");
            DropColumn("dbo.Employees", "Phone");
            DropTable("dbo.Permissions");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.EmployeeLeaveBalances");
        }
    }
}
