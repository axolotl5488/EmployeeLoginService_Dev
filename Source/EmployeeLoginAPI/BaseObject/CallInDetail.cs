using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class CallInDetail
    {
        public long PId { get; set; }

        public int UserId { get; set; }

        public string CallinTime { get; set; }

        public string CalloutTime { get; set; }

        public TimeSpan WorkingHour { get; set; }
    }
    public class CallInListDetail
    {
        public string UserName { get; set; }

        public string CallInTime { get; set; }

        public string CallOutTime { get; set; }

        public string WorkingHour { get; set; }

        public string CallinType { get; set; }

        public string CallInLatelong { get; set; }

        public string CallOutLatelong { get; set; }

        public DateTime? Tempdate { get; set; }

        public string CallinAddress { get; set; }

        public string CallinTitle { get; set; }

        public string CallinDescription { get; set; }

        public string Tests { get; set; }
        


    }
}