namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class replace_field_matchlength_from_table_tournamentgroup_to_tournaments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "MatchLength", c => c.Int(nullable: false));
            DropColumn("dbo.TournamentGroups", "MatchLength");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TournamentGroups", "MatchLength", c => c.Int(nullable: false));
            DropColumn("dbo.Tournaments", "MatchLength");
        }
    }
}
