using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationWZH.Models;

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

        /// <summary>
        /// 获取角色下的账户
        /// </summary>
        /// <returns></returns>
        public string GetUserListString()
        {
            string gridpager = HttpContext.Request.Params["gridPager"];
            JObject jo = JObject.Parse(gridpager);

            string RoleID = jo["parameters"]["RoleID"] == null ? "0" : jo["parameters"]["RoleID"].ToString();

            string UserName = jo["parameters"]["UserName"] == null ? "" : jo["parameters"]["UserName"].ToString();
            
            GridRequestModel grid = JsonConvert.DeserializeObject<GridRequestModel>(gridpager);

            //var d = DB.SqlServer.Select<Users>()
            //     .Where(a => DB.SqlServer.Select<SysUserRole>()
            //     .Where(b=> b.UserID == a.UserID && b.RoleID == int.Parse(RoleID)).Any());

            // var sql = d.ToSql();
            //UserID	UserName	PassWord	ChineseName	Status	IsDelete	CreateTime

            string sql = $@"select a.UserID,a.UserName from Users as a  where not exists 
                     ( select * from SysUserRole as b where a.UserID = b.UserID and  b.RoleID = {RoleID} and a.IsDelete =0 and b.IsActive = 1) order by a.UserID ";

            if(UserName != "")
            {
                sql = $@"select * from (
                            select a.* from Users as a  where not exists 
                            ( select * from SysUserRole as b where a.UserID = b.UserID and  b.RoleID = {RoleID} and a.IsDelete =0 and b.IsActive = 1)
                            ) v 
                            where v.UserName like '%{UserName}%'
                            order by v.UserID";
            }
            var find = DB.SqlServer.Ado.Query<Users>(sql);

            int pageCount = find.Count / grid.pageSize;
            if (find.Count % grid.pageSize != 0)
            {
                pageCount++;
            }

            //GridResponseModel res =   new  GridResponseModel<Users>(find);
            var v = find.Skip((grid.nowPage - 1) * grid.pageSize).Take(grid.pageSize).ToList();
            //var v2 = find.Page(grid.nowPage, grid.pageSize).ToList();

            //var v = find.Skip(grid.nowPage  * grid.pageSize).Take(grid.pageSize).ToList();
            var res = new GridResponseModel<Users>();
            res.exhibitDatas = v;
            res.isSuccess = true;
            res.nowPage = grid.nowPage;
            res.recordCount = find.Count;
            res.pageSize = grid.pageSize;
            res.pageCount = pageCount;

            return Newtonsoft.Json.JsonConvert.SerializeObject(res);

            //var list = DB.SqlServer.Select<Users>().ToList(a => new { a.UserID,a.UserName});
            //return Json(list,JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetUserList()
        {
            var list = DB.SqlServer.Select<Users>().ToList(a => new { a.UserID,a.UserName});
            return Json(list,JsonRequestBehavior.AllowGet);
        }
    }
}
