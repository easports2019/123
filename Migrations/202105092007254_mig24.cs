namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig24 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.BidTeamToTournaments", "TeamId");
            AddForeignKey("dbo.BidTeamToTournaments", "TeamId", "dbo.Teams", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BidTeamToTournaments", "TeamId", "dbo.Teams");
            DropIndex("dbo.BidTeamToTournaments", new[] { "TeamId" });
        }
    }
}
