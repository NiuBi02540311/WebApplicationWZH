using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationWZH.Models
{
    public class ButtonPermission
    {
        public int Tid { get; set; }
        public int RoleID { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ButtonID { get; set; }

        public bool Permission { get; set; } // 0 = 可用 ，1 = 不可用

        public DateTime CreateTime { get; set; }
        public int IsDelete { get; set; } = 0;
    }
}