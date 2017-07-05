namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BusNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Routes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IndexMark = c.String(),
                        Details = c.String(),
                        BusLineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BusLines", t => t.BusLineId, cascadeDelete: true)
                .Index(t => t.BusLineId);
            
            CreateTable(
                "dbo.DepartureTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Hours = c.Int(nullable: false),
                        Minutes = c.Int(nullable: false),
                        WorkingDay = c.Boolean(nullable: false),
                        Saturday = c.Boolean(nullable: false),
                        Sunday = c.Boolean(nullable: false),
                        RouteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Routes", t => t.RouteId, cascadeDelete: true)
                .Index(t => t.RouteId);
            
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        TimeOffset = c.Int(nullable: false),
                        RouteId = c.Int(nullable: false),
                        BusStopId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BusStops", t => t.BusStopId)
                .ForeignKey("dbo.Routes", t => t.RouteId, cascadeDelete: true)
                .Index(t => t.RouteId)
                .Index(t => t.BusStopId);
            
            CreateTable(
                "dbo.BusStops",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
