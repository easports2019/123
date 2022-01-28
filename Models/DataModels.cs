using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace osbackend.Models
{

    public static class CommonSettings
    {
        public static string CurrentDatabase = System.Configuration.ConfigurationManager.AppSettings["CurrentDataBase"];

        


    }

    /// <summary>
    /// основной контекст данных
    /// </summary>
    public class DefaultContext : DbContext
    {
        public DefaultContext()
            : base(CommonSettings.CurrentDatabase) 
        {

        }


        public DbSet<Collect> Collects { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Amplua> Ampluas { get; set; }
        public DbSet<SystemSettings> SystemSettingsItems { get; set; }
        public DbSet<City> Citys { get; set; }
        public DbSet<Leg> Legs { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentGroup> TournamentGroups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminType> AdminTypes { get; set; }
        public DbSet<BidPlayerToTeam> BidPlayerToTeams { get; set; }
        public DbSet<BidTeamToTournament> BidTeamToTournaments { get; set; }
        public DbSet<CityTournamentAdmin> CityTournamentAdmins { get; set; }
        public DbSet<TournamentGroupTableItem> TournamentGroupTableItems { get; set; }
        public DbSet<SimpleCollect> SimpleCollects { get; set; }
        public DbSet<SimpleMember> SimpleMembers { get; set; }
        public DbSet<SimpleCity> SimpleCitys { get; set; }
        public DbSet<SimplePlace> SimplePlaces { get; set; }
        public DbSet<SimpleUserMessage> SimpleUserMessages { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Worktime> Worktimes { get; set; }
        public DbSet<Break> Breaks { get; set; }
        public DbSet<SimpleWidget> SimpleWidgets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>().
                Property(p => p.Birth)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<UserProfile>().
                Property(p => p.LastOnline)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<UserProfile>().
                Property(p => p.Register)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Team>().
                Property(p => p.WhenBorn)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Collect>().
                Property(p => p.WhenDate)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Break>().
                Property(p => p.FromTime)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Break>().
                Property(p => p.ToTime)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Tournament>().
                Property(p => p.WhenBegin)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Tournament>().
                Property(p => p.WhenEnd)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<BidPlayerToTeam>().
                Property(p => p.When)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<BidTeamToTournament>().
                Property(p => p.When)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Match>().
                Property(p => p.When)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<SimpleCollect>().
                Property(p => p.When)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Worktime>().
                Property(p => p.When)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<Rent>().
                Property(p => p.From)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            modelBuilder.Entity<SimpleUserMessage>().
                Property(p => p.When)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();

            //modelBuilder.Entity<Match>()
            //    .HasRequired(c => c.Team2)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);
        }
    }


    /// <summary>
    /// базовый класс модели данных
    /// </summary>
    public class BaseModel
    {
        public string ErrorMessage { get; set; }

        /// <summary>
        /// опубликован
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// удален
        /// </summary>
        public bool Deleted { get; set; }

        public BaseModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public BaseModel() { }
    }


    public class MemberGroup : BaseModel
    {
        public int MemberGroupId { get; set; }
        public Amplua Amplua { get; set; }
        public int NumberOf { get; set; }
        public AccessEnum Access { get; set; }
        public List<Member> Members { get; set; }
        public int Price { get; set; }
    }


    public class UserGroup : BaseModel
    {
        public int UserGroupId { get; set; }
        public Amplua Amplua { get; set; }
        public int NumberOf { get; set; }
        public AccessEnum Access { get; set; }
        public List<Member> Members { get; set; }
        public int Price { get; set; }
    }


    public class Payment : BaseModel
    {
        public int PaymentId { get; set; }
    }

    /// <summary>
    /// сущность любой приглашенный или попросившийся
    /// </summary>
    public class NotMember : BaseModel
    {
        public int NotMemberId { get; set; }
        public UserProfile User { get; set; }
        public Amplua ToBe { get; set; }
        public List<Option> Options { get; set; }
    }

    /// <summary>
    /// Упрощенный участник упрощенного сбора
    /// </summary>
    public class SimpleMember : BaseModel
    {
        public int Id { get; set; }

        public string SimpleMemberTypeName { get; set; }

        /// <summary>
        /// комментарий. например, при удалении из сбора
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// на всякий случай
        /// </summary>
        public string Comment2 { get; set; }

        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public int SimpleCollectId { get; set; }
        public SimpleCollect SimpleCollect { get; set; }

        /// <summary>
        /// Оплатил сумму наличкой
        /// </summary>
        public int PayCash { get; set; }
        
        /// <summary>
        /// Оплатил сумму картой
        /// </summary>
        public int PayCard { get; set; }

        public SimpleMember()
        {
            Published = false;
            Deleted = false;
            ErrorMessage = "";
            Comment = "";
            Comment2 = "";
            PayCash = 0;
            PayCard = 0;
        }

    }


    /// <summary>
    /// сообщения от пользователя к пользователю
    /// </summary>
    public class SimpleUserMessage: BaseModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Watched { get; set; }
        public DateTime When { get; set; }

        /// <summary>
        /// Адресат
        /// </summary>
        public int ToUserProfileId { get; set; }
        [ForeignKey("ToUserProfileId")]
        public UserProfile ToUserProfile { get; set; }

        /// <summary>
        /// Отправитель
        /// </summary>
        public int FromUserProfileId { get; set; }

        [ForeignKey("FromUserProfileId")]
        public UserProfile FromUserProfile { get; set; }

        public SimpleUserMessage()
        {
            Published = false;
            Deleted = false;
            Watched = false;
        }
    }



    /// <summary>
    /// сущность подтвержденный участник сбора
    /// </summary>
    public class Member : BaseModel
    {
        public int MemberId { get; set; }

        /// <summary>
        /// связанный профиль пользователя. если человек не зарегистрирован в вк, тогда в связанном UserProfile поле UserVkId=-1
        /// </summary>
        public UserProfile User { get; set; }

        public Payment Payment { get; set; }

        public List<Option> Options { get; set; }

        public ICollection<MatchEvent> MatchEvents { get; set; }
        public ICollection<Player> Players { get; set; }


        public Member()
        {
            MatchEvents = new List<MatchEvent>();
            Players = new List<Player>();
        }
    }


    public enum AccessEnum { Public = 0, Private = 1, Moderated = 2 };




    /// <summary>
    ///  простой сбор (без наворотов)
    /// </summary>
    public class SimpleCollect: BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime When { get; set; }
        public int DurationMinutes { get; set; }

        /// <summary>
        /// нужны игроки или нужна команда
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Комментарий от создателя (например "верхнее поле, взять то-то" и т.д.)
        /// </summary>
        public string Comment { get; set; }


        /// <summary>
        /// Стомость сбора для участника
        /// </summary>
        public Decimal Cost { get; set; }
        
        /// <summary>
        /// Стомость аренды площадки
        /// </summary>
        public Decimal FullPrice { get; set; }

        /// <summary>
        /// сколько нужно человек для сбора
        /// </summary>
        public int NeedMembers { get; set; }

        /// <summary>
        /// Место проведения
        /// </summary>
        public int SimplePlaceId { get; set; }
        public SimplePlace SimplePlace { get; set; }

        /// <summary>
        /// Создатель
        /// </summary>
        public int CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public UserProfile Creator { get; set; }

        public ICollection<SimpleMember> SimpleMembers { get; set; }
        public SimpleCollect()
        {
            SimpleMembers = new List<SimpleMember>();
        }

        public SimpleCollect(string error)
        {
            SimpleMembers = new List<SimpleMember>();
            this.ErrorMessage = error;
        }
    }

    // через fluidity (нужен доступ через админку)
    /// <summary>
    /// сущность тип доступа
    /// </summary>
    //public class AccessType {
    //    public int AccessTypeId { get; set; }
    //    public string Name { get; set; }

    //    public AccessType()
    //    {

    //    }

    //    public AccessType(int id, string name)
    //    {
    //        Name = name;
    //        AccessTypeId = id;
    //    }
    //}


    /// <summary>
    /// сущность сбор
    /// </summary>
    public class Collect : BaseModel
    {

        public int CollectId { get; set; }
        public Place Place { get; set; }

        /// <summary>
        /// дата сбора
        /// </summary>
        public DateTime WhenDate { get; set; }

        /// <summary>
        /// час сбора (время)
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// минута сбора (время)
        /// </summary>
        public int Minute { get; set; }

        /// <summary>
        /// длительность сбора (в минутах)
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// место сбора
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// опции сбора (что требуется)
        /// </summary>
        public List<Option> Options { get; set; }
        public int FixedPriceByMember { get; set; }
        public List<MemberGroup> MemberGroups { get; set; }
        public List<NotMember> UsersInvited { get; set; }
        public List<NotMember> UsersWantsToParticipate { get; set; }
        public AccessEnum Access { get; set; }
        public bool Permanent { get; set; }
        public bool OrganizatorIsMember { get; set; }
        public bool AcceptedByPlaceOwner { get; set; }
        public UserProfile Organizer { get; set; }


        public Collect()
        {

        }

        public Collect(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

    }

    // через fluidity (нужен доступ через админку)
    /// <summary>
    /// сущность опция сбора (что требуется)
    /// </summary>
    public class Option : BaseModel
    {
        public int OptionId { get; set; }
        public string Name { get; set; }
        public bool Value { get; set; }
        public string Comment { get; set; }

    }

    /// <summary>
    /// Места для сборов
    /// </summary>
    public class SimpleCity : BaseModel
    {
        public int Id { get; set; }
        public int? CityUmbracoId { get; set; } 
        public string Name { get; set; }
        public string Info { get; set; }
        public string Geo { get; set; }
        public string MainPicture { get; set; }
        public bool Enabled { get; set; }

    }
    
    /// <summary>
    /// Места для сборов
    /// </summary>
    public class SimplePlace : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Info { get; set; }

        public int SimpleCityId { get; set; }

        [ForeignKey("SimpleCityId")]
        public SimpleCity City { get; set; }

        public string Address { get; set; }

        public string Geo { get; set; }

        public string MainPicture { get; set; }

        public bool Parking { get; set; }

        public bool BicycleParking { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// Id узла в Umbraco. Используем как идентификатор
        /// </summary>
        public int UmbracoId { get; set; }

        // расписание работы на указанные дни.  Beak'ами сделаем недоступное время.
        public ICollection<Worktime> Worktime { get; set; }

        public SimplePlace()
        {
            Worktime = new List<Worktime>();
        }
    }


    /// <summary>
    /// сущность место сбора
    /// </summary>
    public class Place : BaseModel
    {
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public City City { get; set; }
        public Owner Owner { get; set; }
        public Address Address { get; set; }
        public string Geo { get; set; }
        public List<string> Photos { get; set; }
        public string MainPicture { get; set; }
        public int Stages { get; set; }
        public bool Parking { get; set; }
        public bool BicycleParking { get; set; }
        public List<Worktime> Worktime { get; set; }
        public List<DressingRoom> DressingRooms { get; set; }
        public List<Area> Areas { get; set; }
        public bool Enabled { get; set; }
        //public AccessType Access { get; set; }

        public ICollection<Match> Matchs { get; set; }

        public Place()
        {
            Matchs = new List<Match>();
        }

    }

    public class DressingRoom : BaseModel
    {
        public int DressingRoomId { get; set; }
        public string RoomNumber { get; set; }
        public int Shower { get; set; }
        public bool HotWater { get; set; }
    }

    public class Area : BaseModel
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int CapacitySport { get; set; }
        public int CapacityViewers { get; set; }
        public int Price { get; set; }
    }

    public class Worktime : BaseModel
    {
        public int WorktimeId { get; set; }
        public DateTime When { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public bool Works24 { get; set; }
        public int CostPerHour { get; set; }
        
        /// <summary>
        /// идентфикатор места
        /// </summary>
        public int SimplePlaceId { get; set; }
        public SimplePlace SimplePlace { get; set; }

        public ICollection<Break> Breaks { get; set; }

        public Worktime()
        {
            Breaks = new List<Break>();
        }
    }

    public class Break : BaseModel
    {
        public int BreakId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        
        /// <summary>
        /// Идентификатор расписания, в котором перерыв
        /// </summary>
        public int WorktimeId { get; set; }
        public Worktime Worktime { get; set; }

        
    }


    /// <summary>
    /// Таблица с арендами. Вносим сюда все, что арендовано за деньги. Время, место, длительность. При запросе мест, надо будет вычитать арендованное время из доступного к аренде.
    /// </summary>
    public class Rent: BaseModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Место, где проводится
        /// </summary>
        public int SimplePlaceId { get; set; }
        public SimplePlace SimplePlace { get; set; }

        public DateTime From { get; set; }
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Стоимость аренды. Для финансовой отчетности
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// Арендатор
        /// </summary>
        public int UserProfileId { get; set; }
        [ForeignKey("UserProfileId")]
        public UserProfile Renter { get; set; }

        public Rent()
        {

        }

        public Rent(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }

    public class Address : BaseModel
    {
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string Index { get; set; }
        public string SubjectType { get; set; }
        public string House { get; set; }

        public Address()
        {

        }

        public Address(int addressId, string street, string index, string subjectType, string house)
        {
            AddressId = addressId;
            Street = street;
            Index = index;
            SubjectType = subjectType;
            House = house;
        }
    }

    // через стандартные документы (нужен доступ через админку)
    /// <summary>
    /// сущность амплуа участника
    /// </summary>
    public class Amplua : BaseModel
    {
        public int? AmpluaId { get; set; }
        public int? AmpluaUmbracoId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public ICollection<Player> Players { get; set; }

        public Amplua()
        {
            AmpluaId = 0;
            Name = "Не указано";
            IsDefault = false;
            AmpluaUmbracoId = 0;
            Players = new List<Player>();
        }

        public Amplua(int? ampluaId, string ampluaName, bool isdefault, int umbracoid)
        {
            AmpluaId = ampluaId;
            Name = ampluaName;
            IsDefault = isdefault;
            AmpluaUmbracoId = umbracoid;
            Players = new List<Player>();
        }
    }

    public class City : BaseModel
    {
        public int? CityId { get; set; }
        public int? CityUmbracoId { get; set; }
        public string CityUmbracoName { get; set; }
        public int? CityVkId { get; set; }
        public string Name { get; set; }
        public string GeoPosition { get; set; }
        public int Population { get; set; }
        public bool IsDefault { get; set; }


        /// <summary>
        /// Турниры в этом городе
        /// </summary>
        public ICollection<Tournament> Tournaments { get; set; }
        public ICollection<Team> Teams { get; set; }
        public ICollection<CityTournamentAdmin> CityTournamentAdmins { get; set; }


        public City()
        {
            CityId = 0;
            Name = "Не указан";
            CityUmbracoName = "";
            GeoPosition = "0;0";
            Population = 0;
            IsDefault = false;
            CityUmbracoId = 0;
            CityVkId = 0;
            ErrorMessage = "";
            Tournaments = new List<Tournament>();
            Teams = new List<Team>();
            CityTournamentAdmins = new List<CityTournamentAdmin>();
        }

        public City(int? cityId, string cityName)
        {
            CityId = cityId;
            Name = cityName;
            CityUmbracoName = "";
            GeoPosition = "0;0";
            Population = 0;
            IsDefault = false;
            CityUmbracoId = 0;
            CityVkId = 0;
            ErrorMessage = "";
            Tournaments = new List<Tournament>();
            CityTournamentAdmins = new List<CityTournamentAdmin>();

        }

        public City(int cityId, string cityName, string geo, int population, bool isdefault, int umbracoid, int cityVkId)
        {
            CityId = cityId;
            Name = cityName;
            GeoPosition = geo;
            Population = population;
            IsDefault = isdefault;
            CityUmbracoId = umbracoid;
            CityVkId = cityVkId;
            CityUmbracoName = "";
            ErrorMessage = "";
            Tournaments = new List<Tournament>();
            CityTournamentAdmins = new List<CityTournamentAdmin>();

        }
    }

    public class Leg : BaseModel
    {
        public int? LegId { get; set; }
        public int? LegUmbracoId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }


        public Leg()
        {
            LegId = 0;
            Name = "Не указано";
            IsDefault = false;
            LegUmbracoId = 0;
        }

        public Leg(int? legId, string legName, bool isdefault, int umbracoid)
        {
            LegId = legId;
            Name = legName;
            IsDefault = isdefault;
            LegUmbracoId = umbracoid;
        }
    }

    public class Owner : BaseModel
    {
        public int OwnerId { get; set; }
        public string Name { get; set; }

        public Owner()
        {
            OwnerId = -1;
            Name = "";
        }
    }


    /// <summary>
    /// сущность профиль пользователя сервисом
    /// </summary>
    public class UserProfile : BaseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int UserProfileId { get; set; }

        /// <summary>
        /// Ссылка на профиль
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Дополнительная ссылка для любых нужд
        /// </summary>
        public string Link2 { get; set; }

        /// <summary>
        /// Название Дополнительной ссылки для любых нужд
        /// </summary>
        public string Link2Name { get; set; }

        /// <summary>
        /// Название организатора
        /// </summary>
        public string OrganizatorName { get; set; }

        /// <summary>
        /// Короткое Название организатора
        /// </summary>
        public string OrganizatorNameShort { get; set; }

        /// <summary>
        /// суммарный опыт игрока 
        /// </summary>
        public int TotalExpirience { get; set; }
        // (пока сделаем 0 - не умею играть, 10 - начинающий, 20 - что-то немного умеет, 30 - уровень 5 лиги
        // 40 - уровень 3 лиги, 50 - уровень 1 лиги, 60 - уровень суперлиги, 70 - полупрофессионал, 80 - профессионал пфл, 90 - фнл, 100 - рпл

        /// <summary>
        /// профиль вк ID (если не привязан, тогда -1)
        /// </summary>
        public string UserVkId { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Fathername { get; set; }
        public DateTime Birth { get; set; }
        public DateTime Register { get; set; }
        public DateTime LastOnline { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }

        public string PhotoPath { get; set; }

        public int? CityVkId { get; set; }
        public string CityName { get; set; }
        public int? CityUmbracoId { get; set; }
        public string CityUmbracoName { get; set; }
        //public City City { get; set; }

        public int? LegId { get; set; }
        //public Leg Leg { get; set; }

        public int? AmpluaId { get; set; }
        //public Amplua Amplua { get; set; }

        /// <summary>
        /// IP, с которого заходил крайний раз
        /// </summary>
        public string LastIp { get; set; }

        public UserProfile()
        {
            Birth = DateTime.MinValue;
            Register = DateTime.UtcNow;
            LastOnline = DateTime.UtcNow;
            PhotoPath = "";
            CityName = "";
            CityUmbracoName = "";
            CityUmbracoId = -1;
            CityVkId = -1;

            LegId = 0;

            //City = new City();
            //Leg = new Leg();
            //Amplua = new Amplua();

            Admins = new List<Admin>();
            CityTournamentAdmins = new List<CityTournamentAdmin>();
            Tournaments = new List<Tournament>();

            SimpleCollects = new List<SimpleCollect>();
            SimpleMembers = new List<SimpleMember>();
            SimpleUserMessages = new List<SimpleUserMessage>();
            SimpleWidgets = new List<SimpleWidget>();

            ErrorMessage = "";
        }

        public UserProfile(VKUSerData vkUser)
        {
            Birth = vkUser.bdate;
            Register = DateTime.UtcNow;
            LastOnline = DateTime.UtcNow;
            Name = vkUser.first_name;
            Surname = vkUser.last_name;
            PhotoPath = "";

            LegId = 0;

            CityName = vkUser.city.title;
            CityVkId = vkUser.city.id;
            CityUmbracoId = -1;
            CityUmbracoName = "";

            //Leg = new Leg();
            //Amplua = new Amplua();
            Admins = new List<Admin>();
            CityTournamentAdmins = new List<CityTournamentAdmin>();
            Tournaments = new List<Tournament>();

            SimpleCollects = new List<SimpleCollect>();
            SimpleMembers = new List<SimpleMember>();
            SimpleUserMessages = new List<SimpleUserMessage>();
            SimpleWidgets = new List<SimpleWidget>();

            ErrorMessage = "";
        }

        public UserProfile(UserProfile userProfile)
        {
            Birth = userProfile.Birth;
            Register = userProfile.Register;
            LastOnline = userProfile.LastOnline;
            CityName = userProfile.CityName;
            CityVkId = userProfile.CityVkId;
            CityUmbracoId = userProfile.CityUmbracoId;
            CityUmbracoName = userProfile.CityUmbracoName;
            //Leg = userProfile.Leg;
            LegId = userProfile.LegId;
            Name = userProfile.Name;
            Surname = userProfile.Surname;
            Fathername = userProfile.Fathername;
            //Amplua = userProfile.Amplua;
            AmpluaId = userProfile.AmpluaId;
            Height = userProfile.Height;
            Weight = userProfile.Weight;
            UserProfileId = userProfile.UserProfileId;
            UserVkId = userProfile.UserVkId;
            PhotoPath = userProfile.PhotoPath;
            ErrorMessage = userProfile.ErrorMessage;
            Admins = new List<Admin>(userProfile.Admins);
            Tournaments = new List<Tournament>(userProfile.Tournaments);
            CityTournamentAdmins = new List<CityTournamentAdmin>(userProfile.CityTournamentAdmins);
            SimpleWidgets = new List<SimpleWidget>();

            SimpleCollects = new List<SimpleCollect>();
            SimpleMembers = new List<SimpleMember>();
            SimpleUserMessages = new List<SimpleUserMessage>();
        }

        public UserProfile(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Birth = DateTime.MinValue;
            Register = DateTime.UtcNow;
            LastOnline = DateTime.UtcNow;
            PhotoPath = "";
            SimpleCollects = new List<SimpleCollect>();
            SimpleMembers = new List<SimpleMember>();

            LegId = 0;
            Admins = new List<Admin>();
            Tournaments = new List<Tournament>();
            CityTournamentAdmins = new List<CityTournamentAdmin>();
            SimpleUserMessages = new List<SimpleUserMessage>();
            SimpleWidgets = new List<SimpleWidget>();


            //City = new City();
            //Leg = new Leg();
            //Amplua = new Amplua();
        }

        public IEnumerable<SimpleCollect> SimpleCollects { get; set; }
        public IEnumerable<SimpleUserMessage> SimpleUserMessages { get; set; }

        public IEnumerable<SimpleMember> SimpleMembers { get; set; }
        public IEnumerable<City> Citys { get; set; }
        public IEnumerable<Tournament> Tournaments { get; set; }
        public IEnumerable<Amplua> Ampluas { get; set; }
        public IEnumerable<Leg> Legs { get; set; }
        public IEnumerable<Admin> Admins { get; set; }
        public IEnumerable<CityTournamentAdmin> CityTournamentAdmins { get; set; }
        public IEnumerable<SimpleWidget> SimpleWidgets { get; set; }
    }


    /// <summary>
    /// Виджеты сообществ
    /// </summary>
    public class SimpleWidget: BaseModel
    {
        public int Id { get; set; }
        public int VKGroupId { get; set; }

        /// <summary>
        /// admin widget
        /// </summary>
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public string Token { get; set; }
        public string WidgetType { get; set; }

        public SimpleWidget()
        {
            WidgetType = "List";
        }
    }

    public class VKCity
    {
        public int id { get; set; }
        public string title { get; set; }
    }

    public class VKCountry
    {
        public int id { get; set; }
        public string title { get; set; }
    }

    public class VKUSerData
    {
        public DateTime bdate { get; set; }
        public VKCity city { get; set; }
        public VKCountry country { get; set; }
        public string first_name { get; set; }
        public int id { get; set; }
        public string last_name { get; set; }
        public string photo_100 { get; set; }
        public string photo_200 { get; set; }
        public string photo_max_orig { get; set; }
        public int sex { get; set; }
        public int timezone { get; set; }
    }




    // через стандратные настройки (нужен доступ через админку)
    /// <summary>
    /// сущность системные настройки
    /// </summary>
    public class SystemSettings
    {
        public int SystemSettingsId { get; set; }
    }

    /// <summary>
    /// Турнир
    /// </summary>
    public class Tournament : BaseModel
    {
        public int Id { get; set; }

        /// <summary>
        /// название турнира
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ссылка на профиль или ресурс организатора
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Дополнительная ссылка для любых нужд
        /// </summary>
        public string Link2 { get; set; }

        /// <summary>
        /// Название Дополнительной ссылки для любых нужд
        /// </summary>
        public string Link2Name { get; set; }

        /// <summary>
        /// Название организатора
        /// </summary>
        public string OrganizatorName { get; set; }

        /// <summary>
        /// Короткое Название организатора
        /// </summary>
        public string OrganizatorNameShort { get; set; }

        /// <summary>
        /// год проведения турнира
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// когда начинается турнир
        /// </summary>
        public DateTime WhenBegin { get; set; }

        /// <summary>
        /// когда заканчивается турнир
        /// </summary>
        public DateTime WhenEnd { get; set; }

        /// <summary>
        /// описание турнира
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// текст регламента турнира
        /// </summary>
        public string Reglament { get; set; }

        /// <summary>
        /// логотип турнира
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Длительность матча в минутах
        /// </summary>
        public int MatchLength { get; set; }


        public City City { get; set; }
        public int CityId { get; set; }
        

        public int? UserProfileId { get; set; }
        /// <summary>
        /// основатель турнира
        /// </summary>
        [ForeignKey("UserProfileId")]
        public UserProfile Founder { get; set; }


        /// <summary>
        /// группы турнира
        /// </summary>
        public ICollection<TournamentGroup> TournamentGroups { get; set; }

        /// <summary>
        /// администраторы турнира
        /// </summary>
        public ICollection<Admin> Admins { get; set; }

        public Tournament()
        {
            Published = false;
            Deleted = false;
            TournamentGroups = new List<TournamentGroup>();
            Admins = new List<Admin>();
        }

        public Tournament(string errorMessage)
        {
            Published = false;
            Deleted = false;
            ErrorMessage = errorMessage;
            TournamentGroups = new List<TournamentGroup>();
            Admins = new List<Admin>();
        }

        public Tournament(Tournament tournament)
        {
            ErrorMessage = "";
            Id = tournament.Id;
            Name = tournament.Name;
            Year = tournament.Year;
            WhenBegin = tournament.WhenBegin;
            WhenEnd = tournament.WhenEnd;
            Details = tournament.Details;
            Reglament = tournament.Reglament;
            Logo = tournament.Logo;
            Published = tournament.Published;
            Deleted = tournament.Deleted;
            City = tournament.City;
            CityId = tournament.CityId;
            TournamentGroups = new List<TournamentGroup>(tournament.TournamentGroups);
            Admins = new List<Admin>(tournament.Admins);
        }
    }


    /// <summary>
    /// Группа в турнире
    /// </summary>
    public class TournamentGroup : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        

        public ICollection<Team> Teams { get; set; }
        public ICollection<BidTeamToTournament> BidTeamToTournaments { get; set; }
        public ICollection<Match> Matches { get; set; }

        public TournamentGroup()
        {
            Teams = new List<Team>();
        }
        
        public TournamentGroup(string errorMessage)
        {
            Teams = new List<Team>();
            Matches = new List<Match>();
            ErrorMessage = errorMessage;
            Tournament = null;
            TournamentId = -1;
            Name = "";
            Published = false;
            Id = -1;
        }
        
        public TournamentGroup(TournamentGroup tGroup)
        {
            Teams = new List<Team>(tGroup.Teams);
            Matches = new List<Match>(tGroup.Matches);
            ErrorMessage = "";
            Tournament = tGroup.Tournament;
            TournamentId = tGroup.TournamentId;
            Name = tGroup.Name;
            Published = tGroup.Published;
            Id = tGroup.Id;
        }
    }


    /// <summary>
    /// Команда турнира
    /// </summary>
    public class Team : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public DateTime WhenBorn { get; set; }
        public string Details { get; set; }
        public string Logo { get; set; }

        public int TournamentId { get; set; }
        
        public City City { get; set; }
        public int CityId { get; set; }

        public ICollection<TournamentGroup> TournamentGroups { get; set; }
        public ICollection<Admin> Admins { get; set; }
        public ICollection<Match> Matches { get; set; }
        public ICollection<Player> Players { get; set; }

        public Team()
        {
            TournamentGroups = new List<TournamentGroup>();
            Admins = new List<Admin>();
            Matches = new List<Match>();
            Players = new List<Player>();
        }
        
        public Team(string errorMessage)
        {
            TournamentGroups = new List<TournamentGroup>();
            Admins = new List<Admin>();
            Matches = new List<Match>();
            Players = new List<Player>();
            ErrorMessage = errorMessage;
            Id = -1;
            Name = "";
            Year = -1;
            WhenBorn = DateTime.MinValue;
            Details = "";
            Logo = "";
            TournamentId = -1;
            City = new City();
            CityId = -1;
        }
        
        public Team(Team team)
        {
            TournamentGroups = new List<TournamentGroup>(team.TournamentGroups);
            Admins = new List<Admin>(team.Admins);
            Matches = new List<Match>(team.Matches);
            Players = new List<Player>(team.Players);
            ErrorMessage = team.ErrorMessage;
            Id = team.Id;
            Name = team.Name;
            Year = team.Year;
            WhenBorn = team.WhenBorn;
            Details = team.Details;
            Logo = team.Logo;
            TournamentId = team.TournamentId;
            Published = team.Published;
            Deleted = team.Deleted;
            City = team.City;
            CityId = team.CityId;
        }
    }

    /// <summary>
    /// заявка на добавление игрока в команду
    /// </summary>
    public class BidPlayerToTeam : BaseModel
    {
        public int BidPlayerToTeamId { get; set; }
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public DateTime When { get; set; }
        public bool Approved { get; set; }

        public BidPlayerToTeam()
        {
            PlayerId = -1;
            TeamId = -1;
            When = DateTime.UtcNow;
            Approved = false;
        }

        public BidPlayerToTeam(int playerId, int teamId, bool approved)
        {
            PlayerId = playerId;
            TeamId = teamId;
            When = DateTime.UtcNow;
            Approved = approved;
        }

    }


    /// <summary>
    /// Запись таблицы хранения игровых таблиц турниров. Заполняется динамически во время обновления счёта матча. Служит для ускорения выдачи результатов пользователям
    /// </summary>
    public class TournamentGroupTableItem : BaseModel
    {
        public int Id { get; set; }

        public int? BidTeamToTournamentId { get; set; }
        //public BidTeamToTournament BidTeamToTournament { get; set; }

        public int? TournamentGroupId { get; set; }
        //public TournamentGroup TournamentGroup { get; set; }
        public string TeamName { get; set; }

        public int? Games { get; set; }
        public int? Wins { get; set; }
        public int? Loses { get; set; }
        public int? Draws { get; set; }
        public int? GoalsScored { get; set; }
        public int? GoalsMissed { get; set; }
        public int? GoalsDifference { get; set; }
        public int? Points { get; set; }
        public int? Place { get; set; }

        public TournamentGroupTableItem()
        {
            BidTeamToTournamentId = -1;
            TournamentGroupId = - 1;
            Games = -1;
            Wins = -1;
            Loses = -1;
            Draws = -1;
            GoalsScored = -1;
            GoalsMissed = -1;
            GoalsDifference = -1;
            Points = -1;
            Place = -1;
        }
    }

    /// <summary>
    /// заявка на добавление команды в турнир
    /// </summary>
    public class BidTeamToTournament : BaseModel
    {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int TournamentGroupId { get; set; }
        public TournamentGroup TournamentGroup { get; set; }

        public DateTime When { get; set; }
        public bool Approved { get; set; }
        public string AdminTournamentComment { get; set; }

        // Published - флаг отправки. Если Published=true, а Approved=false - Значит заявка на рассмотрении.
        // Если Published = false, а Approved=true - Значит заявка подтверждена.
        // Если Published=false, а Approved=false - Значит заявка отменена админом команды или лиги.
        // Комментарий для админа лиги

        /// <summary>
        /// Название команды на турнир
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// отправитель заявки
        /// </summary>
        public UserProfile UserProfile { get; set; }
        public int UserProfileId { get; set; }

        public IEnumerable<Match> Matches { get; set; }

        public BidTeamToTournament()
        {
            TeamId = -1;
            TournamentGroupId = -1;
            TournamentGroup = new TournamentGroup();
            When = DateTime.UtcNow;
            Approved = false;
            UserProfile = new UserProfile();
            Team = new Team();
            UserProfileId = -1;
            AdminTournamentComment = "";

            Matches = new List<Match>();
        }

        public BidTeamToTournament(int teamId, int tournamentId, bool approved)
        {
            TeamId = teamId;
            TournamentGroupId = tournamentId;
            TournamentGroup = new TournamentGroup();
            Team = new Team();
            When = DateTime.UtcNow;
            Approved = approved;
            UserProfile = new UserProfile();
            AdminTournamentComment = "";
            UserProfileId = -1;

            Matches = new List<Match>();

        }
        
        public BidTeamToTournament(string errorMessage)
        {
            TeamId = -1;
            TournamentGroupId = -1;
            When = DateTime.UtcNow;
            Approved = false;
            UserProfile = new UserProfile();
            Team = new Team();
            UserProfileId = -1;
            ErrorMessage = errorMessage;
            AdminTournamentComment = "";
            TournamentGroup = new TournamentGroup();

            Matches = new List<Match>();
        }
        
        public BidTeamToTournament(BidTeamToTournament bid)
        {
            Id = bid.Id;
            TeamId = bid.TeamId;
            TournamentGroupId = bid.TournamentGroupId;
            When = DateTime.UtcNow;
            Team = bid.Team;
            Approved = bid.Approved;
            UserProfile = bid.UserProfile;
            UserProfileId = bid.UserProfileId;
            ErrorMessage = bid.ErrorMessage;
            AdminTournamentComment = bid.AdminTournamentComment;
            TournamentGroup = bid.TournamentGroup;

            Matches = new List<Match>();
            Matches = bid.Matches;
        }
    }




    public class Player : BaseModel
    {
        public int PlayerId { get; set; }

        public ICollection<Team> Teams { get; set; }

        public Member Member { get; set; }
        public int MemberId { get; set; }

        public Amplua Amplua { get; set; }
        public int AmpluaId { get; set; }

        public Player()
        {
            Teams = new List<Team>();
        }
    }

    /// <summary>
    /// Роль участника в турнире
    /// </summary>
    public class TournamentMemberRole : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
    }

    public class Match : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public DateTime When { get; set; }

        /// <summary>
        /// Флаг. Означает, что матч сыгран. Для расчета результатов.
        /// </summary>
        public bool Played { get; set; }
        
        public TournamentGroup TournamentGroup { get; set; }
        public int TournamentGroupId { get; set; }

        /// <summary>
        /// место
        /// </summary>
       
        public Place Place { get; set; }
        public int? PlaceId { get; set; }
        public int UmbracoPlaceId { get; set; } // это поле сделано для хранения Id места без занесения его в базу Custom. Потому что пока места берутся из базы умбрако. В будущем можно будет его убрать и использовать классические поля или придумать что-то оригинальное

        /// <summary>
        /// команда 1 (хозяева)
        /// </summary>
        public Team Team1 { get; set; }
        public int Team1Id { get; set; }

        /// <summary>
        /// заявка от команды 1 (чтобы определить точно, что за команда)
        /// </summary>
        public int BidTeamToTournamentId1 { get; set; }
        public BidTeamToTournament BidTeamToTournament1 { get; set; }

        /// <summary>
        /// команда 2 (гости)
        /// </summary>
        public Team Team2 { get; set; }
        public int Team2Id { get; set; }


        /// <summary>
        /// заявка от команды 2 (чтобы определить точно, что за команда)
        /// </summary>
        public int BidTeamToTournamentId2 { get; set; }
        public BidTeamToTournament BidTeamToTournament2 { get; set; }


        /// <summary>
        /// забито голов хозяевами (1)
        /// </summary>
        public int Team1Goals { get; set; }

        /// <summary>
        /// забито голов гостями (2)
        /// </summary>
        public int Team2Goals { get; set; }

        public ICollection<MatchEvent> MatchEvents { get; set; }

        public Match()
        {
            MatchEvents = new List<MatchEvent>();
            TournamentGroup = new TournamentGroup();
            Place = new Place();
            Team1 = new Team();
            Team2 = new Team();
            Played = false;
        }
        
        public Match(string errorMessage)
        {
            ErrorMessage = errorMessage;
            MatchEvents = null;
            TournamentGroup = null;
            Place = null;
            Team1 = null;
            Team2 = null;
            Played = false;

        }
    }

    public class MatchEvent : BaseModel
    {
        public int Id { get; set; }

        /// <summary>
        /// название события
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// описание события
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// матч, в котором состоялось событие
        /// </summary>
        public Match Match { get; set; }
        public int MatchId { get; set; }

        /// <summary>
        /// значение события (текст)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Минута, на которой состоялось событие
        /// </summary>
        public int Minute { get; set; }


        /// <summary>
        /// ссылка на игрока
        /// </summary>
        public Member Member { get; set; }
        public int MemberId { get; set; }
    }

    public class MatchEventType : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }

    /// <summary>
    /// администраторы турниров
    /// </summary>
    public class Admin : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public AdminType AdminType { get; set; }
        public int AdminTypeId { get; set; }

        public Tournament Tournament { get; set; }
        public int? TournamentId { get; set; }

        public Team Team { get; set; }
        public int? TeamId { get; set; }

        public Admin()
        {
            Name = "";
            UserProfileId = -1;
            UserProfile = new UserProfile();
            AdminType = new AdminType();
            AdminTypeId = -1;
            Tournament = new Tournament();
            TournamentId = -1;
            Team = new Team();
            TeamId = -1;
            ErrorMessage = "";
        }
        
        public Admin(Admin admin)
        {
            Id = admin.Id;
            Name = admin.Name;
            UserProfileId = admin.UserProfileId;
            UserProfile = admin.UserProfile;
            AdminType = admin.AdminType;
            AdminTypeId = admin.AdminTypeId;
            Tournament = admin.Tournament;
            TournamentId = admin.TournamentId;
            Team = admin.Team;
            TeamId = admin.TeamId;
            ErrorMessage = admin.ErrorMessage;
            Published = admin.Published;
            Deleted = admin.Deleted;
        }

        public Admin(string errorMessage)
        {
            Id = -1;
            Name = "";
            UserProfileId = -1;
            UserProfile = new UserProfile();
            AdminType = new AdminType();
            AdminTypeId = -1;
            Tournament = new Tournament();
            TournamentId = -1;
            Team = new Team();
            TeamId = -1;
            ErrorMessage = errorMessage;
            Published = false;
            Deleted = false;
        }
    }


    /// <summary>
    /// тип администратора (команды, турнира, системы и тд) влияет на отображение интерфейса
    /// </summary>
    public class AdminType : BaseModel
    {
        public int AdminTypeId { get; set; }
        public string Name { get; set; }


    }


    /// <summary>
    /// хранилище профилей, которым доступно создание турниров города (админы лиги)
    /// </summary>
    public class CityTournamentAdmin: BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UserProfile UserProfile { get; set; }
        public int UserProfileId { get; set; }

        public City City { get; set; }
        public int CityId { get; set; }


        public CityTournamentAdmin()
        {
            Name = "";
            UserProfileId = -1;
            UserProfile = new UserProfile();
            City = new City();
            CityId = -1;
            ErrorMessage = "";
        }

        public CityTournamentAdmin(CityTournamentAdmin cityTournamentAdmin)
        {
            Id = cityTournamentAdmin.Id;
            Name = cityTournamentAdmin.Name;
            UserProfileId = cityTournamentAdmin.UserProfileId;
            UserProfile = cityTournamentAdmin.UserProfile;
            City = cityTournamentAdmin.City;
            CityId = cityTournamentAdmin.CityId;
            ErrorMessage = cityTournamentAdmin.ErrorMessage;
            Published = cityTournamentAdmin.Published;
            Deleted = cityTournamentAdmin.Deleted;
        }

        public CityTournamentAdmin(string errorMessage)
        {
            Id = -1;
            Name = "";
            UserProfileId = -1;
            UserProfile = new UserProfile();
            City = new City();
            CityId = -1;
            ErrorMessage = errorMessage;
            Published = false;
            Deleted = false;
        }
    }

    public class TournamentTable
    {

    }



    public class AppStartupHandler : Umbraco.Core.ApplicationEventHandler
    {
        protected override void ApplicationStarted(Umbraco.Core.UmbracoApplicationBase umbracoApplication, Umbraco.Core.ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);
            System.Web.Http.GlobalConfiguration.Configuration.Formatters.Clear();
            System.Web.Http.GlobalConfiguration.Configuration.Formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());

        }

    }
}