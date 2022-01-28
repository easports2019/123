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
using Newtonsoft.Json;

namespace osbackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "*")]
    public class PlacesController : UmbracoApiController
    {
        HttpRequest currentRequest = HttpContext.Current.Request;

        [HttpPost]
        ///[Route("/umbraco/api/Places/Save")]
        public dynamic Save([FromBody] object model)
        {
            var state = JsonConvert.DeserializeObject<CreateUserViewModel>(model.ToString());
            return model;
        }
        
        //[HttpOptions]
        //public dynamic Save()
        //{
        //    return Ok();
        //}

        //[HttpPost]
        //public CreateUserViewModel Save1()
        //{
        //    return null;
        //}

        [HttpPost]
        public CreateUserViewModel Save2(CreateUserViewModel model)
        {
            return model;
        }

        [HttpOptions]
        public dynamic Save2()
        {
            return Ok();
        }

        //[HttpPost]
        //public CreateUserViewModel Save3([FromBody] CreateUserViewModel model)
        //{
        //    return model;
        //}


        /// <summary>
        /// Возвращает Place по его placeId. Для использования от HTTP-клиента.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Place GetPlaceById()
        {
            //Umbraco.Web.Mvc.UmbracoViewPage

            IPublishedContent placesPage = null;
            Place place = new Place();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {

                int placeId = Common.StrToInt((currentRequest.Form.Get("placeid") != null) ? currentRequest.Form.GetValues("placeid")[0] : "-1");

                if (placeId != -1)
                {
                    var node = umbraco.uQuery.GetNodesByType("place").FirstOrDefault(x => x.Id == placeId);
                    //Common.StrToInt(x.GetProperty("").Value)
                    
                    if (node != null)
                    {
                        placesPage = Umbraco.TypedContent(node.Id);
                        place = PublishedContentToPlace(placesPage);
                    }


                    
                }
            }

            

            return place;
        }
        
        /// <summary>
        /// Возвращает все Place города по cityUmbracoId. Для использования от HTTP-клиента.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<Place> getAllInCityByCityUmbracoId()
        {
            try
            {
                List<Place> places = new List<Place>();

                if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                {
                    int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;
                    int cityUmbracoId = (currentRequest.Form.Get("cityumbracoid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityumbracoid")[0]) : -1;

                    places = getAllInCityFromUmbracoDatabaseByCityUmbracoId(cityUmbracoId, startIndex);
                }

                return places;
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }


        /// <summary>
        /// Возвращает все Place. Для использования от HTTP-клиента. Принимает параметр startindex через POST (начальный индекс площадки для пагинации)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<Place> GetPlaces()
        {
            IPublishedContent placePage = null;
            List<Place> places = new List<Place>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                // читаем начальный индекс
                int placeStartIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                if (placeStartIndex >= 0)
                {
                    var placeNode = umbraco.uQuery.GetNodesByType("place");
                    var pls = (placeNode.Count() > placeStartIndex) ? placeNode.Skip(placeStartIndex).Take(Common.RowsToReturnByTransaction) : null;

                    if (places != null)
                    {
                        foreach(var pl in pls)
                        {
                            placePage = Umbraco.TypedContent(pl.Id);
                            var place = PublishedContentToPlace(placePage);
                            if (place != null)
                                places.Add(place);
                        }
                        
                    }



                }
            }

            return places;
        }


        


        /// <summary>
        /// Преобразует из Umbraco-объекта Place в модель Place
        /// </summary>
        /// <param name="placesPage">Объект IPublishedContent с содержимым в виде площадки</param>
        /// <returns>Заполненный объект Place</returns>
        public Place PublishedContentToPlace(IPublishedContent placesPage)
        {
            Place place = new Place();

            if (placesPage != null)
            {
                // далее нужно вручную вытащить каждое поле из объекта Place от Umbraco и раскидать по полям объекта Place
                //place.Access = placesPage.GetProperty("access") != null ? new AccessType(0, placesPage.GetProperty("access").Value.ToString()) : null; //string list


                //XmlPublishedCache
                //XmlPublishedContent)((Umbraco.Web.PublishedCache.XmlPublishedCache.XmlPublishedProperty)placesPage.GetProperty("owneritem")).Value).Name
                UmbracoHelper helper = new UmbracoHelper(UmbracoContext.Current, placesPage);

                place.PlaceId = placesPage.Id;
                place.Enabled = placesPage.GetProperty("enabled") != null ? Boolean.Parse(placesPage.GetProperty("enabled").Value.ToString()) : false;  //bool
                place.Name = placesPage.GetProperty("placename") != null ? placesPage.GetProperty("placename").Value.ToString() : ""; // 
                place.Info = placesPage.GetProperty("placeinfo") != null ? placesPage.GetProperty("placeinfo").Value.ToString() : ""; // 
                place.Geo = placesPage.GetProperty("geo") != null ? placesPage.GetProperty("geo").Value.ToString() : ""; // geo
                place.Stages = placesPage.GetProperty("stages") != null ? Int32.Parse(placesPage.GetProperty("stages").Value.ToString()) : -1;
                place.Parking = placesPage.GetProperty("parking") != null ? Boolean.Parse(placesPage.GetProperty("parking").Value.ToString()) : false;
                place.BicycleParking = placesPage.GetProperty("bicycleParking") != null ? Boolean.Parse(placesPage.GetProperty("bicycleParking").Value.ToString()) : false;

                place.Address = new Address();
                if ((placesPage.GetProperty("address") != null) && (placesPage.GetProperty("address").HasValue))
                {
                    place.Address.SubjectType = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().HasValue("subjectType") ? ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().GetPropertyValue("subjectType").ToString() : "нет данных";
                    place.Address.Street = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().HasValue("street") ? ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().GetPropertyValue("street").ToString() : "нет данных";
                    place.Address.House = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().HasValue("house") ? ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().GetPropertyValue("house").ToString() : "нет данных";
                    place.Address.Index = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().HasValue("index") ? ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().GetPropertyValue("index").ToString() : "нет данных";
                    place.Address.AddressId = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("address").Value).FirstOrDefault().Id;
                }
                else
                {
                    place.Address.SubjectType = "нет данных";
                    place.Address.Street = "нет данных";
                    place.Address.House = "нет данных";
                    place.Address.Index = "нет данных";
                    place.Address.AddressId = -1;
                }

                place.City = new City();
                if (((IPublishedContent)placesPage).Parent != null)
                {
                    IPublishedContent city = null;

                    if (((IPublishedContent)placesPage).Parent.DocumentTypeAlias == "city")
                        city = ((IPublishedContent)placesPage).Parent;
                    else if (((IPublishedContent)placesPage).Parent.Parent.DocumentTypeAlias == "city")
                        city = ((IPublishedContent)placesPage).Parent.Parent;
                    else if (((IPublishedContent)placesPage).Parent.Parent.Parent.DocumentTypeAlias == "city")
                        city = ((IPublishedContent)placesPage).Parent.Parent.Parent;
                    else
                        city = null;

                    if (city != null)
                    {
                        place.City.CityId = ((IPublishedContent)city).Id;
                        place.City.GeoPosition = city.HasValue("citygeoposition") ? city.GetPropertyValue("citygeoposition").ToString() : "нет данных";
                        place.City.Name = city.HasValue("cityname") ? city.GetPropertyValue("cityname").ToString() : "нет данных";
                        place.City.Population = city.HasValue("citypopulation") ? Common.StrToInt(city.GetPropertyValue("citypopulation").ToString()) : -1;
                    }
                    else
                    {
                        place.City.CityId = -1;
                        place.City.GeoPosition = "нет данных";
                        place.City.Name = "нет данных";
                        place.City.Population = -1;
                    }
                }

                place.Areas = new List<Area>();
                if ((placesPage.GetProperty("areas") != null) && (placesPage.GetProperty("areas").HasValue))
                {
                    // string areaName, length, width, height, capacitySport, capacityViewers, price
                    var areas = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("areas").Value).ToList();
                    foreach (var area in areas)
                    {
                        Area newArea = new Area();
                        newArea.AreaId = area.Id;
                        newArea.AreaName = area.HasValue("areaName") ? area.GetPropertyValue("areaName").ToString() : "нет данных";
                        newArea.CapacitySport = area.HasValue("capacitySport") ? Common.StrToInt(area.GetPropertyValue("capacitySport").ToString()) : -1;
                        newArea.CapacityViewers = area.HasValue("capacityViewers") ? Common.StrToInt(area.GetPropertyValue("capacityViewers").ToString()) : -1;
                        newArea.Height = area.HasValue("height") ? Common.StrToInt(area.GetPropertyValue("height").ToString()) : -1;
                        newArea.Length = area.HasValue("length") ? Common.StrToInt(area.GetPropertyValue("length").ToString()) : -1;
                        newArea.Price = area.HasValue("price") ? Common.StrToInt(area.GetPropertyValue("price").ToString()) : -1;
                        newArea.Width = area.HasValue("width") ? Common.StrToInt(area.GetPropertyValue("width").ToString()) : -1;
                        place.Areas.Add(newArea);
                    }
                }
                else
                {
                    /// no areas
                }
                place.DressingRooms = new List<DressingRoom>();
                if ((placesPage.GetProperty("dressingRooms") != null) && (placesPage.GetProperty("dressingRooms").HasValue))
                {
                    // string roomNumber, shower (bool), hotWater (bool),
                    var dRooms = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("dressingRooms").Value).ToList();
                    foreach (var dRoom in dRooms)
                    {
                        DressingRoom newDressingRoom = new DressingRoom();
                        newDressingRoom.DressingRoomId = dRoom.Id;
                        newDressingRoom.RoomNumber = dRoom.HasValue("roomNumber") ? dRoom.GetPropertyValue("roomNumber").ToString() : "нет данных";
                        newDressingRoom.HotWater = dRoom.HasValue("hotWater") ? Common.StrToBool(dRoom.GetPropertyValue("hotWater").ToString()) : false;
                        newDressingRoom.Shower = dRoom.HasValue("shower") ? Common.StrToInt(dRoom.GetPropertyValue("shower").ToString()) : -1;
                        place.DressingRooms.Add(newDressingRoom);
                    }
                }

                place.Owner = new Owner();
                place.Owner.Name = ((placesPage.GetProperty("owneritem") != null) && (placesPage.GetProperty("owneritem").Value != null)) ? ((IPublishedContent)placesPage.GetProperty("owneritem").Value).GetPropertyValue("ownername").ToString() : "нет данных";
                place.Owner.OwnerId = ((placesPage.GetProperty("owneritem") != null) && (placesPage.GetProperty("owneritem").Value != null)) ? ((IPublishedContent)placesPage.GetProperty("owneritem").Value).Id : -1;


                place.MainPicture = (placesPage.GetProperty("mainpicture") != null) ? ((Umbraco.Web.Models.PublishedContentBase)placesPage.GetProperty("mainpicture").Value).Url : Common.PlaceNoPhoto;

                place.Photos = new List<string>();
                if ((placesPage.GetProperty("photo") != null) && (placesPage.GetProperty("photo").HasValue))
                {

                    var photos = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("photo").Value).ToList();
                    foreach (var photo in photos)
                    {
                        place.Photos.Add(photo != null ? photo.Url : Common.PlaceNoPhoto);
                    }
                }

                // тут какой-то сбой происходит. найти.
                // закомментил, буду потом разбираться. на текущий момент оно мне не требуется.
                place.Worktime = new List<Worktime>();
                //if ((placesPage.GetProperty("worktime") != null) && (placesPage.GetProperty("worktime").HasValue))
                //{
                //    var worktimes = ((IEnumerable<IPublishedContent>)placesPage.GetProperty("worktime").Value).ToList();
                //    foreach (var worktime in worktimes)
                //    {
                //        Worktime wt = new Worktime();
                //        wt.FromTime = worktime.HasValue("fromTime") ? Common.StrToDateTime(worktime.GetPropertyValue("fromTime").ToString()) : DateTime.MinValue;
                //        wt.ToTime = worktime.HasValue("toTime") ? Common.StrToDateTime(worktime.GetPropertyValue("toTime").ToString()) : DateTime.MinValue;
                //        wt.Works24 = worktime.HasValue("works24") ? Common.StrToBool(worktime.GetPropertyValue("toTime").ToString()) : false;
                //        wt.WorktimeId = worktime.Id;
                //        wt.Breaks = new List<Break>();
                //        var breaks = ((IEnumerable<IPublishedContent>)worktime.GetProperty("breakTimes").Value).ToList();

                //        foreach (var breako in breaks)
                //        {
                //            Break b = new Break();
                //            b.FromTime = breako.HasValue("fromTime") ? Common.StrToDateTime(breako.GetPropertyValue("fromTime").ToString()) : DateTime.MinValue;
                //            b.ToTime = breako.HasValue("toTime") ? Common.StrToDateTime(breako.GetPropertyValue("toTime").ToString()) : DateTime.MinValue;
                //            b.BreakId = breako.Id;
                //            wt.Breaks.Add(b);
                //        }

                //        place.Worktime.Add(wt);
                //    }
                //}
            }

            return place;
        }
        
        
        /// <summary>
        /// возвращает Place по его placeid (для пользования внутри серверной части, не для HTTP-клиента!)
        /// </summary>
        /// <param name="placeid">System.Int32</param>
        /// <returns>Place</returns>
        public Place GetPlaceById(int placeid)
        {
            //Umbraco.Web.Mvc.UmbracoViewPage

            IPublishedContent placesPage = null;

            Place place = new Place();

            if (placeid != -1)
            {
                var node = umbraco.uQuery.GetNodesByType("places").FirstOrDefault(x => x.Id == placeid);
                //Common.StrToInt(x.GetProperty("").Value)

                if (node != null)
                {
                    placesPage = Umbraco.TypedContent(node.Id);
                }

                if (placesPage.GetProperty("glavnoeMenyu") != null)
                {
                    place = (Place)placesPage.GetProperty("glavnoeMenyu").Value;
                }
            }
            

            return place;
        }


        /// <summary>
        /// Возвращает список мест по Id города из движка и начальный индекс (для пагинации). работает только внутри сервера, не для POST
        /// </summary>
        /// <param name="umbracoId"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public List<Place> getAllInCityFromUmbracoDatabaseByCityUmbracoId(int umbracoId, int startIndex = 0)
        {
            try
            {
                IPublishedContent placePage = null;
                List<Place> places = new List<Place>();

                if (umbracoId != -1)
                {
                    var cityNode = umbraco.uQuery.GetNode(umbracoId);
                    if ((cityNode != null) && (cityNode.Children.Count > 0))
                    {
                        //cityNode - город
                        var placeNodes = cityNode.ChildrenAsList;
                            //.Skip(startIndex)
                            //.Take(Common.RowsToReturnByTransaction); // подузлы Мест узла Город

                        foreach (var pl in placeNodes)
                        {
                            placePage = Umbraco.TypedContent(pl.Id);
                            var place = PublishedContentToPlace(placePage);
                            if (place != null)
                                places.Add(place);
                        }
                    }
                }

                return places;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
