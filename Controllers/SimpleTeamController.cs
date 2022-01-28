using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using osbackend.Utils;
using osbackend.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;
using Umbraco.Web;
using System.Data.Entity;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using System.Web;
using System.Web.Http.Cors;

namespace osbackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleTeamController : UmbracoApiController
    {
        DefaultContext dc = new DefaultContext();
        HttpRequest currentRequest = HttpContext.Current.Request;

        [HttpOptions]
        public dynamic Add()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Добавляет в базу команду и создает в таблице админов запись и ее же возвращает в случае успеха
        /// </summary>
        /// <param name="createTeamModel">Данные команды и профиля создателя</param>
        /// <returns></returns>
        public dynamic Add(CreateTeamModel createTeamModel)
        {
            if (createTeamModel.team == null)
                return new Team("Error in TeamAdd: no input data");

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (createTeamModel.team != null))
            {

                City city = null;

                using (DefaultContext dc1 = new DefaultContext())
                {
                    city = dc1.Citys.Where(x => x.CityUmbracoId == createTeamModel.team.CityId).FirstOrDefault();
                }


                if (city != null)
                {
                    Team tEam = new Team();
                    int teamId = -1;

                    using (DefaultContext dc2 = new DefaultContext())
                    {
                        System.Data.Entity.DbContextTransaction context = dc2.Database.BeginTransaction();

                        try
                        {

                            tEam.Details = createTeamModel.team.Details;
                            tEam.Logo = createTeamModel.team.Logo;
                            tEam.Name = createTeamModel.team.Name;
                            tEam.WhenBorn = createTeamModel.team.WhenBorn;
                            tEam.Year = createTeamModel.team.Year;
                            tEam.Deleted = createTeamModel.team.Deleted;
                            tEam.Published = createTeamModel.team.Published;
                            tEam.Year = createTeamModel.team.Year;
                            tEam.CityId = (int)city.CityId;

                            tEam.City = null;
                            tEam.Admins = null;
                            tEam.Players = null;
                            tEam.TournamentGroups = null;
                            tEam.Matches = null;


                            // создаем админов команды
                            //Admin adm = dc.Admins.Where(x => x.UserProfileId == createTournamentModel.userProfile.UserProfileId).FirstOrDefault();
                            Admin adm = new Admin();
                            AdminType atype = dc2.AdminTypes.FirstOrDefault(x => x.Name == "TeamAdmin");

                            adm.AdminType = null;
                            adm.AdminTypeId = atype.AdminTypeId;
                            adm.Deleted = false;
                            adm.Published = true;
                            adm.TeamId = tEam.Id;
                            adm.UserProfileId = createTeamModel.userProfile.UserProfileId;

                            adm.Team = null;
                            adm.UserProfile = null;
                            adm.Tournament = null;
                            adm.TournamentId = null;

                            //dc.SaveChanges();

                            dc2.Admins.Add(adm);
                            dc2.Teams.Add(tEam);
                            dc2.SaveChanges();
                            context.Commit();

                            teamId = tEam.Id;
                        }
                        catch (Exception ex)
                        {
                            context.Rollback();
                            return new Team("Error in TeamAdd: " + ex.Message);
                        }
                    }


                    if (teamId > 0)
                    {


                        return GetTeamById(teamId);

                    }
                    else
                    {
                        return new Team("Error in TeamAdd: problem in writing to db");

                    }
                }
                else
                {
                    return new Team("Error in TeamAdd: city not found");
                }
            }
            else
            {
                return new Team("Error in TeamAdd: not authorized");
            }

        }


        [HttpOptions]
        public dynamic Update()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Добавляет в базу команду и создает в таблице админов запись и ее же возвращает в случае успеха
        /// </summary>
        /// <param name="createTeamModel">Данные команды и профиля создателя</param>
        /// <returns></returns>
        public dynamic Update(CreateTeamModel createTeamModel)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTeam(createTeamModel.userProfile, createTeamModel.team))
                && (createTeamModel.team != null))
            {
                Team tmpTeam = dc.Teams
                            .FirstOrDefault(x => x.Id == createTeamModel.team.Id);
                System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();

                try
                {
                    if (ModelState.IsValid)
                    {
                        

                        if (tmpTeam != null)
                        {
                            tmpTeam.Name = createTeamModel.team.Name;
                            tmpTeam.Details = createTeamModel.team.Details;
                            tmpTeam.Deleted = createTeamModel.team.Deleted;
                            tmpTeam.Published = createTeamModel.team.Published;
                            tmpTeam.Logo = createTeamModel.team.Logo;
                            tmpTeam.ErrorMessage = createTeamModel.team.ErrorMessage;
                            tmpTeam.WhenBorn = createTeamModel.team.WhenBorn;
                            tmpTeam.Year = createTeamModel.team.Year;
                            tmpTeam.CityId = createTeamModel.team.CityId;

                            tmpTeam.City = null;
                            tmpTeam.Admins = null;
                            tmpTeam.TournamentGroups = null;
                            tmpTeam.Matches = null;
                            tmpTeam.Players = null;

                            dc.Entry<Team>(tmpTeam).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            context.Commit();
                        }
                        else
                        {
                            context.Rollback();
                            return new Team("Error in TeamUpdate: team with that Id not found");
                        }
                    }
                    else
                    {
                        context.Rollback();
                        return new Team("Error in TeamUpdate: ModelState isn't valid");
                    }


                    return GetTeamById(tmpTeam.Id);
                }
                catch (Exception ex)
                {
                    context.Rollback();
                    return new Team("Error in TeamUpdate: " + ex.Message);
                }
            }
            else
            {
                return new Team("Error in TeamUpdate: not authorized or no rights to edit this team");
            }

        }

        [HttpOptions]
        public dynamic Publish()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Публикация команды
        /// </summary>
        /// <param name="createTeamModel">Данные о команде и пользователь</param>
        /// <returns></returns>
        public dynamic Publish(CreateTeamModel createTeamModel)
        {
            Team tt = new Team("Error in TeamPublish: No team with that Id");



            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null) && (createTeamModel != null))
            {
                // проверить, имеет ли право данный пользователь админить этот турнир (отдельной функцией)
                if (new Common().UserProfileHasAccessToTeam(createTeamModel.userProfile, createTeamModel.team))
                {

                    if (dc.Teams.Any())
                    {
                        tt = dc.Teams.FirstOrDefault(x => x.Id == createTeamModel.team.Id);

                    }

                    try
                    {
                        if (tt != null)
                        {
                            tt.Published = createTeamModel.team.Published;
                            dc.Entry<Team>(tt).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                        }
                        else
                        {
                            return new Team("Error in TeamPublish: No team with that Id");
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Team("Error in TeamPublish: " + ex.Message);
                    }

                    return GetTeamById(tt.Id);
                }
                else
                {
                    return new Team("Error in TeamPublish: user haven't access to this team");
                }
            }
            else
            {
                return new Team("Error in TeamPublish: not authorized");
            }

        }


        // возвращает команду по Id
        public dynamic GetTeamById(int Id)
        {
            using (DefaultContext dc3 = new DefaultContext())
                return dc3.Teams
                    .Include(x => x.TournamentGroups)
                    .Include(x => x.Admins)
                    .Include(x => x.Matches)
                    .Include(x => x.Players)
                    .Where(t => t.Id == Id)
                    .Join(dc3.Admins, f => f.Id, d => d.TeamId, (team, adm) => team)
                    .Select(team =>
                    new
                    {
                        Id = team.Id,
                        Name = team.Name,
                        Year = team.Year,
                        WhenBorn = team.WhenBorn,
                        Details = team.Details,
                        ErrorMessage = team.ErrorMessage,
                        Logo = team.Logo,
                        City = team.City,
                        CityId = team.CityId,
                        Published = team.Published,
                        Deleted = team.Deleted,
                        TournamentGroups = team.TournamentGroups.Select(tg => tg).ToList(),
                        Admins = team.Admins.Select(am => am).ToList(),
                        Matches = team.Matches.Select(ma => ma).ToList(),
                        Players = team.Players.Select(pl => pl).ToList(),
                    }

                    )
                    .FirstOrDefault()
                    ;
        }
        
        
        // возвращает команду по Id заявки на участие в турнире
        public Team GetTeamByBidTeamToTournamentId(int BidId)
        {
            using (DefaultContext dc3 = new DefaultContext())
            {
                try
                {
                    var res = dc3.BidTeamToTournaments
                        //.Include(x => x.Team)
                        //.Include(x => x.TournamentGroup)
                        //.Include(x => x.UserProfile)
                        .FirstOrDefault(t => t.Id == BidId);
                    var team = dc3.Teams
                        .FirstOrDefault(x => x.Id == res.TeamId);
                    return team != null ? team : null;
                }
                catch(Exception ex)
                {
                    return null;
                }
            }

        }


        [HttpOptions]
        public dynamic Delete()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Удаление турнира
        /// </summary>
        /// <param name="createTeamModel">Данные о турнире и пользователе</param>
        /// <returns></returns>
        public dynamic Delete(CreateTeamModel createTeamModel)
        {
            Team tt = new Team("Error in TeamDelete: No team with that Id");

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null) && (createTeamModel != null))
            {
                // проверить, имеет ли право данный пользователь админить эту команду (отдельной функцией)
                if (new Common().UserProfileHasAccessToTeam(createTeamModel.userProfile, createTeamModel.team))
                {

                    if (dc.Teams.Any())
                    {
                        tt = dc.Teams.FirstOrDefault(x => x.Id == createTeamModel.team.Id);

                    }

                    try
                    {
                        if (tt != null)
                        {
                            tt.Deleted = true;
                            tt.Published = false;
                            dc.Entry<Team>(tt).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                        }
                        else
                        {
                            return new Team("Error in TeamDelete: No team with that Id");
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Team("Error in TeamDelete: " + ex.Message);
                    }

                    return GetTeamById(tt.Id);
                }
                else
                {
                    return new Team("Error in TeamDelete: user haven't access to this team");
                }
            }
            else
            {
                return new Team("Error in TeamDelete: not authorized");
            }

        }


        [HttpPost]
        /// <summary>
        /// Возвращает команды, принадлежащие админу по его UserProfileId
        /// </summary>
        /// <returns></returns>
        public dynamic GetAllByAdminId()
        {
            List<Team> teams = new List<Team>();
            List<Admin> admins = new List<Admin>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;
                int adminProfileId = (currentRequest.Form.Get("adminprofileid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("adminprofileid")[0]) : 0;

                bool userIsSystemadmin = Common.GetUserProfileAdminList(adminProfileId).Exists(x => x.AdminTypeId == 1);

                //var result = dc.Admins
                //    .Where(c => c.UserProfileId == adminProfileId)
                //    .Join(dc.Tournaments.Where(x => x.Deleted == false), x => x.TournamentId, y => y.Id, 
                //    (adm, tournament) => tournament).SelectMany(dc.TournamentGroups, x => x.Id);
                try
                {
                    var result = dc.Teams
                        .Include(x => x.TournamentGroups)
                        .Include(x => x.Admins)
                        .Include(x => x.Matches)
                        .Include(x => x.Players)
                        .Where(t => t.Deleted == false)
                        .Join(userIsSystemadmin ? dc.Admins 
                        : dc.Admins
                        .Where(a => ((a.UserProfileId == adminProfileId) && (a.AdminTypeId == 3))
                        || (a.AdminTypeId == 1)), f => f.Id, d => d.TeamId, // доступ админу команд и системному админу
                        (team, adm) => team)
                        .Select(team =>
                        new
                        {
                            Id = team.Id,
                            Name = team.Name,
                            Year = team.Year,
                            WhenBorn = team.WhenBorn,
                            Details = team.Details,
                            ErrorMessage = team.ErrorMessage,
                            Logo = team.Logo,
                            City = team.City,
                            CityId = team.CityId,
                            Published = team.Published,
                            Deleted = team.Deleted,
                            TournamentGroups = team.TournamentGroups.Select(tg => tg).ToList(),
                            Admins = team.Admins.Select(am => am).ToList(),
                            Matches = team.Matches.Select(ma => ma).ToList(),
                            Players = team.Players.Select(pl => pl).ToList(),
                        }

                        )
                        ;


                    return result;
                }
                catch (Exception ex)
                {
                    return new Team("Error in TeamGetAllByAdminId: " + ex.Message);
                }

                //tournaments = (dc.Tournaments
                //    .Where(x => !x.Deleted)
                //    .Count() > startIndex)
                //    ?
                //    dc.Tournaments
                //    .Where(x => !x.Deleted)
                //    .Skip(startIndex)
                //    .Take(Common.RowsToReturnByTransaction).ToList()
                //    : null;
            }

            return teams;
        }

        /// <summary>
        /// Возвращает все команды
        /// </summary>
        /// <returns></returns>
        public List<Team> GetAll()
        {
            List<Team> teams = new List<Team>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    teams = (dc.Teams
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.Teams
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return teams;
        }

        /// <summary>
        /// Возвращает команды команды указанного турнира
        /// </summary>
        /// <param name="tournamentGroupId">Id группы турнира</param>
        /// <returns></returns>
        public List<Team> GetAllInTournament(int tournamentGroupId)
        {
            List<Team> teams = new List<Team>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    int tournamentId = dc.TournamentGroups.Where(x => x.Id == tournamentGroupId).FirstOrDefault().TournamentId;
                    teams = (dc.Teams
                        .Where(x => x.TournamentId == tournamentId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.Teams
                        .Where(x => x.TournamentId == tournamentId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return teams;
        }

        /// <summary>
        /// Возвращает идущие турниры в выбранном городе
        /// </summary>
        /// <param name="cityId">ID города</param>
        /// <returns></returns>
        public List<Team> GetAllInCity(int cityId)
        {
            List<Team> teams = new List<Team>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    teams = (dc.Teams
                        .Where(x => x.CityId == cityId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.Teams
                        .Where(x => x.CityId == cityId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return teams;
        }

        [HttpPost]
        /// <summary>
        /// Возвращает команду по ее Id
        /// </summary>
        /// <returns></returns>
        public Team GetById()
        {
            Team team = new Team();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {

                int teamId = (currentRequest.Form.Get("teamid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("teamid")[0]) : -1;

                using (DefaultContext dc = new DefaultContext())
                {
                    team = (dc.Teams.FirstOrDefault(x => x.Id == teamId) != null)
                        ?
                        dc.Teams
                        //.Include(x => x.Admins)
                        //.Include(x => x.City)
                        .FirstOrDefault(x => x.Id == teamId)
                        :
                        new Team("Error in TeamGetById: No teams with that Id");
                }
            }

            return team;
            
        }
    }
}
