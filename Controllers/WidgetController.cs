using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
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
    public class WidgetController : UmbracoApiController
    {
        DefaultContext dc = new DefaultContext();
        HttpRequest currentRequest = HttpContext.Current.Request;

        [HttpOptions]
        public dynamic Add()
        {
            return Ok();
        }


        /// <summary>
        /// добавление виджета в базу
        /// </summary>
        /// <param name="simpleWidgetUserProfile">настройки виджета и профиль админа</param>
        /// <returns></returns>
        [HttpPost]
        public dynamic Add(SimpleWidgetUserProfile simpleWidgetUserProfile)
        {
            return null;
        }


        /// <summary>
        /// Обновление виджета сообщества
        /// </summary>
        /// <param name="simpleWidget"></param>
        /// <returns></returns>
        public bool SetWidget(
            //SimpleWidget simpleWidget
            )
        {
            System.Net.Http.HttpClient httpClient = new HttpClient();
            
            
            
            
            var pars = new System.Collections.Specialized.NameValueCollection();
            pars.Add("content-type", "application/x-www-form-urlencoded");

            string url =
                //"https://api.vk.com/method/appWidgets.getAppImages?access_token=453e5fb9453e5fb9453e5fb95145531aa24453e453e5fb918a4169155e644065f254005&image_type=50x50&v=5.131";
                "https://api.vk.com/method/appWidgets.getAppImageUploadServer?access_token=453e5fb9453e5fb9453e5fb95145531aa24453e453e5fb918a4169155e644065f254005&image_type=24x24&v=5.131";
            //"https://api.vk.com/method/appWidgets.update?type=text&access_token=c5bbb710d6b41f7cfb3fcf74f6daadf90645e066a38e85bf599f01fc31769339abd4929b0f2315b4b50ea&v=5.131&code=return%20%7B%20%09%09%09%09%09%09%09%09%09%09%09%22title%22%3A%20%22My%20Table%22%2C%20%09%09%09%09%09%09%09%09%09%09%09%22title_url%22%3A%20%22https%3A%2F%2Fvk.com%2Fpublic123%22%2C%20%09%09%09%09%09%09%09%09%09%09%09%22title_counter%22%3A%2031%2C%20%09%09%09%09%09%09%09%09%09%09%09%22more%22%3A%20%22%D0%9F%D0%BE%D1%81%D0%BC%D0%BE%D1%82%D1%80%D0%B5%D1%82%D1%8C%20%D0%B2%D1%81%D0%B5%20%D1%80%D0%B5%D0%B7%D1%83%D0%BB%D1%8C%D1%82%D0%B0%D1%82%D1%8B%22%2C%20%09%09%09%09%09%09%09%09%09%09%09%22more_url%22%3A%20%22https%3A%2F%2Fvk.com%2Fapp123_-888%22%2C%20%09%09%09%09%09%09%09%09%09%09%09%22text%22%3A%20%22text%20field%22%2C%20%09%09%09%09%09%09%09%09%09%09%09%22descr%22%3A%20%22description%20field%22%2C%20%09%09%09%09%09%09%09%09%09%09%7D%3B";

            //WebRequest wr;
            

            using (WebClient wc = new WebClient())
            {
                //wc.DownloadString()
                
                //var response = wc.UploadValues(url, pars);
                //string str = System.Text.Encoding.UTF8.GetString(response);
                //System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

                //FromVKWidgetUpload uploadurlobject = js.Deserialize<FromVKWidgetUpload>(str);
                //string uploadUrl = uploadurlobject != null ? uploadurlobject.response.Values.FirstOrDefault() : "";

                //var resp = wc.UploadFile(uploadUrl, @"c:\temp\24x24.png");
                //string str1 = System.Text.Encoding.UTF8.GetString(resp);

                //url = "https://api.vk.com/method/appWidgets.saveAppImage?access_token=453e5fb9453e5fb9453e5fb95145531aa24453e453e5fb918a4169155e644065f254005&v=5.131&hash=36354f9fec7201da73&image=eyJlcnJvciI6IkVSUl9VUExPQURfRklMRV9OT1RfVVBMT0FERUQ6IGZpbGUgbm90IGZvdW5kIiwiYndhY3QiOiJhcHBfd2lkZ2V0X2ltYWdlIiwic2VydmVyIjo1MzY2MzIsIm1pZCI6MCwiX3NpZyI6IjJhNTU4OGRlNzM1ZTliMWZkMjZhMzE4OGNmZjU0ZjA1In0";
                //ForVKWidgetAppSave forappsaveobject = js.Deserialize<ForVKWidgetAppSave>(str1);
                //pars.Add("hash", forappsaveobject.hash);
                //pars.Add("image", forappsaveobject.image);
                //resp = wc.UploadValues(url, pars);
                //str1 = System.Text.Encoding.UTF8.GetString(resp);


                /*
                 * {"response":{"upload_url":"https:\/\/pu.vk.com\/c520132\/ss2208\/upload.php?_query=eyJhY3QiOiJhcHBfd2lkZ2V0X2ltYWdlIiwib2lkIjo3MTYxMTE1LCJhaWQiOjcxNjExMTUsInR5cGUiOjUsIndpZHRoIjoxNTAsImhlaWdodCI6MTUwLCJhcGlfd3JhcCI6eyJoYXNoIjoiMzYzNTRmOWZlYzcyMDFkYTczIiwiaW1hZ2UiOiJ7cmVzdWx0fSJ9LCJtaWQiOjAsInNlcnZlciI6NTIwMTMyLCJfb3JpZ2luIjoiaHR0cHM6XC9cL2FwaS52ay5jb20iLCJfc2lnIjoiMzk4NTc4NjJkNTUwZWJmMDY5MGIzOGEyYTUxMjJjMGUifQ"}}
                 * 
                 * {"hash":"c81e40be6bf328a768","image":"eyJvaWQiOjcxNjExMTUsInR5cGUiOjUsInBob3RvIjp7InBob3RvIjoiMDBhNjlkYjdlYngiLCJzaXplcyI6W10sImtpZCI6IjhmN2MyNTExZDI0NzRiYTBiY2E0NzYyOTgxMjZhNzA1IiwiZGVidWciOiJ4Y2MiLCJzaXplczIiOltbImEiLCI5NDA4ZGQzZDI5YjM1ZDg5ZTVmMjVlN2Y5OWE1ZjY5ZjFhMzNlNjk5MTg3MjViNmM5ZTFmOGJmYyIsIi00MTUwMTQ1NTc4MzI5MzM3NDY4Iiw1MCw1MF0sWyJiIiwiZmZlMWYzMWNhMGU4NGY2MDY5ZjU3OTgzZTg5OTkzMTM2Yzc0ZWFiMDRlMmY5YzQ5ZjE2ODFmOTMiLCItMTQzOTgxOTU0MTQyNTAzOTIwNyIsMTAwLDEwMF0sWyJjIiwiNTM2NGJkYzRlZjM1YjE4YTRmYTAzNTZmM2Y3N2Q3MzY4MzczMTgwYWZlNDkzNDdhZGFmMzIwZjgiLCIzODYxMTQyNzUxMDY2NzMwMjIyIiwxNTAsMTUwXV0sInVybHMiOltdLCJ1cmxzMiI6WyJsQWpkUFNtelhZbmw4bDVfbWFYMm54b3o1cGtZY2x0c25oLUxfQVwvaEpHRnZaVzRaOFkuanBnIiwiXy1IekhLRG9UMkJwOVhtRDZKbVRFMngwNnJCT0w1eEo4V2dma3dcL21kaXhiTUc3Qk93LmpwZyIsIlUyUzl4Tzgxc1lwUG9EVnZQM2ZYTm9OekdBci1TVFI2MnZNZy1BXC83aTVvLU4tSWxUVS5qcGciXX0sImJ3YWN0IjoiYXBwX3dpZGdldF9pbWFnZSIsInNlcnZlciI6NTI5MzMyLCJtaWQiOjAsIl9zaWciOiI3Zjg0YzFiNDQ3YWU3ODNhZWM4ODAxNWUyYWQ4NWU4NiJ9"}
                 * 
                 * {"response":
                 * {"id":"7161115_2061897",
                 * "type":"50x50",
                 * "images":[
                 * {"height":50,"url":"https:\/\/sun1-95.userapi.com\/lAjdPSmzXYnl8l5_maX2nxoz5pkYcltsnh-L_A\/hJGFvZW4Z8Y.jpg","width":50},
                 * {"height":100,"url":"https:\/\/sun1-89.userapi.com\/_-HzHKDoT2Bp9XmD6JmTE2x06rBOL5xJ8Wgfkw\/mdixbMG7BOw.jpg","width":100},
                 * {"height":150,"url":"https:\/\/sun1-91.userapi.com\/U2S9xO81sYpPoDVvP3fXNoNzGAr-STR62vMg-A\/7i5o-N-IlTU.jpg","width":150}]}}
                 * 
                 * {"response":{"id":"7161115_2061913",
                 * "type":"24x24",
                 * "images":[
                 * {"height":24,"url":"https:\/\/sun1-83.userapi.com\/ufOSPy1BzQxgy-9b_MkLBcHu6AuSN2KJxQKkFQ\/dFiUmCnZA0U.jpg","width":24},
                 * {"height":48,"url":"https:\/\/sun1-91.userapi.com\/AX1BhvrRzclcI4XM6o6feWivSKYNQsA9nk60ZA\/_TP9TgQSqFQ.jpg","width":48},
                 * {"height":72,"url":"https:\/\/sun1-91.userapi.com\/hOrTPTilTnRoX2jhLqWNEfca70RSr3W7yTQJtQ\/HqNY3MJgG_g.jpg","width":72}
                 * ]}}
                 */



            }

            //httpClient.PostAsync(url, pars);

            return true;
        }
    }
}