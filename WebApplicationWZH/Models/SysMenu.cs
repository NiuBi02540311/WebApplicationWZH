using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationWZH.Models
{
    public class SysMenu
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int  MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public int ParentTid { get; set; }
        public int OrderRule { get; set; }
        public int MenuLevel { get; set; }
        //public int lClass { get; set; }
        public int IsActive { get; set; }
        
    }

    public class SysRole
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int OrderRule { get; set; } = 0;
        //public int lClass { get; set; }
        public int IsActive { get; set; } = 1;

    }

    public class SysUserRole
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Tid { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; } = 0;
        //public int lClass { get; set; }
        public int IsActive { get; set; } = 1;

    }

    public class SysMenuRole
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Tid { get; set; }
        public int MenuID { get; set; }
        public int RoleID { get; set; } = 0;
        //public int lClass { get; set; }
        public int IsActive { get; set; } = 1;

    }
}