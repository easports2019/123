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


        [HttpPost]
        public UserProfile Auth(/*VKUSerData userData*/)
        {
            if (Common.CheckSignAndGetUserAndRequestDate(httpRequest.QueryString) != null)
            {

                Common.UserCheckResult result = Common.CheckSignAndGetUserAndRequestDate(httpRequest.QueryString);
                var vkParameters = Common.GetParametersFromVkQueryString(httpRequest.QueryString, true);

                
                IMemberService mService = Services.MemberService;
                string userLogin = "id" + vkParameters["vk_user_id"];
                Umbraco.Core.Models.IMember memb = mService.GetByUsername(userLogin); 


                if (memb == null) // регистрация нового аккаунта
                {
                    UserProfile uProfile = new UserProfile();

                    // block AC0001
                    try
                    {
                        // получение данных от клиента
                        uProfile = GetUserProfileFromAjaxForm(httpRequest);
                        if ((uProfile == null) || (uProfile.ErrorMessage.Length > 0))
                            return new UserProfile("not registred because of error in AC0001 GetUserProfileFromAjaxForm: " + uProfile.ErrorMessage);

                        // регистрация аккаунта на бэке
                        uProfile = RegisterAccount(uProfile);
                        if ((uProfile == null) || (uProfile.ErrorMessage.Length > 0))
                            return new UserProfile("not registred because of error in AC0001 RegisterAccount: " + uProfile.ErrorMessage);

                        // поиск расширенной информации о пользователе
                        UserProfile tmpUP = new UserProfileController().GeUserProfileByVkId(uProfile.UserVkId);

                        if (tmpUP != null) // если данные найдены
                        {
                            // загружаем их из базы
                            uProfile = tmpUP;
                        }
                        else // если данные не найдены
                        {
                            // заполнение расширенных полей аккаунта по умолчанию.

                            // Здесь нужно предусмотреть добавление нового города в базу умбрако, если город прилетел из ВК
                            uProfile = SetDefaultsToAccount(uProfile);

                            // создание расширенной информации об аккаунте в доп.базе
                            uProfile = new Common().CreateUserProfile(uProfile);
                            if ((uProfile == null) || (uProfile.ErrorMessage.Length > 0))
                                return new UserProfile("registred but not added to advanced base (CreateUserProfile): " + uProfile.ErrorMessage);

                        }

                        return uProfile;

                    }
                    catch (Exception ex)
                    {
                        return new UserProfile("not registred because of error in AC0001: " + ex.Message);
                    }
                }
                else // автоизован. загрузка аккаунта из базы
                {
                    UserProfile uProfile = GetUserProfileFromAjaxForm(httpRequest);
                    UserProfile uProfileFromBase = GetUserProfileByUsernameWithUpdateInfo(userLogin, httpRequest);

                    if (uProfileFromBase != null)
                    {
                        if (uProfileFromBase.Birth == uProfile.Birth)
                            return uProfileFromBase;
                        else
                        {
                            
                            return new Common().UpdateUserProfile(uProfileFromBase, uProfile);
                        }
                    }
                    else
                    {

                        uProfileFromBase = GetUserProfileFromAjaxForm(httpRequest);
                        // заполнение расширенных полей аккаунта по умолчанию.
                        uProfileFromBase = SetDefaultsToAccount(uProfileFromBase);
                        return new Common().CreateUserProfile(uProfileFromBase);
                    }
                }
                
            }

            return new UserProfile("Авторизация не пройдена");
        }


        [HttpPost]
        public UserProfile Auth2(/*VKUSerData userData*/)
        {
            Common.UserCheckResult result = Common.CheckSignAndGetUserAndRequestDate(httpRequest.QueryString);


            if (result != null)
            {
                var vkParameters = Common.GetParametersFromVkQueryString(httpRequest.QueryString, true);




                IMemberService mService = Services.MemberService;
                string userLogin = "id" + vkParameters["vk_user_id"];
                Umbraco.Core.Models.IMember memb = mService.GetByUsername(userLogin);


                if (memb == null) // регистрация нового аккаунта
                {
                    UserProfile uProfile = new UserProfile();

                    // block AC0001
                    try
                    {
                        // получение данных от клиента
                        uProfile = GetUserProfileFromAjaxForm(httpRequest);
                        if ((uProfile == null) || (uProfile.ErrorMessage.Length > 0))
                            return new UserProfile("not registred because of error in AC0001 GetUserProfileFromAjaxForm: " + uProfile.ErrorMessage);

                        // регистрация аккаунта на бэке
                        uProfile = RegisterAccount(uProfile);
                        if ((uProfile == null) || (uProfile.ErrorMessage.Length > 0))
                            return new UserProfile("not registred because of error in AC0001 RegisterAccount: " + uProfile.ErrorMessage);

                        // поиск расширенной информации о пользователе
                        UserProfile tmpUP = new UserProfileController().GeUserProfileByVkId(uProfile.UserVkId);

                        if (tmpUP != null) // если данные найдены
                        {
                            // загружаем их из базы
                            uProfile = tmpUP;
                        }
                        else // если данные не найдены
                        {
                            // заполнение расширенных полей аккаунта по умолчанию.

                            // Здесь нужно предусмотреть добавление нового города в базу умбрако, если город прилетел из ВК
                            uProfile = SetDefaultsToAccount(uProfile);

                            // создание расширенной информации об аккаунте в доп.базе
                            uProfile = new Common().CreateUserProfile(uProfile);
                            if ((uProfile == null) || (uProfile.ErrorMessage.Length > 0))
                                return new UserProfile("registred but not added to advanced base (CreateUserProfile): " + uProfile.ErrorMessage);

                        }

                        return uProfile;

                    }
                    catch (Exception ex)
                    {
                        return new UserProfile("not registred because of error in AC0001: " + ex.Message);
                    }
                }
                else // автоизован. загрузка аккаунта из базы
                {
                    UserProfile uProfile = GetUserProfileFromAjaxForm(httpRequest);
                    UserProfile uProfileFromBase = GetUserProfileByUsernameWithUpdateInfo(userLogin, httpRequest);

                    if (uProfileFromBase != null)
                    {
                        if (uProfileFromBase.Birth == uProfile.Birth)
                            return uProfileFromBase;
                        else
                        {

                            return new Common().UpdateUserProfile(uProfileFromBase, uProfile);
                        }
                    }
                    else
                    {

                        uProfileFromBase = GetUserProfileFromAjaxForm(httpRequest);
                        // заполнение расширенных полей аккаунта по умолчанию.
                        uProfileFromBase = SetDefaultsToAccount(uProfileFromBase);
                        return new Common().CreateUserProfile(uProfileFromBase);
                    }
                }

            }

            return new UserProfile("Авторизация не пройдена");
        }


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
