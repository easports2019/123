namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_tables_simpleplace_simplecity_simplemember_and_mod_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimpleCollects", "CreatorId", c => c.Int(nullable: false));
            CreateIndex("dbo.SimpleCollects", "CreatorId");
            AddForeignKey("dbo.SimpleCollects", "CreatorId", "dbo.UserProfiles", "UserProfileId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SimpleCollects", "CreatorId", "dbo.UserProfiles");
            DropIndex("dbo.SimpleCollects", new[] { "CreatorId" });
            DropColumn("dbo.SimpleCollects", "CreatorId");
        }
    }
}
