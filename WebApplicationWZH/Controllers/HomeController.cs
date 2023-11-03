using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationWZH.Models;
using FreeSql;
using System.Diagnostics;
using System.Configuration;

namespace WebApplicationWZH.Controllers
{

    //[FormAuthorize(Roles = "1,2")]
    
    public class HomeController : Controller
    {
        
        public HomeController()
        {
            for (int i = 0; i < 10; i++)
            {

            }
        }
        //[Description( ActionOrderNumber = 1, ActionTitle = "主页")]
        public ActionResult Index()
        {

            return View();
        }
        //[Description(ActionOrderNumber = 2, ActionTitle = "欢迎界面")]
        public ActionResult Welcome()
        {

            List<ButtonPermission> buttonPermissionLists = new List<ButtonPermission>();

            buttonPermissionLists.Add(new ButtonPermission { ButtonID = "bt1", Permission = false});
            buttonPermissionLists.Add(new ButtonPermission { ButtonID = "bt2", Permission = true });
            buttonPermissionLists.Add(new ButtonPermission { ButtonID = "bt3", Permission = false });
            buttonPermissionLists.Add(new ButtonPermission { ButtonID = "bt4", Permission = true });
            buttonPermissionLists.Add(new ButtonPermission { ButtonID = "bt5", Permission = false });
            buttonPermissionLists.Add(new ButtonPermission { ButtonID = "bt6", Permission = true });

            ViewBag.buttonPermissionListsJson = JsonConvert.SerializeObject(buttonPermissionLists);
            return View(buttonPermissionLists);
        }
        
       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page....";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
       
        [HttpPost]
        public ActionResult SheetDataSave(string data)
        {
            //[{"WipName":"检查日期","WipValue":"2023-09-30"},{"WipName":"湿度","WipValue":"1"},{"WipName":"温度","WipValue":"2"},{"WipName":"单据号","WipValue":"23123"},{"WipName":"作业员","WipValue":"b"}]
            //return Json(null, JsonRequestBehavior.AllowGet);

            //List<WipModel> WipModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WipModel>>(data);
            WipModelViewModel WipModels = Newtonsoft.Json.JsonConvert.DeserializeObject<WipModelViewModel>(data);
            return Json(WipModels.WipModels);

        }

        [HttpPost]
        [SkipVerification]
        [MyValidateAntiForgeryToken]
        public ActionResult test(string data)
        {
            //[{"WipName":"检查日期","WipValue":"2023-09-30"},{"WipName":"湿度","WipValue":"1"},{"WipName":"温度","WipValue":"2"},{"WipName":"单据号","WipValue":"23123"},{"WipName":"作业员","WipValue":"b"}]
            //return Json(null, JsonRequestBehavior.AllowGet);

            //List<WipModel> WipModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WipModel>>(data);
            //string a = "ActionResult test";
            var obj =  new { GetHashCode = "110", Message = "OK" };
            return Json(obj);

        }

        void Mainn( )
        {
            //反序列化DeserializeObject()可以将一个JSON字符串转换成一个JSON对象；
            //序列化SerializeObject()可以将JSON对象转成JSON字符串
            var JsonStr = "{name:'三',xing:'张'}";
            var DeJsonStr = JsonConvert.DeserializeObject(JsonStr);

            Console.WriteLine("Json字符串：{0}", JsonStr);
            Console.WriteLine(JsonStr.GetType());
            Console.WriteLine("**********Json对象**********");
            Console.WriteLine(DeJsonStr);
            Console.WriteLine(DeJsonStr.GetType());
            Console.WriteLine("**********序列化**********");
            Console.WriteLine(JsonConvert.SerializeObject(DeJsonStr));
            Console.WriteLine(JsonConvert.SerializeObject(DeJsonStr).GetType());

            Console.ReadKey();
        }

        [HttpGet]
        public ActionResult GetbuttonPermissionList(string pathname = "")
        {
            //string url = HttpContext.Request.Url.AbsoluteUri;//http://localhost:57526/home/GetbuttonPermissionList
            ////string ur2 = HttpContext.Request.UrlReferrer.AbsoluteUri.;
            //string ur3 = HttpContext.Request.RawUrl;//   /home/GetbuttonPermissionList
            //string ur4 = HttpContext.Request.Url.PathAndQuery;///home/GetbuttonPermissionList


            //string a =  RouteData.Route.GetRouteData(this.HttpContext).Values["controller"].ToString();
            //a = RouteData.Route.GetRouteData(this.HttpContext).Values["action"].ToString();
            ////或
            //a = RouteData.Values["controller"].ToString();
            //a = RouteData.Values["action"].ToString();

            ////如果在视图中可以用
            //a = RouteData.Route.GetRouteData(this.HttpContext).Values["controller"].ToString();
            //a = RouteData.Route.GetRouteData(this.HttpContext).Values["action"].ToString();
            ////或
            //a = RouteData.Values["controller"].ToString();
            //a = RouteData.Values["action"].ToString();

            //WipModelViewModel WipModels = Newtonsoft.Json.JsonConvert.DeserializeObject<WipModelViewModel>("");
            string name = pathname.Replace("?", "");//  /a/b
            string[] arr = name.Split('/');
            string ControllerName = arr[1];
            string ActionName = arr[2];
            //var data = xxx();
            //var find = (from p in data 
            //            where p.ControllerName.ToLower() == ControllerName.ToLower() && p.ActionName == ActionName.ToLower()  && p.IsDelete == 0
            //            select p).ToList();

            var find = xxx(ControllerName.ToLower(), ActionName.ToLower());
            return Json(find, JsonRequestBehavior.AllowGet);
        }

        private List<ButtonPermission> xxx(string ControllerName,string ActionName)
        {
            //https://freesql.net/guide/getting-started.html#%E8%BF%81%E7%A7%BB
            //https://www.cnblogs.com/kellynic/p/10645049.html
            //List<ButtonPermission> buttonPermissionLists = new List<ButtonPermission>();

            //buttonPermissionLists.Add(new ButtonPermission { RoleID = 1, ControllerName = "home", ActionName = "welcome", ButtonID = "bt1", Permission = false });
            //buttonPermissionLists.Add(new ButtonPermission { RoleID = 1, ControllerName = "home", ActionName = "welcome", ButtonID = "bt2", Permission = true });
            //buttonPermissionLists.Add(new ButtonPermission { RoleID = 1, ControllerName = "home", ActionName = "welcome", ButtonID = "bt3", Permission = false });
            //buttonPermissionLists.Add(new ButtonPermission { RoleID = 1, ControllerName = "home", ActionName = "welcome", ButtonID = "bt4", Permission = true });
            //buttonPermissionLists.Add(new ButtonPermission { RoleID = 1, ControllerName = "home", ActionName = "welcome", ButtonID = "bt5", Permission = false });
            //buttonPermissionLists.Add(new ButtonPermission { RoleID = 1, ControllerName = "home", ActionName = "welcome", ButtonID = "bt6", Permission = true });

            //var blogs = DB.SqlServer.Select<ButtonPermission>()
            //        .Where(b => b.Rating > 3)
            //        .OrderBy(b => b.Url)
            //        .Skip(100)
            //        .Limit(10) //第100行-110行的记录
            //        .ToList();

            string UserRole = Session["UserRole"].ToString();//"admin,aa,bb";
            List<string> ls = UserRole.Split(',').ToList();
            var intList = ls.Select(x => Convert.ToInt32(x));
            var buttonPermissionLists = DB.SqlServer.Select<ButtonPermission>()
                    .Where(p => p.ControllerName.ToLower() == ControllerName   && p.ActionName == ActionName
                    && p.IsDelete == 0
                    && intList.Contains(p.RoleID)
                    ) .ToList();

            return buttonPermissionLists;

            //1   List<string> 转 List<int>

            //1 var strList = new List<string> { "1", "2", "3" };
            //2 var intList = strList.Select<string, int>(x => Convert.ToInt32(x));

            //2   List<int> 转 List<string>

            //1 List<int> intList = new List<int> { 1, 2, 3 };
            //2 List<string> strList = intList.ConvertAll<string>(x => x.ToString());

            //3  List<string> 转 List<long>

            //1 1 var strList = new List<string> { "1", "2", "3" };
            //2 2 var longList = strList.Select<string, long>(x => Convert.ToInt64(x));

        }
    }

    
}