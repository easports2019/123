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
    public class RentsController : UmbracoApiController
    {
        HttpRequest currentRequest = HttpContext.Current.Request;

        [HttpPost]
        /// <summary>
        /// Возвращает аренды по выбранному городу
        /// </summary>
        /// <returns></returns>
        public dynamic GetRentsInCityByCityId()
        {
            int cityId = (currentRequest.Form.Get("cityId") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityId")[0]) : 0;
            bool tmp = false;
            bool history = (currentRequest.Form.Get("history") != null) ?
                (Boolean.TryParse(currentRequest.Form.GetValues("history")[0].ToString(), out tmp) ? tmp : false) :
                false;

            // доработать пагинацию с использованием startindex и длины страницы как для истории, так и для актуальных


            // проверяем авторизацию запроса
            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        var res1 = history ?
                            dc.Rents
                            .Where(sc => sc.From <= now) // only old
                            :
                            dc.Rents
                            .Where(sc => sc.From > now); // only actual

                        var res = res1
                            .Join(dc.SimplePlaces, a => a.SimplePlaceId, b => b.Id, (rent, place) => new { rent, place })
                            .Join(dc.UserProfiles, a => a.rent.UserProfileId, b => b.UserProfileId, (rentplace, user) => new { rentplace, user })
                            .Select(x => new {
                                Cost = x.rentplace.rent.Cost,
                                Deleted = x.rentplace.rent.Deleted,
                                DurationMinutes = x.rentplace.rent.DurationMinutes,
                                ErrorMessage = x.rentplace.rent.ErrorMessage,
                                From = x.rentplace.rent.From,
                                Id = x.rentplace.rent.Id,
                                Published = x.rentplace.rent.Published,
                                Renter = x.user,
                                SimplePlace = x.rentplace.place,
                                SimplePlaceId = x.rentplace.rent.SimplePlaceId,
                                UserProfileId = x.rentplace.rent.UserProfileId,
                            })
                            .ToList()
                            ;

                        return res;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }



        [HttpOptions]
        public dynamic AddRent()
        {
            return Ok();
        }

        [HttpPost]
        public dynamic AddRent(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                )
            {
                UserProfile me = new UserProfile();
                using (DefaultContext dc0 = new DefaultContext())
                {
                    me = dc0.UserProfiles.FirstOrDefault(u => u.UserProfileId == simpleCollectUserProfile.userProfile.UserProfileId);
                }

                if (me != null)
                {
                    using (DefaultContext dc = new DefaultContext())
                    {
                        System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();
                        int id = -1;
                        SimpleCollect newCollect = new SimpleCollect();


                        try
                        {
                            if (ModelState.IsValid)
                            {

                                if (simpleCollectUserProfile.simpleCollect != null)
                                {
                                    newCollect.Comment = simpleCollectUserProfile.simpleCollect.Comment;
                                    newCollect.Cost = simpleCollectUserProfile.simpleCollect.Cost;
                                    newCollect.CreatorId = simpleCollectUserProfile.simpleCollect.CreatorId;
                                    newCollect.Details = simpleCollectUserProfile.simpleCollect.Details;
                                    newCollect.DurationMinutes = simpleCollectUserProfile.simpleCollect.DurationMinutes;
                                    newCollect.Name = simpleCollectUserProfile.simpleCollect.Name;
                                    newCollect.NeedMembers = simpleCollectUserProfile.simpleCollect.NeedMembers;
                                    newCollect.SimplePlaceId = simpleCollectUserProfile.simpleCollect.SimplePlaceId;
                                    newCollect.When = simpleCollectUserProfile.simpleCollect.When;

                                    // участник минимум один должен быть, иначе выборка не работает
                                    SimpleMember sm = new SimpleMember();
                                    sm.UserProfileId = me.UserProfileId;
                                    sm.Deleted = false;
                                    sm.Published = true;
                                    sm.SimpleMemberTypeName = "member";

                                    newCollect.SimpleMembers.Add(sm);


                                    newCollect.SimplePlace = null;
                                    newCollect.Creator = null;


                                    dc.SaveChanges();
                                    context.Commit();

                                    id = newCollect.Id;
                                }
                                else
                                {
                                    context.Rollback();
                                    return new SimpleCollect("Error in AddCollect");
                                }
                            }
                            else
                            {
                                context.Rollback();
                                return new SimpleCollect("Error in AddCollect: ModelState isn't valid");
                            }


                            return newCollect;
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return new SimpleCollect("Error in AddCollect: No user with that Id");
                }
            }

            return null;
        }


        [HttpOptions]
        public dynamic DelCollect()
        {
            return Ok();
        }

        [HttpPost]
        public dynamic DelRent(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToSimpleCollect(simpleCollectUserProfile.userProfile, simpleCollectUserProfile.simpleCollect))
                )
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();

                    try
                    {
                        if (ModelState.IsValid)
                        {

                            if (simpleCollectUserProfile.simpleCollect != null)
                            {

                                //dc.Entry<Tournament>(tmpTourn).State = System.Data.Entity.EntityState.Modified;
                                dc.SaveChanges();
                                context.Commit();
                            }
                            else
                            {
                                context.Rollback();
                                return new Tournament("Error in DelCollect: tournament with that Id not found");
                            }
                        }
                        else
                        {
                            context.Rollback();
                            return new Tournament("Error in DelCollect: ModelState isn't valid");
                        }


                        return simpleCollectUserProfile.simpleCollect;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }


        [HttpOptions]
        public dynamic EditRent()
        {
            return Ok();
        }

        [HttpPost]
        public dynamic EditRent(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToSimpleCollect(simpleCollectUserProfile.userProfile, simpleCollectUserProfile.simpleCollect))
                )
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();

                    try
                    {
                        if (ModelState.IsValid)
                        {

                            if (simpleCollectUserProfile.simpleCollect != null)
                            {

                                //dc.Entry<Tournament>(tmpTourn).State = System.Data.Entity.EntityState.Modified;
                                dc.SaveChanges();
                                context.Commit();
                            }
                            else
                            {
                                context.Rollback();
                                return new Tournament("Error in TournamentUpdate: tournament with that Id not found");
                            }
                        }
                        else
                        {
                            context.Rollback();
                            return new Tournament("Error in TournamentUpdate: ModelState isn't valid");
                        }


                        return simpleCollectUserProfile.simpleCollect;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }
    }
}