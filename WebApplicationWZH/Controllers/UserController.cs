using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationWZH.Controllers
{
    [Description(ActionOrderNumber =  1, ActionTitle  = "用户")]
    public class UserController : Controller
    {
        //[RoleAuthorize]
        [HttpGet]
        [Description(ActionOrderNumber = 2, ActionTitle = "用户首页")]
        public ActionResult Index()
        {
            return View();
        }
         
        [HttpPost]
        [Description(ActionOrderNumber = 3, ActionTitle = "用户管理")]
        public ActionResult Admin()
        {
            return View();
        }
        //[RoleAuthorize]
        //[HttpPost]
        [Description(ActionOrderNumber = 4, ActionTitle = "用户详情")]
        public ActionResult Detail()
        {
            return View();
        }
        public ActionResult test()
        {
            return View();
        }
    }
}