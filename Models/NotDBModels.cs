using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osbackend.Models
{
    public class TagViewModel
    {
        public int Id { set; get; }
        public string Code { set; get; }
    }

    public class CreateUserViewModel
    {
        public string Id { set; get; }
        public string Name { set; get; }
        //public List<TagViewModel> Tags { set; get; }
    }

    public class CreateTournamentModel
    {
        public Tournament tournament { get; set; }
        public UserProfile userProfile { get; set; }
    }

    public class CityUserProfile
    {
        public City city { get; set; }
        public UserProfile userProfile { get; set; }
    }
    
    public class CreateTeamModel
    {
        public Team team { get; set; }
        public UserProfile userProfile { get; set; }
    }

    public class TeamAdminProfile
    {
        public Team team { get; set; }
        public Admin adminProfile { get; set; }
    }

    public class UserProfileTournamentTournamentGroupModel
    {
        public Tournament tournament { get; set; }
        public TournamentGroup tournamentGroup { get; set; }
        public UserProfile userProfile { get; set; }
    }
    
    public class UserProfileTournamentBidTeamToTournament
    {
        public Tournament tournament { get; set; }
        public BidTeamToTournament bid { get; set; }
        public UserProfile userProfile { get; set; }
    }
    
    public class UserProfileTournamentGroupTeam
    {
        public Team team { get; set; }
        public TournamentGroup oldGroup { get; set; }
        public TournamentGroup newGroup { get; set; }
        public UserProfile userProfile { get; set; }
    }
    
    
    public class UserProfileBidTeamToTournament
    {
        public BidTeamToTournament bidTeamToTournament { get; set; }
        public Team team { get; set; }
        public UserProfile userProfile { get; set; }
    }
    
    public class UserProfileTeamTournamentGroup
    {
        public TournamentGroup tournamentGroup { get; set; }
        public Team team { get; set; }
        public UserProfile userProfile { get; set; }
        public string admintext { get; set; }
    }

    public class MatchUserProfile
    {
        public Match match { get; set; }
        public UserProfile userProfile { get; set; }
    }


    public class SimpleCollectUserProfile
    {
        public SimpleCollect simpleCollect { get; set; }
        public UserProfile userProfile { get; set; }
    }

    public class SimpleCollectUserProfileMember
    {
        public SimpleCollect simpleCollect { get; set; }
        public UserProfile userProfile { get; set; }
        public SimpleMember simpleMember { get; set; }
    }


    public class AddressToConvert
    {
        public string street { get; set; }
        public string index { get; set; }
        public string subjectType { get; set; }
        public string house { get; set; }
    }

    public class SimpleWidgetUserProfile
    {
        public SimpleWidget simpleWidget { get; set; }
        public UserProfile userProfile { get; set; }
    }


    /// <summary>
    /// прототип для распознавания json при загрузке изображения виджета
    /// </summary>
    public class FromVKWidgetUpload
    {
        public System.Collections.Generic.Dictionary<string, string> response { get; set; }
    }

    /// <summary>
    /// прототип для распознавания json для сохранения загруженного изображения
    /// </summary>
    public class ForVKWidgetAppSave
    {
        public string hash { get; set; }
        public string image { get; set; }
    }

    public class WorktimeToConvert
    {
        public DateTime fromTime { get; set; }
        public DateTime toTime { get; set; }
        public int works24 { get; set; }
        public int? costPerHour { get; set; }

        public string breakTimes { get; set; }

        public WorktimeToConvert()
        {
            //breakTimes = new List<BreakToConvert>();
        }
    }

    public class BreakToConvert
    {
        public DateTime fromTime { get; set; }
        public DateTime toTime { get; set; }

    }

    /// <summary>
    /// структура данных, полученная с парсера. для внесения матчей в базу облака в автоматическом режиме
    /// </summary>
    public class MatchFromParser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public DateTime When { get; set; }
        public int TourNumber { get; set; } //номер тура

        /// <summary>
        /// Флаг. Означает, что матч сыгран. Для расчета результатов.
        /// </summary>
        public bool Played { get; set; }

        //public TournamentGroup TournamentGroup { get; set; }
        public int TournamentGroupCloudId { get; set; }

        /// <summary>
        /// место
        /// </summary>

        //public Place Place { get; set; } - вместо этого сделана таблица ассоциаций
        public int? PlaceAssociactionId { get; set; }


        /// <summary>
        /// команда 1 (хозяева)
        /// </summary>
        public int Team1AssociationId { get; set; } // ассоциация команды 1


        /// <summary>
        /// команда 2 (гости)
        /// </summary>
        public int Team2AssociationId { get; set; } // ассоциация команды 2



        /// <summary>
        /// забито голов хозяевами (1)
        /// </summary>
        public int Team1Goals { get; set; }

        /// <summary>
        /// забито голов гостями (2)
        /// </summary>
        public int Team2Goals { get; set; }


    }
}