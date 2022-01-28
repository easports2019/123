namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas");
            DropForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.UserProfiles", "LegId", "dbo.Legs");
            DropIndex("dbo.UserProfiles", new[] { "CityId" });
            DropIndex("dbo.UserProfiles", new[] { "LegId" });
            DropIndex("dbo.UserProfiles", new[] { "AmpluaId" });
            AlterColumn("dbo.UserProfiles", "CityId", c => c.Int());
            AlterColumn("dbo.UserProfiles", "LegId", c => c.Int());
            AlterColumn("dbo.UserProfiles", "AmpluaId", c => c.Int());
            CreateIndex("dbo.UserProfiles", "CityId");
            CreateIndex("dbo.UserProfiles", "LegId");
            CreateIndex("dbo.UserProfiles", "AmpluaId");
            AddForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas", "AmpluaId");
            AddForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities", "CityId");
            AddForeignKey("dbo.UserProfiles", "LegId", "dbo.Legs", "LegId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "LegId", "dbo.Legs");
            DropForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas");
            DropIndex("dbo.UserProfiles", new[] { "AmpluaId" });
            DropIndex("dbo.UserProfiles", new[] { "LegId" });
            DropIndex("dbo.UserProfiles", new[] { "CityId" });
            AlterColumn("dbo.UserProfiles", "AmpluaId", c => c.Int(nullable: false));
            AlterColumn("dbo.UserProfiles", "LegId", c => c.Int(nullable: false));
            AlterColumn("dbo.UserProfiles", "CityId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserProfiles", "AmpluaId");
            CreateIndex("dbo.UserProfiles", "LegId");
            CreateIndex("dbo.UserProfiles", "CityId");
            AddForeignKey("dbo.UserProfiles", "LegId", "dbo.Legs", "LegId", cascadeDelete: true);
            AddForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities", "CityId", cascadeDelete: true);
            AddForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas", "AmpluaId", cascadeDelete: true);
        }
    }
}
