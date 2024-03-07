using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplicationWZH.Models;
namespace WebApplicationWZH.Controllers
{
    public class FreeSqlController : Controller
    {
        // GET: FreeSql httpss://blog.csdn.net/weixin_42401291/article/details/129360772
        //https://freesql.net/guide/getting-started.html#%E5%A3%B0%E6%98%8E 
        //https://github.com/dotnetcore/FreeSql/issues
        public ActionResult Index()
        {
            return View();
        }
         
        /// <summary>
        /// 基础使用(增加、查询、删除、)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlBase()
        {
            //插入单一数据
            var blog = new SysRole() { RoleID = 100, RoleName = "RoleName100", IsActive = 1 };

            var save = await DB.SqlServer.Insert<SysRole>(blog).ExecuteAffrowsAsync();

           

            //查询
            var select1 = await DB.SqlServer.Select<SysRole>().Where(x => x.RoleID == 1).ToListAsync();
            var select2 = DB.SqlServer.Select<SysRole>().ToList();  


            //分页查询
            var select4 = DB.SqlServer.Select<SysRole>()
               .Where(a => a.RoleID > 1);
            var sql = select4.ToSql();
            var total = await select4.CountAsync();
            var list = await select4.Page(1, 20).ToListAsync();


            //修改
            var select3 = await DB.SqlServer.Select<SysRole>().FirstAsync();
            select3.RoleName = DateTime.Now.ToString();
            var save2 = await DB.SqlServer.InsertOrUpdate<SysRole>().SetSource(select3).ExecuteAffrowsAsync();


            //删除
            var delete = await DB.SqlServer.Delete<SysRole>().Where(x => x.RoleID == 120).ExecuteAffrowsAsync();


            return "123";
        }
        /// <summary>
        /// 事务使用
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
       // public async Task<string> TestFreeSqlDbContext1()
      //  {
            //工作单元
            //var ctx = _dbContext;

            //var blog = new Blog() { Rating = 1, Url = DateTime.Now.ToString() };
            //ctx.Set<Blog>().Add(blog);

            //var user = new User() { Name = DateTime.Now.ToString(), Age = 1 };
            //ctx.Set<User>().Add(user);

            //var save = await ctx.SaveChangesAsync();


          //  return "123";
       // }
        /// <summary>
        /// 读从库、写主库,伪功能（需要自己实现数据库数据同步）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlReadWrite()
        {
            //文档：https://freesql.net/guide/read-write-splitting.html


            var select2 = DB.SqlServer.Select<SysRole>().Where(x => x.RoleID > 0).ToList();//读取从库

            //插入单一数据
            var blog = new SysRole() { RoleID = 111, RoleName = "RoleName111" };
            var saveSql = await DB.SqlServer.Insert<SysRole>(blog).ExecuteAffrowsAsync();//写入主库

            var select3 = DB.SqlServer.Select<SysRole>().ToList();//读取从库

            var select4 = DB.SqlServer.Select<SysRole>().Master().ToList();//读取主库


            return "123";
        }
        /// <summary>
        /// 分表（自动分表）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlSeparate()
        {
            //文档：https://github.com/dotnetcore/FreeSql/discussions/1066

            //插入单一数据
            var asTableLog = new SysRole() { RoleID = 111, RoleName = "RoleName111" };
            var saveSql = await DB.SqlServer.Insert<SysRole>(asTableLog).ExecuteAffrowsAsync();

            //插入单一数据

 
            var asTableLog2 = new SysRole() { RoleID = 1111, RoleName = "RoleName1111" };

            var saveSql2 = await DB.SqlServer.Insert<SysRole>(asTableLog2).ExecuteAffrowsAsync();


            //查询
            var select = DB.SqlServer.Select<SysRole>();
            //.Where(a => a.createtime.Between(DateTime.Parse("2022-3-1"), DateTime.Parse("2022-5-1")));
            var sql = select.ToSql();
            var list = select.ToList();

            return "123";
        }
 
    }
}