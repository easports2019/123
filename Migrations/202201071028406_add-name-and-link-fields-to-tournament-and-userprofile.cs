namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnameandlinkfieldstotournamentanduserprofile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "Link", c => c.String());
            AddColumn("dbo.Tournaments", "Link", c => c.String());
            AddColumn("dbo.Tournaments", "Link2", c => c.String());
            AddColumn("dbo.Tournaments", "OrganizatorName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "OrganizatorName");
            DropColumn("dbo.Tournaments", "Link2");
            DropColumn("dbo.Tournaments", "Link");
            DropColumn("dbo.UserProfiles", "Link");
        }
    }
}
