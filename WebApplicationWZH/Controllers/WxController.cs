﻿using Newtonsoft.Json;
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
using System.Configuration;
using System.IO;
using System.Collections;
using System.Globalization;

namespace WebApplicationWZH.Controllers
{
    [SkipCheckLogin]
    public class WxController : Controller
    {
        private static string appid = ConfigurationManager.AppSettings["appid"].ToString();
        private static string secret = ConfigurationManager.AppSettings["secret"].ToString();
        private static string localhost = ConfigurationManager.AppSettings["localhost"].ToString();

        public WxController()
        {

        }
        // GET: Wx
        public ActionResult Index()
        {
            return View();
        }
        #region weixin 小程序接口   / xx00
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
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={secret}";
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
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={secret}";
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
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={secret}";
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
            string url = $"https://api.weixin.qq.com/sns/jscode2session?appid={appid}&secret={secret}&grant_type=authorization_code&js_code=" + code;
            //异步请求
            Task<string> task = http.GetStringAsync(url);
            //获取数据解析，并发送至前台
            //ViewBag.ResultTyle = JsonConvert.DeserializeObject<ResultType>(task.Result);
            //var obj = new { code = 1, Message = task.Result };
            //bool admin = false;
            var obj = JsonConvert.DeserializeObject<session_key_openid>(task.Result);
            if (obj != null && obj.openid != "" && obj.session_key != "")
            {
                useradd(obj.openid, obj.session_key);

                string sql = $"select 1 from wx_users where admin = 1 and openid = '{obj.openid}'";

                sql = $@"SELECT  [uid]
                          ,[openid]
                          ,[session_key]
                          ,[name]
                          ,[addtime]
                          ,[logintime]
                          ,[logincount]
                          ,[admin]
                          ,[flag]
                          ,[isdelete]
                      FROM wx_users where openid = '{obj.openid}'";
                DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);
                //admin = dt != null;
                if (dt != null)
                {
                    obj.uid = dt.Rows[0]["uid"].ToString();
                    obj.admin = dt.Rows[0]["admin"].ToString() == "1";
                }
            }

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

        public ActionResult getGoodDatalist(int pid, string openid, int nowPage = 1, int pageSize = 5, string isadmin = "0", string searchvalue = "")
        {
            //http://localhost:57526/Test/getGoodDatalist?pid=1&openid=aaa&nowPage=5&pageSize=10
            //http://localhost:57526/Test/getGoodDatalist?pid=1&openid=aaa&nowPage=1&pageSize=3
            // approveID = 1 代表信息已被审核通过

            if (string.IsNullOrWhiteSpace(openid))
            {
                return Json(new { rowcount = 0 }, JsonRequestBehavior.AllowGet);
            }

            string sql = $"select 1 from wx_users where admin = 1 and openid = '{openid}'";
            DataTable dt = null;
            //DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            //bool admin = dt != null;
            bool admin = isadmin == "1";
            sql = $@"SELECT [id]
                          ,[pid]
                          ,[openid]
                          ,[title]
                          ,[_desc]
                          ,[num]
                          ,[price]
                          ,[tag]
                          ,[buytime]
                          ,[addtime]
                      FROM wx_goodadd where pid = '{pid}' and openid = '{openid}' and approveID = 1 and isdelete = 0 order by id ";

            string canshu = "";
            if (!string.IsNullOrWhiteSpace(searchvalue))
            {
                canshu = $"  and ( title like '%{searchvalue}%' or  _desc like '%{searchvalue}%' or  tag like '%{searchvalue}%' or  b.name like '%{searchvalue}%' )";
            }
            sql = $@"
                SELECT id  ,pid   ,a.openid  ,title  ,_desc   ,num  ,price  ,tag  ,buytime ,a.addtime,approveID
	                ,b.uid,b.name,b.admin,shared
                  FROM wx_goodadd as a
                  inner join wx_users as b on a.openid = b.openid
                  where pid = '{pid}' and  a.openid = '{openid}' and a.isdelete = 0 and approveID = 1  {canshu}   order by a.id";

            if (admin)
            {


                sql = $@"SELECT [id]
                          ,[pid]
                          ,[openid]
                          ,[title]
                          ,[_desc]
                          ,[num]
                          ,[price]
                          ,[tag]
                          ,[buytime]
                          ,[addtime],approveID
                      FROM wx_goodadd where pid = '{pid}' and isdelete = 0 order by id ";

                if (string.IsNullOrWhiteSpace(searchvalue))
                {
                    sql = $@"
                        SELECT id  ,pid   ,a.openid  ,title  ,_desc   ,num  ,price  ,tag  ,buytime ,a.addtime,approveID
	                        ,b.uid,b.name,b.admin,shared
                          FROM wx_goodadd as a
                          inner join wx_users as b on a.openid = b.openid
                          where pid = '{pid}' and a.isdelete = 0 order by a.id ";
                }
                else
                {
                    if (searchvalue == "未审核")
                    {
                        sql = $@"
                            SELECT id  ,pid   ,a.openid  ,title  ,_desc   ,num  ,price  ,tag  ,buytime ,a.addtime,approveID
	                            ,b.uid,b.name,b.admin,shared
                              FROM wx_goodadd as a
                              inner join wx_users as b on a.openid = b.openid
                              where pid = '{pid}' and a.isdelete = 0 and approveID = 0 order by a.id";
                    }
                    else
                    {
                        sql = $@"
                            SELECT id  ,pid   ,a.openid  ,title  ,_desc   ,num  ,price  ,tag  ,buytime ,a.addtime,approveID
	                            ,b.uid,b.name,b.admin,shared
                              FROM wx_goodadd as a
                              inner join wx_users as b on a.openid = b.openid
                              where pid = '{pid}' and a.isdelete = 0  {canshu}   order by a.id";
                    }
                }

            }

            dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            if (dt == null)
            {
                return Json(new { rowcount = 0 }, JsonRequestBehavior.AllowGet);
            }
            List<goodsView> hm = new List<goodsView>();

            foreach (DataRow w in dt.Rows)
            {
                hm.Add(new goodsView
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
                    buytime = w["buytime"].ToString().Replace(" 0:00:00", ""),
                    addtime = w["addtime"].ToString(),
                    approveID = int.Parse(w["approveID"].ToString()),
                    uid = int.Parse(w["uid"].ToString()),
                    name = w["name"].ToString(),
                    admin = int.Parse(w["admin"].ToString()), isdelete = 0, shared = int.Parse(w["shared"].ToString())
                });
            }
            for (int i = 1; i <= 0; i++)
            {
                hm.Add(new goodsView
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
            var obj = new { rowcount = data.Count, data = v, admin = admin, pageCount = pageCount };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getquanzdatalist(string openid, int nowPage = 1, int pageSize = 5, string isadmin = "0", string searchvalue = "")
        {



            if (!string.IsNullOrWhiteSpace(searchvalue))
            {
                searchvalue = HttpUtility.UrlDecode(searchvalue);
            }
            if (string.IsNullOrWhiteSpace(openid))
            {
                return Json(new { rowcount = 0, message = "1" }, JsonRequestBehavior.AllowGet);
            }

            string sql = $"select 1 from wx_users where openid = '{openid}' and isdelete = 0";
            DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);

            if (dt == null)
            {
                return Json(new { rowcount = 0, message = "2" }, JsonRequestBehavior.AllowGet);
            }

            bool admin = isadmin == "1";



            sql = $@"
               SELECT id  ,pid   ,a.openid  ,title  ,_desc   ,num  ,price  ,tag  ,buytime ,a.addtime ,b.uid,b.name,b.admin
                  FROM wx_goodadd as a
                  inner join wx_users as b on a.openid = b.openid
                  where  approveID = 1 and  shared = 1 and a.isdelete = 0 and b.isdelete = 0 order by a.id  desc";

            //if(admin && searchvalue == "")
            if (!string.IsNullOrWhiteSpace(searchvalue))
            {
                sql = $@"
                  SELECT id  ,pid   ,a.openid  ,title  ,_desc   ,num  ,price  ,tag  ,buytime ,a.addtime ,b.uid,b.name,b.admin
                      FROM wx_goodadd as a
                      inner join wx_users as b on a.openid = b.openid
                      where  approveID = 1 and  shared = 1 and a.isdelete = 0 and b.isdelete = 0 
				      and ( title like '%{searchvalue}%' or _desc like '%{searchvalue}%' or tag like '%{searchvalue}%' or b.name like '{searchvalue}%' )
				      order by a.id desc";

            }
            dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            if (dt == null)
            {
                return Json(new { rowcount = 0, message = "3" }, JsonRequestBehavior.AllowGet);
            }
            List<goodsView> hm = new List<goodsView>();

            foreach (DataRow w in dt.Rows)
            {
                hm.Add(new goodsView
                {
                    openid = openid,
                    pid = int.Parse(w["pid"].ToString()),
                    id = int.Parse(w["id"].ToString()),
                    num = int.Parse(w["num"].ToString()),
                    tag = w["tag"].ToString(),
                    price = Convert.ToDouble(w["price"].ToString()),
                    desc = w["_desc"].ToString(),
                    title = w["title"].ToString(),
                    thumb = "/images/tabs/gd.png",
                    buytime = w["buytime"].ToString().Replace(" 0:00:00", ""),
                    addtime = w["addtime"].ToString(),
                    uid = int.Parse(w["uid"].ToString()),
                    name = w["name"].ToString(),
                    admin = int.Parse(w["admin"].ToString())
                });
            }
            //id	pid	openid	title	_desc	num	price	tag	 buytime	 addtime	approveID	uid	name	admin	shared

            var data = (from p in hm where p.isdelete == 0 orderby p.id descending select p).ToList();

            int pageCount = data.Count / pageSize;
            if (data.Count % pageSize != 0)
            {
                pageCount++;
            }


            // //
            //GridResponseModel res =   new  GridResponseModel<Users>(find);
            var v = data.Skip((nowPage - 1) * pageSize).Take(pageSize).ToList();
            var obj = new { rowcount = data.Count, data = v, admin = admin, pageCount = pageCount };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getGoodData(string openid, int id, string isadmin = "0")
        {

            // approveID = 1 代表信息已被审核通过

            if (string.IsNullOrWhiteSpace(openid))
            {
                return Json(new { rowcount = 0 }, JsonRequestBehavior.AllowGet);
            }

            string sql = $@"SELECT [id]
                          ,[pid]
                          ,[openid]
                          ,[title]
                          ,[_desc]
                          ,[num]
                          ,[price]
                          ,[tag]
                          ,[buytime]
                          ,[addtime],approveID,shared,updatetime
                      FROM wx_goodadd where id = '{id}' and  openid = '{openid}'  and isdelete = 0 ";

            if (isadmin == "1")
            {
                sql = $@"SELECT [id]
                          ,[pid]
                          ,[openid]
                          ,[title]
                          ,[_desc]
                          ,[num]
                          ,[price]
                          ,[tag]
                          ,[buytime]
                          ,[addtime],approveID,shared,updatetime
                      FROM wx_goodadd where id = '{id}' and  isdelete = 0 ";
            }
            DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            if (dt == null)
            {
                return Json(new { rowcount = 0 }, JsonRequestBehavior.AllowGet);
            }
            List<goodsView> hm = new List<goodsView>();

            foreach (DataRow w in dt.Rows)
            {
                hm.Add(new goodsView
                {
                    openid = w["openid"].ToString(),
                    pid = int.Parse(w["pid"].ToString()),
                    id = int.Parse(w["id"].ToString()),
                    num = int.Parse(w["num"].ToString()),
                    tag = w["tag"].ToString(),
                    price = Convert.ToDouble(w["price"].ToString()),
                    desc = w["_desc"].ToString(),
                    title = w["title"].ToString(),
                    thumb = "/images/tabs/gd.png",
                    buytime = w["buytime"].ToString().Replace(" 0:00:00", ""),
                    addtime = w["addtime"].ToString(),
                    approveID = int.Parse(w["approveID"].ToString()), isdelete = 0,
                    shared = int.Parse(w["shared"].ToString()),
                    updatetime = w["updatetime"].ToString()
                });
            }



            var obj = new { rowcount = 1, data = hm[0], admin = isadmin == "1" };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult goodadd(string datalist) {

            //shared
            //{"openid":"oXrvG6wzNllfWpLGlP_AmZDWCjQM","buytime":"2024/2/22","tag":"2222","pid":"3","title":"2","desc":"2","num":1,"price":0}
            goods d = JsonConvert.DeserializeObject<goods>(datalist);
            // insert into wx_goodadd (pid,openid,title,_desc,num,price,tag,buytime)values()

            string sql = $@"insert into wx_goodadd (pid,openid,title,_desc,num,price,tag,buytime,shared)values
            ('{d.pid}','{d.openid}','{d.title}','{d.desc}','{d.num}','{d.price}','{d.tag}','{d.buytime}','{d.shared}')";
            string rs = SqlServerSqlHelper.ExecuteNonQuery2(sql);

            return Json(new { success = rs == "", message = rs == "" ? "ok" : rs }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult goodedit(string datalist, string openid, string isadmin)
        {
            if (string.IsNullOrWhiteSpace(datalist) || string.IsNullOrWhiteSpace(openid))
            {
                return Json(new { success = false, message = "datalist or openid IsNullOrWhiteSpace" }, JsonRequestBehavior.AllowGet);
            }

            //buytime: "2024/2/13"
            //desc: "2"
            //id: "7"
            //num: "3"
            //price: "4"
            //shared: 1
            //tag: "5"
            //title: "1"
            string sql = "";
            try
            {
                //shared
                //{"openid":"oXrvG6wzNllfWpLGlP_AmZDWCjQM","buytime":"2024/2/22","tag":"2222","pid":"3","title":"2","desc":"2","num":1,"price":0}
                goods d = JsonConvert.DeserializeObject<goods>(datalist);

                sql = $@"update wx_goodadd set updatetime = getdate(), buytime = '{d.buytime}',_desc = '{d.desc}'
                           ,num = '{d.num}',price = '{d.price}',shared = '{d.shared}',tag = '{d.tag}',title = '{d.title}'
                            where id = '{d.id}' and openid = '{openid}'";
                if (isadmin == "1")
                {
                    sql = $@"update wx_goodadd set updatetime = getdate(), buytime = '{d.buytime}',_desc = '{d.desc}'
                               ,num = '{d.num}',price = '{d.price}',shared = '{d.shared}',tag = '{d.tag}',title = '{d.title}'
                                where id = '{d.id}'";
                }
                //updatetime
                string rs = SqlServerSqlHelper.ExecuteNonQuery2(sql);

                return Json(new { success = rs == "", message = rs == "" ? "ok" : rs,
                    sql = rs == "" ? "" : sql, updatetime = rs == "" ? DateTime.Now.ToString() : ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, sql = sql }, JsonRequestBehavior.AllowGet);
            }


        }


        private void useradd(string openid, string session_key)
        {
            string sql = $@"  
                if not exists  (select 1 from wx_users where openid = '{openid}')
                   insert into wx_users(openid,session_key)values('{openid}','{session_key}')
               else
                  update wx_users set session_key = '{session_key}',logintime = getdate(),logincount = logincount + 1  where openid ='{openid}'";
            SqlServerSqlHelper.ExecuteNonQuery2(sql);

            // return Json(new { success = rs == "", message = rs == "" ? "ok" : rs });

        }

        public ActionResult getfagui(string openid)
        {

            string htmlSnip = @"<div class=""div_class"">
                    <h5>信息发布说明:</h5>
                    <p class=""div_class_p"">用户所有发布信息必须符合《互联网信息服务管理办法》中所规定的内容</p>
                    <p class=""div_class_p""> 第十五条互联网信息服务提供者不得制作、复制、发布、传播含有下列内容的信息：</p>
                    <ul class=""div_class_ul"">
                    <li>1.反对宪法所确定的基本原则的；</li>
                    <li>2.危害国家安全，泄露国家秘密，颠覆国家政权，破坏国家统一的；</li>
                    <li>3.损害国家荣誉和利益的；</li>
                    <li>4.煽动民族仇恨、民族歧视，破坏民族团结的；</li>
                    <li>5.破坏国家宗教政策，宣扬邪教和封建迷信的；</li>
                    <li>6.散布谣言，扰乱社会秩序，破坏社会稳定的；</li>
                    <li>7.散布淫秽、色情、赌博、暴力、凶杀、恐怖或者教唆犯罪的；</li>
                    <li>8.侮辱或者诽谤他人，侵害他人合法权益的；</li>
                    <li>9.含有法律、行政法规禁止的其他内容的。</li>
                    </ul>
                  </div>`";
            return Json(new { success = true, message = htmlSnip }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult goodapprove(string openid, int id, string isadmin = "0")
        {
            string sql = $"  update wx_goodadd set approveID = 1,approvetime = getdate() where id = {id} and openid = '{openid}'";
            if(IsAdmin("",openid))
            //if (isadmin == "1")
            {
                sql = $"  update wx_goodadd set approveID = 1,approvetime = getdate() where id = {id} ";
                string rs = SqlServerSqlHelper.ExecuteNonQuery2(sql);

                return Json(new { success = rs == "", message = rs == "" ? "ok" : rs });
            }
            return Json(new { success = false, message = "无权限审批" });
        }
        [HttpPost]
        public ActionResult gooddelete(string openid, int id, string isadmin = "0")
        {
            string sql = $"  update wx_goodadd set isdelete = 1,deletetime = getdate() where id = {id} and openid = '{openid}'";
            if (isadmin == "1")
            {
                sql = $"     update wx_goodadd set isdelete = 2,deletetime = getdate() where id = {id} ";
            }
            string rs = SqlServerSqlHelper.ExecuteNonQuery2(sql);

            return Json(new { success = rs == "", message = rs == "" ? "ok" : rs });
        }

        [HttpPost]
        public ActionResult ChangeNickName(string openid, string uid, string NikcName)
        {
            if (string.IsNullOrWhiteSpace(openid) || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(NikcName))
            {
                return Json(new { success = false, message = "IsNullOrWhiteSpace" });
            }
            string sql = $"    update wx_users set Name = '{NikcName}' where openid = '{openid}' and uid = '{uid}'";
             
            string rs = SqlServerSqlHelper.ExecuteNonQuery2(sql);

            return Json(new { success = rs == "", message = rs == "" ? "ok" : rs });
        }
        [HttpPost]
        public ActionResult GetCustomerInfo(string openid, string uid)
        {
            if (string.IsNullOrWhiteSpace(openid) || string.IsNullOrWhiteSpace(uid) )
            {
                return Json(new { success = false, message = "IsNullOrWhiteSpace" });
            }
            string sql = $@"  SELECT  [uid]
                  ,[openid]
                  ,[session_key]
                  ,[name]
                  ,[addtime]
                  ,[logintime]
                  ,[logincount]
                  ,[admin]
                  ,[flag]
                  ,[isdelete],headphoto
              FROM wx_users where openid = '{openid}' and uid = '{uid}' ";

            DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            if(dt == null)
            {
                return Json(new { success = false, message = "DataTable IsNullOrWhiteSpace" });
            }
            ///images/tabs/me.png
            customer c = new customer {
                 uid =  int.Parse(dt.Rows[0]["uid"].ToString()),
                openid = dt.Rows[0]["openid"].ToString(),
                session_key = dt.Rows[0]["session_key"].ToString(),
                name = dt.Rows[0]["name"].ToString(),
                addtime = dt.Rows[0]["addtime"].ToString(),
                logintime = dt.Rows[0]["logintime"].ToString(),
                logincount = int.Parse(dt.Rows[0]["logincount"].ToString()),
                admin = int.Parse(dt.Rows[0]["admin"].ToString()),
                flag = int.Parse(dt.Rows[0]["flag"].ToString()),
                isdelete = int.Parse(dt.Rows[0]["isdelete"].ToString()),
                goodCount = GetGoodCount(dt.Rows[0]["openid"].ToString()),
                headphoto = string.IsNullOrWhiteSpace(dt.Rows[0]["headphoto"].ToString()) ? localhost+ "/Uploads/image/customerphoto/me.png" : localhost + dt.Rows[0]["headphoto"].ToString()

            };
            return Json(new { success = true , message =  "ok" , data = c });
        }
        private int GetGoodCount(string openid)
        {
            string sql = $"select count(id) as qty from wx_goodadd where isdelete = 0 and openid = '{openid}' ";
            DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            if (dt == null)
            {
                return 0;
            }

            return int.Parse(dt.Rows[0]["qty"].ToString());
        }
        //定义存储文件夹
        private string SavePath
        {
            get
            {
                return "/Uploads/";
            }
        }
        [HttpPost]
        public ActionResult imgupload(HttpPostedFileBase imageFile)
        {
            //return Json(new { success = true, message = "" });
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                // 获取保存路径等配置信息
                string savePath = Server.MapPath("~/Uploads");

                // 生成新的文件名
                string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string fileExt = Path.GetExtension(imageFile.FileName).ToLower();
                string newName = Guid.NewGuid().ToString() + fileExt;

                // 将文件保存到指定位置

                string f = Path.Combine(savePath, newName);
                imageFile.SaveAs(f);

                // 返回结果或进行后续操作
                //return RedirectToAction("Index", "Home");
                return Json(new { success = true, message = f });
            }

            // 如果有错误发生，可能需要显示错误消息或重试
            //ModelState.AddModelError("", "无效的文件格式或大小！");
            //return View();
            return Json(new { success = false, message = "无效的文件格式或大小" });
        }
       

        [HttpPost]
        public ActionResult NewUploadImg()
        {

            string GoodID = Request.Form["GoodID"];
            string openid = Request.Form["openid"];
            string uid = Request.Form["uid"];

            string type = Request.Form["type"]; // goodphoto customerphoto
            //  formData: { GoodID: '0',openid: openid,uid:uid,type:'customerphoto'},

            if (type == "goodphoto" || type == "customerphoto")
            {
                
            }
            else
            {
                return NewShowError("请求错误", false);
            }

            //文件保存目录URL
            var saveUrl = SavePath;

            //定义允许上传的文件扩展名
            var extTable = new Hashtable
            {
            {"image", "gif,jpg,jpeg,png,bmp"}
            };
            //最大文件大小
            const int maxSize = 4194304; // 4M大小
            var imgFile = Request.Files[0];

            //HttpPostedFile imgFile = context.Request.Files["imgFile"];

            if (imgFile == null)
            {
                return NewShowError("请选择文件。", false);
            }

            var dirPath = Server.MapPath(SavePath);
            if (!Directory.Exists(dirPath))
            {
                //return ShowError("上传目录不存在。" + dirPath);
                Directory.CreateDirectory(dirPath);
            }
           
            var dirName = Request.QueryString["dir"];
            if (String.IsNullOrEmpty(dirName))
            {
                dirName = "image";
            }

            //if (type == "goodphoto")
            //{
            //    dirName = "image/good";
            //}
            //if (type == "customer")
            //{
            //    dirName = "image/customer";
            //}
            if (!extTable.ContainsKey(dirName))
            {
                return NewShowError("目录名不正确8", false);
            }

            var fileName = imgFile.FileName;
            var extension = Path.GetExtension(fileName);
            if (extension == null)
            {
                return NewShowError("extension == null", false);
            }

            var fileExt = extension.ToLower();

            if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
            {
                return NewShowError("上传文件大小超过限制。", false);
            }

            if (String.IsNullOrEmpty(fileExt) ||
            Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                return NewShowError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。", false);
            }

            string sql = $" select count(sid) as qty from wx_good_img where GoodID = {GoodID} and Isdelete = 0";
            var obj = SqlServerSqlHelper.ExecuteScalar(sql);
            int Max = 5;
            if(Convert.ToInt32(obj) >= Max)
            {
                return NewShowError("上传图片已达到最大数" + Max, false);
            }
            //创建文件夹
            dirPath += dirName + "/";
            saveUrl += dirName + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            dirPath += type + "/" + ymd + "/";
            saveUrl += type + "/" + ymd + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var newFileName = DateTime.Now.ToString("HHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
            newFileName = $"{uid}_{GoodID}_" + DateTime.Now.ToString("yyyyMMdd@HHmmssffff") + fileExt;
            var filePath = dirPath + newFileName;

            imgFile.SaveAs(filePath);
            var fileUrl = saveUrl + newFileName;

            var hash = new Hashtable();

            if (type == "customerphoto")
            {
                return NewShowError(fileUrl, true, GoodID, openid, uid);
            }
            return NewShowError(fileUrl, true, GoodID);

        }


        private JsonResult NewShowError(string fileUrl, bool isImg,string GoodID = "",string openid = "",string uid = "")
        {
            //{"success":true,"message":"/Uploads/image/20240214/1_7_20240214@2251533273.jpg"}

            //var hash = new Hashtable();
            //hash["mess"] = message;
            //hash["success"] = isImg;
            if (isImg && fileUrl.StartsWith("/Uploads"))
            {
                string sql = $"insert into wx_good_img(GoodID,ImgUrl)values('{GoodID}','{fileUrl}')";
                if(openid != "" && uid  != ""){
                       sql = $"update wx_users set headphoto = '{fileUrl}' where openid = '{openid}' and uid = '{uid}'";
                }
               
                 SqlServerSqlHelper.ExecuteNonQuery2(sql);
                var obj2 = new { success = true, message = localhost + fileUrl };
                return Json(obj2);
            }
            //return Json(hash, "text/html;charset=UTF-8");
            var obj = new { success = false, message = "文件保存失败" };
            return Json(obj);
        }

        public ActionResult GetGoodImg(string openid,string GoodID)
        {
            if(string.IsNullOrWhiteSpace(openid) || string.IsNullOrWhiteSpace(GoodID))
            {
                return Json(new { success = false,  message = "openid or GoodID IsNullOrWhiteSpace" }, JsonRequestBehavior.AllowGet);
            }
            // select sid,GoodID,ImgUrl,addtime from wx_good_img where GoodID = 1004 and Isdelete = 0 order by sid
            string sql = $"select sid,GoodID,ImgUrl,addtime from wx_good_img where GoodID = '{GoodID}' and Isdelete = 0 order by sid ";
            DataTable dt = SqlServerSqlHelper.ExecuteDataTable(sql);
            if(dt == null)
            {
                return Json(new { success = false, message = "DataTableIsNullOrWhiteSpace" }, JsonRequestBehavior.AllowGet);
            }
            List<GoodImg> ls = new List<GoodImg>();

            foreach (DataRow w in dt.Rows)
            {
                ls.Add(new GoodImg
                {
                    
                    sid = int.Parse(w["sid"].ToString()),
                    GoodID = int.Parse(w["GoodID"].ToString()),
                    addtime = w["addtime"].ToString(),
                    ImgUrl = localhost + w["ImgUrl"].ToString(),
                });
            }
            return Json(new { success = true, data = ls, message  = "ok"}, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]

        public ActionResult DeleteGoodImg(string openid, string GoodID,string sid )
        {
            if (string.IsNullOrWhiteSpace(openid) || string.IsNullOrWhiteSpace(GoodID))
            {
                return Json(new { success = false, message = "openid or GoodID IsNullOrWhiteSpace" }, JsonRequestBehavior.AllowGet);
            }

            bool admin = IsAdmin("", openid);

            string sql = $@"update wx_good_img  set isdelete = 1 
                    where sid = {sid} and GoodID = (select id from wx_goodadd where isdelete = 0 and openid = '{openid}' and id = {GoodID})";
            if (admin)
            {
                sql = $" update wx_good_img  set isdelete = 1  where sid = {sid} ";
            }
            string rs = SqlServerSqlHelper.ExecuteNonQuery2(sql);
            if(rs == "")
            {

            }
            return Json(new { success = rs == "", message = rs == "" ? "ok" : rs });
        }

        #endregion


        //删除文件
        public ActionResult DeleteImg(string fileSrc)
        {
            var dirPath = Server.MapPath(fileSrc);
            if (System.IO.File.Exists(dirPath))
            {
                System.IO.File.Delete(dirPath);
            }

            return Json("");
        }

        private bool IsAdmin(string uid,string openid)
        {
           
            if (string.IsNullOrWhiteSpace(uid) && string.IsNullOrWhiteSpace(openid))
            {
                return false;
            }
            string sql = "";
            if (uid == "" && openid != "")
            {
                sql = $"select 1 from wx_users where  openid = '{openid}' and admin = 1";
            }
            if (uid != "" && openid == "")
            {
                sql = $"select 1 from wx_users where uid = {uid}  and admin = 1";
            }
            if (uid != "" && openid != "")
            {
                sql = $"select 1 from wx_users where uid = {uid} and openid = '{openid}' and admin = 1";
            }
            var obj = SqlServerSqlHelper.ExecuteScalar(sql);

            return obj != null;
        }

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

        public bool admin { get; set; } = false;

        public string uid { get; set; } = "0";
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
        public int shared { set; get; } = 0;


    }
    public class goodsView
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

        public string updatetime { get; set; }
        public int isdelete { get; set; }

        public int approveID { get; set; }
        public int uid { get; set; }
        public string name { get; set; }

        public int admin { get; set; }
        public int shared { set; get; } = 0;

    }
    //sid,GoodID,ImgUrl,addtime 
    public class GoodImg
    {
        public int sid { get; set; }
        public int GoodID { get; set; }
        public string ImgUrl { get; set; }
       
        public string addtime { set; get; }  

        public int Isdelete { get; set; }
    }

    public class customer
    {

        public int uid { set; get; }
        public string  openid { set; get; }
        public string   session_key{ set; get; }
        public string  name   { set; get; }
        public string addtime { set; get; }
        public string   logintime { set; get; }
        public int  logincount { set; get; }
        public int  admin { set; get; }
        public int  flag { set; get; }
        public int  isdelete { set; get; }

        public int goodCount { set; get; } = 0;

        public string headphoto { set; get; } ///images/tabs/me.png
    }
    //{"code":1,"Message":"{\"session_key\":\"N5vBx9faQv5NImR8KvmWGQ==\",\"openid\":\"oXrvG6wzNllfWpLGlP_AmZDWCjQM\"}"}
}