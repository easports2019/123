namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig11 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas");
            DropForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.UserProfiles", "LegId", "dbo.Legs");
            DropIndex("dbo.UserProfiles", new[] { "CityId" });
            DropIndex("dbo.UserProfiles", new[] { "LegId" });
            DropIndex("dbo.UserProfiles", new[] { "AmpluaId" });
            AddColumn("dbo.UserProfiles", "CityVkId", c => c.Int());
            AddColumn("dbo.UserProfiles", "CityUmbracoId", c => c.Int());
            AddColumn("dbo.UserProfiles", "CityName", c => c.String());
            DropColumn("dbo.UserProfiles", "CityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "CityId", c => c.Int());
            DropColumn("dbo.UserProfiles", "CityName");
            DropColumn("dbo.UserProfiles", "CityUmbracoId");
            DropColumn("dbo.UserProfiles", "CityVkId");
            CreateIndex("dbo.UserProfiles", "AmpluaId");
            CreateIndex("dbo.UserProfiles", "LegId");
            CreateIndex("dbo.UserProfiles", "CityId");
            AddForeignKey("dbo.UserProfiles", "LegId", "dbo.Legs", "LegId");
            AddForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities", "CityId");
            AddForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas", "AmpluaId");
        }
    }
}
