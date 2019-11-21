using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAModel.Model
{
    class EmployeePunchModel
    {
    }

    public class CheckUserLeaveForDay_Model
    {
        public bool IsUserApplyForLeave { get; set; }

        public int LeaveType { get; set; }

        public string LeaveTypeName { get; set; }

        public int MasterLeaveType { get; set; }

        public string MasterLeaveTypeName { get; set; }
    }

    public class Report_Model
    {

        public int userID { get; set; }

        public string UserName { get; set; }

        public string ReportMonth { get; set; }

        public string TotalWorkingHours { get; set; }

        public string AverageWorkingHours { get; set; }

        public int TotalLeaves { get; set; }

        public int TotalLatePunch_In { get; set; }

        public int TotalEarlyPunch_Out { get; set; }

        public int TotalPunchIn_Outside { get; set; }

        public int TotalPunchOut_Outside { get; set; }

        public int TotalSystemPunchOut { get; set; }

        public int TotalHolidays { get; set; }

        public int TotalWeekOffs { get; set; }

        public int TotalWorkingDays { get; set; }

        public List<PunchIn> records = new List<PunchIn>();

        public Report_Model()
        {
            records = new List<PunchIn>();
        }
    }
}
