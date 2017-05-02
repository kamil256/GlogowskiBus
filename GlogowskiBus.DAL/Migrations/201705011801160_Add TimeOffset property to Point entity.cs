namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimeOffsetpropertytoPointentity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Points", "TimeOffset", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Points", "TimeOffset");
        }
    }
}
