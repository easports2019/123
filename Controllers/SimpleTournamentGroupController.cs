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
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using System.Web;
using System.Web.Http.Cors;

namespace osbackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleTournamentGroupController : UmbracoApiController
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
        /// Добавляет в базу группу турнира
        /// </summary>
        /// <param name="userProfileTournamentTournamentGroupModel">Данные группы турнира</param>
        /// <returns></returns>
        public TournamentGroup Add(UserProfileTournamentTournamentGroupModel userProfileTournamentTournamentGroupModel)
        {
            if (userProfileTournamentTournamentGroupModel == null)
                return new TournamentGroup("Error in TournamentGroupAdd: no input data");

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTournament(userProfileTournamentTournamentGroupModel.userProfile, userProfileTournamentTournamentGroupModel.tournament)))

            {
                try
                {
                    TournamentGroup tOurnamentGroup = new TournamentGroup();
                    if (userProfileTournamentTournamentGroupModel.tournamentGroup.Id >= 0)
                        tOurnamentGroup.Id = userProfileTournamentTournamentGroupModel.tournamentGroup.Id;
                    tOurnamentGroup.Name = userProfileTournamentTournamentGroupModel.tournamentGroup.Name;
                    tOurnamentGroup.Published = true;
                    tOurnamentGroup.TournamentId = userProfileTournamentTournamentGroupModel.tournament.Id;
                    //tOurnamentGroup.Tournament = userProfileTournamentTournamentGroupModel.tournament;
                    tOurnamentGroup.Deleted = false;
                    
                    dc.TournamentGroups.Add(tOurnamentGroup);
                    dc.SaveChanges();
    
                    return tOurnamentGroup;
                }
                catch (Exception ex)
                {
                    return new TournamentGroup("Error in TournamentGroupAdd: " + ex.Message);
                }
            }
            else
            {
                return new TournamentGroup("Error in TournamentGroupAdd: not authorized");
            }

        }

        [HttpOptions]
        public dynamic Delete()
        {
            return Ok();
        }

        [HttpPost]
        /// <summary>
        /// Удаление группы турнира
        /// </summary>
        /// <param name="userProfileTournamentTournamentGroupModel">данные группы турнира с турниром и пользователем</param>
        /// <returns></returns>
        public TournamentGroup Delete(UserProfileTournamentTournamentGroupModel userProfileTournamentTournamentGroupModel)
        {
            TournamentGroup t = new TournamentGroup("Error in TournamentGroupDelete: No TournamentGroup with that Id");

            if (userProfileTournamentTournamentGroupModel.tournamentGroup.Id < 0)
                return new TournamentGroup("Error in TournamentGroupDelete: no Id to delete");

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToTournament(userProfileTournamentTournamentGroupModel.userProfile, userProfileTournamentTournamentGroupModel.tournament)))
            {
                try
                {
                    t = dc.TournamentGroups.FirstOrDefault(x => x.Id == userProfileTournamentTournamentGroupModel.tournamentGroup.Id);

                    if (t != null)
                    {
                        //dc.Entry<TournamentGroup>(t).State = System.Data.Entity.EntityState.Deleted;
                        dc.TournamentGroups.Remove(t);
                        dc.SaveChanges();
                    }
                    else
                    {
                        return new TournamentGroup("Error in TournamentGroupDelete: No TournamentGroup with that Id");
                    }
                }
                catch (Exception ex)
                {
                    return new TournamentGroup("Error in TournamentGroupDelete: " + ex.Message);
                }

                return t;
            }
            else
            {
                return new TournamentGroup("Error in TournamentGroupDelete: not authorized");
            }

        }


        /// <summary>
        /// Возвращает все опубликованные группы турнира
        /// </summary>
        /// <param name="tournamentId">Id турнира</param>
        /// <returns></returns>
        public List<TournamentGroup> GetAllFromTournament(int tournamentId)
        {
            if (tournamentId < 0)
                return null;

            List<TournamentGroup> tournamentGroups = new List<TournamentGroup>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    tournamentGroups = (dc.TournamentGroups
                        .Where(x => x.TournamentId == tournamentId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.TournamentGroups
                        .Where(x => x.TournamentId == tournamentId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return tournamentGroups;
        }

        /// <summary>
        /// Возвращает группу турнира по его Id
        /// </summary>
        /// <param name="tournamentGroupId">Id группы турнира</param>
        /// <returns></returns>
        public TournamentGroup GetById(int tournamentGroupId)
        {
            TournamentGroup tournamentGroup = new TournamentGroup();

            if (tournamentGroupId < 0)
                return null;

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    tournamentGroup = (dc.TournamentGroups.FirstOrDefault(x => x.Id == tournamentGroupId) != null)
                        ?
                        dc.TournamentGroups.FirstOrDefault(x => x.Id == tournamentGroupId)
                        :
                        new TournamentGroup("Error in TournamentGroupGetById: No tourney groups with that Id");
                }
            }

            return tournamentGroup;
        }
    }
}
