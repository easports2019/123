namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CityTournamentAdmins", "ErrorMessage", c => c.String());
            AddColumn("dbo.CityTournamentAdmins", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.CityTournamentAdmins", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CityTournamentAdmins", "Deleted");
            DropColumn("dbo.CityTournamentAdmins", "Published");
            DropColumn("dbo.CityTournamentAdmins", "ErrorMessage");
        }
    }
}
