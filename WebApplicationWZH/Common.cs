using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

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
    public sealed class SqlServerSqlHelper
    {
        private SqlServerSqlHelper() { }
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
        public static object ExecuteScalar(string sql)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
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

        /// <summary>
        /// 增删改，事务执行  insert update delete
        /// </summary>
        /// <param name="SqlStrList"></param>
        /// <returns></returns>
        public static string ExecuteNonQuery(List<string> SqlStrList)
        {
            SqlConnection con = null;
            SqlTransaction tran = null;
            string Msg = "";
            string sql = "";
            try
            {
                con = new SqlConnection(connString);
                con.Open();
                tran = con.BeginTransaction();//先实例SqlTransaction类，使用这个事务使用的是con 这个连接，使用BeginTransaction这个方法来开始执行这个事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.Transaction = tran;
                //在try{} 块里执行sqlcommand命令，
                //cmd.CommandText = "update bb set moneys=moneys-'" + Convert.ToInt32(TextBox1.Text) + "' where ID='1'";
                //cmd.ExecuteNonQuery();
                //cmd.CommandText = "update bb set moneys=moneys+' aa ' where ID='2'";
                //cmd.ExecuteNonQuery();
                foreach(string str in SqlStrList)
                {
                    sql = str;
                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();//如果两个sql命令都执行成功，则执行commit这个方法，执行这些操作
                 
            }
            catch(Exception ex)
            {
                Msg = sql + " : Exception = " + ex.ToString();
                tran.Rollback();//如何执行不成功，发生异常，则执行rollback方法，回滚到事务操作开始之前；
            }
            finally
            {
                if(con != null)
                {
                    con.Close();
                    con.Dispose();
                }
              
            }
            return Msg;
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
            SqlDataReader reader = SqlServerSqlHelper.ExecuteReader(sql, pms);
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


    
        /// <summary>
        /// Account Helper
        /// </summary>
        public static class AccountHelper
        {
            /// <summary>
            /// Get all permission list
            /// </summary>
            /// <returns>Permission List</returns>
            public static List<PermissionItem> GetPermissionItems()
            {
                 //  HttpContext.Current.Cache：为当前 HTTP 请求获取Cache对象。
                if (HttpContext.Current.Cache["PermissionItems"] == null)
                {
                    UrlAuthorizeEntities db = new UrlAuthorizeEntities();
                    var items = db.PermissionItems.Where(c => c.PermissionID > 0).ToList();
                    HttpContext.Current.Cache["PermissionItems"] = items;
                }
            
                return (List<PermissionItem>)HttpContext.Current.Cache["PermissionItems"];
            }

            /// <summary>
            /// Get User Permission
            /// </summary>
            /// <param name="userID">User ID</param>
            /// <returns>User Permission Array</returns>
            public static Int32[] GetUserPermission(int userID)
            {
                if (HttpContext.Current.Session["Permission"] == null)
                {
                    UrlAuthorizeEntities db = new UrlAuthorizeEntities();
                    var permissions = db.PermissionList.Where(c => c.UserID == userID).Select(c => c.PermissionID).ToArray();
                    HttpContext.Current.Session["Permission"] = permissions;
                }
                return (Int32[])HttpContext.Current.Session["Permission"];
            }
        }

        public class PermissionList
        {
            public int ID { set; get; }

            public int PermissionID { set; get; }

            public int UserID { set; get; }
        }

        public class PermissionItem
        {
            public int ID { set; get; }

            public int PermissionID { set; get; }

            public string Name { set; get; }

            public string Route { set; get; }
        }

        public class UrlAuthorizeEntities
        {
            //https://blog.csdn.net/afandaafandaafanda/article/details/46780439/
            public IEnumerable<PermissionItem> PermissionItems = new List<PermissionItem>
            {
                new PermissionItem{ ID = 1 , PermissionID = 1, Name = "Test Page 1", Route = "/Home/Page1" },
                new PermissionItem{ ID = 2 , PermissionID = 2, Name = "Test Page 2", Route = "/Home/Page2" },
                new PermissionItem{ ID = 3 , PermissionID = 3, Name = "Test Page 3", Route = "/Home/Page3" },
                new PermissionItem{ ID = 4 , PermissionID = 1, Name = "Test Page 4", Route = "/Home/Page4" },
                new PermissionItem{ ID = 5 , PermissionID = 2, Name = "Test Page 5", Route = "/Login/Index" }
            };

            public IEnumerable<PermissionList> PermissionList = new List<PermissionList>
            {
                new PermissionList{ ID = 1 , PermissionID = 2, UserID = 1},
                new PermissionList{ ID = 2 , PermissionID = 3, UserID = 1},
            };
        }

        public class DescriptionAttribute : Attribute
        {
            public string ActionTitle
        {
                set;
                get;
            }
            public int ActionOrderNumber
            {

                set;
                get;
            }
        }


    /// <summary>  
    /// SqlServer数据访问帮助类  
    /// </summary>  
    public sealed class SqlHelper
    {
        #region 私有构造函数和方法  
        private SqlHelper() { }
        /// <summary>  
        /// 将SqlParameter参数数组(参数值)分配给SqlCommand命令.  
        /// 这个方法将给任何一个参数分配DBNull.Value;  
        /// 该操作将阻止默认值的使用.  
        /// </summary>SqlHelperSqlHelper  
        /// <param>命令名</param>  
        /// <param>SqlParameters数组</param>  
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // 检查未分配值的输出参数,将其分配以DBNull.Value.  
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>  
        /// 将DataRow类型的列值分配到SqlParameter参数数组.  
        /// </summary>  
        /// <param>要分配值的SqlParameter参数数组</param>  
        /// <param>将要分配给存储过程参数的DataRow</param>  
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                return;
            }
            int i = 0;
            // 设置参数值  
            foreach (SqlParameter commandParameter in commandParameters)
            {
                // 创建参数名称,如果不存在,只抛出一个异常.  
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        string.Format("请提供参数{ 0 }一个有效的名称{ 1}.", i, commandParameter.ParameterName));
                // 从dataRow的表中获取为参数数组中数组名称的列的索引.  
                // 如果存在和参数名称相同的列,则将列值赋给当前名称的参数.  
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        /// <summary>  
        /// 将一个对象数组分配给SqlParameter参数数组.  
        /// </summary>  
        /// <param>要分配值的SqlParameter参数数组</param>  
        /// <param>将要分配给存储过程参数的对象数组</param>  
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }
            // 确保对象数组个数与参数个数匹配,如果不匹配,抛出一个异常.  
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("参数值个数与参数不匹配.");
            }
            // 给参数赋值  
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property  
                if (parameterValues is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues;
                }
            }
        }

        /// <summary>  
        /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数  
        /// </summary>  
        /// <param>要处理的SqlCommand</param>  
        /// <param>数据库连接</param>  
        /// <param>一个有效的事务或者是null值</param>  
        /// <param>命令类型 (存储过程,命令文本, 其它.)</param>  
        /// <param>存储过程名或都T-SQL命令文本</param>  
        /// <param>和命令相关联的SqlParameter参数数组,如果没有参数为’null’</param>  
        /// <param><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param>  
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");
            // If the provided connection is not open, we will open it  
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            // 给命令分配一个数据库连接.  
            command.Connection = connection;
            // 设置命令文本(存储过程名或SQL语句)  
            command.CommandText = commandText;
            // 分配事务  
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }
            // 设置命令类型.  
            command.CommandType = commandType;
            // 分配命令参数  
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }
        #endregion 私有构造函数和方法结束  

        #region ExecuteNonQuery命令  
        /// <summary>  
        /// 执行指定连接字符串,类型的SqlCommand.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本, 其它.)</param>  
        /// <param>存储过程名称或SQL语句</param>  
        /// <returns>返回命令影响的行数</returns>  
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定连接字符串,类型的SqlCommand.如果没有提供参数,不返回结果.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本, 其它.)</param>  
        /// <param>存储过程名称或SQL语句</param>  
        /// <param>SqlParameter参数数组</param>  
        /// <returns>返回命令影响的行数</returns>  
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>  
        /// 执行指定连接字符串的存储过程,将对象数组的值赋给存储过程参数,  
        /// 此方法需要在参数缓存方法中探索参数并生成参数.  
        /// </summary>  
        /// <remarks>  
        /// 这个方法没有提供访问输出参数和返回值.  
        /// 示例:    
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串/param>  
        /// <param>存储过程名称</param>  
        /// <param>分配到存储过程输入参数的对象数组</param>  
        /// <returns>返回受影响的行数</returns>  
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果存在参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从探索存储过程参数(加载到缓存)并分配给存储过程参数数组.  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数情况下  
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令   
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型(存储过程,命令文本或其它.)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <returns>返回影响的行数</returns>  
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型(存储过程,命令文本或其它.)</param>  
        /// <param>T存储过程名称或T-SQL语句</param>  
        /// <param>SqlParamter参数数组</param>  
        /// <returns>返回影响的行数</returns>  
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            // 创建SqlCommand命令,并进行预处理  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command  
            int retval = cmd.ExecuteNonQuery();

            // 清除参数,以便再次使用.  
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,将对象数组的值赋给存储过程参数.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值  
        /// 示例:    
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回影响的行数</returns>  
        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                // 给存储过程分配参数值  
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行带事务的SqlCommand.  
        /// </summary>  
        /// <remarks>  
        /// 示例.:    
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型(存储过程,命令文本或其它.)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <returns>返回影响的行数/returns>  
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行带事务的SqlCommand(指定参数).  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型(存储过程,命令文本或其它.)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>SqlParamter参数数组</param>  
        /// <returns>返回影响的行数</returns>  
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            // 预处理  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行  
            int retval = cmd.ExecuteNonQuery();

            // 清除参数集,以便再次使用.  
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>  
        /// 执行带事务的SqlCommand(指定参数值).  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值  
        /// 示例:    
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回受影响的行数</returns>  
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值  
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion ExecuteNonQuery方法结束  

        #region ExecuteDataset方法  
        /// <summary>  
        /// 执行指定数据库连接字符串的命令,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的命令,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 示例:   
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>SqlParamters参数数组</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            // 创建并打开数据库连接对象,操作完成释放对象.  
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // 调用指定数据库连接字符串重载方法.  
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的命令,直接提供参数值,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值.  
        /// 示例:   
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中检索存储过程参数  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                // 给存储过程参数分配值  
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <param>SqlParamter参数数组</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            // 预处理  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建SqlDataAdapter和DataSet.  
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                // 填充DataSet.  
                da.Fill(ds);

                cmd.Parameters.Clear();
                if (mustCloseConnection)
                    connection.Close();
                return ds;
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定参数值,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输入参数和返回值.  
        /// 示例.:    
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 比缓存中加载存储过程参数  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                // 给存储过程参数分配值  
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定事务的命令,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定事务的命令,指定参数,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <param>SqlParamter参数数组</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            // 预处理  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建 DataAdapter & DataSet  
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        /// <summary>  
        /// 执行指定事务的命令,指定参数值,返回DataSet.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输入参数和返回值.  
        /// 示例.:    
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>事务</param>  
        /// <param>存储过程名</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回一个包含结果集的DataSet</returns>  
        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                // 给存储过程参数分配值  
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion ExecuteDataset数据集命令结束  

        #region ExecuteReader 数据阅读器  
        /// <summary>  
        /// 枚举,标识数据库连接是由SqlHelper提供还是由调用者提供  
        /// </summary>  
        private enum SqlConnectionOwnership
        {
            /// <summary>由SqlHelper提供连接</summary>  
            Internal,
            /// <summary>由调用者提供连接</summary>  
            External
        }

        /// <summary>  
        /// 执行指定数据库连接对象的数据阅读器.  
        /// </summary>  
        /// <remarks>  
        /// 如果是SqlHelper打开连接,当连接关闭DataReader也将关闭.  
        /// 如果是调用都打开连接,DataReader由调用都管理.  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>一个有效的事务,或者为 ‘null’</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <param>SqlParameters参数数组,如果没有参数则为’null’</param>  
        /// <param>标识数据库连接对象是由调用者提供还是由SqlHelper提供</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            bool mustCloseConnection = false;
            // 创建命令  
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                // 创建数据阅读器  
                SqlDataReader dataReader;
                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                // 清除参数,以便再次使用..  
                // HACK: There is a problem here, the output parameter values are fletched   
                // when the reader is closed, so if the parameters are detached from the command  
                // then the SqlReader can磘 set its values.   
                // When this happen, the parameters can磘 be used again in other command.  
                bool canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }
                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的数据阅读器.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的数据阅读器,指定参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <param>SqlParamter参数数组(new SqlParameter("@prodid", 24))</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves  
                if (connection != null) connection.Close();
                throw;
            }

        }

        /// <summary>  
        /// 执行指定数据库连接字符串的数据阅读器,指定参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的数据阅读器.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名或T-SQL语句</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>SqlParamter参数数组</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>  
        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>T存储过程名</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///   SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>  
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  SqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>存储过程名称</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值  
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion ExecuteReader数据阅读器  

        #region ExecuteScalar 返回结果集中的第一行第一列          
        /// <summary>  
        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法  
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的命令,指定参数,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            // 创建并打开数据库连接对象,操作完成释放对象.  
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // 调用指定数据库连接字符串重载方法.  
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的命令,指定参数值,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名称</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值  
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法  
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定参数,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            // 创建SqlCommand命令,并进行预处理  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行SqlCommand命令,并返回结果.  
            object retval = cmd.ExecuteScalar();

            // 清除参数,以便再次使用.  
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,指定参数值,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值  
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定数据库事务的命令,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法  
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            // 创建SqlCommand命令,并进行预处理  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行SqlCommand命令,并返回结果.  
            object retval = cmd.ExecuteScalar();

            // 清除参数,以便再次使用.  
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>  
        /// 执行指定数据库事务的命令,指定参数值,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>存储过程名称</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // PPull the parameters for this stored procedure from the parameter cache ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值  
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion ExecuteScalar   

        #region ExecuteXmlReader XML阅读器  
        /// <summary>  
        /// 执行指定数据库连接对象的SqlCommand命令,并产生一个XmlReader对象做为结果集返回.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句 using "FOR XML AUTO"</param>  
        /// <returns>返回XmlReader结果集对象.</returns>  
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法  
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库连接对象的SqlCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句 using "FOR XML AUTO"</param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        /// <returns>返回XmlReader结果集对象.</returns>  
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            bool mustCloseConnection = false;
            // 创建SqlCommand命令,并进行预处理  
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

                // 执行命令  
                XmlReader retval = cmd.ExecuteXmlReader();

                // 清除参数,以便再次使用.  
                cmd.Parameters.Clear();
                return retval;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的SqlCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称 using "FOR XML AUTO"</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回XmlReader结果集对象.</returns>  
        public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值  
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定数据库事务的SqlCommand命令,并产生一个XmlReader对象做为结果集返回.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句 using "FOR XML AUTO"</param>  
        /// <returns>返回XmlReader结果集对象.</returns>  
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法  
            return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>  
        /// 执行指定数据库事务的SqlCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句 using "FOR XML AUTO"</param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        /// <returns>返回XmlReader结果集对象.</returns>  
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            // 创建SqlCommand命令,并进行预处理  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行命令  
            XmlReader retval = cmd.ExecuteXmlReader();

            // 清除参数,以便再次使用.  
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>  
        /// 执行指定数据库事务的SqlCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>存储过程名称</param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        /// <returns>返回一个包含结果集的DataSet.</returns>  
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值  
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion ExecuteXmlReader 阅读器结束  

        #region FillDataset 填充数据集  
        /// <summary>  
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)</param>  
        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");

            // 创建并打开数据库连接对象,操作完成释放对象.  
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // 调用指定数据库连接字符串重载方法.  
                FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集.指定命令参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>  
        public static void FillDataset(string connectionString, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            // 创建并打开数据库连接对象,操作完成释放对象.  
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // 调用指定数据库连接字符串重载方法.  
                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集,指定存储过程参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24);  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名称</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>      
        /// <param>分配给存储过程输入参数的对象数组</param>  
        public static void FillDataset(string connectionString, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            // 创建并打开数据库连接对象,操作完成释放对象.  
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // 调用指定数据库连接字符串重载方法.  
                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>      
        public static void FillDataset(SqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        public static void FillDataset(SqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>  
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定存储过程参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36);  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        public static void FillDataset(SqlConnection connection, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // 没有参数值  
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary>  
        /// 执行指定数据库事务的命令,映射数据表并填充数据集.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>  
        public static void FillDataset(SqlTransaction transaction, CommandType commandType,
            string commandText,
            DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>  
        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        public static void FillDataset(SqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>  
        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定存储过程参数值.  
        /// </summary>  
        /// <remarks>  
        /// 此方法不提供访问存储过程输出参数和返回值参数.  
        ///   
        /// 示例:    
        ///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36);  
        /// </remarks>  
        /// <param>一个有效的连接事务</param>  
        /// <param>存储过程名称</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>  
        /// <param>分配给存储过程输入参数的对象数组</param>  
        public static void FillDataset(SqlTransaction transaction, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果有参数值  
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                // 给存储过程参数赋值  
                AssignParameterValues(commandParameters, parameterValues);
                // 调用重载方法  
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // 没有参数值  
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary>  
        /// [私有方法][内部调用]执行指定数据库连接对象/事务的命令,映射数据表并填充数据集,DataSet/TableNames/SqlParameters.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>一个有效的连接事务</param>  
        /// <param>命令类型 (存储过程,命令文本或其它)</param>  
        /// <param>存储过程名称或T-SQL语句</param>  
        /// <param>要填充结果集的DataSet实例</param>  
        /// <param>表映射的数据表数组  
        /// 用户定义的表名 (可有是实际的表名.)  
        /// </param>  
        /// <param>分配给命令的SqlParamter参数数组</param>  
        private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            // 创建SqlCommand命令,并进行预处理  
            SqlCommand command = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行命令  
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
            {

                // 追加表映射  
                if (tableNames != null && tableNames.Length > 0)
                {
                    string tableName = "Table";
                    for (int index = 0; index < tableNames.Length; index++)
                    {
                        if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName += (index + 1).ToString();
                    }
                }

                // 填充数据集使用默认表名称  
                dataAdapter.Fill(dataSet);
                // 清除参数,以便再次使用.  
                command.Parameters.Clear();
            }
            if (mustCloseConnection)
                connection.Close();
        }
        #endregion

        #region UpdateDataset 更新数据集  
        /// <summary>  
        /// 执行数据集更新到数据库,指定inserted, updated, or deleted命令.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");  
        /// </remarks>  
        /// <param>[追加记录]一个有效的T-SQL语句或存储过程</param>  
        /// <param>[删除记录]一个有效的T-SQL语句或存储过程</param>  
        /// <param>[更新记录]一个有效的T-SQL语句或存储过程</param>  
        /// <param>要更新到数据库的DataSet</param>  
        /// <param>要更新到数据库的DataTable</param>  
        public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (insertCommand == null) throw new ArgumentNullException("insertCommand");
            if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
            if (updateCommand == null) throw new ArgumentNullException("updateCommand");
            if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("tableName");
            // 创建SqlDataAdapter,当操作完成后释放.  
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
            {
                // 设置数据适配器命令  
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;
                // 更新数据集改变到数据库  
                dataAdapter.Update(dataSet, tableName);
                // 提交所有改变到数据集.  
                dataSet.AcceptChanges();
            }
        }
        #endregion

        #region CreateCommand 创建一条SqlCommand命令  
        /// <summary>  
        /// 创建SqlCommand命令,指定数据库连接对象,存储过程名和参数.  
        /// </summary>  
        /// <remarks>  
        /// 示例:    
        ///  SqlCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>源表的列名称数组</param>  
        /// <returns>返回SqlCommand命令</returns>  
        public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 创建命令  
            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            // 如果有参数值  
            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                // 将源表的列到映射到DataSet命令中.  
                for (int index = 0; index < sourceColumns.Length; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];
                // Attach the discovered parameters to the SqlCommand object  
                AttachParameters(cmd, commandParameters);
            }
            return cmd;
        }
        #endregion

        #region ExecuteNonQueryTypedParams 类型化参数(DataRow)  
        /// <summary>  
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回受影响的行数.  
        /// </summary>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回影响的行数</returns>  
        public static int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回受影响的行数.  
        /// </summary>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回影响的行数</returns>  
        public static int ExecuteNonQueryTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回受影响的行数.  
        /// </summary>  
        /// <param>一个有效的连接事务 object</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回影响的行数</returns>  
        public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // Sf the row has values, the store procedure parameters must be initialized  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteDatasetTypedParams 类型化参数(DataRow)  
        /// <summary>  
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataSet.  
        /// </summary>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回一个包含结果集的DataSet.</returns>  
        public static DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            //如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataSet.  
        /// </summary>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回一个包含结果集的DataSet.</returns>  
        ///   
        public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回DataSet.  
        /// </summary>  
        /// <param>一个有效的连接事务 object</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回一个包含结果集的DataSet.</returns>  
        public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteReaderTypedParams 类型化参数(DataRow)  
        /// <summary>  
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataReader.  
        /// </summary>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataReader.  
        /// </summary>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回DataReader.  
        /// </summary>  
        /// <param>一个有效的连接事务 object</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回包含结果集的SqlDataReader</returns>  
        public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteScalarTypedParams 类型化参数(DataRow)  
        /// <summary>  
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalarTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列.  
        /// </summary>  
        /// <param>一个有效的连接事务 object</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回结果集中的第一行第一列</returns>  
        public static object ExecuteScalarTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteXmlReaderTypedParams 类型化参数(DataRow)  
        /// <summary>  
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回XmlReader类型的结果集.  
        /// </summary>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回XmlReader结果集对象.</returns>  
        public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>  
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回XmlReader类型的结果集.  
        /// </summary>  
        /// <param>一个有效的连接事务 object</param>  
        /// <param>存储过程名称</param>  
        /// <param>使用DataRow作为参数值</param>  
        /// <returns>返回XmlReader结果集对象.</returns>  
        public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            // 如果row有值,存储过程必须初始化.  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()  
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值  
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion
    }

    /// <summary>  
    /// SqlHelperParameterCache提供缓存存储过程参数,并能够在运行时从存储过程中探索参数.   微软官方的SQLHelper类(含完整中文注释)
    /// </summary>  
    public sealed class SqlHelperParameterCache
    {
        #region 私有方法,字段,构造函数  
        // 私有构造函数,妨止类被实例化.  
        private SqlHelperParameterCache() { }
        // 这个方法要注意  
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>  
        /// 探索运行时的存储过程,返回SqlParameter参数数组.  
        /// 初始化参数值为 DBNull.Value.  
        /// </summary>  
        /// <param>一个有效的数据库连接</param>  
        /// <param>存储过程名称</param>  
        /// <param>是否包含返回值参数</param>  
        /// <returns>返回SqlParameter参数数组</returns>  
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            // 检索cmd指定的存储过程的参数信息,并填充到cmd的Parameters参数集中.  
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();
            // 如果不包含返回值参数,将参数集中的每一个参数删除.  
            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            // 创建参数数组  
            SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];
            // 将cmd的Parameters参数集复制到discoveredParameters数组.  
            cmd.Parameters.CopyTo(discoveredParameters, 0);
            // 初始化参数值为 DBNull.Value.  
            foreach (SqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>  
        /// SqlParameter参数数组的深层拷贝.  
        /// </summary>  
        /// <param>原始参数数组</param>  
        /// <returns>返回一个同样的参数数组</returns>  
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];
            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters = (SqlParameter[])((ICloneable)originalParameters).Clone();
            }
            return clonedParameters;
        }
        #endregion 私有方法,字段,构造函数结束  

        #region 缓存方法  
        /// <summary>  
        /// 追加参数数组到缓存.  
        /// </summary>  
        /// <param>一个有效的数据库连接字符串</param>  
        /// <param>存储过程名或SQL语句</param>  
        /// <param>要缓存的参数数组</param>  
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");
            string hashKey = connectionString + ":" + commandText;
            paramCache[hashKey] = commandParameters;
        }

        /// <summary>  
        /// 从缓存中获取参数数组.  
        /// </summary>  
        /// <param>一个有效的数据库连接字符</param>  
        /// <param>存储过程名或SQL语句</param>  
        /// <returns>参数数组</returns>  
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");
            string hashKey = connectionString + ":" + commandText;
            SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }
        #endregion 缓存方法结束  

        #region 检索指定的存储过程的参数集  
        /// <summary>  
        /// 返回指定的存储过程的参数集  
        /// </summary>  
        /// <remarks>  
        /// 这个方法将查询数据库,并将信息存储到缓存.  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符</param>  
        /// <param>存储过程名</param>  
        /// <returns>返回SqlParameter参数数组</returns>  
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>  
        /// 返回指定的存储过程的参数集  
        /// </summary>  
        /// <remarks>  
        /// 这个方法将查询数据库,并将信息存储到缓存.  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符.</param>  
        /// <param>存储过程名</param>  
        /// <param>是否包含返回值参数</param>  
        /// <returns>返回SqlParameter参数数组</returns>  
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>  
        /// [内部]返回指定的存储过程的参数集(使用连接对象).  
        /// </summary>  
        /// <remarks>  
        /// 这个方法将查询数据库,并将信息存储到缓存.  
        /// </remarks>  
        /// <param>一个有效的数据库连接字符</param>  
        /// <param>存储过程名</param>  
        /// <returns>返回SqlParameter参数数组</returns>  
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>  
        /// [内部]返回指定的存储过程的参数集(使用连接对象)  
        /// </summary>  
        /// <remarks>  
        /// 这个方法将查询数据库,并将信息存储到缓存.  
        /// </remarks>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名</param>  
        /// <param>  
        /// 是否包含返回值参数  
        /// </param>  
        /// <returns>返回SqlParameter参数数组</returns>  
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>  
        /// [私有]返回指定的存储过程的参数集(使用连接对象)  
        /// </summary>  
        /// <param>一个有效的数据库连接对象</param>  
        /// <param>存储过程名</param>  
        /// <param>是否包含返回值参数</param>  
        /// <returns>返回SqlParameter参数数组</returns>  
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
            SqlParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion 参数集检索结束  
    }

    public class BusinessResultBase
    {
        public string Title { get; set; }
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
         
    }

    public class DB
    {

        // private static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();
        //https://freesql.net/guide/getting-started.html#%E5%A3%B0%E6%98%8E
        //https://www.cnblogs.com/kellynic/p/10645049.html

        static Lazy<IFreeSql> sqlserverLazy = new Lazy<IFreeSql>(() => new FreeSql.FreeSqlBuilder()
         .UseMonitorCommand(cmd => Trace.WriteLine($"Sql：{cmd.CommandText}"))//监听SQL语句,Trace在输出选项卡中查看
         .UseConnectionString(FreeSql.DataType.SqlServer, ConfigurationManager.ConnectionStrings["connString"].ToString())
         .UseAutoSyncStructure(false) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
         .Build());
        public static IFreeSql SqlServer => sqlserverLazy.Value;
    }
    #region DLShouWen Grid
    public class GridRequestModel
    {
        //{"isExport":false,"pageSize":10,"startRecord":0,"nowPage":1,"recordCount":-1,"pageCount":-1,"parameters":{},"fastQueryParameters":{},"advanceQueryConditions":[],"advanceQuerySorts":[]}
        //https://os.dlshouwen.com/grid/doc/i18n/zh-cn/example.html#2.2.2
        public bool isExport { get; set; }
        public int pageSize { get; set; }
        public int startRecord { get; set; }

        public int nowPage { get; set; }
        public int recordCount { get; set; }
        public int pageCount { get; set; }

        public  parameters parameters { get; set; } =  new parameters();
        public fastQueryParameters fastQueryParameters { get; set; } = new fastQueryParameters();

        public object advanceQueryConditions { get; set; }
        public object advanceQuerySorts { get; set; }

        public List<object> exhibitDatas { get; set; }

        public bool exportAllData { get; set; } = false;

        public List<object> exportColumns { get; set; }

        public bool exportDataIsProcessed { get; set; } = false;
    }

    public class GridResponseModel<T> where T : new()
    {
        //{"isExport":false,"pageSize":10,"startRecord":0,"nowPage":1,"recordCount":-1,"pageCount":-1,"parameters":{},"fastQueryParameters":{},"advanceQueryConditions":[],"advanceQuerySorts":[]}
        //https://os.dlshouwen.com/grid/doc/i18n/zh-cn/example.html#2.2.2
        public bool isExport { get; set; }
        public int pageSize { get; set; }
        public int startRecord { get; set; }

        public int nowPage { get; set; }
        public int recordCount { get; set; }
        public int pageCount { get; set; }

        public parameters parameters { get; set; } = new parameters();

        public fastQueryParameters fastQueryParameters { get; set; } = new fastQueryParameters();

        public object advanceQueryConditions { get; set; } = new List<object>();
        public object advanceQuerySorts { get; set; } = new List<object>();

        public List<T> exhibitDatas { get; set; }



        public bool exportAllData { get; set; } = false;

        public List<object> exportColumns { get; set; } = new List<object>();

        public bool exportDataIsProcessed { get; set; } = false;

        public List<object> exportDatas { get; set; } = new List<object>();

        public string exportFileName { get; set; } = "";

        public string exportType { get; set; } = "";

        public bool isSuccess { get; set; } = true;

    }
    public class parameters
    {
        public string __RequestVerificationToken { get; set; }

        public int RoleID { get; set; } = 0;
    }
    public class fastQueryParameters
    {
    }
    /*
     查询返回
     {
	"advanceQueryConditions": [],
	"advanceQuerySorts": [],
	"exhibitDatas": [数据],
	"exportAllData": false,
	"exportColumns": [],
	"exportDataIsProcessed": false,
	"exportDatas": [],
	"exportFileName": "",
	"exportType": "",
	"fastQueryParameters": {},
	"isExport": false,
	"isSuccess": true,
	"nowPage": 1,
	"pageCount": 20,
	"pageSize": 10,
	"parameters": {},
	"recordCount": 200,
	"startRecord": 0
     }
     */
    #endregion
}