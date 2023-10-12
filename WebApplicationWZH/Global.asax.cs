using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using WebApplicationWZH.Controllers;

namespace WebApplicationWZH
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PostAuthenticateRequest(object sender, System.EventArgs e)
        {
            //4.在Global.asax中注册Application_PostAuthenticateRequest事件，保证权限验证前将cookie中的用户信息取出赋值给User
            //HttpContext.Current.User = FormsAuthHelp.TryParsePrincipal(HttpContext.Current);
        }
    }
}
