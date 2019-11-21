using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginAPI.Models
{
    public class Login
    {
        public string ReturnType { get; set; }

        public int UserId { get; set; }

        public string ReturnResult { get; set; }

        public string FailureReason { get; set; }

        public int UDId { get; set; }

        public int EmployeeID { get; set; }

        public int CompanyID { get; set; }

        public int CompanyName { get; set; }

        public string Mobile { get; set; }


    }
}