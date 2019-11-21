using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class UserBankInfo
    {
        public int UserID { get; internal set; }
        public string BankAccountNo { get; internal set; }
        public string BankName { get; internal set; }
        public string BranchCode { get; internal set; }
        public string IFSCode { get; internal set; }
        public int Status { get; internal set; }
        public int Edit { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public string CreatedBy { get; internal set; }
        public DateTime UpdatedOn { get; internal set; }
        public string UpdatedBy { get; internal set; }
        public string IPAddress { get; internal set; }


    }
}