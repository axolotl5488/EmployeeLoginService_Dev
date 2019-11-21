using System.Collections.Generic;

namespace EmployeeLoginService.BaseObject
{
    public class MenuDetail
    {
        public string FormName { get; internal set; }

        public bool IsActive { get; internal set; }

        public int MenuId { get; internal set; }

        public int ParentMenuId { get; internal set; }

        public string MenuName { get; internal set; }

        public int SortNumber { get; internal set; }

        public string IconName { get; internal set; }

        public List<SubMenuDetail> SubMenu { get; set; }
    }
}