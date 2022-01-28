namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_fileds_to_Match_table_which_identificates_teams_by_bids_to_tournaments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "BidTeamToTournamentId1", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "BidTeamToTournamentId2", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "BidTeamToTournament1_Id", c => c.Int());
            AddColumn("dbo.Matches", "BidTeamToTournament2_Id", c => c.Int());
            CreateIndex("dbo.Matches", "BidTeamToTournament1_Id");
            CreateIndex("dbo.Matches", "BidTeamToTournament2_Id");
            AddForeignKey("dbo.Matches", "BidTeamToTournament1_Id", "dbo.BidTeamToTournaments", "Id");
            AddForeignKey("dbo.Matches", "BidTeamToTournament2_Id", "dbo.BidTeamToTournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Matches", "BidTeamToTournament2_Id", "dbo.BidTeamToTournaments");
            DropForeignKey("dbo.Matches", "BidTeamToTournament1_Id", "dbo.BidTeamToTournaments");
            DropIndex("dbo.Matches", new[] { "BidTeamToTournament2_Id" });
            DropIndex("dbo.Matches", new[] { "BidTeamToTournament1_Id" });
            DropColumn("dbo.Matches", "BidTeamToTournament2_Id");
            DropColumn("dbo.Matches", "BidTeamToTournament1_Id");
            DropColumn("dbo.Matches", "BidTeamToTournamentId2");
            DropColumn("dbo.Matches", "BidTeamToTournamentId1");
        }
    }
}
