using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplicationWZH.Models;
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

        [HttpPost]
        public string GetNodes(int? parentId)
        {
            //if (parentId == null)
            //{
            //    return "";
            //}
            //string sql = "SELECT CategoryID,ParentCode,IsEnd,LevelName,CodeName FROM nodes where usable = 0 and parentcode = " + parentId + " order by levelname ";

            //DataTable dt = DB.GetTable(sql);
            //List<sys_device> dsList = new List<sys_device>();
            //if (parentId == 0)
            //{
            //    dsList.Add(new sys_device()
            //    {
            //        id = 0,
            //        pId = 0,
            //        name = "目录总层级",
            //        isParent = true
            //    });
            //}
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dsList.Add(new sys_device()
            //    {
            //        id = Convert.ToInt32(dt.Rows[i]["CategoryID"]),
            //        pId = Convert.ToInt32(dt.Rows[i]["ParentCode"]),
            //        isEnd = Convert.ToInt32(dt.Rows[i]["IsEnd"]),
            //        name = dt.Rows[i]["LevelName"].ToString(),
            //        codename = dt.Rows[i]["CodeName"].ToString(),
            //        isParent = Convert.ToInt32(dt.Rows[i]["IsEnd"]) == 0//似乎必须要
            //    });
            //}
            //res.list = JsonHelper.Instance.Serialize(dsList);
            //res.success = true;
            //res.msg = "查询成功";
            //return JsonOperate.ObjToJson<ResponseEntity>(res);

            return "";
        }

        [HttpGet]
        public string GetAllNodes()
        {
            //string sql = "SELECT CategoryID,ParentCode,IsEnd,LevelName,CodeName FROM node where usable = 0  order by levelname ";

            //DataTable dt = DB.GetTable(sql);
            //List<sys_device> dsList = new List<sys_device>();
            //dsList.Add(new sys_device()
            //{
            //    id = 0,
            //    pId = 0,
            //    name = "目录总层级",
            //    isParent = true
            //});
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dsList.Add(new sys_device()
            //    {
            //        id = Convert.ToInt32(dt.Rows[i]["CategoryID"]),
            //        pId = Convert.ToInt32(dt.Rows[i]["ParentCode"]),
            //        isEnd = Convert.ToInt32(dt.Rows[i]["IsEnd"]),
            //        name = dt.Rows[i]["LevelName"].ToString(),
            //        codename = dt.Rows[i]["CodeName"].ToString(),
            //        isParent = Convert.ToInt32(dt.Rows[i]["IsEnd"]) == 0
            //    });
            //}
            //res.list = JsonHelper.Instance.Serialize(dsList);
            //res.success = true;
            //res.msg = "查询成功";
            //return JsonOperate.ObjToJson<ResponseEntity>(res);

            return "";
        }

        /// <summary>
        /// 给页面提供json格式的节点数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetjsonDb()
        {
            ///节点类集合
            List<MyNodes> myNodes = new List<MyNodes>();
            myNodes.Add(new MyNodes
            {
                id = 1,
                name = "首页        ",
                pId = 0,
                open = false,
                isParent = true
            });
            myNodes.Add(new MyNodes
            {
                id = 2,
                name = "攻略",
                pId = 0,
                isParent = true
            });
            myNodes.Add(new MyNodes
            {
                id = 3,
                name = "王者攻略 ",
                pId = 2
            });
            //将获取的节点集合转换为json格式字符串，并返回
            string json = JsonConvert.SerializeObject(myNodes);
            return json;
        }


        public ActionResult getNodes(string id)
        {
            //根据传入的id查询子节点数据
            List<TreeNode> nodes = new List<TreeNode>();
            nodes.Add(new TreeNode { id = 1, isParent = true, name = "a", open = true });
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }
        private List<SysMenu> GetAllMenu()
        {
            // https://www.treejs.cn/v3/main.php#_zTreeInfo
            //https://blog.csdn.net/qq_26312205/article/details/107718641
            List<SysMenu> sysMenus = new List<SysMenu>();

            //sysMenus.Add(new SysMenu { Tid = 0, MenuName = "财务管理员", MenuUrl = "", ParentTid = -1, OrderRule = 1, Level = 1, IsActive = 1 });

            sysMenus.Add(new SysMenu { Tid = 1, MenuName = "a", MenuUrl = "", ParentTid = 0 , OrderRule = 1, Level = 1, IsActive = 1});
            sysMenus.Add(new SysMenu { Tid = 2, MenuName = "a1", MenuUrl = "", ParentTid = 1, OrderRule = 1, Level = 2, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 3, MenuName = "a2", MenuUrl = "", ParentTid = 1, OrderRule = 1, Level = 2, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 4, MenuName = "a3", MenuUrl = "", ParentTid = 1, OrderRule = 1, Level = 2, IsActive = 1 });

            sysMenus.Add(new SysMenu { Tid = 5, MenuName = "a11", MenuUrl = "", ParentTid = 2, OrderRule = 1, Level = 3, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 6, MenuName = "a21", MenuUrl = "", ParentTid = 3, OrderRule = 1, Level = 3, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 7, MenuName = "a31", MenuUrl = "", ParentTid = 4, OrderRule = 1, Level = 3, IsActive = 1 });

            sysMenus.Add(new SysMenu { Tid = 8, MenuName  = "b", MenuUrl = "", ParentTid = 0, OrderRule = 1, Level = 1, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 9, MenuName  = "b1", MenuUrl = "", ParentTid = 8, OrderRule = 1, Level = 2, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 10, MenuName = "b2", MenuUrl = "", ParentTid = 8, OrderRule = 1, Level = 2, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 11, MenuName = "b3", MenuUrl = "", ParentTid = 8, OrderRule = 1, Level = 2, IsActive = 1 });

            sysMenus.Add(new SysMenu { Tid = 12, MenuName = "b11", MenuUrl = "", ParentTid = 9, OrderRule = 1, Level = 3, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 13, MenuName = "b21", MenuUrl = "", ParentTid = 10, OrderRule = 1, Level = 3, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 14, MenuName = "b31", MenuUrl = "", ParentTid = 11, OrderRule = 1, Level = 3, IsActive = 1 });

            sysMenus.Add(new SysMenu { Tid = 15, MenuName = "c",  MenuUrl = "", ParentTid = 0, OrderRule = 1, Level = 1, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 16, MenuName = "c1", MenuUrl = "", ParentTid = 15, OrderRule = 1, Level = 2, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 17, MenuName = "c2", MenuUrl = "", ParentTid = 15, OrderRule = 1, Level = 2, IsActive = 1 });
            sysMenus.Add(new SysMenu { Tid = 18, MenuName = "c3", MenuUrl = "", ParentTid = 15, OrderRule = 1, Level = 2, IsActive = 1 });

            return sysMenus;

        }

        /// <summary>
        /// 生成zTree标准json数据源
        /// </summary>
        /// <param name="parent_id">父节点</param>
        /// <returns></returns>
        public string GetModuleTreeJson(string parent_id)
        {
            //https://www.cnblogs.com/tlbxygw/p/2642086.html

            // 调用  GetModuleTreeJson("0");
            StringBuilder jsonTree = new StringBuilder();
            if (string.IsNullOrEmpty(parent_id))
            {
                parent_id = "0";
            }
            string where = string.Format(" and parent_id='{0}'", parent_id);
            DataSet ds = null; //DALModul.getModuleList(where);//封装的查询数据库方法

            if (ds.Tables[0].Rows.Count > 0)
            {
                jsonTree.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    jsonTree.Append("{\"id\":\"").Append(dr["id"]).Append("\",");
                    jsonTree.Append("\"name\":\"").Append(dr["module"]).Append("\",");
                    //在此处添加其他DIY属性，注意最后的“,”
                    jsonTree.Append("\"product\":\"").Append(dr["product"]).Append("\",");
                    jsonTree.Append("\"children\":").Append(GetModuleTreeJson(dr["id"].ToString()));
                    jsonTree.Append("},");
                    if (i == (ds.Tables[0].Rows.Count - 1))
                    {
                        jsonTree.Remove(jsonTree.Length - 1, 1);
                    }
                }
                jsonTree.Append("]");
            }
            else
            {
                jsonTree.Append("\"\"");
            }

            return jsonTree.ToString();
        }


        /// <summary>
        /// 获取json字符
        /// </summary>
        public string josn()
        {
            //https://www.cnblogs.com/tlbxygw/p/2642086.html
            List<fruit> listAll = null;//查询全部数据，查询省略，封装到list

            List<zTree> listZtree = GetJsonTreefruit(listAll, 0);

            return JsonConvert.SerializeObject(listZtree);
        }

        public ActionResult josn2()
        {
            
            List<SysMenu> listAll = GetAllMenu();//查询全部数据，查询省略，封装到list

            //List<zTree> listZtree = GetJsonTreeMenu(listAll, -1);// 财务管理员
              List<zTree> listZtree = GetJsonTreeMenu(listAll, 0);
            return Json(listZtree, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 封装成ztree类
        /// </summary>
        /// <param name="listAll">全部数据list</param>
        /// <param name="parentid">根节点的id</param>
        /// <returns></returns>
        private List<zTree> GetJsonTreefruit(List<fruit> listAll, int parentid)
        {
            List<zTree> listTree = new List<zTree>();

            IEnumerable<fruit> list = listAll.Where(p => p.parent_id == parentid);//使用linq查询，必须重复查询数据库，数据量小时适用
            if (list.Count() > 0)
            {
                zTree ztree = null;

                foreach (fruit item in list)
                {
                    ztree = new zTree();
                    ztree.id = item.id.ToString();
                    ztree.pId = item.parent_id.ToString();
                    ztree.name = item.name;
                    List<zTree> listChildren = GetJsonTreefruit(listAll, item.id);
                    if (listChildren.Count > 0)
                    {
                        ztree.isParent = true;
                        ztree.children = listChildren;
                    }
                    else
                    {
                        ztree.isParent = false;
                        ztree.children = null;
                    }

                    listTree.Add(ztree);
                }
            }

            return listTree;
        }

        private List<zTree> GetJsonTreeMenu(List<SysMenu> listAll, int parentid)
        {
            List<zTree> listTree = new List<zTree>();

            IEnumerable<SysMenu> list = listAll.Where(p => p.ParentTid == parentid);//使用linq查询，必须重复查询数据库，数据量小时适用
            if (list.Count() > 0)
            {
                zTree ztree = null;

                foreach (SysMenu item in list)
                {
                    ztree = new zTree();
                    ztree.id = item.Tid.ToString();
                    ztree.pId = item.ParentTid.ToString();
                    ztree.name = item.MenuName;
                    List<zTree> listChildren = GetJsonTreeMenu(listAll, item.Tid);
                    if (listChildren.Count > 0)
                    {
                        ztree.isParent = true;
                        ztree.children = listChildren;
                    }
                    else
                    {
                        ztree.isParent = false;
                        ztree.children = null;
                    }

                    listTree.Add(ztree);
                }
            }

            return listTree;
        }
    }
    public class sys_device
    {
        public int id { get; set; }
        public int mocode { get; set; }
        public int categoryId { get; set; }
        public int pId { get; set; }
        public int isEnd { get; set; }
        public string name { get; set; }
        public string codename { get; set; }
        public bool isParent { get; set; }
    }
    public class ResponseEntity
    {
        public bool success = false;
        public string msg;
        public string list;
        public int row;
        public int index;
        public int total;
    }

    /// <summary>
    /// 节点实体模型类
    /// </summary>
    public class MyNodes
    {
        public int id { get; set; }
        public int pId { get; set; }
        public string iconOpen { get; set; }
        public string iconClose { get; set; }
        /// <summary>
        /// 展开
        /// </summary>
        public bool open { get; set; }
        /// <summary>
        /// 没有子节点
        /// </summary>
        public bool isParent { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string name { get; set; }

        public string icon { get; set; }
    }

    public class zNodes
    {
        /// <summary>
        /// 展开
        /// </summary>
        public bool open { get; set; }
        /// <summary>
        /// 没有子节点
        /// </summary>
        public bool isParent { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string name { get; set; }

        private List<zNodes> _children;
        /// 子节点集合 
        public List<zNodes> children
        {
            get
            {
                if (_children == null)
                {
                    return _children = new List<zNodes>();
                }
                return _children;
            }
            set
            {
                _children = value;
            }
        }
       }

    public class TreeNode
    {
        public int id { get; set; } //节点ID
        public string name { get; set; } //节点名称
        public bool isParent { get; set; } //是否是父节点
        public bool open { get; set; } //是否展开
    }

    /// <summary>
    /// 用来生成zTree需要的json格式，类属性与ztree配置节点对应
    /// </summary>
    public class zTree
    {
        public string id { get; set; }
        /// <summary>
        /// 父节点id
        /// </summary>
        public string pId { get; set; }
        public string name { get; set; }
        public bool isParent { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool open { get; set; } = true;
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<zTree> children { get; set; }
    }

    /// <summary>
    /// 水果类
    /// </summary>
    public class fruit
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parent_id { get; set; }
    }
}