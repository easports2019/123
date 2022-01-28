namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_connected_fields_to_table_Break_and_Worktime : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Breaks", "Worktime_WorktimeId", "dbo.Worktimes");
            DropForeignKey("dbo.Worktimes", "SimplePlace_Id", "dbo.SimplePlaces");
            DropIndex("dbo.Worktimes", new[] { "SimplePlace_Id" });
            DropIndex("dbo.Breaks", new[] { "Worktime_WorktimeId" });
            RenameColumn(table: "dbo.Breaks", name: "Worktime_WorktimeId", newName: "WorktimeId");
            RenameColumn(table: "dbo.Worktimes", name: "SimplePlace_Id", newName: "SimplePlaceId");
            AlterColumn("dbo.Worktimes", "SimplePlaceId", c => c.Int(nullable: false));
            AlterColumn("dbo.Breaks", "WorktimeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Worktimes", "SimplePlaceId");
            CreateIndex("dbo.Breaks", "WorktimeId");
            AddForeignKey("dbo.Breaks", "WorktimeId", "dbo.Worktimes", "WorktimeId", cascadeDelete: true);
            AddForeignKey("dbo.Worktimes", "SimplePlaceId", "dbo.SimplePlaces", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Worktimes", "SimplePlaceId", "dbo.SimplePlaces");
            DropForeignKey("dbo.Breaks", "WorktimeId", "dbo.Worktimes");
            DropIndex("dbo.Breaks", new[] { "WorktimeId" });
            DropIndex("dbo.Worktimes", new[] { "SimplePlaceId" });
            AlterColumn("dbo.Breaks", "WorktimeId", c => c.Int());
            AlterColumn("dbo.Worktimes", "SimplePlaceId", c => c.Int());
            RenameColumn(table: "dbo.Worktimes", name: "SimplePlaceId", newName: "SimplePlace_Id");
            RenameColumn(table: "dbo.Breaks", name: "WorktimeId", newName: "Worktime_WorktimeId");
            CreateIndex("dbo.Breaks", "Worktime_WorktimeId");
            CreateIndex("dbo.Worktimes", "SimplePlace_Id");
            AddForeignKey("dbo.Worktimes", "SimplePlace_Id", "dbo.SimplePlaces", "Id");
            AddForeignKey("dbo.Breaks", "Worktime_WorktimeId", "dbo.Worktimes", "WorktimeId");
        }
    }
}
