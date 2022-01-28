namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_table_simpleusermessage2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SimpleUserMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Watched = c.Boolean(nullable: false),
                        When = c.DateTime(nullable: false, precision: 0, storeType: "datetime2"),
                        ToUserProfileId = c.Int(nullable: false),
                        FromUserProfileId = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.FromUserProfileId, cascadeDelete: false)
                .ForeignKey("dbo.UserProfiles", t => t.ToUserProfileId, cascadeDelete: false)
                .Index(t => t.ToUserProfileId)
                .Index(t => t.FromUserProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SimpleUserMessages", "ToUserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.SimpleUserMessages", "FromUserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.SimpleUserMessages", new[] { "FromUserProfileId" });
            DropIndex("dbo.SimpleUserMessages", new[] { "ToUserProfileId" });
            DropTable("dbo.SimpleUserMessages");
        }
    }
}
