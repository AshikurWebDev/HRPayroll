namespace HR_PayRoll_Management.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddDesignationsAndShifts : DbMigration
    {
        public override void Up()
        {
            // 1. Create the Designations table
            CreateTable(
                "dbo.Designations",
                c => new
                {
                    DesignationId = c.Int(nullable: false, identity: true),
                    DesignationName = c.String(nullable: false, maxLength: 100),
                    Description = c.String(maxLength: 250),
                    IsActive = c.Boolean(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.DesignationId);
            // 2. Build the Relationship (ICollection<Employee>)
            // First, make sure the column exists on the Employees table 
            // (Note: If your Employees table already has this column, you can delete this AddColumn line!)
            AddColumn("dbo.Employees", "DesignationId", c => c.Int(nullable: false, defaultValue: 1));

            // Create an index on the column to make queries fast
            CreateIndex("dbo.Employees", "DesignationId");

            // Finally, link the ICollection relationship between Employees and Designations
            AddForeignKey("dbo.Employees", "DesignationId", "dbo.Designations", "DesignationId", cascadeDelete: false);
        }
        public override void Down()
        {
            // Always dismantle relationships in reverse order before dropping the table
            DropForeignKey("dbo.Employees", "DesignationId", "dbo.Designations");
            DropIndex("dbo.Employees", new[] { "DesignationId" });
            DropColumn("dbo.Employees", "DesignationId");
            DropTable("dbo.Designations");
        }
    }
}
