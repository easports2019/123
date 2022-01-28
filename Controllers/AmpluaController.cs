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
    public class AmpluaController : UmbracoApiController
    {

        /// <summary>
        /// Преобразует из Umbraco-объекта Amplua в модель Amplua
        /// </summary>
        /// <param name="ampluaPage">Объект IPublishedContent с содержимым в виде амплуа</param>
        /// <returns>Заполненный объект Amplua</returns>
        public Amplua PublishedContentToAmplua(IPublishedContent ampluaPage)
        {
            Amplua amplua = new Amplua();

            if (ampluaPage != null)
            {
                amplua.AmpluaUmbracoId = ampluaPage.Id;
                amplua.IsDefault = ampluaPage.GetProperty("def") != null ? Common.StrToBool(ampluaPage.GetProperty("def").Value.ToString()) : false;  //bool
                amplua.Name = ampluaPage.GetProperty("ampluaname") != null ? ampluaPage.GetProperty("ampluaname").Value.ToString() : "Не заполнено"; // 

            }

            return amplua;
        }

    }
}
