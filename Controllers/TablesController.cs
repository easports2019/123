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
    public class TablesController : UmbracoApiController
    {

        HttpRequest currentRequest = HttpContext.Current.Request;

        [HttpPost]
        /// <summary>
        /// Возвращает таблицы по выбранному турниру
        /// </summary>
        /// <returns></returns>
        public dynamic GetByTournamentId()
        {
            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int tournamentId = (currentRequest.Form.Get("tournamentId") != null) ? Common.StrToInt(currentRequest.Form.GetValues("tournamentId")[0]) : 0;

                if (tournamentId != -1)
                {
                    using (DefaultContext dc = new DefaultContext())
                    {

                        // выбираем все строки из таблицы по Id группы турнира
                        // на выходе нужно получить массив из групп турнира со строками из таблицы результатов по командам. 
                        // важно! должно быть максимально быстро, т.к. здесь узкое место
                        var tables =
                            dc.TournamentGroupTableItems
                                .Join(dc.TournamentGroups, a => a.TournamentGroupId, b => b.Id, (tgti, tgr) => new { tgti, tgr })
                                .Where(item => item.tgr.TournamentId == tournamentId)
                                .Select(s => new {
                                    BidTeamToTournamentId = s.tgti.BidTeamToTournamentId,
                                    Draws = s.tgti.Draws,
                                    Games = s.tgti.Games,
                                    GoalsDifference = s.tgti.GoalsDifference,
                                    GoalsMissed = s.tgti.GoalsMissed,
                                    GoalsScored = s.tgti.GoalsScored,
                                    Id = s.tgti.Id,
                                    Loses = s.tgti.Loses,
                                    Points = s.tgti.Points,
                                    Place = s.tgti.Place,
                                    Published = s.tgti.Published,
                                    TournamentGroupId = s.tgti.TournamentGroupId,
                                    Wins = s.tgti.Wins,
                                    TeamName = s.tgti.TeamName

                                })
                                .ToList()
                                ;

                        //: null;
                        return tables;
                    }
                }
            }

            return null;
        }
    }
}