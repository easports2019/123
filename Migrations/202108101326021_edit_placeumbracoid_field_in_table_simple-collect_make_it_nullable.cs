namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit_placeumbracoid_field_in_table_simplecollect_make_it_nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SimpleCollects", "PlaceUmbracoId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SimpleCollects", "PlaceUmbracoId", c => c.Int(nullable: false));
        }
    }
}
