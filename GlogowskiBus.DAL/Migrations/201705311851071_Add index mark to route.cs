namespace GlogowskiBus.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addindexmarktoroute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Routes", "IndexMark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Routes", "IndexMark");
        }
    }
}
