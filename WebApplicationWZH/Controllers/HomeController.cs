using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationWZH.Models;

namespace WebApplicationWZH.Controllers
{

    //[FormAuthorize(Roles = "1,2")]
    
    public class HomeController : Controller
    {
        //[Description( ActionOrderNumber = 1, ActionTitle = "主页")]
        public ActionResult Index()
        {

            return View();
        }
        //[Description(ActionOrderNumber = 2, ActionTitle = "欢迎界面")]
        public ActionResult Welcome()
        {

            List<ViewButtonPermissionList> buttonPermissionLists = new List<ViewButtonPermissionList>();

            buttonPermissionLists.Add(new ViewButtonPermissionList { ButtonID = "bt1", Permission = false});
            buttonPermissionLists.Add(new ViewButtonPermissionList { ButtonID = "bt2", Permission = true });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ButtonID = "bt3", Permission = false });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ButtonID = "bt4", Permission = true });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ButtonID = "bt5", Permission = false });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ButtonID = "bt6", Permission = true });

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
            var data = xxx();
            var find = (from p in data where p.ControllerAction.ToLower() == pathname.ToLower() select p).ToList();

            return Json(find, JsonRequestBehavior.AllowGet);
        }

        private List<ViewButtonPermissionList> xxx()
        {
            List<ViewButtonPermissionList> buttonPermissionLists = new List<ViewButtonPermissionList>();

            buttonPermissionLists.Add(new ViewButtonPermissionList { ControllerAction = "/home/welcome", ButtonID = "bt1", Permission = false });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ControllerAction = "/home/welcome", ButtonID = "bt2", Permission = true });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ControllerAction = "/home/welcome", ButtonID = "bt3", Permission = false });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ControllerAction = "/home/welcome", ButtonID = "bt4", Permission = true });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ControllerAction = "", ButtonID = "bt5", Permission = false });
            buttonPermissionLists.Add(new ViewButtonPermissionList { ControllerAction = "", ButtonID = "bt6", Permission = true });

            return buttonPermissionLists;
        }
    }

    
}