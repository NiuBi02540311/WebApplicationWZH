using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApplicationWZH.Controllers
{

    //但存储文件时名称以BodyPart_打头，不是上传的真实文件名，需要重构原始的MultipartFormDataStreamProvider方法，如下
    //public class MultipartFileWithExtensionStreamProvider : MultipartFileStreamProvider
    //{
    //    public MultipartFileWithExtensionStreamProvider(string rootPath) : base(rootPath) { }
    //    public override string GetLocalFileName(HttpContentHeaders headers)
    //    {
    //        return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
    //    }
    //}
    public class Response
    {
        public string responseCode { get; set; }

        public string responseMessage { get; set; }
    }
    public class FileInfo
    {
        public string responseCode { get; set; }

        public string FileName { get; set; }

        public string FileOldName { get; set; }

        public string FileNewName { get; set; }

        public string FilePath { get; set; }
    }
    public class MyMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public MyMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string tid = HttpContext.Current.Request.Headers["tid"];
            //这里获取上传的文件名 aa.xlsx
            string Name = headers.ContentDisposition.FileName.Replace("\"", string.Empty);

            //这里做了一个判断，只有jpg,png,gif为后缀的，才给保存，否则抛出一个错误(写这个判断的原因是因为需求原因)
            if (Name.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase)  ||
                Name.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase)  ||
                Name.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase)  ||
                Name.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase)
                )  
            {
                //以ContentDisposition的哈希值加上传的名字作为文件名
                //return $"{headers.ContentDisposition.GetHashCode()}_{Name}";
                return $"{tid}_{Name}";
            }
            throw new InvalidOperationException("上传格式错误");
        }
    }

//————————————————
//版权声明：本文为CSDN博主「csdn怀」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
//原文链接：https://blog.csdn.net/u013783095/article/details/102450963
    public class UploadController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string tid = HttpContext.Current.Request.Headers["tid"];

            //string root = HttpContext.Current.Server.MapPath("~/App_Data");
            string root = HttpContext.Current.Server.MapPath("~/Content") + "\\img";
            
            List<string> files = new List<string>();
            try
            {
                //var provider = new MultipartFileWithExtensionStreamProvider(root);
                var provider = new MyMultipartFormDataStreamProvider(root);
                //var provider = new MultipartFormDataStreamProvider(root);
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                //var data =  await Request.Content.ReadAsStreamAsync();

                var f = new FileInfo();

                //This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    files.Add(Path.GetFileName(file.LocalFileName));

                    f.responseCode = "200";
                    f.FileOldName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    f.FileNewName = $"{tid}_{file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty)}";
                    f.FilePath =  file.LocalFileName;
                }

                //foreach (MultipartFileData file in provider.FileData)
                //{
                //    files.Add(Path.GetFileName(file.LocalFileName));
                //}

                //var response = new Response { responseCode = "200", responseMessage = "{}" };
                //return Request.CreateResponse<Response>(HttpStatusCode.OK, response);
                  return Request.CreateResponse(HttpStatusCode.OK, f);

                //// var resp = Request.CreateResponse (HttpStatusCode.OK,  new Response() { responseCode = "200", responseMessage = "" });
                //return resp;

                //return Request.CreateResponse(HttpStatusCode.OK, files);
                //————————————————
                //版权声明：本文为CSDN博主「csdn怀」的原创文章，遵循CC 4.0 BY - SA版权协议，转载请附上原文出处链接及本声明。
                //原文链接：httsps://blog.csdn.net/u013783095/article/details/102450963
                //eturn Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }



        /// <summary>
        /// Headers操作示例
        /// </summary>
        [HttpGet]
        public HttpResponseMessage OperHeaders()
        {
            //获取请求头信息
            string info = HttpContext.Current.Request.Headers["My-Headers-Info"];

            //返回响应结果
            HttpResponseMessage result = new HttpResponseMessage();
            result.Content = new StringContent("请求头信息为：" + info);

            //添加响应头信息
            result.Headers.Add("Access-Control-Expose-Headers", "My-Headers-Info");
            result.Headers.Add("My-Headers-Info", "ABC123");

            return result;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        //[HttpGet]
        //public HttpResponseMessage DownloadFile()
        //{
        //    string fileName = "报表模板.xlsx";
        //    string filePath = HttpContext.Current.Server.MapPath("~/") + "FileRoot\\" + "ReportTemplate.xlsx";
        //    FileStream stream = new FileStream(filePath, FileMode.Open);
        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //    response.Content = new StreamContent(stream);
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //    {
        //        FileName = HttpUtility.UrlEncode(fileName)
        //    };
        //    response.Headers.Add("Access-Control-Expose-Headers", "FileName");
        //    response.Headers.Add("FileName", HttpUtility.UrlEncode(fileName));
        //    return response;
        //    //<a href="http://localhost:51170/api/File/DownloadFile">下载模板</a>
        //}
        //————————————————
        //版权声明：本文为CSDN博主「pan_junbiao」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
        //原文链接：https://blog.csdn.net/pan_junbiao/article/details/84065952
    }
   
}