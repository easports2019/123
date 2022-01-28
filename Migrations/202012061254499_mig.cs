namespace osbackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ampluas",
                c => new
                    {
                        AmpluaId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.AmpluaId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        GeoPosition = c.String(),
                        Population = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "dbo.Collects",
                c => new
                    {
                        CollectId = c.Int(nullable: false, identity: true),
                        WhenDate = c.DateTime(nullable: false),
                        Hour = c.Int(nullable: false),
                        Minute = c.Int(nullable: false),
                        DurationMinutes = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        FixedPriceByMember = c.Int(nullable: false),
                        Permanent = c.Boolean(nullable: false),
                        OrganizatorIsMember = c.Boolean(nullable: false),
                        AcceptedByPlaceOwner = c.Boolean(nullable: false),
                        Access_AccessTypeId = c.Int(),
                        Organizer_UserProfileId = c.Int(),
                        Place_PlaceId = c.Int(),
                    })
                .PrimaryKey(t => t.CollectId)
                .ForeignKey("dbo.AccessTypes", t => t.Access_AccessTypeId)
                .ForeignKey("dbo.UserProfiles", t => t.Organizer_UserProfileId)
                .ForeignKey("dbo.Places", t => t.Place_PlaceId)
                .Index(t => t.Access_AccessTypeId)
                .Index(t => t.Organizer_UserProfileId)
                .Index(t => t.Place_PlaceId);
            
            CreateTable(
                "dbo.AccessTypes",
                c => new
                    {
                        AccessTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.AccessTypeId);
            
            CreateTable(
                "dbo.MemberGroups",
                c => new
                    {
                        MemberGroupId = c.Int(nullable: false, identity: true),
                        NumberOf = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        Access_AccessTypeId = c.Int(),
                        Amplua_AmpluaId = c.Int(),
                        Collect_CollectId = c.Int(),
                    })
                .PrimaryKey(t => t.MemberGroupId)
                .ForeignKey("dbo.AccessTypes", t => t.Access_AccessTypeId)
                .ForeignKey("dbo.Ampluas", t => t.Amplua_AmpluaId)
                .ForeignKey("dbo.Collects", t => t.Collect_CollectId)
                .Index(t => t.Access_AccessTypeId)
                .Index(t => t.Amplua_AmpluaId)
                .Index(t => t.Collect_CollectId);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        MemberId = c.Int(nullable: false, identity: true),
                        Payment_PaymentId = c.Int(),
                        User_UserProfileId = c.Int(),
                        MemberGroup_MemberGroupId = c.Int(),
                    })
                .PrimaryKey(t => t.MemberId)
                .ForeignKey("dbo.Payments", t => t.Payment_PaymentId)
                .ForeignKey("dbo.UserProfiles", t => t.User_UserProfileId)
                .ForeignKey("dbo.MemberGroups", t => t.MemberGroup_MemberGroupId)
                .Index(t => t.Payment_PaymentId)
                .Index(t => t.User_UserProfileId)
                .Index(t => t.MemberGroup_MemberGroupId);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        OptionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.Boolean(nullable: false),
                        Comment = c.String(),
                        Member_MemberId = c.Int(),
                        Collect_CollectId = c.Int(),
                        NotMember_NotMemberId = c.Int(),
                    })
                .PrimaryKey(t => t.OptionId)
                .ForeignKey("dbo.Members", t => t.Member_MemberId)
                .ForeignKey("dbo.Collects", t => t.Collect_CollectId)
                .ForeignKey("dbo.NotMembers", t => t.NotMember_NotMemberId)
                .Index(t => t.Member_MemberId)
                .Index(t => t.Collect_CollectId)
                .Index(t => t.NotMember_NotMemberId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.PaymentId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        UserVkId = c.String(),
                        Name = c.String(),
                        Surname = c.String(),
                        Fathername = c.String(),
                        Birth = c.DateTime(nullable: false),
                        Register = c.DateTime(nullable: false),
                        LastOnline = c.DateTime(nullable: false),
                        Height = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        LegId = c.Int(nullable: false),
                        FavouriteAmpluaId = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                        LastIp = c.String(),
                        FavouriteAmplua_AmpluaId = c.Int(),
                    })
                .PrimaryKey(t => t.UserProfileId)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.Ampluas", t => t.FavouriteAmplua_AmpluaId)
                .ForeignKey("dbo.Legs", t => t.LegId, cascadeDelete: true)
                .Index(t => t.CityId)
                .Index(t => t.LegId)
                .Index(t => t.FavouriteAmplua_AmpluaId);
            
            CreateTable(
                "dbo.Legs",
                c => new
                    {
                        LegId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.LegId);
            
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        PlaceId = c.Int(nullable: false, identity: true),
                        PlaceName = c.String(),
                        PlaceInfo = c.String(),
                        Geo = c.String(),
                        MainPicture = c.String(),
                        Stages = c.Int(nullable: false),
                        Parking = c.Boolean(nullable: false),
                        BicycleParking = c.Boolean(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        Address_AddressId = c.Int(),
                        City_CityId = c.Int(),
                        Owner_OwnerId = c.Int(),
                    })
                .PrimaryKey(t => t.PlaceId)
                .ForeignKey("dbo.Addresses", t => t.Address_AddressId)
                .ForeignKey("dbo.Cities", t => t.City_CityId)
                .ForeignKey("dbo.Owners", t => t.Owner_OwnerId)
                .Index(t => t.Address_AddressId)
                .Index(t => t.City_CityId)
                .Index(t => t.Owner_OwnerId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Street = c.String(),
                        Index = c.String(),
                        SubjectType = c.String(),
                        House = c.String(),
                    })
                .PrimaryKey(t => t.AddressId);
            
            CreateTable(
                "dbo.Areas",
                c => new
                    {
                        AreaId = c.Int(nullable: false, identity: true),
                        AreaName = c.String(),
                        Length = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        CapacitySport = c.Int(nullable: false),
                        CapacityViewers = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        Place_PlaceId = c.Int(),
                    })
                .PrimaryKey(t => t.AreaId)
                .ForeignKey("dbo.Places", t => t.Place_PlaceId)
                .Index(t => t.Place_PlaceId);
            
            CreateTable(
                "dbo.DressingRooms",
                c => new
                    {
                        DressingRoomId = c.Int(nullable: false, identity: true),
                        RoomNumber = c.String(),
                        Shower = c.Int(nullable: false),
                        HotWater = c.Boolean(nullable: false),
                        Place_PlaceId = c.Int(),
                    })
                .PrimaryKey(t => t.DressingRoomId)
                .ForeignKey("dbo.Places", t => t.Place_PlaceId)
                .Index(t => t.Place_PlaceId);
            
            CreateTable(
                "dbo.Owners",
                c => new
                    {
                        OwnerId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.OwnerId);
            
            CreateTable(
                "dbo.Worktimes",
                c => new
                    {
                        WorktimeId = c.Int(nullable: false, identity: true),
                        FromTime = c.DateTime(nullable: false),
                        ToTime = c.DateTime(nullable: false),
                        Works24 = c.Boolean(nullable: false),
                        Place_PlaceId = c.Int(),
                    })
                .PrimaryKey(t => t.WorktimeId)
                .ForeignKey("dbo.Places", t => t.Place_PlaceId)
                .Index(t => t.Place_PlaceId);
            
            CreateTable(
                "dbo.Breaks",
                c => new
                    {
                        BreakId = c.Int(nullable: false, identity: true),
                        FromTime = c.DateTime(nullable: false),
                        ToTime = c.DateTime(nullable: false),
                        Worktime_WorktimeId = c.Int(),
                    })
                .PrimaryKey(t => t.BreakId)
                .ForeignKey("dbo.Worktimes", t => t.Worktime_WorktimeId)
                .Index(t => t.Worktime_WorktimeId);
            
            CreateTable(
                "dbo.NotMembers",
                c => new
                    {
                        NotMemberId = c.Int(nullable: false, identity: true),
                        ToBe_AmpluaId = c.Int(),
                        User_UserProfileId = c.Int(),
                        Collect_CollectId = c.Int(),
                        Collect_CollectId1 = c.Int(),
                    })
                .PrimaryKey(t => t.NotMemberId)
                .ForeignKey("dbo.Ampluas", t => t.ToBe_AmpluaId)
                .ForeignKey("dbo.UserProfiles", t => t.User_UserProfileId)
                .ForeignKey("dbo.Collects", t => t.Collect_CollectId)
                .ForeignKey("dbo.Collects", t => t.Collect_CollectId1)
                .Index(t => t.ToBe_AmpluaId)
                .Index(t => t.User_UserProfileId)
                .Index(t => t.Collect_CollectId)
                .Index(t => t.Collect_CollectId1);
            
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        SystemSettingsId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.SystemSettingsId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotMembers", "Collect_CollectId1", "dbo.Collects");
            DropForeignKey("dbo.NotMembers", "Collect_CollectId", "dbo.Collects");
            DropForeignKey("dbo.NotMembers", "User_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.NotMembers", "ToBe_AmpluaId", "dbo.Ampluas");
            DropForeignKey("dbo.Options", "NotMember_NotMemberId", "dbo.NotMembers");
            DropForeignKey("dbo.Collects", "Place_PlaceId", "dbo.Places");
            DropForeignKey("dbo.Worktimes", "Place_PlaceId", "dbo.Places");
            DropForeignKey("dbo.Breaks", "Worktime_WorktimeId", "dbo.Worktimes");
            DropForeignKey("dbo.Places", "Owner_OwnerId", "dbo.Owners");
            DropForeignKey("dbo.DressingRooms", "Place_PlaceId", "dbo.Places");
            DropForeignKey("dbo.Places", "City_CityId", "dbo.Cities");
            DropForeignKey("dbo.Areas", "Place_PlaceId", "dbo.Places");
            DropForeignKey("dbo.Places", "Address_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Collects", "Organizer_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Options", "Collect_CollectId", "dbo.Collects");
            DropForeignKey("dbo.MemberGroups", "Collect_CollectId", "dbo.Collects");
            DropForeignKey("dbo.Members", "MemberGroup_MemberGroupId", "dbo.MemberGroups");
            DropForeignKey("dbo.Members", "User_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "LegId", "dbo.Legs");
            DropForeignKey("dbo.UserProfiles", "FavouriteAmplua_AmpluaId", "dbo.Ampluas");
            DropForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Members", "Payment_PaymentId", "dbo.Payments");
            DropForeignKey("dbo.Options", "Member_MemberId", "dbo.Members");
            DropForeignKey("dbo.MemberGroups", "Amplua_AmpluaId", "dbo.Ampluas");
            DropForeignKey("dbo.MemberGroups", "Access_AccessTypeId", "dbo.AccessTypes");
            DropForeignKey("dbo.Collects", "Access_AccessTypeId", "dbo.AccessTypes");
            DropIndex("dbo.NotMembers", new[] { "Collect_CollectId1" });
            DropIndex("dbo.NotMembers", new[] { "Collect_CollectId" });
            DropIndex("dbo.NotMembers", new[] { "User_UserProfileId" });
            DropIndex("dbo.NotMembers", new[] { "ToBe_AmpluaId" });
            DropIndex("dbo.Breaks", new[] { "Worktime_WorktimeId" });
            DropIndex("dbo.Worktimes", new[] { "Place_PlaceId" });
            DropIndex("dbo.DressingRooms", new[] { "Place_PlaceId" });
            DropIndex("dbo.Areas", new[] { "Place_PlaceId" });
            DropIndex("dbo.Places", new[] { "Owner_OwnerId" });
            DropIndex("dbo.Places", new[] { "City_CityId" });
            DropIndex("dbo.Places", new[] { "Address_AddressId" });
            DropIndex("dbo.UserProfiles", new[] { "FavouriteAmplua_AmpluaId" });
            DropIndex("dbo.UserProfiles", new[] { "LegId" });
            DropIndex("dbo.UserProfiles", new[] { "CityId" });
            DropIndex("dbo.Options", new[] { "NotMember_NotMemberId" });
            DropIndex("dbo.Options", new[] { "Collect_CollectId" });
            DropIndex("dbo.Options", new[] { "Member_MemberId" });
            DropIndex("dbo.Members", new[] { "MemberGroup_MemberGroupId" });
            DropIndex("dbo.Members", new[] { "User_UserProfileId" });
            DropIndex("dbo.Members", new[] { "Payment_PaymentId" });
            DropIndex("dbo.MemberGroups", new[] { "Collect_CollectId" });
            DropIndex("dbo.MemberGroups", new[] { "Amplua_AmpluaId" });
            DropIndex("dbo.MemberGroups", new[] { "Access_AccessTypeId" });
            DropIndex("dbo.Collects", new[] { "Place_PlaceId" });
            DropIndex("dbo.Collects", new[] { "Organizer_UserProfileId" });
            DropIndex("dbo.Collects", new[] { "Access_AccessTypeId" });
            DropTable("dbo.SystemSettings");
            DropTable("dbo.NotMembers");
            DropTable("dbo.Breaks");
            DropTable("dbo.Worktimes");
            DropTable("dbo.Owners");
            DropTable("dbo.DressingRooms");
            DropTable("dbo.Areas");
            DropTable("dbo.Addresses");
            DropTable("dbo.Places");
            DropTable("dbo.Legs");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.Payments");
            DropTable("dbo.Options");
            DropTable("dbo.Members");
            DropTable("dbo.MemberGroups");
            DropTable("dbo.AccessTypes");
            DropTable("dbo.Collects");
            DropTable("dbo.Cities");
            DropTable("dbo.Ampluas");
        }
    }
}
