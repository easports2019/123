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
    public class LegController : UmbracoApiController
    {

        /// <summary>
        /// Преобразует из Umbraco-объекта Сity в модель Сity
        /// </summary>
        /// <param name="legPage">Объект IPublishedContent с содержимым в виде амплуа</param>
        /// <returns>Заполненный объект Leg</returns>
        public Leg PublishedContentToLeg(IPublishedContent legPage)
        {
            Leg leg = new Leg();

            if (legPage != null)
            {
                leg.LegUmbracoId = legPage.Id;
                leg.IsDefault = legPage.GetProperty("def") != null ? Common.StrToBool(legPage.GetProperty("def").Value.ToString()) : false;  //bool
                leg.Name = legPage.GetProperty("legname") != null ? legPage.GetProperty("legname").Value.ToString() : "Не заполнено"; // 

            }

            return leg;
        }
    }
}
