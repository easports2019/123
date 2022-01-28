namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ampluas", "AmpluaUmbracoId", c => c.Int());
            AddColumn("dbo.Cities", "CityUmbracoId", c => c.Int());
            AddColumn("dbo.Legs", "LegUmbracoId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Legs", "LegUmbracoId");
            DropColumn("dbo.Cities", "CityUmbracoId");
            DropColumn("dbo.Ampluas", "AmpluaUmbracoId");
        }
    }
}
