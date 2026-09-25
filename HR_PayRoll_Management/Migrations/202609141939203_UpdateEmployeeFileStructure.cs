namespace HR_PayRoll_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEmployeeFileStructure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeFiles", "FileType", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmployeeFiles", "FileType");
        }
    }
}
