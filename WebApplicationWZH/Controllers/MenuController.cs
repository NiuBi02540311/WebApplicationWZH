using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationWZH.Controllers
{
    [SkipCheckLogin]
    public class MenuController : Controller
    {
        // GET: Menu
       
        [SkipVerification]
        public ActionResult Index()
        {
            return View();
        }
    }
}