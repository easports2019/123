namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_table_rents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SimplePlaceId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false, precision: 0, storeType: "datetime2"),
                        DurationMinutes = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        UserProfileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: false)
                .ForeignKey("dbo.SimplePlaces", t => t.SimplePlaceId, cascadeDelete: false)
                .Index(t => t.SimplePlaceId)
                .Index(t => t.UserProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rents", "SimplePlaceId", "dbo.SimplePlaces");
            DropForeignKey("dbo.Rents", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.Rents", new[] { "UserProfileId" });
            DropIndex("dbo.Rents", new[] { "SimplePlaceId" });
            DropTable("dbo.Rents");
        }
    }
}
