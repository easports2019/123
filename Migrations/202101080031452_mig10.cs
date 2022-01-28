namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "PhotoPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "PhotoPath");
        }
    }
}
