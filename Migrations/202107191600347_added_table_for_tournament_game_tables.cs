namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_table_for_tournament_game_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TournamentGroupTableItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BidTeamToTournamentId = c.Int(nullable: false),
                        TournamentGroupId = c.Int(nullable: false),
                        Games = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        Loses = c.Int(nullable: false),
                        Draws = c.Int(nullable: false),
                        GoalsScored = c.Int(nullable: false),
                        GoalsMissed = c.Int(nullable: false),
                        GoalsDifference = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TournamentGroupTableItems");
        }
    }
}
