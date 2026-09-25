namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransportAllowanceToSalarySlip : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalarySlips", "TransportAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalarySlips", "TransportAllowance");
        }
    }
}
