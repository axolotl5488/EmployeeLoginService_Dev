using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BAModel;

namespace EmployeePunchService
{
    public partial class Service1 : ServiceBase
    {
        private int OneMinutesInterval = 0;
        private Timer OneMinuteTimer = null;
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

                this.OneMinutesInterval = int.Parse(ConfigurationManager.AppSettings["OneMinute_Interval"]);

                //runTimer for EventStatus Update  on 1 Hour
                OneMinuteTimer = new Timer();
                OneMinuteTimer.Interval = OneMinutesInterval;
                OneMinuteTimer.Elapsed += new ElapsedEventHandler(TimerOneMinute_Tick);
                OneMinuteTimer.Enabled = true;

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
                DateTime currentDatetime = DateTime.Now;
                if (currentDatetime.Hour == 12 && currentDatetime.Minute == 30)
                {
                    Trace.TraceInformation("TimerOneMinute_Tick Process Start:: Datetime::" + DateTime.Now);
                    BAModel.BAModel.CheckAndSetEmployeePunchOutAsSystem();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("TimerOneMinute_Tick Error:: Datetime::"+ DateTime.Now+", Message:: "+ ex.Message);
            }
        }

        public void test()
        {
            TimerOneMinute_Tick(null,null);
        }

    }
}
