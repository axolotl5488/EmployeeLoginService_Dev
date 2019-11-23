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


    public class AllEmployeeReport_Model
    {
        public double TotalWorkingHours { get; set; }
        public string TotalAverageWorkingHorus { get; set; }
        public int TotalWorkingDays { get; set; }
        public int TotalWeekOff { get; set; }
        public int TotalLeaves { get; set; }
        public int TotalHalfLeaves { get; set; }
        public int TotalSystemPunchOut { get; set; }
        public int TotalLatePunchIn { get; set; }
        public int TotalEarlyPunchOut { get; set; }
        public int TotalOutSidePunchIn { get; set; }
        public int TotalOutSidePunchOut { get; set; }

        public List<AllEmployeeReport_Detail_Model> recodrs { get; set; }

        public AllEmployeeReport_Model()
        {
            recodrs = new List<AllEmployeeReport_Detail_Model>();
        }
    }

    public class AllEmployeeReport_Detail_Model
    {
        public int TotalWeekOffDays { get; set; }
        public int TotalWorkingDays { get; set; }
        public int TotalAbsentDays { get; set; }
        public int TotalFullLeaves { get; set; }
        public int TotalHalfLeaves { get; set; }

        public double TotalWorkedHours { get; set; }
        public string AverageWorkedHours { get; set; }

        public int TotalIn_Late { get; set; }
        public int TotalIn_InsideLocation { get; set; }
        public int TotalIn_OutsideLocation { get; set; }

        public int TotalOut_Early { get; set; }
        public int TotalOut_InsideLocation { get; set; }
        public int TotalOut_OutsideLocation { get; set; }
        public int TotalSystemPunch_Out { get; set; }

        public string UserName { get; set; }


    }
}
