namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changebidteamtotournamenttournamenttotournamentgroup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BidTeamToTournaments", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.BidTeamToTournaments", new[] { "TournamentId" });
            AddColumn("dbo.BidTeamToTournaments", "TournamentGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.BidTeamToTournaments", "TournamentGroupId");
            AddForeignKey("dbo.BidTeamToTournaments", "TournamentGroupId", "dbo.TournamentGroups", "Id", cascadeDelete: true);
            DropColumn("dbo.BidTeamToTournaments", "TournamentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BidTeamToTournaments", "TournamentId", c => c.Int(nullable: false));
            DropForeignKey("dbo.BidTeamToTournaments", "TournamentGroupId", "dbo.TournamentGroups");
            DropIndex("dbo.BidTeamToTournaments", new[] { "TournamentGroupId" });
            DropColumn("dbo.BidTeamToTournaments", "TournamentGroupId");
            CreateIndex("dbo.BidTeamToTournaments", "TournamentId");
            AddForeignKey("dbo.BidTeamToTournaments", "TournamentId", "dbo.Tournaments", "Id", cascadeDelete: true);
        }
    }
}
