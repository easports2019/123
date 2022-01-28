using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using osbackend.Utils;
using osbackend.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using System.Web;
using System.Web.Http.Cors;

namespace osbackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleTournamentController : UmbracoApiController
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
        /// Добавляет в базу турнир и создает в таблице админов запись и его же возвращает в случае успеха
        /// </summary>
        /// <param name="createTournamentModel">Данные турнира и профиля создателя</param>
        /// <returns></returns>
        public dynamic Add(CreateTournamentModel createTournamentModel)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (createTournamentModel.tournament != null))
            {
                City city = null;
                using (DefaultContext dc1 = new DefaultContext())
                {
                    city = dc1.Citys.Where(x => x.CityUmbracoId == createTournamentModel.tournament.CityId).FirstOrDefault();
                }

                if (city != null)
                {
                    Tournament tOurnament = new Tournament();
                    int tournId = -1;
                    
                    try
                    {
                        using (DefaultContext dc2 = new DefaultContext())
                        {
                            System.Data.Entity.DbContextTransaction context = dc2.Database.BeginTransaction();

                            tOurnament.Details = createTournamentModel.tournament.Details;
                            tOurnament.Logo = createTournamentModel.tournament.Logo;
                            tOurnament.Name = createTournamentModel.tournament.Name;
                            tOurnament.OrganizatorName = createTournamentModel.tournament.OrganizatorName;
                            tOurnament.OrganizatorNameShort = createTournamentModel.tournament.OrganizatorNameShort;
                            tOurnament.Link = createTournamentModel.tournament.Link;
                            tOurnament.Link2 = createTournamentModel.tournament.Link2;
                            tOurnament.Link2Name = createTournamentModel.tournament.Link2Name;
                            tOurnament.Reglament = createTournamentModel.tournament.Reglament;
                            tOurnament.TournamentGroups = new List<TournamentGroup>();
                            tOurnament.WhenBegin = createTournamentModel.tournament.WhenBegin;
                            tOurnament.WhenEnd = createTournamentModel.tournament.WhenEnd;
                            tOurnament.Year = createTournamentModel.tournament.Year;
                            tOurnament.Deleted = createTournamentModel.tournament.Deleted;
                            tOurnament.Published = createTournamentModel.tournament.Published;
                            tOurnament.Year = createTournamentModel.tournament.Year;
                            tOurnament.CityId = (int)city.CityId;
                            tOurnament.UserProfileId = createTournamentModel.userProfile.UserProfileId;
                            tOurnament.MatchLength = createTournamentModel.tournament.MatchLength;


                            tOurnament.Admins = null;
                            tOurnament.City = null;
                            tOurnament.Founder = null;

                            foreach (TournamentGroup tg in createTournamentModel.tournament.TournamentGroups)
                            {
                                tg.Matches = null;
                                tg.Teams = null;
                                tg.Tournament = tOurnament;
                                tOurnament.TournamentGroups.Add(tg);
                            }

                            if (dc2.Tournaments.Any())
                            {
                                //if (dc.Tournaments.FirstOrDefault(x => x.Name == createTournamentModel.tournament.Name) == null)
                                //{
                                // добавляем турнир


                                // создаем админов турнира
                                //Admin adm = dc.Admins.Where(x => x.UserProfileId == createTournamentModel.userProfile.UserProfileId).FirstOrDefault();
                                Admin adm = new Admin();
                                AdminType atype = dc2.AdminTypes.FirstOrDefault(x => x.Name == "TournamentAdmin");

                                adm.AdminType = null;
                                adm.AdminTypeId = atype.AdminTypeId;
                                adm.Deleted = false;
                                adm.Published = true;
                                adm.TournamentId = tOurnament.Id;
                                adm.Team = null;
                                adm.TeamId = null;
                                adm.Tournament = null;
                                adm.UserProfile = null;
                                adm.UserProfileId = createTournamentModel.userProfile.UserProfileId;

                                //dc.SaveChanges();

                                dc2.Admins.Add(adm);
                                dc2.Tournaments.Add(tOurnament);
                                dc2.SaveChanges();
                                context.Commit();
                                //}

                                tournId = tOurnament.Id;
                            }

                        }

                        if (tournId > 0)
                        {
                            return GetTournamentById(tournId);
                        }
                        else
                        {
                            return new Tournament("Error in TournamentAdd: Can't write to db");
                        }

                       
                    }
                    catch (Exception ex)
                    {
                        return new Tournament("Error in TournamentAdd: " + ex.Message);
                    }
                }
                else
                {
                    return new Tournament("Error in TournamentAdd: city not found");
                }
            }
            else
            {
                return new Tournament("Error in TournamentAdd: not authorized");
            }

        }


        [HttpOptions]
        public dynamic Update()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Добавляет в базу турнир и создает в таблице админов запись и его же возвращает в случае успеха
        /// </summary>
        /// <param name="createTournamentModel">Данные турнира и профиля создателя</param>
        /// <returns></returns>
        public dynamic Update(CreateTournamentModel createTournamentModel)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTournament(createTournamentModel.userProfile, createTournamentModel.tournament))
                && (createTournamentModel.tournament != null))
            {
                Tournament tmpTourn = dc.Tournaments.FirstOrDefault(x => x.Id == createTournamentModel.tournament.Id);
                System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();
                                            
                try
                {
                    if (ModelState.IsValid)
                    {
                        

                        if (tmpTourn != null)
                        {
                            

                            tmpTourn.Name = createTournamentModel.tournament.Name;
                            tmpTourn.OrganizatorName = createTournamentModel.tournament.OrganizatorName;
                            tmpTourn.OrganizatorNameShort = createTournamentModel.tournament.OrganizatorNameShort;
                            tmpTourn.Link = createTournamentModel.tournament.Link;
                            tmpTourn.Link2 = createTournamentModel.tournament.Link2;
                            tmpTourn.Link2Name = createTournamentModel.tournament.Link2Name;
                            tmpTourn.Reglament = createTournamentModel.tournament.Reglament;
                            tmpTourn.Details = createTournamentModel.tournament.Details;
                            tmpTourn.Deleted = createTournamentModel.tournament.Deleted;
                            tmpTourn.Published = createTournamentModel.tournament.Published;
                            tmpTourn.Logo = createTournamentModel.tournament.Logo;
                            tmpTourn.ErrorMessage = createTournamentModel.tournament.ErrorMessage;
                            tmpTourn.WhenBegin = createTournamentModel.tournament.WhenBegin;
                            tmpTourn.WhenEnd = createTournamentModel.tournament.WhenEnd;
                            tmpTourn.MatchLength = createTournamentModel.tournament.MatchLength;
                            tmpTourn.Year = createTournamentModel.tournament.Year;
                            

                            dc.Entry<Tournament>(tmpTourn).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            context.Commit();
                        }
                        else
                        {
                            context.Rollback();
                            return new Tournament("Error in TournamentUpdate: tournament with that Id not found");
                        }
                    }
                    else
                    {
                        context.Rollback();
                        return new Tournament("Error in TournamentUpdate: ModelState isn't valid");
                    }


                    return GetTournamentById(tmpTourn.Id);
                }
                catch (Exception ex)
                {
                    context.Rollback();
                    return new Tournament("Error in TournamentUpdate: " + ex.Message);
                }
            }
            else
            {
                return new Tournament("Error in TournamentUpdate: not authorized or no rights to edit this tournament");
            }

        }



        [HttpOptions]
        public dynamic Publish()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Публикация турнира
        /// </summary>
        /// <param name="createTournamentModel">Данные о турнире и пользователь</param>
        /// <returns></returns>
        public dynamic Publish(CreateTournamentModel createTournamentModel)
        {
            Tournament tt = new Tournament("Error in TournamentPublish: No tournament with that Id");



            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null) && (createTournamentModel != null))
            {
                // проверить, имеет ли право данный пользователь админить этот турнир (отдельной функцией)
                if (new Common().UserProfileHasAccessToTournament(createTournamentModel.userProfile, createTournamentModel.tournament))
                {

                    if (dc.Tournaments.Any())
                    {
                        tt = dc.Tournaments.FirstOrDefault(x => x.Id == createTournamentModel.tournament.Id);

                    }

                    try
                    {
                        if (tt != null)
                        {
                            tt.Published = createTournamentModel.tournament.Published;
                            dc.Entry<Tournament>(tt).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                        }
                        else
                        {
                            return new Tournament("Error in TournamentPublish: No tournament with that Id");
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Tournament("Error in TournamentPublish: " + ex.Message);
                    }

                    return GetTournamentById(tt.Id);
                }
                else
                {
                    return new Tournament("Error in TournamentPublish: user haven't access to this tournament");
                }
            }
            else
            {
                return new Tournament("Error in TournamentPublish: not authorized");
            }

        }

        // возвращает турнир по Id
        public dynamic GetTournamentById(int Id)
        {
            using (DefaultContext dc3 = new DefaultContext())
                return dc3.Tournaments
                    .Where(t => t.Id == Id)
                    .Join(dc3.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (tourn, up) => new { tourn, up })
                    .Select(x => new
                    {
                        Tournament = x.tourn,
                        Founder = x.up
                    })
                    .SelectMany(tu => dc3.TournamentGroups
                                        .Where(y => y.TournamentId == tu.Tournament.Id)
                                        .GroupBy(r => r.TournamentId)
                                        .Select(r => new
                                        {
                                            Id = tu.Tournament.Id,
                                            Name = tu.Tournament.Name,
                                            OrganizatorName = tu.Tournament.OrganizatorName,
                                            OrganizatorNameShort = tu.Tournament.OrganizatorNameShort,
                                            Link = tu.Tournament.Link,
                                            Link2 = tu.Tournament.Link2,
                                            Link2Name = tu.Tournament.Link2Name,
                                            Year = tu.Tournament.Year,
                                            WhenBegin = tu.Tournament.WhenBegin,
                                            WhenEnd = tu.Tournament.WhenEnd,
                                            Details = tu.Tournament.Details,
                                            Reglament = tu.Tournament.Reglament,
                                            MatchLength = tu.Tournament.MatchLength,
                                            Logo = tu.Tournament.Logo,
                                            City = tu.Tournament.City,
                                            CityId = tu.Tournament.CityId,
                                            UserProfileId = tu.Founder.UserProfileId,
                                            Founder = tu.Founder,
                                            Published = tu.Tournament.Published,
                                            Deleted = tu.Tournament.Deleted,
                                            TournamentGroups = r
                                        })
                    )
                    .FirstOrDefault();
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
        /// <param name="createTournamentModel">Данные о турнире и пользователе</param>
        /// <returns></returns>
        public dynamic Delete(CreateTournamentModel createTournamentModel)
        {
            Tournament tt = new Tournament("Error in TournamentDelete: No tournament with that Id");



            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null) && (createTournamentModel != null))
            {
                // проверить, имеет ли право данный пользователь админить этот турнир (отдельной функцией)
                if (new Common().UserProfileHasAccessToTournament(createTournamentModel.userProfile, createTournamentModel.tournament))
                {

                    if (dc.Tournaments.Any())
                    {
                        tt = dc.Tournaments.FirstOrDefault(x => x.Id == createTournamentModel.tournament.Id);

                    }

                    try
                    {
                        if (tt != null)
                        {
                            tt.Published = false;
                            tt.Deleted = true;
                            dc.Entry<Tournament>(tt).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                        }
                        else
                        {
                            return new Tournament("Error in TournamentDelete: No tournament with that Id");
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Tournament("Error in TournamentDelete: " + ex.Message);
                    }

                    return GetTournamentById(tt.Id);
                }
                else
                {
                    return new Tournament("Error in TournamentDelete: user haven't access to this tournament");
                }
            }
            else
            {
                return new Tournament("Error in TournamentDelete: not authorized");
            }

        }



        /// <summary>
        /// Возвращает все турниры
        /// </summary>
        /// <returns></returns>
        public List<Tournament> GetAll()
        {
            List<Tournament> tournaments = new List<Tournament>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    tournaments = (dc.Tournaments
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.Tournaments
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return tournaments;
        }

        /// <summary>
        /// Возвращает идущие турниры
        /// </summary>
        /// <returns></returns>
        public List<Tournament> GetAllCurrent()
        {
            List<Tournament> tournaments = new List<Tournament>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    tournaments = (dc.Tournaments
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Where(x => x.WhenBegin <= DateTime.UtcNow)
                        .Where(x => x.WhenEnd >= DateTime.UtcNow)
                        .Count() > startIndex)
                        ?
                        dc.Tournaments
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Where(x => x.WhenBegin <= DateTime.UtcNow)
                        .Where(x => x.WhenEnd >= DateTime.UtcNow)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return tournaments;
        }


        [HttpPost]
        /// <summary>
        /// Возвращает турниры, принадлежащие админу по его UserProfileId
        /// </summary>
        /// <returns></returns>
        public dynamic GetAllByAdminId()
        {
            List<Tournament> tournaments = new List<Tournament>();
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
                    var result = dc.Tournaments
                        .Include(x => x.TournamentGroups)
                        .Include(x => x.Admins)
                        .Where(t => t.Deleted == false)
                        .Join(userIsSystemadmin ? dc.Admins : dc.Admins
                        .Where(a => (a.UserProfileId == adminProfileId) && (a.AdminTypeId == 2)), 
                        f => f.Id, d => d.TournamentId,
                        (tournament, adm) => tournament)
                        .Select(tour =>
                        new
                        {
                            Id = tour.Id,
                            Name = tour.Name,
                            OrganizatorName = tour.OrganizatorName,
                            OrganizatorNameShort = tour.OrganizatorNameShort,
                            Link = tour.Link,
                            Link2 = tour.Link2,
                            Link2Name = tour.Link2Name,
                            Year = tour.Year,
                            WhenBegin = tour.WhenBegin,
                            WhenEnd = tour.WhenEnd,
                            Details = tour.Details,
                            Reglament = tour.Reglament,
                            MatchLength = tour.MatchLength,
                            ErrorMessage = tour.ErrorMessage,
                            Logo = tour.Logo,
                            City = tour.City,
                            CityId = tour.CityId,
                            Published = tour.Published,
                            Deleted = tour.Deleted,
                            TournamentGroups = tour.TournamentGroups.Select(tg => tg).ToList(),
                            Admins = tour.Admins.Select(am => am).ToList(),
                        }

                        )
                        ;

                   return result;
                }
                catch (Exception ex)
                {
                    return new Tournament("Error in TournamentGetAllByAdminId: " + ex.Message);
                }

            }

            return tournaments;
        }


        [HttpPost]
        /// <summary>
        /// Возвращает идущие турниры в выбранном городе
        /// </summary>
        /// <param name="cityId">ID города</param>
        /// <returns></returns>
        public dynamic GetAllCurrentInCity()
        {
            //List<Tournament> tournaments = new List<Tournament>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;
                int cityUmbracoId = (currentRequest.Form.Get("cityumbracoid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityumbracoid")[0]) : 0;

                City city = dc.Citys.First(x => x.CityUmbracoId == cityUmbracoId);
                if (city != null)
                {
                    using (DefaultContext dc = new DefaultContext())
                    {
                        var tournaments =
                            //(dc.Tournaments
                            //.Where(x => x.Published)
                            //.Where(x => !x.Deleted)
                            //.Where(x => x.CityId == city.CityId)
                            ////.Where(x => x.WhenBegin <= DateTime.UtcNow)
                            //.Where(x => x.WhenEnd >= DateTime.UtcNow)
                            //.Count() > startIndex)
                            //?
                            dc.Tournaments
                            .Where(x => x.Published && !x.Deleted && x.CityId == city.CityId && x.WhenEnd >= DateTime.UtcNow)
                            .Join(dc.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (tourn, up) => new { tourn, up })
                            .Select(x => new
                            {
                                Tournament = x.tourn,
                                Founder = x.up
                            })
                            .SelectMany(tu => dc.TournamentGroups
                                                .Where(y => y.TournamentId == tu.Tournament.Id)
                                                .GroupBy(r => r.TournamentId)
                                                .Select(r => new
                                                {
                                                    Id = tu.Tournament.Id,
                                                    Name = tu.Tournament.Name,
                                                    OrganizatorName = tu.Tournament.OrganizatorName,
                                                    OrganizatorNameShort = tu.Tournament.OrganizatorNameShort,
                                                    Link = tu.Tournament.Link,
                                                    Link2 = tu.Tournament.Link2,
                                                    Link2Name = tu.Tournament.Link2Name,
                                                    Year = tu.Tournament.Year,
                                                    WhenBegin = tu.Tournament.WhenBegin,
                                                    WhenEnd = tu.Tournament.WhenEnd,
                                                    Details = tu.Tournament.Details,
                                                    Reglament = tu.Tournament.Reglament,
                                                    MatchLength = tu.Tournament.MatchLength,
                                                    Logo = tu.Tournament.Logo,
                                                    City = tu.Tournament.City,
                                                    CityId = tu.Tournament.CityId,
                                                    UserProfileId = tu.Founder.UserProfileId,
                                                    Founder = tu.Founder,
                                                    Published = tu.Tournament.Published,
                                                    Deleted = tu.Tournament.Deleted,
                                                    TournamentGroups = r
                                                })
                            );

                        
                        return tournaments
                            .ToList();
                    }
                }
            }

            return null;
        }


        [HttpOptions]
        public dynamic GetTeamsByTournament()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Возвращает новые заявки на турнир (турнир и профиль админа турнира должны быть переданы)
        /// </summary>
        /// <param name="createTeamModel">Профиль запросившего и турнир</param>
        /// <returns></returns>
        public dynamic GetTeamsByTournament(CreateTournamentModel createTournamentModel)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == createTournamentModel.userProfile.UserProfileId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, createTournamentModel.tournament))
                )
            {

                using (DefaultContext dc = new DefaultContext())
                {
                    DateTime now = DateTime.Now;
                    //int tournamentId = dc.TournamentGroups.Where(x => x.Id == tournamentGroupId).FirstOrDefault().TournamentId;
                    var res = (dc.BidTeamToTournaments
                        .Where(t => ((t.Deleted == false) && (t.Published == false) && (t.Approved == true))) // одобренные 
                        .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (bid, tg) => new { bid, tg })
                        .Where(bidtg => bidtg.tg.TournamentId == createTournamentModel.tournament.Id)
                        .Count() > 0)
                        ?
                        dc.BidTeamToTournaments
                        .Where(t => ((t.Deleted == false) && (t.Published == false) && (t.Approved == true))) // одобренные 
                        .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (bid, tg) => new { bid, tg })
                        .Where(bidtg => bidtg.tg.TournamentId == createTournamentModel.tournament.Id)
                        .Join(dc.Teams, a => a.bid.TeamId, b => b.Id, (bidtg, te) => new { bidtg, te })
                        .Join(dc.Tournaments, a => a.bidtg.tg.TournamentId, b => b.Id, (bidtgte, trn) => new
                        {
                            AdminTournamentComment = bidtgte.bidtg.bid.AdminTournamentComment,
                            Approved = bidtgte.bidtg.bid.Approved,
                            Deleted = bidtgte.bidtg.bid.Deleted,
                            ErrorMessage = bidtgte.bidtg.bid.ErrorMessage,
                            Id = bidtgte.bidtg.bid.Id,
                            Published = bidtgte.bidtg.bid.Published,
                            TeamId = bidtgte.bidtg.bid.TeamId,
                            TeamName = bidtgte.bidtg.bid.TeamName,
                            TournamentGroup = bidtgte.bidtg.tg,
                            TournamentGroupId = bidtgte.bidtg.bid.TournamentGroupId,
                            UserProfileId = bidtgte.bidtg.bid.UserProfileId,
                            When = bidtgte.bidtg.bid.When,
                            TournamentId = bidtgte.bidtg.tg.TournamentId,
                            Team = new {
                                Id = bidtgte.te.Id,
                                Name = bidtgte.te.Name,
                                Details = bidtgte.te.Details,
                                Logo = bidtgte.te.Logo,
                                Published = bidtgte.te.Published,
                                Deleted = bidtgte.te.Deleted,
                                CityId = bidtgte.te.CityId,
                            }
                        })
                        .ToList()
                        : null;

                    return res;
                }
            }
            else
            {
                return new List<BidTeamToTournament>() { new BidTeamToTournament("Error in BidTeamToTournamentGetTeamBidsByTournament: not authorized") };
            }
        }

        /// <summary>
        /// Возвращает турнир по его Id
        /// </summary>
        /// <param name="tournamentId">Id турнира</param>
        /// <returns></returns>
        public Tournament GetById(int tournamentId)
        {
            Tournament tournament = new Tournament();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    tournament = (dc.Tournaments.FirstOrDefault(x => x.Id == tournamentId) != null)
                        ?
                        dc.Tournaments.FirstOrDefault(x => x.Id == tournamentId)
                        :
                        new Tournament("Error in TournamentGetById: No tourneys with that Id");
                }
            }

            return tournament;
        }
    }
}
