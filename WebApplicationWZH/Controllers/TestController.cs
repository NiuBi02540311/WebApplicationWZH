using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        public ActionResult Demo5()
        {
            return View();
        }

    }
   
}