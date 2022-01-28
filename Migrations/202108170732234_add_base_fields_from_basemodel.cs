namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_base_fields_from_basemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rents", "ErrorMessage", c => c.String());
            AddColumn("dbo.Rents", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.Rents", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rents", "Deleted");
            DropColumn("dbo.Rents", "Published");
            DropColumn("dbo.Rents", "ErrorMessage");
        }
    }
}
