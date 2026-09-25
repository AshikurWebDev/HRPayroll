namespace HR_PayRoll_Management.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddEmployeeShiftAndUpdateAttendance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
         "dbo.Shifts",
         c => new
         {
             ShiftId = c.Int(nullable: false, identity: true),
             ShiftName = c.String(nullable: false, maxLength: 50),
             StartTime = c.Time(nullable: false, precision: 7),
             EndTime = c.Time(nullable: false, precision: 7),
             LateAllowanceMinutes = c.Int(nullable: false),
         })
     .PrimaryKey(t => t.ShiftId);


            Sql("INSERT INTO Shifts (ShiftName, StartTime, EndTime, LateAllowanceMinutes) VALUES ('General Shift', '09:00:00', '18:00:00', 15)");


            AddColumn("dbo.Attendances", "EntryTime", c => c.Time(precision: 7));
            AddColumn("dbo.Attendances", "ExitTime", c => c.Time(precision: 7));
            AddColumn("dbo.Attendances", "IsLate", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Attendances", "WorkingHours", c => c.Double());


            AddColumn("dbo.Employees", "ShiftId", c => c.Int(nullable: false, defaultValue: 1));


            CreateIndex("dbo.Employees", "ShiftId");


            AddForeignKey(
                "dbo.Employees",
                "ShiftId",
                "dbo.Shifts",
                "ShiftId",
                cascadeDelete: true
            );
        }

        public override void Down()
        {
            DropForeignKey("dbo.Employees", "ShiftId", "dbo.Shifts");
            DropIndex("dbo.Employees", new[] { "ShiftId" });
            DropColumn("dbo.Employees", "ShiftId");
            DropColumn("dbo.Attendances", "WorkingHours");
            DropColumn("dbo.Attendances", "IsLate");
            DropColumn("dbo.Attendances", "ExitTime");
            DropColumn("dbo.Attendances", "EntryTime");
            DropTable("dbo.Shifts");
        }
    }
}
