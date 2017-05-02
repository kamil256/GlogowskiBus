namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyScheduletable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Schedules", newName: "DepartureTimes");
            DropPrimaryKey("dbo.DepartureTimes");
            DropColumn("dbo.DepartureTimes", "ScheduleId");
            AddColumn("dbo.DepartureTimes", "DepartureTimeId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.DepartureTimes", "Hour", c => c.Int(nullable: false));
            AddColumn("dbo.DepartureTimes", "Minute", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DepartureTimes", "DepartureTimeId");
            DropColumn("dbo.DepartureTimes", "StartTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DepartureTimes", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.DepartureTimes", "ScheduleId", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.DepartureTimes");
            DropColumn("dbo.DepartureTimes", "Minute");
            DropColumn("dbo.DepartureTimes", "Hour");
            DropColumn("dbo.DepartureTimes", "DepartureTimeId");
            AddPrimaryKey("dbo.DepartureTimes", "ScheduleId");
            RenameTable(name: "dbo.DepartureTimes", newName: "Schedules");
        }
    }
}
