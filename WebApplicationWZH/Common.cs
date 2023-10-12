using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplicationWZH
{
    public class Common
    {
        //https://blog.csdn.net/dawnZeng/article/details/127630574
        //4.提交信息

         //git commit -m "first commit"  （git commit -m "提交信息"）
    }
    public static class Json
    {
        public static object ToJson(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject(Json);
        }
        public static string ToJson(this object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static string ToJson(this object obj, string datetimeformats)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }
        public static List<T> ToList<T>(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject<List<T>>(Json);
        }
        public static DataTable ToTable(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject<DataTable>(Json);
        }
        public static JObject ToJObject(this string Json)
        {
            return Json == null ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }
    }

    /// <summary>
    /// 验证码类
    /// </summary>
    public class VerifyCode
    {
        public byte[] GetVerifyCode()
        {
            int codeW = 80;
            int codeH = 30;
            int fontSize = 16;
            string chkCode = string.Empty;
            //颜色列表，用于验证码、噪线、噪点 
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            //字体列表，用于验证码 
            string[] font = { "Times New Roman" };
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            //写入Session、验证码加密
            //WebHelper.WriteSession("nfine_session_verifycode", Md5.md5(chkCode.ToLower(), 16));
            //创建画布
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //画噪线 
            for (int i = 0; i < 3; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            //画验证码字符串 
            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, fontSize);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 18, (float)0);
            }
            //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                g.Dispose();
                bmp.Dispose();
            }
        }
    }

    /// <summary>
    /// 验证码 code
    /// </summary>
    public static class ValidCodeUtils
    {
        /// <summary>
        /// 获得随机字符串
        /// </summary>
        /// <param name="intLength">随机数的长度</param>
        /// <returns>随机数字符串</returns>
        public static string GetRandomCode(int intLength)
        {
            /*产生数字和密码混合的随机数*/
            string strReturn = string.Empty;
            Random random = new Random();//随机数
            for (int i = 0; i < intLength; i++)
            {
                char cRerult;
                int intRandom = random.Next();//产生一个非负随机整数
                /*根据当前随机数来确定字符串*/
                //intRandom % 3 获取的是intRandom/3 得到的余数
                if (intRandom % 3 == 0)
                {
                    //产生数字
                    //位数来产生数字
                    cRerult = (char)(0x30 + (intRandom % 10));
                }
                else if (intRandom % 3 == 1)
                {
                    //位数产生大写字母:大写字符 65-97 A 65
                    //68 D  25 Z
                    cRerult = (char)(0x41 + (intRandom % 0x1a));
                }
                else
                {
                    //余数为2
                    //产生小写字母 98 -116
                    cRerult = (char)(0x61 + (intRandom % 0x1a));
                }
                strReturn += cRerult.ToString();
            }
            return strReturn;
        }

        /// <summary>
        /// 根据字符串创建验证码图片 
        /// </summary>
        /// <param name="strRandom">字符串</param>
        /// <returns>图片的二进制数组</returns>
        public static byte[] CreateImage(string strRandom)
        {
            //新增图片
            Bitmap newBitmap = new Bitmap(strRandom.Length * 20, 38);

            Graphics g = Graphics.FromImage(newBitmap);
            g.Clear(Color.White);
            //在图片上绘制文字
            SolidBrush solidBrush = new SolidBrush(Color.Red);
            g.DrawString(strRandom, new Font("Aril", 18), solidBrush, 12, 4);
            //在图片上绘制干扰线
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                //产生一条线，并绘制到画布。 起始点（x,y）  总结点
                int x1 = random.Next(newBitmap.Width);
                int y1 = random.Next(newBitmap.Height);
                int x2 = random.Next(newBitmap.Width);
                int y2 = random.Next(newBitmap.Height);
                g.DrawLine(new Pen(Color.DarkGray), x1, y1, x2, y2);
            }
            //绘制图片的前景干扰点
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(newBitmap.Width);
                int y = random.Next(newBitmap.Height);
                newBitmap.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
            //在最外边绘制边框
            g.DrawRectangle(new Pen(Color.Blue), 0, 0, newBitmap.Width, newBitmap.Height);
            g.DrawRectangle(new Pen(Color.Blue), -1, -1, newBitmap.Width, newBitmap.Height);
            //将图转保存到内存流中
            MemoryStream ms = new MemoryStream();
            newBitmap.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();//将流内容写入byte数组返回
        }
    }

    /// <summary>
    /// AjaxResult类文件
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// 操作结果类型
        /// </summary>
        public object state { get; set; }
        /// <summary>
        /// 获取 消息内容
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 获取 返回数据
        /// </summary>
        public object data { get; set; }
    }
    /// <summary>
    /// 表示 ajax 操作结果类型的枚举
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 消息结果类型
        /// </summary>
        info,
        /// <summary>
        /// 成功结果类型
        /// </summary>
        success,
        /// <summary>
        /// 警告结果类型
        /// </summary>
        warning,
        /// <summary>
        /// 异常结果类型
        /// </summary>
        error
    }

    /// <summary>
    /// 通用数据访问类
    /// </summary>
    public class SQLHelper
    {
        //定义连接字符串
        private static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();

        //执行增删改
        public static int ExecuteNonQuery(string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {

                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);

                    }
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public static int ExecuteNonQuery(string sql)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        //执行返回SqlDataReader
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] pms)
        {
            SqlConnection con = new SqlConnection(connString);

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (pms != null)
                {
                    cmd.Parameters.AddRange(pms);
                }
                try
                {
                    con.Open();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception)
                {
                    con.Close();
                    con.Dispose();
                    throw;
                }

            }

        }
        //执行返回单个值：第一列第一行
        public static object ExecuteScalar(string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
        //执行返回datatable
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connString))
            {
                if (pms != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(pms);

                }
                adapter.Fill(dt);
                return dt;
            }

        }
        public static DataTable ExecuteDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connString))
            {
                adapter.Fill(dt);
                return dt;
            }

        }
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class Sys_user
    {
        public int userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Sys_userService
    {
        //登录
        public Sys_user Login(string username, string password)
        {
            Sys_user sys_User = null;
            //编写Sql语句
            string sql = " select userid,username,password" +
                         " from sys_user" +
                         " where username=@username and password=@password";
            //编写参数
            SqlParameter[] pms = { new SqlParameter("username", username), new SqlParameter("password", password) };
            //调用SQLHelper类中的查询方法
            SqlDataReader reader = SQLHelper.ExecuteReader(sql, pms);
            if (reader.Read())
            {
                sys_User = new Sys_user();
                sys_User.userid = Convert.ToInt32(reader["userid"]);
                sys_User.username = reader["username"].ToString();
                sys_User.password = reader["password"].ToString();
            }
            reader.Close();
            return sys_User;
        }
    }


}