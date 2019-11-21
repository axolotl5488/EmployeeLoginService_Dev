using System;

namespace EmployeeLoginService.BaseObject
{
    public class PunchInDetail
    {
        public int PId { get; set; }

        public int UserId { get; set; }

        public string PunchinTime { get; set; }

        public string PunchoutTime { get; set; }

        public TimeSpan WorkingHour { get; set; }
    }

    public class PunchInListDetail
    {
        public string UserName { get; set; }

        public string PunchInTime { get; set; }

        public string PunchOutTime { get; set; }

        public string WorkingHour { get; set; }

        public string PunchinType { get; set; }

        public string PunchoutType { get; set; }

        public DateTime? Tempdate { get; set; }

        public string PunchinAddress { get; set; }

        public string PunchoutAddress { get; set; }

        public string ExtraWorkingHour { get; set; }

        public int LatePunchin { get; set; }

        public int EarlyPunchout { get; set; }

        public string PunchinLatlong { get; set; }

        public string punchoutLatlong { get; set; }

    }
}