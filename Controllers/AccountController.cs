using System.Web.Http;
using System;
using System.Web;
using Umbraco.Web.WebApi;
using osbackend.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Models;
using System.Web.Http.Cors;
using System.Collections;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using osbackend.Utils;

namespace osbackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountController : UmbracoApiController
    {
        HttpRequest httpRequest = HttpContext.Current.Request;
        DefaultContext dc = new DefaultContext();


        [HttpOptions]
        public dynamic Register()
        {
            return Ok();
        }


        [HttpPost]
        public UserProfile Register(VKUSerData vkUserData)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(httpRequest.QueryString) != null) && (vkUserData != null))
            {
                try
                {
                    UserProfile user = dc.UserProfiles.Where(x => x.UserVkId == "id" + vkUserData.id).FirstOrDefault();

                    if (user == null)
                    {
                        user = new UserProfile(vkUserData);
                        user.UserVkId = "id" + vkUserData.id;
                        user.LastIp = httpRequest.UserHostAddress;
                        user.LastOnline = DateTime.UtcNow;
                        user.Weight = 0;
                        user.Height = 0;
                        City tmpCity = new CityController().GetCityFromBaseByCityVkId(vkUserData.city.id);

                        if (tmpCity != null)
                        {
                            user.CityUmbracoId = tmpCity.CityUmbracoId;
                            user.CityUmbracoName = tmpCity.CityUmbracoName;
                        }
                        else
                        {
                            user.CityUmbracoId = -1;
                            user.CityUmbracoName = "";
                        }
                        
                        // загрузить ногу (не установлено)
                        user.LegId = -1;
                        
                        // загрузить амплуа (не установлено)
                        user.AmpluaId = -1;

                        


                        dc.Entry<UserProfile>(user).State = System.Data.Entity.EntityState.Added;
                        if (dc.SaveChanges() > 0)
                        {
                            IMemberService mService = Services.MemberService;
                            string userLogin = "id" + vkUserData.id;
                            Umbraco.Core.Models.IMember memb = mService.GetByUsername(userLogin);

                            // регистрация аккаунта на бэке
                            UserProfile uProfile = RegisterAccount(user);
                            if ((uProfile == null) || (uProfile.ErrorMessage.Length > 0))
                                return new UserProfile("member not registred because of error in AC0001 RegisterAccount: " + uProfile.ErrorMessage);

                            return user;
                        }
                        else
                        {
                            return new UserProfile("UserProfile not registred because of error in AC0001 CreateUserInDB");
                        }

                    }
                    else // автоизован. загрузка аккаунта из базы
                    {
                        return new Common().UpdateUserProfile(user, vkUserData.bdate, httpRequest.UserHostAddress);
                    }


                }
                catch (Exception ex)
                {
                    return new UserProfile("not registred because of error in AC0001: " + ex.Message);
                }


            }
            else
                return new UserProfile("Авторизация не пройдена");
        }


        [HttpOptions]
        public dynamic GetUserProfile()
        {
            return Ok();
        }

        [HttpPost]
        public UserProfile GetUserProfile(VKUSerData vkUserData)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(httpRequest.QueryString) != null) && (vkUserData != null))
            {
                try
                {
                    UserProfile uProfile = dc.UserProfiles.Where(x => x.UserVkId == "id" + vkUserData.id).FirstOrDefault();
                    if (uProfile != null)
                    {
                        // бэкап значений
                        UserProfile uPr = new UserProfile(uProfile);

                        uProfile.LastOnline = DateTime.UtcNow;
                        uProfile.LastIp = httpRequest.UserHostAddress;
                        uProfile.CityUmbracoName = new CityController().GetCityUmbracoNameByCityUmbracoId(uProfile.CityUmbracoId.Value);


                        dc.Entry<UserProfile>(uProfile).State = System.Data.Entity.EntityState.Modified;

                        if (dc.SaveChanges() == 0)
                            uProfile = uPr;

                    }

                    return uProfile;
                }
                catch (Exception ex)
                {
                    return new UserProfile("not registred because of error in AC0001: " + ex.Message);
                }


            }
            else
            {
                string logres = Common.WriteToLog(httpRequest.QueryString.ToString());
                if (logres != null)
                    return new UserProfile("Авторизация не пройдена, " + logres);
                else
                    return new UserProfile("Авторизация не пройдена");

            }
        }

/*
        /// <summary>
        /// Заполнение данными профиля по умолчанию
        /// </summary>
        /// <param name="uProfile"></param>
        /// <returns></returns>
        private UserProfile SetDefaultsToAccount(UserProfile uProfile)
        {
            UserProfile uPr = new UserProfile(uProfile);
            IPublishedContent ampluaPage = null;
            IPublishedContent legPage = null;
            IPublishedContent cityPage = null;
            Amplua amplua = new Amplua();
            Leg leg = new Leg();
            City city = new City();

            var ampnodes = umbraco.uQuery.GetNodesByType("amplua");
            var legnodes = umbraco.uQuery.GetNodesByType("leg");
            var citnodes = umbraco.uQuery.GetNodesByType("city");

            if (ampnodes != null)
            {
                foreach (var ampnode in ampnodes)
                {
                    ampluaPage = Umbraco.TypedContent(ampnode.Id);
                    amplua = new AmpluaController().PublishedContentToAmplua(ampluaPage);
                    if (amplua.IsDefault)
                    {
                        uPr.Amplua = amplua;
                        uPr.AmpluaId = amplua.AmpluaId;
                        break;
                    }
                }
            }
            if (legnodes != null)
            {
                foreach (var legnode in legnodes)
                {
                    legPage = Umbraco.TypedContent(legnode.Id);
                    leg = new LegController().PublishedContentToLeg(legPage);
                    if (leg.IsDefault)
                    {
                        uPr.Leg = leg;
                        uPr.LegId = leg.LegId;
                        break;
                    }
                }

            }
            if (citnodes != null)
            {
                foreach (var citnode in citnodes)
                {
                    cityPage = Umbraco.TypedContent(citnode.Id);
                    city = new CityController().PublishedContentToCity(cityPage);
                    if (city.IsDefault)
                    {
                        uPr.City = city;
                        uPr.CityId = city.CityId;
                        break;
                    }
                }

            }
            
            uPr.Fathername = "";
            uPr.Height = 0;
            uPr.Weight = 0;
            

            return uPr;
        }

*/


        /// <summary>
        /// Регистрация аккаунта на площадке (нужны только VKid и текущее время (когда зарегистрирован и когда был крайний раз онлайн))
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        public UserProfile RegisterAccount(UserProfile userProfile)
        {
            IMemberService mService = Services.MemberService;
            IMember newMember = null;
            try
            {
                newMember = mService.CreateMemberWithIdentity(userProfile.UserVkId, userProfile.UserVkId + "@oblakosporta.alexsmirnovpro.ru", userProfile.Name + " " + userProfile.Surname, "AppMember");
                if (userProfile.UserVkId != "") newMember.SetValue("vkid", userProfile.UserVkId);

                newMember.SetValue("register", userProfile.Register);
                newMember.SetValue("lastonline", userProfile.LastOnline);
                mService.Save(newMember);
                return userProfile;
            }
            catch (System.Exception ex)
            {
                return new UserProfile(ex.Message + " (RegisterAccount)");
            }
        }


        [HttpOptions]
        public dynamic UpdateUserProfileCity()
        {
            return Ok();
        }


        [HttpPost]
        public UserProfile UpdateUserProfileCity(UserProfile userProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(httpRequest.QueryString) != null) && (userProfile != null))
            {
                try
                {
                    UserProfile user = dc.UserProfiles.Where(x => x.UserProfileId == userProfile.UserProfileId).FirstOrDefault();

                    if (user != null)
                    {
                        UserProfile userCopy = new UserProfile(user); 

                        user.CityUmbracoId = userProfile.CityUmbracoId;
                        user.CityUmbracoName = new CityController().GetCityUmbracoNameByCityUmbracoId(userProfile.CityUmbracoId.Value);
                        user.LastIp = httpRequest.UserHostAddress;
                        user.LastOnline = DateTime.UtcNow;

                        dc.Entry<UserProfile>(user).State = System.Data.Entity.EntityState.Modified;

                        if (dc.SaveChanges() > 0)
                        {
                            return user;
                        }
                        else
                        {
                            return new UserProfile("Error in UpdateUserProfileCity: изменения не сохранены");
                        }
                    }
                    else
                    {
                        return new UserProfile(String.Format("Error in UpdateUserProfileCity: пользователь с таким UserProfileId={0} не найден", userProfile.UserProfileId));
                    }
                }
                catch (Exception ex)
                {
                    return new UserProfile("Error in UpdateUserProfileCity: " + ex.Message);
                }
            }
            else
            {
                return new UserProfile("Error in UpdateUserProfileCity: not authorized");
            }


            
        }
        
        [HttpOptions]
        public dynamic UpdateUserProfile()
        {
            return Ok();
        }


        [HttpPost]
        public UserProfile UpdateUserProfile(UserProfile userProfile)
        {
            if ((Common.CheckSignAndGetUserAndRequestDate(httpRequest.QueryString) != null) && (userProfile != null))
            {
                try
                {
                    UserProfile user = dc.UserProfiles.Where(x => x.UserProfileId == userProfile.UserProfileId).FirstOrDefault();

                    if (user != null)
                    {
                        UserProfile userCopy = new UserProfile(user);

                        user.Name = userProfile.Name;
                        user.Surname = userProfile.Surname;
                        user.TotalExpirience = userProfile.TotalExpirience;
                        user.Birth = userProfile.Birth;
                        user.LastIp = httpRequest.UserHostAddress;
                        user.LastOnline = DateTime.UtcNow;

                        dc.Entry<UserProfile>(user).State = System.Data.Entity.EntityState.Modified;

                        if (dc.SaveChanges() > 0)
                        {
                            return user;
                        }
                        else
                        {
                            return new UserProfile("Error in UpdateUserProfileCity: изменения не сохранены");
                        }
                    }
                    else
                    {
                        return new UserProfile(String.Format("Error in UpdateUserProfileCity: пользователь с таким UserProfileId={0} не найден", userProfile.UserProfileId));
                    }
                }
                catch (Exception ex)
                {
                    return new UserProfile("Error in UpdateUserProfileCity: " + ex.Message);
                }
            }
            else
            {
                return new UserProfile("Error in UpdateUserProfileCity: not authorized");
            }


            
        }


        /// <summary>
        /// Возвращает UserProfile из переданного в параметрах Member и обновляет поля LastIP и LastOnline
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public UserProfile GetUserProfileFromMemberWithUpdateInfo(IMember member, HttpRequest currentHttpRequest)
        {
            DefaultContext dc = new DefaultContext();

            UserProfile user = new UserProfile();
            IMemberService mService = Services.MemberService;


            user.UserVkId = member.HasProperty("vkid") ? member.GetValue("vkid").ToString() : "";
            user.UserProfileId = member.Id;
            user.LastIp = member.HasProperty("lastip") ? member.GetValue("lastip").ToString() : currentHttpRequest.UserHostAddress;

            member.SetValue("lastonline", DateTime.UtcNow);

            if (currentHttpRequest != null)
                member.SetValue("lastip", currentHttpRequest.UserHostAddress);
            
            mService.Save(member);

            return user;
        }

        
/*
        /// <summary>
        /// Вытаскивает из HttpRequest из формы данные и возвращает заполненный объект UserProfile
        /// </summary>
        /// <param name="req">HttpRequest</param>
        /// <returns>UserProfile</returns>
        UserProfile GetUserProfileFromAjaxForm(HttpRequest req)
        {
            NameValueCollection form = req.Form;
            UserProfile uProfile = new UserProfile();
            try
            {
                uProfile.Name = form.Get("first_name") != null ? form.GetValues("first_name")[0] : "не заполнено";
                uProfile.Surname = form.Get("last_name") != null ? form.GetValues("last_name")[0] : "не заполнено";
                uProfile.UserVkId = form.Get("id") != null ? "id" + form.GetValues("id")[0] : "не заполнено";

                // тут нужно сначала поиск этого города
                int tmpCityId =  form.Get("cityid") != null ? Common.StrToInt(form.GetValues("cityid")[0]) : -1;
                CityController cc = new CityController();
                uProfile.City = cc.GetCityFromBaseByCityVkId(tmpCityId) ?? cc.Add(tmpCityId, form.Get("city") != null ? form.GetValues("city")[0] : "Не указан") ;
                uProfile.CityId = uProfile.City.CityId;

                int year = form.GetValues("bdate")[0].Split('.').Length > 2 ? System.Int32.Parse(form.GetValues("bdate")[0].Split('.')[2]) : -1;
                int month = form.GetValues("bdate")[0].Split('.').Length > 1 ? System.Int32.Parse(form.GetValues("bdate")[0].Split('.')[1]) : -1;
                int day = form.GetValues("bdate")[0].Split('.').Length > 0 ? System.Int32.Parse(form.GetValues("bdate")[0].Split('.')[0]) : -1;

                System.DateTime birth = ((year >= 0) && (month >= 0) && (day >= 0)) ? new System.DateTime(year, month, day) : DateTime.MinValue;
                uProfile.Birth = (form.Get("bdate") != null) ? ((birth != null) ? birth : DateTime.MinValue) : DateTime.MinValue;
                uProfile.LastIp = req.UserHostAddress;
            }
            catch (Exception ex)
            {
                return new UserProfile(ex.Message + " (GetUserProfileFromAjaxForm)");
            }
            return uProfile;
        }
        */

        /// <summary>
        /// ВОзвращает объект UserProfile по id пользователя из базы и обновляет поля LastIP и LastOnline
        /// </summary>
        /// <param name="userName">id пользователя VK</param>
        /// <param name="currentHttpRequest"></param>
        /// <returns></returns>
        public UserProfile GetUserProfileByUsernameWithUpdateInfo(string userVkId, HttpRequest currentHttpRequest)
        {
            DefaultContext dc = new DefaultContext();

            UserProfile user = dc.UserProfiles.Where(x => x.UserVkId == userVkId).FirstOrDefault();

            if (user != null)
            {
                IMemberService mService = Services.MemberService;
                Umbraco.Core.Models.IMember member = mService.GetByUsername(userVkId);
                if (member != null)
                    user.LastOnline = UpdateLastIpAndOnline(mService, member, currentHttpRequest, dc, user);
                return user;
            }

            return null;
        }


        /// <summary>
        /// Обновление информации о последнем визите (время и IP)
        /// </summary>
        /// <param name="mService">Сервис участников</param>
        /// <param name="member">Участник</param>
        /// <param name="currentHttpRequest">Текущий запрос</param>
        /// <param name="dc">Контекст БД, в которой таблица с расширенной инфой о пользователях</param>
        /// <param name="user">Профиль пользователя</param>
        /// <returns></returns>
        public DateTime UpdateLastIpAndOnline(IMemberService mService, Umbraco.Core.Models.IMember member, HttpRequest currentHttpRequest, DefaultContext dc, UserProfile user)
        {
            DateTime now = DateTime.UtcNow;

            try
            {
                if (member != null)
                {
                    member.SetValue("lastonline", now);

                    if (currentHttpRequest != null)
                        member.SetValue("lastip", currentHttpRequest.UserHostAddress);
                }
                mService.Save(member);

                user.LastOnline = now;
                user.LastIp = currentHttpRequest.UserHostAddress;
                dc.Entry<UserProfile>(user).State = System.Data.Entity.EntityState.Modified;
                dc.SaveChanges();
            }
            catch
            {
                return DateTime.MinValue;
            }

            return now;
        }
    }
}
