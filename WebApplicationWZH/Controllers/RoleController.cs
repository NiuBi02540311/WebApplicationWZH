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
        [MyValidateAntiForgeryToken]
        //[SkipCheckLogin]
        //[SkipVerification]
        public string GetRoleUser()
        {
            string gridpager = HttpContext.Request.Params["gridPager"];
            GridRequestModel grid = Newtonsoft.Json.JsonConvert.DeserializeObject<GridRequestModel>(gridpager);
            var find = DB.SqlServer.Select<Users>().Where(a => a.IsDelete == 0).ToList();

            int pageCount = find.Count / grid.pageSize;
            if (find.Count % grid.pageSize != 0)
            {
                pageCount++;
            }

            //GridResponseModel res =   new  GridResponseModel<Users>(find);
            var v = find.Skip((grid.nowPage - 1) * grid.pageSize).Take(grid.pageSize).ToList();
            //var v = find.Skip(grid.nowPage  * grid.pageSize).Take(grid.pageSize).ToList();
            var res = new GridResponseModel<Users>();
            res.exhibitDatas = v;
            res.isSuccess = true;
            res.nowPage = grid.nowPage;
            res.recordCount = find.Count;
            res.pageSize = grid.pageSize;
            res.pageCount = pageCount;

            return Newtonsoft.Json.JsonConvert.SerializeObject(res);
            //return Json(res);

            // 2. DataTable 转 List
            //List<CustomerContact> list2 = DtToList<CustomerContact>.ConvertToModel(dt);
        }
    }

 
}