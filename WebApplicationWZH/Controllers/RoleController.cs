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

    public class GridRequestModel
    {
        //{"isExport":false,"pageSize":10,"startRecord":0,"nowPage":1,"recordCount":-1,"pageCount":-1,"parameters":{},"fastQueryParameters":{},"advanceQueryConditions":[],"advanceQuerySorts":[]}
        //https://os.dlshouwen.com/grid/doc/i18n/zh-cn/example.html#2.2.2
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

        public List<object> exhibitDatas { get; set; }

        public bool exportAllData { get; set; } = false;

        public List<object> exportColumns { get; set; }

        public bool exportDataIsProcessed { get; set; } = false;
    }

    public class GridResponseModel<T> where T : new()
    {
        //{"isExport":false,"pageSize":10,"startRecord":0,"nowPage":1,"recordCount":-1,"pageCount":-1,"parameters":{},"fastQueryParameters":{},"advanceQueryConditions":[],"advanceQuerySorts":[]}
        //https://os.dlshouwen.com/grid/doc/i18n/zh-cn/example.html#2.2.2
        public bool isExport { get; set; }
        public int pageSize { get; set; }
        public int startRecord { get; set; }

        public int nowPage { get; set; }
        public int recordCount { get; set; }
        public int pageCount { get; set; }

        public parameters parameters { get; set; } = new parameters();

        public fastQueryParameters fastQueryParameters { get; set; } = new fastQueryParameters();

        public object advanceQueryConditions { get; set; } = new List<object>();
        public object advanceQuerySorts { get; set; } = new List<object>();

        public List<T> exhibitDatas { get; set; }

      

        public bool exportAllData { get; set; } = false;

        public List<object> exportColumns { get; set; } = new List<object>();

        public bool exportDataIsProcessed { get; set; } = false;

        public List<object> exportDatas { get; set; } = new List<object>();

        public string exportFileName { get; set; } = "";

        public string exportType { get; set; } = "";

        public bool isSuccess { get; set; } = true;
 
    }
    public class parameters
    {
    }
    public class fastQueryParameters
    {
    }
    /*
     查询返回
     {
	"advanceQueryConditions": [],
	"advanceQuerySorts": [],
	"exhibitDatas": [数据],
	"exportAllData": false,
	"exportColumns": [],
	"exportDataIsProcessed": false,
	"exportDatas": [],
	"exportFileName": "",
	"exportType": "",
	"fastQueryParameters": {},
	"isExport": false,
	"isSuccess": true,
	"nowPage": 1,
	"pageCount": 20,
	"pageSize": 10,
	"parameters": {},
	"recordCount": 200,
	"startRecord": 0
     }
     */
}