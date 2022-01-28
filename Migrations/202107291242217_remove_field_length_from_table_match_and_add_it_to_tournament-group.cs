namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_field_length_from_table_match_and_add_it_to_tournamentgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentGroups", "MatchLength", c => c.Int(nullable: false));
            DropColumn("dbo.Matches", "Length");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "Length", c => c.Int(nullable: false));
            DropColumn("dbo.TournamentGroups", "MatchLength");
        }
    }
}
