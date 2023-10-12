using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplicationWZH.Models;

namespace WebApplicationWZH.Controllers
{
    //登录页加上此特性，不需要做登录验证，要不加会陷入死循环，导致浏览器崩溃...
    [SkipCheckLoginAttribute]
    public class LoginController : Controller
    {
        [HttpGet]
        //[AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "";

            return View();
        }
        [HttpGet]
        public ActionResult Index3()
        {
            //https://blog.csdn.net/qq_40741855/article/details/88840451

            return View();
        }
        [HttpGet]
        public ActionResult Index4()
        {
            // httpss://blog.csdn.net/weixin_45721343/article/details/103909071?spm=1001.2101.3001.6650.18&utm_medium=distribute.pc_relevant.none-task-blog-2%7Edefault%7EBlogCommendFromBaidu%7ERate-18-103909071-blog-88840451.235%5Ev38%5Epc_relevant_anti_vip_base&depth_1-utm_source=distribute.pc_relevant.none-task-blog-2%7Edefault%7EBlogCommendFromBaidu%7ERate-18-103909071-blog-88840451.235%5Ev38%5Epc_relevant_anti_vip_base&utm_relevant_index=23

            return View();
        }

        //生成验证码
        public ActionResult CreateValidCodeImage()
        {
            //1.生成长度为4的随机验证码字符串
            var strRandom = ValidCodeUtils.GetRandomCode(4);
            //2.根据生成的验证码字符串生成验证码图片
            byte[] byteImg = ValidCodeUtils.CreateImage(strRandom);
            //3.将验证码字符串存入session中
            Session["validCode"] = strRandom;
            //4.将图片返回到视图
            return File(byteImg, @"image/jpeg");
        }

 

        //[AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LogInViewModel logInView )
        {
            //ViewBag.Message = "Your application description page.";

            string validateCode = Session["validateCode"] == null ? string.Empty : Session["validateCode"].ToString();
            if (string.IsNullOrEmpty(validateCode))
            {
                ViewBag.Message = "验证码错误！";
                //return Content("验证码错误！");
                //return View();
            }

            ViewBag.Message = "";
            if (logInView.loginName == "admin" && logInView.loginPassword == "123")
            {
                //设置cookie
               
                FormsAuthentication.SetAuthCookie(logInView.loginName, false);

                logInView.Id = Guid.NewGuid().ToString("D");//为了测试手动设置一个用户id
                UserData userData = new UserData { 
                 UserId = logInView.Id, 
                 UserName = logInView.loginName,
                 Roles = new List<int> { 1,2,3,4 }
                };
                //FormsAuthHelp.AddFormsAuthCookie(logInView.Id, logInView, 60);//设置ticket票据的名称为用户的id，设置有效时间为60分钟
                FormsAuthHelp.AddFormsAuthCookie(logInView.Id, userData, 60);//设置ticket票据的名称为用户的id，设置有效时间为60分钟

                //return RedirectToAction("About");
                return RedirectToAction("SheetList", "Home");
            }
            //else
            //{
            //    return Redirect("Home/Index");

            //}
           
            //方式 1
            //return Content("<script>alert('账号密码不能为空');location.href='/Login/Index'</script>");

            ViewBag.Message = "用户名或密码错误！";
            //return View("~/Views/Login/Index.cshtml", model);
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthHelp.RemoveFormsAuthCookie();
            return Redirect("~/Login/Index");
            //return View("~/Home/Index");
        }

        /// <summary>
        /// 登录系统 验证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Index2(LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                //这里写个方法去数据库判断登录名和密码是否正确
                if (model.loginName == "admin" && model.loginPassword == "123")
                {
                    LoginInfo info = new LoginInfo();
                    info.OperatorID = 1;
                    info.OperatorNO = "mike";
                    info.OperatorName = "mikexu";
                    FormsAuthHelp.SignIn(info, false);

                    return Redirect(FormsAuthentication.DefaultUrl);
                }
                else
                {
                    ModelState.AddModelError("model.KeyValidateMessage", "登录名或密码错误(登录名:mike,密码:123456)");
                }
            }
            return View("~/Views/Login/Index.cshtml", model);
        }

        #region 将用户名存到cookie中
        /// <summary>
        /// 保存Cookie
        /// </summary>
        /// <returns></returns>
        public void CreateCookie()　　　//此Action自动往cookie里写入登录信息
        {
            HttpCookie UserName = new HttpCookie("name");
            UserName.Value = Request["userName"];
            System.Web.HttpContext.Current.Response.SetCookie(UserName);
            //cookie保存时间
            UserName.Expires = DateTime.Now.AddHours(10);

            //判断Cookie用户名密码是否存在
            HttpCookie cookieName = System.Web.HttpContext.Current.Request.Cookies.Get("name");
            if (cookieName == null)
            {
               // filterContext.Result = new RedirectResult("/Login/Index");
            }
 
        }
        #endregion

        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");

        }
        public ActionResult GetAuthCode2()
        {
            string code = ValidCodeUtils.GetRandomCode(4);
            return File(ValidCodeUtils.CreateImage(code), @"image/Gif");

        }
        public ActionResult CheckLogin(string username, string password, string code)
        {
            try
            {
                if (username == "admin")
                    return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
                else if (password == "123456")
                    return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
                else
                    return Content(new AjaxResult { state = ResultType.error.ToString(), message = "请验证帐号及密码！" }.ToJson());
            }

            catch (Exception ex)
            {
                return Content(new AjaxResult { state = ResultType.error.ToString(), message = ex.Message }.ToJson());
            }
        }

    }
    public class FormsAuthHelp
    {

        //FormsAuthentication类
        //string LoginUrl 用户访问且验证不通过时，重定向到的登录页面的URL
        //TimeSpan TimeOut 获取身份验证票证的到期前的时间量
        //void SetAuthCookie(string userName, bool createPersistentCookie) 为提供的用户名创建一个身份验证票据，并将该票据添加到响应的Cookie集合或Url中，常用于登录
        //viod SignOut()  从浏览器中删除Forms身份验证票据，常用语注销
        //string Encrypt(FormsAuthenticationTicket ticket)    将验证票据对象加密成一个字符串
        //FormsAuthenticationTicket Decrypt(string encryptedTicket)   将加密过的用户身份票据字符串解密成一个票据对象


        /// <summary>
        /// 将当前登入用户的信息生成ticket添加到到cookie中(用于登入)
        /// </summary>
        /// <param name="loginName">Forms身份验证票相关联的用户名(一般是当前用户的id，作为ticket的名称使用)</param>
        /// <param name="userData">用户信息</param>
        /// <param name="expireMin">有效期</param>
        public static void AddFormsAuthCookie(string loginName, object userData, int expireMin)
        {
            //将当前登入的用户信息序列化
            var data = JsonConvert.SerializeObject(userData);

            //创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
            var ticket = new FormsAuthenticationTicket(1,
             loginName, DateTime.Now, DateTime.Now.AddDays(1), true, data);

            //加密Ticket，变成一个加密的字符串。
            var cookieValue = FormsAuthentication.Encrypt(ticket);

            //根据加密结果创建登录Cookie
            //FormsAuthentication.FormsCookieName是配置文件中指定的cookie名称，默认是".ASPXAUTH"
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Domain = FormsAuthentication.CookieDomain,
                Path = FormsAuthentication.FormsCookiePath
            };
            //设置有效时间
            if (expireMin > 0)
                cookie.Expires = DateTime.Now.AddMinutes(expireMin);

            var context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException();

            //写登录Cookie
            context.Response.Cookies.Remove(cookie.Name);
            context.Response.Cookies.Add(cookie);

            HttpContext.Current.Session["User"] = data;
            HttpContext.Current.Session["UserRole"] = "admin,aa,bb";
        }
        /// <summary>
        /// 删除用户ticket票据
        /// </summary>
        public static void RemoveFormsAuthCookie()
        {
            HttpCookie cookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];
            cookie.Value = null;
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Set(cookie);

            if (HttpContext.Current.Session != null)
                HttpContext.Current.Session.Abandon();

            FormsAuthentication.SignOut();
        }

        //获取cookie并解析出用户信息
        public static Principal TryParsePrincipal(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            HttpRequest request = context.Request;
            HttpCookie cookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return null;
            }
            //解密coolie值
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            UserData account = JsonConvert.DeserializeObject<UserData>(ticket.UserData);


            return new Principal(ticket, account);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="remember"></param>
        public static void SignIn(LoginInfo info, bool remember = false)
        {
            DateTime timeoutDate = DateTime.Now.AddDays(7);
            FormsAuthenticationTicket frmTicket = new FormsAuthenticationTicket(
                   19,
                   "AuthConfig.NAME",
                   DateTime.Now,
                   timeoutDate,
                   false,
                   "GetUserData(info, AuthConfig.ROLE_ADMIN)");
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(frmTicket));
            if (remember)
                cookie.Expires = timeoutDate;

            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);

            info.LoginRole = "AuthConfig.ROLE_ADMIN";
            info.LoginName = "AuthConfig.NAME";
            HttpContext.Current.Session["AuthConfig.SESSION_LOGIN_KEY"] = info;
        }
    }

    public class UserData
    {
        /// <summary>
        /// ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<int> Roles { get; set; }
    }
    public class Principal : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public UserData Account { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="account"></param>
        public Principal(FormsAuthenticationTicket ticket, UserData account)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");
            if (account == null)
                throw new ArgumentNullException("UserData");

            this.Identity = new FormsIdentity(ticket);
            this.Account = account;
        }

        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return true;
            if (this.Account == null || this.Account.Roles == null)
                return false;
            return role.Split(',').Any(q => Account.Roles.Contains(int.Parse(q)));
        }
    }

    public class LoginInfo
    {
        public int OperatorID { get;  set; }
        public string OperatorName { get;  set; }
        public string OperatorNO { get;  set; }
        public object LoginRole { get;  set; }
        public object LoginName { get;  set; }
    }



}

/*
 * httpss://www.jb51.net/article/126684.htm
 提示：当检测到用户未登入，则跳转到web.config中配置的url页面，当用户填写密码并提交时，用户输入的数据会提交到LoginController控制器下的Login方法，
验证用户的输入，认证失败重新返回到登入界面，当认证成功，将会执行

<<***FormsAuthHelp.AddFormsAuthCookie(user.Id, user, 60);//设置ticket票据的名称为用户的id，设置有效时间为60分钟***>>,这条语句的作用是生成一个ticket票据，
并封装到cookie中，asp.net mvc正式通过检测这个cookie认证用户是否登入的，具体代码如下
 */