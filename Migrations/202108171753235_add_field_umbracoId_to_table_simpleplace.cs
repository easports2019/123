namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_umbracoId_to_table_simpleplace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimplePlaces", "UmbracoId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimplePlaces", "UmbracoId");
        }
    }
}
