namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_when_costperhour_to_table_worktime_field_worktime_to_simpleplace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Worktimes", "When", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AddColumn("dbo.Worktimes", "CostPerHour", c => c.Int(nullable: false));
            AddColumn("dbo.Worktimes", "SimplePlace_Id", c => c.Int());
            CreateIndex("dbo.Worktimes", "SimplePlace_Id");
            AddForeignKey("dbo.Worktimes", "SimplePlace_Id", "dbo.SimplePlaces", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Worktimes", "SimplePlace_Id", "dbo.SimplePlaces");
            DropIndex("dbo.Worktimes", new[] { "SimplePlace_Id" });
            DropColumn("dbo.Worktimes", "SimplePlace_Id");
            DropColumn("dbo.Worktimes", "CostPerHour");
            DropColumn("dbo.Worktimes", "When");
        }
    }
}
