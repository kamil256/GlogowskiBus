namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddlatitudeandlongitudetoBusStopentity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusStops", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.BusStops", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusStops", "Longitude");
            DropColumn("dbo.BusStops", "Latitude");
        }
    }
}
