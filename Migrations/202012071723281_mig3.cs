namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", "FavouriteAmplua_AmpluaId", "dbo.Ampluas");
            DropIndex("dbo.UserProfiles", new[] { "FavouriteAmplua_AmpluaId" });
            RenameColumn(table: "dbo.UserProfiles", name: "FavouriteAmplua_AmpluaId", newName: "AmpluaId");
            AlterColumn("dbo.UserProfiles", "AmpluaId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserProfiles", "AmpluaId");
            AddForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas", "AmpluaId", cascadeDelete: true);
            DropColumn("dbo.UserProfiles", "FavouriteAmpluaId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "FavouriteAmpluaId", c => c.Int(nullable: false));
            DropForeignKey("dbo.UserProfiles", "AmpluaId", "dbo.Ampluas");
            DropIndex("dbo.UserProfiles", new[] { "AmpluaId" });
            AlterColumn("dbo.UserProfiles", "AmpluaId", c => c.Int());
            RenameColumn(table: "dbo.UserProfiles", name: "AmpluaId", newName: "FavouriteAmplua_AmpluaId");
            CreateIndex("dbo.UserProfiles", "FavouriteAmplua_AmpluaId");
            AddForeignKey("dbo.UserProfiles", "FavouriteAmplua_AmpluaId", "dbo.Ampluas", "AmpluaId");
        }
    }
}
