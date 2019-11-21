using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Xml.Linq;
using DataModel;
using EmployeeLoginService.BaseObject;

namespace EmployeeLoginAPI.Models
{
    public class GeneralFunction
    {
        internal Login CheckValidUser(string UserName, string Password, string DeviceId, string DeviceType)
        {
            var LD = new Login();

            LD.ReturnResult = string.Empty;
            LD.FailureReason = string.Empty;

            int ValidUserId = 0;

            using (var context = new EmployeeLoginEntities())
            {
                // Check Username and Password
                var CheckUser = context.Users.Where(s =>
                           (s.UserName == UserName) &&
                           (s.Password == Password) &&
                           (s.Status == 1));
                if (CheckUser.Count() == 1)
                {
                    try
                    {
                        ValidUserId = CheckUser.First().UserId;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }

                if (ValidUserId != 0)
                {


                    if (CheckUser.First().RoleId == 1)
                    {
                        LD.ReturnType = "success";
                        LD.UserId = ValidUserId;
                        LD.Mobile = (from u in context.Users where u.UserId == ValidUserId select u.MobileNoCmp).FirstOrDefault();
                        LD.UDId = -1;
                    }
                    else
                    {
                        //Check Aproval DeviceId current user
                        var CheckDevice = context.UserDevices.Where(s => (s.UserId == ValidUserId) && (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId) && (s.IsApproved == true));
                        if (CheckDevice.Count() == 1)
                        {
                            LD.ReturnType = "success";
                            LD.UserId = ValidUserId;
                            LD.Mobile = (from u in context.Users where u.UserId == ValidUserId select u.MobileNoCmp).FirstOrDefault();
                            LD.UDId = CheckDevice.FirstOrDefault().UDId;
                        }
                        else
                        {
                            //Check Aproval DeviceId for another user
                            var CheckDeviceOtherUser = context.UserDevices.Join(context.Users, o => o.UserId, i => i.UserId, (o, i) => new { o, i }).Where(s => (s.o.DeviceType == DeviceType) && (s.o.DeviceId == DeviceId) && (s.o.DeviceType != "PC") && (s.o.IsApproved == true));
                            if (CheckDeviceOtherUser.Count() == 1)
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "The device is approved for another username. Contact Administrator to approve your device.";
                                LD.FailureReason = "login";
                            }
                            else
                            {
                                var CheckDeviceExist = context.UserDevices.Where(s => (s.UserId == ValidUserId) && (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId) && (s.IsDeleted == false));
                                if (CheckDeviceExist.Count() > 0)
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "The device is not approved. Contact Administrator to approve your device.";
                                    LD.FailureReason = "login";
                                }
                                else
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "The device is not registered. Please register your device.";
                                    LD.FailureReason = "login";

                                }
                            }
                        }
                    }
                }
                else
                {
                    LD.ReturnType = "failure";
                    LD.ReturnResult = "The username or password you entered is incorrect. ";
                    LD.FailureReason = "login";
                }
            }

            return LD;
        }
        internal void LogException(Exception exc, string source)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(ms);
            writer.Write("Hello its my sample file");

            writer.WriteLine("********** {0} **********", DateTime.Now);

            if (exc.InnerException != null)
            {
                writer.Write("Inner Exception Type: ");
                writer.WriteLine(exc.InnerException.GetType().ToString());
                writer.Write("Inner Exception: ");
                writer.WriteLine(exc.InnerException.Message);
                writer.Write("Inner Source: ");
                writer.WriteLine(exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    writer.WriteLine("Inner Stack Trace: ");
                    writer.WriteLine(exc.InnerException.StackTrace);
                }
            }
            writer.Write("Exception Type: ");
            writer.WriteLine(exc.GetType().ToString());
            writer.WriteLine("Exception: " + exc.Message);
            writer.WriteLine("Source: " + source);
            writer.WriteLine("Stack Trace: ");
            if (exc.StackTrace != null)
            {
                writer.WriteLine(exc.StackTrace);
                writer.WriteLine();
            }

            writer.Flush();
            writer.Dispose();

            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Plain);
            System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms, ct);
            attach.ContentDisposition.FileName = "myFile.txt";

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("hardik@nktpl.com");
                mail.To.Add("hardik@nktpl.com");
                mail.Subject = "NKTPL Login Server";
                mail.Body = "PFA of error";

                mail.Attachments.Add(attach);

                SmtpServer.Port = 25;
                //SmtpServer.Credentials = new System.Net.NetworkCredential("hardik.linathinfotech@gmail.com", "linath123$");
                SmtpServer.Credentials = new System.Net.NetworkCredential("hardik@nktpl.com", "hardik5488");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                // I guess you know how to send email with an attachment
                // after sending email
                ms.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        internal int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]
            if (sinceLastDay < 0) sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay) count++;

            return count;
        }
        internal int CountAlternativeDays(DayOfWeek day, DateTime start, DateTime end, string alternativeDay)
        {
            int daycount = 0;
            foreach (var ad in alternativeDay.Split(','))
            {
                var tempdate = new DateTime(start.Year, start.Month, 1);
                for (; tempdate <= end;)
                {
                    tempdate = tempdate.Next(day).AddDays((Convert.ToInt32(ad) - 1) * 7);
                    if (start <= tempdate && tempdate <= end)
                    {
                        daycount++;
                    }
                    var gg = new DateTime(tempdate.Year, tempdate.Month, 1).AddMonths(1);
                    tempdate = gg;
                }
            }
            return daycount;
        }
        internal List<HierarchyUserDetail> GetChildren(EmployeeLoginEntities context, int parentId)
        {
            return context.Users
                    .Where(c => c.TopId == parentId && c.Status == 1)
                    .Select(c => new
                    {
                        UserId = c.UserId,
                        UserName = c.FirstName + " " + c.LastName,
                        TopId = c.TopId,
                        ProfilePic = c.ProfilePic,
                        GroupName = (from g in context.GroupMasters where g.ID == c.GroupID select g.GroupName).FirstOrDefault(),
                        TopName = (from u in context.Users where u.UserId == c.TopId select u.UserName).FirstOrDefault(),
                        RoleName = (from r in context.RoleMasters where r.RoleId == c.RoleId select r.RoleName).FirstOrDefault(),
                        DeviceCount = (from ud in context.UserDevices where ud.UserId == c.UserId select ud.UserId).Count(),
                        CompanyName = (from ic in context.Companies where ic.CompanyId == c.CompanyID select ic.CompanyName).FirstOrDefault(),
                        LeaveApprovalTypeName = (from la in context.LeaveApprovalTypes where la.LeaveApprovalTypeId == c.LeaveApprovalTypeId select la.LeaveApprovalTypeName).FirstOrDefault(),
                        LeaveApprovalSecondLevelName = (from u in context.Users where u.UserId == c.LeaveApprovalSecondLevelId select u.UserName).FirstOrDefault(),
                        Weekoff = string.Join(",", (context.UserWeekOffDays.Where(w => (w.UserId == c.UserId) && (w.EndDate == null)).Select(s => s.WeekOffDay + "(" + (s.AlternativeDay == null ? "All" : s.AlternativeDay) + ")"))),
                        IsEmployeeAddress = (from ea in context.EmployeeAddresses where ea.UserID == c.UserId select ea.ID).Count() > 0 ? "True" : "False",
                        IsEmployeeBankDetails = (from ea in context.EmployeeBankDetails where ea.UserID == c.UserId select ea.AccountId).Count() > 0 ? "True" : "False",
                        IsEmployeeEducationDetails = (from ea in context.EmployeeEducationDetails where ea.UserID == c.UserId select ea.EduId).Count() > 0 ? "True" : "False",
                        IsEmployeeFamily = (from ea in context.EmployeeFamilies where ea.UserID == c.UserId select ea.ID).Count() > 0 ? "True" : "False",
                        IsEmployeePersonalIDs = (from ea in context.EmployeePersonalIDs where ea.UserID == c.UserId select ea.ID).Count() > 0 ? "True" : "False",
                        IsPersonalInfo = (from ea in context.Users where ea.GenderTypeId != null && ea.DateOfBirth != null && ea.ProfilePic != null && ea.MaritalStatus != null && ea.City != null && ea.State != null && ea.Country != null && ea.Pincode != null && ea.EmailIDPer != null && ea.MobileNoCmp != null && ea.UserId == c.UserId select ea.UserId).Count() > 0 ? "True" : "False",
                        //  UsrDevice = (from ud in context.UserDevices where ud.UserId == c.UserId select ud).ToList(),
                        Children = GetChildren(context, c.UserId)
                    })
                             .AsEnumerable()
                            .Select(c => new HierarchyUserDetail()
                            {
                                UserId = c.UserId,
                                UserName = c.UserName,
                                TopId = c.TopId,
                                GroupName = c.GroupName,
                                TopName = c.TopName,
                                RoleName = c.RoleName,
                                DeviceCount = c.DeviceCount,
                                ProfilePic = c.ProfilePic,
                                CompanyName = c.CompanyName,
                                //DefaultFrom = (c.DefaultFrom != null ? c.DefaultFrom.ToString("hh:mm tt") + " To " + c.DefaultTo.ToString("hh:mm tt") : "Not Available"),
                                //WorkHourInString = c.WorkHour != null ? (c.WorkHour.Hours != 0 ? c.WorkHour.Hours + " Hours " : "") + (c.WorkHour.Minutes != 0 ? c.WorkHour.Minutes + " Minutes " : "") : "Not Available",
                                LeaveApprovalTypeName = c.LeaveApprovalTypeName,
                                LeaveApprovalSecondLevelName = c.LeaveApprovalSecondLevelName,
                                Weekoff = c.Weekoff == "" ? "Not Available" : c.Weekoff,
                                IsEmployeeAddress = c.IsEmployeeAddress,
                                IsEmployeeBankDetails = c.IsEmployeeBankDetails,
                                IsEmployeeEducationDetails = c.IsEmployeeEducationDetails,
                                IsEmployeeFamily = c.IsEmployeeFamily,
                                IsEmployeePersonalIDs = c.IsEmployeePersonalIDs,
                                IsPersonalInfo = c.IsPersonalInfo,
                                // UsrDevice = c.UsrDevice,
                                Children = c.Children,
                            }).ToList();
        }
        internal void HieararchyWalk(List<HierarchyUserDetail> uRDetail, List<UserDetail> final)
        {
            if (uRDetail != null)
            {
                foreach (var item in uRDetail)
                {
                    final.Add(new UserDetail()
                    {
                        UserId = item.UserId,
                        UserName = item.UserName,
                        FirstName = item.FirstName,
                        LastName = item.LatName,
                        Password = item.Password,
                        EmailID = item.EmailID,
                        TopId = item.TopId,
                        TopName = item.TopName,
                        RoleName = item.RoleName,
                        DeviceCount = item.DeviceCount,
                        ProfilePic = item.ProfilePic,
                        CompanyName = item.CompanyName,
                        GroupName = item.GroupName,
                        wtimings = item.wtimings,
                        WOffDay = item.WOffDay,
                        Weekoff = item.Weekoff == "" ? "Not Available" : item.Weekoff,
                        IsEmployeeAddress = item.IsEmployeeAddress,
                        IsEmployeeBankDetails = item.IsEmployeeBankDetails,
                        IsEmployeeEducationDetails = item.IsEmployeeEducationDetails,
                        IsEmployeeFamily = item.IsEmployeeFamily,
                        IsEmployeePersonalIDs = item.IsEmployeePersonalIDs,
                        // UsrDevice = item.UsrDevice,
                        IsPersonalInfo = item.IsPersonalInfo,
                    });
                    HieararchyWalk(item.Children, final);
                }
            }
        }
        internal bool ShowAllCompanyDetail(EmployeeLoginEntities context, int UserId)
        {
            if ((from u in context.Users where u.UserId == UserId && u.RoleId == -1 select u.RoleId).Count() > 0)
                return true;
            else
                return false;
        }
        //Android push message to GCM server method
        internal void AndroidPush(string RegIds, string Data, string NotificationType)
        {
            // applicationID means google Api key
            var applicationID = "AAAA0MaMILU:APA91bG_WicYOTgWO6oPJB5zQqM3zqfLNjIsPlRDPiSx3mVYvXQmeBPZUPfcSZbtiCNPZDGpSLOjs7B7xckBcfzop_o851WqYjht5UKtZa3W6nBcMfWAomdn5O_b6bkXYQhKsyokpyQM";
            // SENDER_ID is nothing but your ProjectID (from API Console- google code)//
            var SENDER_ID = "896684269749";

            WebRequest tRequest;

            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

            tRequest.Method = "post";

            tRequest.ContentType = "application/json";

            //  tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";

            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":86400,\"delay_while_idle\":false,\"data\": { \"message\" : " + Data + ",\"NType\": " + "\"" + NotificationType + "\"},\"registration_ids\":" + RegIds + "}";

            ////Data post to server
            //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_ids=" + regId + "";

            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            tRequest.ContentLength = byteArray.Length;

            Stream dataStream = tRequest.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);

            dataStream.Close();

            WebResponse tResponse = tRequest.GetResponse();

            dataStream = tResponse.GetResponseStream();

            StreamReader tReader = new StreamReader(dataStream);

            String sResponseFromServer = tReader.ReadToEnd();   //Get response from GCM server.

            tReader.Close();

            dataStream.Close();
            tResponse.Close();
        }
        internal void IOSPush(string RegIds, string Data, string NotificationType)
        {
            // applicationID means google Api key
            var applicationID = "AAAA0MaMILU:APA91bG_WicYOTgWO6oPJB5zQqM3zqfLNjIsPlRDPiSx3mVYvXQmeBPZUPfcSZbtiCNPZDGpSLOjs7B7xckBcfzop_o851WqYjht5UKtZa3W6nBcMfWAomdn5O_b6bkXYQhKsyokpyQM";
            // SENDER_ID is nothing but your ProjectID (from API Console- google code)//
            var SENDER_ID = "896684269749";

            WebRequest tRequest;

            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

            tRequest.Method = "post";

            tRequest.ContentType = "application/json";

            //  tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";

            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":86400,\"delay_while_idle\":false,\"data\": { \"message\" : " + Data + ",\"NType\": " + "\"" + NotificationType + "\"},\"registration_ids\":" + RegIds + "}";

            ////Data post to server
            //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_ids=" + regId + "";

            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            tRequest.ContentLength = byteArray.Length;

            Stream dataStream = tRequest.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);

            dataStream.Close();

            WebResponse tResponse = tRequest.GetResponse();

            dataStream = tResponse.GetResponseStream();

            StreamReader tReader = new StreamReader(dataStream);

            String sResponseFromServer = tReader.ReadToEnd();   //Get response from GCM server.

            tReader.Close();

            dataStream.Close();
            tResponse.Close();
        }
        //internal void AndroidPushNew(string RegIds, string Data)
        //{
        //    //Create the web request with fire base API  
        //    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //    tRequest.Method = "post";
        //    //serverKey - Key from Firebase cloud messaging server  
        //    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
        //    //Sender Id - From firebase project setting  
        //    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
        //    tRequest.ContentType = "application/json";
        //    var payload = new
        //    {
        //        to = deviceId,
        //        priority = "high",
        //        content_available = true,
        //        notification = new
        //        {
        //            body = txtmsg,
        //            title = txttitle.Replace(":", ""),
        //            sound = "sound.caf",
        //            badge = badgeCounter
        //        },
        //    };
        //    var serializer = new JavaScriptSerializer();
        //    Byte[] byteArray = Encoding.UTF8.GetBytes(payload);
        //    tRequest.ContentLength = byteArray.Length;
        //    using (Stream dataStream = tRequest.GetRequestStream())
        //    {
        //        dataStream.Write(byteArray, 0, byteArray.Length);
        //        using (WebResponse tResponse = tRequest.GetResponse())
        //        {
        //            using (Stream dataStreamResponse = tResponse.GetResponseStream())
        //            {
        //                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
        //                    {
        //                        String sResponseFromServer = tReader.ReadToEnd();
        //                        result.Response = sResponseFromServer;
        //                    }
        //            }
        //        }
        //    }

        //    tReader.Close();

        //    dataStream.Close();
        //    tResponse.Close();
        //}
        internal int GetTotalWeekoffDay(List<IGrouping<string, UserWeekOffDay>> WeekoffDayWiseList, DateTime Start, DateTime Finish)
        {
            var ReportStart = Start;
            var ReportFinish = Finish;
            var TotalweekoffDay = 0;

            foreach (var getWeekoffList in WeekoffDayWiseList)
            {
                ReportStart = Start;
                ReportFinish = Finish;
                foreach (var wo in getWeekoffList)
                {
                    if (wo.EndDate != null)
                    {
                        if ((wo.CreatedDate.Date <= Start.Date && Start.Date <= wo.EndDate.Value.Date) && (wo.CreatedDate.Date <= Finish.Date && Finish.Date <= wo.EndDate.Value.Date))
                        {
                            Console.Write("fdgfdgfdg");
                            TotalweekoffDay += wo.IsAlternative ? CountAlternativeDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish, wo.AlternativeDay) : CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish);
                            break;
                        }
                        else if ((wo.CreatedDate.Date <= Start.Date && Start.Date < wo.EndDate.Value.Date))
                        {
                            ReportFinish = wo.EndDate.Value.AddDays(-1);
                            TotalweekoffDay += wo.IsAlternative ? CountAlternativeDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish, wo.AlternativeDay) : CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish);
                            ReportStart = wo.EndDate.Value;
                        }
                        if (wo.CreatedDate.Date >= Start.Date && Start.Date < wo.EndDate.Value.Date && wo.EndDate.Value.Date < Finish.Date)
                        {
                            ReportStart = wo.CreatedDate;
                            ReportFinish = wo.EndDate.Value.AddDays(-1);
                            TotalweekoffDay += wo.IsAlternative ? CountAlternativeDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish, wo.AlternativeDay) : CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish);
                            ReportStart = wo.EndDate.Value;
                        }
                        if ((wo.CreatedDate.Date <= Finish.Date && Finish.Date <= wo.EndDate.Value.Date))
                        {
                            ReportStart = wo.CreatedDate;
                            ReportFinish = Finish.Date;
                            TotalweekoffDay += wo.IsAlternative ? CountAlternativeDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish, wo.AlternativeDay) : CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish);
                        }
                    }
                    else
                    {
                        if ((wo.CreatedDate.Date <= Start.Date))
                        {
                            ReportFinish = Finish.Date;
                            TotalweekoffDay += wo.IsAlternative ? CountAlternativeDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish, wo.AlternativeDay) : CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), wo.WeekOffDay), ReportStart, ReportFinish);
                        }
                    }
                }
            }
            return TotalweekoffDay;
        }
        internal bool TodayIsWeekoff(List<UserWeekOffDay> WeekoffList)
        {
            var IsValid = false;

            foreach (var weekoffday in WeekoffList)
            {
                if (weekoffday.WeekOffDay == DateTime.Today.DayOfWeek.ToString())
                {
                    if (weekoffday.IsAlternative)
                    {
                        if (CountAlternativeDays(DateTime.Today.DayOfWeek, DateTime.Today, DateTime.Today, weekoffday.AlternativeDay) > 0)
                        {
                            IsValid = true;
                        }

                    }
                    else
                    {
                        IsValid = true;
                    }
                }
            }

            return IsValid;
        }
        internal void SendEmail(string Body, string Functionality, string Parameters)
        {
            //  MailMessage  mailMessage = new MailMessage("gorakhia.jimmy8@gmail.com", "gorakhia.jimmy8@yahoo.in");
            // MailMessage mailMessage = new MailMessage("websiteinq@nktpl.com", "hardik@nktpl.com");
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("websiteinq@nktpl.com", "Epunch Errors");
            mailMessage.To.Add("hardik@nktpl.com");
            // Specify the email body
            // string str = Convert.ToString(sb);
            //   string str = "Contact Name :" + txtName.Text + "<br> Email Address :" + txtEmail.Text + "<br> Contact No :" + txtPhone.Text + "<br> Message : " + txtMessage.Text;
            mailMessage.Subject = "Error in " + Functionality + " Dated " + DateTime.Now;
            mailMessage.Body = string.Format(Body) + Parameters;
            mailMessage.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            //smtp.Host = "smtp.gmail.com";
            //  smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("websiteinq@nktpl.com", "reset1234");
            smtp.Send(mailMessage);
        }
        internal void SendEmailNotification(string SenderName, string EmailTo, string Body, string Subject)
        {
            try
            {
                //  MailMessage  mailMessage = new MailMessage("gorakhia.jimmy8@gmail.com", "gorakhia.jimmy8@yahoo.in");
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]), SenderName);
                mailMessage.To.Add(EmailTo);
                mailMessage.Subject = Convert.ToString(Subject);
                mailMessage.Body = Body;
                mailMessage.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(Convert.ToString(ConfigurationManager.AppSettings["SMTP"]), Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]), Convert.ToString(ConfigurationManager.AppSettings["EmailFromPwd"]));
                smtp.Send(mailMessage);
            }
            catch (Exception ex)
            {

                SendEmail(ex.ToString(), "Send Email", "Email" + EmailTo + "::: Body:" + Body + "::: Subject:" + Subject);
            }


        }
        internal string RetrieveFormatedAddress(string lat, string lng)
        {
            string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
            string location = string.Empty;
            string requestUri = string.Format(baseUri, lat, lng);

            using (WebClient wc = new WebClient())
            {
                string result = wc.DownloadString(requestUri);
                var xmlElm = XElement.Parse(result);
                var status = (from elm in xmlElm.Descendants()
                              where elm.Name == "status"
                              select elm).FirstOrDefault();
                if (status.Value.ToLower() == "ok")
                {
                    var res = (from elm in xmlElm.Descendants()
                               where elm.Name == "formatted_address"
                               select elm).FirstOrDefault();
                    requestUri = res.Value;
                }
            }

            return requestUri;
        }
        //internal string SMSSend(string uid, string pwd, string gsmsenderid, string mob, string msg)
        //{
        //    string strRequest = "username=" + uid + "&pass=" + pwd + "&senderid=" + gsmsenderid + "&to=" + mob + "&msg=" + msg + "&priority=0&dnd=1&unicode=0";
        //    string url = "http://smsidea.co.in/smsstatuswithid.aspx?";

        //    // https://www.smsidea.co.in/smsstatuswithid.aspx?mobile=your_username&pass=your_password
        //    //     &senderid = your_senderid & to = your_recipient1 & msg = your_msg
        //    string Result_FromSMS = "";
        //    StreamWriter myWriter = null;
        //    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url + strRequest);
        //    objRequest.Method = "GET";
        //    objRequest.ContentLength = strRequest.Length;
        //    objRequest.ContentType = "application/x-www-form-urlencoded";
        //    myWriter = new StreamWriter(objRequest.GetRequestStream());
        //    myWriter.Write(strRequest);
        //    myWriter.Close();
        //    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        //    using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
        //    {
        //        Result_FromSMS = sr.ReadToEnd();
        //        sr.Close();
        //    }
        //    return Result_FromSMS;
        //}
        public string SMSSend(string uid, string pwd, string gsmsenderid, string mob, string msg)
        {
            string strRequest = "mobile=" + uid + "&pass=" + pwd + "&senderid=" + gsmsenderid + "&to=" + mob + "&msg=" + msg + "&priority=0&dnd=1&unicode=0";
            string url = "https://www.smsidea.co.in/smsstatuswithid.aspx?";
            Uri targetUri = new Uri(url + strRequest);
            HttpWebRequest webRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(targetUri);
            webRequest.Method = WebRequestMethods.Http.Get;
            try
            {
                string webResponse = string.Empty;
                using (HttpWebResponse getresponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(getresponse.GetResponseStream()))
                    {
                        webResponse = reader.ReadToEnd();
                        reader.Close();
                    }
                    getresponse.Close();
                }
                return webResponse;
            }
            catch (System.Net.WebException ex)
            {
                return "Request-Timeout";
            }
            catch (Exception ex)
            {
                return "error";
            }
            finally { webRequest.Abort(); }
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }
    }
}