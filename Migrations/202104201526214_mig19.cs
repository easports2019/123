namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "CityId", c => c.Int(nullable: false));
            AlterColumn("dbo.Teams", "WhenBorn", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.Matches", "When", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.Tournaments", "WhenBegin", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.Tournaments", "WhenEnd", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.Breaks", "FromTime", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.Breaks", "ToTime", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.BidPlayerToTeams", "When", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.BidTeamToTournaments", "When", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.Collects", "WhenDate", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            CreateIndex("dbo.Teams", "CityId");
            AddForeignKey("dbo.Teams", "CityId", "dbo.Cities", "CityId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teams", "CityId", "dbo.Cities");
            DropIndex("dbo.Teams", new[] { "CityId" });
            AlterColumn("dbo.Collects", "WhenDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.BidTeamToTournaments", "When", c => c.DateTime(nullable: false));
            AlterColumn("dbo.BidPlayerToTeams", "When", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Breaks", "ToTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Breaks", "FromTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tournaments", "WhenEnd", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tournaments", "WhenBegin", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Matches", "When", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Teams", "WhenBorn", c => c.DateTime(nullable: false));
            DropColumn("dbo.Teams", "CityId");
        }
    }
}
