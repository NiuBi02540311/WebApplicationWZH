using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationWZH.Models;

namespace WebApplicationWZH.Controllers
{
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [SkipCheckLogin]
        [SkipVerification]
        public ActionResult GetRoleUser()
        {
            string gridpager = HttpContext.Request.Params["gridPager"];
            gridPager grid = Newtonsoft.Json.JsonConvert.DeserializeObject<gridPager>(gridpager);
            var find = DB.SqlServer.Select<Users>().Where(a => a.IsDelete == 0).ToList();
            return Json(find);
        }
    }

    public class gridPager
    {
        //{"isExport":false,"pageSize":10,"startRecord":0,"nowPage":1,"recordCount":-1,"pageCount":-1,"parameters":{},"fastQueryParameters":{},"advanceQueryConditions":[],"advanceQuerySorts":[]}

        public bool isExport { get; set; }
        public int pageSize { get; set; }
        public int startRecord { get; set; }

        public int nowPage { get; set; }
        public int recordCount { get; set; }
        public int pageCount { get; set; }

        public class parameters
        { 
        }
        public class fastQueryParameters
        {
        }

        public object advanceQueryConditions { get; set; }
        public object advanceQuerySorts { get; set; }
    }
}