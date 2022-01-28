namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_basemodel_fields_tosimplecollect_simpleplace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimpleCollects", "ErrorMessage", c => c.String());
            AddColumn("dbo.SimpleCollects", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.SimpleCollects", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SimpleMembers", "ErrorMessage", c => c.String());
            AddColumn("dbo.SimpleMembers", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.SimpleMembers", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimpleMembers", "Deleted");
            DropColumn("dbo.SimpleMembers", "Published");
            DropColumn("dbo.SimpleMembers", "ErrorMessage");
            DropColumn("dbo.SimpleCollects", "Deleted");
            DropColumn("dbo.SimpleCollects", "Published");
            DropColumn("dbo.SimpleCollects", "ErrorMessage");
        }
    }
}
