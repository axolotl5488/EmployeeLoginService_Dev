using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class WeeklyTimingDetail
    {
        public int Id { get; internal set; }
        public string TimingFor { get; internal set; }
        public string ObjectId { get; internal set; }
        public string TimingType { get; internal set; }
        public string Day { get; internal set; }
        public string DayType { get; internal set; }
        public string TimeFrom { get; internal set; }
        public string TimeUpto { get; internal set; }
        public string WorkingHours { get; internal set; }
    }
}