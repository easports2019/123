namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_fullprice_and_change_datatype_cost_in_simplecollect : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimpleCollects", "FullPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SimpleCollects", "Cost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SimpleCollects", "Cost", c => c.Int(nullable: false));
            DropColumn("dbo.SimpleCollects", "FullPrice");
        }
    }
}
