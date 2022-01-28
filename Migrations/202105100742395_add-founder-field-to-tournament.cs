namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfounderfieldtotournament : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "UserProfileId", c => c.Int());
            CreateIndex("dbo.Tournaments", "UserProfileId");
            AddForeignKey("dbo.Tournaments", "UserProfileId", "dbo.UserProfiles", "UserProfileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.Tournaments", new[] { "UserProfileId" });
            DropColumn("dbo.Tournaments", "UserProfileId");
        }
    }
}
