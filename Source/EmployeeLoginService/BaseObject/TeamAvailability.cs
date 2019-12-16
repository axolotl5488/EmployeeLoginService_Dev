using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class TeamAvailability
    {
        public string UserName { get; internal set; }
        public bool IsPunchin { get; internal set; }
        public string LatePunchinReason { get; internal set; }
        public string EarlyPunchoutReason { get; internal set; }
        public string Latlong { get; internal set; }
        public string In_Latlong_Image { get; internal set; }
        public string Out_Latlong_Image { get; internal set; }
        public string OutLatlong { get; internal set; }
        public string DepartmentName { get; internal set; }
        public string PunchinTime { get; internal set; }
        public string PunchoutTime { get; internal set; }
        public string OutLocationReason { get; internal set; }
        public string OutLocationReason_PunchOut { get; internal set; }
        public string MobileNo { get; internal set; }

        public string OutLocation_Name { get; internal set; }
        public string InLocation_Name { get; internal set; }
    }
}