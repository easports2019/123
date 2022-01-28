using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using osbackend.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Models;



namespace osbackend.Utils
{
    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, object> _expr { get; set; }
        public GenericCompare(Func<T, object> expr)
        {
            this._expr = expr;
        }
        public bool Equals(T x, T y)
        {
            var first = _expr.Invoke(x);
            var sec = _expr.Invoke(y);
            if (first != null && first.Equals(sec))
                return true;
            else
                return false;
        }
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    public class Common
    {
        
        /// <summary>
        /// Путь к картинке с изображением "нет картинки"
        /// </summary>
        public const string PlaceNoPhoto = @"~/data/img/common/noimage.jpg";

        /// <summary>
        /// Количество возвращаемых объектов из API-функции по умолчанию
        /// </summary>
        public const int RowsToReturnByTransaction = 10;

        public class UserCheckResult
        {
            public string UserVKId { get; set; }
            public DateTime RequestDateTime { get; set; }

            public UserCheckResult(string userVkId, DateTime requestDateTime)
            {
                UserVKId = userVkId;
                RequestDateTime = requestDateTime;
            }
        }

        /// <summary>
        /// проверка запроса, от реально ли пользователя он пришел
        /// </summary>
        /// <param name="queryString">строка с параметрами запроса</param>
        /// <returns>Возвращает  UserCheckResult, где UserVKId=id пользователя вконтакте, для которого актуальна строка, а RequestDateTime актуальное время запроса (равно DateTime.Min, если не прошло в диапазон +-1 минута от запроса)</returns>
        public static UserCheckResult CheckSignAndGetUserAndRequestDate(System.Collections.Specialized.NameValueCollection queryString)
        {

            string clientSecret = System.Configuration.ConfigurationManager.AppSettings["VKMiniAppKey"];
            List<string> queryParams = new List<string>();
            string stringWithParams = "";
            string sign = (queryString != null) ? queryString.Get("sign") : "no-sign-from-client";

            // получить и отсортировать массив по ключам
            var qPSorted = GetParametersFromVkQueryString(queryString, true).OrderBy(x => x.Key);
            int qPSortedLength = qPSorted.Count();

            // сформировать строку вида ключ=значение из отсортированных ключей "vk_" со значениями
            foreach (var item in qPSorted)
            {
                qPSortedLength--;
                stringWithParams += item.Key + "=" + item.Value;
                if (qPSortedLength >= 1)
                    stringWithParams += "&";

            }

            // внести проверку на текущего пользователя и на время генерации строки (сравнить с текущей, чтобы разница была не более нескольких секунд)
            string resUser = ((queryString != null) && (queryString.Get("vk_user_id") != null)) ? queryString.Get("vk_user_id") : "";
            DateTime resDateTime = GetDateTimeFromUTCJsString(queryString.Get("vk_ts"));
            if ((DateTime.UtcNow.AddMinutes(-1.0) > resDateTime) || (DateTime.UtcNow.AddMinutes(1.0) < resDateTime))
                resDateTime = DateTime.MinValue;

            stringWithParams = System.Text.RegularExpressions.Regex.Replace(stringWithParams, ",", "%2C");
            string hash = Convert.ToBase64String(HMACHASH(stringWithParams, clientSecret));


            hash = System.Text.RegularExpressions.Regex.Replace(hash, @"=$", "");
            hash = System.Text.RegularExpressions.Regex.Replace(hash, @"\+", "-");
            hash = System.Text.RegularExpressions.Regex.Replace(hash, @"\/", "_");

            
            return (sign == "no-sign-from-client") ? null : (hash == sign) ? new UserCheckResult(resUser, resDateTime) : null;
        }

        /// <summary>
        /// Пишет в лог текст, переданный в параметре. При удаче возвращает null, В случае ошибки возвращает строку с текстом ошибки
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string WriteToLog(string text)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(HttpContext.Current.Server.MapPath("~/bin/log.txt"));
                sw.WriteLine(text);
                sw.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static DateTime GetDateTimeFromUTCJsString(string jsDateTime)
        {
            long tempTicks = long.TryParse(jsDateTime + "000", out tempTicks) ? long.Parse(jsDateTime + "000") : 0;
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(tempTicks);
        }

        public static Dictionary<string, string> GetParametersFromVkQueryString(System.Collections.Specialized.NameValueCollection queryString, bool onlyVkParameters)
        {
            Dictionary<string, string> qParams = new Dictionary<string, string>();
            
            // получить все параметры из URL
            // processing...
            foreach (string key in queryString.AllKeys)
            {
                // отобрать параметры, которые начинаются с "vk_" 
                // processing...
                if (onlyVkParameters)
                {
                    if (key.StartsWith("vk_"))
                        qParams.Add(key, queryString.Get(key));
                }
                else
                {
                    qParams.Add(key, queryString.Get(key));
                }
            }

            return qParams;
        }


        public static byte[] HMACHASH(string str, string key)
        {
            byte[] bkey = Encoding.Default.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(bkey))
            {
                byte[] bstr = Encoding.Default.GetBytes(str);
                return hmac.ComputeHash(bstr);
            }
        }


        /// <summary>
        /// Возвращает число из строки с предварительной проверкой на ошибку (TryParse), если неверное преобразование, возвращает -1
        /// </summary>
        /// <param name="value">Строка string</param>
        /// <returns>Возвращает число или -1 в случае ошибки</returns>
        public static int StrToInt(string value)
        {
            int tempNumber = -1;

            if (Int32.TryParse(value, out tempNumber))
            {
                return tempNumber;
            }

            return tempNumber;
        }



        /// <summary>
        /// Считает сколько минут между двумя значениями времени (без учета даты)
        /// </summary>
        /// <param name="dateTime1">дата+время 1 (отсюда вычитаем)</param>
        /// <param name="dateTime2">дата+время 2 (это вычитаем)</param>
        /// <returns>количество минут в double</returns>
        public static double TimeRangeInMinutes(DateTime dateTime1, DateTime dateTime2)
        {
            return (dateTime1.TimeOfDay - dateTime2.TimeOfDay).TotalMinutes;
            //return (dateTime1 - dateTime2).TotalMinutes;
        }


        /// <summary>
        /// Возвращает дату без времени
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetDate(DateTime dateTime)
        {
            return dateTime.Date;
            //return (dateTime1 - dateTime2).TotalMinutes;
        }


        /// <summary>
        /// Возвращает дату из строки с предварительной проверкой на ошибку (TryParse), если неверное преобразование, возвращает DateTime.MinValue
        /// </summary>
        /// <param name="value">Строка string</param>
        /// <returns></returns>
        public static DateTime StrToDateTime(string value)
        {
            DateTime tempDT = DateTime.MinValue;

            if (DateTime.TryParse(value, out tempDT))
            {
                return tempDT;
            }

            return tempDT;
        }

        /// <summary>
        /// Возвращает Boolean из строки с предварительной проверкой на ошибку (TryParse), если неверное преобразование, возвращает false
        /// </summary>
        /// <param name="value">Строка string</param>
        /// <returns></returns>
        public static bool StrToBool(string value)
        {
            bool tempBool = false;

            if (Boolean.TryParse(value, out tempBool))
            {
                return tempBool;
            }

            return tempBool;
        }


        /// <summary>
        /// Создание в базе профиля пользователя
        /// </summary>
        /// <param name="uProfile">заполненный UserProfile</param>
        /// <returns>True - если добавлено успешно, False в случае ошибки</returns>
        public UserProfile CreateUserProfile(UserProfile uProfile)
        {
            /*
             создание пользователя инициируется сервером. Поэтому где-то мы принимаем команду на создание, делаем какие-то действия и вызываем создание профиля. Предварительно
             необходимо проверить, не существует ли уже такой пользователь (по Id Vk например)
             */
            UserProfile usProfile = new UserProfile();

            try
            {
                DefaultContext dc = new DefaultContext();
                dc.Entry<UserProfile>(uProfile).State = System.Data.Entity.EntityState.Added;
                //dc.UserProfiles.Add(uProfile);
                if (dc.SaveChanges() > 0)
                {
                    if (uProfile.UserProfileId >= 0)
                        return uProfile;
                    //return dc.UserProfiles.ToList().Last();
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                return new UserProfile(ex.InnerException.Message + " (CreateUserProfile)" + 
                    uProfile.AmpluaId + "; " +
                    uProfile.Birth.ToString() + "; " +
                    uProfile.CityUmbracoId + "; " +
                    uProfile.CityUmbracoName + "; " +
                    uProfile.CityVkId + "; " +
                    uProfile.CityName + "; " +
                    uProfile.Fathername + "; " +
                    uProfile.Height.ToString() + "; " +
                    uProfile.LastIp + "; " +
                    uProfile.LastOnline.ToString() + "; " +
                    uProfile.LegId + "; " +
                    uProfile.Name + "; " +
                    uProfile.Register.ToString() + "; " +
                    uProfile.Surname + "; " +
                    uProfile.UserProfileId + "; " +
                    uProfile.UserVkId + "; " +
                    uProfile.Weight.ToString()
                    );
            }

            return null;
        }


        /// <summary>
        ///  возвращает записи администрирования из таблицы админов
        /// </summary>
        /// <param name="userProfileId">Id пользователя</param>
        /// <returns></returns>
        public static List<Admin> GetUserProfileAdminList(int userProfileId)
        {
            List<Admin> adminGroups = null;
            using (DefaultContext dc = new DefaultContext())
            {
                try
                {
                    adminGroups = dc.Admins.Where(x => x.UserProfileId == userProfileId).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return adminGroups;
        }

        /// <summary>
        /// Проверка доступности управления турниром
        /// </summary>
        /// <param name="uProfile">заполненный UserProfile</param>
        /// <param name="tournament">турнир, который надо проверить</param>
        /// <returns>True - если имеет доступ, False - не имеет</returns>
        public bool UserProfileHasAccessToTournament(UserProfile uProfile, Tournament tournament)
        {
            try
            {
                DefaultContext dc = new DefaultContext();
                // производим выборку таблицы админов турниров, фильтруем по Id турнира и по опубликованности админа, соединяем с таблицей профилей пользователей
                // по ключу Id профиля, проверяем есть ли такой профиль в списке по переданному в параметре профилю
                //bool contains = dc.Admins
                //    .Where(x => x.TournamentId == tournament.Id)
                //    .Where(x => x.Published)
                //    .Join(dc.UserProfiles, a => a.UserProfileId, u => u.UserProfileId, (adm, user) => user)
                //    .Contains(uProfile);

                var list = dc.Admins.Where(x => ((x.TournamentId == tournament.Id) && (x.AdminTypeId == 2)) 
                || (x.AdminTypeId == 1)) // добавляем полномочия системному админу на все турниры
                    .Where(x => x.Published)
                    .Where(x => !x.Deleted)
                    .Join(dc.UserProfiles, a => a.UserProfileId, u => u.UserProfileId, (adm, user) => user);

                foreach(UserProfile up in list)
                {
                    if (up.UserProfileId == uProfile.UserProfileId)
                        return true;
                }
                //if (list.Contains(uProfile, new GenericCompare<UserProfile>(x => x.UserProfileId)))
                //    return true;

                return false;

            }
            catch (Exception ex)
            {
                
            }

            return false;
        }


        /// <summary>
        /// Проверка доступности управления командой
        /// </summary>
        /// <param name="uProfile">заполненный UserProfile</param>
        /// <param name="team">команда, которую надо проверить</param>
        /// <returns>True - если имеет доступ, False - не имеет</returns>
        public bool UserProfileHasAccessToTeam(UserProfile uProfile, Team team)
        {
            try
            {
                DefaultContext dc = new DefaultContext();
                // производим выборку таблицы админов турниров, фильтруем по Id турнира и по опубликованности админа, соединяем с таблицей профилей пользователей
                // по ключу Id профиля, проверяем есть ли такой профиль в списке по переданному в параметре профилю
                //bool contains = dc.Admins
                //    .Where(x => x.TournamentId == tournament.Id)
                //    .Where(x => x.Published)
                //    .Join(dc.UserProfiles, a => a.UserProfileId, u => u.UserProfileId, (adm, user) => user)
                //    .Contains(uProfile);

                var list = dc.Admins.Where(x => ((x.TeamId == team.Id) && (x.AdminTypeId == 3)) 
                || (x.AdminTypeId == 1)) // добавляем полномочия системному админу на все команды
                    .Where(x => x.Published)
                    .Where(x => !x.Deleted)
                    .Join(dc.UserProfiles, a => a.UserProfileId, u => u.UserProfileId, (adm, user) => user);

                foreach(UserProfile up in list)
                {
                    if (up.UserProfileId == uProfile.UserProfileId)
                        return true;
                }
                //if (list.Contains(uProfile, new GenericCompare<UserProfile>(x => x.UserProfileId)))
                //    return true;

                return false;

            }
            catch (Exception ex)
            {
                
            }

            return false;
        }


        /// <summary>
        /// Проверка доступности управления сбором
        /// </summary>
        /// <param name="uProfile">заполненный UserProfile</param>
        /// <param name="tournament">простой сбор, который надо проверить</param>
        /// <returns>True - если имеет доступ, False - не имеет</returns>
        public bool UserProfileHasAccessToSimpleCollect(UserProfile uProfile, SimpleCollect simpleCollect)
        {
            try
            {
                if (uProfile.UserProfileId == simpleCollect.CreatorId)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// Создание в базе профиля пользователя
        /// </summary>
        /// <param name="uProfile">заполненный UserProfile</param>
        /// <returns>True - если добавлено успешно, False в случае ошибки</returns>
        public UserProfile UpdateUserProfile(UserProfile uProfile, DateTime newBirthDate, string lastIP)
        {
            /*
             создание пользователя инициируется сервером. Поэтому где-то мы принимаем команду на создание, делаем какие-то действия и вызываем создание профиля. Предварительно
             необходимо проверить, не существует ли уже такой пользователь (по Id Vk например)
             */

            try
            {
                DefaultContext dc = new DefaultContext();
                dc.Entry<UserProfile>(uProfile).State = System.Data.Entity.EntityState.Modified;
                uProfile.Birth = newBirthDate;
                uProfile.LastOnline = DateTime.UtcNow;
                uProfile.LastIp = lastIP;

                if (dc.SaveChanges() > 0)
                {
                    if (uProfile.UserProfileId >= 0)
                        return uProfile;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return new UserProfile(ex.InnerException.Message + " (CreateUserProfile)" +
                    uProfile.AmpluaId + "; " +
                    uProfile.Birth.ToString() + "; " +
                    uProfile.CityUmbracoId + "; " +
                    uProfile.CityUmbracoName + "; " +
                    uProfile.CityVkId + "; " +
                    uProfile.CityName + "; " +
                    uProfile.Fathername + "; " +
                    uProfile.Height.ToString() + "; " +
                    uProfile.LastIp + "; " +
                    uProfile.LastOnline.ToString() + "; " +
                    uProfile.LegId + "; " +
                    uProfile.Name + "; " +
                    uProfile.Register.ToString() + "; " +
                    uProfile.Surname + "; " +
                    uProfile.UserProfileId + "; " +
                    uProfile.UserVkId + "; " +
                    uProfile.Weight.ToString()
                    );
            }

            return null;
        }
    }

}



