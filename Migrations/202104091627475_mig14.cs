namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig14 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MatchEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        MatchId = c.Int(nullable: false),
                        Value = c.String(),
                        MemberId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MatchId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Picture = c.String(),
                        When = c.DateTime(nullable: false),
                        PlaceId = c.Int(nullable: false),
                        Team1Id = c.Int(nullable: false),
                        Team2Id = c.Int(nullable: false),
                        Team1Goals = c.Int(nullable: false),
                        Team2Goals = c.Int(nullable: false),
                        Team_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Places", t => t.PlaceId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team_Id)
                .ForeignKey("dbo.Teams", t => t.Team1Id, cascadeDelete: false)
                .ForeignKey("dbo.Teams", t => t.Team2Id, cascadeDelete: false)
                .Index(t => t.PlaceId)
                .Index(t => t.Team1Id)
                .Index(t => t.Team2Id)
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Year = c.Int(nullable: false),
                        WhenBorn = c.DateTime(nullable: false),
                        Details = c.String(),
                        Logo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TournamentGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TournamentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.TournamentId);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Year = c.Int(nullable: false),
                        WhenBegin = c.DateTime(nullable: false),
                        WhenEnd = c.DateTime(nullable: false),
                        Details = c.String(),
                        Reglament = c.String(),
                        Logo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TournamentGroupTeams",
                c => new
                    {
                        TournamentGroup_Id = c.Int(nullable: false),
                        Team_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TournamentGroup_Id, t.Team_Id })
                .ForeignKey("dbo.TournamentGroups", t => t.TournamentGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .Index(t => t.TournamentGroup_Id)
                .Index(t => t.Team_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MatchEvents", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Matches", "Team2Id", "dbo.Teams");
            DropForeignKey("dbo.Matches", "Team1Id", "dbo.Teams");
            DropForeignKey("dbo.TournamentGroups", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentGroupTeams", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.TournamentGroupTeams", "TournamentGroup_Id", "dbo.TournamentGroups");
            DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Matches", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.MatchEvents", "MatchId", "dbo.Matches");
            DropIndex("dbo.TournamentGroupTeams", new[] { "Team_Id" });
            DropIndex("dbo.TournamentGroupTeams", new[] { "TournamentGroup_Id" });
            DropIndex("dbo.TournamentGroups", new[] { "TournamentId" });
            DropIndex("dbo.Matches", new[] { "Team_Id" });
            DropIndex("dbo.Matches", new[] { "Team2Id" });
            DropIndex("dbo.Matches", new[] { "Team1Id" });
            DropIndex("dbo.Matches", new[] { "PlaceId" });
            DropIndex("dbo.MatchEvents", new[] { "MemberId" });
            DropIndex("dbo.MatchEvents", new[] { "MatchId" });
            DropTable("dbo.TournamentGroupTeams");
            DropTable("dbo.Tournaments");
            DropTable("dbo.TournamentGroups");
            DropTable("dbo.Teams");
            DropTable("dbo.Matches");
            DropTable("dbo.MatchEvents");
        }
    }
}
