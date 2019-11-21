using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class UserPersonalIDDetail
    {

        public int UserID { get; internal set; }
        public int IDType { get; internal set; }
        public string IDNumber { get; internal set; }
        public string imageUrl { get; internal set; }


        public int Status { get; internal set; }
        public int Edit { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public string CreatedBy { get; internal set; }
        public DateTime UpdatedOn { get; internal set; }
        public string UpdatedBy { get; internal set; }
        public string IPAddress { get; internal set; }




    }
}