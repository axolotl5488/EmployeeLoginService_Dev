using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BAModel;
using DataModel;
using System.Configuration;
using System.Data.Entity;
using BAModel.Common;
using static BAModel.Common.AppEnum;
using System.Timers;
using FirebaseNet.Messaging;

namespace AxolotlPushService
{
    public partial class Service1 : ServiceBase
    {
        #region Variables
        private int timerMinuteInterval = 0;
        private Timer timerOnMinute = null;

        #endregion

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                string activityName = "OnStart:";
                Trace.TraceInformation("{0}Status Checking service starting...", activityName);


                this.timerMinuteInterval = int.Parse(ConfigurationManager.AppSettings["timerInterval_OneMinutes"]);

                //runTimer for EventStatus Update  on 1 Hour
                timerOnMinute = new Timer();
                timerOnMinute.Interval = timerMinuteInterval;
                timerOnMinute.Elapsed += new ElapsedEventHandler(TimerOneMinute_Tick);
                timerOnMinute.Enabled = true;


                Trace.TraceInformation("{0}Completed initialization of service", activityName);

            }
            catch (Exception ex)
            {
                Trace.TraceInformation("OnStart -- Error at {0}", ex.Message);
            }
        }

        protected override void OnStop()
        {
            string activityName = "OnStop:";
            //this.timerOnMinute.Enabled = false;

            Trace.TraceInformation("{0}Stopping  Check service...", activityName);
            //we need to drain the queue if any outstanding notifications

            Trace.TraceInformation("{0}Shutting down, goodbye", activityName);
        }

        private void TimerOneMinute_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                SendPushNotification();
                AutoClockOut();
                PunchInReminder();
                PunchOutReminder();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("TimerOneMinute_Tick Error:: Message::"+ex.Message+", Date:: "+DateTime.UtcNow);
            }
        }


        public void PunchInReminder()
        {
            try
            {
                DateTime utctime = DateTime.UtcNow;
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime istdatrtime = TimeZoneInfo.ConvertTimeFromUtc(utctime, timeZoneInfo);
                TimeSpan isttimespan = istdatrtime.TimeOfDay;

                int PunchIn_ReminderAlert_InMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["PunchIn_ReminderAlert_InMinutes"]);

                AxolotlEntities db = new AxolotlEntities();
                List<AspNetUser> records = db.AspNetUsers.Where(x =>
                !string.IsNullOrEmpty(x.DeviceToken)).ToList();
                records = records.Where(x=> x.OfficeShiftType.Subtract(isttimespan).TotalMinutes == PunchIn_ReminderAlert_InMinutes).ToList();
                string message = "Clock time is due in 30 min";
                string messagetype = MessageTye.PunchInReMinder.ToString();
                foreach (AspNetUser obj in records)
                {
                    Common.AddNotification(obj.CompanyID, obj.Company.Name, obj.DeviceToken, null, null, null, null, obj.Id, obj.FirstName + " " + obj.LastName,
                        obj.DeviceID, obj.DeviceType, message, messagetype);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("PunchInReminder Error:: Message::" + ex.Message + ", Date:: " + DateTime.UtcNow);
            }
        }

        public void PunchOutReminder()
        {
            try
            {
                DateTime utctime = DateTime.UtcNow;
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime istdatrtime = TimeZoneInfo.ConvertTimeFromUtc(utctime, timeZoneInfo);
                int PunchOut_ReminderAlert_InMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["PunchOut_ReminderAlert_InMinutes"]);

                AxolotlEntities db = new AxolotlEntities();
                List<EmployeePunch> todayspunchins = db.EmployeePunches.Where(x => x.IsOutSidePunchOut == true &&
                x.ClockOutTime == null && DbFunctions.TruncateTime(x.ClockInTime) == DbFunctions.TruncateTime(istdatrtime))
                .Include(x => x.AspNetUser)
                .ToList();

                string message = "You can end your day at work in 5 mins";
                string messagetype = MessageTye.PunchOutReMinder.ToString();
                foreach (EmployeePunch objpunch in todayspunchins)
                {
                    Company objcompany = objpunch.AspNetUser.Company;
                    DateTime expectedouttime = objpunch.ClockInTime.AddMinutes(objcompany.WorkingHoursInMinutes);
                    if (istdatrtime.Subtract(expectedouttime).TotalMinutes == PunchOut_ReminderAlert_InMinutes)
                    {
                        AspNetUser obj = objpunch.AspNetUser;
                        Common.AddNotification(obj.CompanyID, obj.Company.Name, obj.DeviceToken, null, null, null, null, obj.Id, obj.FirstName + " " + obj.LastName,
                            obj.DeviceID, obj.DeviceType, message, messagetype);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("PunchOutReminder Error:: Message::" + ex.Message + ", Date:: " + DateTime.UtcNow);
            }
        }

        public void SendPushNotification()
        {
            try
            {
                string FCM_ServerKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FCM_ServerKey"]);
                FCMClient client = new FCMClient(FCM_ServerKey);

                AxolotlEntities db = new AxolotlEntities();

                List<Notification> records = db.Notifications.Where(x => x.HasSent == false).ToList();
                foreach (Notification obj in records)
                {
                    obj.HasSent = true;
                    obj.SentDate = DateTime.UtcNow;
                    obj.DateModified = DateTime.UtcNow;
                    db.SaveChanges();

                    Message message = new Message()
                    {

                        To = obj.DeviceToken,
                        Priority = MessagePriority.high,
                        TimeToLive = 2419200,
                        Notification = new AndroidNotification()
                        {
                            Body = obj.Message,
                            Title = obj.MessageType,
                            Icon = "",
                            Sound = "default",
                        },
                        Data = new Dictionary<string, string>
                                            {
                                                { "CompanyID",obj.CompanyID.ToString()},
                                                { "CompanyName", obj.CompanyName },
                                                { "EmployeeCallID", Convert.ToString(obj.EmployeeCallID) },
                                                { "EmployeeLeaveID", Convert.ToString(obj.EmployeeLeaveID) },
                                                { "EmployeePunchID", Convert.ToString(obj.EmployeePunchID) },
                                                { "EmployeeTaskID", Convert.ToString(obj.EmployeeTaskID) },
                                                { "ID", Convert.ToString(obj.ID) },
                                                { "Message", Convert.ToString(obj.Message) },
                                                { "MessageType", Convert.ToString(obj.MessageType) },
                                                { "UserID", Convert.ToString(obj.UserID) },
                                                { "UserName", Convert.ToString(obj.UserName) },
                                                { "ContentType", "text" },
                                            }
                    };

                    client.SendMessageAsync(message).Wait();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("SendPushNotification Error:: Message::" + ex.Message + ", Date:: " + DateTime.UtcNow);
            }
        }

        public void AutoClockOut()
        {
            try
            {
                DateTime utctime = DateTime.UtcNow;
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime istdatrtime = TimeZoneInfo.ConvertTimeFromUtc(utctime, timeZoneInfo);

                if (istdatrtime.Hour == 0 && istdatrtime.Minute == 0)
                {
                    AxolotlEntities db = new AxolotlEntities();
                    List<EmployeePunch> records = db.EmployeePunches.Where(x => x.ClockOutTime == null).ToList();
                    foreach(EmployeePunch obj in records)
                    {
                        obj.DateModified = DateTime.UtcNow;
                        obj.IsSystemClockOut = true;
                        obj.ClockOutTime = obj.ClockInTime;

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("AutoClockOut Error:: Message::" + ex.Message + ", Date:: " + DateTime.UtcNow);
            }
        }

        public void test()
        {
            PunchInReminder();
        }
    }
}
