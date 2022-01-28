namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "CityUmbracoName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "CityUmbracoName");
        }
    }
}
