namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CityTournamentAdmins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserProfileId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId)
                .Index(t => t.CityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CityTournamentAdmins", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.CityTournamentAdmins", "CityId", "dbo.Cities");
            DropIndex("dbo.CityTournamentAdmins", new[] { "CityId" });
            DropIndex("dbo.CityTournamentAdmins", new[] { "UserProfileId" });
            DropTable("dbo.CityTournamentAdmins");
        }
    }
}
