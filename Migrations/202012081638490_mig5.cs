namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ampluas", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cities", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.Legs", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Legs", "IsDefault");
            DropColumn("dbo.Cities", "IsDefault");
            DropColumn("dbo.Ampluas", "IsDefault");
        }
    }
}
