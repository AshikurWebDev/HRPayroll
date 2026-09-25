namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attendances",
                c => new
                    {
                        AttendanceId = c.Int(nullable: false, identity: true),
                        AttendanceDate = c.DateTime(nullable: false),
                        Status = c.String(nullable: false, maxLength: 20),
                        Remarks = c.String(maxLength: 200),
                        EmployeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AttendanceId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        EmailAddress = c.String(nullable: false, maxLength: 150),
                        Designation = c.String(maxLength: 100),
                        PhotoPath = c.String(),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HouseRent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MedicalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        JoiningDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false, maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.EmployeeFiles",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 200),
                        FilePath = c.String(nullable: false, maxLength: 300),
                        UploadDate = c.DateTime(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.SalarySlips",
                c => new
                    {
                        SalarySlipId = c.Int(nullable: false, identity: true),
                        PayrollCycleId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HouseRent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MedicalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonusAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AbsentDays = c.Int(nullable: false),
                        AbsentDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SalarySlipId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.PayrollCycles", t => t.PayrollCycleId, cascadeDelete: true)
                .Index(t => t.PayrollCycleId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.PayrollCycles",
                c => new
                    {
                        PayrollCycleId = c.Int(nullable: false, identity: true),
                        CycleMonth = c.Int(nullable: false),
                        CycleYear = c.Int(nullable: false),
                        ProcessedDate = c.DateTime(nullable: false),
                        TotalEmployees = c.Int(nullable: false),
                        TotalGrossAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDeductions = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalNetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.PayrollCycleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalarySlips", "PayrollCycleId", "dbo.PayrollCycles");
            DropForeignKey("dbo.SalarySlips", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EmployeeFiles", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Attendances", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.SalarySlips", new[] { "EmployeeId" });
            DropIndex("dbo.SalarySlips", new[] { "PayrollCycleId" });
            DropIndex("dbo.EmployeeFiles", new[] { "EmployeeId" });
            DropIndex("dbo.Employees", new[] { "DepartmentId" });
            DropIndex("dbo.Attendances", new[] { "EmployeeId" });
            DropTable("dbo.PayrollCycles");
            DropTable("dbo.SalarySlips");
            DropTable("dbo.EmployeeFiles");
            DropTable("dbo.Departments");
            DropTable("dbo.Employees");
            DropTable("dbo.Attendances");
        }
    }
}
