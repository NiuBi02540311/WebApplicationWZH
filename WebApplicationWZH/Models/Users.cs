using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationWZH.Models
{
    public class Users
    {

        //string sql = $@"SELECT   [UserID]
        //                  ,[UserName]
        //                  ,[PassWord]
        //                  ,[ChineseName]
        //                  ,[Status]
        //              FROM [WEBAPI].[dbo].[Users] where IsDelete = 0 and UserName = '{logInView.loginName}' and PassWord = '{logInView.loginPassword}'";

        [Column(IsIdentity = true, IsPrimary = true)]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string ChineseName { get; set; }
        public int Status { get; set; }
        public int IsDelete { get; set; }
         
        public DateTime CreateTime { get; set; }


    }
}