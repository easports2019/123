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
    public class SimpeAdminController : UmbracoApiController
    {
        DefaultContext dc = new DefaultContext();
        HttpRequest currentRequest = HttpContext.Current.Request;

        /// <summary>
        /// Добавляет в базу админа и его же возвращает в случае успеха. Если админ с таким профилем уже существует, возвращается он
        /// </summary>
        /// <param name="admin">Данные админа</param>
        /// <returns></returns>
        public Admin Add(Admin admin)
        {
            if (admin == null)
                return new Admin("Error in AdminAdd: no input data");

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                try
                {
                    Admin adminTmp = new Admin();
                    
                    if (dc.Admins.Any())
                    {
                        if (dc.Admins
                            .Where(x => x.TeamId == admin.TeamId)
                            .Where(x => x.TournamentId == admin.TournamentId)
                            .FirstOrDefault()
                            == null)
                        {
                            dc.Admins.Add(adminTmp);
                            dc.SaveChanges();
                        }
                        else
                        {
                            return dc.Admins
                            .Where(x => x.TeamId == admin.TeamId)
                            .Where(x => x.TournamentId == admin.TournamentId)
                            .FirstOrDefault();
                        }
                    }
                    else
                    {
                        dc.Admins.Add(adminTmp);
                        dc.SaveChanges();
                    }

                    return adminTmp;
                }
                catch (Exception ex)
                {
                    return new Admin("Error in AdminAdd: " + ex.Message);
                }
            }
            else
            {
                return new Admin("Error in AdminAdd: not authorized");
            }

        }

        /// <summary>
        /// Отправка (публикация) заявки
        /// </summary>
        /// <param name="adminId">id заявки</param>
        /// <param name="publish">пометить как отправленную/опубликованную (да/нет)</param>
        /// <returns></returns>
        public Admin Publish(int adminId, bool publish)
        {
            Admin t = new Admin("Error in AdminPublish: No Admins with that Id");

            if (adminId < 0)
                return t;

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                if (dc.Admins.Any())
                {
                    Admin tt = dc.Admins.FirstOrDefault(x => x.Id == adminId);
                    t = new Admin(tt);
                }

                try
                {
                    if (t != null)
                    {
                        t.Published = publish;
                        dc.Entry<Admin>(t).State = System.Data.Entity.EntityState.Modified;
                        dc.SaveChanges();
                    }
                    else
                    {
                        return new Admin("Error in AdminPublish: No Admin with that Id");
                    }
                }
                catch (Exception ex)
                {
                    return new Admin("Error in AdminPublish: " + ex.Message);
                }

                return t;
            }
            else
            {
                return new Admin("Error in AdminPublish: not authorized");
            }

        }


        /// <summary>
        /// Удаление заявки на добавление в турнир (пометка)
        /// </summary>
        /// <param name="adminId">id заявки</param>
        /// <param name="delete">Пометить как удаленную (да/нет)</param>
        /// <returns></returns>
        public Admin Delete(int adminId, bool delete)
        {
            Admin t = new Admin("Error in AdminDelete: No Admins with that Id");

            if (adminId < 0)
                return t;

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                if (dc.Admins.Any())
                {
                    Admin tt = dc.Admins.FirstOrDefault(x => x.Id == adminId);
                    t = new Admin(tt);
                }

                try
                {
                    if (t != null)
                    {
                        t.Deleted = delete;
                        dc.Entry<Admin>(t).State = System.Data.Entity.EntityState.Modified;
                        dc.SaveChanges();
                    }
                    else
                    {
                        return new Admin("Error in AdminDelete: No Admin with that Id");
                    }
                }
                catch (Exception ex)
                {
                    return new Admin("Error in AdminDelete: " + ex.Message);
                }

                return t;
            }
            else
            {
                return new Admin("Error in AdminDelete: not authorized");
            }

        }

        /// <summary>
        /// Возвращает все заявки
        /// </summary>
        /// <returns></returns>
        public List<Admin> GetAll()
        {
            List<Admin> admins = new List<Admin>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {
                    admins = (dc.Admins
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.Admins
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return admins;
        }

        /// <summary>
        /// Возвращает заявки от команд указанного турнира
        /// </summary>
        /// <param name="tournamentGroupId">Id турнира</param>
        /// <returns></returns>
        public List<Admin> GetAllInTournament(int tournamentId)
        {
            List<Admin> admins = new List<Admin>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

                using (DefaultContext dc = new DefaultContext())
                {

                    admins = (dc.Admins
                        .Where(x => x.TournamentId == tournamentId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.Admins
                        .Where(x => x.TournamentId == tournamentId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;

                }
            }

            return admins;
        }

        /// <summary>
        /// Возвращает идущие турниры в выбранном городе
        /// </summary>
        /// <param name="cityId">ID города</param>
        /// <returns></returns>
        public List<Admin> GetAllInCity(int cityId)
        {
            List<Admin> admins = new List<Admin>();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;


                using (DefaultContext dc = new DefaultContext())
                {
                    List<Tournament> cityTournaments = dc.Tournaments.Where(x => x.CityId == cityId).ToList();

                    // турнир относится к городу. заявка относится к турниру. мне нужно выборку заявок по указанному городу.

                    admins = (dc.Admins
                        .Where(x => x.Tournament.CityId == cityId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Count() > startIndex)
                        ?
                        dc.Admins
                        .Where(x => x.Tournament.CityId == cityId)
                        .Where(x => x.Published)
                        .Where(x => !x.Deleted)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }

            return admins;
        }


        /// <summary>
        /// Возвращает заявка команды на турнир по ее Id
        /// </summary>
        /// <param name="adminId">Id заявки</param>
        /// <returns></returns>
        public Admin GetById(int adminId)
        {
            Admin admin = new Admin();

            if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
            {
                using (DefaultContext dc = new DefaultContext())
                {
                    admin = (dc.Admins.FirstOrDefault(x => x.Id == adminId) != null)
                        ?
                        dc.Admins.FirstOrDefault(x => x.Id == adminId)
                        :
                        new Admin("Error in AdminGetById: No Admins with that Id");
                }
            }

            return admin;
        }
    }
}
