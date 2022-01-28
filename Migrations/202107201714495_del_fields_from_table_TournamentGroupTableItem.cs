namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class del_fields_from_table_TournamentGroupTableItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TournamentGroupTableItems", "BidTeamToTournamentId", "dbo.BidTeamToTournaments");
            DropForeignKey("dbo.TournamentGroupTableItems", "TournamentGroupId", "dbo.TournamentGroups");
            DropIndex("dbo.TournamentGroupTableItems", new[] { "BidTeamToTournamentId" });
            DropIndex("dbo.TournamentGroupTableItems", new[] { "TournamentGroupId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.TournamentGroupTableItems", "TournamentGroupId");
            CreateIndex("dbo.TournamentGroupTableItems", "BidTeamToTournamentId");
            AddForeignKey("dbo.TournamentGroupTableItems", "TournamentGroupId", "dbo.TournamentGroups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TournamentGroupTableItems", "BidTeamToTournamentId", "dbo.BidTeamToTournaments", "Id", cascadeDelete: true);
        }
    }
}
