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
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Welcome()
        {

            return View();
        }
        
        public ActionResult Index2()
        {
            Note note = new Note();
            List<Movie> ms = new List<Movie>();

            ms.Add(new Movie
            {
                ID = 11,
                Title = "文本框",
                DataStyle = "text",
                DataValue = "",
                OrderID = 1,
                IsMust = 1
            }); ;

            ms.Add(new Movie
            {
                ID = 22,
                Title = "单选按钮",
                DataStyle = "radio",
                DataValue = "1,2,3,4",
                OrderID = 2,
                IsMust = 1
            });

            ms.Add(new Movie
            {
                ID = 33,
                Title = "复选框",
                DataStyle = "checkbox",
                DataValue = "1,2,3,4",
                OrderID = 3,
                IsMust = 1
            });

            ms.Add(new Movie
            {
                ID = 44,
                Title = "下拉框",
                DataStyle = "select",
                DataValue = "1,2,3,4",
                OrderID = 5,
                IsMust = 1
            });

            ms.Add(new Movie
            {
                ID = 55,
                Title = "文本域",
                DataStyle = "textarea",
                DataValue = "",
                OrderID = 6,
                IsMust = 1
            });

            ms.Add(new Movie
            {
                ID = 66,
                Title = "日期录入",
                DataStyle = "date",
                DataValue = DateTime.Now.ToString("yyyy-MM-dd"),
                OrderID = 7,
                IsMust = 0
            });
            ms.Add(new Movie
            {
                ID = 77,
                Title = "数字录入",
                DataStyle = "number",
                DataValue = "",
                OrderID = 8,
                IsMust = 1
            });
            ms.Add(new Movie
            {
                ID = 88,
                Title = "表格",
                DataStyle = "table",
                DataValue = "A1,A2,A3",
                OrderID = 8,
                IsMust = 1
            });
            ms.Add(new Movie
            {
                ID = 99,
                Title = "表格2",
                DataStyle = "table",
                DataValue = "A1,A2,A3",
                OrderID = 9,
                IsMust = 1
            });
            ms.Add(new Movie
            {
                ID = 100,
                Title = "文件上传",
                DataStyle = "file",
                DataValue = "",
                OrderID = 10,
                IsMust = 1
            });
            var list = (from p in ms orderby p.OrderID ascending select p).ToList();
            note.movies = list;
            note.Title = "设备点检表单";
            note.SheetID = "1001";
            //return View(list);
            return View(note);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult SheetList()
        {
            ViewBag.Message = "Your contact SheetList.";

            return View();
        }
        public ActionResult SheetBody()
        {
            ViewBag.Message = "Your contact SheetList.";

            List<ConfigModel> cm = new List<ConfigModel>();

            cm.Add(new ConfigModel { ID = 1, WipName = "检查日期", Datatype = "date", IsMustInput = 1, defaultData = "" });
            cm.Add(new ConfigModel { ID = 2, WipName = "湿度", Datatype = "number", IsMustInput = 1, defaultData = "" });
            cm.Add(new ConfigModel { ID = 3, WipName = "温度", Datatype = "number", IsMustInput = 1, defaultData = "" });
            cm.Add(new ConfigModel { ID = 4, WipName = "单据号", Datatype = "text", IsMustInput = 0, defaultData = "" });
            cm.Add(new ConfigModel { ID = 5, WipName = "作业员", Datatype = "text", IsMustInput = 1, defaultData = ",a,b,c,d" });
            //cm.Add(new ConfigModel { ID = 6, WipName = "检查日期2", Datatype = "date", IsMustInput = 1, defaultData = "" });
            //cm.Add(new ConfigModel { ID = 7, WipName = "湿度2", Datatype = "number", IsMustInput = 1, defaultData = "" });
            //cm.Add(new ConfigModel { ID = 8, WipName = "温度2", Datatype = "number", IsMustInput = 0, defaultData = "" });
            //cm.Add(new ConfigModel { ID = 9, WipName = "单据号2", Datatype = "text", IsMustInput = 0, defaultData = "" });
            //cm.Add(new ConfigModel { ID = 10, WipName = "作业员2", Datatype = "text", IsMustInput = 1, defaultData = ",a,b,c,d" });
            var list = (from p in cm orderby p.ID ascending select p ).ToList();

            ConfigModelViewModel model = new ConfigModelViewModel {  SheetName = "SheetName123",  WipModels = list  };
            //return View(list);
            return View(model);
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
 
    }

    
}