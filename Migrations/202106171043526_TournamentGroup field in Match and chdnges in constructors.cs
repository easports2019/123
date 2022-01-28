namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TournamentGroupfieldinMatchandchdngesinconstructors : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.Matches", new[] { "TournamentId" });
            AddColumn("dbo.Matches", "TournamentGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Matches", "TournamentGroupId");
            AddForeignKey("dbo.Matches", "TournamentGroupId", "dbo.TournamentGroups", "Id", cascadeDelete: true);
            DropColumn("dbo.Matches", "TournamentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "TournamentId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Matches", "TournamentGroupId", "dbo.TournamentGroups");
            DropIndex("dbo.Matches", new[] { "TournamentGroupId" });
            DropColumn("dbo.Matches", "TournamentGroupId");
            CreateIndex("dbo.Matches", "TournamentId");
            AddForeignKey("dbo.Matches", "TournamentId", "dbo.Tournaments", "Id", cascadeDelete: true);
        }
    }
}
