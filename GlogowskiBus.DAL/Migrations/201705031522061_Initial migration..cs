namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialmigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusLines",
                c => new
                    {
                        BusLineId = c.Int(nullable: false, identity: true),
                        BusNumber = c.String(),
                    })
                .PrimaryKey(t => t.BusLineId);
            
            CreateTable(
                "dbo.Routes",
                c => new
                    {
                        RouteId = c.Int(nullable: false, identity: true),
                        BusLineId = c.Int(nullable: false),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.RouteId)
                .ForeignKey("dbo.BusLines", t => t.BusLineId, cascadeDelete: true)
                .Index(t => t.BusLineId);
            
            CreateTable(
                "dbo.DepartureTimes",
                c => new
                    {
                        DepartureTimeId = c.Int(nullable: false, identity: true),
                        RouteId = c.Int(nullable: false),
                        Hours = c.Int(nullable: false),
                        Minutes = c.Int(nullable: false),
                        WorkingDay = c.Boolean(nullable: false),
                        Saturday = c.Boolean(nullable: false),
                        Sunday = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DepartureTimeId)
                .ForeignKey("dbo.Routes", t => t.RouteId, cascadeDelete: true)
                .Index(t => t.RouteId);
            
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        PointId = c.Int(nullable: false, identity: true),
                        RouteId = c.Int(nullable: false),
                        BusStopId = c.Int(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        TimeOffset = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PointId)
                .ForeignKey("dbo.BusStops", t => t.BusStopId, cascadeDelete: true)
                .ForeignKey("dbo.Routes", t => t.RouteId, cascadeDelete: true)
                .Index(t => t.RouteId)
                .Index(t => t.BusStopId);
            
            CreateTable(
                "dbo.BusStops",
                c => new
                    {
                        BusStopId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.BusStopId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Points", "RouteId", "dbo.Routes");
            DropForeignKey("dbo.Points", "BusStopId", "dbo.BusStops");
            DropForeignKey("dbo.DepartureTimes", "RouteId", "dbo.Routes");
            DropForeignKey("dbo.Routes", "BusLineId", "dbo.BusLines");
            DropIndex("dbo.Points", new[] { "BusStopId" });
            DropIndex("dbo.Points", new[] { "RouteId" });
            DropIndex("dbo.DepartureTimes", new[] { "RouteId" });
            DropIndex("dbo.Routes", new[] { "BusLineId" });
            DropTable("dbo.BusStops");
            DropTable("dbo.Points");
            DropTable("dbo.DepartureTimes");
            DropTable("dbo.Routes");
            DropTable("dbo.BusLines");
        }
    }
}
