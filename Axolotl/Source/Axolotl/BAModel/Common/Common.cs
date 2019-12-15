using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.ComponentModel;

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
    }

    public static class AppEnum
    {
        public enum DeviceTypeEnum
        {
            Android,
            IOS
        }
        public  enum OfficeShiftEnum
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
    }
}
