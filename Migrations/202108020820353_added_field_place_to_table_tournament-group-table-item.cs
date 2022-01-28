namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_place_to_table_tournamentgrouptableitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentGroupTableItems", "Place", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentGroupTableItems", "Place");
        }
    }
}
