namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_comment_fields_to_simplemember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimpleMembers", "Comment", c => c.String());
            AddColumn("dbo.SimpleMembers", "Comment2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimpleMembers", "Comment2");
            DropColumn("dbo.SimpleMembers", "Comment");
        }
    }
}
