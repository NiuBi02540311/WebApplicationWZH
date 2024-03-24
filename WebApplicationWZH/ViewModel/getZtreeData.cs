
using System.Collections.Generic;
using System.Data;


namespace WebApplicationWZH.ViewModel
{
 public class zTree
    {
        /// <summary>
        /// layUI 树控件 https://www.layui.com/doc/modules/tree.html
        /// </summary>
        public int id { get; set; }
        public string name { get; set; }
        public int pid { get; set; }

        public bool @checked { get; set; }//节点是否初始展开，默认 false

       
        public bool open { get; set; }//节点是否初始展开，默认 false
        public List<zTree> children { get; set; }
    }
    public class getZtreeData
    {
        /// <summary>
        /// 获取所有菜单数据，用于zTree树形结构展示
        /// </summary>
        /// <returns></returns>
        /// https://www.cnblogs.com/hanzhaoxin/p/4232572.html
        public string GetRoleMenuList(string RoleID="")
        {
            //if (RoleID != "") {
            //    getRoleMenuData(RoleID);
            //}

            //List<SysMenu> lc = new SysMenuControl().SysMenuList.Where(p=> p.IsShowTree==true).ToList();

            //List<zTree> ls = new List<zTree>();
            //foreach (var v in lc)
            //{

            //    if (RoleID == "") {

            //        ls.Add(new zTree { id = v.MenuID, name = v.MenuName, pid = v.MenuPID, open = true, @checked = false });
            //    }
            //    else
            //    {
            //        ls.Add(new zTree { id = v.MenuID, name = v.MenuName, pid = v.MenuPID, open = true, @checked = getIsChecked(v.MenuID.ToString()) });
            //    }

            //}

            //var dictMenus = new Dictionary<int, zTree>(ls.Count);

            //foreach (var menu in ls)
            //{
            //    dictMenus.Add(menu.id, menu);
            //}

            //foreach (var value in dictMenus.Values)
            //{
            //    if (dictMenus.ContainsKey(value.pid))
            //    {
            //        if (dictMenus[value.pid].children == null)
            //            dictMenus[value.pid].children = new List<zTree>();
            //        dictMenus[value.pid].children.Add(value);
            //    }
            //}
            //var result = dictMenus.Values.Where(t => t.pid == 0).ToList();

            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            ////return Json(result, JsonRequestBehavior.AllowGet);
            //return json;

            return "";
        }

        private bool getIsChecked(string MenuID = "") {


            if (getRoleMenuDataList.IndexOf(MenuID) != -1)
            {
                return true;
            }
            else {
                return false;
            }

           
        }

        private List<string> getRoleMenuDataList;
        private void getRoleMenuData(string RoleID) {


            string sql = string.Format(@"SELECT [MenuID] FROM [dbo].[tb_RoleMenu] where RoleID={0} and is_delete=0", RoleID);

            DataTable dt = new DataTable();
           // dt = new DB.SqlDbHelper().Mssql_Get_DataTable(sql);
           

            if (dt != null)
            {
                getRoleMenuDataList = new List<string>();

                for (int i = 0; i < dt.Rows.Count; i++) {
                    getRoleMenuDataList.Add(dt.Rows[i]["MenuID"].ToString());
                }
            }
           
        }
    }
}