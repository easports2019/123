namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addlink2nameandlink2fieldstotournamentanduserprofile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "Link2", c => c.String());
            AddColumn("dbo.UserProfiles", "Link2Name", c => c.String());
            AddColumn("dbo.UserProfiles", "OrganizatorName", c => c.String());
            AddColumn("dbo.UserProfiles", "OrganizatorNameShort", c => c.String());
            AddColumn("dbo.Tournaments", "Link2Name", c => c.String());
            AddColumn("dbo.Tournaments", "OrganizatorNameShort", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "OrganizatorNameShort");
            DropColumn("dbo.Tournaments", "Link2Name");
            DropColumn("dbo.UserProfiles", "OrganizatorNameShort");
            DropColumn("dbo.UserProfiles", "OrganizatorName");
            DropColumn("dbo.UserProfiles", "Link2Name");
            DropColumn("dbo.UserProfiles", "Link2");
        }
    }
}
