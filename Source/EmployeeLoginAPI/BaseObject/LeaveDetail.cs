namespace EmployeeLoginService.BaseObject
{
    public class LeaveDetail
    {
        public int ApprovalId { get; set; }

        public string Description { get; set; }

        public string FirstLevelApproved { get; set; }

        public string FirstLevelApprovedBy { get; set; }

        public string HalfDayLeave { get; set; }

        public int HolidayBetweenLeave { get; set; }

        public bool? IsApproved { get; set; }

        public string IsApprovedInText { get; set; }

        public bool IsLeaveLimitOver { get; set; }

        public int LeaveApprovalTypeLevel { get; set; }

        public int LeaveDays { get; set; }

        public string LeaveFromDate { get; set; }

        public int LeaveId { get; set; }

        public string LeaveLimitMsg { get; set; }

        public string LeaveToDate { get; set; }

        public string LeaveTypeName { get; set; }

        public string SecondLevelApproved { get; set; }

        public string SecondLevelApprovedBy { get; set; }

        public string UserName { get; set; }

        public int WeekoffBetweenLeave { get; set; }
    }
}