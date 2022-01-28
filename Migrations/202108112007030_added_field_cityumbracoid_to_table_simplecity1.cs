namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_cityumbracoid_to_table_simplecity1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SimpleCities", "CityUmbracoId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SimpleCities", "CityUmbracoId", c => c.Int(nullable: false));
        }
    }
}
