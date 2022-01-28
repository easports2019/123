namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldPlaceIdinMatchestablecanbenull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "PlaceId", "dbo.Places");
            DropIndex("dbo.Matches", new[] { "PlaceId" });
            AlterColumn("dbo.Matches", "PlaceId", c => c.Int());
            CreateIndex("dbo.Matches", "PlaceId");
            AddForeignKey("dbo.Matches", "PlaceId", "dbo.Places", "PlaceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Matches", "PlaceId", "dbo.Places");
            DropIndex("dbo.Matches", new[] { "PlaceId" });
            AlterColumn("dbo.Matches", "PlaceId", c => c.Int(nullable: false));
            CreateIndex("dbo.Matches", "PlaceId");
            AddForeignKey("dbo.Matches", "PlaceId", "dbo.Places", "PlaceId", cascadeDelete: true);
        }
    }
}
