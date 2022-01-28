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
    public class CityController : UmbracoApiController
    {
        DefaultContext dc = new DefaultContext();
        HttpRequest currentRequest = HttpContext.Current.Request;


        /// <summary>
        /// ДОбавляет в базу новый город и его же возвращает
        /// </summary>
        /// <param name="city">Vk структура города</param>
        /// <returns></returns>
        public City Add(VKCity city)
        {
            City newCity = new City();
            newCity.CityId = city.id;
            newCity.Name = city.title;
            if (dc.Citys.Any())
            {
                if (dc.Citys.FirstOrDefault(x => x.CityVkId == city.id) == null)
                {
                    dc.Citys.Add(newCity);
                    dc.SaveChanges();
                }
                else
                {
                    return dc.Citys.FirstOrDefault(x => x.CityVkId == city.id);
                }
            }
            else
            {
                dc.Citys.Add(newCity);
                dc.SaveChanges();
            }
            return newCity;
        }
        

        /// <summary>
        /// ДОбавляет в базу новый город и его же возвращает
        /// </summary>
        /// <param name="cityVkId">Vk Id города</param>
        /// <param name="cityName">Название города</param>
        /// <returns></returns>
        public City Add(int? cityVkId, string cityName)
        {
            City newCity = new City();
            newCity.CityVkId = cityVkId;
            newCity.Name = cityName;
            if (dc.Citys.Any())
            {
                if (dc.Citys.FirstOrDefault(x => x.CityVkId == cityVkId) == null)
                {
                    dc.Citys.Add(newCity);
                    dc.SaveChanges();
                }
                else
                {
                    return dc.Citys.FirstOrDefault(x => x.CityVkId == cityVkId);
                }
            }
            else
            {
                dc.Citys.Add(newCity);
                dc.SaveChanges();
            }

            return newCity;
        }


        
        /// <summary>
        /// Возвращает все доступные города из базы умбрако
        /// </summary>
        /// <returns>List<City></returns>
        [HttpPost]
        public List<City> GetAll()
        {
            List<City> cities = new List<City>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                var cityNodes = umbraco.uQuery.GetNodesByType("city");
                IPublishedContent cityPage = null;


                if (cityNodes != null)
                {
                    foreach (var cn in cityNodes)
                    {
                        cityPage = Umbraco.TypedContent(cn.Id);
                        var city = PublishedContentToCity(cityPage);
                        if (city != null)
                            cities.Add(city);
                    }

                }
            }

            return cities;
        }
        
        /// <summary>
        /// Возвращает все доступные города из базы умбрако из категории Areas
        /// </summary>
        /// <returns>List<City></returns>
        [HttpPost]
        public List<City> GetAllFromAreas()
        {
            List<City> cities = new List<City>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                var catNodes = umbraco.uQuery.GetNodesByType("cathegory"); // верхний уровень узлов типа категория
                var areasNode = catNodes.Where(x => x.Name == "Areas").FirstOrDefault(); // узел Areas узлов типа Категория
                var countryNodes = areasNode.ChildrenAsList; // подузел Страны узла Areas


                IPublishedContent cityPage = null;

                if (countryNodes != null)
                {
                    foreach (var couN in countryNodes) // пробегаем по всем странам
                    {
                        foreach (var cityNode in couN.ChildrenAsList) // пробегаем по всем городам страны
                        {
                            if (cityNode.Name != "Not set") // если название не содержит Not set (это системное название, его устанавливаем по умолчанию когда создается что-то.. не помню)
                            {
                                cityPage = Umbraco.TypedContent(cityNode.Id);

                                var city = PublishedContentToCity(cityPage);
                                if (city != null)
                                    cities.Add(city);
                            }
                        }
                        
                    }

                }
            }

            return cities;
        }


        /// <summary>
        /// Возвращает имя города по его Id умбрако
        /// </summary>
        /// <param name="cityUmbracoId">UmbracoId города</param>
        /// <returns>Название города</returns>
        public string GetCityUmbracoNameByCityUmbracoId(int cityUmbracoId)
        {
            //var cityNodes = umbraco.uQuery.GetNodesByType("city");
            IPublishedContent cityPage = null;
            cityPage = Umbraco.TypedContent(cityUmbracoId);

            if (cityPage != null)
            {
                var city = PublishedContentToCity(cityPage);
                return city.CityUmbracoName;
            }

            return "";
        }


        /// <summary>
        /// Возвращает город по его Id вконтакте из базы Umbraco
        /// </summary>
        /// <param name="cityVkId"></param>
        /// <returns></returns>
        public City GetCityFromBaseByCityVkId(int cityVkId)
        {
            City city = null;
            IPublishedContent cityPage = null;

            try
            {
                var catNodes = umbraco.uQuery.GetNodesByType("cathegory"); // верхний уровень узлов типа категория
                var areasNode = catNodes.Where(x => x.Name == "Areas").FirstOrDefault(); // узел Areas узлов типа Категория
                var countryNodes = areasNode.ChildrenAsList; // подузел Страны узла Areas

                if (countryNodes != null)
                {
                    foreach (var couN in countryNodes) // пробегаем по всем странам
                    {
                        var cn = couN.ChildrenAsList.FirstOrDefault(x => x.GetProperty("CityVkId").Value.ToString() == cityVkId.ToString()) ?? null;
                        if (cn != null)
                        {
                            cityPage = Umbraco.TypedContent(cn.Id);
                            city = PublishedContentToCity(cityPage);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return city;
        }  
        
        
        /// <summary>
        /// Возвращает город по его Id вконтакте
        /// </summary>
        /// <param name="cityVkId"></param>
        /// <returns></returns>
        public City GetCityFromBaseByName(string cityName)
        {
            if (dc.Citys.Any())
                return dc.Citys.FirstOrDefault(x => x.Name == cityName);
            else
                return null;
        }

        /// <summary>
        /// Преобразует из Umbraco-объекта Сity в модель Сity
        /// </summary>
        /// <param name="cityPage">Объект IPublishedContent с содержимым в виде амплуа</param>
        /// <returns>Заполненный объект city</returns>
        public City PublishedContentToCity(IPublishedContent cityPage)
        {
            City city = new City();

            if (cityPage != null)
            {
                city.CityUmbracoId = cityPage.Id;
                city.IsDefault = cityPage.GetProperty("def") != null ? Common.StrToBool(cityPage.GetProperty("def").Value.ToString()) : false;  //bool
                city.CityUmbracoName = cityPage.GetProperty("cityname") != null ? cityPage.GetProperty("cityname").Value.ToString() : "Не заполнено"; // 
            }

            return city;
        }
    }
}
