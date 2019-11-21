using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class SubMenuDetail
    {
        public string FormName { get; internal set; }

        public bool IsActive { get; internal set; }

        public int MenuId { get; internal set; }

        public string MenuName { get; internal set; }

        public int SortNumber { get; internal set; }

        public string IconName { get; internal set; }
    }
}