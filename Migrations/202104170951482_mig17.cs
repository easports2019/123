namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tournaments", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tournaments", "CityId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tournaments", "CityId");
            AddForeignKey("dbo.Tournaments", "CityId", "dbo.Cities", "CityId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "CityId", "dbo.Cities");
            DropIndex("dbo.Tournaments", new[] { "CityId" });
            DropColumn("dbo.Tournaments", "CityId");
            DropColumn("dbo.Tournaments", "Deleted");
            DropColumn("dbo.Tournaments", "Published");
        }
    }
}
