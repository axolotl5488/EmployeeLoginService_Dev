using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class UserAddressDetail
    {
        public long EAID { get; internal set; }
        public  long? UserID { get; internal set; }
        public  string Address { get; internal set; }
        public  string City { get; internal set; }
        public  string State { get; internal set; }
        public  string Pincode { get; internal set; }
        public string CurrAddress { get; internal set; }
        public string CurrCity { get; internal set; }
        public string CurrState { get; internal set; }
        public string CurrPincode { get; internal set; }
        public string ImergencyName { get; internal set; }
        public string ImergencyCntNo { get; internal set; }



        public int Status { get; internal set; }
        public int Edit { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public string CreatedBy { get; internal set; }
        public DateTime UpdatedOn { get; internal set; }
        public string UpdatedBy { get; internal set; }
        public string IPAddress { get; internal set; }

    }
}