using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class UserPersonaInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte? GenderTypeId { get; set; }
        public string DateOfBirth { get; set; }
        public string ProfilePic { get; set; }
        public string UserCode { get; set; }
        public string MaritalStatus { get; set; }
        public string PhoneNoPer { get; set; }
        public string EmailIDPer { get; set; }
        public string UserImg { get; set; }
        public string Imgext { get; set; }
    }
}