namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delete_fields_userprofile_in_simplecollect : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SimpleCollects", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "SimpleCollect_Id", "dbo.SimpleCollects");
            DropForeignKey("dbo.SimpleCollects", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.UserProfiles", new[] { "SimpleCollect_Id" });
            DropIndex("dbo.SimpleCollects", new[] { "UserProfileId" });
            DropIndex("dbo.SimpleCollects", new[] { "UserProfile_UserProfileId" });
            CreateTable(
                "dbo.SimpleMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SimpleMemberTypeName = c.String(),
                        UserProfileId = c.Int(nullable: false),
                        SimpleCollectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SimpleCollects", t => t.SimpleCollectId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId)
                .Index(t => t.SimpleCollectId);
            
            CreateTable(
                "dbo.SimplePlaces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Info = c.String(),
                        SimpleCityId = c.Int(nullable: false),
                        Address = c.String(),
                        Geo = c.String(),
                        MainPicture = c.String(),
                        Parking = c.Boolean(nullable: false),
                        BicycleParking = c.Boolean(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        ErrorMessage = c.String(),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SimpleCities", t => t.SimpleCityId, cascadeDelete: true)
                .Index(t => t.SimpleCityId);
            
            CreateTable(
                "dbo.SimpleCities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Info = c.String(),
                        Geo = c.String(),
                        MainPicture = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        ErrorMessage = c.String(),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.SimpleCollects", "SimplePlaceId", c => c.Int(nullable: false));
            AlterColumn("dbo.SimpleCollects", "When", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            CreateIndex("dbo.SimpleCollects", "SimplePlaceId");
            AddForeignKey("dbo.SimpleCollects", "SimplePlaceId", "dbo.SimplePlaces", "Id", cascadeDelete: true);
            DropColumn("dbo.UserProfiles", "SimpleCollect_Id");
            DropColumn("dbo.SimpleCollects", "PlaceUmbracoId");
            DropColumn("dbo.SimpleCollects", "UserProfileId");
            DropColumn("dbo.SimpleCollects", "UserProfile_UserProfileId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SimpleCollects", "UserProfile_UserProfileId", c => c.Int());
            AddColumn("dbo.SimpleCollects", "UserProfileId", c => c.Int(nullable: false));
            AddColumn("dbo.SimpleCollects", "PlaceUmbracoId", c => c.Int());
            AddColumn("dbo.UserProfiles", "SimpleCollect_Id", c => c.Int());
            DropForeignKey("dbo.SimpleCollects", "SimplePlaceId", "dbo.SimplePlaces");
            DropForeignKey("dbo.SimplePlaces", "SimpleCityId", "dbo.SimpleCities");
            DropForeignKey("dbo.SimpleMembers", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.SimpleMembers", "SimpleCollectId", "dbo.SimpleCollects");
            DropIndex("dbo.SimplePlaces", new[] { "SimpleCityId" });
            DropIndex("dbo.SimpleMembers", new[] { "SimpleCollectId" });
            DropIndex("dbo.SimpleMembers", new[] { "UserProfileId" });
            DropIndex("dbo.SimpleCollects", new[] { "SimplePlaceId" });
            AlterColumn("dbo.SimpleCollects", "When", c => c.DateTime(nullable: false));
            DropColumn("dbo.SimpleCollects", "SimplePlaceId");
            DropTable("dbo.SimpleCities");
            DropTable("dbo.SimplePlaces");
            DropTable("dbo.SimpleMembers");
            CreateIndex("dbo.SimpleCollects", "UserProfile_UserProfileId");
            CreateIndex("dbo.SimpleCollects", "UserProfileId");
            CreateIndex("dbo.UserProfiles", "SimpleCollect_Id");
            AddForeignKey("dbo.SimpleCollects", "UserProfile_UserProfileId", "dbo.UserProfiles", "UserProfileId");
            AddForeignKey("dbo.UserProfiles", "SimpleCollect_Id", "dbo.SimpleCollects", "Id");
            AddForeignKey("dbo.SimpleCollects", "UserProfileId", "dbo.UserProfiles", "UserProfileId", cascadeDelete: true);
        }
    }
}
