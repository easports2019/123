namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedfieldUmbracoPlaceIdtoMatchestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "UmbracoPlaceId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "UmbracoPlaceId");
        }
    }
}
