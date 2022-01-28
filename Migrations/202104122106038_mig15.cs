namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig15 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerId = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        AmpluaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlayerId)
                .ForeignKey("dbo.Ampluas", t => t.AmpluaId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MemberId)
                .Index(t => t.AmpluaId);
            
            CreateTable(
                "dbo.TeamPlayers",
                c => new
                    {
                        Team_Id = c.Int(nullable: false),
                        Player_PlayerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Team_Id, t.Player_PlayerId })
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.Player_PlayerId, cascadeDelete: true)
                .Index(t => t.Team_Id)
                .Index(t => t.Player_PlayerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "MemberId", "dbo.Members");
            DropForeignKey("dbo.TeamPlayers", "Player_PlayerId", "dbo.Players");
            DropForeignKey("dbo.TeamPlayers", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Players", "AmpluaId", "dbo.Ampluas");
            DropIndex("dbo.TeamPlayers", new[] { "Player_PlayerId" });
            DropIndex("dbo.TeamPlayers", new[] { "Team_Id" });
            DropIndex("dbo.Players", new[] { "AmpluaId" });
            DropIndex("dbo.Players", new[] { "MemberId" });
            DropTable("dbo.TeamPlayers");
            DropTable("dbo.Players");
        }
    }
}
