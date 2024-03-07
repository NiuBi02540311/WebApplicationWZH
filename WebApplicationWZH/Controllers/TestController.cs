using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationWZH.Controllers
{
    [SkipCheckLogin]
    public class TestController : Controller
    {
        public TestController() {

            string aa = DateTime.Now.ToString();
            string a = DateTime.Now.AddHours(1).ToString();
            string aaaa = DateTime.Now.AddMinutes(3).ToString();
            string aaa = DateTime.Now.AddHours(1).ToString();
        }
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Demo1()
        {
            return View();
        }
        public ActionResult Demo2()
        {
            return View();
        }
        public ActionResult Demo3()
        {
            return View();
        }
        public ActionResult Demo4()
        {
            return View();
        }
        public ActionResult Demo5()
        {
            return View();
        }
        [HttpPost]
        [MyAuthorize]
        public string About()
        {
            string rtJson = "{\"code\": 0}";
            try
            {

                rtJson = "{\"code\":0,\"data\":[],\"msg\":\"Your application description page.\",\"count\":1}";
            }
            catch
            {
                rtJson = "{\"code\": 0}";
            }
            return rtJson;
        }


        [HttpGet]
        public ActionResult GetToken()
        {
            string token = JwtHelp.SetJwtEncode(null);
            return Json(new { token = token }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SayToken(string token)
        {
            string Message;
            var u = JwtHelp.GetJwtDecode(token,out Message);
            return Json(new { UserInfo = u , Message = Message }, JsonRequestBehavior.AllowGet);
        }
    }

    public class UserInfo
    {
        public string username { get; set; }

        public string pwd { get; set; }

        public string time { get; set; } 

        public double exp { get; set; }
    }
    public class JwtHelp
    {
        //https://www.jb51.net/article/207569.htm
        //私钥 web.config中配置
        //"GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private static string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        //ConfigurationManager.AppSettings["Secret"].ToString();

        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <param name="payload">不敏感的用户数据</param>
        /// <returns></returns>
        public static string SetJwtEncode(Dictionary<string, object> payload)
        {
            /*
             * httpss://www.cnblogs.com/refuge/p/9069932.html
             （3）添加过期时间，过期时间即这个时间之后JWT不接受处理，时间的有效值为某一时刻和1970/1/1 00:00：00 相差的秒数
                下面的例子是当前时间到1970/1/1 00：00：00 的秒数，即过期时间为当前时间。如果设置为当前时间+10秒，
                可添加secondsSinceEpoch=secondsSinceEpoch+10
             */

            IDateTimeProvider provider = new UtcDateTimeProvider();
            var now = provider.GetNow();

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds) + 60;  // 60秒过期

            //格式如下
            var payload2 = new Dictionary<string, object>
            {
             { "username","admin" },
             { "pwd", "claim2-value" },
             { "time", DateTime.Now.ToString() },
             { "exp",  secondsSinceEpoch}
             
               
            };
            // expires:DateTime.Now.AddDays(1),//当前时间加一小时，一小时后过期
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload2, secret);
            return token;
        }

        /// <summary>
        /// 根据jwtToken 获取实体
        /// </summary>
        /// <param name="token">jwtToken</param>
        /// <returns></returns>
        public static UserInfo GetJwtDecode(string token,out string Message)
        {
            try
            {
                Message = "";
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                var algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                var userInfo = decoder.DecodeToObject<UserInfo>(token, secret, verify: true);//token为之前生成的字符串
                return userInfo;
            }
            //catch(Exception ex)
            //{
            //    Message = ex.Message;
            //    return null;
            //}
            catch (TokenExpiredException)
            {
                Message = "Token has expired";
                Console.WriteLine("Token has expired");
                return null;
            }
            catch (SignatureVerificationException)
            {
                Message = "Token has invalid signature";
                Console.WriteLine("Token has invalid signature");
                return null;
            }


        }

        public string JwtDecode(string jwttoken, string publickey)
        {
            try
            {
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                var json = decoder.Decode(jwttoken, publickey, verify: true);
                return json;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }

    // JWT 加密解密类
    public class JwtTest
    {
        //public string JwtEncode(string keyvalue, Claim[] claims)
        //{
        //    var key = Encoding.UTF8.GetBytes(keyvalue);
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {

        //        Subject = new ClaimsIdentity(claims),
        //        Expires = DateTime.UtcNow.AddSeconds(10),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var tokenString = tokenHandler.WriteToken(token);
        //    Console.WriteLine($"加密后的JWT: {tokenString}");
        //    return tokenString;
        //}
        public string JwtDecode(string jwttoken, string publickey)
        {
            try
            {
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                var json = decoder.Decode(jwttoken, publickey, verify: true);
                return json;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}