namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig21 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Admins");
            DropColumn("dbo.Admins", "AdminId");
            AddColumn("dbo.Admins", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Matches", "TournamentId", c => c.Int(nullable: false));
            AddColumn("dbo.MatchEvents", "Minute", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Admins", "Id");
            CreateIndex("dbo.Matches", "TournamentId");
            AddForeignKey("dbo.Matches", "TournamentId", "dbo.Tournaments", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Admins", "AdminId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Matches", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.Matches", new[] { "TournamentId" });
            DropPrimaryKey("dbo.Admins");
            DropColumn("dbo.MatchEvents", "Minute");
            DropColumn("dbo.Matches", "TournamentId");
            DropColumn("dbo.Admins", "Id");
            AddPrimaryKey("dbo.Admins", "AdminId");
        }
    }
}
