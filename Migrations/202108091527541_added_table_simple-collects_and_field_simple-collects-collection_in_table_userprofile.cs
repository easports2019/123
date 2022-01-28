namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_table_simplecollects_and_field_simplecollectscollection_in_table_userprofile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SimpleCollects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        When = c.DateTime(nullable: false),
                        DurationMinutes = c.Int(nullable: false),
                        Details = c.String(),
                        Comment = c.String(),
                        Cost = c.Int(nullable: false),
                        NeedMembers = c.Int(nullable: false),
                        PlaceId = c.Int(nullable: false),
                        UserProfileId = c.Int(nullable: false),
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Places", t => t.PlaceId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId)
                .Index(t => t.PlaceId)
                .Index(t => t.UserProfileId)
                .Index(t => t.UserProfile_UserProfileId);
            
            AddColumn("dbo.UserProfiles", "SimpleCollect_Id", c => c.Int());
            CreateIndex("dbo.UserProfiles", "SimpleCollect_Id");
            AddForeignKey("dbo.UserProfiles", "SimpleCollect_Id", "dbo.SimpleCollects", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SimpleCollects", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.SimpleCollects", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.UserProfiles", "SimpleCollect_Id", "dbo.SimpleCollects");
            DropForeignKey("dbo.SimpleCollects", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.SimpleCollects", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.SimpleCollects", new[] { "UserProfileId" });
            DropIndex("dbo.SimpleCollects", new[] { "PlaceId" });
            DropIndex("dbo.UserProfiles", new[] { "SimpleCollect_Id" });
            DropColumn("dbo.UserProfiles", "SimpleCollect_Id");
            DropTable("dbo.SimpleCollects");
        }
    }
}
