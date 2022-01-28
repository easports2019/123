namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_fields_length_and_played_to_table_match : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Played", c => c.Boolean(nullable: false));
            AddColumn("dbo.Matches", "Length", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "Length");
            DropColumn("dbo.Matches", "Played");
        }
    }
}
