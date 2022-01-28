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
    public class SimpleCollectController : UmbracoApiController
    {
        HttpRequest currentRequest = HttpContext.Current.Request;

        [HttpPost]
        /// <summary>
        /// Возвращает сборы по выбранному городу
        /// </summary>
        /// <returns></returns>
        public dynamic GetSimpleCollectsInCityByCityId()
        {
            int cityId = (currentRequest.Form.Get("cityId") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityId")[0]) : 0;
            bool tmp = false;
            bool history = (currentRequest.Form.Get("history") != null) ? 
                (Boolean.TryParse(currentRequest.Form.GetValues("history")[0].ToString(), out tmp) ? tmp : false) : 
                false;

            // доработать пагинацию с использованием startindex и длины страницы как для истории, так и для актуальных
            new WidgetController().SetWidget();

            // проверяем авторизацию запроса
            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    try
                    {
                        DateTime now = DateTime.Today;
                        var res1 = history ?
                            dc.SimpleCollects
                            .Where(sc => sc.When <= now) // only old
                            :
                            dc.SimpleCollects
                            .Where(sc => sc.When > now); // only actual

                        var res = res1
                            .Join(dc.SimplePlaces, a => a.SimplePlaceId, b => b.Id, (collect, place) => new { collect, place })
                            .Join(dc.SimpleCitys, a => a.place.SimpleCityId, b => b.Id, (collectplace, city) => new { collectplace, city })
                            .Join(dc.UserProfiles, a => a.collectplace.collect.CreatorId, b => b.UserProfileId, (collectplacecity, userprofile) => new { collectplacecity, userprofile })
                            .Where(item => item.collectplacecity.city.CityUmbracoId == cityId)
                            .Select(x => new {
                                place = x.collectplacecity.collectplace.place,
                                collect = x.collectplacecity.collectplace.collect,
                                city = x.collectplacecity.city,
                                user = x.userprofile
                            })
                            .SelectMany(sm => dc.SimpleMembers
                            .Where(mem => ((mem.Published) && (!mem.Deleted)))
                            .Join(dc.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (member, userProfile) => new { 
                                Id = member.Id,
                                SimpleCollect = member.SimpleCollect,
                                SimpleCollectId = member.SimpleCollectId,
                                UserProfile = member.UserProfile,
                                UserProfileId = member.UserProfileId
                            })
                            .Where(m => ((m.SimpleCollectId == sm.collect.Id) && (m.SimpleCollect.Published) && (!m.SimpleCollect.Deleted)))
                            .GroupBy(r => r.SimpleCollectId)
                            .Select(r => new
                            {
                                Id = sm.collect.Id,
                                Name = sm.collect.Name,
                                When = sm.collect.When,
                                DurationMinutes = sm.collect.DurationMinutes,
                                Details = sm.collect.Details,
                                Comment = sm.collect.Comment,
                                Cost = sm.collect.Cost,
                                NeedMembers = sm.collect.NeedMembers,
                                PlaceId = sm.place.Id,
                                Place = sm.place,
                                Published = sm.collect.Published,
                                Deleted = sm.collect.Deleted,
                                FullPrice = sm.collect.FullPrice,
                                CreatorId = sm.collect.CreatorId,
                                Creator = new
                                {
                                    CityUmbracoName = sm.user.CityUmbracoName,
                                    Name = sm.user.Name,
                                    Surname = sm.user.Surname,
                                    CityVkId = sm.user.CityVkId,
                                    UserProfileId = sm.user.UserProfileId,
                                    UserVkId = sm.user.UserVkId,
                                    LastOnline = sm.user.LastOnline,
                                    Birth = sm.user.Birth,
                                    PhotoPath = sm.user.PhotoPath
                                },
                                Members = r
                            })
                            )
                            .ToList()
                            ;

                        return res;
                    }
                    catch(Exception ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        [HttpOptions]
        public dynamic DeleteMember()
        {
            return Ok();
            //deleteMemberFromSimpleCollect
        }


        /// <summary>
        /// удаление участника сбора
        /// </summary>
        /// <param name="simpleCollectUserProfileMember">объект, содержащий профиль пользователя, сбор и информацию об участнике</param>
        /// <returns></returns>
        [HttpPost]
        public dynamic DeleteMember(SimpleCollectUserProfileMember simpleCollectUserProfileMember)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (ModelState.IsValid)
                && ((new Common().UserProfileHasAccessToSimpleCollect(simpleCollectUserProfileMember.userProfile, simpleCollectUserProfileMember.simpleCollect))
                || simpleCollectUserProfileMember.simpleMember.UserProfileId == simpleCollectUserProfileMember.userProfile.UserProfileId)
                )
            {
                if (simpleCollectUserProfileMember.simpleMember.UserProfileId == simpleCollectUserProfileMember.simpleCollect.CreatorId) // нельзя удалить орагнизатора
                {
                    return new Tournament("Error in DeleteMember: you can't delete collect creator");
                }

                SimpleMember smem = null;
                using (DefaultContext dc1 = new DefaultContext())
                {
                    smem = dc1.SimpleMembers.FirstOrDefault(sm => sm.SimpleCollectId == simpleCollectUserProfileMember.simpleCollect.Id
                    && sm.UserProfileId == simpleCollectUserProfileMember.simpleMember.UserProfileId);
                }

                if (smem != null)
                {
                    using (DefaultContext dc = new DefaultContext())
                    {
                        System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();

                        try
                        {
                            smem.Deleted = true;
                            smem.Comment = simpleCollectUserProfileMember.simpleMember.Comment;
                            dc.Entry(smem).State = EntityState.Modified;
                            //dc.Entry<Tournament>(tmpTourn).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();

                            context.Commit();

                            
                        }
                        catch (Exception ex)
                        {
                            context.Rollback();
                            return new Tournament("Error in DeleteMember: " + ex.Message);
                        }

                        
                    }

                    using (DefaultContext dc2 = new DefaultContext())
                    {
                        var xxx = dc2.SimpleMembers
                                .Join(dc2.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (memb, prof) => new { memb, prof })
                                .Join(dc2.SimpleCollects, a => a.memb.SimpleCollectId, b => b.Id, (membprof, scol) => new { membprof, scol })
                                .Where(sc => sc.membprof.memb.Id == smem.Id)
                                .Select(x => new
                                {
                                    Id = x.membprof.memb.Id,
                                    Comment2 = x.membprof.memb.Comment2,
                                    Comment = x.membprof.memb.Comment,
                                    Deleted = x.membprof.memb.Deleted,
                                    ErrorMessage = x.membprof.memb.ErrorMessage,
                                    PayCard = x.membprof.memb.PayCard,
                                    PayCash = x.membprof.memb.PayCash,
                                    Published = x.membprof.memb.Published,
                                    SimpleCollect = x.scol,
                                    SimpleCollectId = x.membprof.memb.SimpleCollectId,
                                    SimpleMemberTypeName = x.membprof.memb.SimpleMemberTypeName,
                                    UserProfile = x.membprof.prof,
                                    UserProfileId = x.membprof.memb.UserProfileId,
                                }
                                ).FirstOrDefault();

                        ;
                        return xxx;
                    }
                }
            }

            return null;
            //deleteMemberFromSimpleCollect
        }
        
        
        [HttpOptions]
        public dynamic RegisterMember()
        {
            return Ok();
            //deleteMemberFromSimpleCollect
        }


        /// <summary>
        /// регистрация участника на сбор
        /// </summary>
        /// <param name="simpleCollectUserProfile">объект, содержащий профиль пользователя и сбор</param>
        /// <returns></returns>
        [HttpPost]
        public dynamic RegisterMember(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (ModelState.IsValid)
                )
            {
                

                SimpleMember smem = null;
                using (DefaultContext dc1 = new DefaultContext())
                {
                    smem = dc1.SimpleMembers.FirstOrDefault(sm => sm.SimpleCollectId == simpleCollectUserProfile.simpleCollect.Id
                    && sm.UserProfileId == simpleCollectUserProfile.userProfile.UserProfileId);
                }

                if (smem != null) // участник уже регистрировался, удалялся, но хочет вновь зарегистрироваться
                {
                    using (DefaultContext dc = new DefaultContext())
                    {
                        System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();

                        try
                        {
                            smem.Deleted = false;

                            dc.Entry(smem).State = EntityState.Modified;
                            //dc.Entry<Tournament>(tmpTourn).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            context.Commit();


                        }
                        catch (Exception ex)
                        {
                            context.Rollback();
                            return new Tournament("Error in RegisterMember: " + ex.Message);
                        }
                    }

                    using (DefaultContext dc2 = new DefaultContext())
                    {
                        var xxx = dc2.SimpleMembers
                                .Join(dc2.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (memb, prof) => new { memb, prof })
                                .Join(dc2.SimpleCollects, a => a.memb.SimpleCollectId, b => b.Id, (membprof, scol) => new { membprof, scol })
                                .Where(sc => sc.membprof.memb.Id == smem.Id)
                                .Select(x => new
                                {
                                    Id = x.membprof.memb.Id,
                                    Comment2 = x.membprof.memb.Comment2,
                                    Comment = x.membprof.memb.Comment,
                                    Deleted = x.membprof.memb.Deleted,
                                    ErrorMessage = x.membprof.memb.ErrorMessage,
                                    PayCard = x.membprof.memb.PayCard,
                                    PayCash = x.membprof.memb.PayCash,
                                    Published = x.membprof.memb.Published,
                                    SimpleCollect = x.scol,
                                    SimpleCollectId = x.membprof.memb.SimpleCollectId,
                                    SimpleMemberTypeName = x.membprof.memb.SimpleMemberTypeName,
                                    UserProfile = x.membprof.prof,
                                    UserProfileId = x.membprof.memb.UserProfileId,
                                }
                                ).FirstOrDefault()

                                ;
                        return xxx;
                    }
                }
                else // участник хочет в первый раз зарегистрироваться
                {
                    using (DefaultContext dc = new DefaultContext())
                    {
                        System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();

                        try
                        {
                            SimpleMember sm = new SimpleMember();
                            sm.UserProfileId = simpleCollectUserProfile.userProfile.UserProfileId;
                            sm.SimpleCollectId = simpleCollectUserProfile.simpleCollect.Id;
                            sm.Deleted = false;
                            sm.Published = true;
                            sm.SimpleMemberTypeName = "member";

                            dc.SimpleMembers.Add(sm);

                            dc.SaveChanges();
                            context.Commit();

                            smem = sm;
                        }
                        catch (Exception ex)
                        {
                            context.Rollback();
                            return new Tournament("Error in RegisterMember: " + ex.Message);
                        }
                    }

                    using (DefaultContext dc2 = new DefaultContext())
                    {
                        var xxx = dc2.SimpleMembers
                                .Join(dc2.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (memb, prof) => new { memb, prof })
                                .Join(dc2.SimpleCollects, a => a.memb.SimpleCollectId, b => b.Id, (membprof, scol) => new { membprof, scol })
                                .Where(sc => sc.membprof.memb.Id == smem.Id)
                                .Select(x => new
                                {
                                    Id = x.membprof.memb.Id,
                                    Comment2 = x.membprof.memb.Comment2,
                                    Comment = x.membprof.memb.Comment,
                                    Deleted = x.membprof.memb.Deleted,
                                    ErrorMessage = x.membprof.memb.ErrorMessage,
                                    PayCard = x.membprof.memb.PayCard,
                                    PayCash = x.membprof.memb.PayCash,
                                    Published = x.membprof.memb.Published,
                                    SimpleCollect = x.scol,
                                    SimpleCollectId = x.membprof.memb.SimpleCollectId,
                                    SimpleMemberTypeName = x.membprof.memb.SimpleMemberTypeName,
                                    UserProfile = x.membprof.prof,
                                    UserProfileId = x.membprof.memb.UserProfileId,
                                }
                                ).FirstOrDefault()

                                ;
                        return xxx;
                    }
                }
            }

            return null;
            //deleteMemberFromSimpleCollect
        }

        public dynamic AddInviteMember()
        {
            return null;
        }

        public dynamic DelInviteMember()
        {
            return null;
        }

        public dynamic ApproveToInviteMember()
        {
            return null;
        }

        public dynamic DeclineToInviteMember()
        {
            return null;
        }


        public dynamic AddWantstoMember()
        {
            return null;
        }

        public dynamic DelWantstoMember()
        {
            return null;
        }

        public dynamic ApproveWantstoMember()
        {
            return null;
        }

        public dynamic DeclineWantstoMember()
        {
            return null;
        }

        [HttpOptions]
        public dynamic AddCollect()
        {
            return Ok();
        }

        [HttpPost]
        public dynamic AddCollect(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            SimpleCollect sColOrig = simpleCollectUserProfile.simpleCollect;
            UserProfile userOrig = simpleCollectUserProfile.userProfile;

            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                )
            {
                UserProfile me = new UserProfile();
                SimpleCollect newCollect = new SimpleCollect();
                List<Worktime> wtms = new List<Worktime>();
                Decimal fPrice = 0;

                using (DefaultContext dc0 = new DefaultContext())
                {
                    me = dc0.UserProfiles.FirstOrDefault(u => u.UserProfileId == userOrig.UserProfileId);
                    newCollect = dc0.SimpleCollects.Where(sc => 
                    (
                        (sc.CreatorId == userOrig.UserProfileId) &&
                        (sc.SimplePlaceId == sColOrig.SimplePlaceId) &&
                        (sc.When == sColOrig.When) &&
                        (sc.Deleted == false) &&
                        (sc.Published == true)
                    )
                    ).FirstOrDefault();

                    //// узнаем сколько стоит выбранное время, чтобы записать в базу реальное значение, а не пришедшее с клиента
                    //SimplePlace splace = dc0.SimplePlaces.FirstOrDefault(sp => sp.Id == sColOrig.SimplePlaceId);
                    //if (splace != null) // площадка реально существует
                    //{
                    //    // в wtimes все диапазоны времени этого места с ценами за час.
                    //    var wtimes = dc0.Worktimes.Where(wt => (
                    //        (wt.SimplePlaceId == sColOrig.SimplePlaceId)
                    //        && (sColOrig.When.Date == wt.FromTime.Date)
                    //    //&& (wt.FromTime.AddMinutes(Common.TimeRangeInMinutes(wt.ToTime, wt.FromTime)) < sColOrig.When.AddMinutes(sColOrig.DurationMinutes))
                    //    ));

                    //    List<Worktime> timesPerHalfHour = new List<Worktime>();

                    //    foreach (var w in wtimes)
                    //    {
                    //        for (int i=0; i < ((w.FromTime.TimeOfDay - w.ToTime.TimeOfDay).TotalMinutes / 30); i++)
                    //        {
                    //            Worktime tmp = new Worktime();
                    //            tmp.CostPerHour = w.CostPerHour / 2;
                    //            tmp.FromTime = w.FromTime.AddMinutes(i * 30);
                    //            tmp.ToTime = tmp.FromTime.AddMinutes(30);
                    //            timesPerHalfHour.Add(tmp);
                    //        }
                    //    }

                    //    double res = timesPerHalfHour.Where(tph => sColOrig.When >= tph.FromTime && sColOrig.When < tph.ToTime).Select(t => t.CostPerHour).Sum();

                    //    // считаем по полчаса
                        

                    //}
                }

                if ((me != null) && (newCollect == null))
                {
                    newCollect = new SimpleCollect();

                    using (DefaultContext dc = new DefaultContext())
                    {
                        System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();
                        int id = -1;
                        


                        try
                        {
                            if (ModelState.IsValid)
                            {

                                if (sColOrig != null)
                                {
                                    newCollect.Comment = sColOrig.Comment;
                                    newCollect.Cost = sColOrig.Cost;
                                    newCollect.CreatorId = sColOrig.CreatorId;
                                    newCollect.Details = sColOrig.Details;
                                    newCollect.DurationMinutes = sColOrig.DurationMinutes;
                                    newCollect.Name = sColOrig.Name;
                                    newCollect.NeedMembers = sColOrig.NeedMembers;
                                    newCollect.SimplePlaceId = sColOrig.SimplePlaceId;
                                    
                                    // !!! эту полную стоимость надо получать не с клиента, а из базы данных. переделать.
                                    newCollect.FullPrice = sColOrig.FullPrice;


                                    newCollect.When = sColOrig.When;
                                    newCollect.Published = true;
                                    newCollect.Deleted = false;

                                    // участник минимум один должен быть, иначе выборка не работает
                                    SimpleMember sm = new SimpleMember();
                                    sm.UserProfileId = me.UserProfileId;
                                    sm.Deleted = false;
                                    sm.Published = true;
                                    sm.SimpleMemberTypeName = "member";

                                    newCollect.SimpleMembers.Add(sm);

                                    
                                    newCollect.SimplePlace = null;
                                    newCollect.Creator = null;
                                    dc.SimpleCollects.Add(newCollect);

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


                            
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }

                    if ((newCollect != null) && (newCollect.Id > 0))
                        return GetCollectById(newCollect.Id, false);
                    else
                        return new SimpleCollect("Error in AddCollect: Collect wasn't created");

                }
                else
                {
                    return new SimpleCollect("Error in AddCollect: No user with that Id or collect by this user to this place on that time is already created");
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
        public dynamic DelCollect(SimpleCollectUserProfile simpleCollectUserProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
                && (new Common().UserProfileHasAccessToSimpleCollect(simpleCollectUserProfile.userProfile, simpleCollectUserProfile.simpleCollect))
                && (ModelState.IsValid)
                )
            {
                UserProfile me = new UserProfile();
                SimpleCollect collect = new SimpleCollect();

                using (DefaultContext dc0 = new DefaultContext())
                {
                    me = dc0.UserProfiles.FirstOrDefault(u => u.UserProfileId == simpleCollectUserProfile.userProfile.UserProfileId);
                    
                }

                if ((me != null) && (simpleCollectUserProfile.simpleCollect != null))
                {


                    using (DefaultContext dc = new DefaultContext())
                    {
                        System.Data.Entity.DbContextTransaction context = dc.Database.BeginTransaction();

                        try
                        {
                            collect = dc.SimpleCollects.Where(sc =>
                            (
                                sc.Id == simpleCollectUserProfile.simpleCollect.Id
                            )
                            ).FirstOrDefault();

                            if (collect != null)
                            {
                                var members = dc.SimpleMembers.Where(sm => (sm.SimpleCollectId == collect.Id));

                                collect.Published = false;
                                collect.Deleted = true;

                                // помечаем участников сбора неактивными
                                foreach (SimpleMember smem in members)
                                {
                                    smem.Published = false;
                                    smem.Deleted = true;
                                    dc.Entry(smem).State = EntityState.Modified;
                                }

                                dc.Entry(collect).State = EntityState.Modified;

                                dc.SaveChanges();
                                context.Commit();
                            }
                            else
                            {
                                context.Rollback();
                                return new SimpleCollect("Error in DelCollect: Collect with that Id not found");
                            }

                        }
                        catch (Exception ex)
                        {
                            context.Rollback();
                            return new SimpleCollect("Error in DelCollect: " + ex.Message);

                        }
                    }

                    return GetCollectById(simpleCollectUserProfile.simpleCollect.Id, true);

                }
                else
                {
                    return new SimpleCollect("Error in DelCollect: No collect with that Id");
                }
                
                
            }
            else
            {
                return new SimpleCollect("Error in DelCollect: You haven't access to this collect");

            }
        }


        /// <summary>
        /// Возвращает сбор по его Id
        /// </summary>
        /// <param name="collectId">Id сбора</param>
        /// <param name="withDeleted">если True, то перебор идет в том числе по помеченным как удаленные и не опубликованные</param>
        /// <returns></returns>
        public dynamic GetCollectById(int collectId, bool withDeleted)
        {
            using (DefaultContext dc = new DefaultContext())
            {
                var res = withDeleted ?
                    dc.SimpleCollects
                            .Join(dc.SimplePlaces, a => a.SimplePlaceId, b => b.Id, (collect, place) => new { collect, place })
                            .Join(dc.UserProfiles, a => a.collect.CreatorId, b => b.UserProfileId, (collectplacecity, userprofile) => new { collectplacecity, userprofile })
                            .Where(item => item.collectplacecity.collect.Id == collectId)
                            .Select(x => new
                            {
                                place = x.collectplacecity.place,
                                collect = x.collectplacecity.collect,
                                user = x.userprofile
                            })
                            .SelectMany(sm => dc.SimpleMembers
                            //.Where(mem => ((mem.Published) && (!mem.Deleted)))
                            .Join(dc.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (member, userProfile) => new
                            {
                                Id = member.Id,
                                SimpleCollect = member.SimpleCollect,
                                SimpleCollectId = member.SimpleCollectId,
                                UserProfile = member.UserProfile,
                                UserProfileId = member.UserProfileId
                            })
                            .Where(m => ((m.SimpleCollectId == sm.collect.Id) 
                            //&& (m.SimpleCollect.Published) && (!m.SimpleCollect.Deleted)
                            ))
                            .GroupBy(r => r.SimpleCollectId)
                            .Select(r => new
                            {
                                Id = sm.collect.Id,
                                Name = sm.collect.Name,
                                When = sm.collect.When,
                                DurationMinutes = sm.collect.DurationMinutes,
                                Details = sm.collect.Details,
                                Comment = sm.collect.Comment,
                                Cost = sm.collect.Cost,
                                NeedMembers = sm.collect.NeedMembers,
                                PlaceId = sm.place.Id,
                                Place = sm.place,
                                Published = sm.collect.Published,
                                Deleted = sm.collect.Deleted,
                                FullPrice = sm.collect.FullPrice,
                                CreatorId = sm.collect.CreatorId,
                                Creator = new
                                {
                                    CityUmbracoName = sm.user.CityUmbracoName,
                                    Name = sm.user.Name,
                                    Surname = sm.user.Surname,
                                    CityVkId = sm.user.CityVkId,
                                    UserProfileId = sm.user.UserProfileId,
                                    UserVkId = sm.user.UserVkId,
                                    LastOnline = sm.user.LastOnline,
                                    Birth = sm.user.Birth,
                                    PhotoPath = sm.user.PhotoPath
                                },
                                Members = r
                            })
                            )
                            .FirstOrDefault()
                    :
                    dc.SimpleCollects
                            .Join(dc.SimplePlaces, a => a.SimplePlaceId, b => b.Id, (collect, place) => new { collect, place })
                            .Join(dc.UserProfiles, a => a.collect.CreatorId, b => b.UserProfileId, (collectplacecity, userprofile) => new { collectplacecity, userprofile })
                            .Where(item => item.collectplacecity.collect.Id == collectId)
                            .Select(x => new
                            {
                                place = x.collectplacecity.place,
                                collect = x.collectplacecity.collect,
                                user = x.userprofile
                            })
                            .SelectMany(sm => dc.SimpleMembers
                            .Where(mem => ((mem.Published) && (!mem.Deleted)))
                            .Join(dc.UserProfiles, a => a.UserProfileId, b => b.UserProfileId, (member, userProfile) => new
                            {
                                Id = member.Id,
                                SimpleCollect = member.SimpleCollect,
                                SimpleCollectId = member.SimpleCollectId,
                                UserProfile = member.UserProfile,
                                UserProfileId = member.UserProfileId
                            })
                            .Where(m => ((m.SimpleCollectId == sm.collect.Id) && (m.SimpleCollect.Published) && (!m.SimpleCollect.Deleted)
                            ))
                            .GroupBy(r => r.SimpleCollectId)
                            .Select(r => new
                            {
                                Id = sm.collect.Id,
                                Name = sm.collect.Name,
                                When = sm.collect.When,
                                DurationMinutes = sm.collect.DurationMinutes,
                                Details = sm.collect.Details,
                                Comment = sm.collect.Comment,
                                Cost = sm.collect.Cost,
                                NeedMembers = sm.collect.NeedMembers,
                                PlaceId = sm.place.Id,
                                Place = sm.place,
                                Published = sm.collect.Published,
                                Deleted = sm.collect.Deleted,
                                FullPrice = sm.collect.FullPrice,
                                CreatorId = sm.collect.CreatorId,
                                Creator = new
                                {
                                    CityUmbracoName = sm.user.CityUmbracoName,
                                    Name = sm.user.Name,
                                    Surname = sm.user.Surname,
                                    CityVkId = sm.user.CityVkId,
                                    UserProfileId = sm.user.UserProfileId,
                                    UserVkId = sm.user.UserVkId,
                                    LastOnline = sm.user.LastOnline,
                                    Birth = sm.user.Birth,
                                    PhotoPath = sm.user.PhotoPath
                                },
                                Members = r
                            })
                            )
                            .FirstOrDefault()
                            ;
                return res;
            }
        }


        [HttpOptions]
        public dynamic EditCollect()
        {
            return Ok();
        }

        [HttpPost]
        public dynamic EditCollect(SimpleCollectUserProfile simpleCollectUserProfile)
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
                            SimpleCollect scol = dc.SimpleCollects.FirstOrDefault(sc => sc.Id == simpleCollectUserProfile.simpleCollect.Id);

                            if (scol != null)
                            {
                                scol.NeedMembers = simpleCollectUserProfile.simpleCollect.NeedMembers;
                                scol.Details = simpleCollectUserProfile.simpleCollect.Details;
                                scol.Cost = simpleCollectUserProfile.simpleCollect.Cost;
                                dc.Entry(scol).State = EntityState.Modified; // System.Data.Entity.EntityState.Modified;
                                dc.SaveChanges();
                                context.Commit();
                            }
                            else
                            {
                                context.Rollback();
                                return new SimpleCollect("Error in SimpleCollectUpdate: simpleCollect with that Id not found");
                            }
                        }
                        else
                        {
                            context.Rollback();
                            return new SimpleCollect("Error in SimpleCollectUpdate: ModelState isn't valid");
                        }


                        
                    }
                    catch (Exception ex)
                    {
                        return new SimpleCollect("Error in SimpleCollectUpdate: " + ex.Message);
                    }
                }

                return GetCollectById(simpleCollectUserProfile.simpleCollect.Id, false);
            }
            else
                return new SimpleCollect("Error in SimpleCollectUpdate: Not authorized");

        }
    }
}