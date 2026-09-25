namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRequiredDesignationRelationship : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Employees", "Designation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Designation", c => c.String(maxLength: 100));
        }
    }
}
