namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeeSalaryStructure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeSalaryStructures",
                c => new
                    {
                        SalaryStructureId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HouseRent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MedicalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonusAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EffectiveDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SalaryStructureId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeSalaryStructures", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.EmployeeSalaryStructures", new[] { "EmployeeId" });
            DropTable("dbo.EmployeeSalaryStructures");
        }
    }
}
