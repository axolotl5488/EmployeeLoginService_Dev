using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService
{
    public class GroupDetail
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public string GroupCode { get; set; }

        public string Description { get; set; }

        public int CompanyID { get; set; }

        public int Status { get; set; }

        public string CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public string CompanyName { get; internal set; }

        public int UserCount { get; internal set; }

        public List<string> GrpUsrName { get; internal set; }

    }
}