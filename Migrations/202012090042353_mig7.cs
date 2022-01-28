namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cities", "CityVkId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cities", "CityVkId");
        }
    }
}
