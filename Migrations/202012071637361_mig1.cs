namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserProfiles", "Birth", c => c.DateTime());
            AlterColumn("dbo.UserProfiles", "Register", c => c.DateTime());
            AlterColumn("dbo.UserProfiles", "LastOnline", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserProfiles", "LastOnline", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserProfiles", "Register", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserProfiles", "Birth", c => c.DateTime(nullable: false));
        }
    }
}
