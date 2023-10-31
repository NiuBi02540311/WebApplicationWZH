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
        public int  Tid { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public int ParentTid { get; set; }
        public int OrderRule { get; set; }
        public int Level { get; set; }
        public int lClass { get; set; }
        public int IsActive { get; set; }
        
    }
}