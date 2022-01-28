namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admins", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Admins", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AdminTypes", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.AdminTypes", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Teams", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Teams", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Matches", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Matches", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.MatchEvents", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.MatchEvents", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Members", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Members", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Options", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Options", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Payments", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Payments", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Players", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Players", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Ampluas", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Ampluas", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfiles", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfiles", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Places", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Places", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Areas", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Areas", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cities", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cities", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.TournamentGroups", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.TournamentGroups", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.DressingRooms", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.DressingRooms", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owners", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owners", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Worktimes", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Worktimes", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Breaks", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Breaks", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.BidPlayerToTeams", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.BidPlayerToTeams", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.BidTeamToTournaments", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.BidTeamToTournaments", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Collects", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Collects", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.MemberGroups", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.MemberGroups", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.NotMembers", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.NotMembers", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Legs", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Legs", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Legs", "Deleted");
            DropColumn("dbo.Legs", "Published");
            DropColumn("dbo.NotMembers", "Deleted");
            DropColumn("dbo.NotMembers", "Published");
            DropColumn("dbo.MemberGroups", "Deleted");
            DropColumn("dbo.MemberGroups", "Published");
            DropColumn("dbo.Collects", "Deleted");
            DropColumn("dbo.Collects", "Published");
            DropColumn("dbo.BidTeamToTournaments", "Deleted");
            DropColumn("dbo.BidTeamToTournaments", "Published");
            DropColumn("dbo.BidPlayerToTeams", "Deleted");
            DropColumn("dbo.BidPlayerToTeams", "Published");
            DropColumn("dbo.Breaks", "Deleted");
            DropColumn("dbo.Breaks", "Published");
            DropColumn("dbo.Worktimes", "Deleted");
            DropColumn("dbo.Worktimes", "Published");
            DropColumn("dbo.Owners", "Deleted");
            DropColumn("dbo.Owners", "Published");
            DropColumn("dbo.DressingRooms", "Deleted");
            DropColumn("dbo.DressingRooms", "Published");
            DropColumn("dbo.TournamentGroups", "Deleted");
            DropColumn("dbo.TournamentGroups", "Published");
            DropColumn("dbo.Cities", "Deleted");
            DropColumn("dbo.Cities", "Published");
            DropColumn("dbo.Areas", "Deleted");
            DropColumn("dbo.Areas", "Published");
            DropColumn("dbo.Addresses", "Deleted");
            DropColumn("dbo.Addresses", "Published");
            DropColumn("dbo.Places", "Deleted");
            DropColumn("dbo.Places", "Published");
            DropColumn("dbo.UserProfiles", "Deleted");
            DropColumn("dbo.UserProfiles", "Published");
            DropColumn("dbo.Ampluas", "Deleted");
            DropColumn("dbo.Ampluas", "Published");
            DropColumn("dbo.Players", "Deleted");
            DropColumn("dbo.Players", "Published");
            DropColumn("dbo.Payments", "Deleted");
            DropColumn("dbo.Payments", "Published");
            DropColumn("dbo.Options", "Deleted");
            DropColumn("dbo.Options", "Published");
            DropColumn("dbo.Members", "Deleted");
            DropColumn("dbo.Members", "Published");
            DropColumn("dbo.MatchEvents", "Deleted");
            DropColumn("dbo.MatchEvents", "Published");
            DropColumn("dbo.Matches", "Deleted");
            DropColumn("dbo.Matches", "Published");
            DropColumn("dbo.Teams", "Deleted");
            DropColumn("dbo.Teams", "Published");
            DropColumn("dbo.AdminTypes", "Deleted");
            DropColumn("dbo.AdminTypes", "Published");
            DropColumn("dbo.Admins", "Deleted");
            DropColumn("dbo.Admins", "Published");
        }
    }
}
