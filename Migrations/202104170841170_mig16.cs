namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig16 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TeamPlayers", newName: "PlayerTeams");
            DropPrimaryKey("dbo.PlayerTeams");
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        AdminId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserProfileId = c.Int(nullable: false),
                        AdminTypeId = c.Int(nullable: false),
                        TournamentId = c.Int(nullable: false),
                        TeamId = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                    })
                .PrimaryKey(t => t.AdminId)
                .ForeignKey("dbo.AdminTypes", t => t.AdminTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId)
                .Index(t => t.AdminTypeId)
                .Index(t => t.TournamentId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.AdminTypes",
                c => new
                    {
                        AdminTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ErrorMessage = c.String(),
                    })
                .PrimaryKey(t => t.AdminTypeId);
            
            CreateTable(
                "dbo.BidPlayerToTeams",
                c => new
                    {
                        BidPlayerToTeamId = c.Int(nullable: false, identity: true),
                        PlayerId = c.Int(nullable: false),
                        TeamId = c.Int(nullable: false),
                        When = c.DateTime(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        ErrorMessage = c.String(),
                    })
                .PrimaryKey(t => t.BidPlayerToTeamId);
            
            CreateTable(
                "dbo.BidTeamToTournaments",
                c => new
                    {
                        BidTeamToTournamentId = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        TournamentId = c.Int(nullable: false),
                        When = c.DateTime(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        ErrorMessage = c.String(),
                    })
                .PrimaryKey(t => t.BidTeamToTournamentId);
            
            AddColumn("dbo.Ampluas", "ErrorMessage", c => c.String());
            AddColumn("dbo.Players", "ErrorMessage", c => c.String());
            AddColumn("dbo.Members", "ErrorMessage", c => c.String());
            AddColumn("dbo.MatchEvents", "ErrorMessage", c => c.String());
            AddColumn("dbo.Matches", "ErrorMessage", c => c.String());
            AddColumn("dbo.Places", "ErrorMessage", c => c.String());
            AddColumn("dbo.Addresses", "ErrorMessage", c => c.String());
            AddColumn("dbo.Areas", "ErrorMessage", c => c.String());
            AddColumn("dbo.DressingRooms", "ErrorMessage", c => c.String());
            AddColumn("dbo.Owners", "ErrorMessage", c => c.String());
            AddColumn("dbo.Worktimes", "ErrorMessage", c => c.String());
            AddColumn("dbo.Breaks", "ErrorMessage", c => c.String());
            AddColumn("dbo.Teams", "TournamentId", c => c.Int(nullable: false));
            AddColumn("dbo.Teams", "ErrorMessage", c => c.String());
            AddColumn("dbo.TournamentGroups", "ErrorMessage", c => c.String());
            AddColumn("dbo.Tournaments", "ErrorMessage", c => c.String());
            AddColumn("dbo.Options", "ErrorMessage", c => c.String());
            AddColumn("dbo.Payments", "ErrorMessage", c => c.String());
            AddColumn("dbo.MemberGroups", "ErrorMessage", c => c.String());
            AddColumn("dbo.NotMembers", "ErrorMessage", c => c.String());
            AddColumn("dbo.Legs", "ErrorMessage", c => c.String());
            AddPrimaryKey("dbo.PlayerTeams", new[] { "Player_PlayerId", "Team_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Admins", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Admins", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Admins", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.Admins", "AdminTypeId", "dbo.AdminTypes");
            DropIndex("dbo.Admins", new[] { "TeamId" });
            DropIndex("dbo.Admins", new[] { "TournamentId" });
            DropIndex("dbo.Admins", new[] { "AdminTypeId" });
            DropIndex("dbo.Admins", new[] { "UserProfileId" });
            DropPrimaryKey("dbo.PlayerTeams");
            DropColumn("dbo.Legs", "ErrorMessage");
            DropColumn("dbo.NotMembers", "ErrorMessage");
            DropColumn("dbo.MemberGroups", "ErrorMessage");
            DropColumn("dbo.Payments", "ErrorMessage");
            DropColumn("dbo.Options", "ErrorMessage");
            DropColumn("dbo.Tournaments", "ErrorMessage");
            DropColumn("dbo.TournamentGroups", "ErrorMessage");
            DropColumn("dbo.Teams", "ErrorMessage");
            DropColumn("dbo.Teams", "TournamentId");
            DropColumn("dbo.Breaks", "ErrorMessage");
            DropColumn("dbo.Worktimes", "ErrorMessage");
            DropColumn("dbo.Owners", "ErrorMessage");
            DropColumn("dbo.DressingRooms", "ErrorMessage");
            DropColumn("dbo.Areas", "ErrorMessage");
            DropColumn("dbo.Addresses", "ErrorMessage");
            DropColumn("dbo.Places", "ErrorMessage");
            DropColumn("dbo.Matches", "ErrorMessage");
            DropColumn("dbo.MatchEvents", "ErrorMessage");
            DropColumn("dbo.Members", "ErrorMessage");
            DropColumn("dbo.Players", "ErrorMessage");
            DropColumn("dbo.Ampluas", "ErrorMessage");
            DropTable("dbo.BidTeamToTournaments");
            DropTable("dbo.BidPlayerToTeams");
            DropTable("dbo.AdminTypes");
            DropTable("dbo.Admins");
            AddPrimaryKey("dbo.PlayerTeams", new[] { "Team_Id", "Player_PlayerId" });
            RenameTable(name: "dbo.PlayerTeams", newName: "TeamPlayers");
        }
    }
}
