using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class MenuWithComponentDetail
    {
        public int PageId { get; internal set; }

        public string PageName { get; internal set; }

        public List<ComponentDetail> Comp { get; internal set; }
    }

    public class ComponentDetail
    {
        public int ComponentId { get; internal set; }

        public string ComponentName { get; internal set; }

        public int PageId { get; internal set; }

        public string PageName { get; internal set; }

    }

    public class UserComponentDetail
    {
        public int UserId { get; internal set; }
        public int ComponentId { get; internal set; }

        public string ComponentName { get; internal set; }

        public int PageId { get; internal set; }

        public string PageName { get; internal set; }

    }
}