using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using System.Data.Entity;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using osbackend.Models;


namespace osbackend.Controllers
{
    public class LogWhenPublished : ApplicationEventHandler
    {
        /// Here we'll subscribe to an event
        /// 
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //ContentService.Saving += ContentService_Saving; // rasied before the content has been saved.
            ContentService.Saved += ContentService_Saving; // raised after the content has been saved.
            base.ApplicationStarted(umbracoApplication, applicationContext);
        }

        /// <summary>
        /// Event raised when content is saved/saving.
        /// </summary>
        /// <param name="contentService">Can be used for creating/deleting/editing/move other content etc.</param>
        /// <param name="eventArgs">Can be used for showing custom notifications, canceling the save event, and accessing the content beeing saved etc.</param>
        private void ContentService_Saving(IContentService contentService, SaveEventArgs<IContent> eventArgs)
        {
            IEnumerable<IContent> content = eventArgs.SavedEntities;
            foreach (var c in content)
            {
                if (c.ContentType.Alias == "place")
                {
                    int resultId = -1;
                    SimplePlace splace = new SimplePlace();
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

                    AddressToConvert adr = null;
                    if (c.Properties["address"].Value != null)
                        adr = js.Deserialize<List<AddressToConvert>>(c.Properties["address"].Value.ToString()).First();

                    //object worktime = c.Properties["worktime"].Value;
                    List<WorktimeToConvert> worktime = null;

                    if (c.Properties["worktime"].Value != null)
                        worktime = js.Deserialize<List<WorktimeToConvert>>(c.Properties["worktime"].Value.ToString());

                    splace.SimpleCityId = 3; //3-кострома umbracocityid=1080; // ВРЕМЕННОЕ РЕШЕНИЕ! сделать конфигурируемый!

                    splace.Address = (adr == null) ? "" : ((adr.subjectType != "" ? adr.subjectType + " " : "") +
                        (adr.street != "" ? adr.street + " " : "") +
                        (adr.house != "" ? adr.house : ""));

                    splace.BicycleParking = (c.Properties["bicycleParking"].Value != null) ? (Convert.ToInt32(
                        js.Deserialize<int>(c.Properties["bicycleParking"].Value.ToString())) == 1 ? true : false
                        ) : false;
                    splace.Deleted = false;
                    splace.UmbracoId = c.Id;
                    splace.Published = (c.Properties["enabled"].Value != null) ? (Convert.ToInt32(
                        js.Deserialize<int>(c.Properties["enabled"].Value.ToString())) == 1 ? true : false
                        ) : false;
                    splace.Enabled = (c.Properties["enabled"].Value != null) ? (Convert.ToInt32(
                        js.Deserialize<int>(c.Properties["enabled"].Value.ToString())) == 1 ? true : false
                        ) : false;
                    splace.Geo = (c.Properties["geo"].Value != null) ? c.Properties["geo"].Value.ToString() : "";
                    splace.MainPicture = (c.Properties["photo"].Value != null) ? c.Properties["photo"].Value.ToString() : "";
                    splace.Name = (c.Properties["placename"].Value != null) ? c.Properties["placename"].Value.ToString() : "";
                    splace.Info = (c.Properties["placeinfo"].Value != null) ? c.Properties["placeinfo"].Value.ToString() : "";
                    splace.Parking = (c.Properties["parking"].Value != null) ? (Convert.ToInt32(
                        js.Deserialize<int>(c.Properties["parking"].Value.ToString())) == 1 ? true : false
                        ) : false;

                    splace.Worktime = new List<Worktime>();

                    // заносим расписание (создаем)
                    foreach(var item in worktime)
                    {
                        Worktime wT = new Worktime();
                        List<BreakToConvert> breaks = null;

                        if (item.breakTimes != null)
                            breaks = js.Deserialize<List<BreakToConvert>>(item.breakTimes);

                        if (item.fromTime != null)
                        {
                            wT.FromTime = item.fromTime;
                            wT.When = item.fromTime;
                        }
                        if (item.toTime != null)
                        {
                            wT.ToTime = item.toTime;
                        }
                        if (item.costPerHour != 0)
                        {
                            wT.CostPerHour = item.costPerHour.HasValue ? item.costPerHour.Value : 0;

                        }
                        wT.Works24 = item.works24 == 1 ? true : false;
                        wT.Published = true;
                        wT.Deleted = false;
                        
                        wT.Breaks = new List<Break>();

                        if (breaks != null)
                            foreach (var itm in breaks)
                            {
                                Break bT = new Break();
                                if (itm.fromTime != null)
                                {
                                    bT.FromTime = itm.fromTime;
                                }
                                if (itm.toTime != null)
                                {
                                    bT.ToTime = itm.toTime;
                                }
                                bT.Published = true;
                                bT.Deleted = false;
                                wT.Breaks.Add(bT);
                            }

                        splace.Worktime.Add(wT);
                    }

                    bool edit = false;
                    SimplePlace spl = new SimplePlace();

                    // чистим расписание
                    using (DefaultContext dc1 = new DefaultContext())
                    {
                        spl = dc1.SimplePlaces
                            .FirstOrDefault(sp => sp.UmbracoId == splace.UmbracoId);

                        if (spl != null)
                        {
                            edit = true;


                            List<Worktime> wtimes = (List<Worktime>)dc1.SimplePlaces
                            .Where(x => x.UmbracoId == splace.UmbracoId)
                            .Join(dc1.Worktimes, a => a.Id, b => b.SimplePlaceId, (spla, wt) => new { spla, wt })
                            .Select(x => x.wt).ToList();

                            List<Break> brks = wtimes
                                .Join(dc1.Breaks, a => a.WorktimeId, b => b.WorktimeId, (wtms, bks) => new { wtms, bks })
                                .Select(x => x.bks).ToList();

                            dc1.Breaks.RemoveRange(brks);
                            dc1.Worktimes.RemoveRange(wtimes);

                            dc1.SaveChanges();
                        }
                    }

                    if (!edit)
                    {
                        using (DefaultContext dc = new DefaultContext())
                        {
                            dc.SimplePlaces.Add(splace);
                            dc.SaveChanges();
                            resultId = splace.Id;
                        }
                    }
                    else
                    {
                        spl.Address = splace.Address;
                        spl.BicycleParking = splace.BicycleParking;
                        spl.City = null;
                        spl.Deleted = splace.Deleted;
                        spl.Enabled = splace.Enabled;
                        spl.ErrorMessage = splace.ErrorMessage;
                        spl.Geo = splace.Geo;
                        spl.Info = splace.Info;
                        spl.MainPicture = splace.MainPicture;
                        spl.Name = splace.Name;
                        spl.Parking = splace.Parking;
                        spl.Published = splace.Published;
                        spl.SimpleCityId = splace.SimpleCityId;
                        spl.Worktime = splace.Worktime;

                        using (DefaultContext dc = new DefaultContext())
                        {
                            foreach(Worktime wt in spl.Worktime)
                            {
                                wt.SimplePlaceId = spl.Id;
                                wt.SimplePlace = null;
                                dc.Worktimes.Add(wt);
                            }
                            dc.Entry(spl).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            //resultId = splace.Id;
                        }
                    }

                    // при изменении расписания нужно делать оповещение людей, которые арендовали время

                    // редактируем место, пересоздаем расписание на сегодня и дальше. старое расписание не трогаем. вносим в кастом базу только новые значения
                    //new SimplePlaceController().EditSimplePlace();
                }
            }

            /* Do custom wizard stuff here.. */
            //(new System.Collections.Generic.Mscorlib_KeyedCollectionDebugView<string, Umbraco.Core.Models.Property>(((Umbraco.Core.Models.ContentBase)(new System.Collections.Generic.Mscorlib_CollectionDebugView<Umbraco.Core.Models.IContent>(eventArgs.EventObject).Items[0])).Properties).Items[1]).Value
        }

    }
}