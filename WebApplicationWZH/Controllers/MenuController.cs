using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationWZH.Controllers
{
    //[SkipCheckLogin]
    [Description(ActionOrderNumber = 1, ActionTitle = "菜单管理")]
    public class MenuController : Controller
    {
        // GET: Menu

        //[SkipVerification]
        [Description(ActionOrderNumber = 2, ActionTitle = "主页")]
        public ActionResult Index()
        {
            return View();
        }

        [Description(ActionOrderNumber = 2, ActionTitle = "新增")]
        [HttpPost]
        public ActionResult Insert()
        {
            return View();
        }

        [Description(ActionOrderNumber = 2, ActionTitle = "修改")]
        [HttpPost]
        public ActionResult Update()
        {
            return View();
        }

        [Description(ActionOrderNumber = 2, ActionTitle = "删除")]
        [HttpPost]
        public ActionResult Delete()
        {
            return View();
        }
    }
}