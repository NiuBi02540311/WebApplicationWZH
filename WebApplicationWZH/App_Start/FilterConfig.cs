using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using WebApplicationWZH.Controllers;

namespace WebApplicationWZH
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());


            //将内置的权限过滤器添加到全局过滤中
            // filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            //全局注册filter
            //filters.Add(new FormAuthorizeAttribute());

            filters.Add(new CheckLoginAttribute());// 例子1

            //filters.Add(new Verify());

            //filters.Add(new UrlAuthorizeAttribute());// 例子2 2023.10.13
            
        }
    }

    public class Verify : AuthorizeAttribute
    {
        //原文链接：https://blog.csdn.net/sanpi199274/article/details/105227980
        public override void OnAuthorization(AuthorizationContext filterContext)

        {
            var user = filterContext.HttpContext.Session["CurrentUser"];
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            //判断是否Action判断是否跳过授权过滤器
            {
                return;
            }
            else if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            //判断是否Controller判断是否跳过授权过滤器
            {
                return;
            }
            else if (user == null || string.IsNullOrWhiteSpace(user.ToString()))
            //判断用户是否登录
            {
                filterContext.Result = new RedirectResult("../Login/Login");
            }
            else
            {
                return;
            }
        }


    //控制器授权验证
    //[Verify]
    //public class LoginController : Controller { }
    }

    /// <summary>
    /// 在Filters文件夹，添加一个类SkipVerification，继承Attribute，主要用来跳过Action访问权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SkipVerification : Attribute
    {
    }
    /// <summary>
    /// 统一登陆验证
    /// </summary>
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        // /// OnActionExecuting方法在Controller的Action执行前执行
        //  1.OnActionExecuting  
        //     在Action方法调用前使用，使用场景：如何验证登录等。
        // 2.OnActionExecuted
        //    在Action方法调用后，result方法调用前执行，使用场景：异常处理。
        // 3.OnResultExecuting
        //   在result执行前发生(在view 呈现前)，使用场景：设置客户端缓存，服务器端压缩.
        //4.OnResultExecuted
        //  在result执行后发生，使用场景：异常处理，页面尾部输出调试信息

        //重写ActionFilterAttribute中的 OnActionExecuted 方法，表示在执行Action之前执行此方法
        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            string ActionName = filterContext.ActionDescriptor.ActionName.ToLower();
            string ControllerNameActionName = $"/{ControllerName}/{ActionName}";

            //循环获取在Controller的Action方法中定义的参数
            var arr = filterContext.ActionDescriptor.GetParameters();
            foreach (var parameter in arr)
            {
                var parameterName = parameter.ParameterName;//获取Action方法中参数的名字
                var parameterType = parameter.ParameterType;//获取Action方法中参数的类型

                //判断该Controller的Action方法是否有类型为LoginLogoutRequest的参数
                //if (parameterType == typeof(LoginLogoutRequest))
                //{
                //    //如果有，那么就获取LoginLogoutRequest类型参数的值
                //    var loginLogoutRequest = context.ActionArguments[parameterName] as LoginLogoutRequest;

                //    var username = loginLogoutRequest.Username;
                //    var password = loginLogoutRequest.Password;
                //}
            }

            //https://www.cnblogs.com/dragon2017/p/11718769.html

            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            //判断是否Action判断是否跳过授权过滤器
            {
                return;
            }
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            //判断是否Controller判断是否跳过授权过滤器
            {
                return;
            }
 
            //判断Action方法的Control是否跳过登录验证
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false))
            {
                return;
            }
            //判断Action方法是否跳过登录验证
            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false))
            {
                return;
            }

            // 1、允许匿名访问 用于标记在授权期间要跳过 AuthorizeAttribute 的控制器和操作的特性 
            var actionAnonymous = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true) as IEnumerable<AllowAnonymousAttribute>;
            var controllerAnonymous = filterContext.Controller.GetType().GetCustomAttributes(typeof(AllowAnonymousAttribute), true) as IEnumerable<AllowAnonymousAttribute>;
            if ((actionAnonymous != null && actionAnonymous.Any()) || (controllerAnonymous != null && controllerAnonymous.Any()))
            {
                return;
            }
 
            if (filterContext.HttpContext.Session["User"] == null || filterContext.HttpContext.Request.IsAuthenticated == false )
            {
                bool bAjax = filterContext.HttpContext.Request.IsAjaxRequest();
                if (bAjax)
                {

                    //Ajax 输出错误信息给脚本
                   filterContext.Result = AjaxError(filterContext);
                   filterContext.HttpContext.Response.AddHeader("StatusCode", "401");
                   filterContext.HttpContext.Response.StatusCode = 401;//应将状态代码设置为401(未授权)
                   filterContext.HttpContext.Response.End();
                    return;
                }

                if (bAjax)
                {
                    //https://blog.csdn.net/XR1986687846/article/details/89346166
                    //BusinessResultBase result = new BusinessResultBase();
                    //result.Title = "未登录或登录已超时";
                    //result.Status = false;
                    //result.StatusCode = 401;
                    //result.StatusMessage = "请重新登录系统。";

                    //var jsonResult = new JsonResult();
                    //jsonResult.Data = result;
                    //filterContext.Result = jsonResult;
                    //return;
                }
 
                //看上面的代码，假如Session为空是乎就会跳转，但事实上接下去会继续执行你的ActionResult,执行完了之后才会跳转！很可能你的ActionResult中调用Session就会出错！
                //解决办法：
                filterContext.Result = new HttpUnauthorizedResult(); // 返回未授权Result
                //filterContext.Result = new RedirectResult("~/Login/Index"); // 返回未授权Result

                //跳转方法1：
                // filterContext.HttpContext.Response.Redirect("/Login/Index");
                //跳转方法2：
                //ViewResult view = new ViewResult();
                //指定要返回的完整视图名称
                //view.ViewName = "~/View/Login/Login.cshtml";

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index", returnUrl = filterContext.HttpContext.Request.Url, returnMessage = "NoLogin您无权查看" }));
                

                return;
            }

            //判断Action方法的Control是否跳过权限验证
            
           if (filterContext.ActionDescriptor.IsDefined(typeof(SkipVerification), false))
           { 
              return;
            }

            //例外情况(不验证权限)
            var skipVerifications = filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipVerification), true);
            if (skipVerifications != null && skipVerifications.Length > 0)
            {
                return;
            }
              


            //admin,aa,bb
            string UserRole = filterContext.HttpContext.Session["UserRole"] as string;
            if (UserRole == "admin" || UserRole.StartsWith("admin,") || UserRole.EndsWith(",admin") || UserRole.Contains(",admin,"))
            {
                //管理员不验证权限
                //return;
            }
            // role = 1 代表是超级管理员
            if (UserRole.Split(',').Contains("1"))
            {
                //管理员不验证权限
                //return;
            }
            //UserRole = string.Empty;
            //ajax 请求判断访问权限
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //var request = filterContext.HttpContext.Request;
                if (ControllerNameActionName == "/home/test")
                {
                    filterContext.HttpContext.Response.AddHeader("StatusCode", "302");
                    filterContext.HttpContext.Response.End();
                    return;
                }
            }

            
            
             //判断访问权限
            if (ControllerNameActionName == "/home/index")
            {
                //filterContext.HttpContext.Response.AddHeader("StatusCode", "302");
                //filterContext.HttpContext.Response.End();
                ContentResult result = new ContentResult();
                result.Content = $"<div style='text-align:center;padding:1em;' >当前地址（{ControllerNameActionName}）没有访问权限</div>";
                //filterContext.Result = new ContentResult() { Content = "抱歉,登录异常！", ContentEncoding = System.Text.Encoding.UTF8 };
               
                //filterContext.Result = result;
                //return;
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// OnActionExecuted方法在Controller的Action执行后执行 ddd
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext OnActionExecuted)
        {
            //TODO
            base.OnActionExecuted(OnActionExecuted);
        }
        /// <summary>
        /// Ajaxes the error.
        /// </summary>
        /// <param name="filterContext">过滤器上下文</param>
        /// <returns>返回json串</returns>
        protected JsonResult AjaxError(ActionExecutingContext filterContext)
        {
            //将响应状态代码设置为500
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            return new JsonResult
            {
                Data = new { Code = 10001, Msg = "" },
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }
        private string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }

 
    }

    public class MyValidateAntiForgeryToken : AuthorizeAttribute
    {
        //httpss://www.cnblogs.com/Qintai/p/11828220.html
        //httpss://blog.csdn.net/weixin_42297982/article/details/119506621
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            //if (request.HttpMethod == WebRequestMethods.Http.Post && request.Url.Host.ToLower() != WebConfigBLL.LIVE_VZAN_DOMAIN.ToLower())
            if (request.IsAjaxRequest()){

                if (request.HttpMethod == WebRequestMethods.Http.Post)
                {
                    HttpCookie antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];
                    string cookieValue = antiForgeryCookie != null ? antiForgeryCookie.Value : null;
                    //从cookies 和 Headers 中 验证防伪标记 如果验证不通过会抛出异常

                    try
                    {
                        // AntiForgery.Validate在NetCore中没有的
                        //AntiForgery.Validate(cookieValue, request["__RequestVerificationToken"]);//验证 HTML 表单字段中的输入数据是否来自已提交数据的用户。
                        AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
                    }
                    catch (Exception ex)
                    {
                        ContentResult result = new ContentResult();
                        //result.Content = "<div style='text-align:center;padding:1em;' >当前已经处于退出状态，请重新登录</div>";
                        result.Content = "当前已经处于退出状态，请重新登录";
                        //filterContext.Result = new ContentResult() { Content = "抱歉,登录异常！", ContentEncoding = System.Text.Encoding.UTF8 };

                        filterContext.Result = result;
                        //filterContext.HttpContext.Response.StatusCode = 401;//应将状态代码设置为401(未授权)
                        filterContext.HttpContext.Response.AddHeader("StatusCode", "401");
                        return;
                    }
                }
                else
                {
                    throw new Exception("没有权限");
                    //base.HandleUnauthorizedRequest(filterContext);
                }
            }
            else
            {
                new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
            }
           
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ValidateAntiForgeryTokenOnAllPosts : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            // Only validate POSTs
            if (request.HttpMethod == WebRequestMethods.Http.Post)
            {
                // Ajax POSTs and normal form posts have to be treated differently when it comes
                // to validating the AntiForgeryToken
                if (request.IsAjaxRequest())
                {
                    var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];

                    var cookieValue = antiForgeryCookie != null
                    ? antiForgeryCookie.Value
                    : null;

                    try
                    {

                        AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
                    }
                    catch (Exception e)
                    {
                        //filterContext.Result = new RedirectResult("/Account/Login?returnUrl=" +
                        // HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.ToString()));

                        ContentResult result = new ContentResult();
                        result.Content = "<div style='text-align:center;padding:1em;' >当前已经处于退出状态，请重新登录</div>";
                        filterContext.Result = result;

                    }
                }
                else
                {
                    new ValidateAntiForgeryTokenAttribute()
                    .OnAuthorization(filterContext);
                }
            }
            else
            {
                throw new Exception("没有权限");
                //base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
    //要继承所有属性的基类Attribute
    public class SkipCheckLoginAttribute : System.Attribute
    {

    }
    public class FormAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 先进入此方法，此方法中会调用 AuthorizeCore 验证逻辑，验证不通过会调用 HandleUnauthorizedRequest 方法
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User as Principal;
            if (user != null)
                return user.IsInRole(base.Roles);
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //验证不通过，直接跳转到相应页面，注意：如果不是哟娜那个以下跳转，则会继续执行Action方法
            filterContext.Result = new RedirectResult("~/Login/Index");
        }
    }


    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        //我们都知道ASP.net mvc权限控制都是实现AuthorizeAttribute类的OnAuthorization方法
        //https://blog.csdn.net/VisageNocturne/article/details/112265239
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var isAuth = false;
            if (!filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
            {
                isAuth = false;
            }
            else
            {
                if (filterContext.RequestContext.HttpContext.User.Identity != null)
                {
                    var roleService = new RoleService();
                    var actionDescriptor = filterContext.ActionDescriptor;
                    var controllerDescriptor = actionDescriptor.ControllerDescriptor;
                    var controller = controllerDescriptor.ControllerName;
                    var action = actionDescriptor.ActionName;
                    var ticket = (filterContext.RequestContext.HttpContext.User.Identity as FormsIdentity).Ticket;
                    var role = roleService.GetById(ticket.Version);
                    //if (role != null)
                    {
                        //isAuth = role.Permissions.Any(x => x.Permission.Controller.ToLower() == controller.ToLower() && x.Permission.Action.ToLower() == action.ToLower());
                    }
                }
            }
            if (!isAuth)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "account", action = "login", returnUrl = filterContext.HttpContext.Request.Url, returnMessage = "您无权查看." }));
                return;
            }
            else
            {
                base.OnAuthorization(filterContext);
            }
        }
    }

    public class PermissionDefinition
    {
        public virtual int Id
        {
            set;
            get;
        }
        public virtual int ActionNo
        {
            set;
            get;
        }

        public virtual int ControllerNo
        {
            set;
            get;
        }
        public virtual string Name
        {
            set;
            get;
        }

        public virtual string ControllerName
        {
            set;
            get;
        }
        public virtual string Controller
        {
            set;
            get;
        }
        public virtual string Action
        {
            set;
            get;
        }
        public virtual DateTime AddDate
        {
            set;
            get;
        }
    }

    public class RoleService
    {
        public int GetById(int v)
        {
            return 1;
        }
    }

    
    /// <summary>
    /// URL permission
    /// </summary>
    public class UrlAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Rewrite OnAuthorization
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            //判断是否Action判断是否跳过授权过滤器
            {
                return;
            }
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            //判断是否Controller判断是否跳过授权过滤器
            {
                return;
            }

            //判断Action方法的Control是否跳过登录验证
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false))
            {
                return;
            }
            //判断Action方法是否跳过登录验证
            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false))
            {
                return;
            }

            //Get permission list
            List<PermissionItem> pItems = AccountHelper.GetPermissionItems();

            //Get current page permission ID,if items is null,the page you what to access has not been configed.
            //filterContext.HttpContext.Request.Path = Login/Index
            var item = pItems.FirstOrDefault(c => c.Route == filterContext.HttpContext.Request.Path);

            if (item != null)
            {
                int[] permissions = AccountHelper.GetUserPermission(int.Parse(filterContext.HttpContext.Session["UserID"].ToString()));
                if (Array.IndexOf<Int32>(permissions, item.PermissionID) == -1)
                {
                    //have not permission
                    filterContext.HttpContext.Response.Write("You have no permission to access this page.");
                    filterContext.HttpContext.Response.End();
                }
            }
            else
            {
                //the page you what to access has not been configed.
                filterContext.HttpContext.Response.Write("The page you want to access has not been configed permission.");
                filterContext.HttpContext.Response.End();
            }
            base.OnAuthorization(filterContext);
        }
    }
    
}
