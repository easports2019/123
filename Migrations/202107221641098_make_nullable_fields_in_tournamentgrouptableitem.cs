namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class make_nullable_fields_in_tournamentgrouptableitem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TournamentGroupTableItems", "BidTeamToTournamentId", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "TournamentGroupId", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "Games", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "Wins", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "Loses", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "Draws", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "GoalsScored", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "GoalsMissed", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "GoalsDifference", c => c.Int());
            AlterColumn("dbo.TournamentGroupTableItems", "Points", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TournamentGroupTableItems", "Points", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "GoalsDifference", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "GoalsMissed", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "GoalsScored", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "Draws", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "Loses", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "Wins", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "Games", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "TournamentGroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentGroupTableItems", "BidTeamToTournamentId", c => c.Int(nullable: false));
        }
    }
}
