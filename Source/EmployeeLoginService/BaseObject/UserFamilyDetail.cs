using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class UserFamilyDetail
    {
        public long ID { get; internal set; }
        public long? UserID { get; internal set; }
        public string Name { get; internal set; }
        public string DateOfBirth { get; internal set; }
        public string Relation { get; internal set; }

        public byte? Status { get; internal set; }
        public bool? Edit { get; internal set; }
        public DateTime? CreatedOn { get; internal set; }
        public string CreatedBy { get; internal set; }
        public DateTime? UpdatedOn { get; internal set; }
        public string UpdatedBy { get; internal set; }
        public string IPAddress { get; internal set; }

    }
}