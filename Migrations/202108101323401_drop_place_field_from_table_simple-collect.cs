namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class drop_place_field_from_table_simplecollect : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SimpleCollects", "PlaceId", "dbo.Places");
            DropIndex("dbo.SimpleCollects", new[] { "PlaceId" });
            AddColumn("dbo.SimpleCollects", "PlaceUmbracoId", c => c.Int(nullable: false));
            DropColumn("dbo.SimpleCollects", "PlaceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SimpleCollects", "PlaceId", c => c.Int(nullable: false));
            DropColumn("dbo.SimpleCollects", "PlaceUmbracoId");
            CreateIndex("dbo.SimpleCollects", "PlaceId");
            AddForeignKey("dbo.SimpleCollects", "PlaceId", "dbo.Places", "PlaceId", cascadeDelete: true);
        }
    }
}
