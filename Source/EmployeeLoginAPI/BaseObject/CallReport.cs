using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class CallReport
    {
        public string AverageWorkingHour { get; internal set; }
        public int DayInMonth { get; internal set; }
        public List<PunchinDetailList> DetailList { get; internal set; }
        public int HalfDay { get; internal set; }
        public int Holiday { get; internal set; }
        public int LeaveDay { get; internal set; }
        public string MonthName { get; internal set; }
        public int PunchinOnHoliday { get; internal set; }
        public int PunchinOnWeekoff { get; internal set; }
        public int TotalAbsentDay { get; internal set; }

        public int TotalEarlyPunchout { get; internal set; }

        public int TotalLatePunchin { get; internal set; }

        public int TotalPresentDay { get; internal set; }

        public int TotalPunchinOutside { get; internal set; }

        public int TotalPunchoutOutside { get; internal set; }

        public int TotalSystemPunchout { get; internal set; }
        public int TotalWeekoff { get; internal set; }
        public int TotalWorkingDay { get; internal set; }

        public string TotalWorkingHour { get; internal set; }

        public int UncomletedWorkhour { get; internal set; }

        public string UserName { get; internal set; }
        public int WeekoffDay { get; internal set; }
        public int WorkingDay { get; internal set; }
    }

    public class CallInDetailList
    {
        public string Date { get; internal set; }

        public string IsLocationTypePunchin { get; internal set; }

        public string IsLocationTypePunchout { get; internal set; }

        public bool IsSystemPunchout { get; internal set; }

        public string PunchinDeviceId { get; internal set; }

        public string PunchinLocation { get; internal set; }

        public string PunchinOutsideLocationReason { get; internal set; }

        public string PunchinTime { get; internal set; }

        public string PunchoutDeviceId { get; internal set; }

        public string PunchoutLocation { get; internal set; }

        public string PunchoutOutsideLocationReason { get; internal set; }

        public string PunchOutTime { get; internal set; }
        public string WorkHour { get; internal set; }
        public TimeSpan WorkHourInTimeSpan { get; internal set; }
        public string WorkHourReason { get; internal set; }
    }
}