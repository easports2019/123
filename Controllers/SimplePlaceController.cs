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
    public class SimplePlaceController : UmbracoApiController
    {
        HttpRequest currentRequest = HttpContext.Current.Request;

        [HttpOptions]
        public dynamic EditSimplePlace()
        {
            return Ok();
        }

        [HttpPost]
        public dynamic EditSimplePlace(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                )
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    int id = -1;
                    SimpleCollect newCollect = new SimpleCollect();


                    try
                    {



                        return newCollect;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Добавление простого места в базу
        /// </summary>
        /// <param name="simpleCollectUserProfile"></param>
        /// <returns></returns>
        public dynamic AddSimplePlace(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                )
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();
                    int id = -1;
                    SimpleCollect newCollect = new SimpleCollect();


                    try
                    {

                        return newCollect;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        [HttpPost]
        public dynamic GetSimplePlacesInCity()
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                )
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    //List<SimplePlace> places = new List<SimplePlace>();

                    try
                    {
                        DateTime now = DateTime.Today;
                        int cityumbracoid = (currentRequest.Form.Get("cityumbracoid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityumbracoid")[0]) : -1;
                        // нужно, чтобы возвращало также расписание (и занятое время в отдельном запросе)

                        var places = dc.SimplePlaces
                            .Join(dc.SimpleCitys, a => a.SimpleCityId, b => b.Id, (spla, scit) => new { spla, scit })
                            .Where(x => x.scit.CityUmbracoId == cityumbracoid)
                            .SelectMany(sm => dc.Worktimes
                                        .Where(w => !w.Deleted && w.Published && (w.SimplePlaceId == sm.spla.Id)
                                        //&& (w.FromTime >= now)
                                        )
                                        .GroupBy(r => r.SimplePlaceId)
                                        .Select(x => new
                                        {
                                            SimpleCityId = sm.spla.SimpleCityId,
                                            Id = sm.spla.Id,
                                            Address = sm.spla.Address,
                                            BicycleParking = sm.spla.BicycleParking,
                                            Deleted = sm.spla.Deleted,
                                            City = sm.scit,
                                            Enabled = sm.spla.Enabled,
                                            Geo = sm.spla.Geo,
                                            Info = sm.spla.Info,
                                            MainPicture = sm.spla.MainPicture,
                                            Name = sm.spla.Name,
                                            Parking = sm.spla.Parking,
                                            Published = sm.spla.Published,
                                            UmbracoId = sm.spla.UmbracoId,
                                            Worktime = x.GroupJoin(dc.Breaks,
                                            a => a.WorktimeId,
                                            b => b.WorktimeId,
                                            (worktime, breakCollection) =>
                                            new
                                            {
                                                WorktimeId = worktime.WorktimeId,
                                                SimplePlace = worktime.SimplePlace,
                                                SimplePlaceId = worktime.SimplePlaceId,
                                                FromTime = worktime.FromTime,
                                                ToTime = worktime.ToTime,
                                                Works24 = worktime.Works24,
                                                CostPerHour = worktime.CostPerHour,
                                                When = worktime.When,
                                                Deleted = worktime.Deleted,
                                                Published = worktime.Published,
                                                Breaks = breakCollection.Select(o => o),
                                            }
                                            ).ToList(),

                                        }
                                        )
                            //.Join(dc.Breaks, a => a.wt.WorktimeId, b => b.WorktimeId, (splascitwt, bt) => new { splascitwt, bt})


                            )



                            //dc.SimplePlaces
                            //.Join(dc.SimpleCitys, a => a.SimpleCityId, b => b.Id, (spla, scit) => new { spla, scit })
                            //.Where(x => x.scit.CityUmbracoId == cityumbracoid)
                            //.SelectMany(sm => dc.Worktimes
                            //            .Where(w => !w.Deleted && w.Published && (w.SimplePlaceId == sm.spla.Id))
                            //            .GroupBy(r => r.SimplePlaceId)
                            //            .Select(x => new
                            //            {
                            //                SimpleCityId = sm.spla.SimpleCityId,
                            //                Id = sm.spla.Id,
                            //                Address = sm.spla.Address,
                            //                BicycleParking = sm.spla.BicycleParking,
                            //                Deleted = sm.spla.Deleted,
                            //                City = sm.scit,
                            //                Enabled = sm.spla.Enabled,
                            //                Geo = sm.spla.Geo,
                            //                Info = sm.spla.Info,
                            //                MainPicture = sm.spla.MainPicture,
                            //                Name = sm.spla.Name,
                            //                Parking = sm.spla.Parking,
                            //                Published = sm.spla.Published,
                            //                UmbracoId = sm.spla.UmbracoId,
                            //                Worktime = x.SelectMany(b => dc.Breaks
                            //                            .Where(w => !w.Deleted && w.Published)
                            //                            .GroupBy(gr => gr.WorktimeId)
                            //                            .Select(y => new
                            //                            {
                            //                                WorktimeId = b.WorktimeId,
                            //                                SimplePlace = b.SimplePlace,
                            //                                SimplePlaceId = b.SimplePlaceId,
                            //                                FromTime = b.FromTime,
                            //                                ToTime = b.ToTime,
                            //                                Works24 = b.Works24,
                            //                                CostPerHour = b.CostPerHour,
                            //                                When = b.When,
                            //                                Deleted = b.Deleted,
                            //                                Published = b.Published,
                            //                                Breaks = y.ToList(),
                            //                            })


                            //                ),

                            //            }
                            //            )
                            //                //.Join(dc.Breaks, a => a.wt.WorktimeId, b => b.WorktimeId, (splascitwt, bt) => new { splascitwt, bt})
                                        
                                            
                            //)
                            .ToList();

                        return places;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// возвращает простой город по идентификатору
        /// </summary>
        /// <param name="placeId"></param>
        /// <returns></returns>
        public SimplePlace GetPlaceById(int? placeId)
        {
            using (DefaultContext dc = new DefaultContext())
            {
                return dc.SimplePlaces.FirstOrDefault(x => x.Id == placeId);
            }
        }
    }
}