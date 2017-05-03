namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeBusStopIdnullableinPointentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Points", "BusStopId", "dbo.BusStops");
            DropIndex("dbo.Points", new[] { "BusStopId" });
            AlterColumn("dbo.Points", "BusStopId", c => c.Int());
            CreateIndex("dbo.Points", "BusStopId");
            AddForeignKey("dbo.Points", "BusStopId", "dbo.BusStops", "BusStopId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Points", "BusStopId", "dbo.BusStops");
            DropIndex("dbo.Points", new[] { "BusStopId" });
            AlterColumn("dbo.Points", "BusStopId", c => c.Int(nullable: false));
            CreateIndex("dbo.Points", "BusStopId");
            AddForeignKey("dbo.Points", "BusStopId", "dbo.BusStops", "BusStopId", cascadeDelete: true);
        }
    }
}
