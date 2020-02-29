using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;

namespace BAModel.Common
{
    public static class Common
    {
        public static void AddAPIActivityLog(string API, DateTime StartTime, DateTime EndTime, string Request, string Response, Exception Error, bool IsSuccess)
        {
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                APILogActivity obj = new APILogActivity();
                obj.API = API;
                obj.DateCreated = DateTime.UtcNow;
                obj.EndTime = EndTime;
                obj.Error = JsonConvert.SerializeObject(Error);
                obj.IsSuccess = IsSuccess;
                obj.Request = Request;
                obj.Response = Response;
                obj.StarteTime = StartTime;

                db.APILogActivities.Add(obj);
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("AddAPIActivityLog:: Datetime::" + DateTime.UtcNow + ", Error::" + ex.Message);
            }
        }

        public static TimeSpan GetShiftTimeByType(int OfficeShifttype)
        {
            TimeSpan shifttime = TimeSpan.Parse("09:00");
            switch (OfficeShifttype)
            {
                case (int)AppEnum.OfficeShiftEnum.Shift_1:
                    shifttime = TimeSpan.Parse("09:00");
                    break;
                case (int)AppEnum.OfficeShiftEnum.Shift_2:
                    shifttime = TimeSpan.Parse("09:30");
                    break;
                case (int)AppEnum.OfficeShiftEnum.Shift_3:
                    shifttime = TimeSpan.Parse("10:00");
                    break;
                case (int)AppEnum.OfficeShiftEnum.Shift_4:
                    shifttime = TimeSpan.Parse("10:30");
                    break;
            }

            return shifttime;
        }

        public static int GetShiftTimeBySpan(string OfficeShifttype)
        {
            int shifttime = (int)AppEnum.OfficeShiftEnum.Shift_1;
            switch (OfficeShifttype)
            {
                case "09:00":
                    shifttime = (int)AppEnum.OfficeShiftEnum.Shift_1;
                    break;
                case "09:30":
                    shifttime = (int)AppEnum.OfficeShiftEnum.Shift_2;
                    break;
                case "10:00":
                    shifttime = (int)AppEnum.OfficeShiftEnum.Shift_3;
                    break;
                case "10:30":
                    shifttime = (int)AppEnum.OfficeShiftEnum.Shift_4;
                    break;
            }

            return shifttime;
        }

        public static DateTime GetDateTimeFromTimeStamp(double timestamp)
        {
            try
            {
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(timestamp / 1000d)).ToLocalTime();

                return dt;
            }
            catch (Exception ex)
            {
                return DateTime.UtcNow;
            }
        }

        public static long ConvertToTimestamp(DateTime value)
        {
            DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static void AddNotification(int CompanyID,string CompanyName,string DeviceToken,long? EmployeeCallID,
            long? EmployeeLeaveID,long? EmployeePunchID,long? EmployeeTaskID,long UserID,string UserName,string DeviceID,int? DeviceType,
            string Message,string MessageType)
        {
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                Notification obj = new Notification();
                obj.CompanyID = CompanyID;
                obj.CompanyName = CompanyName;
                obj.DateCreated = DateTime.UtcNow;
                obj.DateModified = DateTime.UtcNow;
                obj.DeviceToken = DeviceToken;
                obj.EmployeeCallID = EmployeeCallID;
                obj.EmployeeLeaveID = EmployeeLeaveID;
                obj.EmployeePunchID = EmployeePunchID;
                obj.EmployeeTaskID = EmployeeTaskID;
                obj.HasRead = false;
                obj.HasSent = false;
                obj.SentDate = null;
                obj.UserID = UserID;
                obj.UserName = UserName;
                obj.DeviceID = DeviceID;
                obj.DeviceType = DeviceType;
                obj.Message = Message;
                obj.MessageType = MessageType;

                db.Notifications.Add(obj);
                db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }
    }

    public static class AppEnum
    {
        public enum DeviceTypeEnum
        {
            Android,
            IOS
        }
        public enum OfficeShiftEnum
        {
            [Description("09:00 To 09:30")]
            Shift_1,
            [Description("09:30 To 10:00")]
            Shift_2,
            [Description("10:00 To 10:30")]
            Shift_3,
            [Description("10:30 To 11:00")]
            Shift_4,
        }

        public enum PunchTypeEnum
        {
            In = 1,
            Out = 2
        }

        public enum LeaveTypeEnum
        {
            [Description("Casual Leave")]
            CasualLeave = 1,
            [Description("Duty Leave")]
            DutyLeave = 2,
            [Description("Sick Leave")]
            SickLeave = 3,
            [Description("Leave Without Pay")]
            LeaveWithoutPay = 4,
        }

        public enum DayTypeEnum
        {
            [Description("Full Leave")]
            FullLeave = 1,
            [Description("Half Leave")]
            HalfLeave = 2
        }

        public enum LeaveStatusEnum
        {
            [Description("Pending")]
            Pending = 1,
            [Description("Sanctioned")]
            Sanctioned = 2,
            [Description("Rejected")]
            Rejected = 3,
            [Description("Reverted")]
            Reverted = 4,
            [Description("Canceled")]
            Canceled = 5
        }

        public enum CompanyDefaultRole
        {
            [Description("Admin")]
            Admin = 1,
            [Description("Employee")]
            Employee = 2
        }


        public enum AppScreen
        {
            [Description("Punch")]
            Punch = 1,
            [Description("Leave")]
            Leave = 2,
            [Description("Team")]
            Team = 3
        }

        public enum EmployeeTaskStatus
        {
            [Description("Pending")]
            Pending = 1,
            [Description("In-Progress")]
            InProgress = 2,
            [Description("Completed")]
            Completed = 3,
            [Description("Cancelled")]
            Cancelled = 4
        }

        public enum EmployeeWeekOff_Enum
        {
            [Description("Week One")]
            Week_One = 1,
            [Description("Week Two")]
            Week_Two = 2,
            [Description("Week Three")]
            Week_Three = 3,
            [Description("Week Four")]
            Week_Four = 4,
            [Description("Week Five")]
            Week_Fice = 5,
        }

        public enum EmployeeCallType
        {
            [Description("Personal")]
            Personal = 1,
            [Description("Client")]
            Client = 2,
        }

        public enum MessageTye
        {
            PunchInReMinder,
            PunchOutReMinder,
            PunchInReporting,
            LeaveCreated,
            LeaveSanctioned,
            LeaveRejected,
            LeaveReverted
        }
    }
}
