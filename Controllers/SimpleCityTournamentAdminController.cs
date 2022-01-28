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
    public class SimpleCityTournamentAdminController : UmbracoApiController
    {
        DefaultContext dc = new DefaultContext();
        HttpRequest currentRequest = HttpContext.Current.Request;


        [HttpPost]
        /// <summary>
        /// Возвращает всех админов
        /// </summary>
        /// <returns></returns>
        public List<CityTournamentAdmin> GetAll()
        {
            List<CityTournamentAdmin> cityTournamentAdmins = new List<CityTournamentAdmin>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    cityTournamentAdmins = (dc.CityTournamentAdmins
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.CityTournamentAdmins
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return cityTournamentAdmins;
        }


        [HttpPost]
        /// <summary>
        /// Возвращает админов в выбранном городе
        /// </summary>
        /// <returns></returns>
        public List<CityTournamentAdmin> GetAllInCity()
        {
            List<CityTournamentAdmin> cityTournamentAdmins = new List<CityTournamentAdmin>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;
                int cityUmbracoId = (currentRequest.Form.Get("cityumbracoid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityumbracoid")[0]) : -1;

                if (cityUmbracoId != -1)
                {
                    using (DefaultContext dc = new DefaultContext())
                    {
                        City city = dc.Citys.Where(x => x.CityUmbracoId == cityUmbracoId).FirstOrDefault();

                        cityTournamentAdmins = (dc.CityTournamentAdmins
                            .Where(x => x.CityId == city.CityId)
                            .Where(x => x.Published)
                            .Where(x => !x.Deleted)
                            .Count() > startIndex)
                            ?
                            dc.CityTournamentAdmins
                            .Where(x => x.CityId == city.CityId)
                            .Where(x => x.Published)
                            .Where(x => !x.Deleted)
                            .OrderBy(x => x.CityId)
                            .Skip(startIndex)
                            .Take(Common.RowsToReturnByTransaction)
                            //.Join(dc.Citys, adm => adm.CityId, c => c.CityId, (adm, c) => new CityTournamentAdmin(new CityTournamentAdmin() {
                            //    Id = adm.Id,
                            //    City = c,
                            //    CityId = adm.CityId,
                            //    Name = adm.Name,
                            //    UserProfile = adm.UserProfile,
                            //    UserProfileId = adm.UserProfileId,
                            //    Published = adm.Published,
                            //    Deleted = adm.Deleted,
                            //    ErrorMessage = adm.ErrorMessage,
                            //}))
                            .ToList()
                            : null;
                    }
                }
            }

            return cityTournamentAdmins;
        }


        /// <summary>
        /// Возвращает админа по его Id
        /// </summary>
        /// <param name="cityTournamentAdminId">Id заявки</param>
        /// <returns></returns>
        public CityTournamentAdmin GetById(int cityTournamentAdminId)
        {
            CityTournamentAdmin cityTournamentAdmin = new CityTournamentAdmin();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    cityTournamentAdmin = (dc.CityTournamentAdmins.FirstOrDefault(x => x.Id == cityTournamentAdminId) != null)
                        ?
                        dc.CityTournamentAdmins.FirstOrDefault(x => x.Id == cityTournamentAdminId)
                        :
                        new CityTournamentAdmin("Error in CityTournamentAdminGetById: No CityTournamentAdmins with that Id");
                }
            }

            return cityTournamentAdmin;
        }
    }
}
