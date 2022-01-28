namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_cityumbracoid_to_table_simplecity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimpleCities", "CityUmbracoId", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimpleCities", "CityUmbracoId");
        }
    }
}
