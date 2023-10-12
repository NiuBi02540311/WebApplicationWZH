using System;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
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
            filters.Add(new CheckLoginAttribute());

            //filters.Add(new Verify());

            //if (filterContext.HttpContext.Session["User"] == null || filterContext.HttpContext.Request.IsAuthenticated == false){
            //if(filterContext.HttpContext.Request.IsAjaxRequest()){

            //    filterContext.HttpContext.Response.AddHeader("StatusCode", "401");
            //    filterContext.HttpContext.Response.StatusCode = 401;//应将状态代码设置为401(未授权)
            //    filterContext.HttpContext.Response.End();

            //}
            //else
            //{
            //    //判断访问权限

            //    if (ControllerNameActionName == "/home/test")
            //    {
            //        filterContext.HttpContext.Response.AddHeader("StatusCode", "302");
            //        filterContext.HttpContext.Response.End();
            //    }
            //    else
            //    {
            //        base.OnActionExecuted(filterContext);
            //    }


            //}
            //filterContext.HttpContext.Response.Redirect("/Login/Index");
            //return;
            //}
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
           
            if (filterContext.HttpContext.Session["User"] == null || filterContext.HttpContext.Request.IsAuthenticated == false )
            {
                if(filterContext.HttpContext.Request.IsAjaxRequest()){

                   filterContext.HttpContext.Response.AddHeader("StatusCode", "401");
                   filterContext.HttpContext.Response.StatusCode = 401;//应将状态代码设置为401(未授权)
                   filterContext.HttpContext.Response.End();

                }

                //跳转方法1：
                filterContext.HttpContext.Response.Redirect("/Login/Index");
                //跳转方法2：
                //ViewResult view = new ViewResult();
                //指定要返回的完整视图名称
                //view.ViewName = "~/View/Login/Login.cshtml";
                return;
            }

            //admin,aa,bb
            string UserRole = filterContext.HttpContext.Session["UserRole"] as string;
            if (UserRole == "admin" || UserRole.StartsWith("admin,") || UserRole.EndsWith(",admin") || UserRole.Contains(",admin,"))
            {
                //管理员不验证权限
                return;
            }
            UserRole = string.Empty;
            //ajax 请求判断访问权限
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //var request = filterContext.HttpContext.Request;
                if (ControllerNameActionName == "/home/test")
                {
                    //filterContext.HttpContext.Response.AddHeader("StatusCode", "302");
                    //filterContext.HttpContext.Response.End();
                    //return;
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


}
