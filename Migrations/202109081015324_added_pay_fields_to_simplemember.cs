namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_pay_fields_to_simplemember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimpleMembers", "PayCash", c => c.Int(nullable: false));
            AddColumn("dbo.SimpleMembers", "PayCard", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimpleMembers", "PayCard");
            DropColumn("dbo.SimpleMembers", "PayCash");
        }
    }
}
