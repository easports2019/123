namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adminfieldsteamandtournamentsetnullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Admins", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.Admins", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.Admins", new[] { "TournamentId" });
            DropIndex("dbo.Admins", new[] { "TeamId" });
            AlterColumn("dbo.Admins", "TournamentId", c => c.Int());
            AlterColumn("dbo.Admins", "TeamId", c => c.Int());
            CreateIndex("dbo.Admins", "TournamentId");
            CreateIndex("dbo.Admins", "TeamId");
            AddForeignKey("dbo.Admins", "TeamId", "dbo.Teams", "Id");
            AddForeignKey("dbo.Admins", "TournamentId", "dbo.Tournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Admins", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Admins", "TeamId", "dbo.Teams");
            DropIndex("dbo.Admins", new[] { "TeamId" });
            DropIndex("dbo.Admins", new[] { "TournamentId" });
            AlterColumn("dbo.Admins", "TeamId", c => c.Int(nullable: false));
            AlterColumn("dbo.Admins", "TournamentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Admins", "TeamId");
            CreateIndex("dbo.Admins", "TournamentId");
            AddForeignKey("dbo.Admins", "TournamentId", "dbo.Tournaments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Admins", "TeamId", "dbo.Teams", "Id", cascadeDelete: true);
        }
    }
}
