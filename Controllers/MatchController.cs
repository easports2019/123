using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
    public class MatchController : UmbracoApiController
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
        /// Добавляет в расписание матч (Внимание! Изменен источник PlaceId. В будущем доработать!)
        /// </summary>
        /// <param name="matchUserProfile">Данные матча, группа турнира и профиля создателя</param>
        /// <returns></returns>
        public dynamic Add(MatchUserProfile matchUserProfile)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == matchUserProfile.userProfile.UserProfileId);
            Tournament tourn = dc.TournamentGroups
                .Where(x => x.Id == matchUserProfile.match.TournamentGroupId)
                .Join(dc.Tournaments, a => a.TournamentId, b => b.Id, (tgr, t) => new { tgr, t })
                .Select(x => x.t)
                .FirstOrDefault();


            int id = -1;
            Match mat = null;

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (tourn != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, tourn))
                )
            {
                try
                {
                    mat = dc.Matches
                        .Where(x => (x.Team1Id == matchUserProfile.match.Team1Id)
                        && (x.Team2Id == matchUserProfile.match.Team2Id)
                        && (x.TournamentGroupId == matchUserProfile.match.TournamentGroupId))
                        .FirstOrDefault(); // ищем такой же матч. если нашли, значит его не нужно создавать еще раз
                }
                catch (Exception ex)
                {
                    return new Match("Error in MatchAdd: " + ex.Message);
                }

                if (mat == null) // добавление нового матча, если такого еще нет.
                {
                    dc.Database.BeginTransaction();

                    Match newMatch = new Match();

                    try
                    {


                        newMatch.Deleted = false;
                        newMatch.Description = matchUserProfile.match.Description;
                        newMatch.ErrorMessage = "";
                        newMatch.Name = matchUserProfile.match.Name;
                        newMatch.Picture = matchUserProfile.match.Picture;
                        newMatch.UmbracoPlaceId = matchUserProfile.match.PlaceId.HasValue ? matchUserProfile.match.PlaceId.Value : -1;

                        // пока null, потом изменим
                        newMatch.PlaceId = null;

                        newMatch.Published = false;
                        newMatch.Team1Id = matchUserProfile.match.Team1Id;
                        newMatch.Team2Id = matchUserProfile.match.Team2Id;
                        newMatch.Team1Goals = 0;
                        newMatch.Team2Goals = 0;
                        newMatch.TournamentGroupId = matchUserProfile.match.TournamentGroupId;
                        newMatch.When = matchUserProfile.match.When;

                        newMatch.TournamentGroup = null;
                        newMatch.Team1 = null;
                        newMatch.Team2 = null;
                        newMatch.Place = null;
                        /*  таблица мест не заполнена, т.к. места мы тащим из базы данных умбрако. нужно либо писать места в базу, 
                         *  либо убрать объект Place и писать только Id Place   */

                        newMatch.MatchEvents = null;

                        dc.Matches.Add(newMatch);

                        dc.SaveChanges();
                        dc.Database.CurrentTransaction.Commit();
                        id = newMatch.Id;

                    }
                    catch (Exception ex)
                    {
                        dc.Database.CurrentTransaction.Rollback();
                        return new Match("Error in MatchAdd: " + ex.Message);

                    }

                    // сделать проверку на повторное добавление отклоненной заявки. возможно заявка та же, модифицируем лишь группу

                }
                else // редактирование существующей
                {

                    dc.Database.BeginTransaction();

                    try
                    {

                        mat.Deleted = false;
                        mat.Description = matchUserProfile.match.Description;
                        mat.ErrorMessage = "";
                        mat.Name = matchUserProfile.match.Name;
                        mat.Picture = matchUserProfile.match.Picture;

                        mat.UmbracoPlaceId = matchUserProfile.match.PlaceId.HasValue ? matchUserProfile.match.PlaceId.Value : -1;

                        // пока null, потом изменим
                        mat.PlaceId = null;

                        mat.Published = false;
                        mat.Team1Id = matchUserProfile.match.Team1Id;
                        mat.Team2Id = matchUserProfile.match.Team2Id;
                        mat.Team1Goals = matchUserProfile.match.Team1Goals;
                        mat.Team2Goals = matchUserProfile.match.Team2Goals;
                        mat.TournamentGroupId = matchUserProfile.match.TournamentGroupId;
                        mat.When = matchUserProfile.match.When;

                        mat.TournamentGroup = null;
                        mat.Team1 = null;
                        mat.Team2 = null;
                        mat.Place = null;
                        mat.MatchEvents = null;

                        dc.Entry(mat).State = EntityState.Modified;
                        dc.SaveChanges();



                        dc.Database.CurrentTransaction.Commit();
                        id = mat.Id;
                    }
                    catch (Exception ex)
                    {
                        dc.Database.CurrentTransaction.Rollback();
                        return new Match("Error in MatchAdd(Edit): " + ex.Message);

                    }
                }

                //dc.Database.CurrentTransaction.Commit();

                try
                {
                    var r = dc.Matches
                        .Where(x => x.Id == id)
                        .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (bd, trg) => new { bd, trg })
                        .Join(dc.Teams, a => a.bd.Team1Id, b => b.Id, (bidtrg, team1) => new { bidtrg, team1 })
                        .Join(dc.Teams, a => a.bidtrg.bd.Team2Id, b => b.Id, (bidtrgteam1, team2) => new { bidtrgteam1, team2 })
                        .Join(dc.Tournaments, a => a.bidtrgteam1.bidtrg.trg.TournamentId, trn => trn.Id, (bidtrgteams, trn) => new { bidtrgteams, trn })
                        .Select(a => new
                        {
                            Id = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Id,
                            //TournamentGroup = a.bidtrgteam.bidtrg.trg,

                            TournamentGroup = new
                            {
                                Id = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Id,
                                ErrorMessage = a.bidtrgteams.bidtrgteam1.bidtrg.trg.ErrorMessage,
                                Deleted = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Deleted,
                                Name = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Name,
                                Published = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Published,
                                Tournament = new
                                {
                                    TournamentId = a.trn.Id,
                                    Name = a.trn.Name,
                                    OrganizatorName = a.trn.OrganizatorName,
                                    OrganizatorNameShort = a.trn.OrganizatorNameShort,
                                    Link = a.trn.Link,
                                    Link2 = a.trn.Link2,
                                    Link2Name = a.trn.Link2Name,
                                    Logo = a.trn.Logo,
                                    Founder = a.trn.Founder,
                                    Details = a.trn.Details
                                },
                                TournamentId = a.bidtrgteams.bidtrgteam1.bidtrg.trg.TournamentId,
                            },
                            //TournamentId = a.bidtrgteam.bidtrg.trg.TournamentId,
                            Team1 = a.bidtrgteams.bidtrgteam1.team1,
                            Team2 = a.bidtrgteams.team2,
                            Team1Goals = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Team1Goals,
                            Team2Goals = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Team2Goals,
                            When = a.bidtrgteams.bidtrgteam1.bidtrg.bd.When,
                            Deleted = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Deleted,
                            Published = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Published,
                            TournamentGroupId = a.bidtrgteams.bidtrgteam1.bidtrg.bd.TournamentGroupId,
                            PlaceId = a.bidtrgteams.bidtrgteam1.bidtrg.bd.UmbracoPlaceId, // обрати внимание откуда берется PlaceId!!
                        })
                        .FirstOrDefault()
                        ;
                    return r;
                }
                catch (Exception ex)
                {
                    return new Match("Error in MatchAdd(Return): " + ex.Message);
                }

            }
            else
            {
                return new Match("Error in MatchAdd: not authorized");
            }
        }


        [HttpOptions]
        public dynamic Add2()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Добавляет в расписание матч (Внимание! Изменен источник PlaceId. В будущем доработать!)
        /// </summary>
        /// <param name="matchUserProfile">Данные матча, группа турнира и профиля создателя</param>
        /// <returns></returns>
        public dynamic Add2(MatchUserProfile matchUserProfile)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == matchUserProfile.userProfile.UserProfileId);
            Tournament tourn = dc.TournamentGroups
                .Where(x => x.Id == matchUserProfile.match.TournamentGroupId)
                .Join(dc.Tournaments, a => a.TournamentId, b => b.Id, (tgr, t) => new { tgr, t })
                .Select(x => x.t)
                .FirstOrDefault();


            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (tourn != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, tourn))
                )
            {
                return AddMatch(matchUserProfile.match);

            }
            else
            {
                return new Match("Error in MatchAdd: not authorized");
            }
        }

        private dynamic AddMatch(Match match)
        {
            int id = -1;
            Match mat = null;

            // запрашиваем матч с этими двумя командами. если есть, тогда mat != null
            // если нет, тогда mat == null
            try
            {
                mat = dc.Matches
                    .Where(x => (x.Team1Id == match.Team1Id)
                    && (x.Team2Id == match.Team2Id)
                    && (x.TournamentGroupId == match.TournamentGroupId))
                    .FirstOrDefault(); // ищем такой же матч. если нашли, значит его не нужно создавать еще раз
            }
            catch (Exception ex)
            {
                return new Match("Error in MatchAdd: " + ex.Message);
            }

            int tgId = -1, bidId1 = -1, bidId2 = -1;
            // вычисляем bidId1 и bidId2 (ид заявок в турнир)
            using (DefaultContext dc1 = new DefaultContext())
            {

                tgId = match.TournamentGroupId; // занести сюда Id заявки и Id группы турнира!
                bidId1 = match.BidTeamToTournamentId1 == -1
                   ?
                   dc1.BidTeamToTournaments.AsNoTracking().FirstOrDefault(x => ((x.TournamentGroupId == tgId) && (x.TeamId == match.Team1Id))).Id
                   :
                   match.BidTeamToTournamentId1;
                bidId2 = match.BidTeamToTournamentId2 == -1
                   ?
                   dc1.BidTeamToTournaments.AsNoTracking().FirstOrDefault(x => ((x.TournamentGroupId == tgId) && (x.TeamId == match.Team2Id))).Id
                   :
                   match.BidTeamToTournamentId2;
            }

            // если матча нет в таблице, тогда
            if (mat == null) // добавление нового матча, если такого еще нет.
            {
                // создаем объект матча
                Match newMatch = new Match();

                try
                {
                    dc.Database.BeginTransaction();


                    //AddOrEditRowInTournamentTable(bidId1, bidId2, tgId, match.When);
                    // ищем в таблице результатов команды. возвращаются в массиве на добавление или редактирование
                    TournamentGroupTableItem[] rows = CalculateRowInTournamentTable(bidId1, bidId2, match);

                    newMatch.Deleted = false;
                    newMatch.Description = match.Description;
                    newMatch.ErrorMessage = "";
                    newMatch.Name = match.Name;
                    newMatch.Picture = match.Picture;
                    newMatch.UmbracoPlaceId = match.UmbracoPlaceId > 0 ? match.UmbracoPlaceId : match.PlaceId.Value;
                        //.PlaceId.HasValue ? match.PlaceId.Value : -1;

                    // пока null, потом изменим
                    newMatch.PlaceId = null;

                    newMatch.Played = match.Played;
                    newMatch.Published = false;
                    newMatch.Team1Id = match.Team1Id;
                    newMatch.Team2Id = match.Team2Id;
                    newMatch.BidTeamToTournamentId1 = bidId1;
                    newMatch.BidTeamToTournamentId2 = bidId2;
                    newMatch.Team1Goals = match.Team1Goals;
                    newMatch.Team2Goals = match.Team2Goals;
                    newMatch.TournamentGroupId = match.TournamentGroupId;
                    newMatch.When = match.When;

                    newMatch.TournamentGroup = null;
                    newMatch.Team1 = null;
                    newMatch.Team2 = null;
                    newMatch.Place = null;
                    newMatch.BidTeamToTournament1 = null;
                    newMatch.BidTeamToTournament2 = null;
                    newMatch.MatchEvents = null;

                    /*  таблица мест не заполнена, т.к. места мы тащим из базы данных умбрако. нужно либо писать места в базу, 
                     *  либо убрать объект Place и писать только Id Place   */

                    // команды на добавление в таблицу результатов лежат в 0 и 1 элементе массива
                    //if (rows[0].BidTeamToTournamentId != -1)
                    //    dc.TournamentGroupTableItems.Add(rows[0]);
                    //if (rows[1].BidTeamToTournamentId != -1)
                    //    dc.TournamentGroupTableItems.Add(rows[1]);

                    // команды на редактирование в таблице результатов лежат во 2 и 3 элементе массива
                    // хуй знает, что за проблема, но если выбираем редактирование, ругается длинным
                    // предложением на аттач и не хочет редактировать. поэтому внутри сделаем 
                    // новый запрос по Id и заново пропишем значения полей
                    //if (rows[2].BidTeamToTournamentId != -1)
                    //{
                    //    TournamentGroupTableItem tgiTemp = rows[2];
                    //    TournamentGroupTableItem tgi = null;
                    //    using (DefaultContext defC = new DefaultContext())
                    //    {
                    //        tgi = defC.TournamentGroupTableItems
                    //            .FirstOrDefault(x => x.Id == tgiTemp.Id);
                    //        tgi.BidTeamToTournamentId = tgiTemp.BidTeamToTournamentId;
                    //        tgi.Deleted = tgiTemp.Deleted;
                    //        tgi.Draws = tgiTemp.Draws;
                    //        tgi.TeamName = "";
                    //        tgi.Games = tgiTemp.Games;
                    //        tgi.GoalsDifference = tgiTemp.GoalsDifference;
                    //        tgi.GoalsMissed = tgiTemp.GoalsMissed;
                    //        tgi.GoalsScored = tgiTemp.GoalsScored;
                    //        tgi.Loses = tgiTemp.Loses;
                    //        tgi.Points = tgiTemp.Points;
                    //        tgi.Published = tgiTemp.Published;
                    //        tgi.TeamName = tgiTemp.TeamName;
                    //        tgi.TournamentGroupId = tgiTemp.TournamentGroupId;
                    //        tgi.Wins = tgiTemp.Wins;
                    //        tgi.Place = null;
                    //    }
                    //    if (tgi != null)
                    //        dc.Entry(tgi).State = EntityState.Modified;
                    //}
                    //if (rows[3].BidTeamToTournamentId != -1)
                    //{
                    //    TournamentGroupTableItem tgiTemp = rows[3];
                    //    TournamentGroupTableItem tgi = null;
                    //    using (DefaultContext defC = new DefaultContext())
                    //    {
                    //        tgi = defC.TournamentGroupTableItems
                    //            .FirstOrDefault(x => x.Id == tgiTemp.Id);
                    //        tgi.BidTeamToTournamentId = tgiTemp.BidTeamToTournamentId;
                    //        tgi.Deleted = tgiTemp.Deleted;
                    //        tgi.Draws = tgiTemp.Draws;
                    //        tgi.TeamName = "";
                    //        tgi.Games = tgiTemp.Games;
                    //        tgi.GoalsDifference = tgiTemp.GoalsDifference;
                    //        tgi.GoalsMissed = tgiTemp.GoalsMissed;
                    //        tgi.GoalsScored = tgiTemp.GoalsScored;
                    //        tgi.Loses = tgiTemp.Loses;
                    //        tgi.Points = tgiTemp.Points;
                    //        tgi.Published = tgiTemp.Published;
                    //        tgi.TeamName = tgiTemp.TeamName;
                    //        tgi.TournamentGroupId = tgiTemp.TournamentGroupId;
                    //        tgi.Wins = tgiTemp.Wins;
                    //        tgi.Place = null;
                    //    }
                    //    if (tgi != null)
                    //        dc.Entry(tgi).State = EntityState.Modified;
                    //}

                    dc.TournamentGroupTableItems.AddOrUpdate(rows);
                    //dc.Set<TournamentGroupTableItem>().AddOrUpdate()

                    // добавляем новый матч (его еще не было)
                    dc.Matches.Add(newMatch);

                    // сохраняем изменения (в тч в таблице результатов матчей)
                    dc.SaveChanges();
                    dc.Database.CurrentTransaction.Commit();
                    id = newMatch.Id;

                    // перерасчет мест в турнирной таблице
                    CalculatePlacesInTournamentTable(match.TournamentGroupId);

                }
                catch (Exception ex)
                {
                    dc.Database.CurrentTransaction.Rollback();
                    return new Match("Error in MatchAdd: " + ex.Message);

                }

                // сделать проверку на повторное добавление отклоненной заявки. возможно заявка та же, модифицируем лишь группу

            }
            else // если матч уже записан в таблицу матчей, редактируем его
            {
                
                try
                {
                    dc.Database.BeginTransaction();

                    // запрос существования команд в таблице результатов
                    // в 0 и 1 возвращаеются команды на добавление, во 2 и 3 на редактирование в таблице результатов
                    TournamentGroupTableItem[] rows = CalculateRowInTournamentTable(bidId1, bidId2, match);

                    // внесение изменений в существующий матч
                    mat.Deleted = false;
                    mat.Description = match.Description;
                    mat.ErrorMessage = "";
                    mat.Name = match.Name;
                    mat.Picture = match.Picture;

                    mat.UmbracoPlaceId = match.UmbracoPlaceId > 0 ? match.UmbracoPlaceId : match.PlaceId.Value;
                        //.PlaceId.HasValue ? match.PlaceId.Value : -1;

                    // пока null, потом изменим
                    mat.PlaceId = null;

                    mat.Published = false;
                    mat.Team1Id = match.Team1Id;
                    mat.Team2Id = match.Team2Id;
                    mat.Team1Goals = match.Team1Goals;
                    mat.Team2Goals = match.Team2Goals;
                    mat.BidTeamToTournamentId1 = bidId1;
                    mat.BidTeamToTournamentId2 = bidId2;
                    mat.TournamentGroupId = match.TournamentGroupId;
                    mat.When = match.When;
                    mat.Played = match.Played;

                    mat.TournamentGroup = null;
                    mat.Team1 = null;
                    mat.Team2 = null;
                    mat.Place = null;
                    mat.MatchEvents = null;
                    mat.BidTeamToTournament1 = null;
                    mat.BidTeamToTournament2 = null;

                    // говорим фреймворку, что надо внести изменения в этот существущий матч
                    dc.Entry(mat).State = EntityState.Modified;

                    dc.TournamentGroupTableItems.AddOrUpdate(rows);

                    // в 0 и 1 элементе массива лежат команды на добавление в турнирную таблицу
                    //if (rows[0].BidTeamToTournamentId != -1)
                    //    dc.TournamentGroupTableItems.Add(rows[0]);
                    //if (rows[1].BidTeamToTournamentId != -1)
                    //    dc.TournamentGroupTableItems.Add(rows[1]);

                    //// команды на редактирование в таблице результатов лежат во 2 и 3 элементе массива
                    //// хуй знает, что за проблема, но если выбираем редактирование, ругается длинным
                    //// предложением на аттач и не хочет редактировать. поэтому внутри сделаем 
                    //// новый запрос по Id и заново пропишем значения полей
                    //if (rows[2].BidTeamToTournamentId != -1)
                    //{
                    //    TournamentGroupTableItem tgiTemp = rows[2];
                    //    TournamentGroupTableItem tgi = null;
                    //    using (DefaultContext defC = new DefaultContext())
                    //    {
                    //        tgi = defC.TournamentGroupTableItems
                    //            .FirstOrDefault(x => x.Id == tgiTemp.Id);
                    //        tgi.BidTeamToTournamentId = tgiTemp.BidTeamToTournamentId;
                    //        tgi.Deleted = tgiTemp.Deleted;
                    //        tgi.Draws = tgiTemp.Draws;
                    //        tgi.TeamName = "";
                    //        tgi.Games = tgiTemp.Games;
                    //        tgi.GoalsDifference = tgiTemp.GoalsDifference;
                    //        tgi.GoalsMissed = tgiTemp.GoalsMissed;
                    //        tgi.GoalsScored = tgiTemp.GoalsScored;
                    //        tgi.Loses = tgiTemp.Loses;
                    //        tgi.Points = tgiTemp.Points;
                    //        tgi.Published = tgiTemp.Published;
                    //        tgi.TeamName = tgiTemp.TeamName;
                    //        tgi.TournamentGroupId = tgiTemp.TournamentGroupId;
                    //        tgi.Wins = tgiTemp.Wins;
                    //        tgi.Place = null;
                    //    }
                    //    if (tgi != null)
                    //        dc.Entry(tgi).State = EntityState.Modified;
                    //}
                    //if (rows[3].BidTeamToTournamentId != -1)
                    //{
                    //    TournamentGroupTableItem tgiTemp = rows[3];
                    //    TournamentGroupTableItem tgi = null;
                    //    using (DefaultContext defC = new DefaultContext())
                    //    {
                    //        tgi = defC.TournamentGroupTableItems
                    //            .FirstOrDefault(x => x.Id == tgiTemp.Id);
                    //        tgi.BidTeamToTournamentId = tgiTemp.BidTeamToTournamentId;
                    //        tgi.Deleted = tgiTemp.Deleted;
                    //        tgi.Draws = tgiTemp.Draws;
                    //        tgi.TeamName = "";
                    //        tgi.Games = tgiTemp.Games;
                    //        tgi.GoalsDifference = tgiTemp.GoalsDifference;
                    //        tgi.GoalsMissed = tgiTemp.GoalsMissed;
                    //        tgi.GoalsScored = tgiTemp.GoalsScored;
                    //        tgi.Loses = tgiTemp.Loses;
                    //        tgi.Points = tgiTemp.Points;
                    //        tgi.Published = tgiTemp.Published;
                    //        tgi.TeamName = tgiTemp.TeamName;
                    //        tgi.TournamentGroupId = tgiTemp.TournamentGroupId;
                    //        tgi.Wins = tgiTemp.Wins;
                    //        tgi.Place = null;
                    //    }
                    //    if (tgi != null)
                    //        dc.Entry(tgi).State = EntityState.Modified;
                    //}

                    //// в 2 и 3 элементе массива лежат команды на изменение существующих команд в турнирной таблице
                    //if (rows[2].BidTeamToTournamentId != -1)
                    //    dc.Entry(rows[2]).State = EntityState.Modified;
                    //if (rows[3].BidTeamToTournamentId != -1)
                    //    dc.Entry(rows[3]).State = EntityState.Modified;


                    dc.ChangeTracker.DetectChanges();
                    var h = dc.ChangeTracker.Entries();

                    // очень много изменений приходит. надо выносить запросы с FirstOrDefault() за пределы кода. чтобы все приходило извне (возможно
                    // подпроцедуру сделать изменения, чтобы текущий контекст закрылся от всего остального)
                    // то есть все запрашиваем и передаем в виде параметров внутрь другой функции, на выходе получаем добавленный объект
                    //IEnumerable <System.Data.Entity.Infrastructure.DbEntityEntry>
                    dc.SaveChanges();



                    dc.Database.CurrentTransaction.Commit();
                    id = mat.Id;

                    // пересчитываем места в турнирной таблице
                    CalculatePlacesInTournamentTable(match.TournamentGroupId);
                }
                catch (Exception ex)
                {
                    dc.Database.CurrentTransaction.Rollback();
                    return new Match("Error in MatchAdd(Edit): " + ex.Message);

                }
            }

            //dc.Database.CurrentTransaction.Commit();
            // возвращаем информацию о добавленном матче
            try
            {
                var r = dc.Matches
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (bd, trg) => new { bd, trg })
                    .Join(dc.Teams, a => a.bd.Team1Id, b => b.Id, (bidtrg, team1) => new { bidtrg, team1 })
                    .Join(dc.Teams, a => a.bidtrg.bd.Team2Id, b => b.Id, (bidtrgteam1, team2) => new { bidtrgteam1, team2 })
                    .Join(dc.Tournaments, a => a.bidtrgteam1.bidtrg.trg.TournamentId, trn => trn.Id, (bidtrgteams, trn) => new { bidtrgteams, trn })
                    .Select(a => new
                    {
                        Id = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Id,
                            //TournamentGroup = a.bidtrgteam.bidtrg.trg,

                            TournamentGroup = new
                        {
                            Id = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Id,
                            ErrorMessage = a.bidtrgteams.bidtrgteam1.bidtrg.trg.ErrorMessage,
                            Deleted = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Deleted,
                            Name = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Name,
                            Published = a.bidtrgteams.bidtrgteam1.bidtrg.trg.Published,
                            Tournament = new
                            {
                                TournamentId = a.trn.Id,
                                Name = a.trn.Name,
                                OrganizatorName = a.trn.OrganizatorName,
                                OrganizatorNameShort = a.trn.OrganizatorNameShort,
                                Link = a.trn.Link,
                                Link2 = a.trn.Link2,
                                Link2Name = a.trn.Link2Name,
                                Logo = a.trn.Logo,
                                Founder = a.trn.Founder,
                                Details = a.trn.Details
                            },
                            TournamentId = a.bidtrgteams.bidtrgteam1.bidtrg.trg.TournamentId,
                        },
                            //TournamentId = a.bidtrgteam.bidtrg.trg.TournamentId,
                            Team1 = a.bidtrgteams.bidtrgteam1.team1,
                        Team2 = a.bidtrgteams.team2,
                        Team1BidId = a.bidtrgteams.bidtrgteam1.bidtrg.bd.BidTeamToTournamentId1,
                        Team2BidId = a.bidtrgteams.bidtrgteam1.bidtrg.bd.BidTeamToTournamentId2,
                        Team1Goals = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Team1Goals,
                        Team2Goals = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Team2Goals,
                        When = a.bidtrgteams.bidtrgteam1.bidtrg.bd.When,
                        Played = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Played,
                        Deleted = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Deleted,
                        Published = a.bidtrgteams.bidtrgteam1.bidtrg.bd.Published,
                        TournamentGroupId = a.bidtrgteams.bidtrgteam1.bidtrg.bd.TournamentGroupId,
                        PlaceId = a.bidtrgteams.bidtrgteam1.bidtrg.bd.UmbracoPlaceId, // обрати внимание откуда берется PlaceId!!
                        ErrorMessage = "",
                        })
                    .AsNoTracking()
                    .FirstOrDefault()
                    ;
                return r;
            }
            catch (Exception ex)
            {
                return new Match("Error in MatchAdd(Return): " + ex.Message);
            }
        }

        /// <summary>
        /// Расчет данных в строках команд турнирных таблиц
        /// </summary>
        /// <param name="bidTeam1ToTournamentId">Номер заявки от команды 1 в турнир</param>
        /// <param name="bidTeam2ToTournamentId">Номер заявки от команды 2 в турнир</param>
        /// <param name="match">матч</param>
        /// <returns></returns>
        public TournamentGroupTableItem[] CalculateRowInTournamentTable(int bidTeam1ToTournamentId, int bidTeam2ToTournamentId, Match match)
        {
            int goalsTeam1 = match.Team1Goals;
            int goalsTeam2 = match.Team2Goals;
            int tournamentGroupId = match.TournamentGroupId;
            DateTime whenMatch = match.When;
            bool played = match.Played;

            // ======================= добавление строки в турнирную таблицу =============================
            TournamentGroupTableItem[] tableRows = new TournamentGroupTableItem[2];
            tableRows[0] = new TournamentGroupTableItem();  // добавлена строка 1 команды
            tableRows[1] = new TournamentGroupTableItem();  // добавлена строка 2 команды
            //tableRows[2] = new TournamentGroupTableItem();  // изменена строка 1 команды
            //tableRows[3] = new TournamentGroupTableItem();  // изменена строка 2 команды

            try
            {

                TournamentGroupTableItem tgti1 = null, tgti2 = null;
                Match prevMatch = null;
                bool addTeam1 = false, addTeam2 = false;

                // ищем строки в турнирной таблице
                using (DefaultContext dc1 = new DefaultContext())
                {
                    tgti1 = dc1.TournamentGroupTableItems.FirstOrDefault(row => ((row.BidTeamToTournamentId == bidTeam1ToTournamentId) && (row.TournamentGroupId == tournamentGroupId)));
                    tgti2 = dc1.TournamentGroupTableItems.FirstOrDefault(row => ((row.BidTeamToTournamentId == bidTeam2ToTournamentId) && (row.TournamentGroupId == tournamentGroupId)));
                }

                string Team1Name = "", Team2Name = "";
                using (DefaultContext dc5 = new DefaultContext())
                {
                    Team1Name = dc5.BidTeamToTournaments
                        .AsNoTracking()
                        .FirstOrDefault(bid => bid.Id == bidTeam1ToTournamentId).TeamName;
                    Team2Name = dc5.BidTeamToTournaments
                        .AsNoTracking()
                        .FirstOrDefault(bid => bid.Id == bidTeam2ToTournamentId).TeamName;
                }

                // Проверяем предыдущее значение Played у матча.
                // Если оно было True, тогда надо удалить из таблицы результатов значения.
                using (DefaultContext dc4 = new DefaultContext())
                {
                    prevMatch = dc4.Matches
                        .AsNoTracking()
                        //.FirstOrDefault(row => row.Id == match.Id);
                        .FirstOrDefault(row => 
                        (row.BidTeamToTournamentId1 == match.BidTeamToTournamentId1
                        && row.BidTeamToTournamentId2 == match.BidTeamToTournamentId2
                        && row.TournamentGroupId == match.TournamentGroupId)
                        );
                }

                // проверяем, состоялся ли матч
                if (!played) // матч еще не состоялся, значит мы не можем считать очки от текущей игры,
                // значит, если строки нет еще, тогда добавляем ее, редактировать ничего не надо
                {
                    if ((prevMatch != null) && (prevMatch.Played)) 
                    // если этот матч уже был записан в базу и значение Played было true,
                    // значит его нужно убрать из таблицы результатов
                    {
                        int gDifference = prevMatch.Team1Goals - prevMatch.Team2Goals;
                        tgti1.Games -= 1;
                        tgti1.Wins -= gDifference > 0 ? 1 : 0;
                        tgti1.Loses -= gDifference < 0 ? 1 : 0;
                        tgti1.Draws -= gDifference == 0 ? 1 : 0;
                        tgti1.GoalsScored -= prevMatch.Team1Goals;
                        tgti1.GoalsMissed -= prevMatch.Team2Goals;
                        tgti1.GoalsDifference -= gDifference;
                        tgti1.Points -= gDifference > 0 ?
                            3 :
                            gDifference == 0 ? 1 : 0;

                        gDifference = prevMatch.Team2Goals - prevMatch.Team1Goals;
                        tgti2.Games -= 1;
                        tgti2.Wins -= gDifference > 0 ? 1 : 0;
                        tgti2.Loses -= gDifference < 0 ? 1 : 0;
                        tgti2.Draws -= gDifference == 0 ? 1 : 0;
                        tgti2.GoalsScored -= prevMatch.Team2Goals;
                        tgti2.GoalsMissed -= prevMatch.Team1Goals;
                        tgti2.GoalsDifference -= gDifference;
                        tgti2.Points -= gDifference > 0 ?
                            3 :
                            gDifference == 0 ? 1 : 0;
                    }

                    if (tgti1 == null) // если команда 1 еще не занесена в игровую таблицу
                    {
                        tgti1 = new TournamentGroupTableItem();
                        tgti1.Deleted = false;
                        tgti1.Published = true;
                        tgti1.TournamentGroupId = tournamentGroupId;
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

                        addTeam1 = true; // запоминаем то, что данных о команде 1 не было в таблице результатов
                    }

                    if (tgti2 == null) // если команда 1 еще не занесена в игровую таблицу
                    {
                        tgti2 = new TournamentGroupTableItem();
                        tgti2.Deleted = false;
                        tgti2.Published = true;
                        tgti2.TournamentGroupId = tournamentGroupId;
                        tgti2.BidTeamToTournamentId = bidTeam2ToTournamentId;
                        tgti2.Games = 0;
                        tgti2.Wins = 0;
                        tgti2.Loses = 0;
                        tgti2.Draws = 0;
                        tgti2.Place = 0;
                        tgti2.GoalsScored = 0;
                        tgti2.GoalsMissed = 0;
                        tgti2.GoalsDifference = 0;
                        tgti2.Points = 0;
                        tgti2.TeamName = Team2Name;

                        addTeam2 = true; // запоминаем то, что данных о команде 2 не было в таблице результатов
                    }

                }
                else // матч состоялся или идет
                {
                    // ищем этот матч, записанный в базу, чтобы потом скорректировать счёт и очки.
                    Match tempMatch = null;
                    using (DefaultContext dc3 = new DefaultContext())
                    {
                        tempMatch = dc3.Matches.Where(x =>
                        (x.TournamentGroupId == tournamentGroupId
                        && (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId)
                        && (x.BidTeamToTournamentId2 == bidTeam2ToTournamentId)
                        ))
                        .AsNoTracking()
                        .FirstOrDefault();
                        //.Select(x => new
                        //{
                        //    TeamBidId = bidTeam1ToTournamentId,
                        //    When = x.When,
                        //    Scored = (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId) ? x.Team1Goals : x.Team2Goals,
                        //    Missed = (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId) ? x.Team2Goals : x.Team1Goals,
                        //    GoalsDifferenceInGame = (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId) ? x.Team1Goals - x.Team2Goals : x.Team2Goals - x.Team1Goals
                        //});
                    }


                    if (tgti1 == null) // если команда 1 еще не занесена в игровую таблицу
                    {
                        tgti1 = new TournamentGroupTableItem();
                        tgti1.Deleted = false;
                        tgti1.Published = true;
                        tgti1.TournamentGroupId = tournamentGroupId;
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

                        // учет текущей игры
                        int goalDifference = goalsTeam1 - goalsTeam2;

                        tgti1.Games += 1;
                        tgti1.Wins += goalDifference > 0 ? 1 : 0;
                        tgti1.Loses += goalDifference < 0 ? 1 : 0;
                        tgti1.Draws += goalDifference == 0 ? 1 : 0;
                        tgti1.GoalsScored += goalsTeam1;
                        tgti1.GoalsMissed += goalsTeam2;
                        tgti1.GoalsDifference += goalDifference;
                        tgti1.Points += goalDifference > 0 ?
                            3 :
                            goalDifference == 0 ? 1 : 0;

                        addTeam1 = true; // запоминаем то, что данных о команде 1 не было в таблице результатов
                    }
                    else // строка в таблице результатов уже существует
                    {
                        tgti1.Deleted = false;
                        tgti1.Published = true;
                        tgti1.TournamentGroupId = tournamentGroupId;
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


                        // учет текущей игры
                        int goalDifference = goalsTeam1 - goalsTeam2;


                        // это уже не актуально, т.к. в таблице уже создана запись расписания игры этих команд и там ничья...
                        // нужно найти эту строку и не учитывать ее при расчете. а ведь там может быть и не ничья. там может быть счет какой-то. 

                        tgti1.Games += 1;
                        tgti1.Wins += goalDifference > 0 ? 1 : 0;
                        tgti1.Loses += goalDifference < 0 ? 1 : 0;
                        tgti1.Draws += goalDifference == 0 ? 1 : 0;
                        tgti1.GoalsScored += goalsTeam1;
                        tgti1.GoalsMissed += goalsTeam2;
                        tgti1.GoalsDifference += goalDifference;
                        tgti1.Points += goalDifference > 0 ?
                            3 :
                            goalDifference == 0 ? 1 : 0;

                        // получаем данные из базы матчей и считаем очки на их основе
                        using (DefaultContext dc2 = new DefaultContext())
                        {
                            // нужно получить из базы (таблица матчей) данные об играх этой команды в этом турнире
                            var teamSeasonInfo1 = dc2.Matches
                                .AsNoTracking()
                                .Where(x => (x.TournamentGroupId == tournamentGroupId
                                    && (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId || x.BidTeamToTournamentId2 == bidTeam1ToTournamentId)
                                    && (x.Played)
                                    ))
                                .Select(x => new
                                {
                                    TeamBidId = bidTeam1ToTournamentId,
                                    When = x.When,
                                    Scored = (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId) ? x.Team1Goals : x.Team2Goals,
                                    Missed = (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId) ? x.Team2Goals : x.Team1Goals,
                                    GoalsDifferenceInGame = (x.BidTeamToTournamentId1 == bidTeam1ToTournamentId) ? x.Team1Goals - x.Team2Goals : x.Team2Goals - x.Team1Goals
                                });

                            foreach (var tsi1 in teamSeasonInfo1)
                            {
                                tgti1.Games += 1;
                                tgti1.Wins += tsi1.GoalsDifferenceInGame > 0 ? 1 : 0;
                                tgti1.Loses += tsi1.GoalsDifferenceInGame < 0 ? 1 : 0;
                                tgti1.Draws += tsi1.GoalsDifferenceInGame == 0 ? 1 : 0;
                                tgti1.GoalsScored += tsi1.Scored;
                                tgti1.GoalsMissed += tsi1.Missed;
                                tgti1.GoalsDifference += tsi1.GoalsDifferenceInGame;
                                tgti1.Points += tsi1.GoalsDifferenceInGame > 0 ?
                                    3 :
                                    tsi1.GoalsDifferenceInGame == 0 ? 1 : 0;
                            }
                        }


                        // корректируем данные команды 1, учитывая прошлую запись об этом матче в базе данных
                        if (tempMatch != null)
                        {
                            goalDifference = tempMatch.Team1Goals - tempMatch.Team2Goals;
                            tgti1.Games -= 1;
                            tgti1.Wins -= goalDifference > 0 ? 1 : 0;
                            tgti1.Loses -= goalDifference < 0 ? 1 : 0;
                            tgti1.Draws -= goalDifference == 0 ? 1 : 0;
                            tgti1.GoalsScored -= tempMatch.Team1Goals;
                            tgti1.GoalsMissed -= tempMatch.Team2Goals;
                            tgti1.GoalsDifference -= goalDifference;
                            tgti1.Points -= goalDifference > 0 ?
                                3 :
                                goalDifference == 0 ? 1 : 0;
                        }

                        addTeam1 = false; // запоминаем то, что данные о команде 1 были в таблице результатов
                    }

                    if (tgti2 == null) // если команда 2 еще не занесена в игровую таблицу
                    {
                        tgti2 = new TournamentGroupTableItem();
                        tgti2.Deleted = false;
                        tgti2.Published = true;
                        tgti2.TournamentGroupId = tournamentGroupId;
                        tgti2.BidTeamToTournamentId = bidTeam2ToTournamentId;
                        tgti2.Games = 0;
                        tgti2.Wins = 0;
                        tgti2.Loses = 0;
                        tgti2.Draws = 0;
                        tgti2.Place = 0;
                        tgti2.GoalsScored = 0;
                        tgti2.GoalsMissed = 0;
                        tgti2.GoalsDifference = 0;
                        tgti2.Points = 0;
                        tgti2.TeamName = Team2Name;

                        // учет текущей игры
                        int goalDifference = goalsTeam2 - goalsTeam1;

                        tgti2.Games += 1;
                        tgti2.Wins += goalDifference > 0 ? 1 : 0;
                        tgti2.Loses += goalDifference < 0 ? 1 : 0;
                        tgti2.Draws += goalDifference == 0 ? 1 : 0;
                        tgti2.GoalsScored += goalsTeam2;
                        tgti2.GoalsMissed += goalsTeam1;
                        tgti2.GoalsDifference += goalDifference;
                        tgti2.Points += goalDifference > 0 ?
                            3 :
                            goalDifference == 0 ? 1 : 0;

                        addTeam2 = true; // запоминаем то, что данных о команде 2 не было в таблице результатов
                    }
                    else // строка в таблице результатов уже существует
                    {
                        tgti2.Deleted = false;
                        tgti2.Published = true;
                        tgti2.TournamentGroupId = tournamentGroupId;
                        tgti2.BidTeamToTournamentId = bidTeam2ToTournamentId;
                        tgti2.Games = 0;
                        tgti2.Wins = 0;
                        tgti2.Loses = 0;
                        tgti2.Draws = 0;
                        tgti2.Place = 0;
                        tgti2.GoalsScored = 0;
                        tgti2.GoalsMissed = 0;
                        tgti2.GoalsDifference = 0;
                        tgti2.Points = 0;
                        tgti2.TeamName = Team2Name;


                        // учет текущей игры
                        int goalDifference = goalsTeam2 - goalsTeam1;

                        tgti2.Games += 1;
                        tgti2.Wins += goalDifference > 0 ? 1 : 0;
                        tgti2.Loses += goalDifference < 0 ? 1 : 0;
                        tgti2.Draws += goalDifference == 0 ? 1 : 0;
                        tgti2.GoalsScored += goalsTeam2;
                        tgti2.GoalsMissed += goalsTeam1;
                        tgti2.GoalsDifference += goalDifference;
                        tgti2.Points += goalDifference > 0 ?
                            3 :
                            goalDifference == 0 ? 1 : 0;

                        // получаем данные из базы матчей и считаем результаты
                        using (DefaultContext dc2 = new DefaultContext())
                        {
                            // нужно получить из базы (таблица матчей) данные об играх этой команды в этом турнире
                            var teamSeasonInfo2 = dc2.Matches
                                .AsNoTracking()
                                .Where(x => (x.TournamentGroupId == tournamentGroupId
                                    && (x.BidTeamToTournamentId1 == bidTeam2ToTournamentId || x.BidTeamToTournamentId2 == bidTeam2ToTournamentId)
                                    && (x.Played)
                                    ))
                                .Select(x => new
                                {
                                    TeamBidId = bidTeam2ToTournamentId,
                                    When = x.When,
                                    Scored = (x.BidTeamToTournamentId1 == bidTeam2ToTournamentId) ? x.Team1Goals : x.Team2Goals,
                                    Missed = (x.BidTeamToTournamentId1 == bidTeam2ToTournamentId) ? x.Team2Goals : x.Team1Goals,
                                    GoalsDifferenceInGame = (x.BidTeamToTournamentId1 == bidTeam2ToTournamentId) ? x.Team1Goals - x.Team2Goals : x.Team2Goals - x.Team1Goals
                                });


                            foreach (var tsi2 in teamSeasonInfo2)
                            {
                                tgti2.Games += 1;
                                tgti2.Wins += tsi2.GoalsDifferenceInGame > 0 ? 1 : 0;
                                tgti2.Loses += tsi2.GoalsDifferenceInGame < 0 ? 1 : 0;
                                tgti2.Draws += tsi2.GoalsDifferenceInGame == 0 ? 1 : 0;
                                tgti2.GoalsScored += tsi2.Scored;
                                tgti2.GoalsMissed += tsi2.Missed;
                                tgti2.GoalsDifference += tsi2.GoalsDifferenceInGame;
                                tgti2.Points += tsi2.GoalsDifferenceInGame > 0 ?
                                    3 :
                                    tsi2.GoalsDifferenceInGame == 0 ? 1 : 0;
                            }
                        }


                        // корректируем данные команды 2, учитывая прошлую запись об этом матче в базе данных
                        if (tempMatch != null)
                        {
                            goalDifference = tempMatch.Team2Goals - tempMatch.Team1Goals;
                            tgti2.Games -= 1;
                            tgti2.Wins -= goalDifference > 0 ? 1 : 0;
                            tgti2.Loses -= goalDifference < 0 ? 1 : 0;
                            tgti2.Draws -= goalDifference == 0 ? 1 : 0;
                            tgti2.GoalsScored -= tempMatch.Team2Goals;
                            tgti2.GoalsMissed -= tempMatch.Team1Goals;
                            tgti2.GoalsDifference -= goalDifference;
                            tgti2.Points -= goalDifference > 0 ?
                                3 :
                                goalDifference == 0 ? 1 : 0;
                        }

                        addTeam2 = false; // запоминаем то, что данные о команде 2 были в таблице результатов
                    }
                }

                // возврат данных для добавления или обновления 
                if (addTeam1) // для добавления
                    tableRows[0] = tgti1;
                else // для обновления
                    tableRows[0] = tgti1;
                    //tableRows[2] = tgti1;

                if (addTeam2) // если команда еще не записывалась в таблицу результатов
                    tableRows[1] = tgti2;
                else // если команда уже была в таблице результатов
                    tableRows[1] = tgti2;
                    //tableRows[3] = tgti2;


            }
            catch(Exception ex)
            {
                return null;
            }

            
            return tableRows;
            // ======================= добавление строки в турнирную таблицу =============================
        }

        /// <summary>
        /// Расчет данных в строках команд турнирных таблиц, когда происходит удаление матча из расписания
        /// </summary>
        /// <param name="bidTeam1ToTournamentId"></param>
        /// <param name="bidTeam2ToTournamentId"></param>
        /// <param name="goalsTeam1"></param>
        /// <param name="goalsTeam2"></param>
        /// <param name="tournamentGroupId"></param>
        /// <param name="whenMatch"></param>
        /// <returns></returns>
        public TournamentGroupTableItem[] CalculateRowInTournamentTableWhenDeleteMatch(Match match)
        {
            int bidTeam1ToTournamentId = match.BidTeamToTournamentId1, 
                bidTeam2ToTournamentId = match.BidTeamToTournamentId2, 
                goalsTeam1 = match.Team1Goals, 
                goalsTeam2 = match.Team2Goals, 
                tournamentGroupId = match.TournamentGroupId;
            DateTime whenMatch = match.When;

            // ======================= добавление строки в турнирную таблицу =============================
            TournamentGroupTableItem[] tableRows = new TournamentGroupTableItem[2];
            tableRows[0] = new TournamentGroupTableItem();  // измененная строка команды 1
            tableRows[1] = new TournamentGroupTableItem();  // измененная строка команды 2

            try
            {

                TournamentGroupTableItem tgti1 = null, tgti2 = null;

                // ищем строки в турнирной таблице
                using (DefaultContext dc1 = new DefaultContext())
                {
                    tgti1 = dc1.TournamentGroupTableItems.FirstOrDefault(row => ((row.BidTeamToTournamentId == bidTeam1ToTournamentId) && (row.TournamentGroupId == tournamentGroupId)));
                    tgti2 = dc1.TournamentGroupTableItems.FirstOrDefault(row => ((row.BidTeamToTournamentId == bidTeam2ToTournamentId) && (row.TournamentGroupId == tournamentGroupId)));
                }

                if (match.Played)
                {
                    if (tgti1 != null) // если команда 1 занесена в игровую таблицу
                    {
                        int goalDifference = match.Team1Goals - match.Team2Goals;
                        tgti1.Games -= 1;
                        tgti1.Wins -= goalDifference > 0 ? 1 : 0;
                        tgti1.Loses -= goalDifference < 0 ? 1 : 0;
                        tgti1.Draws -= goalDifference == 0 ? 1 : 0;
                        tgti1.GoalsScored -= match.Team1Goals;
                        tgti1.GoalsMissed -= match.Team2Goals;
                        tgti1.GoalsDifference -= goalDifference;
                        tgti1.Points -= goalDifference > 0 ?
                            3 :
                            goalDifference == 0 ? 1 : 0;
                    }

                    if (tgti2 != null) // если команда 2 занесена в игровую таблицу
                    {

                        // корректируем данные команды 2, учитывая прошлую запись об этом матче в базе данных
                        int goalDifference = match.Team2Goals - match.Team1Goals;
                        tgti2.Games -= 1;
                        tgti2.Wins -= goalDifference > 0 ? 1 : 0;
                        tgti2.Loses -= goalDifference < 0 ? 1 : 0;
                        tgti2.Draws -= goalDifference == 0 ? 1 : 0;
                        tgti2.GoalsScored -= match.Team2Goals;
                        tgti2.GoalsMissed -= match.Team1Goals;
                        tgti2.GoalsDifference -= goalDifference;
                        tgti2.Points -= goalDifference > 0 ?
                            3 :
                            goalDifference == 0 ? 1 : 0;

                    }
                }
                

                // возврат данных для добавления или обновления 
                tableRows[0] = tgti1;
                tableRows[1] = tgti2;


            }
            catch(Exception ex)
            {
                return null;
            }

            
            return tableRows;
            // ======================= добавление строки в турнирную таблицу =============================
        }


        /// <summary>
        /// расчет мест в таблице результатов
        /// </summary>
        /// <param name="tournamentGroupId">идентификатор группы/лиги, в которой нужен расчет</param>
        /// <returns></returns>
        public bool CalculatePlacesInTournamentTable(int tournamentGroupId)
        {
            List<TournamentGroupTableItem> rows = null;
            List<Match> matchesInTournament = null;


            using (DefaultContext dc6 = new DefaultContext())
            {
                try
                {
                    // вытаскиваем все строки из таблицы, сразу сортируем по убыванию очков
                    // и пока комментим по убыванию разницы мячей
                    rows = dc6.TournamentGroupTableItems
                    .Where(item => item.TournamentGroupId == tournamentGroupId)
                    .OrderByDescending(x => x.Points)
                    //.ThenByDescending(x => x.GoalsDifference)
                    .ToList();

                    // вытаскиваем все матчи лиги
                    matchesInTournament = dc6.Matches
                        .Where(match => match.TournamentGroupId == tournamentGroupId)
                        .ToList();
                }
                catch (Exception ex)
                {

                }
            }

            if ((rows != null) && (matchesInTournament != null))
                using (DefaultContext dc7 = new DefaultContext())
                {
                    try
                    {
                        //dc7.Database.BeginTransaction();

                    

                        // учет должен быть на выбор (сначала личные встречи, потом лучшая разница мячей или наоборот)
                        // сейчас закладываем жестко сначала личные встречи

                        //
                        for (int i = 1; i < rows.Count; i++)
                        {
                            if (rows[i-1].Points == rows[i].Points)
                            {
                                int teamBid1Id = rows[i-1].BidTeamToTournamentId.GetValueOrDefault(-1);
                                int teamBid2Id = rows[i].BidTeamToTournamentId.GetValueOrDefault(-1);
                                List<Match> matchesOfTheseTwoTeams = new List<Match>();
                                if ((teamBid1Id >= 0) && (teamBid2Id >= 0))
                                {
                                    matchesOfTheseTwoTeams = matchesInTournament
                                        .Where(match => 
                                        (
                                            (
                                                (match.BidTeamToTournamentId1 == teamBid1Id) && (match.BidTeamToTournamentId2 == teamBid2Id)
                                            ) 
                                            ||
                                            (
                                                (match.BidTeamToTournamentId1 == teamBid2Id) && (match.BidTeamToTournamentId2 == teamBid1Id))
                                            )
                                        )
                                        .ToList();
                                }

                                // нужно подвести итоги по сыгранным матчам между этими командами, кто выше.
                                // для этого считаем кто кому сколько забил и сколько очков заработал на ком.
                                // далее, у кого личная статистика выше, поднимается выше, реплейсим в массиве его выше и прописываем место.
                                var microresults = matchesOfTheseTwoTeams
                                    .Select(x => new
                                    {
                                        Points1 = (x.Team1Goals - x.Team2Goals > 0) ? 3 : (x.Team1Goals - x.Team2Goals == 0) ? 1 : -3,
                                        Points2 = (x.Team1Goals - x.Team2Goals < 0) ? 3 : (x.Team1Goals - x.Team2Goals == 0) ? 1 : -3,
                                        GoalDif = x.Team1Goals - x.Team2Goals
                                    })
                                    .ToList();

                                int t1Points = 0, t2Points = 0, Difference = 0;

                                foreach(var m in microresults)
                                {
                                    t1Points += m.Points1;
                                    t2Points += m.Points2;
                                    Difference += m.GoalDif;
                                }



                                if (t1Points < t2Points) // меняем местами, т.к. у 2 команды больше очков
                                {
                                    // меняем местами команды в таблице
                                    TournamentGroupTableItem temp = new TournamentGroupTableItem();
                                    temp = rows[i - 1];
                                    rows[i - 1] = rows[i];
                                    rows[i] = temp;
                                }
                                else if (t1Points == t2Points) // равно очков, проверяем по разнице мячей между командами
                                {
                                    if (Difference < 0) // меняем местами, т.к. 2 команда забила больше
                                    {
                                        // меняем местами команды в таблице
                                        TournamentGroupTableItem temp = new TournamentGroupTableItem();
                                        temp = rows[i - 1];
                                        rows[i - 1] = rows[i];
                                        rows[i] = temp;
                                    }
                                }

                            }

                            // прописываем места
                            rows[i - 1].Place = i;
                            rows[i].Place = i + 1; // это тоже прописываем, чтобы последний элемент был с местом

                            dc7.Entry(rows[i-1]).State = EntityState.Modified;
                            dc7.Entry(rows[i]).State = EntityState.Modified;
                        }

                        // вносим изменения в базу данных
                        dc7.ChangeTracker.DetectChanges();
                        var o = dc7.ChangeTracker.Entries();
                        dc7.SaveChanges();
                        //dc7.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        //dc7.Database.CurrentTransaction.Rollback();
                        return false;
                    }

                }

            return true;
        }

        [HttpOptions]
        public dynamic GetByTournament()
        {
            return Ok();
        }


        /// <summary>
        /// Возвращает все матчи турнира
        /// </summary>
        /// <param name="createTournamentModel">Турнир и профиль админа</param>
        /// <returns></returns>
        [HttpPost]
        public dynamic GetByTournament(CreateTournamentModel createTournamentModel)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == createTournamentModel.userProfile.UserProfileId);

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (createTournamentModel.tournament != null)
                //&& (new Common().UserProfileHasAccessToTournament(uProfile, createTournamentModel.tournament))
                )
            {
                try
                {
                    var r = dc.Matches
                        .Join(dc.SimplePlaces, a => a.UmbracoPlaceId, b => b.UmbracoId, (mat, spl) => new { mat, spl })
                        .Join(dc.TournamentGroups, a => a.mat.TournamentGroupId, b => b.Id, (matspl, trg) => new { matspl, trg })
                        .Join(dc.Teams, a => a.matspl.mat.Team1Id, b => b.Id, (matspltrg, team1) => new { matspltrg, team1 })
                        .Join(dc.Teams, a => a.matspltrg.matspl.mat.Team2Id, b => b.Id, (matspltrgteam1, team2) => new { matspltrgteam1, team2 })
                        .Join(dc.Tournaments, a => a.matspltrgteam1.matspltrg.trg.TournamentId, trn => trn.Id, (matspltrgteams, trn) => new { matspltrgteams, trn })
                        .Where(x => ((x.trn.Id == createTournamentModel.tournament.Id) && (!x.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Deleted)))
                        .Select(a => new
                        {
                            Id = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Id,
                            //TournamentGroup = a.bidtrgteam.bidtrg.trg,

                            TournamentGroup = new
                            {
                                Id = a.matspltrgteams.matspltrgteam1.matspltrg.trg.Id,
                                ErrorMessage = a.matspltrgteams.matspltrgteam1.matspltrg.trg.ErrorMessage,
                                Deleted = a.matspltrgteams.matspltrgteam1.matspltrg.trg.Deleted,
                                Name = a.matspltrgteams.matspltrgteam1.matspltrg.trg.Name,
                                Published = a.matspltrgteams.matspltrgteam1.matspltrg.trg.Published,
                                Tournament = new
                                {
                                    TournamentId = a.trn.Id,
                                    Name = a.trn.Name,
                                    OrganizatorName = a.trn.OrganizatorName,
                                    OrganizatorNameShort = a.trn.OrganizatorNameShort,
                                    Link = a.trn.Link,
                                    Link2 = a.trn.Link2,
                                    Link2Name = a.trn.Link2Name,
                                    Logo = a.trn.Logo,
                                    Founder = a.trn.Founder,
                                    Details = a.trn.Details
                                },
                                TournamentId = a.matspltrgteams.matspltrgteam1.matspltrg.trg.TournamentId,
                            },
                            //TournamentId = a.bidtrgteam.bidtrg.trg.TournamentId,
                            Team1 = a.matspltrgteams.matspltrgteam1.team1,
                            Team2 = a.matspltrgteams.team2,
                            Team1BidId = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.BidTeamToTournamentId1,
                            Team2BidId = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.BidTeamToTournamentId2,
                            Team1Goals = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Team1Goals,
                            Team2Goals = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Team2Goals,
                            When = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.When,
                            Played = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Played,
                            Description = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Description,
                            Deleted = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Deleted,
                            Published = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.Published,
                            TournamentGroupId = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.TournamentGroupId,
                            PlaceId = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.mat.UmbracoPlaceId, // обрати внимание откуда берется PlaceId!!
                            Place = a.matspltrgteams.matspltrgteam1.matspltrg.matspl.spl
                        })
                        ;

                    return r;
                }
                catch (Exception ex)
                {
                    //dc.Database.CurrentTransaction.Rollback();
                    return new Match("Error in GetByTournament: " + ex.Message);
                }
            }
            else
            {
                return new Match("Error in GetByTournament: not authorized");
            }


        }


        [HttpOptions]
        public dynamic Del()
        {
            return Ok();
        }


        [HttpPost]
        /// <summary>
        /// Добавляет в расписание матч (Внимание! Изменен источник PlaceId. В будущем доработать!)
        /// </summary>
        /// <param name="matchUserProfile">Данные матча, группа турнира и профиля создателя</param>
        /// <returns></returns>
        public dynamic Del(MatchUserProfile matchUserProfile)
        {
            UserProfile uProfile = dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == matchUserProfile.userProfile.UserProfileId);
            Tournament tourn = dc.TournamentGroups
                .Where(x => x.Id == matchUserProfile.match.TournamentGroupId)
                .Join(dc.Tournaments, a => a.TournamentId, b => b.Id, (tgr, t) => new { tgr, t })
                .Select(x => x.t)
                .FirstOrDefault();
            TournamentGroupTableItem[] resultTableItems = new TournamentGroupTableItem[2];


            int id = -1;

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (uProfile != null)
                && (tourn != null)
                && (new Common().UserProfileHasAccessToTournament(uProfile, tourn))
                )
            {
                try
                {
                    dc.Database.BeginTransaction();

                    Match mat = dc.Matches
                        .Where(x => (x.Team1Id == matchUserProfile.match.Team1Id)
                        && (x.Team2Id == matchUserProfile.match.Team2Id)
                        && (x.TournamentGroupId == matchUserProfile.match.TournamentGroupId))
                        .FirstOrDefault(); // ищем матч


                    if (mat != null)
                    {
                        mat.Deleted = true;

                        mat.TournamentGroup = null;
                        mat.Team1 = null;
                        mat.Team2 = null;
                        mat.Place = null;
                        mat.MatchEvents = null;

                        dc.Entry(mat).State = EntityState.Deleted;


                        // при удалении нужно пересчитать турнирную таблицу. не удалить, а именно перерасчитать и записать значения заново. 
                        // вообще в турнирную таблицу нужно вносить когда заявка от команды на турнир принята
                        resultTableItems = CalculateRowInTournamentTableWhenDeleteMatch(mat);

                        if (resultTableItems[0] != null)
                            dc.Entry(resultTableItems[0]).State = EntityState.Modified;

                        if (resultTableItems[1] != null)
                            dc.Entry(resultTableItems[1]).State = EntityState.Modified;

                        dc.SaveChanges();

                        dc.Database.CurrentTransaction.Commit();

                        id = mat.Id;
                    }

                    

                    return mat;
                }
                catch (Exception ex)
                {
                    dc.Database.CurrentTransaction.Rollback();
                    return new Match("Error in MatchDel: " + ex.Message);
                }
            }
            else
            {
                return new Match("Error in MatchDel: not authorized");
            }
        }




        /// <summary>
        /// Возвращает все матчи города
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public dynamic GetByCity()
        {
            int cityUmbracoId = (currentRequest.Form.Get("cityumbracoid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityumbracoid")[0]) : -1;

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (cityUmbracoId != -1)
                )
            {
                try
                {
                    City city = dc.Citys.Where(x => x.CityUmbracoId == cityUmbracoId).FirstOrDefault();

                    // вычисляем диапазон даты от вчера до послезавтра
                    DateTime from = DateTime.Today.AddDays(-1);
                    DateTime to = DateTime.Today.AddDays(2);
                    

                    var r = dc.Matches
                        .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (match, trg) => new { match, trg })
                        .Join(dc.Tournaments, a => a.trg.TournamentId, trn => trn.Id, (matchtrg, trn) => new { matchtrg, trn })
                        .Join(dc.BidTeamToTournaments, a => a.matchtrg.match.BidTeamToTournamentId1, b => b.Id, (matchtrgtrn, bid1) => new { matchtrgtrn, bid1 })
                        .Join(dc.BidTeamToTournaments, a => a.matchtrgtrn.matchtrg.match.BidTeamToTournamentId2, b => b.Id, (matchtrgtrnbid1, bid2) => new { matchtrgtrnbid1, bid2 })
                        .Join(dc.SimplePlaces, a => a.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.UmbracoPlaceId, b => b.UmbracoId, (matchtrgtrnbids, spl) => new { matchtrgtrnbids, spl })
                        .Where(x => ((x.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.CityId == city.CityId) 
                        && (!x.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Deleted)
                        && (x.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.When >= from)
                        && (x.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.When <= to)
                        )
                        )
                        .Select(a => new
                        {
                            Id = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Id,
                            TournamentGroup = new
                            {
                                Id = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.trg.Id,
                                ErrorMessage = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.trg.ErrorMessage,
                                Deleted = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.trg.Deleted,
                                Name = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.trg.Name,
                                Published = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.trg.Published,
                                Tournament = new
                                {
                                    TournamentId = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Id,
                                    Name = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Name,
                                    OrganizatorName = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.OrganizatorName,
                                    OrganizatorNameShort = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.OrganizatorNameShort,
                                    Link = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Link,
                                    Link2 = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Link2,
                                    Link2Name = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Link2Name,
                                    Logo = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Logo,
                                    Founder = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Founder,
                                    Details = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.trn.Details
                                },
                                TournamentId = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.trg.TournamentId,
                            },
                            Team1Name = a.matchtrgtrnbids.matchtrgtrnbid1.bid1.TeamName,
                            Team2Name = a.matchtrgtrnbids.bid2.TeamName,
                            Team1BidId = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.BidTeamToTournamentId1,
                            Team2BidId = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.BidTeamToTournamentId2,
                            Team1Goals = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Team1Goals,
                            Team2Goals = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Team2Goals,
                            When = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.When,
                            Description = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Description,
                            Played = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Played,
                            Deleted = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Deleted,
                            Published = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.Published,
                            TournamentGroupId = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.TournamentGroupId,
                            PlaceId = a.matchtrgtrnbids.matchtrgtrnbid1.matchtrgtrn.matchtrg.match.UmbracoPlaceId, // обрати внимание откуда берется PlaceId!!
                            Place = a.spl, // обрати внимание откуда берется PlaceId!!
                        })
                        ;

                    return r.OrderBy(x => x.When).ToList();
                }
                catch (Exception ex)
                {
                    //dc.Database.CurrentTransaction.Rollback();
                    return new Match("Error in GetByCity: " + ex.Message);
                }
            }
            else
            {
                return new Match("Error in GetByCity: not authorized");
            }


        }




        // ======== for parser service =======

        [HttpOptions]
        public dynamic UpdateMatches()
        {

            return Ok();
        }

        [HttpPost]
        public dynamic UpdateMatches(List<MatchFromParser> matchesFromParser)
        {
            int resultStrings = 0;
            if (matchesFromParser != null)
            {
                foreach (var mfp in matchesFromParser)
                {
                    if (mfp.Team1AssociationId == -1
                        || mfp.Team2AssociationId == -1
                        || mfp.PlaceAssociactionId == -1
                        )
                        break;
                    Match mt = new Match();
                    mt.BidTeamToTournamentId1 = mfp.Team1AssociationId;
                    mt.BidTeamToTournamentId2 = mfp.Team2AssociationId;
                    mt.Deleted = false;
                    mt.Description = "";
                    mt.ErrorMessage = "";
                    mt.Published = true;

                    mt.Played = mfp.Played;
                    Team t1 = new SimpleTeamController().GetTeamByBidTeamToTournamentId(mt.BidTeamToTournamentId1);
                    Team t2 = new SimpleTeamController().GetTeamByBidTeamToTournamentId(mt.BidTeamToTournamentId2);
                    mt.Team1Id = t1 != null ? t1.Id : -1;
                    mt.Team2Id = t2 != null ? t2.Id : -1;
                    if (t1 == null || t2 == null)
                    {
                        throw new Exception();
                    }
                    mt.Team1Goals = mfp.Team1Goals;
                    mt.Team2Goals = mfp.Team2Goals;
                    if (mt.Team1Goals < 0 || mt.Team2Goals < 0)
                    {
                        mt.Played = false;
                        mt.Team1Goals = 0;
                        mt.Team2Goals = 0;
                    }

                    mt.TournamentGroupId = mfp.TournamentGroupCloudId;
                    mt.When = mfp.When;
                    mt.Name = "from parser"; // = mfp.Name;
                    mt.Description = mfp.Description;
                    mt.Picture = mfp.Picture;

                    SimplePlace sp = new SimplePlaceController().GetPlaceById(mfp.PlaceAssociactionId.Value);
                    mt.PlaceId = mfp.PlaceAssociactionId;
                    mt.UmbracoPlaceId = sp != null ? sp.UmbracoId : 0;
                    if (sp == null)
                    {
                        throw new Exception();
                    }


                    mt.MatchEvents = null;
                    mt.BidTeamToTournament1 = null;
                    mt.BidTeamToTournament2 = null;
                    mt.Place = null;
                    mt.Team1 = null;
                    mt.Team2 = null;
                    mt.TournamentGroup = null;

                    var res = AddMatch(mt);
                    if (res.ErrorMessage == "")
                    {
                        resultStrings++;
                    }

                }    

            }

            return resultStrings;
        }

    }


}
