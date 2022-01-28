namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Collects", "Access_AccessTypeId", "dbo.AccessTypes");
            DropForeignKey("dbo.MemberGroups", "Access_AccessTypeId", "dbo.AccessTypes");
            DropIndex("dbo.Collects", new[] { "Access_AccessTypeId" });
            DropIndex("dbo.MemberGroups", new[] { "Access_AccessTypeId" });
            AddColumn("dbo.Collects", "Access", c => c.Int(nullable: false));
            AddColumn("dbo.Collects", "ErrorMessage", c => c.String());
            AddColumn("dbo.MemberGroups", "Access", c => c.Int(nullable: false));
            DropColumn("dbo.Collects", "Access_AccessTypeId");
            DropColumn("dbo.MemberGroups", "Access_AccessTypeId");
            DropTable("dbo.AccessTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AccessTypes",
                c => new
                    {
                        AccessTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.AccessTypeId);
            
            AddColumn("dbo.MemberGroups", "Access_AccessTypeId", c => c.Int());
            AddColumn("dbo.Collects", "Access_AccessTypeId", c => c.Int());
            DropColumn("dbo.MemberGroups", "Access");
            DropColumn("dbo.Collects", "ErrorMessage");
            DropColumn("dbo.Collects", "Access");
            CreateIndex("dbo.MemberGroups", "Access_AccessTypeId");
            CreateIndex("dbo.Collects", "Access_AccessTypeId");
            AddForeignKey("dbo.MemberGroups", "Access_AccessTypeId", "dbo.AccessTypes", "AccessTypeId");
            AddForeignKey("dbo.Collects", "Access_AccessTypeId", "dbo.AccessTypes", "AccessTypeId");
        }
    }
}
