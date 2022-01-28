namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Places", "Name", c => c.String());
            AddColumn("dbo.Places", "Info", c => c.String());
            DropColumn("dbo.Places", "PlaceName");
            DropColumn("dbo.Places", "PlaceInfo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "PlaceInfo", c => c.String());
            AddColumn("dbo.Places", "PlaceName", c => c.String());
            DropColumn("dbo.Places", "Info");
            DropColumn("dbo.Places", "Name");
        }
    }
}
