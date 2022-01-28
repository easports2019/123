namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserProfiles", "Birth", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.UserProfiles", "Register", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.UserProfiles", "LastOnline", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserProfiles", "LastOnline", c => c.DateTime());
            AlterColumn("dbo.UserProfiles", "Register", c => c.DateTime());
            AlterColumn("dbo.UserProfiles", "Birth", c => c.DateTime());
        }
    }
}
