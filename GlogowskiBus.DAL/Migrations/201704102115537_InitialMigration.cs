namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BusLineBusStops", "BusLineId", "dbo.BusLines");
            DropForeignKey("dbo.BusLineBusStops", "BusStopId", "dbo.BusStops");
            DropForeignKey("dbo.Points", "BusStopId", "dbo.BusStops");
            DropIndex("dbo.BusLineBusStops", new[] { "BusLineId" });
            DropIndex("dbo.BusLineBusStops", new[] { "BusStopId" });
            DropIndex("dbo.Points", new[] { "BusStopId" });
            AddColumn("dbo.Points", "IsBusStop", c => c.Boolean(nullable: false));
            DropColumn("dbo.Points", "BusStopId");
            DropTable("dbo.BusLineBusStops");
            DropTable("dbo.BusStops");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BusStops",
                c => new
                    {
                        BusStopId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.BusStopId);
            
            CreateTable(
                "dbo.BusLineBusStops",
                c => new
                    {
                        BusLineId = c.Int(nullable: false),
                        BusStopId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        TimeOffsetInSeconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BusLineId, t.BusStopId });
            
            AddColumn("dbo.Points", "BusStopId", c => c.Int());
            DropColumn("dbo.Points", "IsBusStop");
            CreateIndex("dbo.Points", "BusStopId");
            CreateIndex("dbo.BusLineBusStops", "BusStopId");
            CreateIndex("dbo.BusLineBusStops", "BusLineId");
            AddForeignKey("dbo.Points", "BusStopId", "dbo.BusStops", "BusStopId");
            AddForeignKey("dbo.BusLineBusStops", "BusStopId", "dbo.BusStops", "BusStopId", cascadeDelete: true);
            AddForeignKey("dbo.BusLineBusStops", "BusLineId", "dbo.BusLines", "BusLineId", cascadeDelete: true);
        }
    }
}
