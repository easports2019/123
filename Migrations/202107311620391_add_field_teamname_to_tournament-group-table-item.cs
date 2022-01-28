namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_teamname_to_tournamentgrouptableitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentGroupTableItems", "TeamName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentGroupTableItems", "TeamName");
        }
    }
}
