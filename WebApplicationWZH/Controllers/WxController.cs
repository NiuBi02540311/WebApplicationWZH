using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace WebApplicationWZH.Controllers
{
    [SkipCheckLogin]
    public class WxController : Controller
    {
        // GET: Wx
        public ActionResult Index()
        {
            return View();
        }
        #region weixin 小程序接口  wx 8402e983cc0607 df / xx00
        public ActionResult Demo5(string name)
        {

            string access_token = HttpContext.Request.Headers["access_token"];
            var obj = new { code = 1, Message = $"HttpGet hello 2024:  {name},{access_token},{DateTime.Now.ToString()}" };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Demo6(string name)
        {

            string access_token = HttpContext.Request.Headers["access_token"];
            var obj = new { code = 1, Message = $"HttpPost hello 2024:  {name},{access_token},{DateTime.Now.ToString()}" };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getAppsByCategory()
        {

            List<Category> Category = new List<Category>();
            Category.Add(new Controllers.Category { url_mid = "http://localhost:57526/Content/images/1.jpg", page = "/pkgA/pages/dog/dog" });
            Category.Add(new Controllers.Category { url_mid = "http://localhost:57526/Content/images/2.jpg", page = "/pkgA/pages/cat/cat" });
            Category.Add(new Controllers.Category { url_mid = "http://localhost:57526/Content/images/3.jpg", page = "/pkgB/pages/apple/apple" });
            //Category.Add(new Controllers.Category { url_mid = "http://localhost:57526/Content/images/4.jpg", page = "/pkgA/pages/dog/dog" });
            return Json(Category, JsonRequestBehavior.AllowGet);


        }
        public ActionResult checkSessionKey(string access_token, string openid, string signature)
        {
            //access_token
            //openid	
            //signature
            string sig_method = "hmac_sha256";
            //{"openid": "xxxxxxxxx", "signature": "xxxxx", "sig_method": "hmac_sha256"}

            //客户端请求
            HttpClient http = new HttpClient();
            //请求地址
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=xx00&secret=a189e38e324c1782321d0d1f01bcfdd9";
            //异步请求
            Task<string> task = http.GetStringAsync(url);
            //获取数据解析，并发送至前台
            //ViewBag.ResultTyle = JsonConvert.DeserializeObject<ResultType>(task.Result);
            var obj = new { code = 1, Message = task.Result };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getAccessToken2()
        {
            //http://localhost:57526/Test/getAccessToken2

            //客户端请求
            HttpClient http = new HttpClient();
            //请求地址
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=xx00&secret=a189e38e324c1782321d0d1f01bcfdd9";
            //异步请求
            Task<string> task = http.GetStringAsync(url);
            //获取数据解析，并发送至前台
            //ViewBag.ResultTyle = JsonConvert.DeserializeObject<ResultType>(task.Result);
            var obj = JsonConvert.DeserializeObject<AccessToken>(task.Result);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getAccessToken()
        {
            //https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET
            //https://developers.weixin.qq.com/miniprogram/dev/OpenApiDoc/mp-access-token/getAccessToken.html



            //客户端请求
            HttpClient http = new HttpClient();
            //请求地址
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=xx00&secret=a189e38e324c1782321d0d1f01bcfdd9";
            //异步请求
            Task<string> task = http.GetStringAsync(url);
            //获取数据解析，并发送至前台
            //ViewBag.ResultTyle = JsonConvert.DeserializeObject<ResultType>(task.Result);
            var obj = new { code = 1, Message = task.Result };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult code2Session(string code)
        {


            //客户端请求
            HttpClient http = new HttpClient();
            //请求地址
            string url = "https://api.weixin.qq.com/sns/jscode2session?" +
                "appid=xx00&secret=a189e38e324c1782321d0d1f01bcfdd9&grant_type=authorization_code&js_code=" + code;
            //异步请求
            Task<string> task = http.GetStringAsync(url);
            //获取数据解析，并发送至前台
            //ViewBag.ResultTyle = JsonConvert.DeserializeObject<ResultType>(task.Result);
            //var obj = new { code = 1, Message = task.Result };

            var obj = JsonConvert.DeserializeObject<session_key_openid>(task.Result);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 校验服务器所保存的登录态 session_key 是否合法。
        /// 为了保持 session_key 私密性，接口不明文传输 session_key，而是通过校验登录态签名完成
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult checksession(string access_token, string openid, string signature)
        {

            string sig_method = "hmac_sha256";
            //客户端请求
            HttpClient http = new HttpClient();
            //请求地址
            string url = "https://api.weixin.qq.com/wxa/checksession?access_token=" + access_token;

            //异步请求
            Task<string> task = http.GetStringAsync(url);
            //获取数据解析，并发送至前台
            //ViewBag.ResultTyle = JsonConvert.DeserializeObject<ResultType>(task.Result);
            //var obj = new { code = 1, Message = task.Result };

            var obj = JsonConvert.DeserializeObject<session_key_openid>(task.Result);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPluginOpenPId(string access_token, string code)
        {


            //客户端请求
            HttpClient http = new HttpClient();
            var str = "access_token=" + access_token + "&code=" + code;//请求数据。这里为空
            //application/json
            str = "{\"code\": \"" + code + "\"}";
            //var data = new { access_token = access_token, code = code };
            //  str = JsonConvert.SerializeObject(data);
            HttpContent content = new StringContent(str);
            //请求地址
            string url = "https://api.weixin.qq.com/wxa/getpluginopenpid?access_token=" + access_token + "&code=" + code;
            url = "https://api.weixin.qq.com/wxa/getpluginopenpid?access_token=" + access_token;
            Task<HttpResponseMessage> postTask = http.PostAsync(url, content);
            HttpResponseMessage result = postTask.Result;//拿到网络请求结果
            result.EnsureSuccessStatusCode();//抛出异常
            Task<string> task = result.Content.ReadAsStringAsync();//异步读取数据
                                                                   //发送值前台
                                                                   //ViewBag.ResultTyle = JsonConvert.DeserializeObject<ResultType>(task.Result);
            var obj = new { code = 1, Message = task.Result, access_token = access_token, code2 = code };

            return Json(obj, JsonRequestBehavior.AllowGet);

            // "appid": "wx7169728c65a29aac" 测试

        }


        /// <summary>
        /// 微信小程序解密算法
        /// </summary>
        /// <param name="encryptedData">加密数据</param>
        /// <param name="iv">初始向量</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns></returns>
        [HttpPost]
        public string Decrypt(string encryptedData, string iv, string sessionKey)
        {
            try
            {
                // 最好用post方法，否则string encryptedData, string iv, string sessionKey 编码会出问题
                //https://developers.weixin.qq.com/miniprogram/dev/framework/open-ability/signature.html
                /*
                 加密数据解密算法
                接口如果涉及敏感数据（如wx.getUserInfo当中的 openId 和 unionId），接口的明文内容将不包含这些敏感数据。开发者如需要获取敏感数据，需要对接口返回的加密数据(encryptedData) 进行对称解密。 解密算法如下：

                对称解密使用的算法为 AES-128-CBC，数据采用PKCS#7填充。
                对称解密的目标密文为 Base64_Decode(encryptedData)。
                对称解密秘钥 aeskey = Base64_Decode(session_key), aeskey 是16字节。
                对称解密算法初始向量 为Base64_Decode(iv)，其中iv由数据接口返回。
                 */
                sessionKey = "bSOA+aMRaMkb2KNlcsAqBQ==";
                iv = "DI6lF3c8ugW7qi5Ms5yG8w==";
                //var encryptedContent = encryptedData.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");

                ////base字符串必须被4整除，不足的在末尾填充'='号
                //if (encryptedContent.Length % 4 > 0)
                //{
                //    encryptedContent = encryptedContent.PadRight(encryptedContent.Length + 4 - encryptedContent.Length % 4, '=');
                //}
                encryptedData = @"7mUdU31gNktr6gG4kl1jtGK7k0WFG9kEubxTjzfN6l4wuYUyfgOrlJnFWl7VNQXRAutJCo4VTpqHN/z1+CvwEZh0ub8EjnPVAOH/6a4K5FkZ/x0mPs7P1cJyQOvEhMldWK6Nbu4TqtFJktCJ0GChbhd+f1/bo3R7oL6BkCpYKGicQjaeRZZIs1ZYr5z5MO/0MOWjbMnqtwww1AUNc/JFHJtZpMD+3kegPdXh3cIwodg9Mz4dUpopogjdhWKF7K/XKLRsNXz4GhTPGM9OCv1Y2wm9BnCfchOndeXM49+OEqsfM5DAPY6tZw0iwyjtHDUakmcku4aNNMzp5KorHOfocwcc4hQ1cBFq1OUF8ev2XNcMgdjLEVZEWjIefDiLFD7Ce5uJv5oafyi8RRUxdE3MmTUEepJH+seff0jI63yOK6i9rkhNL3vCSaiVOIybyiT2bfO9j1ajEzVNVBwncM2pHUqICy3+GcOTwDyWOJaut1HJlLy4E4rFIi8FgYQFCz2vYphc5YFVpio7xvU77gOQwWhMomPB2V5vV0r5SGRh7V0bOu2b8niD24dBCS4hmgNPrgGQQ/XjvYErLYWQWMB9+OebfpDMDrfrjETB5WhQSNhw9hueFoviclp/tYvXtI9Mmr+AOiHCGBjffIUjXrvGHuUcuS4zjEGk1t43GGW91y14WMmrFf36jbika3KalsFdk5TgMMGbn8IDOYeumEpU4uhMJyh+BaIzBa+KALBJVhN/oGtwT0NpshGf6AgxfqQg4zH+tPh7s+Yc3e7XoN59SXHoNtcwBFBEcDYIlTGDBL71h2cojSg5JGvncyUuPkScOciZfWEpT+uGm4dKd+6s5+wZx8mrKcoMwry4r3sNa8Yd5hSoleoWhmbbDVeI3gI6CMFgWwooc6LxD4G4I+PAbnJraHvgxq7ncEcLITe8U+jI0dLAeUbQbn6xinGKNQdSGEPv6MkrQyxy932384aHYe72D0YUUV4MTZeCOGr/frah4Vo7YXoufWskqBISNZFvKBQwVMNA2gMhBcpW5lIudJjMpuLDRO7HWSc+nr8LZFxX0GLwcarwACHljb0sf9G9jgAVaYGhkDCXWAdZJ1oL1R0VeDGgHg2N8Ifz/4xNGut2ql5nzlJISe17VK1p+WFgupw0i2ePr91aePnZ1w7lZ5VgSefqc42TQCz0Dmq0Hv/Nu/Rt0PXA8NhCS5/rIiaVq+qhO0Dr9H4A7iizjPI0mdcvCnaSSi+T4VgrOmHGAIjZywprDPl5y3Dw+JNVKIja/IWaI8QBymfsjFyk1i2E1iZ8Xw5XaUEpYDWcf/qS2Haj39+iDh1A7+p9PEUH1i5D2biJny8RkV9srWRO1kWg3Jo7CLV7dSwEG7lxyzJU1QHVUly2qwgp2wlypUU2dmIx1SkSLbIW33esCu/+dOyqz7Az0DlF7PUkW9oMwWwaqrNZobv3hoJBe7ZCEFdmZX/ZUf0xSUfCGGjJ6dS2codFNvQgtqd9KjcSJezsB5DBdkhdNrN14AyzYIT+xu8mgKX9S3hhDaj/jgeS8wfJhNL9Gu9INH8qplcb87Fp+jgUfhUseyG7kfJcuF8Eyh88H7etqKEENKbVaRwr67AglLcBEzCw9a7Lqw9Tgv9xUVzFN0oEL41NE1F/VR8uPuQVSJTd";
                //创建解密器生成工具实例
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                //设置解密器参数
                aes.Mode = CipherMode.CBC;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                //格式化待处理字符串
                byte[] byte_encryptedData = Convert.FromBase64String(encryptedData);
                byte[] byte_iv = Convert.FromBase64String(iv);
                byte[] byte_sessionKey = Convert.FromBase64String(sessionKey);

                aes.IV = byte_iv;
                aes.Key = byte_sessionKey;
                //根据设置好的数据生成解密器实例
                ICryptoTransform transform = aes.CreateDecryptor();

                //解密
                byte[] final = transform.TransformFinalBlock(byte_encryptedData, 0, byte_encryptedData.Length);
                //生成结果
                string result = Encoding.UTF8.GetString(final);
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
                //LogHelper.Error(ex, "SnsProcessing", "Decrypt");
            }
            return string.Empty;
        }

        /// <summary>
        /// openid
        /// </summary>
        /// <param name="openuid"></param>
        /// <returns></returns>
        public ActionResult getHomeDatalist(string openid)
        {
            List<Homedatalist> hm = new List<Homedatalist>();

            //for (int i = 1; i<= 6; i++) {
            //    hm.Add(new Homedatalist
            //    {
            //         id = i, name="name" + i, url= "/pages/goodlist/goodlist",src = $"https://img.yzcdn.cn/vant/apple-{i}.jpg"
            //    });
            //}

            hm.Add(new Homedatalist
            {
                id = 1,
                name = "家具家居",
                url = "/pages/goodlist/goodlist",
                src = $"https://img.yzcdn.cn/vant/apple-1.jpg"
            });
            hm.Add(new Homedatalist
            {
                id = 2,
                name = "服装鞋子",
                url = "/pages/goodlist/goodlist",
                src = $"https://img.yzcdn.cn/vant/apple-2.jpg"
            });
            hm.Add(new Homedatalist
            {
                id = 3,
                name = "家电电器",
                url = "/pages/goodlist/goodlist",
                src = $"https://img.yzcdn.cn/vant/apple-3.jpg"
            });
            hm.Add(new Homedatalist
            {
                id = 4,
                name = "数码产品",
                url = "/pages/goodlist/goodlist",
                src = $"https://img.yzcdn.cn/vant/apple-4.jpg"
            });

            hm.Add(new Homedatalist
            {
                id = 5,
                name = "食品酒类",
                url = "/pages/goodlist/goodlist",
                src = $"https://img.yzcdn.cn/vant/apple-5.jpg"
            });
            hm.Add(new Homedatalist
            {
                id = 6,
                name = "其他物品",
                url = "/pages/goodlist/goodlist",
                src = $"https://img.yzcdn.cn/vant/apple-6.jpg"
            });
            var data = (from p in hm where p.isdelete == 0 select p).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getGoodDatalist(int pid, string openid, int nowPage = 1, int pageSize = 5)
        {
            //http://localhost:57526/Test/getGoodDatalist?pid=1&openid=aaa&nowPage=5&pageSize=10
            //http://localhost:57526/Test/getGoodDatalist?pid=1&openid=aaa&nowPage=1&pageSize=3
            string sql = $@"SELECT [id]
                          ,[pid]
                          ,[openid]
                          ,[title]
                          ,[_desc]
                          ,[num]
                          ,[price]
                          ,[tag]
                          ,[buytime]
                          ,[addtime]
                      FROM wx_goodadd where pid = '{pid}' and openid = '{openid}' and isdelete = 0 order by id ";

            DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            if (dt == null)
            {
                return Json(new { rowcount = 0 }, JsonRequestBehavior.AllowGet);
            }
            List<goods> hm = new List<goods>();

            foreach(DataRow w in dt.Rows)
            {
                hm.Add(new goods
                {
                    openid = openid,
                    pid = pid,
                    id = int.Parse(w["id"].ToString()),
                    num = int.Parse(w["num"].ToString()),
                    tag = w["tag"].ToString(),
                    price = Convert.ToDouble(w["price"].ToString()),
                    desc = w["_desc"].ToString(),
                    title = w["title"].ToString(),
                    thumb = "/images/tabs/gd.png",
                    buytime = w["buytime"].ToString().Replace(" 0:00:00",""),
                    addtime = w["addtime"].ToString(),
                });
            }
            for (int i = 1; i <= 0; i++)
            {
                hm.Add(new goods
                {
                    openid = "aaa",
                    pid = 1,
                    id = i,
                    num = i,
                    tag = "tag" + i,
                    price = i,
                    desc = "描述" + i,
                    title = "title" + i,
                    thumb = "/images/tabs/gd.png"
                     ,
                    buytime = "2024.01.10",
                    addtime = "2024.02.28"
                });
            }

            var data = (from p in hm where p.isdelete == 0 orderby p.id ascending select p).ToList();

            int pageCount = data.Count / pageSize;
            if (data.Count % pageSize != 0)
            {
                pageCount++;
            }

            //GridResponseModel res =   new  GridResponseModel<Users>(find);
            var v = data.Skip((nowPage - 1) * pageSize).Take(pageSize).ToList();
            var obj = new { rowcount = data.Count, data = v };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult goodadd(string datalist) {

            //{"openid":"oXrvG6wzNllfWpLGlP_AmZDWCjQM","buytime":"2024/2/22","tag":"2222","pid":"3","title":"2","desc":"2","num":1,"price":0}
            goods d = JsonConvert.DeserializeObject<goods>(datalist);
            // insert into wx_goodadd (pid,openid,title,_desc,num,price,tag,buytime)values()

            string sql = $@"insert into wx_goodadd (pid,openid,title,_desc,num,price,tag,buytime)values
            ('{d.pid}','{d.openid}','{d.title}','{d.desc}','{d.num}','{d.price}','{d.tag}','{d.buytime}')";
            string rs = SqlServerSqlHelper.ExecuteNonQuery2(sql);

            return Json(new { success = rs == "" ,message = rs == "" ? "ok":rs },JsonRequestBehavior.AllowGet);
        
        }

        #endregion
    }
    public class Category
    {
        public string url_mid { get; set; }
        public string page { get; set; }
    }

    public class session_key_openid
    {
        public string session_key { get; set; }
        public string openid { get; set; }
    }
    public class AccessToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class Homedatalist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string src { get; set; }

        public int isdelete { get; set; } = 0;
    }

    public class goods
    {
        public string openid { get; set; }
        public int pid { get; set; }
        public int id { get; set; }
        public int num { get; set; } //ok
        public string tag { get; set; } //ok
        public double price { get; set; }  //ok
        public string desc { get; set; }  //ok
        public string title { get; set; } //ok
        public string thumb { get; set; }
        public string buytime { get; set; }
        public string addtime { get; set; }
        public int isdelete { get; set; }


    }
    //{"code":1,"Message":"{\"session_key\":\"N5vBx9faQv5NImR8KvmWGQ==\",\"openid\":\"oXrvG6wzNllfWpLGlP_AmZDWCjQM\"}"}
}