using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginAPI.Models
{
   
    public class UpdateToken_request
    {
               public    string UserName{ get; set; }
               public    string Password{ get; set; }
               public    string DeviceId{ get; set; }
               public    string DeviceType{ get; set; }
               public    int CompanyId{ get; set; }
               public    string AppVersion{ get; set; }
               public    int BreakCategoryId{ get; set; }
               public    string BreakCategoryName{ get; set; }
               public    bool IsActive{ get; set; }
               public    bool HasTextbox { get; set; }
    }
}