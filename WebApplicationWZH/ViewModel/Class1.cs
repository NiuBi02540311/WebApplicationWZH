using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplicationWZH.Models;
namespace WebApplicationWZH.ViewModel
{
    class Class1
    {
    }

    public class MenuViewModel
    {
       public List<SysRole> sysRoles { get; set; }
    }

    public class SysUserRoleViewModel
    {
        public string UserName { get; set; }
        public int Tid { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; } = 0;
        //public int lClass { get; set; }
         
    }
}