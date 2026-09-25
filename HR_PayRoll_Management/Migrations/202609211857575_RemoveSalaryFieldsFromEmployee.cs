namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSalaryFieldsFromEmployee : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Employees", "BasicSalary");
            DropColumn("dbo.Employees", "HouseRent");
            DropColumn("dbo.Employees", "MedicalAllowance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "MedicalAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Employees", "HouseRent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Employees", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
