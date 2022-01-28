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




/*
 План:
 1. описание данных (сделано) =2д
 2. написать контроллеры:
 - места: getAll, get =2д
 - сборы: getAll, get, add, update, согласиться участвовать, отказаться участвовать, попроситься в сбор, принять попросившегося в сбор, удалить участника из сбора, =7д
 выполнение задач по таймеру (оповещение о сборе, установка начала сбора, закрытие сбора) =7д
 - профили: get, getAll, updateMy, голосование, регистрация (добавление), удаление (пометка) =5д
 - амплуа: get, getAll =1д
 - системные настройки: get, set =2д
 - оплаты: создание оплаты к сбору, обновление оплаты к сбору  =4д
 - тип доступа: get, getAll  =1д
 - опции: get, getAll   =1д
 - раздевалки: get, getAll  =1д
 - площади: get, getAll =1д
 - города: get, getAll =1д
 - валдельцы: get, getAll  =1д
 - временные промежутки: get, getAll, set, update (?) =2д
 - медиа-данные, фото, видео: get, set, getAll, delete  =3д
 3. научиться отправлять оповещения ВК  =5д
 4. научиться взаимодействовать с ВК  =3д
 5. сделать axios API в клиенте  =7д
 6. завязать интерфейс на работу с получением данных от сервера и ВК  =4д
 7. добписать в клиенте: работу с профилем, редактирование сбора, работу с набором игроков, страницу фиксации оплат по сбору,   =7д
 8. наполнение данными системы.  =4д
     
     */

[EnableCors(origins: "*", headers: "*", methods: "*")]
public class CollectsController : UmbracoApiController
{
    HttpRequest currentRequest = HttpContext.Current.Request;

    [HttpOptions]
    public dynamic Add()
    {
        return Ok();
    }

   [HttpOptions]
    public dynamic Add2()
    {
        return Ok();
    }
   

    [HttpPost]
    public Collect Add(Collect collect)
    {

        //Collect collect = null;


        if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
        {
            using (DefaultContext dc = new DefaultContext())
            {

                // создаем два объекта User
                //collect = new Collect { /*Name = "Кострома", GeoPosition = "55;45"*/ };
                //collect = GetCollectFromAjaxForm(currentRequest);

                // добавляем их в бд
                dc.Collects.Add(collect);
                dc.SaveChanges();
                //Console.WriteLine("Объекты успешно сохранены");

            }

        }
        return collect;
    }

    [HttpPost]
    public Collect Add2(object collect2)
    {

        Collect collect = Newtonsoft.Json.JsonConvert.DeserializeObject<Collect>(collect2.ToString());


        if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
        {
            using (DefaultContext dc = new DefaultContext())
            {

                // создаем два объекта User
                //collect = new Collect { /*Name = "Кострома", GeoPosition = "55;45"*/ };
                //collect = GetCollectFromAjaxForm(currentRequest);

                // добавляем их в бд
                dc.Collects.Add(collect);
                dc.SaveChanges();
                //Console.WriteLine("Объекты успешно сохранены");

            }

        }
        return collect;
    }

    /// <summary>
    /// Вытаскивает из HttpRequest из формы данные и возвращает заполненный объект Collect
    /// </summary>
    /// <param name="req">HttpRequest</param>
    /// <returns>Collect</returns>
    Collect GetCollectFromForm(HttpRequest req)
    {
        NameValueCollection form = req.Form;
        Collect collect = new Collect();
        try
        {
            collect.AcceptedByPlaceOwner = true; // пока делаем по умолчанию True, т.к. некому разрешать
            collect.DurationMinutes = form.Get("durationMinutes") != null ? Common.StrToInt(form.GetValues("durationMinutes")[0]) : 0;
            collect.WhenDate = form.Get("date") != null ? Common.StrToDateTime(form.GetValues("date")[0]) : DateTime.MinValue;
            collect.FixedPriceByMember = form.Get("fixedByMemberPrice") != null ? Common.StrToInt(form.GetValues("fixedByMemberPrice")[0]) : 0;
            collect.Price = form.Get("price") != null ? Common.StrToInt(form.GetValues("price")[0]) : 0; 
            collect.Hour = form.Get("hour") != null ? Common.StrToInt(form.GetValues("hour")[0]) : 0;
            collect.Minute = form.Get("minute") != null ? Common.StrToInt(form.GetValues("minute")[0]) : 0;
            collect.OrganizatorIsMember = form.Get("organizatorIsMember") != null ? Common.StrToBool(form.GetValues("organizatorIsMember")[0]) : true;
            collect.Options = null;// form.Get("minute") != null ? Common.StrToInt(form.GetValues("minute")[0]) : 0;
            collect.Access = AccessEnum.Public;
            collect.Organizer = null; 
            collect.Permanent = false; 
            collect.Place = null; 
            collect.UsersInvited = null; 
            collect.UsersWantsToParticipate = null; 
            collect.MemberGroups = null; //form.Get("price") != null ? Common.StrToInt(form.GetValues("price")[0]) : 0;

            //uProfile.Name = form.Get("first_name") != null ? form.GetValues("first_name")[0] : "не заполнено";
            //uProfile.Surname = form.Get("last_name") != null ? form.GetValues("last_name")[0] : "не заполнено";
            //uProfile.UserVkId = form.Get("id") != null ? "id" + form.GetValues("id")[0] : "не заполнено";

            //// тут нужно сначала поиск этого города
            //int tmpCityId = form.Get("cityid") != null ? Common.StrToInt(form.GetValues("cityid")[0]) : -1;
            //CityController cc = new CityController();
            //uProfile.City = cc.GetCityFromBaseByCityVkId(tmpCityId) ?? cc.Add(tmpCityId, form.Get("city") != null ? form.GetValues("city")[0] : "Не указан");
            //uProfile.CityId = uProfile.City.CityId;

            //int year = form.GetValues("bdate")[0].Split('.').Length > 2 ? System.Int32.Parse(form.GetValues("bdate")[0].Split('.')[2]) : -1;
            //int month = form.GetValues("bdate")[0].Split('.').Length > 1 ? System.Int32.Parse(form.GetValues("bdate")[0].Split('.')[1]) : -1;
            //int day = form.GetValues("bdate")[0].Split('.').Length > 0 ? System.Int32.Parse(form.GetValues("bdate")[0].Split('.')[0]) : -1;

            //System.DateTime birth = ((year >= 0) && (month >= 0) && (day >= 0)) ? new System.DateTime(year, month, day) : DateTime.MinValue;
            //uProfile.Birth = (form.Get("bdate") != null) ? ((birth != null) ? birth : DateTime.MinValue) : DateTime.MinValue;
            //uProfile.LastIp = req.UserHostAddress;
        }
        catch (Exception ex)
        {
            return new Collect(ex.Message + " (GetCollectFromAjaxForm)");
        }
        return collect;
    }


    public string Cancel()
    {
        return "";
    }

    public string Edit()
    {
        return "";
    }

    public string Get()
    {
        return "";
    }

    [HttpPost]
    public List<Collect> GetAll()
    {
        List<Collect> collects = new List<Collect>();

        if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
        {
            int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;

            using (DefaultContext dc = new DefaultContext())
            {
                collects = (dc.Collects
                    .Where(x => x.WhenDate >= DateTime.UtcNow).Count() > startIndex)
                    ? 
                    dc.Collects
                    .Where(x => x.WhenDate >= DateTime.UtcNow)
                    .Skip(startIndex)
                    .Take(Common.RowsToReturnByTransaction).ToList() 
                    : null;
            }
        }

        return collects;
    }

    [HttpPost]
    public List<Collect> GetAllInPlace()
    {
        List<Collect> collects = new List<Collect>();

        if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
        {

            int placeId = (currentRequest.Form.Get("placeid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("placeid")[0]) : -1;
            int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;


            if (placeId != -1)
            {
                // тут надо доставать не из дефолтной базы, а из бэкэнда. там все места будут занесены. (обращение к базе Umbraco)
                using (DefaultContext dc = new DefaultContext())
                {
                    collects = (dc.Collects
                        .Where(x => x.WhenDate >= DateTime.UtcNow)
                        .Where(y => y.Place.PlaceId == placeId).Count() > startIndex)
                        ?
                        dc.Collects
                        .Where(x => x.WhenDate >= DateTime.UtcNow)
                        .Where(y => y.Place.PlaceId == placeId)
                        .Skip(startIndex)
                        .Take(Common.RowsToReturnByTransaction).ToList()
                        : null;
                }
            }
        }

        return collects;
    }

    [HttpPost]
    public List<Collect> GetAllInCityByCityId()
    {
        List<Collect> collects = new List<Collect>();


        if (Common.CheckSignAndGetUserAndRequestDate(currentRequest.QueryString) != null)
        {

            int startIndex = (currentRequest.Form.Get("startindex") != null) ? Common.StrToInt(currentRequest.Form.GetValues("startindex")[0]) : 0;
            int cityId = (currentRequest.Form.Get("cityid") != null) ? Common.StrToInt(currentRequest.Form.GetValues("cityid")[0]) : -1;

            if (cityId != -1)
            {
                // тут надо доставать не из дефолтной базы, а из бэкэнда. там все места будут занесены. (обращение к базе Umbraco)
                using (DefaultContext dc = new DefaultContext())
                {
                    // ищем в базе умбрако город с указанным id и вытаскиваем оттуда его id в umbraco
                    int cityUmbracoId = (cityId >= 0) ? 
                        (
                            dc.Citys.FirstOrDefault(x => x.CityId == cityId) != null ?
                            (
                                dc.Citys.FirstOrDefault(x => x.CityId == cityId).CityUmbracoId.HasValue ?
                                    dc.Citys.FirstOrDefault(x => x.CityId == cityId).CityUmbracoId.Value 
                                : -1
                            )
                            : -1
                        ) 
                        : -1;

                    if (cityUmbracoId >= 0)
                    {
                        collects = (dc.Collects
                            .Where(x => x.WhenDate >= DateTime.UtcNow)
                            .Where(y => y.Place.City.CityUmbracoId == cityUmbracoId).Count() > startIndex)
                            ?
                            dc.Collects
                            .Where(x => x.WhenDate >= DateTime.UtcNow)
                            .Where(y => y.Place.City.CityUmbracoId == cityUmbracoId)
                            .Skip(startIndex)
                            .Take(Common.RowsToReturnByTransaction).ToList()
                            : null;
                    }
                    else
                        collects = new List<Collect>();
                }
            }
        }

        return collects;
    }



    [HttpPost]
    public bool CheckIdentity()
    {
        return Common.CheckSignAndGetUserAndRequestDate(HttpContext.Current.Request.QueryString).RequestDateTime != DateTime.MinValue;

    }





    [HttpPost]
    public string MakeAnything()
    {
        string result = "";

        using (DefaultContext dc = new DefaultContext())
        {

            // создаем два объекта User
            Collect collect1 = new Collect { /*Name = "Кострома", GeoPosition = "55;45"*/ };


            // добавляем их в бд
            dc.Collects.Add(collect1);
            dc.SaveChanges();
            //Console.WriteLine("Объекты успешно сохранены");

            // получаем объекты из бд и выводим на консоль
            var collects = dc.Collects;
            //Console.WriteLine("Список объектов:");
            foreach (Collect c in collects)
            {
                //result += c.Name + " = " + c.GeoPosition + "; ";
            }
        }


        return result;
    }

}