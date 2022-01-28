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
    public class UserProfileController : UmbracoApiController
    {
        HttpRequest currentRequest = HttpContext.Current.Request;

        /// <summary>
        /// Возвращает UserProfile по значению из параметров запуска. Для использования от HTTP-клиента.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public UserProfile GetUserProfile()
        {
            UserProfile uProfile = new UserProfile();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                Dictionary<string, string> userParameters = Common.GetParametersFromVkQueryString(currentRequest.QueryString, true);
                string userVkId = "id" + userParameters["vk_user_id"];

                DefaultContext dc = new DefaultContext();
                uProfile = (userVkId.Trim() != "") ? dc.UserProfiles.FirstOrDefault(x => x.UserVkId == userVkId) : null;
            }


            return uProfile;
        }


        /// <summary>
        /// Возвращает UserProfile по его Id. Для использования от HTTP-клиента.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public UserProfile GeUserProfileById()
        {
            UserProfile uProfile = new UserProfile();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                DefaultContext dc = new DefaultContext();
                int userProfileId = Common.StrToInt(currentRequest.Form.GetValues("userprofileid").FirstOrDefault().ToString());
                uProfile = (userProfileId > -1) ? dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileId) : null;
            }


            return uProfile;
        }


        /// <summary>
        /// Возвращает все UserProfile. Для использования от HTTP-клиента. Принимает параметр userprofilestartindex через POST (начальный индекс пользователя для пагинации)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<UserProfile> GetUserProfiles()
        {
            List<UserProfile> uProfiles = new List<UserProfile>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                DefaultContext dc = new DefaultContext();
                int userProfileStartIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex").FirstOrDefault().ToString()) : 0;
                uProfiles = (dc.UserProfiles.Count() > userProfileStartIndex) ? dc.UserProfiles.Skip(userProfileStartIndex).Take(Common.RowsToReturnByTransaction).ToList() : null;
            }


            return uProfiles;
        }



        /// <summary>
        /// Возвращает UserProfile по его Id. Для использования внутри сервера.
        /// </summary>
        /// <param name="userProfileId">Id профиля пользователя</param>
        /// <returns></returns>
        public UserProfile GeUserProfileById(int userProfileId)
        {
            DefaultContext dc = new DefaultContext();
            
            return (userProfileId > -1) ? dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileId) : null;
        }
        
        
        /// <summary>
        /// Возвращает UserProfile по его Vk Id. Для использования внутри сервера.
        /// </summary>
        /// <param name="userProfileVkId">Vk Id профиля пользователя</param>
        /// <returns></returns>
        public UserProfile GeUserProfileByVkId(string userProfileVkId)
        {
            DefaultContext dc = new DefaultContext();
            
            return (userProfileVkId.Length > 2) ? dc.UserProfiles.FirstOrDefault(x => x.UserVkId == userProfileVkId) : null;
        }


        /// <summary>
        /// Возвращает все UserProfile. Для использования внутри сервера. 
        /// </summary>
        /// <param name="userProfileStartIndex">Стартовый индекс возвращаемого списка</param>
        /// <returns></returns>
        public List<UserProfile> GetAllUserProfiles(int userProfileStartIndex)
        {
            DefaultContext dc = new DefaultContext();

            return (dc.UserProfiles.Count() > userProfileStartIndex) ? dc.UserProfiles.Skip(userProfileStartIndex).Take(Common.RowsToReturnByTransaction).ToList() : null;
        }


        /// <summary>
        /// Обновление профиля пользователя. В случае успеха возвращает обновленный UserProfile, иначе null
        /// </summary>
        /// <param name="newUserProfile">UserProfile заполненный новыми данными</param>
        /// <param name="userProfileId">Id профиля пользователя для обновления</param>
        /// <returns>обновленный UserProfile или null</returns>
        public UserProfile UpdateUserProfile(UserProfile newUserProfile, int userProfileId)
        {
            DefaultContext dc = new DefaultContext();
            UserProfile modUserProfile = new UserProfile(newUserProfile);
            modUserProfile.UserProfileId = userProfileId;

            try
            {
                dc.Entry<UserProfile>(modUserProfile).State = System.Data.Entity.EntityState.Modified;
                dc.SaveChanges();
            }
            catch
            {
                return null;
            }
            return modUserProfile;
        }
        

/*
        /// <summary>
        /// Команда на обновление профиля пользователя, отправленная с HTTP-клиента
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public UserProfile UpdateUserProfile()
        {
            UserProfile uProfile = new UserProfile();


            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                DefaultContext dc = new DefaultContext();
                int userProfileId = (currentRequest.Form.Get("userprofileid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("userprofileid").FirstOrDefault().ToString()) : -1;

                if (userProfileId > -1)
                {
                    uProfile = (dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileId) != null) ? dc.UserProfiles.FirstOrDefault(x => x.UserProfileId == userProfileId) : null;


                    if (uProfile == null)
                    {
                        return null;
                    }
                    uProfile.Birth = (currentRequest.Form.Get("userprofilebirth") != null) ? Common.StrToDateTime(currentRequest.Form.GetValues("userprofilebirth").FirstOrDefault()) : uProfile.Birth;
                    uProfile.LastOnline = DateTime.UtcNow;


                    uProfile.CityId = (currentRequest.Form.Get("userprofilecityid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("userprofilecityid").FirstOrDefault()) : uProfile.CityId;
                    if (uProfile.CityId >= 0)
                    {
                        var CityNode = umbraco.uQuery.GetNodesByType("city");
                        IPublishedContent CityPage = (CityNode.Count() > 0) ? Umbraco.TypedContent(CityNode.FirstOrDefault(x => x.Id == uProfile.CityId)) : null;
                        uProfile.City = CityPage.GetProperty("cityname") != null ? new City(uProfile.CityId, CityPage.GetProperty("cityname").Value.ToString()) : uProfile.City;
                    }


                    uProfile.LegId = (currentRequest.Form.Get("userprofilelegid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("userprofilelegid").FirstOrDefault()) : uProfile.LegId;
                    if (uProfile.LegId >= 0)
                    {
                        var LegNode = umbraco.uQuery.GetNodesByType("leg");
                        IPublishedContent LegPage = (LegNode.Count() > 0) ? Umbraco.TypedContent(LegNode.FirstOrDefault(x => x.Id == uProfile.LegId)) : null;
                        uProfile.Leg = LegPage.GetProperty("legname") != null ? new Leg(uProfile.LegId, LegPage.GetProperty("legname").Value.ToString(), Common.StrToBool(LegPage.GetProperty("default").Value.ToString()), uProfile.LegId.Value) : uProfile.Leg;
                    }
                    
                    
                    uProfile.AmpluaId = (currentRequest.Form.Get("userprofileampluaid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("userprofileampluaid").FirstOrDefault()) : uProfile.AmpluaId;
                    if (uProfile.AmpluaId >= 0)
                    {
                        var AmpluaNode = umbraco.uQuery.GetNodesByType("amplua");
                        IPublishedContent AmpluaPage = (AmpluaNode.Count() > 0) ? Umbraco.TypedContent(AmpluaNode.FirstOrDefault(x => x.Id == uProfile.AmpluaId)) : null;
                        uProfile.Amplua = (AmpluaPage.GetProperty("legname") != null) ? new Amplua(uProfile.AmpluaId, AmpluaPage.GetProperty("ampluaname").Value.ToString(), Common.StrToBool(AmpluaPage.GetProperty("default").Value.ToString()), uProfile.AmpluaId.Value) : uProfile.Amplua;
                    }
                    
                    uProfile.Name = (currentRequest.Form.Get("userprofilename") != null) ? (currentRequest.Form.GetValues("userprofilename").FirstOrDefault()) : uProfile.Name;
                    uProfile.Surname = (currentRequest.Form.Get("userprofilesurname") != null) ? (currentRequest.Form.GetValues("userprofilesurname").FirstOrDefault()) : uProfile.Surname;
                    uProfile.Fathername = (currentRequest.Form.Get("userprofilefathername") != null) ? (currentRequest.Form.GetValues("userprofilefathername").FirstOrDefault()) : uProfile.Fathername;
                    
                    
                    uProfile.Height = (currentRequest.Form.Get("userprofileheight") != null) ? Common.StrToInt(currentRequest.Form.GetValues("userprofileheight").FirstOrDefault()) : uProfile.Height;
                    uProfile.Weight = (currentRequest.Form.Get("userprofileweight") != null) ? Common.StrToInt(currentRequest.Form.GetValues("userprofileweight").FirstOrDefault()) : uProfile.Weight;
                    uProfile.UserProfileId = (currentRequest.Form.Get("userprofileid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("userprofileid").FirstOrDefault()) : uProfile.UserProfileId;
                    uProfile.UserVkId = (currentRequest.Form.Get("userprofilevkid") != null) ? (currentRequest.Form.GetValues("userprofilevkid").FirstOrDefault()) : uProfile.UserVkId;

                    uProfile = UpdateUserProfile(uProfile, uProfile.UserProfileId);
                    if (uProfile == null)
                        return null;
                }
                else
                {
                    return null;
                }
            }

            return uProfile;
        }
        */

    }
}
