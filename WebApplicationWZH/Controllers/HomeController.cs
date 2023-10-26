﻿using Newtonsoft.Json;
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

            return View();
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
 
    }

    
}