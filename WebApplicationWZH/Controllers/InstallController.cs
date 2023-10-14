using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebApplicationWZH.Models;

namespace WebApplicationWZH.Controllers
{
    [SkipCheckLogin]
    public class InstallController : Controller
    {
        [SkipVerification]
        public ActionResult ControllerList()
        {
            var roleService = new RoleService();
            #region init permission
            string nspace = "WebApplicationWZH.Controllers"; // WebApplicationWZH.Controllers...
            var q = from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && t.Namespace == nspace && t.FullName.EndsWith("Controller") select t;
            var controllers = q.ToList();
            //q.ToList().ForEach(
            //    t => Console.WriteLine(t.Name)
            //    );
            foreach (var c in controllers)
            {
                createPermission(c);
            }
            createPermission(new UserController());
            #endregion

            //var allDefinedPermissions = roleService.GetDefinedPermissions();
            #region 超级管理员角色初始化
            //var adminPermissions = new List<RolePermissionInfo>();
            //foreach (var d in allDefinedPermissions)
            //{
            //    adminPermissions.Add(new RolePermissionInfo { AddDate = DateTime.Now, Permission = d, });
            //}
            //int adminRoleId = roleService.AddRole(new Entities.RoleInfo
            //{
            //    AddDate = DateTime.Now,
            //    Description = "",
            //    Name = "超级管理员",
            //    Permissions = adminPermissions
            //});
            #endregion
            return RedirectToAction("Success");
        }

        [SkipVerification]
        public ActionResult Index()
        {

             
            string nspace = "WebApplicationWZH.Controllers"; // WebApplicationWZH.Controllers...
            var q = from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && t.Namespace == nspace && t.FullName.EndsWith("Controller") select t;
            var controllers = q.ToList();
            //q.ToList().ForEach(
            //    t => Console.WriteLine(t.Name)
            //    );
            List<ControllerArry> controllerArry = new List<ControllerArry>();
            foreach (var c in controllers)
            {

                
               
                List<ControllerItem> controllerItem = new List<ControllerItem>();
                foreach (var m in c.GetMethods())
                {
                    Type actionType = m.ReturnType;//https://www.cnblogs.com/dreamman/p/4932551.html
                    string ReturnParameter = actionType.FullName;
                    if (actionType.IsSubclassOf(typeof(JsonResult)))
                    {
                        ReturnParameter = "JsonResult";
                    }
                    if (actionType.IsSubclassOf(typeof(System.Net.Http.HttpResponseMessage)))
                    {
                        ReturnParameter = "HttpResponseMessage";
                    }
                    if (actionType.IsSubclassOf(typeof(ActionResult)))
                    {
                        ReturnParameter = "ActionResult";
                    }
                    //if (actionType.IsDefined(typeof(ActionResult),false))
                    //{
                    //    ReturnParameter = "ActionResult";
                    //}
                    //if (actionType.IsDefined(typeof(System.Net.Http.HttpResponseMessage), false))
                    //{
                    //    ReturnParameter = "HttpResponseMessage";
                    //}

                    if (ReturnParameter.Contains("ActionResult") || ReturnParameter.Contains("HttpResponseMessage"))
                    {
                        //ReturnParameter = "ActionResult";
                        //continue;
                    }
                    else
                    {
                        continue;
                    }
                     

                    if (!m.IsPublic) continue;
                    bool IsPost = m.IsDefined(typeof(HttpPostAttribute), false);
                    string HttpAttribute = IsPost ? "Post" : "Get";
                    string action = m.Name;//Index
                    //if( m.ReturnParameter is HttpResponseMessage
                    //string ReturnParameter = m.ReturnParameter.Name;
                   
                    controllerItem.Add(new ControllerItem { ActionName = action, ActionType = HttpAttribute });
                }
                string controller = c.Name.Replace("Controller", "");//remobe controller posfix
                controllerArry.Add(new ControllerArry { controlleName = controller, controllerItem = controllerItem });
            }
           
            
            return View(controllerArry);
        }
        private void createPermission(Controller customController)
        {
            var roleService = new RoleService();

            var controllerName = "";
            var controller = ""; var controllerNo = 0;
            var actionName = ""; var action = ""; var actionNo = 0;
            var controllerDesc = new KeyValuePair<string, int>();

            var controllerType = customController.GetType();
            controller = controllerType.Name.Replace("Controller", "");//remobe controller posfix
            controllerDesc = getdesc(controllerType);
            if (!string.IsNullOrEmpty(controllerDesc.Key))
            {
                controllerName = controllerDesc.Key;
                controllerNo = controllerDesc.Value;
                foreach (var m in controllerType.GetMethods())
                {
                    var mDesc = getPropertyDesc(m);
                    if (string.IsNullOrEmpty(mDesc.Key)) continue;
                    action = m.Name;
                    actionName = mDesc.Key;
                    actionNo = mDesc.Value;
                    //roleService.CreatePermissions(actionNo, controllerNo, actionName, controllerName, controller, action);
                    CreatePermissions(actionNo, controllerNo, actionName, controllerName, controller, action);
                }
            }
        }
        private void createPermission(Type t)
        {
            var roleService = new RoleService();

            var controllerName = "";
            var controller = ""; var controllerNo = 0;
            var actionName = ""; var action = ""; var actionNo = 0;
            var controllerDesc = new KeyValuePair<string, int>();

            var controllerType = t;
            string HttpAttribute = "";// post or get
            controller = controllerType.Name.Replace("Controller", "");//remobe controller posfix
            controllerDesc = getdesc(controllerType);
            if (!string.IsNullOrEmpty(controllerDesc.Key))
            {
                controllerName = controllerDesc.Key;
                controllerNo = controllerDesc.Value;
                foreach (var m in controllerType.GetMethods())
                {
                    bool IsPost = m.IsDefined(typeof(HttpPostAttribute), false);
                    HttpAttribute = IsPost ? "Post" : "Get";
                    var mDesc = getPropertyDesc(m);
                    if (string.IsNullOrEmpty(mDesc.Key)) continue;
                    action = m.Name;//Index
                    actionName = mDesc.Key;//用户首页
                    actionNo = mDesc.Value;//1
                    //roleService.CreatePermissions(actionNo, controllerNo, actionName, controllerName, controller, action);
                    CreatePermissions(actionNo, controllerNo, actionName, controllerName, controller, action);
                }
            }

            //下面面只是httpPost的例子，其他都一样，主要是获取控制器中的所有定义了HttpPost等特性的方法
            //var controllerType = typeof(HomeController);
            //var httpPostMethods = from method in controllerType.GetMethods()
            //                      where method.IsDefined(typeof(HttpPostAttribute), false)
            //                      select method.Name;
            //foreach (var methodName in httpPostMethods)
            //{
            //    Debug.WriteLine(methodName);
            //}
        }
        /// <summary>
        /// 将Persion存入数据库
        /// </summary>
        /// <param name="actionNo"></param>
        /// <param name="controllerNo"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        private void CreatePermissions(int actionNo, int controllerNo, string actionName, string controllerName, string controller, string action)
        {
            //throw new NotImplementedException();
        }
        private KeyValuePair<string, int> getdesc(Type type)
        {
            var descriptionAttribute = (DescriptionAttribute)(type.GetCustomAttributes(false).FirstOrDefault(x => x is DescriptionAttribute));
            if (descriptionAttribute == null) return new KeyValuePair<string, int>();
            return new KeyValuePair<string, int>(descriptionAttribute.ActionTitle, descriptionAttribute.ActionOrderNumber);
        }
        private KeyValuePair<string, int> getPropertyDesc(System.Reflection.MethodInfo type)
        {
            var descriptionAttribute = (DescriptionAttribute)(type.GetCustomAttributes(false).FirstOrDefault(x => x is DescriptionAttribute));
            if (descriptionAttribute == null) return new KeyValuePair<string, int>();
            return new KeyValuePair<string, int>(descriptionAttribute.ActionTitle, descriptionAttribute.ActionOrderNumber);
        }

        public ActionResult Success()
        {
            return View();
        }

        private List<Type> GetList()
        {
            List<Type> types = new List<Type>();
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace == "Namespace")

                    types.Add(type);
            }
            return types;
        }
    }
}