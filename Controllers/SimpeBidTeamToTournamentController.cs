using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
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
    public class SimpeBidTeamToTournamentController : UmbracoApiController
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
        /// Добавляет в базу запрос команды на добавление в турнир и ее же возвращает в случае успеха. Если команда с таким же именем существует, возвращается она
        /// </summary>
        /// <param name="userProfileBidTeamToTournament">Данные заявки</param>
        /// <returns></returns>
        public dynamic Add(UserProfileBidTeamToTournament userProfileBidTeamToTournament)
        {
            if (userProfileBidTeamToTournament == null)
                return new BidTeamToTournament("Error in BidTeamToTournamentAdd: no input data");

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTeam(userProfileBidTeamToTournament.userProfile, userProfileBidTeamToTournament.team))
                )
            {
                //dc.Database.BeginTransaction();
                try
                {
                    int id = -1;
                    TournamentGroup trgr = dc.TournamentGroups
                        .Where(x => x.Id == userProfileBidTeamToTournament.bidTeamToTournament.TournamentGroupId)
                        .FirstOrDefault();
                    BidTeamToTournament btt = null;

                    if (trgr != null)
                    {
                        btt = dc.BidTeamToTournaments
                        //.Include(x => dc.TournamentGroups)
                        .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (bid, trg) => new { bid, trg })
                        .Where(x => x.trg.TournamentId == trgr.TournamentId)
                        .Where(x => x.bid.TeamId == userProfileBidTeamToTournament.bidTeamToTournament.TeamId)
                        .Where(x => x.bid.Deleted == false)
                        //.Where(x => x.TeamId == userProfileBidTeamToTournament.bidTeamToTournament.TeamId)
                        //.Where(x => x.TournamentGroup.TournamentId == trgr.TournamentId)
                        //.Where(x => x.Deleted == false)
                        .Select(x => x.bid)
                        .FirstOrDefault();
                    }

                    if (btt == null) // добавление новой заявки
                    {
                        
                        BidTeamToTournament bidTeamToTournamentTmp = new BidTeamToTournament();
                        bidTeamToTournamentTmp.TeamName = userProfileBidTeamToTournament.bidTeamToTournament.TeamName; // название команды на турнир
                        bidTeamToTournamentTmp.When = userProfileBidTeamToTournament.bidTeamToTournament.When;
                        bidTeamToTournamentTmp.TournamentGroupId = userProfileBidTeamToTournament.bidTeamToTournament.TournamentGroupId;
                        bidTeamToTournamentTmp.UserProfileId = userProfileBidTeamToTournament.bidTeamToTournament.UserProfileId;
                        bidTeamToTournamentTmp.TeamId = userProfileBidTeamToTournament.bidTeamToTournament.TeamId;
                        bidTeamToTournamentTmp.ErrorMessage = "";
                        bidTeamToTournamentTmp.AdminTournamentComment = ""; // комментарий при отклонении
                        bidTeamToTournamentTmp.Approved = false; // true - согласована, false - отклонена
                        bidTeamToTournamentTmp.Deleted = false;  // true - удалена, false - активна
                        bidTeamToTournamentTmp.Published = true; // true - на рассмотрении, false - рассмотрена

                        bidTeamToTournamentTmp.Team = null;
                        bidTeamToTournamentTmp.TournamentGroup = null;
                        bidTeamToTournamentTmp.UserProfile = null;

                        dc.BidTeamToTournaments.Add(bidTeamToTournamentTmp);
                        dc.SaveChanges();

                        id = bidTeamToTournamentTmp.Id;
                        // сделать проверку на повторное добавление отклоненной заявки. возможно заявка та же, модифицируем лишь группу

                    }
                    else // редактирование существующей
                    {
                        btt.TeamName = userProfileBidTeamToTournament.bidTeamToTournament.TeamName;
                        btt.Approved = false;
                        btt.Published = true;
                        btt.Deleted = false;
                        btt.TournamentGroupId = userProfileBidTeamToTournament.bidTeamToTournament.TournamentGroupId;
                        btt.When = DateTime.UtcNow;
                        btt.Team = null;
                        btt.UserProfile = null;
                        btt.TournamentGroup = null;


                        dc.Entry(btt).State = EntityState.Modified;
                        dc.SaveChanges();

                        id = btt.Id;
                    }

                    //dc.Database.CurrentTransaction.Commit();

                    

                    return GetBidById(id);
                }
                catch (Exception ex)
                {
                    //dc.Database.CurrentTransaction.Rollback();
                    return new BidTeamToTournament("Error in BidTeamToTournamentAdd: " + ex.Message);
                }
            }
            else
            {
                return new BidTeamToTournament("Error in BidTeamToTournamentAdd: not authorized");
            }

        }

        /// <summary>
        /// возвращает заявку по Id
        /// </summary>
        public dynamic GetBidById(int Id)
        {
            using (DefaultContext dc3 = new DefaultContext())
                return dc3.BidTeamToTournaments
                        .Where(x => x.Id == Id)
                        .Join(dc3.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (bd, trg) => new { bd, trg })
                        .Join(dc3.Teams, a => a.bd.TeamId, b => b.Id, (bidtrg, team) => new { bidtrg, team })
                        .Join(dc3.Tournaments, a => a.bidtrg.trg.TournamentId, trn => trn.Id, (bidtrgteam, trn) => new { bidtrgteam, trn })
                        .Select(a => new {
                            Id = a.bidtrgteam.bidtrg.bd.Id,
                            //TournamentGroup = a.bidtrgteam.bidtrg.trg,

                            TournamentGroup = new
                            {
                                Id = a.bidtrgteam.bidtrg.trg.Id,
                                ErrorMessage = a.bidtrgteam.bidtrg.trg.ErrorMessage,
                                Deleted = a.bidtrgteam.bidtrg.trg.Deleted,
                                Name = a.bidtrgteam.bidtrg.trg.Name,
                                Published = a.bidtrgteam.bidtrg.trg.Published,
                                Tournament = new
                                {
                                    TournamentId = a.trn.Id,
                                    Name = a.trn.Name,
                                    Logo = a.trn.Logo,
                                    Founder = a.trn.Founder,
                                    Details = a.trn.Details
                                },
                                TournamentId = a.bidtrgteam.bidtrg.trg.TournamentId,
                            },
                            //TournamentId = a.bidtrgteam.bidtrg.trg.TournamentId,
                            Team = a.bidtrgteam.team,
                            When = a.bidtrgteam.bidtrg.bd.When,
                            Approved = a.bidtrgteam.bidtrg.bd.Approved,
                            Deleted = a.bidtrgteam.bidtrg.bd.Deleted,
                            Published = a.bidtrgteam.bidtrg.bd.Published,
                            AdminTournamentComment = a.bidtrgteam.bidtrg.bd.AdminTournamentComment,
                            TeamId = a.bidtrgteam.bidtrg.bd.TeamId,
                            TournamentGroupId = a.bidtrgteam.bidtrg.bd.TournamentGroupId,
                            UserProfileId = a.bidtrgteam.bidtrg.bd.UserProfileId,
                            TeamName = a.bidtrgteam.bidtrg.bd.TeamName

                        })
                        .FirstOrDefault()
                        ;
        }

        [HttpOptions]
        public dynamic Publish()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Отправка (публикация) заявки
        /// </summary>
        /// <param name="userProfileBidTeamToTournament">данные заявки и профиль пользователя</param>
        /// <returns></returns>
        public dynamic Publish(UserProfileBidTeamToTournament userProfileBidTeamToTournament)
        {
            BidTeamToTournament t = new BidTeamToTournament("Error in BidTeamToTournamentPublish: No BidTeamToTournaments with that Id");

            
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTeam(userProfileBidTeamToTournament.userProfile, userProfileBidTeamToTournament.bidTeamToTournament.Team))

                )
            {
                if (dc.BidTeamToTournaments.Any())
                {
                    BidTeamToTournament tt = dc.BidTeamToTournaments.FirstOrDefault(x => x.Id == userProfileBidTeamToTournament.bidTeamToTournament.Id);
                    t = new BidTeamToTournament(tt);
                }

                try
                {
                    if (t != null)
                    {
                        t.Published = userProfileBidTeamToTournament.bidTeamToTournament.Published;
                        dc.Entry<BidTeamToTournament>(t).State = System.Data.Entity.EntityState.Modified;
                        dc.SaveChanges();
                    }
                    else
                    {
                        return new BidTeamToTournament("Error in BidTeamToTournamentPublish: No BidTeamToTournament with that Id");
                    }
                }
                catch (Exception ex)
                {
                    return new BidTeamToTournament("Error in BidTeamToTournamentPublish: " + ex.Message);
                }

                return GetBidById(t.Id);
            }
            else
            {
                return new BidTeamToTournament("Error in BidTeamToTournamentPublish: not authorized");
            }

        }


        [HttpOptions]
        public dynamic Approve()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Согласование заявки. Заявку согласовывает админ лиги. Нужен профиль админа, заявка и турнир
        /// </summary>
        /// <param name="userProfileBidTeamToTournament">данные заявки и профиль пользователя</param>
        /// <returns></returns>
        public dynamic Approve(UserProfileTournamentBidTeamToTournament userProfileTournamentBidTeamToTournament)
        {
            BidTeamToTournament t = new BidTeamToTournament("Error in BidTeamToTournamentPublish: No BidTeamToTournaments with that Id");


            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTournament(userProfileTournamentBidTeamToTournament.userProfile, userProfileTournamentBidTeamToTournament.tournament))

                )
            {
                if (dc.BidTeamToTournaments.Any())
                {
                    BidTeamToTournament tt = dc.BidTeamToTournaments.FirstOrDefault(x => x.Id == userProfileTournamentBidTeamToTournament.bid.Id);
                    t = new BidTeamToTournament(tt);
                }

                try
                {
                    if (t != null)
                    {
                        t.Approved = userProfileTournamentBidTeamToTournament.bid.Approved;
                        t.Published = false;
                        t.Deleted = false;
                        dc.Entry<BidTeamToTournament>(t).State = System.Data.Entity.EntityState.Modified;
                        dc.SaveChanges();


                    }
                    else
                    {
                        return new BidTeamToTournament("Error in BidTeamToTournamentPublish: No BidTeamToTournament with that Id");
                    }
                }
                catch (Exception ex)
                {
                    return new BidTeamToTournament("Error in BidTeamToTournamentPublish: " + ex.Message);
                }

                return GetBidById(t.Id);
            }
            else
            {
                return new BidTeamToTournament("Error in BidTeamToTournamentPublish: not authorized");
            }

        }


        [HttpOptions]
        public dynamic Delete()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Удаление заявки на добавление в турнир (пометка)
        /// </summary>
        /// <param name="userProfileBidTeamToTournament">Заявка, пользователь, команда</param>
        /// <returns></returns>
        public dynamic Delete(UserProfileBidTeamToTournament userProfileBidTeamToTournament)
        {
            if (userProfileBidTeamToTournament == null)
                return new BidTeamToTournament("Error in BidTeamToTournamentAdd: no input data");

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTeam(userProfileBidTeamToTournament.userProfile, userProfileBidTeamToTournament.team))
                )
            {
                BidTeamToTournament t = dc.BidTeamToTournaments.FirstOrDefault(x => x.Id == userProfileBidTeamToTournament.bidTeamToTournament.Id);

                try
                {
                    if (t != null)
                    {
                        t.Deleted = true;
                        t.Team = null;
                        t.TournamentGroup = null;
                        t.UserProfile = null;
                        dc.Entry<BidTeamToTournament>(t).State = System.Data.Entity.EntityState.Modified;
                        dc.SaveChanges();

                        userProfileBidTeamToTournament.bidTeamToTournament.Deleted = true;

                        return GetBidById(t.Id);
                    }
                    else
                    {
                        return new BidTeamToTournament("Error in BidTeamToTournamentDelete: No BidTeamToTournament with that Id");
                    }
                }
                catch (Exception ex)
                {
                    return new BidTeamToTournament("Error in BidTeamToTournamentDelete: " + ex.Message);
                }
            }
            else
            {
                return new BidTeamToTournament("Error in BidTeamToTournamentDelete: not authorized");
            }

        }

        [HttpOptions]
        public dynamic GetActualTournaments()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Возвращает актуальные турниры (команда и профиль админа команды должны быть переданы)
        /// </summary>
        /// <param name="createTeamModel">Профиль запросившего и его команда</param>
        /// <returns></returns>
        public List<Tournament> GetActualTournaments(CreateTeamModel createTeamModel)
        {
            List<Tournament> tournaments = new List<Tournament>() { new Tournament("No tournaments") };
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == createTeamModel.userProfile.UserProfileId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (new Common().UserProfileHasAccessToTeam(uProfile, createTeamModel.team))
                )
                {
                    //int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                    using (DefaultContext dc = new DefaultContext())
                    {
                        DateTime now = DateTime.Now;
                        //int tournamentId = dc.TournamentGroups.Where(x => x.Id == tournamentGroupId).FirstOrDefault().TournamentId;
                        tournaments = (dc.Tournaments
                            //.Include(x => x.TournamentGroups)
                            //.Include(x => x.Founder)
                            .Where(t => ((t.Deleted == false) && (t.Published == true)))
                            .Where(t => t.CityId == createTeamModel.team.CityId)
                            .Where(t => t.WhenBegin > now)
                            .Count() > 0)
                            ?
                            dc.Tournaments
                            //.Include(x => x.TournamentGroups)
                            .Include(x => x.Founder)
                            .Where(t => ((t.Deleted == false) && (t.Published == true)))
                            .Where(t => t.CityId == createTeamModel.team.CityId)
                            .Where(t => t.WhenBegin > now)
                            .OrderBy(x => x.WhenBegin)
                            .ToList()
                            //.Skip(startIndex)
                            //.Take(Common.RowsToReturnByTransaction).ToList()
                            : null;
                    }
                }
            else
            {
                return new List<Tournament>() { new Tournament("Error in BidTeamToTournamentGetActualTournaments: not authorized") };
            }

            return tournaments;
        }


        [HttpOptions]
        public dynamic SetTeamTournamentGroup()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Изменяет привязку к группе турнира у команды (команда, группа турнира и профиль админа турнира должны быть переданы)
        /// </summary>
        /// <param name="userProfileTournamentGroupTeam">Профиль админа, команда и новая группа турнира</param>
        /// <returns></returns>
        public dynamic SetTeamTournamentGroup(UserProfileTournamentGroupTeam userProfileTournamentGroupTeam)
        {
            Tournament tournament = dc.Tournaments.FirstOrDefault(x => x.Id == userProfileTournamentGroupTeam.oldGroup.TournamentId);
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileTournamentGroupTeam.userProfile.UserProfileId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (tournament != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, tournament))
                )
            {
                BidTeamToTournament bid = dc.BidTeamToTournaments.Where(x => (x.TeamId == userProfileTournamentGroupTeam.team.Id && x.TournamentGroupId == userProfileTournamentGroupTeam.oldGroup.Id)).FirstOrDefault();
                // не всегда почему-то меняет группу
                bid.TournamentGroupId = userProfileTournamentGroupTeam.newGroup.Id;
                bid.Team = null;
                bid.TournamentGroup = null;
                bid.UserProfile = null;
                dc.Entry(bid).State = EntityState.Modified;
                
                dc.SaveChanges();

                // здесь неверный запрос, тк в указанной функции идет выборка 
                // .Where(t => ((t.Deleted == false) && (t.Published == true) && (t.Approved == false)))
                // а нам надо .Where(t => ((t.Deleted == false) && (t.Published == false) && (t.Approved == true)))
                return new SimpleTournamentController().GetTeamsByTournament(new CreateTournamentModel() { tournament = tournament, userProfile = uProfile });
                
            }

            else
            {
                return new Tournament("Error in SetTeamTournamentGroup: not authorized");
            }

            //return null;
        }

        [HttpOptions]
        public dynamic GetTeamBidsByTournament()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Возвращает новые заявки на турнир (турнир и профиль админа турнира должны быть переданы)
        /// </summary>
        /// <param name="createTeamModel">Профиль запросившего и турнир</param>
        /// <returns></returns>
        public dynamic GetTeamBidsByTournament(CreateTournamentModel createTournamentModel)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == createTournamentModel.userProfile.UserProfileId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, createTournamentModel.tournament))
                )
                {
                    //int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                    using (DefaultContext dc = new DefaultContext())
                    {
                        DateTime now = DateTime.Now;
                        //int tournamentId = dc.TournamentGroups.Where(x => x.Id == tournamentGroupId).FirstOrDefault().TournamentId;
                        var res = 
                        //(
                        //dc.BidTeamToTournaments
                        //    .Where(t => ((t.Deleted == false) && (t.Published == true) && (t.Approved == false)))
                        //    .Count() > 0)
                        //    ?
                            dc.BidTeamToTournaments
                            .Include(x => x.TournamentGroup)
                            .Include(x => x.UserProfile)
                            .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (bid, tg) => new { bid, tg })
                            .Join(dc.UserProfiles, a => a.bid.UserProfileId, b => b.UserProfileId, (bidtg, up) => new { bidtg, up })
                            .Join(dc.Tournaments, a => a.bidtg.tg.TournamentId, b => b.Id, (bidtgup, trn) => new  
                                { 
                                    AdminTournamentComment = bidtgup.bidtg.bid.AdminTournamentComment,
                                    Approved = bidtgup.bidtg.bid.Approved,
                                    Deleted = bidtgup.bidtg.bid.Deleted,
                                    ErrorMessage = bidtgup.bidtg.bid.ErrorMessage,
                                    Id = bidtgup.bidtg.bid.Id,
                                    Published = bidtgup.bidtg.bid.Published,
                                    Team = bidtgup.bidtg.bid.Team,
                                    TeamId = bidtgup.bidtg.bid.TeamId,
                                    TeamName = bidtgup.bidtg.bid.TeamName,
                                    TournamentGroup = bidtgup.bidtg.tg,
                                    TournamentGroupId = bidtgup.bidtg.bid.TournamentGroupId,
                                    UserProfile = bidtgup.up,
                                    UserProfileId = bidtgup.bidtg.bid.UserProfileId,
                                    When = bidtgup.bidtg.bid.When,
                                    TournamentId = bidtgup.bidtg.tg.TournamentId
                                })
                            .Where(t => ((t.Deleted == false) && (t.Published == true) && (t.Approved == false)))
                            .Where(t => t.TournamentId == createTournamentModel.tournament.Id)
                            .ToList()
                            //: new List<BidTeamToTournament>()
                            ;

                        return res;
                    }
                }
            else
            {
                return new List<BidTeamToTournament>() { new BidTeamToTournament("Error in BidTeamToTournamentGetTeamBidsByTournament: not authorized") };
            }
        }
        
        [HttpOptions]
        public dynamic AcceptBid()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Согласовывает заявку на турнир (турнир и профиль админа турнира должны быть переданы)
        /// </summary>
        /// <param name="userProfileTournamentBidTeamToTournament">Профиль, турнир и заявка запросившего</param>
        /// <returns></returns>
        public dynamic AcceptBid(UserProfileTournamentBidTeamToTournament userProfileTournamentBidTeamToTournament)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileTournamentBidTeamToTournament.userProfile.UserProfileId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, userProfileTournamentBidTeamToTournament.tournament))
                )
                {
                    //int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                    using (DefaultContext dc = new DefaultContext())
                    {
                        BidTeamToTournament bid = dc.BidTeamToTournaments.Where(x => x.Id == userProfileTournamentBidTeamToTournament.bid.Id).FirstOrDefault();

                        bid.AdminTournamentComment = userProfileTournamentBidTeamToTournament.bid.AdminTournamentComment;
                        bid.Published = false;
                        bid.Approved = true;
                        bid.Team = null;
                        bid.TournamentGroup = null;
                        bid.UserProfile = null;
                        dc.Entry(bid).State = EntityState.Modified;
                        dc.SaveChanges();

                    if (!AddTeamToTournamentTable(userProfileTournamentBidTeamToTournament.bid.Id))
                        bid.ErrorMessage = "Error when creating new row in tournament table";

                    return GetBidById(bid.Id);
                    }
                }
            else
            {
                return new BidTeamToTournament("Error in AcceptBid: not authorized");
            }
        }


        /// <summary>
        /// добавление команды в таблицу результатов
        /// </summary>
        /// <param name="bidTeam1ToTournamentId">Id заявки команды на турнир</param>
        /// <returns></returns>
        public bool AddTeamToTournamentTable(int bidTeam1ToTournamentId)
        {
            // ======================= добавление строки в турнирную таблицу =============================
            DbContextTransaction trans = dc.Database.BeginTransaction();

            try
            {

                TournamentGroupTableItem tgti1 = null;

                string Team1Name = "";
                int tGroupId = -1;
                bool approvedPublishedAndNotDeleted = false;

                using (DefaultContext dc5 = new DefaultContext())
                {
                    BidTeamToTournament bttt = dc5.BidTeamToTournaments
                        .FirstOrDefault(bid => bid.Id == bidTeam1ToTournamentId);
                    Team1Name = bttt.TeamName;
                    tGroupId = bttt.TournamentGroupId;
                    approvedPublishedAndNotDeleted = bttt.Approved && !bttt.Deleted && !bttt.Published;
                }

                // ищем строки в турнирной таблице
                using (DefaultContext dc1 = new DefaultContext())
                {
                    tgti1 = dc1.TournamentGroupTableItems.FirstOrDefault(row => ((row.BidTeamToTournamentId == bidTeam1ToTournamentId) && (row.TournamentGroupId == tGroupId)));
                }


                if ((tgti1 == null) && (tGroupId != -1) && approvedPublishedAndNotDeleted) // если команда 1 еще не занесена в игровую таблицу
                {
                    tgti1 = new TournamentGroupTableItem();
                    tgti1.Deleted = false;
                    tgti1.Published = true;
                    tgti1.TournamentGroupId = tGroupId;
                    tgti1.BidTeamToTournamentId = bidTeam1ToTournamentId;
                    tgti1.Games = 0;
                    tgti1.Wins = 0;
                    tgti1.Loses = 0;
                    tgti1.Draws = 0;
                    tgti1.Place = 0;
                    tgti1.GoalsScored = 0;
                    tgti1.GoalsMissed = 0;
                    tgti1.GoalsDifference = 0;
                    tgti1.Points = 0;
                    tgti1.TeamName = Team1Name;

                    dc.TournamentGroupTableItems.Add(tgti1);
                    dc.SaveChanges();
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                    return false;
                }
                

                
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return false;
            }


            return true;
            // ======================= добавление строки в турнирную таблицу =============================
        }



        [HttpOptions]
        public dynamic DeleteTeamFromTournament()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Удаляет команду из турнира (Отклоняет, ранее одобренную, заявку на турнир (команда, группа турнира и профиль админа турнира должны быть переданы))
        /// </summary>
        /// <param name="userProfileTeamTournamentGroup">Профиль, группа турнира и команда</param>
        /// <returns></returns>
        public dynamic DeleteTeamFromTournament(UserProfileTeamTournamentGroup userProfileTeamTournamentGroup)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileTeamTournamentGroup.userProfile.UserProfileId);
            TournamentGroup tGroup = dc.TournamentGroups.FirstOrDefault(x => x.Id == userProfileTeamTournamentGroup.tournamentGroup.Id);
            Tournament tourn = null;

            if (tGroup != null)
                tourn = dc.Tournaments.FirstOrDefault(x => x.Id == tGroup.TournamentId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (tourn != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, tourn))
                )
            {
                //int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    BidTeamToTournament bid = dc.BidTeamToTournaments.Where(x => ((x.TeamId == userProfileTeamTournamentGroup.team.Id)
                     && (x.TournamentGroupId == tGroup.Id)
                     && (x.Published == false)
                     && (x.Approved == true)
                     )).FirstOrDefault();

                    bid.AdminTournamentComment = userProfileTeamTournamentGroup.admintext;
                    bid.Published = false;
                    bid.Approved = false;
                    bid.Team = null;
                    bid.TournamentGroup = null;
                    bid.UserProfile = null;
                    dc.Entry(bid).State = EntityState.Modified;
                    dc.SaveChanges();

                    return new SimpleTournamentController().GetTeamsByTournament(new CreateTournamentModel() { tournament = tourn, userProfile = uProfile });


                    //return bid;
                }
            }
            else
            {
                return new BidTeamToTournament("Error in DeleteTeamFromTournament: not authorized");
            }
        }


        [HttpOptions]
        public dynamic DeclineBid()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Отклоняет заявку на турнир (турнир и профиль админа турнира должны быть переданы)
        /// </summary>
        /// <param name="userProfileTournamentBidTeamToTournament">Профиль, турнир и заявка запросившего</param>
        /// <returns></returns>
        public dynamic DeclineBid(UserProfileTournamentBidTeamToTournament userProfileTournamentBidTeamToTournament)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileTournamentBidTeamToTournament.userProfile.UserProfileId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, userProfileTournamentBidTeamToTournament.tournament))
                )
                {
                    //int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                    using (DefaultContext dc = new DefaultContext())
                    {
                        BidTeamToTournament bid = dc.BidTeamToTournaments.Where(x => x.Id == userProfileTournamentBidTeamToTournament.bid.Id).FirstOrDefault();

                        bid.AdminTournamentComment = userProfileTournamentBidTeamToTournament.bid.AdminTournamentComment;
                        bid.Published = false;
                        bid.Approved = false;
                        bid.Team = null;
                        bid.TournamentGroup = null;
                        bid.UserProfile = null;
                        dc.Entry(bid).State = EntityState.Modified;
                        dc.SaveChanges();
                    
                        

                        return GetBidById(bid.Id);
                    }
                }
            else
            {
                return new BidTeamToTournament("Error in AcceptBid: not authorized");
            }
        }

        
        [HttpPost]
        /// <summary>
        /// Возвращает актуальные турниры (команда и профиль админа команды должны быть переданы)
        /// </summary>
        /// <param name="createTeamModel">Профиль запросившего и его команда</param>
        /// <returns></returns>
        public List<TournamentGroup> GetTournamentGroups()
        {
            List<TournamentGroup> tournamentGroups = new List<TournamentGroup>() { new TournamentGroup("Error in SimpeBidTeamToTournamentGetTournamentGroups: No groups") };
            int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;
            int tournamentId = (currentRequest.Form.Get("tournamentId") != null) ? Common.StrToInt(currentRequest.Form.GetValues("tournamentId")[0]) : -1;


            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (tournamentId != -1)
              )
                {
                //int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;


                tournamentGroups = dc.TournamentGroups
                    //.Where(t => ((t.Deleted == false) && (t.Published == true)))
                    .Where(t => t.TournamentId == tournamentId)
                    .ToList();
                    
                }
            else
            {
                return new List<TournamentGroup>() {  new TournamentGroup("Error in SimpeBidTeamToTournamentGetTournamentGroups: Not authorized") };
            }

            return tournamentGroups;
        }


        [HttpOptions]
        public dynamic GetTeamBidsByTeam()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Возвращает заявки от команды
        /// </summary>
        /// <returns></returns>
        public dynamic GetTeamBidsByTeam(CreateTeamModel createTeamModel)
        {
            
            //int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;
            //int teamId = (currentRequest.Form.Get("teamId") != null) ? Common.StrToInt(currentRequest.Form.GetValues("teamId")[0]) : -1;


            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (createTeamModel != null)
                && (new Common().UserProfileHasAccessToTeam(createTeamModel.userProfile, createTeamModel.team))
              )
                {

                return dc.BidTeamToTournaments
                    //.Include(x => x.Team)
                    //.Include(x => x.TournamentGroup)
                    //.Include(x => x.TournamentGroup.Tournament)
                    //.Include(x => x.UserProfile)
                    .Where(t => t.TeamId == createTeamModel.team.Id)
                    .Where(t => t.Deleted == false)
                    .Join(dc.Teams, b => b.TeamId, t => t.Id, (bid, team) => new { bid, team})
                    .Join(dc.TournamentGroups, b => b.bid.TournamentGroupId, tg => tg.Id, (bidteam, tgroup) => new { bidteam, tgroup})
                    .Join(dc.Tournaments, b => b.tgroup.TournamentId, to => to.Id, (bidteamtgroup, tourn) => new { bidteamtgroup, tourn})
                    .Join(dc.UserProfiles, tg => tg.bidteamtgroup.bidteam.bid.UserProfileId, u => u.UserProfileId, (bidteamtgrouptourn, uprofile) => new { bidteamtgrouptourn, uprofile})
                    .Select(all => new
                    {
                        Id = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.Id,
                        TeamId = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.TeamId,
                        TournamentGroupId = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.TournamentGroupId,
                        When = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.When,
                        Team = all.bidteamtgrouptourn.bidteamtgroup.bidteam.team,
                        Approved = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.Approved,
                        UserProfile = all.uprofile,
                        UserProfileId = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.UserProfileId,
                        ErrorMessage = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.ErrorMessage,
                        AdminTournamentComment = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.AdminTournamentComment,
                        TournamentGroup = new {
                            Id = all.bidteamtgrouptourn.bidteamtgroup.tgroup.Id,
                            ErrorMessage = all.bidteamtgrouptourn.bidteamtgroup.tgroup.ErrorMessage,
                            Deleted = all.bidteamtgrouptourn.bidteamtgroup.tgroup.Deleted,
                            Name = all.bidteamtgrouptourn.bidteamtgroup.tgroup.Name,
                            Published = all.bidteamtgrouptourn.bidteamtgroup.tgroup.Published,
                            Tournament = all.bidteamtgrouptourn.tourn,
                            TournamentId = all.bidteamtgrouptourn.bidteamtgroup.tgroup.TournamentId,
                        },
                        Published = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.Published,
                        Deleted = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.Deleted,
                        TeamName = all.bidteamtgrouptourn.bidteamtgroup.bidteam.bid.TeamName
                    })
                    .ToList();
                // .Select и включить туда Tournament
                    
                }
            else
            {
                return new List<BidTeamToTournament>() {  new BidTeamToTournament("Error in SimpeBidTeamToTournamentGetTeamBidsByTeam: Not authorized") };
            }

            //return new List<BidTeamToTournament>() { new BidTeamToTournament("Error in SimpeBidTeamToTournamentGetTeamBidsByTeam: No bids") };
        }


    }
}
