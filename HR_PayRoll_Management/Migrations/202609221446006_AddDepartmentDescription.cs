namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartmentDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "Description", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "Description");
        }
    }
}
