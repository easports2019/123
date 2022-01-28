namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_fields_to_table_TournamentGroupTableItem : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TournamentGroupTableItems", "BidTeamToTournamentId");
            CreateIndex("dbo.TournamentGroupTableItems", "TournamentGroupId");
            AddForeignKey("dbo.TournamentGroupTableItems", "BidTeamToTournamentId", "dbo.BidTeamToTournaments", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TournamentGroupTableItems", "TournamentGroupId", "dbo.TournamentGroups", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentGroupTableItems", "TournamentGroupId", "dbo.TournamentGroups");
            DropForeignKey("dbo.TournamentGroupTableItems", "BidTeamToTournamentId", "dbo.BidTeamToTournaments");
            DropIndex("dbo.TournamentGroupTableItems", new[] { "TournamentGroupId" });
            DropIndex("dbo.TournamentGroupTableItems", new[] { "BidTeamToTournamentId" });
        }
    }
}
