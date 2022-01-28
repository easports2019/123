namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_totalexpirience_to_userprofile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "TotalExpirience", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "TotalExpirience");
        }
    }
}
