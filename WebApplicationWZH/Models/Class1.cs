using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationWZH.Models
{
    public class Class1
    {
    }

    public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public string DataStyle { get; set; }

        public string DataValue { get; set; }

        public int OrderID { get; set; } = 0;

        public int IsMust { get; set; } = 0;
    }

    public class Note
    {
        public string SheetID { get; set; }
        public string Title { get; set; }
        public List<Movie> movies { get; set; }

    }

    public class ConfigModel
    {
        public int ID { get; set; } = 0;
        public string WipName { get; set; }
        public string Datatype { get; set; }
        public int IsMustInput { get; set; } = 0;
        public string defaultData { get; set; } 
    }
    public class WipModel
    {
        public string WipName { get; set; }
        public string WipValue { get; set; }

    }
    public class ConfigModelViewModel
    {
        public string SheetName { get; set; }
        public List<ConfigModel> WipModels { get; set; }

    }
    public class WipModelViewModel
    {
        public string SheetName { get; set; }
        public List<WipModel> WipModels { get; set; }

    }

    public class LogInViewModel
    {
        public string Id { get; set; }
        public string loginName { get; set; }
        public string loginPassword { get; set; }
    }

    public class ControllerArry
    {
        //ControllerList
         public string controlleName { get; set; }
         public List<ControllerItem> controllerItem { get; set; }
}
    public class ControllerItem
    {
        //ControllerList
        public string ActionName { get; set; }

        public string ActionType { get; set; }
    }
    /*
     {
	"SheetName": "SheetName123",
	"Models": [{
		"WipName": "检查日期",
		"WipValue": "2023-10-01"
	}, {
		"WipName": "湿度",
		"WipValue": "11"
	}, {
		"WipName": "温度",
		"WipValue": "222"
	}, {
		"WipName": "单据号",
		"WipValue": "333333"
	}, {
		"WipName": "作业员",
		"WipValue": "a"
	}]
}
     */
}