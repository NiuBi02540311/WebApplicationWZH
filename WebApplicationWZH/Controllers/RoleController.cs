using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplicationWZH.Models;
using WebApplicationWZH.ViewModel; 
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
        public  string GetRoleUser()
        {
            string gridpager = HttpContext.Request.Params["gridPager"];
            GridRequestModel grid = JsonConvert.DeserializeObject<GridRequestModel>(gridpager);
            var find = DB.SqlServer.Select<Users>().Where(a => a.IsDelete == 0).ToList();

            int pageCount = find.Count / grid.pageSize;
            if (find.Count % grid.pageSize != 0)
            {
                pageCount++;
            }

            //GridResponseModel res =   new  GridResponseModel<Users>(find);
            var v = find.Skip((grid.nowPage - 1) * grid.pageSize).Take(grid.pageSize).ToList();

            var find2 = DB.SqlServer.Select<Users>().Where(a => a.IsDelete == 0);
            var v2 = find2.Page(grid.nowPage, grid.pageSize).ToList();

            //var v = find.Skip(grid.nowPage  * grid.pageSize).Take(grid.pageSize).ToList();
            var res = new GridResponseModel<Users>();
            //res.exhibitDatas = v;
            res.exhibitDatas = v2;
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

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public string GetSysUserRoleViewModel()
        {
            //fsql.Select<Xxx>.LeftJoin<Yyy>((a, b) => a.YyyId == b.Id).ToList();
            //var data = DB.SqlServer.Select<SysUserRole>()
            //      .InnerJoin<Users>((a, b) => a.UserID == b.UserID && a.IsActive == 1 && b.IsDelete == 0)
            //      .ToList(a => new { a.UserID, a.RoleID, a.Tid });

            //string json = "{ 'name': 'Tom', 'age': 30 }";
            //JObject jo = JObject.Parse(json);
            //string name = (string)jo["name"];
            //int age = (int)jo["age"];

            //JObject jj = (JObject)JsonConvert.DeserializeObject(json);

            string gridpager = HttpContext.Request.Params["gridPager"];
            JObject grid = JObject.Parse(gridpager);

            string RoleID = grid["parameters"]["RoleID"] == null ? "0": grid["parameters"]["RoleID"].ToString();

            //GridRequestModel grid = JsonConvert.DeserializeObject<GridRequestModel>(gridpager);

            string sql = $@"
                 SELECT b.UserName,a.Tid,a.UserID,a.RoleID FROM dbo.SysUserRole a 
                  inner join dbo.Users b on a.UserID = b.UserID
                  where a.IsActive = 1 and b.IsDelete = 0 and a.RoleID = {RoleID}";

            //直接查询
            var find = DB.SqlServer.Ado.Query<SysUserRoleViewModel>(sql);
            int pageSize = (int)grid["pageSize"];
            int nowPage = (int)grid["nowPage"];
            int pageCount = find.Count / pageSize;
            if (find.Count % pageSize != 0)
            {
                pageCount++;
            }

            var v = find.Skip((nowPage - 1) * pageSize).Take(pageSize).ToList();
                
            //var v = find.Skip(grid.nowPage  * grid.pageSize).Take(grid.pageSize).ToList();
            var res = new GridResponseModel<SysUserRoleViewModel>();
            res.exhibitDatas = v;
            res.isSuccess = true;
            res.nowPage = nowPage;
            res.recordCount = find.Count;
            res.pageSize = pageSize;
            res.pageCount = pageCount;

            return Newtonsoft.Json.JsonConvert.SerializeObject(res);
        }


        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult SysUserRoleDelete(string data)
        {
            //2,1002,1003,1007,1008
             
            var a = data.Split(',').ToList();
            var b = a.ConvertAll(x => Convert.ToInt32(x));
            var rows = DB.SqlServer.Delete<SysUserRole>(b).ExecuteAffrows();
                  
            return Json(new { success = true, ExecuteAffrows = rows });
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult SysUserRoleAdd(string data)
        {
            //2,1002,1003,1007,1008
            // let str = checkRoleID + "_" + Arr.join(',');
            string[] arr = data.Split('_');
            string RoleID = arr[0];
            var UserList = arr[1].Split(',').ToList();
            var b = UserList.ConvertAll(x => Convert.ToInt32(x));
            List<SysUserRole> sysUserRoles = new List<SysUserRole>();

            foreach(int t in b)
            {
                sysUserRoles.Add(new SysUserRole {  RoleID = int.Parse(RoleID), UserID = t, IsActive = 1 });
            }
            var rows = DB.SqlServer.Insert<SysUserRole>().AppendData(sysUserRoles).ExecuteAffrows();

            return Json(new { success = true, ExecuteAffrows = rows });
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult SysMenuRoleAdd(string data)
        {
            
            // let str = checkRoleID + "_" + Arr.join(',');
            //2=1,2,3,4
            string[] arr = data.Split('=');
            string RoleID = arr[0];
            var MenuList = arr[1].Split(',').ToList();
            var b = MenuList.ConvertAll(x => Convert.ToInt32(x));
            List<SysMenuRole> sysUserRoles = new List<SysMenuRole>();

            foreach (int t in b)
            {
                sysUserRoles.Add(new SysMenuRole { RoleID = int.Parse(RoleID), MenuID = t, IsActive = 1 });
            }
            var rows = DB.SqlServer.Insert<SysMenuRole>().AppendData(sysUserRoles).ExecuteAffrows();

            return Json(new { success = true, ExecuteAffrows = rows });
        }
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult GetSysUserRoleViewModel2(string RoleID = "1")
        {
            //fsql.Select<Xxx>.LeftJoin<Yyy>((a, b) => a.YyyId == b.Id).ToList();
            //var data = DB.SqlServer.Select<SysUserRole>()
            //      .InnerJoin<Users>((a, b) => a.UserID == b.UserID && a.IsActive == 1 && b.IsDelete == 0)
            //      .ToList(a => new { a.UserID, a.RoleID, a.Tid });

            string sql = $@"
                 SELECT b.UserName,a.Tid,a.UserID,a.RoleID FROM dbo.SysUserRole a 
                  inner join dbo.Users b on a.UserID = b.UserID
                  where a.IsActive = 1 and b.IsDelete = 0 and a.RoleID = {RoleID}";
            //直接查询
          var data = DB.SqlServer.Ado.Query<SysUserRoleViewModel>(sql);

            //嵌套一层做二次查询
            //fsql.Select<T>().WithSql(sql).Page(1, 10).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }

 
}