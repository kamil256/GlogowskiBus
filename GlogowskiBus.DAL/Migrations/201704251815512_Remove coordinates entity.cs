namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removecoordinatesentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Points", "CoordinatesId", "dbo.Coordinates");
            DropIndex("dbo.Points", new[] { "CoordinatesId" });
            AddColumn("dbo.Points", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Points", "Longitude", c => c.Double(nullable: false));
            DropColumn("dbo.Points", "CoordinatesId");
            DropTable("dbo.Coordinates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Coordinates",
                c => new
                    {
                        CoordinatesId = c.Int(nullable: false, identity: true),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CoordinatesId);
            
            AddColumn("dbo.Points", "CoordinatesId", c => c.Int(nullable: false));
            DropColumn("dbo.Points", "Longitude");
            DropColumn("dbo.Points", "Latitude");
            CreateIndex("dbo.Points", "CoordinatesId");
            AddForeignKey("dbo.Points", "CoordinatesId", "dbo.Coordinates", "CoordinatesId", cascadeDelete: true);
        }
    }
}
