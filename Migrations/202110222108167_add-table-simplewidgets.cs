namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablesimplewidgets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SimpleWidgets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VKGroupId = c.Int(nullable: false),
                        UserProfileId = c.Int(nullable: false),
                        Token = c.String(),
                        WidgetType = c.String(),
                        ErrorMessage = c.String(),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SimpleWidgets", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.SimpleWidgets", new[] { "UserProfileId" });
            DropTable("dbo.SimpleWidgets");
        }
    }
}
