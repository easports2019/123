namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cities", "CityUmbracoName", c => c.String());
            AddColumn("dbo.Cities", "ErrorMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cities", "ErrorMessage");
            DropColumn("dbo.Cities", "CityUmbracoName");
        }
    }
}
