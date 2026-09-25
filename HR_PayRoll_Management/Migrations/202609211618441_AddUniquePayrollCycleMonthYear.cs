namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniquePayrollCycleMonthYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalarySlips", "OvertimeAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalarySlips", "GrossSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalarySlips", "TotalDeduction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.PayrollCycles", new[] { "CycleMonth", "CycleYear" }, unique: true, name: "IX_PayrollCycle_MonthYear");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PayrollCycles", "IX_PayrollCycle_MonthYear");
            DropColumn("dbo.SalarySlips", "TotalDeduction");
            DropColumn("dbo.SalarySlips", "GrossSalary");
            DropColumn("dbo.SalarySlips", "OvertimeAmount");
        }
    }
}
