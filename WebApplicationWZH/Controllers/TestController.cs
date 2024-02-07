using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationWZH.Controllers
{
    [SkipCheckLoginAttribute]
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Demo1()
        {
            return View();
        }
        public ActionResult Demo2()
        {
            return View();
        }
        public ActionResult Demo3()
        {
            return View();
        }
        public ActionResult Demo4()
        {
            return View();
        }
        public ActionResult Demo5(string name)
        {

            string access_token = HttpContext.Request.Headers["access_token"];
            var obj = new { code = 1, Message = $"HttpGet hello 2024:  {name},{access_token},{DateTime.Now.ToString()}" };
            return Json(obj,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Demo6(string name)
        {

            string access_token = HttpContext.Request.Headers["access_token"];
            var obj = new { code = 1, Message = $"HttpPost hello 2024:  {name},{access_token},{DateTime.Now.ToString()}" };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getAppsByCategory()
        {

            List<Category> Category = new List<Category>();
            Category.Add(new Controllers.Category { url_mid = "/images/1.jpg" });
            Category.Add(new Controllers.Category { url_mid = "/images/2.jpg" });
            Category.Add(new Controllers.Category { url_mid = "/images/3.jpg" });
            Category.Add(new Controllers.Category { url_mid = "/images/4.jpg" });
            return Json(Category, JsonRequestBehavior.AllowGet);
        }
        
    }
    public class Category
    {
        public string url_mid { get; set; }
    }
}