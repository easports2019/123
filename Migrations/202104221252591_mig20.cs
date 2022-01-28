namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig20 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.BidTeamToTournaments");
            DropColumn("dbo.BidTeamToTournaments", "BidTeamToTournamentId");
            AddColumn("dbo.BidTeamToTournaments", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.BidTeamToTournaments", "AdminTournamentComment", c => c.String());
            AddColumn("dbo.BidTeamToTournaments", "TeamName", c => c.String());
            AddColumn("dbo.BidTeamToTournaments", "UserProfileId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.BidTeamToTournaments", "Id");
            CreateIndex("dbo.BidTeamToTournaments", "TournamentId");
            CreateIndex("dbo.BidTeamToTournaments", "UserProfileId");
            AddForeignKey("dbo.BidTeamToTournaments", "TournamentId", "dbo.Tournaments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BidTeamToTournaments", "UserProfileId", "dbo.UserProfiles", "UserProfileId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.BidTeamToTournaments", "BidTeamToTournamentId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.BidTeamToTournaments", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.BidTeamToTournaments", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.BidTeamToTournaments", new[] { "UserProfileId" });
            DropIndex("dbo.BidTeamToTournaments", new[] { "TournamentId" });
            DropPrimaryKey("dbo.BidTeamToTournaments");
            DropColumn("dbo.BidTeamToTournaments", "UserProfileId");
            DropColumn("dbo.BidTeamToTournaments", "TeamName");
            DropColumn("dbo.BidTeamToTournaments", "AdminTournamentComment");
            DropColumn("dbo.BidTeamToTournaments", "Id");
            AddPrimaryKey("dbo.BidTeamToTournaments", "BidTeamToTournamentId");
        }
    }
}
