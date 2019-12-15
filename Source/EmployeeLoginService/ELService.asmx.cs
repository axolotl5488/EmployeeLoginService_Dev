using EmployeeLoginService.BaseObject;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web.Script.Serialization;
using System.Web.Services;
using BAModel;
using BAModel.Model;



namespace EmployeeLoginService
{
    /// <summary>
    /// Summary description for ELService
    /// </summary>
    // [WebService(Namespace = "http://linath.in/")]
    //  [WebService(Namespace = "http://192.168.0.111/")]
    [WebService(Namespace = "https://nktpl.co.uk/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the
    // following line.
    [System.Web.Script.Services.ScriptService]
    public class ELService : WebService
    {
        private GeneralFunction GF = new GeneralFunction();
        private Login LD = new Login();

        #region Services
        [WebMethod]
        public string UpdateToken(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakCategoryId, string BreakCategoryName, bool IsActive, bool HasTextbox)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (BreakCategoryId == 0)
                        {
                            BreakCategory bt = new BreakCategory();

                            bt.BreakCategoryName = BreakCategoryName;
                            bt.HasTextbox = HasTextbox;
                            bt.IsActive = IsActive;
                            bt.CreatedBy = LD.UserId;
                            bt.CreatedDate = DateTime.Now;
                            bt.UpdatedBy = LD.UserId;
                            bt.UpdatedDate = DateTime.Now;



                            context.BreakCategories.InsertOnSubmit(bt);
                            context.SubmitChanges();

                            LD.ReturnResult = "Break Type " + BreakCategoryName + " added sucessfully.";
                        }
                        else
                        {
                            var bgdetail = (from m in context.BreakCategories where m.BreakCategoryId == BreakCategoryId select m).FirstOrDefault();

                            bgdetail.BreakCategoryName = BreakCategoryName;
                            bgdetail.HasTextbox = HasTextbox;
                            bgdetail.IsActive = IsActive;
                            bgdetail.UpdatedBy = LD.UserId;
                            bgdetail.UpdatedDate = DateTime.Now;

                            context.SubmitChanges();

                            LD.ReturnResult = "Break Type " + BreakCategoryName + " update sucessfully.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        [WebMethod]
        public string AddBreakCategory(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakCategoryId, string BreakCategoryName, bool IsActive, bool HasTextbox)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (BreakCategoryId == 0)
                        {
                            BreakCategory bt = new BreakCategory();

                            bt.BreakCategoryName = BreakCategoryName;
                            bt.HasTextbox = HasTextbox;
                            bt.IsActive = IsActive;
                            bt.CreatedBy = LD.UserId;
                            bt.CreatedDate = DateTime.Now;
                            bt.UpdatedBy = LD.UserId;
                            bt.UpdatedDate = DateTime.Now;



                            context.BreakCategories.InsertOnSubmit(bt);
                            context.SubmitChanges();

                            LD.ReturnResult = "Break Type " + BreakCategoryName + " added sucessfully.";
                        }
                        else
                        {
                            var bgdetail = (from m in context.BreakCategories where m.BreakCategoryId == BreakCategoryId select m).FirstOrDefault();

                            bgdetail.BreakCategoryName = BreakCategoryName;
                            bgdetail.HasTextbox = HasTextbox;
                            bgdetail.IsActive = IsActive;
                            bgdetail.UpdatedBy = LD.UserId;
                            bgdetail.UpdatedDate = DateTime.Now;

                            context.SubmitChanges();

                            LD.ReturnResult = "Break Type " + BreakCategoryName + " update sucessfully.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddCompany(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int NewCompanyId, string CompanyName, bool IsActive,
                                        string DefaultFrom, string DefaultTo, string LunchFrom, string LunchTo, string TeaFrom, string TeaTo, int BreakCategoryId, int TotalBreakCount, string WeekoffDays, string LeaveDays, int UserLimit, bool IsUpdateLeaveType, bool IsUpdateUserWeekoff)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //check Username Password with device
                    LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                    if (LD.ReturnType == "success")
                    {
                        using (var context = new ELDBDataContext())
                        {
                            var IsExist = (from c in context.Companies where c.CompanyName == CompanyName && c.IsActive == true select c);

                            var Valid = true;
                            if (IsExist.Count() > 0)
                            {
                                if (NewCompanyId == 0)
                                    Valid = false;
                                else if (IsExist.First().CompanyId != NewCompanyId)
                                    Valid = false;
                            }

                            if (Valid == false)
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "Company " + CompanyName + " is already exists.";
                                LD.FailureReason = "";
                            }
                            else
                            {
                                if (NewCompanyId == 0)
                                {
                                    Company cmp = new Company();

                                    cmp.CompanyName = CompanyName;
                                    cmp.IsActive = IsActive;
                                    cmp.DefaultTimeFrom = Convert.ToDateTime(DefaultFrom);
                                    cmp.DefaultTimeTo = Convert.ToDateTime(DefaultTo);
                                    cmp.LunchTimeFrom = Convert.ToDateTime(LunchFrom);
                                    cmp.LunchTimeTo = Convert.ToDateTime(LunchTo);
                                    cmp.TeaTimeFrom = Convert.ToDateTime(TeaFrom);
                                    cmp.TeaTimeTo = Convert.ToDateTime(TeaTo);
                                    cmp.BreakCategoryId = BreakCategoryId;
                                    cmp.TotalBreakCount = TotalBreakCount;
                                    cmp.UserLimit = UserLimit;
                                    cmp.CreatedBy = LD.UserId;
                                    cmp.CreatedDate = DateTime.Now;
                                    cmp.UpdatedBy = LD.UserId;
                                    cmp.UpdatedDate = DateTime.Now;
                                    context.Companies.InsertOnSubmit(cmp);
                                    context.SubmitChanges();



                                    //List<CompanyWeekOffDay> cwd = new List<CompanyWeekOffDay>();
                                    //JavaScriptSerializer js = new JavaScriptSerializer();
                                    //var LstObj = js.Deserialize<List<List<string>>>(WeekoffDays);
                                    //foreach (var LO in LstObj)
                                    //{
                                    //    var AD = String.Join(",", LO.GetRange(2, LO.Count() - 2));

                                    //    cwd.Add(new CompanyWeekOffDay
                                    //    {
                                    //        CompanyId = cmp.CompanyId,
                                    //        WeekOffDay = LO[0],
                                    //        IsAlternative = Convert.ToBoolean(LO[1]),
                                    //        AlternativeDay = AD != "" ? AD : null,
                                    //        CreatedBy = LD.UserId,
                                    //        CreatedDate = DateTime.Now,
                                    //    });
                                    //}
                                    //context.CompanyWeekOffDays.InsertAllOnSubmit(cwd);

                                    //List<CompanyLeaveTypeCount> cltc = new List<CompanyLeaveTypeCount>();

                                    //var clLstObj = js.Deserialize<List<List<string>>>(LeaveDays);
                                    //foreach (var LO in clLstObj)
                                    //{
                                    //    cltc.Add(new CompanyLeaveTypeCount
                                    //    {
                                    //        CompanyId = cmp.CompanyId,
                                    //        LeaveTypeId = Convert.ToInt32(LO[0]),
                                    //        LeaveCountByType = Convert.ToInt32(LO[1]),
                                    //        CreatedBy = LD.UserId,
                                    //        CreatedDate = DateTime.Now,
                                    //        UpdatedBy = LD.UserId,
                                    //        UpdatedDate = DateTime.Now,
                                    //    });
                                    //}
                                    //context.CompanyLeaveTypeCounts.InsertAllOnSubmit(cltc);

                                    //context.SubmitChanges();

                                    LD.ReturnResult = "Company " + CompanyName + " added sucessfully.";



                                    GroupMaster g = new GroupMaster();

                                    g.GroupName = "Super Admin";
                                    g.GroupCode = "SUPERADMIN";
                                    g.CreatedBy = LD.UserId;
                                    g.CompanyID = cmp.CompanyId;
                                    g.CreatedDate = Convert.ToDateTime(DateTime.Now);
                                    g.Status = IsActive;
                                    context.GroupMasters.InsertOnSubmit(g);
                                    context.SubmitChanges();


                                    //Create Super Admin 


                                    User usr = new User();

                                    usr.UserName = "~" + CompanyName.Replace(" ", string.Empty).ToLower();
                                    usr.Password = usr.UserName + "@#";
                                    usr.RoleId = 1;
                                    usr.TopId = -1;
                                    usr.CompanyID = cmp.CompanyId;
                                    usr.Status = 1;
                                    usr.GroupID = g.ID;
                                    usr.FirstName = cmp.CompanyName;
                                    usr.LastName = "~SU";
                                    usr.EmailID = Convert.ToString(ConfigurationManager.AppSettings["CmpAdminEmail"]);
                                    usr.MobileNoCmp = Convert.ToString(ConfigurationManager.AppSettings["CmpAdminPhone"]); ;
                                    usr.Status = 1;
                                    usr.CreatedBy = LD.UserId;
                                    usr.CreatedDate = DateTime.Now;
                                    usr.CreatedBy = LD.UserId;
                                    usr.CreatedDate = DateTime.Now;
                                    usr.UpdatedBy = LD.UserId;
                                    usr.UpdatedDate = DateTime.Now;
                                    context.Users.InsertOnSubmit(usr);
                                    context.SubmitChanges();

                                    string[] Weektime = { "Monday", "Tuesday", "Wednesday", "Thrusday", "Friday", "Saturday", "Sunday" };
                                    foreach (var item in Weektime)
                                    {
                                        var Wtime = new WeeklyTiming();
                                        Wtime.UserID = usr.UserId;
                                        Wtime.TimingFor = 3;
                                        Wtime.ObjectId = -1;
                                        Wtime.TimingType = "WorkHours";
                                        Wtime.Day = item;
                                        Wtime.DayType = 1;
                                        Wtime.TimeFrom = Convert.ToDateTime("2019-02-20 08:00:00");
                                        Wtime.TimeUpto = Convert.ToDateTime("2019-02-20 09:00:00");
                                        Wtime.WorkingHours = Wtime.TimeUpto.Subtract(Wtime.TimeFrom);
                                        Wtime.Status = true;
                                        Wtime.Edit = false;
                                        Wtime.CreatedOn = DateTime.Now;
                                        Wtime.CreatedBy = usr.UserId;
                                        Wtime.UpdatedOn = DateTime.Now;
                                        Wtime.UpdatedBy = usr.UserId;
                                        Wtime.IPAddress = "";
                                        context.WeeklyTimings.InsertOnSubmit(Wtime);
                                        context.SubmitChanges();
                                    }



                                }
                                else
                                {
                                    var UserCount = context.Users.Where(w => (w.CompanyID == NewCompanyId) && (w.Status == 1)).Count();
                                    if (UserCount <= UserLimit)
                                    {
                                        var cmpdetail = (from c in context.Companies where c.CompanyId == NewCompanyId select c).FirstOrDefault();
                                        cmpdetail.CompanyName = CompanyName;
                                        cmpdetail.IsActive = IsActive;
                                        cmpdetail.DefaultTimeFrom = Convert.ToDateTime(DefaultFrom);
                                        cmpdetail.DefaultTimeTo = Convert.ToDateTime(DefaultTo);
                                        cmpdetail.LunchTimeFrom = Convert.ToDateTime(LunchFrom);
                                        cmpdetail.LunchTimeTo = Convert.ToDateTime(LunchTo);
                                        cmpdetail.TeaTimeFrom = Convert.ToDateTime(TeaFrom);
                                        cmpdetail.TeaTimeTo = Convert.ToDateTime(TeaTo);
                                        cmpdetail.BreakCategoryId = BreakCategoryId;
                                        cmpdetail.TotalBreakCount = TotalBreakCount;
                                        cmpdetail.UserLimit = UserLimit;
                                        cmpdetail.UpdatedBy = LD.UserId;
                                        cmpdetail.UpdatedDate = DateTime.Now;
                                        context.SubmitChanges();
                                        //List<CompanyWeekOffDay> cwd = new List<CompanyWeekOffDay>();
                                        //context.CompanyWeekOffDays.Where(x => (x.CompanyId == cmpdetail.CompanyId) && (x.EndDate == null)).ToList().ForEach(e => e.EndDate = DateTime.Now);

                                        //JavaScriptSerializer js = new JavaScriptSerializer();
                                        //var LstObj = js.Deserialize<List<List<string>>>(WeekoffDays);
                                        //foreach (var LO in LstObj)
                                        //{
                                        //    var AD = String.Join(",", LO.GetRange(2, LO.Count() - 2));

                                        //    var UpdateDetail = context.CompanyWeekOffDays.Where(x => (x.CompanyId == cmpdetail.CompanyId) && (x.EndDate == null) &&
                                        //              (x.WeekOffDay == LO[0]) &&
                                        //              (Convert.ToBoolean(LO[1]) == true ? ((x.IsAlternative == Convert.ToBoolean(LO[1])) && (x.AlternativeDay == AD)) : (x.IsAlternative == Convert.ToBoolean(LO[1]))));

                                        //    if (UpdateDetail.Count() > 0)
                                        //    {
                                        //        UpdateDetail.FirstOrDefault().EndDate = null;
                                        //    }
                                        //    else
                                        //    {
                                        //        cwd.Add(new CompanyWeekOffDay
                                        //        {
                                        //            CompanyId = cmpdetail.CompanyId,
                                        //            WeekOffDay = LO[0],
                                        //            IsAlternative = Convert.ToBoolean(LO[1]),
                                        //            AlternativeDay = AD != "" ? AD : null,
                                        //            CreatedBy = LD.UserId,
                                        //            CreatedDate = DateTime.Now,
                                        //        });
                                        //    }

                                        //}
                                        //context.CompanyWeekOffDays.InsertAllOnSubmit(cwd);
                                        //context.SubmitChanges();


                                        //if (IsUpdateUserWeekoff)
                                        //{
                                        //    var CmpUserList = context.Users.Where(iw => iw.CompanyID == cmpdetail.CompanyId).Select(s => s.UserId).ToList();

                                        //    List<UserWeekOffDay> uwd = new List<UserWeekOffDay>();
                                        //    context.UserWeekOffDays.Where(x => CmpUserList.Contains(x.UserId) && x.EndDate == null).ToList().ForEach(e => e.EndDate = DateTime.Now);
                                        //    foreach (var user in CmpUserList)
                                        //    {
                                        //        foreach (var LO in LstObj)
                                        //        {

                                        //            var AD = String.Join(",", LO.GetRange(2, LO.Count() - 2));

                                        //            var UpdateDetail = context.UserWeekOffDays.Where(x => x.UserId == user && (x.EndDate == null) &&
                                        //                      (x.WeekOffDay == LO[0]) &&
                                        //                      (Convert.ToBoolean(LO[1]) == true ? ((x.IsAlternative == Convert.ToBoolean(LO[1])) && (x.AlternativeDay == AD)) :
                                        //                            (x.IsAlternative == Convert.ToBoolean(LO[1]))));

                                        //            if (UpdateDetail.Count() > 0)
                                        //            {
                                        //                UpdateDetail.FirstOrDefault().EndDate = null;
                                        //            }
                                        //            else
                                        //            {
                                        //                uwd.Add(new UserWeekOffDay
                                        //                {
                                        //                    UserId = user,
                                        //                    WeekOffDay = LO[0],
                                        //                    IsAlternative = Convert.ToBoolean(LO[1]),
                                        //                    AlternativeDay = AD != "" ? AD : null,
                                        //                    CreatedBy = LD.UserId,
                                        //                    CreatedDate = DateTime.Now,
                                        //                });
                                        //            }
                                        //        }
                                        //    }
                                        //    context.UserWeekOffDays.InsertAllOnSubmit(uwd);
                                        //    context.SubmitChanges();




                                        //}


                                        //var clLstObj = js.Deserialize<List<List<string>>>(LeaveDays);
                                        //foreach (var LO in clLstObj)
                                        //{
                                        //    var record = context.CompanyLeaveTypeCounts.Where(w => w.CompanyId == cmpdetail.CompanyId && w.LeaveTypeId == Convert.ToInt32(LO[0]));
                                        //    if (record.Count() > 0)
                                        //    {
                                        //        var updateRecord = record.FirstOrDefault();
                                        //        updateRecord.LeaveCountByType = Convert.ToInt32(LO[1]);
                                        //        updateRecord.UpdatedBy = LD.UserId;
                                        //        updateRecord.UpdatedDate = DateTime.Now;
                                        //        if (IsUpdateLeaveType)
                                        //            context.UserLeaveTypeCounts.Where(w => context.Users.Where(iw => iw.CompanyID == cmpdetail.CompanyId).Select(s => s.UserId)
                                        //            .Contains(w.UserId) && w.LeaveTypeId == Convert.ToInt32(LO[0])).ToList().ForEach(e => { e.LeaveCountByType = Convert.ToInt32(LO[1]); e.UpdatedBy = LD.UserId; e.UpdatedDate = DateTime.Now; });

                                        //    }
                                        //    else
                                        //    {
                                        //        CompanyLeaveTypeCount cltc = new CompanyLeaveTypeCount();

                                        //        cltc.CompanyId = cmpdetail.CompanyId;
                                        //        cltc.LeaveTypeId = Convert.ToInt32(LO[0]);
                                        //        cltc.LeaveCountByType = Convert.ToInt32(LO[1]);
                                        //        cltc.CreatedBy = cltc.UpdatedBy = LD.UserId;
                                        //        cltc.CreatedDate = cltc.UpdatedDate = DateTime.Now;
                                        //        context.CompanyLeaveTypeCounts.InsertOnSubmit(cltc);
                                        //    }
                                        //    context.SubmitChanges();
                                        //}
                                        LD.ReturnResult = "Company " + CompanyName + " update sucessfully.";
                                    }
                                    else
                                    {
                                        LD.ReturnType = "failure";
                                        LD.ReturnResult = "User limit not valid. Already " + UserCount + " users exist in " + CompanyName;
                                        LD.FailureReason = "";
                                    }
                                }
                            }
                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                string Parameters = "";
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
                //  GF.SendEmail(ex.ToString(), "Add Company", Parameters);
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddHoliday(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, string HolidayDates, string Description)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var CheckHoliday = (from hd in context.Holidays where hd.HolidayDate.Date == Convert.ToDateTime(HolidayDates).Date && hd.CompanyId == CompanyId select hd.Id).Count();
                        if (CheckHoliday == 0)
                        {
                            Holiday hl = new Holiday();
                            hl.HolidayDate = Convert.ToDateTime(HolidayDates);
                            hl.CompanyId = CompanyId;
                            hl.Description = Description;
                            hl.CreatedBy = LD.UserId;
                            hl.CreatedDate = DateTime.Now;
                            hl.UpdatedBy = LD.UserId;
                            hl.UpdatedDate = DateTime.Now;

                            context.Holidays.InsertOnSubmit(hl);
                            context.SubmitChanges();
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "Holiday is already exist.";
                            LD.FailureReason = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, string LeaveDateFrom, string LeaveDateTo, int LeaveTypeId, string Description, List<DateTime> ListHalfDay)
        {
            try
            {
                var LDFrom = Convert.ToDateTime(LeaveDateFrom);
                var LDTo = Convert.ToDateTime(LeaveDateTo);
                TimeSpan difference = LDTo - LDFrom;

                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var CheckLeave = (context.Leaves.Where(w => (w.UserId == LD.UserId) &&
                                            ((w.LeaveFromDate >= LDFrom && w.LeaveFromDate <= LDTo) ||
                                                (w.LeaveToDate >= LDFrom && w.LeaveToDate <= LDTo) || w.LeaveFromDate <= LDFrom && w.LeaveToDate >= LDTo))).Count();
                        if (CheckLeave == 0)
                        {
                            Leave l = new Leave();
                            l.UserId = LD.UserId;
                            l.LeaveTypeId = LeaveTypeId;
                            l.LeaveFromDate = LDFrom;
                            l.LeaveToDate = LDTo;
                            l.Description = Description;
                            l.WeekoffBetweenLeave = context.UserWeekOffDays.Where(w => w.UserId == LD.UserId && w.EndDate == null).ToList().AsEnumerable().
                                   Sum(s => s.IsAlternative == false ?
                                       GF.CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), s.WeekOffDay), LDFrom, LDTo) :
                                       GF.CountAlternativeDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), s.WeekOffDay), LDFrom, LDTo, s.AlternativeDay));
                            var tmpWeekofday = context.UserWeekOffDays.Where(w => w.UserId == LD.UserId && w.EndDate == null);
                            l.WeekoffId = ((tmpWeekofday.Count()) > 0 && (l.WeekoffBetweenLeave > 0)) ? string.Join(",", tmpWeekofday.AsEnumerable().Select(s => s.WeekOffId + "(" +
                                        (s.IsAlternative == false ? GF.CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), s.WeekOffDay), LDFrom, LDTo) :
                                        GF.CountAlternativeDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), s.WeekOffDay), LDFrom, LDTo, s.AlternativeDay)) +
                                        ")")) : null;
                            var tmpHolidayInLeave = context.Holidays.Where(w => w.CompanyId == CompanyId && w.HolidayDate >= LDFrom && w.HolidayDate <= LDTo).Select(s => s.Id).ToList();
                            l.HolidayBetweenLeave = tmpHolidayInLeave.Count();
                            l.HolidayId = string.Join(",", tmpHolidayInLeave);
                            l.LeaveDays = Convert.ToInt32(difference.TotalDays) + 1 - (l.WeekoffBetweenLeave + l.HolidayBetweenLeave);
                            l.UpdatedBy = l.CreatedBy = LD.UserId;
                            l.UpdatedDate = l.CreatedDate = DateTime.Now;

                            context.Leaves.InsertOnSubmit(l);
                            context.SubmitChanges();

                            List<HalfDay> hflist = new List<HalfDay>();
                            foreach (var objhf in ListHalfDay)
                            {
                                hflist.Add(new HalfDay { LeaveID = l.LeaveId, HalfDayDate = objhf });
                            }
                            context.HalfDays.InsertAllOnSubmit(hflist);
                            context.SubmitChanges();
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "leave is already exist on these dates.";
                            LD.FailureReason = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString(); ;
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddMenu(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int MenuId, string MenuName, string FormName, int SortNumber, bool IsActive, string IconName)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (MenuId == 0)
                        {
                            Menu m = new Menu();

                            m.MenuName = MenuName;
                            m.FormName = FormName;
                            m.SortNumber = SortNumber;
                            m.IconName = IconName;
                            m.IsActive = IsActive;
                            context.Menus.InsertOnSubmit(m);
                            context.SubmitChanges();

                            LD.ReturnResult = "Menu " + MenuName + " added sucessfully.";
                        }
                        else
                        {
                            var mdetail = (from m in context.Menus where m.MId == MenuId select m).FirstOrDefault();

                            mdetail.MenuName = MenuName;
                            mdetail.FormName = FormName;
                            mdetail.SortNumber = SortNumber;
                            mdetail.IconName = IconName;
                            mdetail.IsActive = IsActive;
                            context.SubmitChanges();

                            LD.ReturnResult = "Menu " + MenuName + " update sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddUserLocations(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserId, int SelectedUserDeviceId, List<int> UpdatedLocation)
        {


            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        UpdatedLocation = (from l in context.Locations.Where(l => l.IsDeleted == false) select l.LocationId).ToList();

                        var deleteUL = context.UserLocations.Where(x => (x.UserId == SelectedUserId) && (x.UserDeviceId == SelectedUserDeviceId));
                        context.UserLocations.DeleteAllOnSubmit(deleteUL);

                        var insertUL = (from p in UpdatedLocation select new UserLocation { UserId = SelectedUserId, UserDeviceId = SelectedUserDeviceId, LocationId = p, CreatedBy = LD.UserId, CreatedDate = DateTime.Now });
                        context.UserLocations.InsertAllOnSubmit(insertUL);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddUserRights(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserId, List<int> UpdatedRight)
        {
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var deleteUR = context.UserRights.Where(x => x.UserId == SelectedUserId);
                        context.UserRights.DeleteAllOnSubmit(deleteUR);

                        var insertUR = (from p in UpdatedRight select new UserRight { UserId = SelectedUserId, MenuId = p, });
                        context.UserRights.InsertAllOnSubmit(insertUR);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ApproveLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int ApprovalId, int LeaveId, int LeaveApprovalLevel, bool IsApprove, string Description)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var Desc = Description != "" ? Description : null;

                        if (ApprovalId == 0)
                        {
                            UserLeaveApproval ula = new UserLeaveApproval();
                            ula.LeaveId = LeaveId;
                            ula.IsApprovedFirst = IsApprove;
                            ula.FirstApprovedBy = LD.UserId;
                            ula.FirstDescription = Desc;
                            ula.FirstCreatedDate = ula.FirstUpdatedDate = DateTime.Now;
                            context.UserLeaveApprovals.InsertOnSubmit(ula);
                        }
                        else
                        {
                            var ula = context.UserLeaveApprovals.Where(w => w.ApprovalId == ApprovalId).FirstOrDefault();
                            if (LeaveApprovalLevel == 1)
                            {
                                ula.IsApprovedFirst = IsApprove;
                                ula.FirstApprovedBy = LD.UserId;
                                ula.FirstDescription = Desc;
                                ula.FirstUpdatedDate = DateTime.Now;
                            }
                            else if (LeaveApprovalLevel == 2)
                            {
                                if (ula.IsApprovedSecond == null)
                                    ula.SecondCreatedDate = ula.SecondUpdatedDate = DateTime.Now;
                                else
                                    ula.SecondUpdatedDate = DateTime.Now;
                                ula.IsApprovedSecond = IsApprove;
                                ula.SecondApprovedBy = LD.UserId;
                                ula.SecondDescription = Desc;
                            }
                        }
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ApproveUserDevice(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UDId, int SelectedCompanyId, bool IsApproved)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var valid = false;
                        var URDetail = (from ud in context.UserDevices
                                        where ud.UDId.Equals(UDId)
                                        select ud).FirstOrDefault();

                        var DeviceApprovedForOtherUser = (from u in context.Users
                                                          join ud in context.UserDevices on u.UserId equals ud.UserId
                                                          where (ud.DeviceId == URDetail.DeviceId) && (u.CompanyID == SelectedCompanyId) &&
                                                          (ud.DeviceType != "PC") && (ud.IsApproved == true) && (ud.UserId != URDetail.UserId)
                                                          select ud).Count();
                        if (DeviceApprovedForOtherUser == 0 && IsApproved == false)
                        {
                            valid = true;
                        }
                        else if (IsApproved)
                        {
                            valid = true;
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "This device is approved for another user.";
                            LD.FailureReason = "";
                        }
                        if (valid)
                        {
                            URDetail.IsApproved = ((IsApproved == false) ? true : false);

                            if (URDetail.IsApproved)
                            {
                                var DeviceInOtherCompany = (from u in context.Users
                                                            join ud in context.UserDevices on u.UserId equals ud.UserId
                                                            where ud.DeviceId.Equals(URDetail.DeviceId) && u.CompanyID != SelectedCompanyId
                                                            select ud).ToList();
                                foreach (var device in DeviceInOtherCompany)
                                {
                                    device.IsApproved = false;
                                    device.IsDeleted = true;
                                }

                                URDetail.ApprovedBy = LD.UserId;
                                URDetail.ApprovedDate = DateTime.Now;

                                context.SubmitChanges();
                                LD.ReturnType = "success";
                                LD.ReturnResult = "Device has been Approved Sucessfully";
                                LD.FailureReason = "";



                            }
                            else
                            {
                                URDetail.ApprovedBy = -1;
                                URDetail.ApprovedDate = null;
                                context.SubmitChanges();
                                LD.ReturnType = "success";
                                LD.ReturnResult = "Device has been disapproved Sucessfully";
                                LD.FailureReason = "";
                            }

                            try
                            {

                                #region SendEmailDeviceApproval
                                string BodyText = "";
                                if (File.Exists(Server.MapPath("~\\MailTemplates\\Device-Approval.html")))
                                {
                                    string email = "";
                                    string approvedby = "";
                                    string subject = "";
                                    email = (from u in context.Users where u.UserId == URDetail.UserId select u.UserName).FirstOrDefault();
                                    approvedby = (from u in context.Users where u.UserId == URDetail.ApprovedBy select u.FirstName + " " + u.LastName).FirstOrDefault();

                                    string company = (from c in context.Companies
                                                      join u in context.Users on c.CompanyId equals u.CompanyID
                                                      where u.UserId == URDetail.UserId
                                                      select c.CompanyName).FirstOrDefault();


                                    if (URDetail.IsApproved == true)
                                        subject = "[" + company + " +] - Congratulations, your device was approved";
                                    else
                                        subject = "[" + company + " +] - Apologies, your device was rejected";

                                    StreamReader sr = new StreamReader(Server.MapPath("~\\MailTemplates\\Device-Approval.html"));
                                    BodyText = sr.ReadToEnd();
                                    sr.Close();
                                    BodyText = BodyText.Replace("##Username##", (from u in context.Users where u.UserId == URDetail.UserId select u.FirstName + " " + u.LastName).FirstOrDefault());
                                    BodyText = BodyText.Replace("##DeviceName##", URDetail.DeviceName);
                                    BodyText = BodyText.Replace("##DeviceID##", URDetail.DeviceId);
                                    BodyText = BodyText.Replace("##OSVersion##", URDetail.OSVersion);
                                    BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
                                    BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
                                    BodyText = BodyText.Replace("##RegistrationDate##", URDetail.RequestDate.ToString());
                                    BodyText = BodyText.Replace("##RegistrationTime##", URDetail.RequestDate.TimeOfDay.ToString());
                                    BodyText = BodyText.Replace("##ApprovedBy##", approvedby != "" ? approvedby : "-");
                                    BodyText = BodyText.Replace("##ApprovedDate##", approvedby != "" ? URDetail.ApprovedDate.ToString() : "-");
                                    BodyText = BodyText.Replace("##ApprovedTime##", approvedby != "" ? URDetail.ApprovedDate.ToString() : "-");
                                    BodyText = BodyText.Replace("##Status##", URDetail.IsApproved == true ? "Approved" : "Rejected");
                                    //  GF.SendEmailNotification("NKTPL+ Device Status", Convert.ToString(email), BodyText, subject);
                                }
                                #endregion

                            }
                            catch (Exception)
                            {

                                throw;
                            }

                            context.SubmitChanges();
                        }
                    }
                }

                else
                {
                    LD.ReturnType = "failure";
                    LD.ReturnResult = "You are not an Authorised person.";
                    LD.FailureReason = "";
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string CheckCompanyStatus(string CompanyName)
        {
            var CompanyStatus = false;
            var CompanyId = 0;
            var CName = "";
            try
            {
                //check Username Password with device
                using (var context = new ELDBDataContext())
                {
                    var Company = (from c in context.Companies where c.CompanyName.Equals(CompanyName) select c);

                    if (Company.Count() > 0)
                    {
                        CompanyStatus = true;
                        var temp = Company.FirstOrDefault();
                        CompanyId = temp.CompanyId;
                        CName = temp.CompanyName;
                    }

                    LD.ReturnType = "success";
                    if (CompanyId == 0)
                    {
                        LD.ReturnResult = "Company does not exist. Contact the administrator";
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                CompanyStatus = CompanyStatus,
                CompanyId = CompanyId,
                CompanyName = CName,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string DeleteBreakCategory(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakCategoryId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var BGDetail = (from bt in context.BreakCategories
                                        where bt.BreakCategoryId == BreakCategoryId
                                        select bt).FirstOrDefault();

                        context.BreakCategories.DeleteOnSubmit(BGDetail);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public void DeleteDownloadExcel(string FileName)
        {
            var filepath = Server.MapPath(FileName);
            System.IO.File.Delete(filepath);
        }

        [WebMethod]
        public string DeleteHoliday(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int HolidayId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var HDetail = (from h in context.Holidays
                                       where h.Id.Equals(HolidayId)
                                       select h).FirstOrDefault();
                        context.Holidays.DeleteOnSubmit(HDetail);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string DeleteLocation(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int LocationId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var LDetail = (from l in context.Locations
                                       where l.LocationId.Equals(LocationId)
                                       select l).FirstOrDefault();
                        LDetail.IsDeleted = true;
                        //context.Locations.DeleteOnSubmit(LDetail);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string DeleteMenu(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int MenuId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var MDetail = (from m in context.Menus
                                       where m.MId.Equals(MenuId)
                                       select m).FirstOrDefault();
                        context.Menus.DeleteOnSubmit(MDetail);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string DeleteUser(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UserId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var URDetail = (from c in context.Users
                                        where c.UserId == UserId
                                        select c).FirstOrDefault();

                        URDetail.Status = 1;
                        URDetail.UpdatedDate = DateTime.Now;
                        URDetail.UpdatedBy = LD.UserId;
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string DeleteUserDevice(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UDId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var URDetail = (from c in context.UserDevices
                                        where c.UDId == UDId
                                        select c).FirstOrDefault();
                        URDetail.IsDeleted = true;
                        URDetail.IsApproved = false;
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetAssignLocationByUser(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            dynamic LDetail = null;

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        LDetail = (from l in context.Locations
                                   join lpin in context.LocationPins on l.LocationId equals lpin.LocationID
                                   where l.CompanyId == CompanyId
                                   select new
                                   {
                                       LocationId = l.LocationId,
                                       LocacationName = l.PlaceName,
                                       LocationImage = l.LocationImage,
                                       Latitude = lpin.Lat,
                                       Longitude = lpin.Long
                                   }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                LDetail = LDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetBreakCategoryAndLunchTimeAndTeaTime(bool BreakCategory, bool LunchAndTeaTime, int CompanyId)
        {
            List<BreakCategoryDetail> BGDetail = new List<BreakCategoryDetail>();
            dynamic LTTime = null;
            try
            {
                using (var context = new ELDBDataContext())
                {
                    if (BreakCategory)
                    {
                        BGDetail = (from b in context.BreakCategories
                                    where b.IsActive.Equals(true)
                                    select new BreakCategoryDetail
                                    {
                                        BreakCategoryId = b.BreakCategoryId,
                                        BreakCategoryName = b.BreakCategoryName,
                                        HasTextbox = b.HasTextbox,
                                    }).ToList();
                    }
                    if (LunchAndTeaTime)
                    {
                        LTTime = (from c in context.Companies
                                  where c.CompanyId == CompanyId
                                  select new
                                  {
                                      LunchTimeFrom = c.LunchTimeFrom != null ? string.Format("{0:hh\\:mm tt}", c.LunchTimeFrom) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                      LunchTimeTo = c.LunchTimeTo != null ? string.Format("{0:hh\\:mm tt}", c.LunchTimeTo) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                      TeaTimeFrom = c.TeaTimeFrom != null ? string.Format("{0:hh\\:mm tt}", c.TeaTimeFrom) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                      TeaTimeTo = c.TeaTimeTo != null ? string.Format("{0:hh\\:mm tt}", c.TeaTimeTo) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                      BreakCategoryId = c.BreakCategoryId,
                                      TotalBreakCount = c.TotalBreakCount
                                  }).FirstOrDefault();
                    }
                }
                LD.ReturnType = "success";
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                BGDetail = BGDetail,
                LTTime = LTTime,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetBreakCategoryDetailByBreakCategoryId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakCategoryId)
        {
            BreakCategoryDetail BGDetail = new BreakCategoryDetail();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        BGDetail = context.BreakCategories
                            .Where(c => c.BreakCategoryId == BreakCategoryId)
                            .Select(c => new BreakCategoryDetail()
                            {
                                BreakCategoryId = c.BreakCategoryId,
                                BreakCategoryName = c.BreakCategoryName,
                                HasTextbox = c.HasTextbox,
                                IsActive = c.IsActive,
                            }).FirstOrDefault();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                BGDetail = BGDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetCompanyDefaultTimeAndLeaveDayAndWeekoffDay(int CompanyId)
        {
            CompanyDetail CMPDetail = new CompanyDetail();
            List<WeekoffDayDetail> UWDetail = new List<WeekoffDayDetail>();
            List<LeaveTypeCountDetail> LCDetail = new List<LeaveTypeCountDetail>();
            try
            {
                using (var context = new ELDBDataContext())
                {
                    CMPDetail = (from c in context.Companies
                                 where c.CompanyId == CompanyId
                                 select new CompanyDetail()
                                 {
                                     DefaultFrom = c.DefaultTimeFrom != null ? string.Format("{0:hh\\:mm tt}", c.DefaultTimeFrom) : "09:00 AM",
                                     DefaultTo = c.DefaultTimeTo != null ? string.Format("{0:hh\\:mm tt}", c.DefaultTimeTo) : "06:00 PM",
                                 }).FirstOrDefault();

                    LCDetail = context.CompanyLeaveTypeCounts.Where(w => (w.CompanyId == CompanyId)).Select(s => new LeaveTypeCountDetail()
                    {
                        LeaveTypeId = s.LeaveTypeId,
                        LeaveCountByType = s.LeaveCountByType
                    }).ToList();

                    UWDetail = context.CompanyWeekOffDays
                          .Where(w => w.CompanyId == CompanyId && w.EndDate == null)
                          .Select(c => new WeekoffDayDetail
                          {
                              WeekOffDay = c.WeekOffDay,
                              IsAlternative = c.IsAlternative,
                              AlternativeDay = c.AlternativeDay,
                          }).ToList();
                }
                LD.ReturnType = "success";
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                CMPDetail = CMPDetail,
                LCDetail = LCDetail,
                UWDetail = UWDetail,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetCompanyDetailByCompanyId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int EditCompanyId)
        {
            CompanyDetail CMPDetail = new CompanyDetail();
            List<WeekoffDayDetail> UWDetail = new List<WeekoffDayDetail>();
            List<LeaveTypeCountDetail> LCDetail = new List<LeaveTypeCountDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        CMPDetail = context.Companies
                            .Where(c => c.CompanyId == EditCompanyId)
                            .Select(c => new CompanyDetail()
                            {
                                CompanyId = c.CompanyId,
                                CompanyName = c.CompanyName,
                                Active = c.IsActive,
                                DefaultFrom = c.DefaultTimeFrom != null ? string.Format("{0:hh\\:mm tt}", c.DefaultTimeFrom) : "09:00 AM",
                                DefaultTo = c.DefaultTimeTo != null ? string.Format("{0:hh\\:mm tt}", c.DefaultTimeTo) : "06:00 PM",
                                LunchFrom = c.LunchTimeFrom != null ? string.Format("{0:hh\\:mm tt}", c.LunchTimeFrom) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                LunchTo = c.LunchTimeTo != null ? string.Format("{0:hh\\:mm tt}", c.LunchTimeTo) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                TeaFrom = c.TeaTimeFrom != null ? string.Format("{0:hh\\:mm tt}", c.TeaTimeFrom) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                TeaTo = c.TeaTimeTo != null ? string.Format("{0:hh\\:mm tt}", c.TeaTimeTo) : string.Format("{0:hh\\:mm tt}", DateTime.Now),
                                BreakCategoryId = c.BreakCategoryId,
                                TotalBreakCount = c.TotalBreakCount,
                                UserLimit = c.UserLimit
                            })
                            .FirstOrDefault();

                        UWDetail = context.CompanyWeekOffDays
                           .Where(w => w.CompanyId == EditCompanyId && w.EndDate == null)
                           .Select(c => new WeekoffDayDetail
                           {
                               WeekOffDay = c.WeekOffDay,
                               IsAlternative = c.IsAlternative,
                               AlternativeDay = c.AlternativeDay,
                           }).ToList();

                        LCDetail = context.CompanyLeaveTypeCounts
                            .Where(w => w.CompanyId == EditCompanyId)
                            .Select(s => new LeaveTypeCountDetail
                            {
                                LeaveTypeId = s.LeaveTypeId,
                                LeaveCountByType = s.LeaveCountByType
                            }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                CMPDetail = CMPDetail,
                LCDetail = LCDetail,
                UWDetail = UWDetail,
                UWDetailCount = UWDetail.Count(),
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLeaveTypeCount(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            var LeaveTypeCountInText = "";
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var LTDetail = (from lt in context.LeaveTypes
                                        join ult in context.UserLeaveTypeCounts on lt.LeaveTypeId equals ult.LeaveTypeId
                                        where ult.UserId.Equals(LD.UserId)
                                        select lt.LeaveTypeName + " ( " +
                                          ((from l in context.Leaves
                                            join u in context.Users on l.UserId equals u.UserId
                                            join la in context.UserLeaveApprovals on l.LeaveId equals la.LeaveId into finalResult
                                            from fr in finalResult.DefaultIfEmpty()
                                            where l.LeaveTypeId.Equals(lt.LeaveTypeId) &&
                                              context.Leaves.Where(w => w.UserId.Equals(LD.UserId)).Select(s => s.LeaveId).Contains(l.LeaveId) &&
                                              (
                                                  u.LeaveApprovalTypeId.Equals(3) ||
                                                  (u.LeaveApprovalTypeId.Equals(1) && fr.LeaveId == l.LeaveId && fr.IsApprovedFirst == true) ||
                                                  (u.LeaveApprovalTypeId.Equals(2) && fr.LeaveId == l.LeaveId && fr.IsApprovedFirst == true && fr.IsApprovedSecond == true)
                                              )
                                            select l.LeaveDays).Count() > 0 ?
                                                  (from l in context.Leaves
                                                   join u in context.Users on l.UserId equals u.UserId
                                                   join la in context.UserLeaveApprovals on l.LeaveId equals la.LeaveId into finalResult
                                                   from fr in finalResult.DefaultIfEmpty()
                                                   where l.LeaveTypeId.Equals(lt.LeaveTypeId) &&
                                                         context.Leaves.Where(w => w.UserId.Equals(LD.UserId)).Select(s => s.LeaveId).Contains(l.LeaveId) &&
                                                         (
                                                             u.LeaveApprovalTypeId.Equals(3) ||
                                                             (u.LeaveApprovalTypeId.Equals(1) && fr.LeaveId == l.LeaveId && fr.IsApprovedFirst == true) ||
                                                             (u.LeaveApprovalTypeId.Equals(2) && fr.LeaveId == l.LeaveId && fr.IsApprovedFirst == true && fr.IsApprovedSecond == true)
                                                         )
                                                   select l.LeaveDays).Sum() : 0) + " / " + ult.LeaveCountByType + " ) ").ToList();

                        LeaveTypeCountInText = string.Join("\r\n", LTDetail);
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                LeaveTypeCountInText = LeaveTypeCountInText,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLeaveTypeDetail()
        {
            dynamic LTDetail = null;
            try
            {
                using (var context = new ELDBDataContext())
                {
                    LTDetail = context.LeaveTypes.Select(s => new { LeaveTypeId = s.LeaveTypeId, LeaveTypeName = s.LeaveTypeName }).ToList();
                }
                LD.ReturnType = "success";
            }
            catch (Exception)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                LTDetail = LTDetail,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLocationByUserId(int UserId, int UserDeviceId, string DeviceType, int CompanyId)
        {
            List<LocationDetail> LDetail = new List<LocationDetail>();

            try
            {
                using (var context = new ELDBDataContext())
                {
                    LDetail = context.Locations.Where(p => (p.IsDeleted == false) && (p.CompanyId == CompanyId))
                    .Select(l => new LocationDetail()
                    {
                        PlaceName = l.PlaceName,
                        LocationId = l.LocationId,
                        IsChecked = context.UserLocations.Where(p => (p.UserId == UserId) && (p.LocationId == l.LocationId) && (p.UserDeviceId == UserDeviceId)).Select(p => p.ULId).Count() > 0 ? true : false,
                    }).ToList();
                }
                LD.ReturnType = "success";
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                LDetail = LDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLocationDetailByLocationId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int LocationId)
        {
            LocationDetail LDetail = new LocationDetail();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        LDetail = context.Locations
                            .Where(c => c.LocationId == LocationId)
                            .Select(c => new LocationDetail()
                            {
                                LocationId = c.LocationId,
                                PlaceName = c.PlaceName,
                                Latitude = c.Latitude,
                                Longitude = c.Longitude,
                                LocationAddress = c.LocationName,
                                //   BreakCategoryId = c.BreakCategoryId,
                                //BreakCategoryName = context.BreakCategories.Where(p => (p.BreakCategoryId == c.BreakCategoryId)).Select(p => p.BreakCategoryName).FirstOrDefault(),
                                //LunchFrom = string.Format("{0:hh\\:mm tt}", c.LunchTimeFrom),
                                //LunchTo = string.Format("{0:hh\\:mm tt}", c.LunchTimeTo),
                                //TeaFrom = string.Format("{0:hh\\:mm tt}", c.TeaTimeFrom),
                                //TeaTo = string.Format("{0:hh\\:mm tt}", c.TeaTimeTo),
                                CompanyId = c.CompanyId,
                                //TotalBreakCount = c.TotalBreakCount,
                            }).FirstOrDefault();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                LDetail = LDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetMenuDetailByMenuId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int EditMenuId)
        {
            MenuDetail MDetail = new MenuDetail();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        MDetail = context.Menus
                            .Where(c => c.MId == EditMenuId)
                            .Select(c => new MenuDetail()
                            {
                                MenuId = c.MId,
                                MenuName = c.MenuName,
                                FormName = c.FormName,
                                IconName = c.IconName,
                                SortNumber = c.SortNumber.Value,
                                IsActive = c.IsActive,
                            }).FirstOrDefault();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                MDetail = MDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetRoleAndCompany(int RoleId)
        {
            List<RoleDetail> RDetail = new List<RoleDetail>();
            dynamic CDetail = null;

            try
            {
                //check Username Password with device

                using (var context = new ELDBDataContext())
                {
                    RDetail = (from r in context.RoleMasters
                               where r.RoleId != 1 && r.RoleId != 2 && r.RoleId != 3
                               select new RoleDetail()
                               {
                                   RoleId = r.RoleId,
                                   RoleName = r.RoleName,
                               }).ToList();

                    // RDetail.Insert(0, new RoleDetail() { RoleId = 0, RoleName = "Select Role" });

                    if (RoleId == 1)
                    {
                        CDetail = (from c in context.Companies
                                   where c.IsActive == true
                                   select new
                                   {
                                       CompanyId = c.CompanyId,
                                       CompanyName = c.CompanyName,
                                   }).ToList();
                        CDetail.Insert(0, new { CompanyId = 0, CompanyName = "Select Company Name" });
                    }
                }
                LD.ReturnType = "success";
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                RDetail = RDetail,
                CDetail = CDetail,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetSecondLevelApprovalList(int SelectedRoleId, int CompanyId)
        {
            dynamic UDetail = null;
            try
            {
                //check Username Password with device

                using (var context = new ELDBDataContext())
                {
                    UDetail = (from u in context.Users
                               where u.RoleId == (SelectedRoleId - 2) && u.CompanyID == CompanyId
                               select new
                               {
                                   UserId = u.UserId,
                                   UserName = u.UserName,
                               }).ToList();

                    //var top = "Select Employee";
                    //if (UDetail.Count == 0)
                    //    top = "Employee not exist";
                    //UDetail.Insert(0, new { UserId = 0, UserName = top });
                }
                LD.ReturnType = "success";
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                UDetail = UDetail,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetUserDetailByUserId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UserId)
        {
            UserDetail URDetail = new UserDetail();
            List<WeekoffDayDetail> UWDetail = new List<WeekoffDayDetail>();
            List<LeaveTypeCountDetail> LCDetail = new List<LeaveTypeCountDetail>();
            List<WeeklyTiming> UWTDetail = new List<WeeklyTiming>();
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        URDetail = context.Users
                            .Where(c => c.UserId == UserId)
                            .Select(c => new UserDetail()
                            {
                                UserId = c.UserId,
                                UserName = c.UserName,
                                Password = c.Password,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                EmailID = c.EmailID,
                                MobileNoCmp = c.MobileNoCmp,
                                TopId = c.TopId,
                                TopName = context.Users.Where(w => w.UserId == c.TopId).FirstOrDefault().FirstName + " " + c.LastName,
                                RoleId = c.RoleId,
                                RoleName = context.RoleMasters.Where(w => w.RoleId == c.RoleId).FirstOrDefault().RoleName,
                                CompanyId = c.CompanyID,
                                CompanyName = context.Companies.Where(w => w.CompanyId == c.CompanyID).FirstOrDefault().CompanyName,
                            }).FirstOrDefault();

                        UWDetail = context.UserWeekOffDays
                            .Where(w => w.UserId == UserId && w.EndDate == null)
                            .Select(c => new WeekoffDayDetail
                            {
                                WeekOffDay = c.WeekOffDay,
                                IsAlternative = c.IsAlternative,
                                AlternativeDay = c.AlternativeDay,
                            }).ToList();




                        UWTDetail = context.WeeklyTimings
                            .Where(w => w.ObjectId == UserId && w.TimingFor == 3 && w.TimingType == "WorkHours")
                            .Select(s => new WeeklyTiming
                            {
                                Day = s.Day,
                                TimeFrom = s.TimeFrom,
                                TimeUpto = s.TimeUpto,
                                WorkingHours = s.WorkingHours
                            }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                URDetail = URDetail,
                UWDetail = UWDetail,
                LCDetail = LCDetail,
                UWDetailCount = UWDetail.Count(),
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetUserHierachyWise(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<UserDetail> UDetail = new List<UserDetail>();
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        UDetail = context.GetUserHierarchyWise(LD.UserId).Select(p => new UserDetail { UserId = p.UserId.Value, UserName = p.UserName }).ToList();
                        UDetail.Insert(0, new UserDetail() { UserId = 0, UserName = "Select User" });
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                UDetail = UDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetUserRights(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserId)
        {
            List<int> RDetail = new List<int>();
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        RDetail = (from m in context.Menus
                                   join r in context.UserRights on m.MId equals r.MenuId
                                   where r.UserId == SelectedUserId
                                   select m.MId).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                RDetail = RDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListApprovalLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<LeaveDetail> LADetail = new List<LeaveDetail>();

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        LADetail = (from u in context.Users
                                    join l in context.Leaves on u.UserId equals l.UserId
                                    join la in context.UserLeaveApprovals on l.LeaveId equals la.LeaveId into finalDetail
                                    from fd in finalDetail.DefaultIfEmpty()
                                    where (u.TopId == LD.UserId || (u.LeaveApprovalSecondLevelId == LD.UserId && u.LeaveApprovalTypeId == 2 && u.TopId == fd.FirstApprovedBy && fd.IsApprovedFirst.Equals(true)))
                                    && u.LeaveApprovalTypeId != 3
                                    select new
                                    {
                                        LeaveId = l.LeaveId,
                                        UserName = u.UserName,
                                        LeaveFromDate = l.LeaveFromDate,
                                        LeaveToDate = l.LeaveToDate,
                                        LeaveDays = l.LeaveDays,
                                        WeekoffBetweenLeave = l.WeekoffBetweenLeave,
                                        HolidayBetweenLeave = l.HolidayBetweenLeave,
                                        Description = l.Description,
                                        LeaveApprovalTypeLevel = u.TopId == LD.UserId ? 1 : 2,
                                        ApprovalId = fd != null ? fd.ApprovalId : 0,
                                        IsApproved = fd != null ? u.TopId == LD.UserId ? fd.IsApprovedFirst : fd.IsApprovedSecond : null,
                                        LeaveTypeName = context.LeaveTypes.Where(w => w.LeaveTypeId == l.LeaveTypeId).FirstOrDefault().LeaveTypeName,
                                        ApprovedLeaveCountTypeWise = (int?)((from il in context.Leaves
                                                                             join ila in context.UserLeaveApprovals on il.LeaveId equals ila.LeaveId into finalResult
                                                                             from fr in finalResult.DefaultIfEmpty()
                                                                             where il.LeaveTypeId.Equals(l.LeaveTypeId) &&
                                                                                 context.Leaves.Where(w => w.UserId.Equals(l.UserId)).Select(s => s.LeaveId).Contains(il.LeaveId) &&
                                                                                 (
                                                                                     (u.LeaveApprovalTypeId.Equals(1) && fr.LeaveId == il.LeaveId && fr.IsApprovedFirst == true) ||
                                                                                     (u.LeaveApprovalTypeId.Equals(2) && fr.LeaveId == il.LeaveId && fr.IsApprovedFirst == true && fr.IsApprovedSecond == true)
                                                                                 )
                                                                             select il.LeaveDays).Sum()),
                                        TotalLeaveTypeWise = context.UserLeaveTypeCounts.Where(w => w.LeaveTypeId == l.LeaveTypeId && w.UserId == l.UserId).FirstOrDefault().LeaveCountByType,
                                    }).AsEnumerable()
                                    .Select(l => new LeaveDetail
                                    {
                                        LeaveId = l.LeaveId,
                                        UserName = l.UserName,
                                        LeaveFromDate = l.LeaveFromDate.ToString("dd-MMM-yyyy"),
                                        LeaveToDate = l.LeaveToDate.ToString("dd-MMM-yyyy"),
                                        LeaveDays = l.LeaveDays,
                                        WeekoffBetweenLeave = l.WeekoffBetweenLeave,
                                        HolidayBetweenLeave = l.HolidayBetweenLeave,
                                        LeaveApprovalTypeLevel = l.LeaveApprovalTypeLevel,
                                        Description = l.Description,
                                        ApprovalId = l.ApprovalId,
                                        IsApproved = l.IsApproved,
                                        IsApprovedInText = l.IsApproved != null ? (l.IsApproved == true ? "Approved" : "Rejected") : "Pending",
                                        LeaveTypeName = l.LeaveTypeName,
                                        IsLeaveLimitOver = l.ApprovedLeaveCountTypeWise != null ? l.ApprovedLeaveCountTypeWise.Value >= l.TotalLeaveTypeWise ? true : false : false,
                                        LeaveLimitMsg = l.UserName + "'s " + l.LeaveTypeName + "Limit is " + l.TotalLeaveTypeWise + " and already " + l.ApprovedLeaveCountTypeWise + " " + l.LeaveTypeName + " approved.",
                                    }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                LADetail = LADetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListBreakCategory(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<BreakCategoryDetail> BGDetail = new List<BreakCategoryDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        BGDetail = (from bt in context.BreakCategories
                                    select new BreakCategoryDetail
                                    {
                                        BreakCategoryId = bt.BreakCategoryId,
                                        BreakCategoryName = bt.BreakCategoryName,
                                        HasTextbox = bt.HasTextbox,
                                        IsActive = bt.IsActive,
                                    }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                BGDetail = BGDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListCompany(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<CompanyDetail> CMPDetail = new List<CompanyDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        CMPDetail = context.Companies
                            .Select(c => new
                            {
                                CompanyId = c.CompanyId,
                                CompanyName = c.CompanyName,
                                IsActive = c.IsActive == true ? "Active" : "InActive",
                                DefaultFrom = c.DefaultTimeFrom,
                                DefaultTo = c.DefaultTimeTo,
                                LunchFrom = c.LunchTimeFrom,
                                LunchTo = c.LunchTimeTo,
                                TeaFrom = c.TeaTimeFrom,
                                TeaTo = c.TeaTimeTo,
                                TotalBreakCount = c.TotalBreakCount,
                                UserLimit = c.UserLimit,
                                BreakCategoryName = context.BreakCategories.Where(p => (p.BreakCategoryId == c.BreakCategoryId)).Select(p => p.BreakCategoryName).FirstOrDefault(),
                                Weekoff = string.Join(",", (context.CompanyWeekOffDays.Where(w => (w.CompanyId == c.CompanyId) && (w.EndDate == null)).Select(s => s.WeekOffDay + "(" + (s.AlternativeDay == null ? "All" : s.AlternativeDay) + ")"))),
                            })
                           .AsEnumerable()
                           .Select(c => new CompanyDetail()
                           {
                               CompanyId = c.CompanyId,
                               CompanyName = c.CompanyName,
                               IsActive = c.IsActive,
                               DefaultFrom = (c.DefaultFrom != null ? c.DefaultFrom.ToString("hh:mm tt") + " To " + c.DefaultTo.ToString("hh:mm tt") : "Not Available"),
                               LunchFrom = (c.LunchFrom != null ? c.LunchFrom.ToString("hh:mm tt") + " To " + c.LunchTo.ToString("hh:mm tt") : "Not Available"),
                               TeaFrom = (c.TeaFrom != null ? c.TeaFrom.ToString("hh:mm tt") + " To " + c.TeaTo.ToString("hh:mm tt") : "Not Available"),
                               TotalBreakCountWord = c.TotalBreakCount == 0 ? "No Limit" : c.TotalBreakCount.ToString(),
                               UserLimit = c.UserLimit,
                               BreakCategoryName = c.BreakCategoryName,
                               Weekoff = c.Weekoff == "" ? "Not Available" : c.Weekoff,
                           }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                CMPDetail = CMPDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListHoliday(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<HolidayDetail> HDDetail = new List<HolidayDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (GF.ShowAllCompanyDetail(context, LD.UserId))
                        {
                            HDDetail = (from hd in context.Holidays
                                        select new
                                        {
                                            HolidayDate = hd.HolidayDate,
                                            TempDate = hd.HolidayDate,
                                            Description = hd.Description,
                                            HolidayId = hd.Id,
                                            CompanyName = (from c in context.Companies where c.CompanyId == hd.CompanyId select c.CompanyName).FirstOrDefault(),
                                        }).AsEnumerable()
                                        .Select(hd => new HolidayDetail
                                        {
                                            HolidayDate = hd.HolidayDate.ToString("dd-MMM-yyyy"),
                                            TempDate = hd.TempDate,
                                            Description = hd.Description,
                                            HolidayId = hd.HolidayId,
                                            CompanyName = hd.CompanyName
                                        }
                                        ).OrderBy(p => p.CompanyName).ThenByDescending(p => p.TempDate).ToList();
                        }
                        else
                        {
                            HDDetail = (from hd in context.Holidays
                                        select new
                                        {
                                            HolidayDate = hd.HolidayDate,
                                            TempDate = hd.HolidayDate,
                                            Description = hd.Description,
                                            HolidayId = hd.Id,
                                        }).AsEnumerable()
                                        .Select(hd => new HolidayDetail
                                        {
                                            HolidayDate = hd.HolidayDate.ToString("dd-MMM-yyyy"),
                                            TempDate = hd.TempDate,
                                            Description = hd.Description,
                                            HolidayId = hd.HolidayId,
                                        }
                                        ).OrderByDescending(p => p.TempDate).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                HDDetail = HDDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<LeaveDetail> LDetail = new List<LeaveDetail>();
            int LeaveAprrovalTypeFlag = 3;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        LeaveAprrovalTypeFlag = context.Users.Where(w => w.UserId == LD.UserId).Select(s => s.LeaveApprovalTypeId).FirstOrDefault();
                        LDetail = (from l in context.Leaves
                                   join ula in context.UserLeaveApprovals on l.LeaveId equals ula.LeaveId into finalResult
                                   from fr in finalResult.DefaultIfEmpty()
                                   where l.UserId == LD.UserId
                                   select new
                                   {
                                       LeaveFromDate = l.LeaveFromDate,
                                       LeaveToDate = l.LeaveToDate,
                                       LeaveDays = l.LeaveDays,
                                       LeaveTypeName = context.LeaveTypes.Where(w => w.LeaveTypeId == l.LeaveTypeId).FirstOrDefault().LeaveTypeName,
                                       HalfDayLeave = context.HalfDays.Where(w => w.LeaveID == l.LeaveId).Count(),
                                       ArrHalfDay = context.HalfDays.Where(w => w.LeaveID == l.LeaveId).Select(s => s.HalfDayDate.Day).ToList(),
                                       FirstLevelApproved = fr != null ? (bool?)fr.IsApprovedFirst : null,
                                       FirstLevelApprovedBy = fr != null ? context.Users.Where(w => w.UserId.Equals(fr.FirstApprovedBy)).FirstOrDefault().UserName : null,
                                       SecondLevelApproved = fr.IsApprovedSecond,
                                       SecondLevelApprovedBy = context.Users.Where(w => w.UserId.Equals(fr.SecondApprovedBy)).FirstOrDefault().UserName,
                                       Description = l.Description,
                                   }).AsEnumerable()
                                      .Select(l => new LeaveDetail
                                      {
                                          LeaveFromDate = l.LeaveFromDate.ToString("dd-MMM-yyyy"),
                                          LeaveToDate = l.LeaveToDate.ToString("dd-MMM-yyyy"),
                                          LeaveDays = l.LeaveDays,
                                          LeaveTypeName = l.LeaveTypeName,
                                          HalfDayLeave = l.HalfDayLeave > 0 ? l.HalfDayLeave + " (" + string.Join(",", l.ArrHalfDay) + ")" : "",
                                          FirstLevelApproved = l.FirstLevelApproved != null ? l.FirstLevelApproved == true ? "Approved" : "Reject" : "Pending",
                                          SecondLevelApproved = l.FirstLevelApproved != null ? l.SecondLevelApproved != null ? l.FirstLevelApproved == true ? "Approved" : "Reject" : "Pending" : "",
                                          FirstLevelApprovedBy = l.FirstLevelApprovedBy != null ? l.FirstLevelApprovedBy : "",
                                          SecondLevelApprovedBy = l.SecondLevelApprovedBy != null ? l.SecondLevelApprovedBy : "",
                                          Description = l.Description,
                                      }
                                      ).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                LDetail = LDetail,
                LeaveAprrovalTypeFlag = LeaveAprrovalTypeFlag,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListRights(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<MenuDetail> MDetail = new List<MenuDetail>();
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var RoleId = context.Users.Where(a => a.UserId == LD.UserId).Select(a => a.RoleId).FirstOrDefault();
                        if (RoleId == 1)
                        {
                            MDetail = (from m in context.Menus select new MenuDetail { MenuName = m.MenuName, MenuId = m.MId }).ToList();
                        }
                        else
                        {
                            MDetail = (from m in context.Menus
                                       join r in context.UserRights on m.MId equals r.MenuId
                                       where r.UserId == LD.UserId && m.IsActive == true
                                       select new MenuDetail { MenuName = m.MenuName, MenuId = m.MId }).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                MDetail = MDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListUsersDevice(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<UserDeviceDetail> UDDetail = new List<UserDeviceDetail>();
            bool IsAllList = false;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        IsAllList = GF.ShowAllCompanyDetail(context, LD.UserId);
                        if (IsAllList)
                        {
                            UDDetail = (from ud in context.UserDevices
                                        join u in context.Users on ud.UserId equals u.UserId
                                        where u.Status.Equals(1) && ud.IsDeleted.Equals(false) && u.UserId != LD.UserId
                                        select new UserDeviceDetail()
                                        {
                                            UDId = ud.UDId,
                                            UserId = u.UserId,
                                            UserName = u.FirstName + " " + u.LastName,
                                            DeviceId = ud.DeviceId,
                                            Description = ud.Description,
                                            DeviceType = ud.DeviceType,
                                            IsApproved = ud.IsApproved,
                                            CompanyId = u.CompanyID,
                                            OSVersion = ud.OSVersion,
                                            DeviceName = ud.DeviceName,
                                            LocationName = ud.LocationName,
                                            IPAddress = ud.IPAddress,
                                            ApprovedDate = Convert.ToString(ud.ApprovedDate),
                                            ApprovedBy = (from au in context.Users where au.UserId == ud.ApprovedBy select au.FirstName + " " + au.LastName).FirstOrDefault(),
                                            CompanyName = (from c in context.Companies where c.CompanyId == u.CompanyID select c.CompanyName).FirstOrDefault(),
                                        }).OrderBy(p => p.CompanyName).ToList();
                        }
                        else
                        {
                            UDDetail = (from ud in context.UserDevices
                                        join u in context.Users on ud.UserId equals u.UserId
                                        join ua in context.UserAccesses on u.UserId equals ua.UserAccessID
                                        where u.Status.Equals(1) && ud.IsDeleted.Equals(false) && u.CompanyID == CompanyId && u.UserId != LD.UserId && ua.UserID == LD.UserId
                                        select new UserDeviceDetail()
                                        {
                                            UDId = ud.UDId,
                                            UserId = u.UserId,
                                            UserName = u.FirstName + " " + u.LastName,
                                            DeviceId = ud.DeviceId,
                                            Description = ud.Description,
                                            DeviceType = ud.DeviceType,
                                            IsApproved = ud.IsApproved,
                                            CompanyId = u.CompanyID,
                                            OSVersion = ud.OSVersion,
                                            DeviceName = ud.DeviceName,
                                            LocationName = ud.LocationName,
                                            IPAddress = ud.IPAddress,
                                            ApprovedDate = Convert.ToString(ud.ApprovedDate),
                                            ApprovedBy = (from au in context.Users where au.UserId == ud.ApprovedBy select au.FirstName + " " + au.LastName).FirstOrDefault(),
                                            CompanyName = (from c in context.Companies where c.CompanyId == u.CompanyID select c.CompanyName).FirstOrDefault(),
                                        }).OrderBy(p => p.UDId).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                UDDetail = UDDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                IsAll = IsAllList,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string Login(string UserName, string Password, string DeviceId, string DeviceType, string AccessToken, string AppVersion)
        {
            List<MenuDetail> MDetail = new List<MenuDetail>();
            LocationDetail LDetail = new LocationDetail();
            List<WeeklyTimingDetail> WTDetail = new List<WeeklyTimingDetail>();
            var RoleId = 0;
            var CompanyID = -1;
            var CompanyName = "";
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {


                        RoleId = (from u in context.Users where u.UserId == LD.UserId select u.RoleId).FirstOrDefault();

                        CompanyID = (from u in context.Users where u.UserId == LD.UserId select u.CompanyID).FirstOrDefault();

                        CompanyName = (from u in context.Companies where u.CompanyId == CompanyID select u.CompanyName).FirstOrDefault();

                        if (RoleId != 1)
                        {
                            if (DeviceType != "PC")
                            {
                                var UpdateToken = (from ud in context.UserDevices where ud.UDId == LD.UDId select ud).FirstOrDefault();

                                if (UpdateToken.AccessToken != AccessToken)
                                {
                                    UpdateToken.AccessToken = AccessToken;
                                    context.SubmitChanges();
                                }
                            }
                        }




                        if (RoleId == 1)
                        {
                            MDetail = context.Menus.Where(m => m.PMId == -1 && m.IsActive == true).Select(s => new MenuDetail
                            {
                                MenuId = s.MId,
                                MenuName = s.MenuName,
                                FormName = s.FormName,
                                SortNumber = s.SortNumber.Value,
                                IconName = s.IconName,
                                IsActive = s.IsActive,
                                ParentMenuId = s.PMId,
                                SubMenu = (from sm in context.Menus
                                           where sm.PMId == s.MId
                                           select new SubMenuDetail()
                                           {
                                               MenuId = sm.MId,
                                               MenuName = sm.MenuName,
                                               FormName = sm.FormName,
                                               SortNumber = sm.SortNumber.Value,
                                               IconName = sm.IconName,
                                               IsActive = sm.IsActive,
                                           }).ToList()
                            }).OrderBy(o => o.SortNumber).ToList();

                            if (DeviceType == "PC")
                            {
                                LDetail = (from l in context.Locations
                                           join ul in context.UserLocations on l.LocationId equals ul.LocationId
                                           where ul.UserId == LD.UserId && ul.UserDeviceId == LD.UDId
                                           select new LocationDetail
                                           {
                                               LocationId = ul.LocationId,
                                               Latitude = l.Latitude,
                                               Longitude = l.Longitude
                                           }).FirstOrDefault();
                                if (LDetail == null)
                                {
                                    LDetail = new LocationDetail();
                                    LDetail.LocationId = 0;
                                    LDetail.Latitude = 0;
                                    LDetail.Longitude = 0;
                                    LDetail.CompanyId = CompanyID;
                                }


                            }
                        }
                        else
                        {
                            int[] mm = { 1, 18, 10 };
                            //MDetail = MDetail = context.Menus.Where(m => m.PMId == -1 && m.IsActive == true).Select(m => new MenuDetail
                            MDetail = MDetail = context.Menus.Where(m => m.PMId == -1 && m.IsActive == true && mm.Contains(m.MId)).Select(m => new MenuDetail
                            {
                                MenuName = m.MenuName,
                                FormName = m.FormName,
                                MenuId = m.MId,
                                ParentMenuId = m.PMId,
                                SubMenu = (from sm in context.Menus
                                           where sm.PMId == m.MId
                                           select new SubMenuDetail()
                                           {
                                               MenuId = sm.MId,
                                               MenuName = sm.MenuName,
                                               FormName = sm.FormName,
                                               SortNumber = sm.SortNumber.Value,
                                               IconName = sm.IconName,
                                               IsActive = sm.IsActive,
                                           }).ToList(),
                                SortNumber = m.SortNumber.Value
                            }).OrderBy(o => o.SortNumber).ToList();




                            if (DeviceType == "PC")
                            {
                                LDetail = (from l in context.Locations
                                           join ul in context.UserLocations on l.LocationId equals ul.LocationId
                                           where ul.UserId == LD.UserId && ul.UserDeviceId == LD.UDId
                                           select new LocationDetail
                                           {
                                               LocationId = ul.LocationId,
                                               Latitude = l.Latitude,
                                               Longitude = l.Longitude
                                           }).FirstOrDefault();
                            }
                        }

                        WTDetail = (from wt in context.WeeklyTimings
                                    where wt.ObjectId == LD.UserId && wt.TimingFor == 3
                                    select new WeeklyTimingDetail
                                    {
                                        Day = wt.Day,
                                        TimeFrom = Convert.ToString(wt.TimeFrom),
                                        TimeUpto = Convert.ToString(wt.TimeUpto),

                                    }).ToList();

                        //// Check rights For Admin Role..........

                        //var UserDetail = (from u in context.Users
                        //                  where u.UserId == LD.UserId
                        //                  select u.RoleId).FirstOrDefault();

                        //RoleId = UserDetail;
                        //if (UserDetail == 1)
                        //{
                        //    IsAdmin = true;
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                type = LD.ReturnType,
                msg = LD.ReturnResult,
                UserId = LD.UserId,
                EmailID = LD.EmailAddress,
                MDetail = MDetail,
                RoleId = RoleId,
                LDetail = LDetail,
                WTDetail = WTDetail,
                CompanyID = CompanyID,
                CompanyName = CompanyName

            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string RegistrationRequest(string UserName, string Password, string DeviceId, string DeviceType, string Description, string AccessToken, string OS, string DeviceName, string LocationName, string IPAddress)
        {
            var ReturnResult = "";
            var ReturnType = "success";
            try
            {
                var IsValidUser = 0;
                using (var context = new ELDBDataContext())
                {
                    var CheckUser = context.Users.Where(s =>
                        (s.UserName == UserName) &&
                        (s.Password == Password) &&
                        (s.Status == 1));
                    IsValidUser = CheckUser.Count();
                    var ResultCheckUser = CheckUser.FirstOrDefault();

                    if (IsValidUser == 1)
                    {
                        if (DeviceType == "PC")
                        {
                            var CheckDevice = context.UserDevices.Where(s => (s.UserId == ResultCheckUser.UserId) && (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId));
                            if (CheckDevice.Count() == 0)
                            {
                                UserDevice ud = new UserDevice
                                {
                                    UserId = ResultCheckUser.UserId,
                                    DeviceId = DeviceId,
                                    DeviceType = DeviceType,
                                    IsApproved = false,
                                    RequestDate = DateTime.Now,
                                    OSVersion = OS,
                                    DeviceName = DeviceName,
                                    LocationName = LocationName,
                                    IPAddress = IPAddress,
                                    Description = Description != "" ? Description : null
                                };

                                context.UserDevices.InsertOnSubmit(ud);
                                context.SubmitChanges();
                                SendEmailforDeviceApproval(ud);
                                ReturnType = "success";
                                ReturnResult = "Registration request sent sucessfully. Contact Administrator to approve your device.";
                            }
                            else
                            {
                                var ResultCheckDevice = CheckDevice.FirstOrDefault();
                                if (ResultCheckDevice.IsDeleted)
                                {
                                    UserDevice UDI = context.UserDevices.Single(s => (s.UserId == ResultCheckUser.UserId) && (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId));
                                    UDI.RequestDate = DateTime.Now;
                                    UDI.Description = Description != "" ? Description : null;
                                    UDI.IsApproved = false;
                                    UDI.IsDeleted = false;
                                    UDI.RequestDate = DateTime.Now;
                                    UDI.OSVersion = OS;
                                    UDI.DeviceName = DeviceName;
                                    UDI.LocationName = LocationName;
                                    UDI.IPAddress = IPAddress;
                                    UDI.DeviceId = DeviceId;
                                    UDI.DeviceType = DeviceType;

                                    context.SubmitChanges();
                                    SendEmailforDeviceApproval(UDI);
                                    ReturnType = "success";
                                    ReturnResult = "Registration request sent sucessfully. Contact Administrator to approve your device.";
                                }
                                else
                                {
                                    if (ResultCheckDevice.IsApproved)
                                    {
                                        ReturnType = "success";
                                        ReturnResult = UserName + " is already approved for this device";
                                    }
                                    else
                                    {
                                        UserDevice UDI = context.UserDevices.Single(s => (s.UserId == ResultCheckUser.UserId) && (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId));
                                        UDI.RequestDate = DateTime.Now;
                                        UDI.Description = Description != "" ? Description : null;
                                        UDI.IsApproved = false;
                                        UDI.IsDeleted = false;
                                        UDI.RequestDate = DateTime.Now;
                                        UDI.OSVersion = OS;
                                        UDI.IPAddress = IPAddress;
                                        UDI.DeviceName = DeviceName;
                                        UDI.LocationName = LocationName;
                                        UDI.DeviceId = DeviceId;
                                        UDI.DeviceType = DeviceType;
                                        context.SubmitChanges();

                                        SendEmailforDeviceApproval(UDI);
                                        ReturnType = "success";
                                        ReturnResult = "Registration request resent sucessfully. Contact Administrator to approve your device.";
                                    }
                                }
                            }
                        }
                        //Check For Iphone And Android
                        else
                        {
                            //  var CheckDevice = context.UserDevices.Where(s => (s.UserId == ResultCheckUser.UserId) && (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId));
                            //   var CheckDevice = context.UserDevices.Where(s => (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId));

                            var CheckDevice = (from u in context.Users
                                               join ud in context.UserDevices on u.UserId equals ud.UserId
                                               where u.CompanyID == ResultCheckUser.CompanyID && ud.DeviceId == DeviceId && ud.DeviceType == DeviceType
                                               select ud);

                            bool DeviceApproved = false;

                            foreach (var ud in CheckDevice.ToList())
                            {
                                if (ud.IsApproved)
                                {
                                    DeviceApproved = ud.IsApproved;
                                    break;
                                }
                            }
                            if (DeviceApproved)
                            {
                                if (context.UserDevices.Where(s => (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId) && (s.UserId == ResultCheckUser.UserId) && (s.IsApproved.Equals(true))).Count() == 1)
                                {
                                    ReturnType = "success";
                                    ReturnResult = "The Device is already approved.";
                                }
                                else
                                {
                                    ReturnType = "success";
                                    ReturnResult = "The Device is already approved for another user. Contact Administrator.";
                                }
                            }
                            else
                            {
                                var CheckCurrentUserDevice = context.UserDevices.Where(s => (s.DeviceType == DeviceType) && (s.DeviceId == DeviceId) && (s.UserId == ResultCheckUser.UserId));
                                if (CheckCurrentUserDevice.Count() == 0)
                                {
                                    UserDevice ud = new UserDevice
                                    {
                                        UserId = ResultCheckUser.UserId,
                                        DeviceId = DeviceId,
                                        DeviceType = DeviceType,
                                        IsApproved = false,
                                        RequestDate = DateTime.Now,
                                        OSVersion = OS,
                                        IPAddress = IPAddress,
                                        DeviceName = DeviceName,
                                        LocationName = LocationName,
                                        Description = Description != "" ? Description : null,
                                        AccessToken = AccessToken
                                    };

                                    context.UserDevices.InsertOnSubmit(ud);
                                    context.SubmitChanges();
                                    SendEmailforDeviceApproval(ud);
                                    ReturnType = "success";
                                    ReturnResult = "Registration request sent sucessfully. Contact Administrator to approve your device.";
                                }
                                else
                                {
                                    var UDI = CheckCurrentUserDevice.FirstOrDefault();
                                    if (UDI.IsDeleted)
                                    {
                                        UDI.RequestDate = DateTime.Now;
                                        UDI.Description = Description != "" ? Description : null;
                                        UDI.IsApproved = false;
                                        UDI.IsDeleted = false;
                                        UDI.OSVersion = OS;
                                        UDI.IPAddress = IPAddress;
                                        UDI.DeviceName = DeviceName;
                                        UDI.LocationName = LocationName;
                                        UDI.DeviceId = DeviceId;
                                        UDI.DeviceType = DeviceType;

                                        context.SubmitChanges();

                                        ReturnType = "success";
                                        SendEmailforDeviceApproval(UDI);
                                        ReturnResult = "Registration request sent sucessfully. Contact Administrator to approve your device.";
                                    }
                                    else
                                    {
                                        UDI.RequestDate = DateTime.Now;
                                        UDI.Description = Description != "" ? Description : null;

                                        context.SubmitChanges();

                                        ReturnType = "success";
                                        SendEmailforDeviceApproval(UDI);
                                        ReturnResult = "Registration request resent sucessfully. Contact Administrator to approve your device.";
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        ReturnType = "failure";
                        ReturnResult = "The username or password you entered is incorrect. ";
                    }
                }
            }
            catch (Exception ex)
            {
                ReturnType = "failure";
                ReturnResult = "Server Is Not Responding... ";
                //  GF.SendEmail(ex.ToString(), "Registration request", "");
            }

            var FinalResult = new
            {
                type = ReturnType,
                msg = ReturnResult
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName, int j)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[j];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            return ws;
        }

        [WebMethod]
        public string AddGroup(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int GroupId, string GroupName, string Description, string GroupCode, bool IsActive)
        {
            GroupMaster mdetail = new GroupMaster();
            try
            {
                int UserID = -1;
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        UserID = (from u in context.Users where u.UserName == UserName select u.UserId).FirstOrDefault();
                        if (GroupId == 0)
                        {
                            GroupMaster g = new GroupMaster();

                            g.GroupName = GroupName;
                            g.GroupCode = GroupCode;
                            g.CreatedBy = UserID;
                            g.Description = Description;
                            g.CompanyID = CompanyId;
                            g.CreatedDate = Convert.ToDateTime(DateTime.Now);
                            g.Status = IsActive;
                            context.GroupMasters.InsertOnSubmit(g);
                            context.SubmitChanges();

                            LD.ReturnResult = "Group " + GroupName + " added sucessfully.";


                            int AdminGrpID = (from u in context.GroupMasters where (u.CompanyID == CompanyId) orderby u.ID ascending select u.ID).FirstOrDefault();

                            string GrpAccess = AddGroupAccessID(UserName, Password, DeviceId, DeviceType, CompanyId, AdminGrpID, Convert.ToString(g.ID));
                        }
                        else
                        {
                            mdetail = (from g in context.GroupMasters where g.ID == GroupId select g).FirstOrDefault();

                            mdetail.GroupName = GroupName;
                            mdetail.GroupCode = GroupCode;
                            mdetail.Status = IsActive;
                            mdetail.CompanyID = CompanyId;
                            mdetail.Description = Description;
                            context.SubmitChanges();

                            LD.ReturnResult = "Group " + GroupName + " update sucessfully.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = ex.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                mdetail = mdetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListGroup(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<GroupDetail> UDDetail = new List<GroupDetail>();
            bool IsAllList = false;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        IsAllList = GF.ShowAllCompanyDetail(context, LD.UserId);

                        UDDetail = (from ud in context.GroupMasters
                                    where ud.CompanyID.Equals(CompanyId)
                                    select new GroupDetail()
                                    {
                                        GroupID = ud.ID,
                                        GroupName = ud.GroupName,
                                        GroupCode = ud.GroupCode,
                                        Description = ud.Description,
                                        CompanyID = Convert.ToInt32(ud.CompanyID),
                                        Status = Convert.ToInt32(ud.Status),
                                        CreatedDate = Convert.ToString(ud.CreatedDate),
                                        CreatedByName = (from u in context.Users where u.UserId == ud.CreatedBy select u.FirstName + " " + u.LastName).FirstOrDefault(),
                                        CompanyName = (from c in context.Companies where c.CompanyId == ud.CompanyID select c.CompanyName).FirstOrDefault(),
                                        UserCount = (from u in context.Users where u.GroupID == ud.ID select u.UserId).Count(),
                                        GrpUsrName = (from u in context.Users where u.GroupID == ud.ID select u.FirstName + "" + u.LastName).ToList()
                                    }).OrderBy(ud => ud.GroupID).ToList();

                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                UDDetail = UDDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                IsAll = IsAllList,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        public string AddWeeklyTimings(string UserName, string Password, string DeviceId, string DeviceType, int UserID, byte TimingFor, int ObjectId, string TimingType, string Day, byte DayType, DateTime TimeFrom, DateTime TimeUpto, TimeSpan WorkingHours, int Status, int CreatedBy, string IPAddress, int WTID)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (WTID == 0)
                        {
                            WeeklyTiming wt = new WeeklyTiming();

                            wt.UserID = UserID;
                            wt.TimingFor = TimingFor;
                            wt.ObjectId = ObjectId;
                            wt.TimingType = TimingType;
                            wt.Day = Day;
                            wt.DayType = DayType;
                            wt.TimeFrom = TimeFrom;
                            wt.TimeUpto = TimeUpto;
                            wt.WorkingHours = WorkingHours;
                            wt.Status = true;
                            wt.Edit = false;
                            wt.CreatedOn = DateTime.Now;
                            wt.CreatedBy = CreatedBy;
                            wt.IPAddress = IPAddress;
                            context.WeeklyTimings.InsertOnSubmit(wt);
                            context.SubmitChanges();

                            LD.ReturnResult = "Weekly Timings are  added sucessfully.";
                        }
                        else
                        {
                            var mdetail = (from wt in context.WeeklyTimings where wt.ID == WTID select wt).FirstOrDefault();

                            mdetail.UserID = UserID;
                            mdetail.TimingFor = TimingFor;
                            mdetail.ObjectId = ObjectId;
                            mdetail.TimingType = TimingType;
                            mdetail.Day = Day;
                            mdetail.DayType = DayType;
                            mdetail.TimeFrom = TimeFrom;
                            mdetail.TimeUpto = TimeUpto;
                            mdetail.WorkingHours = WorkingHours;
                            mdetail.Status = true;
                            mdetail.CreatedOn = DateTime.Now;
                            mdetail.UpdatedBy = CreatedBy;
                            mdetail.Edit = true;
                            mdetail.IPAddress = IPAddress;
                            context.SubmitChanges();

                            LD.ReturnResult = "Weekly Timings are  Updated sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddCompanyBranch(string UserName, string Password, string DeviceId, string DeviceType, int CmpBrID, int CompanyID, string BranchName, string BranchAddress, string BranchCode, byte BranchTypeId, int ParentBranchId, string City, string State, string Country, string Pincode, string Location, int CretedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (CmpBrID == 0)
                        {
                            CompanyBranch CB = new CompanyBranch();

                            CB.CompanyID = CompanyID;
                            CB.BranchName = BranchName;
                            CB.BranchAddress = BranchAddress;
                            CB.Status = true;
                            CB.BranchCode = BranchCode;
                            CB.BranchTypeId = BranchTypeId;
                            CB.ParentBranchId = ParentBranchId;
                            CB.City = City;
                            CB.State = State;
                            CB.Country = Country;
                            CB.Pincode = Pincode;
                            CB.Location = Location;
                            CB.CretedBy = CretedBy;
                            CB.CreatedDate = DateTime.Now;
                            CB.Edit = false;
                            CB.IPAddress = IPAddress;
                            context.CompanyBranches.InsertOnSubmit(CB);
                            context.SubmitChanges();

                            LD.ReturnResult = "Weekly Timings are  added sucessfully.";
                        }
                        else
                        {
                            var cbdetail = (from wt in context.CompanyBranches where wt.ID == CmpBrID select wt).FirstOrDefault();

                            cbdetail.CompanyID = CompanyID;
                            cbdetail.BranchName = BranchName;
                            cbdetail.BranchAddress = BranchAddress;
                            cbdetail.Status = true;
                            cbdetail.BranchCode = BranchCode;
                            cbdetail.BranchTypeId = BranchTypeId;
                            cbdetail.ParentBranchId = ParentBranchId;
                            cbdetail.City = City;
                            cbdetail.State = State;
                            cbdetail.Country = Country;
                            cbdetail.Pincode = Pincode;
                            cbdetail.Location = Location;
                            cbdetail.UpdatedBy = CretedBy;
                            cbdetail.UpdatedDate = DateTime.Now;
                            cbdetail.Edit = true;
                            cbdetail.IPAddress = IPAddress;
                            context.SubmitChanges();

                            LD.ReturnResult = "Weekly Timings are  Updated sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        #region ~Employeereg

        [WebMethod]
        public string AddEmployeeAddress(string UserName, string Password, string DeviceId, string DeviceType, int EAID, int EmployeeID, string Address, string City, string State, string Pincode, int CreatedBy, string ImergencyCntNo, string ImergencyName, string CurrAddress, string CurrCity, string CurrState, string CurrPincode)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (EAID == 0)
                        {
                            EmployeeAddress EA = new EmployeeAddress();

                            EA.UserID = EmployeeID;
                            EA.Address = Address;
                            // EA.AddressType = AddressType;
                            EA.City = City;
                            EA.State = State;
                            EA.Pincode = Pincode;
                            EA.CreatedBy = CreatedBy;
                            EA.ImergencyName = ImergencyName;
                            EA.ImergencyCntNo = ImergencyCntNo;
                            EA.CurrAddress = CurrAddress;
                            EA.CurrCity = CurrCity;
                            EA.CurrState = CurrState;
                            EA.CurrPincode = CurrPincode;


                            context.EmployeeAddresses.InsertOnSubmit(EA);
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Address is added sucessfully.";
                        }
                        else
                        {
                            var EAdetail = (from wt in context.EmployeeAddresses where wt.ID == EAID select wt).FirstOrDefault();

                            EAdetail.UserID = EmployeeID;
                            EAdetail.Address = Address;
                            //EAdetail.AddressType = AddressType;
                            EAdetail.City = City;
                            EAdetail.State = State;
                            EAdetail.Pincode = Pincode;
                            EAdetail.UpdatedBy = CreatedBy;
                            EAdetail.ImergencyName = ImergencyName;
                            EAdetail.ImergencyCntNo = ImergencyCntNo;
                            EAdetail.CurrAddress = CurrAddress;
                            EAdetail.CurrCity = CurrCity;
                            EAdetail.CurrState = CurrState;
                            EAdetail.CurrPincode = CurrPincode;
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Address is  Updated sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddEmployeeBankDetails(string UserName, string Password, string DeviceId, string DeviceType, int AccountId, int UserID, string BankAccountNo, string BankName, string BranchCode, string IFSCode, byte Status, int CreatedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (AccountId == 0)
                        {
                            EmployeeBankDetail EB = new EmployeeBankDetail();


                            EB.UserID = UserID;
                            EB.BankAccountNo = BankAccountNo;
                            EB.BankName = BankName;
                            EB.BranchCode = BranchCode;
                            EB.IFSCode = IFSCode;
                            EB.Status = 1;
                            EB.Edit = false;
                            EB.CreatedOn = DateTime.Now;
                            EB.CreatedBy = CreatedBy;
                            EB.IPAddress = IPAddress;
                            context.EmployeeBankDetails.InsertOnSubmit(EB);
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Address is added sucessfully.";
                        }
                        else
                        {
                            var EBdetail = (from wt in context.EmployeeBankDetails where wt.AccountId == AccountId select wt).FirstOrDefault();

                            EBdetail.UserID = UserID;
                            EBdetail.BankAccountNo = BankAccountNo;
                            EBdetail.BankName = BankName;
                            EBdetail.BranchCode = BranchCode;
                            EBdetail.IFSCode = IFSCode;
                            EBdetail.Status = 1;
                            EBdetail.Edit = true;
                            EBdetail.UpdatedBy = CreatedBy;
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Address is  Updated sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddEmployeeEducationDetails(string UserName, string Password, string DeviceId, string DeviceType, int EduId, int UserID, string Degree, string Institute, string University, int YearOfPassing, byte GradeType, string Grade, byte CourseType, int CreatedBy, string IpAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (EduId == 0)
                        {
                            EmployeeEducationDetail Ed = new EmployeeEducationDetail();


                            Ed.UserID = UserID;
                            Ed.Degree = Degree;
                            Ed.Institute = Institute;
                            Ed.University = University;
                            Ed.YearOfPassing = YearOfPassing;
                            Ed.GradeType = GradeType;
                            Ed.Grade = Grade;
                            Ed.CourseType = CourseType;
                            Ed.Status = true;
                            Ed.Edit = false;
                            Ed.CreatedOn = DateTime.Now;
                            Ed.CreatedBy = CreatedBy;
                            Ed.IpAddress = IpAddress;

                            context.EmployeeEducationDetails.InsertOnSubmit(Ed);
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Education is added sucessfully.";
                        }
                        else
                        {
                            var Eddetail = (from wt in context.EmployeeEducationDetails where wt.EduId == EduId select wt).FirstOrDefault();

                            Eddetail.UserID = UserID;
                            Eddetail.Degree = Degree;
                            Eddetail.Institute = Institute;
                            Eddetail.University = University;
                            Eddetail.YearOfPassing = YearOfPassing;
                            Eddetail.GradeType = GradeType;
                            Eddetail.Grade = Grade;
                            Eddetail.CourseType = CourseType;
                            Eddetail.Status = true;
                            Eddetail.Edit = false;
                            Eddetail.UpdatedOn = DateTime.Now;
                            Eddetail.UpdatedBy = CreatedBy;
                            Eddetail.IpAddress = IpAddress;
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Education is  Updated sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddEmployeeFamily(string UserName, string Password, string DeviceId, string DeviceType, int EFID, int UserID, string Name, DateTime DateOfBirth, string Relation, int CreatedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (EFID == 0)
                        {
                            EmployeeFamily EF = new EmployeeFamily();


                            EF.UserID = UserID;
                            EF.Name = Name;
                            EF.DateOfBirth = DateOfBirth;
                            EF.Relation = Relation;
                            EF.Status = 1;
                            EF.Edit = false;
                            EF.CreatedOn = DateTime.Now;
                            EF.CreatedBy = CreatedBy;
                            EF.IPAddress = IPAddress;
                            context.EmployeeFamilies.InsertOnSubmit(EF);
                            context.SubmitChanges();

                            LD.ReturnResult = "Family Member is added sucessfully.";
                        }
                        else
                        {
                            var EFdetail = (from wt in context.EmployeeFamilies where wt.ID == EFID select wt).FirstOrDefault();

                            EFdetail.UserID = UserID;
                            EFdetail.Name = Name;
                            EFdetail.DateOfBirth = DateOfBirth;
                            EFdetail.Relation = Relation;
                            EFdetail.Status = 1;
                            EFdetail.Edit = true;
                            EFdetail.UpdatedOn = DateTime.Now;
                            EFdetail.UpdatedBy = CreatedBy;
                            EFdetail.IPAddress = IPAddress;
                            context.SubmitChanges();

                            LD.ReturnResult = "Family Member is  Updated sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddEmployeePersonalIDs(string UserName, string Password, string DeviceId, string DeviceType, int EFID, int UserID, int IDType, string IDNumber, string imageUrl, int CreatedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (EFID == 0)
                        {
                            EmployeePersonalID EP = new EmployeePersonalID();


                            EP.UserID = UserID;
                            EP.IDType = IDType;
                            EP.IDNumber = IDNumber;
                            EP.imageUrl = imageUrl;
                            EP.Status = true;
                            EP.Edit = false;
                            EP.CreatedOn = DateTime.Now;
                            EP.CreatedBy = CreatedBy;
                            EP.IPAddress = IPAddress;
                            context.EmployeePersonalIDs.InsertOnSubmit(EP);
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Document is added sucessfully.";
                        }
                        else
                        {
                            var EIDdetail = (from wt in context.EmployeePersonalIDs where wt.ID == EFID select wt).FirstOrDefault();

                            EIDdetail.UserID = UserID;
                            EIDdetail.IDType = IDType;
                            EIDdetail.IDNumber = IDNumber;
                            EIDdetail.imageUrl = imageUrl;
                            EIDdetail.Status = true;
                            EIDdetail.Edit = true;
                            EIDdetail.UpdatedOn = DateTime.Now;
                            EIDdetail.UpdatedBy = CreatedBy;
                            EIDdetail.IPAddress = IPAddress;
                            context.SubmitChanges();

                            LD.ReturnResult = "Employee Document is  Updated sucessfully.";
                        }
                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddEmployeePersonaldetails(string UserName, string Password, string DeviceId, string DeviceType, int UserId, int CreatedBy, string IPAddress, UserPersonaInfo Usrpersonal)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {








                        var EIDdetail = (from wt in context.Users where wt.UserId == UserId select wt).FirstOrDefault();
                        EIDdetail.FirstName = Usrpersonal.FirstName;
                        EIDdetail.LastName = Usrpersonal.LastName;
                        EIDdetail.GenderTypeId = Usrpersonal.GenderTypeId;
                        EIDdetail.DateOfBirth = DateTime.Parse(Usrpersonal.DateOfBirth);
                        EIDdetail.ProfilePic = Usrpersonal.ProfilePic;
                        EIDdetail.MaritalStatus = Usrpersonal.MaritalStatus;
                        EIDdetail.PhoneNoPer = Usrpersonal.PhoneNoPer;
                        EIDdetail.EmailIDPer = Usrpersonal.EmailIDPer;

                        if (Usrpersonal.UserImg != "")
                        {
                            #region iMAGEuPLOAD
                            try
                            {
                                var filepath = Server.MapPath("Images\\UserProfile");

                                if (!Directory.Exists(filepath))
                                {
                                    Directory.CreateDirectory(filepath);
                                }

                                if (File.Exists(filepath + "\\" + EIDdetail.ProfilePic))
                                {
                                    File.Delete(filepath + "\\" + EIDdetail.ProfilePic);
                                }

                                File.WriteAllBytes((filepath + "\\" + EIDdetail.UserCode + "." + Usrpersonal.Imgext), Convert.FromBase64String(Usrpersonal.UserImg));

                            }
                            catch (Exception)
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "Server Is Not Responding...";
                                LD.FailureReason = "";
                            }
                            #endregion
                        }
                        EIDdetail.ProfilePic = EIDdetail.UserCode + "." + Usrpersonal.Imgext;
                        context.SubmitChanges();

                        LD.ReturnResult = "User details are Updated sucessfully.";

                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        #endregion

        [WebMethod]
        public string ReportAllUsersMonthwiseExcel(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedCompanyId, string SelectedMonth)
        {
            CSVDetail PDetail = new CSVDetail();
            ExcelPackage ep = new ExcelPackage();
            List<CSVDetail> CommonReportList = new List<CSVDetail>();
            string rootDocsPath = "";
            try
            {
                var ConvertedSelectedMonth = Convert.ToDateTime(SelectedMonth);
                // check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);
                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var GetDBDetail = (from p in context.PunchIns
                                           join u in context.Users on p.UserId equals u.UserId
                                           join c in context.Companies on u.CompanyID equals c.CompanyId
                                           where p.PunchinTime.Month == ConvertedSelectedMonth.Month
                                           && p.PunchinTime.Year == ConvertedSelectedMonth.Year && p.PunchoutTime != null
                                            && c.CompanyId == SelectedCompanyId
                                           select new
                                           {
                                               UserId = p.UserId,
                                               PunchInTime = p.PunchinTime,
                                               LatePunchIn = p.LatePunchin,
                                               LatePunchInReason = p.LatePunchinReason,
                                               PunchInType = p.PunchinType,
                                               PunchinLocation = (p.PunchinType == true ? (from l in context.Locations where l.LocationId == p.PILocationId select l.PlaceName).FirstOrDefault() : p.PILatitudeLongitude),
                                               PunchInOutsideLocationReason = p.PunchinOutsideLocationReason,
                                               PunchOutTime = p.PunchoutTime,
                                               EarlyPunchOut = p.EarlyPunchout,
                                               EarlyPunchoutReason = p.EarlyPunchoutReason,
                                               PunchOutType = p.PunchoutType,
                                               PunchoutLocation = (p.PunchoutType == true ? (from l in context.Locations where l.LocationId == p.POLocationId select l.PlaceName).FirstOrDefault() : p.POLatitudeLongitude),
                                               PunchOutOutsideLocationReason = p.PunchoutOutsideLocationReason,
                                               SystemPunchOut = p.SystemPunchout,
                                               PunchinDeviceId = p.PunchinDeviceId,
                                               PunchoutDeviceId = p.PunchoutDeviceId,
                                               WorkHourReason = p.WorkHourReason,
                                               OfficeHour = p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay,
                                               TotalBreak = context.Breaks.Where(w => w.UserId == u.UserId && w.BreakinTime.Date == p.PunchinTime.Date).Count(),
                                               TotalBreakTimeTicks = context.Breaks.Where(w => w.UserId == u.UserId && w.BreakinTime.Date == p.PunchinTime.Date).Count() != 0 ? (context.Breaks.Where(w => w.UserId == u.UserId && w.BreakinTime.Date == p.PunchinTime.Date).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks))) : 0,
                                               PunchinOnHoliday = p.PunchinOnHoliday,
                                               PunchinOnWeekoff = p.PunchinOnWeekoff,
                                               Halfday = p.IsHalfDay,
                                           }).ToList();

                        var UserList = (from u in context.Users
                                        where u.Status == 1 && u.CompanyID == SelectedCompanyId
                                        select new
                                        {
                                            u.UserName,
                                            u.UserId,
                                            UserWeekOffDayList = context.UserWeekOffDays.Where(w => w.UserId == u.UserId).GroupBy(o => o.WeekOffDay).AsEnumerable().ToList(),
                                            TotalLeavelCountUserWise = (from il in context.Leaves
                                                                        join ila in context.UserLeaveApprovals on il.LeaveId equals ila.LeaveId into finalResult
                                                                        from fr in finalResult.DefaultIfEmpty()
                                                                        where il.UserId == u.UserId && (ConvertedSelectedMonth.Month == DateTime.Today.Month ? (il.LeaveFromDate.Month == DateTime.Today.Month && il.LeaveFromDate < DateTime.Today) || il.LeaveToDate.Month == DateTime.Today.Month : il.LeaveFromDate.Month == DateTime.Today.Month || il.LeaveToDate.Month == DateTime.Today.Month) &&
                                                                        (context.Leaves.Where(w => w.UserId.Equals(u.UserId)).Select(s => s.LeaveId).Contains(il.LeaveId) &&
                                                                          ((u.LeaveApprovalTypeId.Equals(3)) || (u.LeaveApprovalTypeId.Equals(1) && fr.LeaveId == il.LeaveId && fr.IsApprovedFirst == true) || (u.LeaveApprovalTypeId.Equals(2) && fr.LeaveId == il.LeaveId && fr.IsApprovedFirst == true && fr.IsApprovedSecond == true)))

                                                                        select new
                                                                        {
                                                                            tc = ConvertedSelectedMonth.Month == DateTime.Today.Month ?
                                                                                il.LeaveFromDate < ConvertedSelectedMonth ?
                                                                                    il.LeaveToDate < DateTime.Today.AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - ConvertedSelectedMonth.Date).Days + 1 : (DateTime.Today.AddDays(-1).Date - ConvertedSelectedMonth.Date).Days + 1 :
                                                                                    il.LeaveToDate < DateTime.Today.AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - il.LeaveFromDate.Date).Days + 1 : (DateTime.Today.AddDays(-1).Date - il.LeaveFromDate.Date).Days + 1 :
                                                                                il.LeaveFromDate < ConvertedSelectedMonth ?
                                                                                    il.LeaveToDate < ConvertedSelectedMonth.AddMonths(1).AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - ConvertedSelectedMonth.Date).Days + 1 : (ConvertedSelectedMonth.AddMonths(1).AddDays(-1).Date - ConvertedSelectedMonth.Date).Days + 1 :
                                                                                    il.LeaveToDate < ConvertedSelectedMonth.AddMonths(1).AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - il.LeaveFromDate.Date).Days + 1 : (ConvertedSelectedMonth.AddMonths(1).AddDays(-1).Date - il.LeaveFromDate.Date).Days + 1
                                                                        }).ToList(),
                                        }).ToList();

                        var TotalHoliday = 0;

                        if (ConvertedSelectedMonth.Month == DateTime.Today.Month)
                        {
                            TotalHoliday = (from u in context.Holidays
                                            where u.HolidayDate.Month == DateTime.Today.Month && u.HolidayDate.Date < DateTime.Today.Date
                                            select u).Count();
                        }
                        else
                        {
                            TotalHoliday = (from u in context.Holidays
                                            where u.HolidayDate.Month == ConvertedSelectedMonth.Month
                                            select u).Count();
                        }

                        var Sheet = 2;
                        ExcelWorksheet Commonws = CreateSheet(ep, "AllUserCommonReport", 1);

                        foreach (var ul in UserList)
                        {
                            var Finish = ConvertedSelectedMonth.Month == DateTime.Today.Month ? DateTime.Today.AddDays(-1) : ConvertedSelectedMonth.AddMonths(1).AddDays(-1);
                            var TotalWeekoffDay = GF.GetTotalWeekoffDay(ul.UserWeekOffDayList, ConvertedSelectedMonth, Finish);

                            PDetail.DetailList = (from p in GetDBDetail
                                                  where p.UserId == ul.UserId
                                                  select new PunchinDetailList()
                                                  {
                                                      Date = p.PunchInTime.ToString("dd-MMM-yyyy"),//string.Format("{0:dd-MM-yy}", p.PunchInTime),
                                                      PunchinTime = p.PunchInTime.ToString("hh:mm:ss tt"),
                                                      LatePunchin = p.LatePunchIn,
                                                      LatePunchinReason = p.LatePunchInReason,
                                                      IsLocationTypePunchin = p.PunchInType.Value == true ? "Inside Location" : "Outside Location",
                                                      PunchinLocation = p.PunchinLocation,
                                                      PunchinOutsideLocationReason = p.PunchInOutsideLocationReason,
                                                      PunchOutTime = p.PunchOutTime.Value.ToString("hh:mm:ss tt"),
                                                      EarlyPunchOut = p.EarlyPunchOut,
                                                      EarlyPunchoutReason = p.EarlyPunchoutReason,
                                                      IsLocationTypePunchout = p.PunchOutType == true ? "Inside Location" : "Outside Location",
                                                      PunchoutLocation = p.PunchoutLocation,
                                                      PunchoutOutsideLocationReason = p.PunchOutOutsideLocationReason,
                                                      IsSystemPunchout = p.SystemPunchOut,
                                                      PunchinDeviceId = p.PunchinDeviceId,
                                                      PunchoutDeviceId = p.PunchoutDeviceId,
                                                      WorkHourReason = p.WorkHourReason,
                                                      OfficeHour = p.OfficeHour.Hours + ":" + p.OfficeHour.Minutes + ":" + p.OfficeHour.Seconds,
                                                      TotalBreak = p.TotalBreak,
                                                      TotalBreakTime = p.TotalBreak > 0 ? new TimeSpan(p.TotalBreakTimeTicks).Hours + ":" + new TimeSpan(p.TotalBreakTimeTicks).Minutes + ":" + new TimeSpan(p.TotalBreakTimeTicks).Seconds : "",
                                                      WorkHour = p.TotalBreak > 0 ? new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks).Hours + ":" + new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks).Minutes + ":" + new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks).Seconds : p.OfficeHour.Hours + ":" + p.OfficeHour.Minutes + ":" + p.OfficeHour.Seconds,
                                                      WorkHourInTimeSpan = p.TotalBreak > 0 ? new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks) : p.OfficeHour,
                                                      PunchinOnHoliday = p.PunchinOnHoliday,
                                                      PunchinOnWeekoff = p.PunchinOnWeekoff,
                                                      Halfday = p.Halfday,
                                                  }
                                       ).OrderBy(x => x.Date).ToList();

                            PDetail.UserName = ul.UserName;
                            PDetail.MonthName = SelectedMonth;

                            var tmp = (from p in GetDBDetail
                                       where p.UserId == ul.UserId
                                       group p by p.UserId into g
                                       select g).AsEnumerable()
                                     .Select(g => new
                                     {
                                         TotalPreserntDay = g.Count(c => c.PunchinOnHoliday == false && c.PunchinOnWeekoff == false),
                                         TotalWorkingHour = TimeSpan.FromTicks(g.Where(w => w.Halfday == false).Sum(a => a.OfficeHour.Ticks - a.TotalBreakTimeTicks)),
                                         AverageWorkingHour = g.Count(c => c.Halfday == false) > 0 ? TimeSpan.FromTicks(g.Where(w => w.Halfday == false).Aggregate(new TimeSpan(), (sum, nextData) => sum.Add(nextData.OfficeHour - TimeSpan.FromTicks(nextData.TotalBreakTimeTicks))).Ticks / g.Count(c => c.Halfday == false)) : TimeSpan.FromTicks(0),
                                         TotalLatePunchin = g.Where(x => x.LatePunchIn == true).Count(),
                                         TotalEarlyPunchout = g.Where(x => x.EarlyPunchOut == true).Count(),
                                         TotalPunchinOutside = g.Where(x => x.PunchInType == false).Count(),
                                         TotalPunchoutOutside = g.Where(x => x.PunchOutType == false).Count(),
                                         TotalSystemPunchout = g.Where(x => x.SystemPunchOut == true).Count(),
                                         UncomletedWorkhour = g.Where(x => x.WorkHourReason != null).Count(),
                                         TotalHalfday = g.Count(c => c.Halfday == true),
                                         PunchinOnHoliday = g.Count(c => c.PunchinOnHoliday),
                                         PunchinOnWeekoff = g.Count(c => c.PunchinOnWeekoff),
                                     }).FirstOrDefault();

                            if (tmp != null)
                            {
                                PDetail.DayInMonth = ConvertedSelectedMonth.Month == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.AddDays(-1).Day) : DateTime.DaysInMonth(ConvertedSelectedMonth.Year, ConvertedSelectedMonth.Month);
                                PDetail.Holiday = TotalHoliday;
                                PDetail.WeekoffDay = TotalWeekoffDay;
                                PDetail.LeaveDay = ul.TotalLeavelCountUserWise.Count() > 0 ? ul.TotalLeavelCountUserWise.Sum(t => t.tc) - (TotalHoliday + TotalWeekoffDay) : 0;
                                PDetail.WorkingDay = PDetail.DayInMonth - (PDetail.Holiday + PDetail.WeekoffDay);
                                PDetail.TotalPresentDay = tmp.TotalPreserntDay;
                                PDetail.PunchinOnHoliday = tmp.PunchinOnHoliday;
                                PDetail.PunchinOnWeekoff = tmp.PunchinOnWeekoff;
                                PDetail.TotalAbsentDay = PDetail.WorkingDay - PDetail.TotalPresentDay;
                                PDetail.TotalWorkingDay = PDetail.WorkingDay + PDetail.PunchinOnWeekoff + PDetail.PunchinOnHoliday - PDetail.TotalAbsentDay;
                                PDetail.HalfDay = tmp.TotalHalfday;
                                PDetail.TotalWorkingHour = ((tmp.TotalWorkingHour.Days * 24) + tmp.TotalWorkingHour.Hours) + ":" + tmp.TotalWorkingHour.Minutes + ":" + tmp.TotalWorkingHour.Seconds;
                                PDetail.AverageWorkingHour = tmp.AverageWorkingHour.Hours + ":" + tmp.AverageWorkingHour.Minutes + ":" + tmp.AverageWorkingHour.Seconds;
                                PDetail.TotalLatePunchin = tmp.TotalLatePunchin;
                                PDetail.TotalEarlyPunchout = tmp.TotalEarlyPunchout;
                                PDetail.TotalPunchinOutside = tmp.TotalPunchinOutside;
                                PDetail.TotalPunchoutOutside = tmp.TotalPunchoutOutside;
                                PDetail.TotalSystemPunchout = tmp.TotalSystemPunchout;
                                PDetail.UncomletedWorkhour = tmp.UncomletedWorkhour;

                                CommonReportList.Add(new CSVDetail
                                {
                                    UserName = ul.UserName,
                                    TotalWorkingDay = PDetail.TotalWorkingDay,
                                    TotalPresentDay = PDetail.TotalPresentDay + PDetail.PunchinOnHoliday + PDetail.PunchinOnWeekoff,
                                    TotalAbsentDay = PDetail.TotalAbsentDay,
                                    TotalWeekoff = PDetail.WeekoffDay,
                                    HalfDay = PDetail.HalfDay,
                                    TotalWorkingHour = PDetail.TotalWorkingHour,
                                    AverageWorkingHour = PDetail.AverageWorkingHour,
                                    TotalLatePunchin = PDetail.TotalLatePunchin,
                                    TotalEarlyPunchout = PDetail.TotalEarlyPunchout,
                                    TotalPunchinOutside = PDetail.TotalPunchinOutside,
                                    TotalPunchoutOutside = PDetail.TotalPunchoutOutside,
                                    TotalSystemPunchout = PDetail.TotalSystemPunchout,
                                    UncomletedWorkhour = PDetail.UncomletedWorkhour
                                });
                            }
                            else
                            {
                                CommonReportList.Add(new CSVDetail
                                {
                                    UserName = ul.UserName,
                                    TotalWorkingDay = ConvertedSelectedMonth.Month == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.Day) - (TotalHoliday + TotalWeekoffDay + 1) : System.DateTime.DaysInMonth(ConvertedSelectedMonth.Year, ConvertedSelectedMonth.Month) - (TotalHoliday + TotalWeekoffDay),
                                    TotalPresentDay = 0,
                                    TotalAbsentDay = ConvertedSelectedMonth.Month == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.Day) - (TotalHoliday + TotalWeekoffDay + 1) : System.DateTime.DaysInMonth(ConvertedSelectedMonth.Year, ConvertedSelectedMonth.Month) - (TotalHoliday + TotalWeekoffDay),
                                    HalfDay = 0,
                                    TotalWorkingHour = "0",
                                    AverageWorkingHour = "0",
                                    TotalLatePunchin = 0,
                                    TotalEarlyPunchout = 0,
                                    TotalPunchinOutside = 0,
                                    TotalPunchoutOutside = 0,
                                    TotalSystemPunchout = 0,
                                    UncomletedWorkhour = 0
                                });
                            }

                            //Create a sheet
                            ExcelWorksheet ws = CreateSheet(ep, ul.UserName, Sheet);
                            Sheet++;

                            ws.Cells[1, 1].Value = "User";
                            ws.Cells[1, 1].Style.Font.Size = 15;

                            ws.Cells[1, 2].Value = PDetail.UserName;
                            ws.Cells[1, 2].Style.Font.Bold = true;
                            ws.Cells[1, 2].Style.Font.Size = 15;

                            ws.Cells[1, 4].Value = "Month";
                            ws.Cells[1, 4].Style.Font.Size = 15;

                            ws.Cells[1, 5].Value = PDetail.MonthName;
                            ws.Cells[1, 5].Style.Font.Bold = true;
                            ws.Cells[1, 5].Style.Font.Size = 15;

                            var collist = new string[]
                            { "Date","Work Hour","Punchin Time","Punchout Time","Late Punchin","Late Punchin Reason","Punchin Type","Punchin Location","Punchin Outside Location Reason",
                                "Early Punchout","Early Punchout Reason","Punchout Type","Punchout Location","Punchout Outside Location Reason","System Punchout",
                                "Punchin Device","Punchout Device","Office Hour","Break Hour","Total Break","Work Hour Reason","Holiday","Weekoff","Halfday"
                            };

                            var templength = collist.Length;
                            for (var hi = 1; hi <= templength; hi++)
                            {
                                var cell = ws.Cells[3, hi];
                                var fill = cell.Style.Fill;
                                fill.PatternType = ExcelFillStyle.Solid;
                                //Setting the background color of header cells to Gray
                                fill.BackgroundColor.SetColor(Color.Gray);

                                //Setting Top/left,right/bottom borders.
                                var border = cell.Style.Border;
                                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                                //Setting Value in cell
                                cell.Value = collist[hi - 1];
                            }

                            var k = 3;
                            foreach (var pl in PDetail.DetailList)
                            {
                                var l = 1;
                                k++;
                                ws.Cells[k, l].Value = pl.Date;

                                ws.Cells[k, ++l].Value = pl.WorkHour;
                                if (pl.WorkHourInTimeSpan < TimeSpan.Parse("08:30"))
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }
                                ws.Cells[k, ++l].Value = pl.PunchinTime;
                                ws.Cells[k, ++l].Value = pl.PunchOutTime;
                                ws.Cells[k, ++l].Value = pl.LatePunchin;
                                if (pl.LatePunchin == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.LatePunchinReason;
                                ws.Cells[k, ++l].Value = pl.IsLocationTypePunchin;
                                if (pl.IsLocationTypePunchin == "Outside Location")
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.PunchinLocation;
                                ws.Cells[k, ++l].Value = pl.PunchinOutsideLocationReason;

                                ws.Cells[k, ++l].Value = pl.EarlyPunchOut;
                                if (pl.EarlyPunchOut == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }
                                ws.Cells[k, ++l].Value = pl.EarlyPunchoutReason;
                                ws.Cells[k, ++l].Value = pl.IsLocationTypePunchout;
                                if (pl.IsLocationTypePunchout == "Outside Location")
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.PunchoutLocation;
                                ws.Cells[k, ++l].Value = pl.PunchoutOutsideLocationReason;
                                ws.Cells[k, ++l].Value = pl.IsSystemPunchout;
                                if (pl.IsSystemPunchout == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.PunchinDeviceId;
                                ws.Cells[k, ++l].Value = pl.PunchoutDeviceId;

                                ws.Cells[k, ++l].Value = pl.OfficeHour;
                                ws.Cells[k, ++l].Value = pl.TotalBreakTime;

                                ws.Cells[k, ++l].Value = pl.TotalBreak;
                                ws.Cells[k, ++l].Value = pl.WorkHourReason;
                                ws.Cells[k, ++l].Value = pl.PunchinOnHoliday;
                                if (pl.PunchinOnHoliday == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.LightSeaGreen);
                                }
                                ws.Cells[k, ++l].Value = pl.PunchinOnWeekoff;
                                if (pl.PunchinOnWeekoff == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.LightSeaGreen);
                                }
                                ws.Cells[k, ++l].Value = pl.Halfday;
                                if (pl.Halfday == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.CornflowerBlue);
                                }
                            }

                            k = k + 3;
                            if (tmp != null)
                            {
                                var SummuryStartRow = k;
                                ws.Cells[k, 1].Value = "Days in month";
                                ws.Cells[k, 2].Value = PDetail.DayInMonth;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "  (-)Holiday";
                                ws.Cells[k, 2].Value = PDetail.Holiday;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "  (-)Weekoff";
                                ws.Cells[k, 2].Value = PDetail.WeekoffDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = " Working days";
                                ws.Cells[k, 2].Value = PDetail.WorkingDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "  (-)Absent days";
                                ws.Cells[k, 2].Value = PDetail.TotalAbsentDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "Present on working days";
                                ws.Cells[k, 2].Value = PDetail.TotalPresentDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;
                                ws.Cells[k, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;

                                if (PDetail.PunchinOnHoliday > 0)
                                {
                                    ws.Cells[++k, 1].Value = "  (+)Present on holiday";
                                    ws.Cells[k, 2].Value = PDetail.PunchinOnHoliday;
                                    ws.Cells[k, 2].Style.Font.Bold = true;
                                }

                                if (PDetail.PunchinOnWeekoff > 0)
                                {
                                    ws.Cells[++k, 1].Value = "  (+)Present on weekoff";
                                    ws.Cells[k, 2].Value = PDetail.PunchinOnWeekoff;
                                    ws.Cells[k, 2].Style.Font.Bold = true;
                                }



                                ws.Cells[++k, 1].Value = "Total working days";
                                ws.Cells[k, 2].Value = PDetail.TotalWorkingDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "";

                                ws.Cells[++k, 1].Value = "Total Halfday";
                                ws.Cells[k, 2].Value = PDetail.HalfDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                k = SummuryStartRow;

                                ws.Cells[k, 4].Value = "Total working hours \n\r (Excluding Halfday)";
                                ws.Cells[k, 5].Value = PDetail.TotalWorkingHour;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                                ws.Cells[++k, 4].Value = "Average working hours \n\r (Excluding Halfday)";
                                ws.Cells[k, 5].Value = PDetail.AverageWorkingHour;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total punchin late";
                                ws.Cells[k, 5].Value = PDetail.TotalLatePunchin;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                                ws.Cells[++k, 4].Value = "Total early punchout";
                                ws.Cells[k, 5].Value = PDetail.TotalEarlyPunchout;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total punchin outside";
                                ws.Cells[k, 5].Value = PDetail.TotalPunchinOutside;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                                ws.Cells[++k, 4].Value = "Total punchout outside";
                                ws.Cells[k, 5].Value = PDetail.TotalPunchoutOutside;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total system punchout";
                                ws.Cells[k, 5].Value = PDetail.TotalSystemPunchout;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total uncompleted workhour day";
                                ws.Cells[k, 5].Value = PDetail.UncomletedWorkhour;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                            }

                            ws.Cells["A1:U50"].AutoFitColumns();
                        }

                        // Start-- Common Report On First sheet
                        var i = 1;
                        Commonws.Cells[i, 1].Value = "Month";
                        Commonws.Cells[i, 1].Style.Font.Size = 15;

                        Commonws.Cells[i, 2].Value = PDetail.MonthName;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;
                        Commonws.Cells[i, 2].Style.Font.Size = 15;

                        Commonws.Cells[++i, 1].Value = "Days in month";
                        Commonws.Cells[i, 2].Value = PDetail.DayInMonth;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;

                        Commonws.Cells[++i, 1].Value = "  (-)Holiday";
                        Commonws.Cells[i, 2].Value = PDetail.Holiday;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;

                        Commonws.Cells[++i, 1].Value = " Working days (Excl. weekoff)";
                        Commonws.Cells[i, 2].Value = PDetail.DayInMonth - PDetail.Holiday;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;

                        i += 2;
                        var CommonCollist = new string[]
                        { "Username","Total Working Days (Workingday - Weekoff - Absent)","Total Absent Days","Total Weekoff","Total halfday",
                            "Total working hours(Excl. Halfday)","Average working hour(Excl. Halfday)",
                            "Total punchin late","Total early punchout","Total punchin outside",
                                "Total punchout outside","Total system punchout","Total uncompleted workhour day"
                        };

                        var CommonListlength = CommonCollist.Length;
                        for (var hi = 1; hi <= CommonListlength; hi++)
                        {
                            var cell = Commonws.Cells[i, hi];
                            var fill = cell.Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            //Setting the background color of header cells to Gray
                            fill.BackgroundColor.SetColor(Color.Gray);

                            //Setting Top/left,right/bottom borders.
                            var border = cell.Style.Border;
                            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                            //Setting Value in cell
                            cell.Value = CommonCollist[hi - 1];
                        }

                        i++;

                        foreach (var pl in CommonReportList)
                        {
                            var j = 1;

                            Commonws.Cells[i, j].Value = pl.UserName;
                            Commonws.Cells[i, ++j].Value = pl.TotalWorkingDay;
                            Commonws.Cells[i, ++j].Value = pl.TotalAbsentDay;
                            Commonws.Cells[i, ++j].Value = pl.TotalWeekoff;
                            Commonws.Cells[i, ++j].Value = pl.HalfDay;
                            Commonws.Cells[i, ++j].Value = pl.TotalWorkingHour;
                            Commonws.Cells[i, ++j].Value = pl.AverageWorkingHour;
                            Commonws.Cells[i, ++j].Value = pl.TotalLatePunchin;
                            Commonws.Cells[i, ++j].Value = pl.TotalEarlyPunchout;
                            Commonws.Cells[i, ++j].Value = pl.TotalPunchinOutside;
                            Commonws.Cells[i, ++j].Value = pl.TotalPunchoutOutside;
                            Commonws.Cells[i, ++j].Value = pl.TotalSystemPunchout;
                            Commonws.Cells[i, ++j].Value = pl.UncomletedWorkhour;

                            i++;
                        }

                        Commonws.Cells["A1:U50"].AutoFitColumns();

                        // END-- Common Report On First sheet

                        var bin = ep.GetAsByteArray();
                        var filepath = Server.MapPath("ExcelDownload");

                        if (!Directory.Exists(filepath))
                        {
                            Directory.CreateDirectory(filepath);
                        }

                        var temp = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

                        File.WriteAllBytes((filepath + "\\" + temp), bin);
                        rootDocsPath = "ExcelDownload/" + temp;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                ExcelFilePath = rootDocsPath,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string CallReportAllUsersMonthwiseExcel(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedCompanyId, string SelectedMonth)
        {
            CSVDetail PDetail = new CSVDetail();
            ExcelPackage ep = new ExcelPackage();
            List<CSVDetail> CommonReportList = new List<CSVDetail>();
            string rootDocsPath = "";
            try
            {
                var ConvertedSelectedMonth = Convert.ToDateTime(SelectedMonth);
                // check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);
                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var GetDBDetail = (from p in context.PunchIns
                                           join u in context.Users on p.UserId equals u.UserId
                                           join c in context.Companies on u.CompanyID equals c.CompanyId
                                           where p.PunchinTime.Month == ConvertedSelectedMonth.Month
                                           && p.PunchinTime.Year == ConvertedSelectedMonth.Year && p.PunchoutTime != null
                                            && c.CompanyId == SelectedCompanyId
                                           select new
                                           {
                                               UserId = p.UserId,
                                               PunchInTime = p.PunchinTime,
                                               LatePunchIn = p.LatePunchin,
                                               LatePunchInReason = p.LatePunchinReason,
                                               PunchInType = p.PunchinType,
                                               PunchinLocation = (p.PunchinType == true ? (from l in context.Locations where l.LocationId == p.PILocationId select l.PlaceName).FirstOrDefault() : p.PILatitudeLongitude),
                                               PunchInOutsideLocationReason = p.PunchinOutsideLocationReason,
                                               PunchOutTime = p.PunchoutTime,
                                               EarlyPunchOut = p.EarlyPunchout,
                                               EarlyPunchoutReason = p.EarlyPunchoutReason,
                                               PunchOutType = p.PunchoutType,
                                               PunchoutLocation = (p.PunchoutType == true ? (from l in context.Locations where l.LocationId == p.POLocationId select l.PlaceName).FirstOrDefault() : p.POLatitudeLongitude),
                                               PunchOutOutsideLocationReason = p.PunchoutOutsideLocationReason,
                                               SystemPunchOut = p.SystemPunchout,
                                               PunchinDeviceId = p.PunchinDeviceId,
                                               PunchoutDeviceId = p.PunchoutDeviceId,
                                               WorkHourReason = p.WorkHourReason,
                                               OfficeHour = p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay,
                                               TotalBreak = context.Breaks.Where(w => w.UserId == u.UserId && w.BreakinTime.Date == p.PunchinTime.Date).Count(),
                                               TotalBreakTimeTicks = context.Breaks.Where(w => w.UserId == u.UserId && w.BreakinTime.Date == p.PunchinTime.Date).Count() != 0 ? (context.Breaks.Where(w => w.UserId == u.UserId && w.BreakinTime.Date == p.PunchinTime.Date).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks))) : 0,
                                               PunchinOnHoliday = p.PunchinOnHoliday,
                                               PunchinOnWeekoff = p.PunchinOnWeekoff,
                                               Halfday = p.IsHalfDay,
                                           }).ToList();

                        var UserList = (from u in context.Users
                                        where u.Status == 1 && u.CompanyID == SelectedCompanyId
                                        select new
                                        {
                                            u.UserName,
                                            u.UserId,
                                            UserWeekOffDayList = context.UserWeekOffDays.Where(w => w.UserId == u.UserId).GroupBy(o => o.WeekOffDay).AsEnumerable().ToList(),
                                            TotalLeavelCountUserWise = (from il in context.Leaves
                                                                        join ila in context.UserLeaveApprovals on il.LeaveId equals ila.LeaveId into finalResult
                                                                        from fr in finalResult.DefaultIfEmpty()
                                                                        where il.UserId == u.UserId && (ConvertedSelectedMonth.Month == DateTime.Today.Month ? (il.LeaveFromDate.Month == DateTime.Today.Month && il.LeaveFromDate < DateTime.Today) || il.LeaveToDate.Month == DateTime.Today.Month : il.LeaveFromDate.Month == DateTime.Today.Month || il.LeaveToDate.Month == DateTime.Today.Month) &&
                                                                        (context.Leaves.Where(w => w.UserId.Equals(u.UserId)).Select(s => s.LeaveId).Contains(il.LeaveId) &&
                                                                          ((u.LeaveApprovalTypeId.Equals(3)) || (u.LeaveApprovalTypeId.Equals(1) && fr.LeaveId == il.LeaveId && fr.IsApprovedFirst == true) || (u.LeaveApprovalTypeId.Equals(2) && fr.LeaveId == il.LeaveId && fr.IsApprovedFirst == true && fr.IsApprovedSecond == true)))

                                                                        select new
                                                                        {
                                                                            tc = ConvertedSelectedMonth.Month == DateTime.Today.Month ?
                                                                                il.LeaveFromDate < ConvertedSelectedMonth ?
                                                                                    il.LeaveToDate < DateTime.Today.AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - ConvertedSelectedMonth.Date).Days + 1 : (DateTime.Today.AddDays(-1).Date - ConvertedSelectedMonth.Date).Days + 1 :
                                                                                    il.LeaveToDate < DateTime.Today.AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - il.LeaveFromDate.Date).Days + 1 : (DateTime.Today.AddDays(-1).Date - il.LeaveFromDate.Date).Days + 1 :
                                                                                il.LeaveFromDate < ConvertedSelectedMonth ?
                                                                                    il.LeaveToDate < ConvertedSelectedMonth.AddMonths(1).AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - ConvertedSelectedMonth.Date).Days + 1 : (ConvertedSelectedMonth.AddMonths(1).AddDays(-1).Date - ConvertedSelectedMonth.Date).Days + 1 :
                                                                                    il.LeaveToDate < ConvertedSelectedMonth.AddMonths(1).AddDays(-1) ?
                                                                                        (il.LeaveToDate.Date - il.LeaveFromDate.Date).Days + 1 : (ConvertedSelectedMonth.AddMonths(1).AddDays(-1).Date - il.LeaveFromDate.Date).Days + 1
                                                                        }).ToList(),
                                        }).ToList();

                        var TotalHoliday = 0;

                        if (ConvertedSelectedMonth.Month == DateTime.Today.Month)
                        {
                            TotalHoliday = (from u in context.Holidays
                                            where u.HolidayDate.Month == DateTime.Today.Month && u.HolidayDate.Date < DateTime.Today.Date
                                            select u).Count();
                        }
                        else
                        {
                            TotalHoliday = (from u in context.Holidays
                                            where u.HolidayDate.Month == ConvertedSelectedMonth.Month
                                            select u).Count();
                        }

                        var Sheet = 2;
                        ExcelWorksheet Commonws = CreateSheet(ep, "AllUserCommonReport", 1);

                        foreach (var ul in UserList)
                        {
                            var Finish = ConvertedSelectedMonth.Month == DateTime.Today.Month ? DateTime.Today.AddDays(-1) : ConvertedSelectedMonth.AddMonths(1).AddDays(-1);
                            var TotalWeekoffDay = GF.GetTotalWeekoffDay(ul.UserWeekOffDayList, ConvertedSelectedMonth, Finish);

                            PDetail.DetailList = (from p in GetDBDetail
                                                  where p.UserId == ul.UserId
                                                  select new PunchinDetailList()
                                                  {
                                                      Date = p.PunchInTime.ToString("dd-MMM-yyyy"),//string.Format("{0:dd-MM-yy}", p.PunchInTime),
                                                      PunchinTime = p.PunchInTime.ToString("hh:mm:ss tt"),
                                                      LatePunchin = p.LatePunchIn,
                                                      LatePunchinReason = p.LatePunchInReason,
                                                      IsLocationTypePunchin = p.PunchInType.Value == true ? "Inside Location" : "Outside Location",
                                                      PunchinLocation = p.PunchinLocation,
                                                      PunchinOutsideLocationReason = p.PunchInOutsideLocationReason,
                                                      PunchOutTime = p.PunchOutTime.Value.ToString("hh:mm:ss tt"),
                                                      EarlyPunchOut = p.EarlyPunchOut,
                                                      EarlyPunchoutReason = p.EarlyPunchoutReason,
                                                      IsLocationTypePunchout = p.PunchOutType == true ? "Inside Location" : "Outside Location",
                                                      PunchoutLocation = p.PunchoutLocation,
                                                      PunchoutOutsideLocationReason = p.PunchOutOutsideLocationReason,
                                                      IsSystemPunchout = p.SystemPunchOut,
                                                      PunchinDeviceId = p.PunchinDeviceId,
                                                      PunchoutDeviceId = p.PunchoutDeviceId,
                                                      WorkHourReason = p.WorkHourReason,
                                                      OfficeHour = p.OfficeHour.Hours + ":" + p.OfficeHour.Minutes + ":" + p.OfficeHour.Seconds,
                                                      TotalBreak = p.TotalBreak,
                                                      TotalBreakTime = p.TotalBreak > 0 ? new TimeSpan(p.TotalBreakTimeTicks).Hours + ":" + new TimeSpan(p.TotalBreakTimeTicks).Minutes + ":" + new TimeSpan(p.TotalBreakTimeTicks).Seconds : "",
                                                      WorkHour = p.TotalBreak > 0 ? new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks).Hours + ":" + new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks).Minutes + ":" + new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks).Seconds : p.OfficeHour.Hours + ":" + p.OfficeHour.Minutes + ":" + p.OfficeHour.Seconds,
                                                      WorkHourInTimeSpan = p.TotalBreak > 0 ? new TimeSpan(p.OfficeHour.Ticks - p.TotalBreakTimeTicks) : p.OfficeHour,
                                                      PunchinOnHoliday = p.PunchinOnHoliday,
                                                      PunchinOnWeekoff = p.PunchinOnWeekoff,
                                                      Halfday = p.Halfday,
                                                  }
                                       ).OrderBy(x => x.Date).ToList();

                            PDetail.UserName = ul.UserName;
                            PDetail.MonthName = SelectedMonth;

                            var tmp = (from p in GetDBDetail
                                       where p.UserId == ul.UserId
                                       group p by p.UserId into g
                                       select g).AsEnumerable()
                                     .Select(g => new
                                     {
                                         TotalPreserntDay = g.Count(c => c.PunchinOnHoliday == false && c.PunchinOnWeekoff == false),
                                         TotalWorkingHour = TimeSpan.FromTicks(g.Where(w => w.Halfday == false).Sum(a => a.OfficeHour.Ticks - a.TotalBreakTimeTicks)),
                                         AverageWorkingHour = g.Count(c => c.Halfday == false) > 0 ? TimeSpan.FromTicks(g.Where(w => w.Halfday == false).Aggregate(new TimeSpan(), (sum, nextData) => sum.Add(nextData.OfficeHour - TimeSpan.FromTicks(nextData.TotalBreakTimeTicks))).Ticks / g.Count(c => c.Halfday == false)) : TimeSpan.FromTicks(0),
                                         TotalLatePunchin = g.Where(x => x.LatePunchIn == true).Count(),
                                         TotalEarlyPunchout = g.Where(x => x.EarlyPunchOut == true).Count(),
                                         TotalPunchinOutside = g.Where(x => x.PunchInType == false).Count(),
                                         TotalPunchoutOutside = g.Where(x => x.PunchOutType == false).Count(),
                                         TotalSystemPunchout = g.Where(x => x.SystemPunchOut == true).Count(),
                                         UncomletedWorkhour = g.Where(x => x.WorkHourReason != null).Count(),
                                         TotalHalfday = g.Count(c => c.Halfday == true),
                                         PunchinOnHoliday = g.Count(c => c.PunchinOnHoliday),
                                         PunchinOnWeekoff = g.Count(c => c.PunchinOnWeekoff),
                                     }).FirstOrDefault();

                            if (tmp != null)
                            {
                                PDetail.DayInMonth = ConvertedSelectedMonth.Month == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.AddDays(-1).Day) : DateTime.DaysInMonth(ConvertedSelectedMonth.Year, ConvertedSelectedMonth.Month);
                                PDetail.Holiday = TotalHoliday;
                                PDetail.WeekoffDay = TotalWeekoffDay;
                                PDetail.LeaveDay = ul.TotalLeavelCountUserWise.Count() > 0 ? ul.TotalLeavelCountUserWise.Sum(t => t.tc) - (TotalHoliday + TotalWeekoffDay) : 0;
                                PDetail.WorkingDay = PDetail.DayInMonth - (PDetail.Holiday + PDetail.WeekoffDay);
                                PDetail.TotalPresentDay = tmp.TotalPreserntDay;
                                PDetail.PunchinOnHoliday = tmp.PunchinOnHoliday;
                                PDetail.PunchinOnWeekoff = tmp.PunchinOnWeekoff;
                                PDetail.TotalAbsentDay = PDetail.WorkingDay - PDetail.TotalPresentDay;
                                PDetail.TotalWorkingDay = PDetail.WorkingDay + PDetail.PunchinOnWeekoff + PDetail.PunchinOnHoliday - PDetail.TotalAbsentDay;
                                PDetail.HalfDay = tmp.TotalHalfday;
                                PDetail.TotalWorkingHour = ((tmp.TotalWorkingHour.Days * 24) + tmp.TotalWorkingHour.Hours) + ":" + tmp.TotalWorkingHour.Minutes + ":" + tmp.TotalWorkingHour.Seconds;
                                PDetail.AverageWorkingHour = tmp.AverageWorkingHour.Hours + ":" + tmp.AverageWorkingHour.Minutes + ":" + tmp.AverageWorkingHour.Seconds;
                                PDetail.TotalLatePunchin = tmp.TotalLatePunchin;
                                PDetail.TotalEarlyPunchout = tmp.TotalEarlyPunchout;
                                PDetail.TotalPunchinOutside = tmp.TotalPunchinOutside;
                                PDetail.TotalPunchoutOutside = tmp.TotalPunchoutOutside;
                                PDetail.TotalSystemPunchout = tmp.TotalSystemPunchout;
                                PDetail.UncomletedWorkhour = tmp.UncomletedWorkhour;

                                CommonReportList.Add(new CSVDetail
                                {
                                    UserName = ul.UserName,
                                    TotalWorkingDay = PDetail.TotalWorkingDay,
                                    TotalPresentDay = PDetail.TotalPresentDay + PDetail.PunchinOnHoliday + PDetail.PunchinOnWeekoff,
                                    TotalAbsentDay = PDetail.TotalAbsentDay,
                                    TotalWeekoff = PDetail.WeekoffDay,
                                    HalfDay = PDetail.HalfDay,
                                    TotalWorkingHour = PDetail.TotalWorkingHour,
                                    AverageWorkingHour = PDetail.AverageWorkingHour,
                                    TotalLatePunchin = PDetail.TotalLatePunchin,
                                    TotalEarlyPunchout = PDetail.TotalEarlyPunchout,
                                    TotalPunchinOutside = PDetail.TotalPunchinOutside,
                                    TotalPunchoutOutside = PDetail.TotalPunchoutOutside,
                                    TotalSystemPunchout = PDetail.TotalSystemPunchout,
                                    UncomletedWorkhour = PDetail.UncomletedWorkhour
                                });
                            }
                            else
                            {
                                CommonReportList.Add(new CSVDetail
                                {
                                    UserName = ul.UserName,
                                    TotalWorkingDay = ConvertedSelectedMonth.Month == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.Day) - (TotalHoliday + TotalWeekoffDay + 1) : System.DateTime.DaysInMonth(ConvertedSelectedMonth.Year, ConvertedSelectedMonth.Month) - (TotalHoliday + TotalWeekoffDay),
                                    TotalPresentDay = 0,
                                    TotalAbsentDay = ConvertedSelectedMonth.Month == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.Day) - (TotalHoliday + TotalWeekoffDay + 1) : System.DateTime.DaysInMonth(ConvertedSelectedMonth.Year, ConvertedSelectedMonth.Month) - (TotalHoliday + TotalWeekoffDay),
                                    HalfDay = 0,
                                    TotalWorkingHour = "0",
                                    AverageWorkingHour = "0",
                                    TotalLatePunchin = 0,
                                    TotalEarlyPunchout = 0,
                                    TotalPunchinOutside = 0,
                                    TotalPunchoutOutside = 0,
                                    TotalSystemPunchout = 0,
                                    UncomletedWorkhour = 0
                                });
                            }

                            //Create a sheet
                            ExcelWorksheet ws = CreateSheet(ep, ul.UserName, Sheet);
                            Sheet++;

                            ws.Cells[1, 1].Value = "User";
                            ws.Cells[1, 1].Style.Font.Size = 15;

                            ws.Cells[1, 2].Value = PDetail.UserName;
                            ws.Cells[1, 2].Style.Font.Bold = true;
                            ws.Cells[1, 2].Style.Font.Size = 15;

                            ws.Cells[1, 4].Value = "Month";
                            ws.Cells[1, 4].Style.Font.Size = 15;

                            ws.Cells[1, 5].Value = PDetail.MonthName;
                            ws.Cells[1, 5].Style.Font.Bold = true;
                            ws.Cells[1, 5].Style.Font.Size = 15;

                            var collist = new string[]
                            { "Date","Work Hour","Punchin Time","Punchout Time","Late Punchin","Late Punchin Reason","Punchin Type","Punchin Location","Punchin Outside Location Reason",
                                "Early Punchout","Early Punchout Reason","Punchout Type","Punchout Location","Punchout Outside Location Reason","System Punchout",
                                "Punchin Device","Punchout Device","Office Hour","Break Hour","Total Break","Work Hour Reason","Holiday","Weekoff","Halfday"
                            };

                            var templength = collist.Length;
                            for (var hi = 1; hi <= templength; hi++)
                            {
                                var cell = ws.Cells[3, hi];
                                var fill = cell.Style.Fill;
                                fill.PatternType = ExcelFillStyle.Solid;
                                //Setting the background color of header cells to Gray
                                fill.BackgroundColor.SetColor(Color.Gray);

                                //Setting Top/left,right/bottom borders.
                                var border = cell.Style.Border;
                                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                                //Setting Value in cell
                                cell.Value = collist[hi - 1];
                            }

                            var k = 3;
                            foreach (var pl in PDetail.DetailList)
                            {
                                var l = 1;
                                k++;
                                ws.Cells[k, l].Value = pl.Date;

                                ws.Cells[k, ++l].Value = pl.WorkHour;
                                if (pl.WorkHourInTimeSpan < TimeSpan.Parse("08:30"))
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }
                                ws.Cells[k, ++l].Value = pl.PunchinTime;
                                ws.Cells[k, ++l].Value = pl.PunchOutTime;
                                ws.Cells[k, ++l].Value = pl.LatePunchin;
                                if (pl.LatePunchin == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.LatePunchinReason;
                                ws.Cells[k, ++l].Value = pl.IsLocationTypePunchin;
                                if (pl.IsLocationTypePunchin == "Outside Location")
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.PunchinLocation;
                                ws.Cells[k, ++l].Value = pl.PunchinOutsideLocationReason;

                                ws.Cells[k, ++l].Value = pl.EarlyPunchOut;
                                if (pl.EarlyPunchOut == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }
                                ws.Cells[k, ++l].Value = pl.EarlyPunchoutReason;
                                ws.Cells[k, ++l].Value = pl.IsLocationTypePunchout;
                                if (pl.IsLocationTypePunchout == "Outside Location")
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.PunchoutLocation;
                                ws.Cells[k, ++l].Value = pl.PunchoutOutsideLocationReason;
                                ws.Cells[k, ++l].Value = pl.IsSystemPunchout;
                                if (pl.IsSystemPunchout == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.Red);
                                }

                                ws.Cells[k, ++l].Value = pl.PunchinDeviceId;
                                ws.Cells[k, ++l].Value = pl.PunchoutDeviceId;

                                ws.Cells[k, ++l].Value = pl.OfficeHour;
                                ws.Cells[k, ++l].Value = pl.TotalBreakTime;

                                ws.Cells[k, ++l].Value = pl.TotalBreak;
                                ws.Cells[k, ++l].Value = pl.WorkHourReason;
                                ws.Cells[k, ++l].Value = pl.PunchinOnHoliday;
                                if (pl.PunchinOnHoliday == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.LightSeaGreen);
                                }
                                ws.Cells[k, ++l].Value = pl.PunchinOnWeekoff;
                                if (pl.PunchinOnWeekoff == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.LightSeaGreen);
                                }
                                ws.Cells[k, ++l].Value = pl.Halfday;
                                if (pl.Halfday == true)
                                {
                                    var fill = ws.Cells[k, l].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.Solid;
                                    fill.BackgroundColor.SetColor(Color.CornflowerBlue);
                                }
                            }

                            k = k + 3;
                            if (tmp != null)
                            {
                                var SummuryStartRow = k;
                                ws.Cells[k, 1].Value = "Days in month";
                                ws.Cells[k, 2].Value = PDetail.DayInMonth;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "  (-)Holiday";
                                ws.Cells[k, 2].Value = PDetail.Holiday;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "  (-)Weekoff";
                                ws.Cells[k, 2].Value = PDetail.WeekoffDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = " Working days";
                                ws.Cells[k, 2].Value = PDetail.WorkingDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "  (-)Absent days";
                                ws.Cells[k, 2].Value = PDetail.TotalAbsentDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "Present on working days";
                                ws.Cells[k, 2].Value = PDetail.TotalPresentDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;
                                ws.Cells[k, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;

                                if (PDetail.PunchinOnHoliday > 0)
                                {
                                    ws.Cells[++k, 1].Value = "  (+)Present on holiday";
                                    ws.Cells[k, 2].Value = PDetail.PunchinOnHoliday;
                                    ws.Cells[k, 2].Style.Font.Bold = true;
                                }

                                if (PDetail.PunchinOnWeekoff > 0)
                                {
                                    ws.Cells[++k, 1].Value = "  (+)Present on weekoff";
                                    ws.Cells[k, 2].Value = PDetail.PunchinOnWeekoff;
                                    ws.Cells[k, 2].Style.Font.Bold = true;
                                }



                                ws.Cells[++k, 1].Value = "Total working days";
                                ws.Cells[k, 2].Value = PDetail.TotalWorkingDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                ws.Cells[++k, 1].Value = "";

                                ws.Cells[++k, 1].Value = "Total Halfday";
                                ws.Cells[k, 2].Value = PDetail.HalfDay;
                                ws.Cells[k, 2].Style.Font.Bold = true;

                                k = SummuryStartRow;

                                ws.Cells[k, 4].Value = "Total working hours \n\r (Excluding Halfday)";
                                ws.Cells[k, 5].Value = PDetail.TotalWorkingHour;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                                ws.Cells[++k, 4].Value = "Average working hours \n\r (Excluding Halfday)";
                                ws.Cells[k, 5].Value = PDetail.AverageWorkingHour;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total punchin late";
                                ws.Cells[k, 5].Value = PDetail.TotalLatePunchin;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                                ws.Cells[++k, 4].Value = "Total early punchout";
                                ws.Cells[k, 5].Value = PDetail.TotalEarlyPunchout;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total punchin outside";
                                ws.Cells[k, 5].Value = PDetail.TotalPunchinOutside;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                                ws.Cells[++k, 4].Value = "Total punchout outside";
                                ws.Cells[k, 5].Value = PDetail.TotalPunchoutOutside;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total system punchout";
                                ws.Cells[k, 5].Value = PDetail.TotalSystemPunchout;
                                ws.Cells[k, 5].Style.Font.Bold = true;

                                ws.Cells[++k, 4].Value = "Total uncompleted workhour day";
                                ws.Cells[k, 5].Value = PDetail.UncomletedWorkhour;
                                ws.Cells[k, 5].Style.Font.Bold = true;
                            }

                            ws.Cells["A1:U50"].AutoFitColumns();
                        }

                        // Start-- Common Report On First sheet
                        var i = 1;
                        Commonws.Cells[i, 1].Value = "Month";
                        Commonws.Cells[i, 1].Style.Font.Size = 15;

                        Commonws.Cells[i, 2].Value = PDetail.MonthName;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;
                        Commonws.Cells[i, 2].Style.Font.Size = 15;

                        Commonws.Cells[++i, 1].Value = "Days in month";
                        Commonws.Cells[i, 2].Value = PDetail.DayInMonth;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;

                        Commonws.Cells[++i, 1].Value = "  (-)Holiday";
                        Commonws.Cells[i, 2].Value = PDetail.Holiday;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;

                        Commonws.Cells[++i, 1].Value = " Working days (Excl. weekoff)";
                        Commonws.Cells[i, 2].Value = PDetail.DayInMonth - PDetail.Holiday;
                        Commonws.Cells[i, 2].Style.Font.Bold = true;

                        i += 2;
                        var CommonCollist = new string[]
                        { "Username","Total Working Days (Workingday - Weekoff - Absent)","Total Absent Days","Total Weekoff","Total halfday",
                            "Total working hours(Excl. Halfday)","Average working hour(Excl. Halfday)",
                            "Total punchin late","Total early punchout","Total punchin outside",
                                "Total punchout outside","Total system punchout","Total uncompleted workhour day"
                        };

                        var CommonListlength = CommonCollist.Length;
                        for (var hi = 1; hi <= CommonListlength; hi++)
                        {
                            var cell = Commonws.Cells[i, hi];
                            var fill = cell.Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            //Setting the background color of header cells to Gray
                            fill.BackgroundColor.SetColor(Color.Gray);

                            //Setting Top/left,right/bottom borders.
                            var border = cell.Style.Border;
                            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                            //Setting Value in cell
                            cell.Value = CommonCollist[hi - 1];
                        }

                        i++;

                        foreach (var pl in CommonReportList)
                        {
                            var j = 1;

                            Commonws.Cells[i, j].Value = pl.UserName;
                            Commonws.Cells[i, ++j].Value = pl.TotalWorkingDay;
                            Commonws.Cells[i, ++j].Value = pl.TotalAbsentDay;
                            Commonws.Cells[i, ++j].Value = pl.TotalWeekoff;
                            Commonws.Cells[i, ++j].Value = pl.HalfDay;
                            Commonws.Cells[i, ++j].Value = pl.TotalWorkingHour;
                            Commonws.Cells[i, ++j].Value = pl.AverageWorkingHour;
                            Commonws.Cells[i, ++j].Value = pl.TotalLatePunchin;
                            Commonws.Cells[i, ++j].Value = pl.TotalEarlyPunchout;
                            Commonws.Cells[i, ++j].Value = pl.TotalPunchinOutside;
                            Commonws.Cells[i, ++j].Value = pl.TotalPunchoutOutside;
                            Commonws.Cells[i, ++j].Value = pl.TotalSystemPunchout;
                            Commonws.Cells[i, ++j].Value = pl.UncomletedWorkhour;

                            i++;
                        }

                        Commonws.Cells["A1:U50"].AutoFitColumns();

                        // END-- Common Report On First sheet

                        var bin = ep.GetAsByteArray();
                        var filepath = Server.MapPath("ExcelDownload");

                        if (!Directory.Exists(filepath))
                        {
                            Directory.CreateDirectory(filepath);
                        }

                        var temp = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

                        File.WriteAllBytes((filepath + "\\" + temp), bin);
                        rootDocsPath = "ExcelDownload/" + temp;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                ExcelFilePath = rootDocsPath,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        [WebMethod]
        public string GetDashboardData(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID, int UserID)
        {
            //  List<MenuDetail> MDetail = new List<MenuDetail>();

            Dashboard UDetail = new Dashboard();
            Dashboard PreviousUDetail = new Dashboard();
            List<TeamAvailability> TeamDetail = new List<TeamAvailability>();
            List<MonthHours> MonthDetail = new List<MonthHours>();

            var ConvertedSelectedMonth = Convert.ToDateTime(DateTime.Now);
            int prevMonths = ConvertedSelectedMonth.AddMonths(-1).Month;
            int prevyear = ConvertedSelectedMonth.AddMonths(-1).Year;
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {

                    using (var context = new ELDBDataContext())
                    {

                        var userID = (from u in context.Users where u.UserName == UserName select u.UserId).FirstOrDefault();

                        #region PieChart

                        var Punchin = (from u in context.PunchIns where u.UserId == userID && u.PunchinTime.Month == ConvertedSelectedMonth.Month && u.PunchinTime.Year == ConvertedSelectedMonth.Year && u.PunchoutTime != null select u.PId).Count();
                        var OutsidePunchin = (from u in context.PunchIns where u.UserId == userID && u.PunchinType == false && u.PunchinTime.Month == ConvertedSelectedMonth.Month && u.PunchinTime.Year == ConvertedSelectedMonth.Year && u.PunchoutTime != null select u.PId).Count();
                        var LatePunchin = (from u in context.PunchIns where u.UserId == userID && u.LatePunchin == true && u.PunchinTime.Month == ConvertedSelectedMonth.Month && u.PunchinTime.Year == ConvertedSelectedMonth.Year && u.PunchoutTime != null select u.PId).Count();
                        var EarlyPounchout = (from u in context.PunchIns where u.UserId == userID && u.EarlyPunchout == true && u.PunchinTime.Month == ConvertedSelectedMonth.Month && u.PunchinTime.Year == ConvertedSelectedMonth.Year && u.PunchoutTime != null select u.PId).Count();
                        var UserWeekOffDayList = context.UserWeekOffDays.Where(w => w.UserId == userID).GroupBy(o => o.WeekOffDay).AsEnumerable().ToList();
                        var DayInMonth = ConvertedSelectedMonth.Month == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.AddDays(-1).Day) : DateTime.DaysInMonth(ConvertedSelectedMonth.Year, ConvertedSelectedMonth.Month);

                        var setpunchin = (from u in context.SystemSettings where u.KeyName == "Punchinper" select u.value).FirstOrDefault();
                        var setOutsidePunchin = (from u in context.SystemSettings where u.KeyName == "OutsidePunchinper" select u.value).FirstOrDefault();
                        var setLatePunchin = (from u in context.SystemSettings where u.KeyName == "LatePunchinper" select u.value).FirstOrDefault();
                        var setEarlyPounchout = (from u in context.SystemSettings where u.KeyName == "EarlyPounchoutper" select u.value).FirstOrDefault();


                        var OutsidePunchinper = 0.00;
                        var LatePunchinper = 0.00;
                        var EarlyPounchoutper = 0.00;
                        var Punchinper = 0.00;
                        if (DayInMonth != 0)
                        {
                            Punchinper = Convert.ToDouble(((Punchin * 100) / (DayInMonth)));
                        }

                        if (Punchinper != 0.00)
                        {
                            OutsidePunchinper = Convert.ToDouble(((OutsidePunchin * 100) / Punchin));
                            LatePunchinper = Convert.ToDouble(((LatePunchin * 100) / Punchin));
                            EarlyPounchoutper = Convert.ToDouble(((EarlyPounchout * 100) / Punchin));

                        }

                        var rptOutsudepunch = 0.00;
                        var rptLatePunchin = 0.00;
                        var rptPunchout = 0.00;

                        var rptppunch = (Punchinper * Convert.ToInt32(setpunchin)) / 100;
                        if (rptppunch != 0)
                        {
                            rptOutsudepunch = ((100 - OutsidePunchinper) * Convert.ToInt32(setOutsidePunchin)) / 100;
                            rptLatePunchin = ((100 - LatePunchinper) * Convert.ToInt32(setLatePunchin)) / 100;
                            rptPunchout = ((100 - EarlyPounchoutper) * Convert.ToInt32(setEarlyPounchout)) / 100;
                        }

                        var Accuracy = rptppunch + rptOutsudepunch + rptLatePunchin + rptPunchout;

                        UDetail.Punchin = Punchinper;
                        UDetail.OutsidePunchin = OutsidePunchinper;
                        UDetail.LatePunchin = LatePunchinper;
                        UDetail.EarlyPounchout = EarlyPounchoutper;
                        UDetail.RepPunchin = rptppunch;
                        UDetail.RepOutsidePunchin = rptOutsudepunch;
                        UDetail.RepLatePunchin = rptLatePunchin;
                        UDetail.RepEarlyPounchout = rptPunchout;
                        UDetail.FinalRepPer = Math.Round(Accuracy, 2);
                        UDetail.PunchinCount = Punchin;
                        UDetail.LatePunchinCount = LatePunchin;
                        UDetail.OutsidePunchinCount = OutsidePunchin;
                        UDetail.EarlyPounchoutCount = EarlyPounchout;
                        #endregion

                        #region PieChartPrevious

                        var PunchinPre = (from u in context.PunchIns where u.UserId == userID && u.PunchinTime.Month == prevMonths && u.PunchinTime.Year == prevyear && u.PunchoutTime != null select u.PId).Count();
                        var OutsidePunchinPre = (from u in context.PunchIns where u.UserId == userID && u.PunchinType == false && u.PunchinTime.Month == prevMonths && u.PunchinTime.Year == prevyear && u.PunchoutTime != null select u.PId).Count();
                        var LatePunchinPre = (from u in context.PunchIns where u.UserId == userID && u.LatePunchin == true && u.PunchinTime.Month == prevMonths && u.PunchinTime.Year == prevyear && u.PunchoutTime != null select u.PId).Count();
                        var EarlyPounchoutPre = (from u in context.PunchIns where u.UserId == userID && u.EarlyPunchout == true && u.PunchinTime.Month == prevMonths && u.PunchinTime.Year == prevyear && u.PunchoutTime != null select u.PId).Count();
                        var UserWeekOffDayListPre = context.UserWeekOffDays.Where(w => w.UserId == userID).GroupBy(o => o.WeekOffDay).AsEnumerable().ToList();
                        var DayInMonthPre = prevMonths == DateTime.Today.Month ? Convert.ToInt32(DateTime.Today.AddDays(-1).Day) : DateTime.DaysInMonth(prevyear, prevMonths);

                        var setpunchinPre = (from u in context.SystemSettings where u.KeyName == "Punchinper" select u.value).FirstOrDefault();
                        var setOutsidePunchinPre = (from u in context.SystemSettings where u.KeyName == "OutsidePunchinper" select u.value).FirstOrDefault();
                        var setLatePunchinPre = (from u in context.SystemSettings where u.KeyName == "LatePunchinper" select u.value).FirstOrDefault();
                        var setEarlyPounchoutPre = (from u in context.SystemSettings where u.KeyName == "EarlyPounchoutper" select u.value).FirstOrDefault();

                        var OutsidePunchinperPre = 0.00;
                        var LatePunchinperPre = 0.00;
                        var EarlyPounchoutperPre = 0.00;
                        var PunchinperPre = 0.00;
                        if (DayInMonth != 0)
                        {
                            PunchinperPre = Convert.ToDouble(((Punchin * 100) / (DayInMonth)));
                        }

                        if (Punchinper != 0.00)
                        {
                            OutsidePunchinperPre = Convert.ToDouble(((OutsidePunchin * 100) / Punchin));
                            LatePunchinperPre = Convert.ToDouble(((LatePunchin * 100) / Punchin));
                            EarlyPounchoutperPre = Convert.ToDouble(((EarlyPounchout * 100) / Punchin));

                        }

                        var rptOutsudepunchPre = 0.00;
                        var rptLatePunchinPre = 0.00;
                        var rptPunchoutPre = 0.00;

                        var rptppunchPre = (Punchinper * Convert.ToInt32(setpunchin)) / 100;
                        if (rptppunch != 0)
                        {
                            rptOutsudepunchPre = ((100 - OutsidePunchinper) * Convert.ToInt32(setOutsidePunchin)) / 100;
                            rptLatePunchinPre = ((100 - LatePunchinper) * Convert.ToInt32(setLatePunchin)) / 100;
                            rptPunchoutPre = ((100 - EarlyPounchoutper) * Convert.ToInt32(setEarlyPounchout)) / 100;
                        }

                        var AccuracyPre = rptppunchPre + rptOutsudepunchPre + rptLatePunchinPre + rptPunchoutPre;

                        PreviousUDetail.Punchin = PunchinperPre;
                        PreviousUDetail.OutsidePunchin = OutsidePunchinperPre;
                        PreviousUDetail.LatePunchin = LatePunchinperPre;
                        PreviousUDetail.EarlyPounchout = EarlyPounchoutperPre;
                        PreviousUDetail.RepPunchin = rptppunchPre;
                        PreviousUDetail.RepOutsidePunchin = rptOutsudepunchPre;
                        PreviousUDetail.RepLatePunchin = rptLatePunchinPre;
                        PreviousUDetail.RepEarlyPounchout = rptPunchoutPre;
                        PreviousUDetail.FinalRepPer = Math.Round(AccuracyPre, 2);
                        PreviousUDetail.PunchinCount = PunchinPre;
                        PreviousUDetail.LatePunchinCount = LatePunchinPre;
                        PreviousUDetail.OutsidePunchinCount = OutsidePunchinPre;
                        PreviousUDetail.EarlyPounchoutCount = EarlyPounchoutPre;
                        #endregion

                        #region TeamAvailiblity
                        TeamDetail = (from u in context.Users
                                      join ua in context.Users on u.GroupID equals ua.GroupID
                                      where u.UserId == userID && ua.UserId != u.UserId
                                      select new TeamAvailability()
                                      {
                                          UserName = (from us in context.Users where us.UserId == ua.UserId select us.FirstName + " " + us.LastName).FirstOrDefault(),
                                          IsPunchin = (from p in context.PunchIns where p.UserId == ua.UserId && p.PunchinTime.Date == DateTime.Now.Date select p.UserId).Count() != 0 ? true : false,
                                          LatePunchinReason = (from p in context.PunchIns where p.UserId == ua.UserId && p.PunchinTime.Date == DateTime.Now.Date select p.LatePunchinReason).FirstOrDefault(),
                                          OutLocationReason = (from p in context.PunchIns where p.UserId == ua.UserId && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinOutsideLocationReason).FirstOrDefault(),
                                          PunchinTime = Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserId && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinTime).FirstOrDefault()),
                                          PunchoutTime = Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserId && p.PunchinTime.Date == DateTime.Now.Date select p.PunchoutTime).FirstOrDefault()),
                                          Latlong = (from p in context.PunchIns where p.UserId == ua.UserId && p.PunchinTime.Date == DateTime.Now.Date select p.PILatitudeLongitude).FirstOrDefault(),
                                          DepartmentName = (from g in context.GroupMasters where g.ID == ua.GroupID select g.GroupName).FirstOrDefault(),
                                          MobileNo = (from us in context.Users where us.UserId == ua.UserId select us.MobileNoCmp).FirstOrDefault(),

                                      }).ToList();


                        //TeamDetail.AddRange((from u in context.Users
                        //                     join ua in context.UserAccesses on u.UserId equals ua.UserID
                        //                     where ua.UserID == userID
                        //                     select Map_TeamAvailability(context, ua)).ToList());

                        List<UserAccess> records = (from u in context.Users
                                                    join ua in context.UserAccesses on u.UserId equals ua.UserID
                                                    where ua.UserID == userID
                                                    select ua).ToList();

                        List<long> userIDs = records.Where(x => x.UserAccessID != null).Select(x => x.UserAccessID.Value).ToList();
                        List<User> userrecords = context.Users.Where(x => userIDs.Contains(x.UserId)).ToList();
                        List<PunchIn> punchRecords = (from p in context.PunchIns where userIDs.Contains(p.UserId) && p.PunchinTime.Date == DateTime.Now.Date select p).ToList();

                        List<int> groupMasterIds = userrecords.Where(x => x.GroupID != null).Select(x => x.GroupID.Value).ToList();
                        List<GroupMaster> GroupMasters = context.GroupMasters.Where(x => groupMasterIds.Contains(x.ID)).ToList();
                        foreach (UserAccess obj in records)
                        {
                            TeamDetail.Add(Map_TeamAvailability(context, obj, userrecords, punchRecords, GroupMasters));
                        }

                        //TeamDetail.AddRange((from u in context.Users
                        //                     join ua in context.UserAccesses on u.UserId equals ua.UserID
                        //                     where ua.UserID == userID
                        //                     select new TeamAvailability()
                        //                     {
                        //                         UserName = (from us in context.Users where us.UserId == ua.UserAccessID select us.FirstName + " " + us.LastName).FirstOrDefault(),
                        //                         IsPunchin = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.UserId).Count() != 0 ? true : false,
                        //                         LatePunchinReason = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.LatePunchinReason).FirstOrDefault(),
                        //                         EarlyPunchoutReason = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.EarlyPunchoutReason).FirstOrDefault(),
                        //                         OutLocationReason = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinOutsideLocationReason).FirstOrDefault(),
                        //                         PunchinTime = Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinTime).FirstOrDefault()),
                        //                         PunchoutTime = Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchoutTime).FirstOrDefault()),
                        //                         Latlong = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PILatitudeLongitude).FirstOrDefault(),
                        //                         DepartmentName = (from g in context.GroupMasters where g.ID == ((from uaa in context.Users where uaa.UserId == ua.UserAccessID select uaa.GroupID).FirstOrDefault()) select g.GroupName).FirstOrDefault(),
                        //                         MobileNo = (from us in context.Users where us.UserId == ua.UserAccessID select us.MobileNoCmp).FirstOrDefault(),
                        //                         OutLatlong = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.POLatitudeLongitude).FirstOrDefault(),
                        //                         OutLocationReason_PunchOut = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchoutOutsideLocationReason).FirstOrDefault(),

                        //                     }).ToList());

                        #endregion

                        #region 2ndPieChart

                        int year = DateTime.Now.Year;
                        int month = DateTime.Now.Month;
                        var ret = new List<DateTime>();
                        for (int i = 7; i != 0; i--)
                        {
                            ret.Add(new DateTime(year, month, 1).AddDays(-i));
                        }


                        for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
                        {
                            ret.Add(new DateTime(year, month, i));
                        }






                        foreach (var item in ret)
                        {
                            MonthHours mm = new MonthHours();
                            mm.CurrMonthDate = Convert.ToString(item.Date);
                            mm.Extra = ((from p in context.PunchIns where p.UserId == userID && p.PunchinTime.Date == item.Date select p.ExtraWorkingHour).FirstOrDefault()) != null ? Math.Round((TimeSpan.Parse(((from p in context.PunchIns where p.UserId == userID && p.PunchinTime.Date == item.Date select p.ExtraWorkingHour).FirstOrDefault()))).TotalHours, 2) : 0.00;
                            MonthDetail.Add(mm);
                        }



                        #endregion

                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {

                TeamDetail = TeamDetail,
                UDetail = UDetail,
                PreviousUDetail = PreviousUDetail,
                MonthDetail = MonthDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        public TeamAvailability Map_TeamAvailability(ELDBDataContext context, UserAccess ua, List<User> userrecords, List<PunchIn> punchrecords, List<GroupMaster> GroupMasters)
        {
            PunchIn obj_Punchin = (from p in punchrecords where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p).FirstOrDefault();


            TeamAvailability map = new TeamAvailability();

            if (obj_Punchin != null && obj_Punchin.PILocationId != null)
            {
                Location obo_location = context.Locations.FirstOrDefault(x => x.LocationId == obj_Punchin.PILocationId);
                if (obo_location != null)
                {
                    map.In_Latlong_Image = obo_location.LocationImage;
                    map.InLocation_Name = obo_location.PlaceName;
                }
            }

            if (obj_Punchin != null && obj_Punchin.POLocationId != null)
            {
                Location obo_location = context.Locations.FirstOrDefault(x => x.LocationId == obj_Punchin.POLocationId);
                if (obo_location != null)
                {
                    map.Out_Latlong_Image = obo_location.LocationImage;
                    map.OutLocation_Name = obo_location.PlaceName;
                }
            }

            try
            {
                User user = (from us in userrecords where us.UserId == ua.UserAccessID select us).FirstOrDefault();
                map.UserName = user.FirstName + " " + user.LastName;// (from us in context.Users where us.UserId == ua.UserAccessID select us.FirstName + " " + us.LastName).FirstOrDefault();
                map.DepartmentName = (from g in GroupMasters where g.ID == (user.GroupID) select g.GroupName).FirstOrDefault();
                map.MobileNo = user.MobileNoCmp;// (from us in context.Users where us.UserId == ua.UserAccessID select us.MobileNoCmp).FirstOrDefault();

                if (obj_Punchin != null)
                {
                    map.IsPunchin = true;// (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.UserId).Count() != 0 ? true : false;
                    map.LatePunchinReason = obj_Punchin.LatePunchinReason;// (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.LatePunchinReason).FirstOrDefault();
                    map.EarlyPunchoutReason = obj_Punchin.EarlyPunchoutReason;// (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.EarlyPunchoutReason).FirstOrDefault();
                    map.OutLocationReason = obj_Punchin.PunchinOutsideLocationReason; //(from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinOutsideLocationReason).FirstOrDefault();
                    map.PunchinTime = Convert.ToString(obj_Punchin.PunchinTime); //Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinTime).FirstOrDefault());
                    map.PunchoutTime = Convert.ToString(obj_Punchin.PunchoutTime); //Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchoutTime).FirstOrDefault());
                    map.Latlong = obj_Punchin.PILatitudeLongitude;// (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PILatitudeLongitude).FirstOrDefault();
                    map.OutLatlong = obj_Punchin.POLatitudeLongitude; //(from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.POLatitudeLongitude).FirstOrDefault();
                    map.OutLocationReason_PunchOut = obj_Punchin.PunchoutOutsideLocationReason; //(from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchoutOutsideLocationReason).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                map = new TeamAvailability();
            }


            return map;
        }
        #endregion

        #region ~Test

        [WebMethod]
        public string GetTest001(string abc)
        {
            using (var context = new ELDBDataContext())
            {

                string[] str_arr = abc.Split(',').ToArray();
                int[] int_arr = Array.ConvertAll(str_arr, Int32.Parse);

                var udetails = (from u in context.Users where int_arr.Contains(u.UserId) select u).ToList();

            }
            return Convert.ToString(DateTime.Now.DayOfWeek);

        }

        [WebMethod]
        public string GetTest(string Command, int UserId, int CompanyId)
        {

            using (var context = new ELDBDataContext())
            {
                var Udetail = (from u in context.Users where u.Status == 1 select u).ToList();


                foreach (var item in Udetail)
                {
                    var it = (from u in context.Users where u.Status == 1 && u.UserId == item.UserId select u).FirstOrDefault();
                    var cmpname = (from cm in context.Companies where cm.CompanyId == item.CompanyID select cm.CompanyName).FirstOrDefault();
                    it.UserCode = cmpname.Length > 5 ? cmpname.Substring(0, 5).ToUpper() + item.UserId.ToString() : cmpname + item.UserId.ToString();

                }
                context.SubmitChanges();
            }


            return Convert.ToString(DateTime.Now.DayOfWeek);

        }

        [WebMethod]
        public string GetTimetest(string date)
        {

            var callintime = DateTime.Now.ToString("yyyy-MMM-dd'T'HH:mm:ss.SSS");
            using (var context = new ELDBDataContext())
            {
                var data = (from c in context.CallIns where c.CallinTime.Date == DateTime.Now.Date select c).FirstOrDefault();
                callintime = data.CallinTime.ToString("yyyy-MMM-dd'T'HH:mm:ss");
            }
            //   return Convert.ToString(((DateTime.Now.Minute) - (dt.Minute) <= 2) ? "true" : "false");
            return callintime;
        }

        [WebMethod]
        public string ImageTest(string ImgString, string ImageType, string ImageName)
        {
            string succ = "Success";
            try
            {
                //byte[] bytes = Convert.FromBase64String(ImgString);

                //Image image;
                //using (MemoryStream ms = new MemoryStream(bytes))
                //{
                //    image = Image.FromStream(ms);
                //}

                var filepath = Server.MapPath("Images\\LocationImages");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var temp = ImageName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + ImageType;
                //var temp = ImageName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + ImageType;

                //image.Save((filepath + "\\" + temp));


                File.WriteAllBytes((filepath + "\\" + temp), Convert.FromBase64String(ImgString));


            }
            catch (Exception ex)
            {

                succ = ex.ToString();
            }




            return succ;

        }



        public Image LoadImage()
        {
            //data:image/gif;base64,
            //this image is a single pixel (black)
            byte[] bytes = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==");

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }


        [WebMethod]
        public List<DateTime> getAllDates()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;


            var ret = new List<DateTime>();
            for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            {
                ret.Add(new DateTime(year, month, i));
            }
            return ret;
        }

        #endregion

        #region ~Location
        [WebMethod]
        public string AddLocation(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int LocationId, string LocationName, int LocationCompanyId, List<LocationPin> locpinobj, string LocationImage, string LocationImageType)
        {
            var temp = LocationName + DateTime.Now.ToString("ddmmyyyy") + "." + LocationImageType;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                        //List<LocationPin> locpin = jsonSerializer.Deserialize<List<LocationPin>>(locpinobj);
                        //  List<LocationPin> locpin = JsonConvert.DeserializeObject<List<LocationPin>>(locpinobj);
                        //  var CheckLocation = (from l in context.Locations where l.Latitude == Convert.ToDouble(Latitude) && l.Longitude == Convert.ToDouble(Longitude) && l.IsDeleted.Equals(false) && CompanyId == LocationCompanyId select l.LocationId).Count();
                        if (LocationId == 0)
                        {
                            var LDetail = new Location();
                            LDetail.CompanyId = LocationCompanyId;
                            LDetail.PlaceName = LocationName;
                            LDetail.LocationName = LocationName;
                            LDetail.LocationImage = temp;
                            LDetail.IsDeleted = false;
                            LDetail.CreatedBy = LD.UserId;
                            LDetail.CreatedDate = DateTime.Now;
                            LDetail.UpdatedBy = LD.UserId;
                            LDetail.UpdatedDate = DateTime.Now;
                            context.Locations.InsertOnSubmit(LDetail);
                            context.SubmitChanges();

                            int id = (from loc in context.Locations orderby loc.LocationId select loc.LocationId).ToList().Last();




                            foreach (var item in locpinobj)
                            {
                                var Lpin = new LocationPin();

                                Lpin.Lat = item.Lat;
                                Lpin.Long = item.Long;
                                Lpin.PinAddress = item.PinAddress;
                                Lpin.Status = item.Status;
                                Lpin.LocationID = id;
                                context.LocationPins.InsertOnSubmit(Lpin);
                                context.SubmitChanges();
                            }



                            var filepath = Server.MapPath("Images\\LocationImages");

                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                            File.WriteAllBytes((filepath + "\\" + temp), Convert.FromBase64String(LocationImage));




                            LD.ReturnResult = "Location Added Successfully";




                        }
                        else
                        {

                            string oldimage = "";
                            var LDetail = (from l in context.Locations where l.LocationId == LocationId select l).FirstOrDefault();

                            LDetail.CompanyId = LocationCompanyId;
                            LDetail.PlaceName = LocationName;
                            LDetail.LocationName = LocationName;
                            if (LocationImage != "")
                            {
                                oldimage = LDetail.LocationImage;
                                temp = LocationName + DateTime.Now + "." + LocationImageType;
                                LDetail.LocationImage = temp;
                            }
                            LDetail.IsDeleted = false;
                            LDetail.UpdatedBy = LD.UserId;
                            LDetail.UpdatedDate = DateTime.Now;
                            context.SubmitChanges();





                            context.LocationPins.DeleteAllOnSubmit(context.LocationPins.Where(c => c.LocationID == LocationId));
                            context.SubmitChanges();

                            foreach (var item in locpinobj)
                            {
                                var Lpin = new LocationPin();

                                Lpin.Lat = item.Lat;
                                Lpin.Long = item.Long;
                                Lpin.PinAddress = item.PinAddress;
                                Lpin.Status = item.Status;
                                Lpin.LocationID = LocationId;
                                context.LocationPins.InsertOnSubmit(Lpin);
                                context.SubmitChanges();

                            }
                            LD.ReturnResult = "Location is  Updated sucessfully.";

                            if (LocationImage != "")
                            {
                                var filepath = Server.MapPath("Images\\LocationImages");

                                if (!Directory.Exists(filepath))
                                {
                                    Directory.CreateDirectory(filepath);
                                }

                                //image.Save((filepath + "\\" + temp));
                                try
                                {
                                    if (oldimage != "")
                                    {
                                        File.Delete(filepath + "\\" + oldimage);
                                    }

                                }
                                catch (Exception)
                                {

                                    throw;
                                }


                                File.WriteAllBytes((filepath + "\\" + temp), Convert.FromBase64String(LocationImage));
                            }
                        }



                    }
                }

            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server is not Resopnding";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListLocation(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<LocationDetail> LDetail = new List<LocationDetail>();
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        if (GF.ShowAllCompanyDetail(context, LD.UserId))
                        {
                            LDetail = context.Locations.Where(p => p.IsDeleted == false)
                            .Select(l => new
                            {
                                LocationId = l.LocationId,
                                // Longitude = l.Longitude,
                                //  Latitude = l.Latitude,
                                LocationAddress = l.LocationName,
                                LocationImage = l.LocationImage,
                                PlaceName = l.PlaceName,
                                CompanyName = (from c in context.Companies where c.CompanyId == l.CompanyId select c.CompanyName).FirstOrDefault(),
                                LPinDetail = (from c in context.LocationPins where c.LocationID == l.LocationId select new LocationPin { LocationID = c.LocationID, Lat = c.Lat, Long = c.Long, PinAddress = c.PinAddress, Status = c.Status }).ToList(),

                                //LunchFrom = l.LunchTimeFrom,
                                //LunchTo = l.LunchTimeTo,
                                //TeaFrom = l.TeaTimeFrom,
                                //TeaTo = l.TeaTimeTo,
                                //   TotalBreakCount = l.TotalBreakCount,
                                // BreakCategoryName = context.BreakCategories.Where(p => (p.BreakCategoryId == l.BreakCategoryId)).Select(p => p.BreakCategoryName).FirstOrDefault(),
                            })
                            .AsEnumerable()
                            .Select(c => new LocationDetail()
                            {
                                LocationId = c.LocationId,
                                // Longitude = c.Longitude,
                                // Latitude = c.Latitude,
                                LocationAddress = c.LocationAddress,
                                PlaceName = c.PlaceName,
                                CompanyName = c.CompanyName,
                                LocationImage = c.LocationImage,
                                //  Lp = c.LPinDetail,

                                //LunchFrom = (c.LunchFrom != null ? c.LunchFrom.ToString("hh:mm tt") + " To " + c.LunchTo.ToString("hh:mm tt") : "Not Available"),
                                //TeaFrom = (c.TeaFrom != null ? c.TeaFrom.ToString("hh:mm tt") + " To " + c.TeaTo.ToString("hh:mm tt") : "Not Available"),
                                //  TotalBreakCountWord = c.TotalBreakCount == 0 ? "No Limit" : c.TotalBreakCount.ToString(),
                                //  BreakCategoryName = c.BreakCategoryName,
                            }).OrderBy(p => p.CompanyName).ToList();
                        }
                        else
                        {
                            LDetail = context.Locations.Where(p => (p.IsDeleted == false) && (p.CompanyId == CompanyId))
                            .Select(l => new
                            {
                                LocationId = l.LocationId,
                                Longitude = l.Longitude,
                                Latitude = l.Latitude,
                                LocationAddress = l.LocationName,
                                PlaceName = l.PlaceName,
                                LocationImage = l.LocationImage,
                            })
                            .AsEnumerable()
                            .Select(c => new LocationDetail()
                            {
                                LocationId = c.LocationId,
                                Longitude = c.Longitude,
                                Latitude = c.Latitude,
                                LocationAddress = c.LocationAddress,
                                LocationImage = c.LocationImage,
                                PlaceName = c.PlaceName,
                            }).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
                //GF.SendEmail(e.ToString(), "ListLocations", "");
            }
            var FinalResult = new
            {
                LDetail = LDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListLocationPin(int LocationID)
        {
            List<LocationPin> LPDetail = new List<LocationPin>();
            try
            {
                //check Username Password with device



                using (var context = new ELDBDataContext())
                {
                    LPDetail = context.LocationPins.Where(p => p.LocationID == LocationID)
                    .Select(l => new
                    {
                        LocationID = l.LocationID,
                        Lat = l.Lat,
                        Long = l.Long,
                        PinAddress = l.PinAddress,
                        Status = l.Status,
                    })
                    .AsEnumerable()
                    .Select(c => new LocationPin()
                    {
                        LocationID = c.LocationID,
                        Lat = c.Lat,
                        Long = c.Long,
                        PinAddress = c.PinAddress,
                        Status = c.Status,


                        //LunchFrom = (c.LunchFrom != null ? c.LunchFrom.ToString("hh:mm tt") + " To " + c.LunchTo.ToString("hh:mm tt") : "Not Available"),
                        //TeaFrom = (c.TeaFrom != null ? c.TeaFrom.ToString("hh:mm tt") + " To " + c.TeaTo.ToString("hh:mm tt") : "Not Available"),
                        //  TotalBreakCountWord = c.TotalBreakCount == 0 ? "No Limit" : c.TotalBreakCount.ToString(),
                        //  BreakCategoryName = c.BreakCategoryName,
                    }).ToList();

                }

            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
                //  GF.SendEmail(e.ToString(), "ListLocationPins", "");
            }
            var FinalResult = new
            {
                LPDetail = LPDetail,
                type = "success",
                reason = LD.FailureReason,
                msg = "Location Pin",
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        #endregion

        #region ~Punchinreg

        [WebMethod]
        public string CheckPunchinStatus(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            var IsPunchinExist = false;
            var IsPunchinComplete = false;
            var UncompletedBreakId = 0;
            var PunchinTime = "";
            var PunchoutTime = "";
            var Punchinlatlong = "";
            var Punchoutlatlong = "";
            var PunchinLocImg = "";
            var PunchoutLocImg = "";
            var PunchinLocName = "";
            var PunchoutLocName = "";
            var LatePunchinReason = "";
            var PunchinOutsideLocationReason = "";
            var EarlyPunchoutReason = "";
            var PunchoutOutsideLocationReason = "";
            var WorkingHour = "";
            var TotalWorkHour = "";
            List<BreakDetail> BDetail = new List<BreakDetail>();
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var p = (from pn in context.PunchIns where pn.UserId == LD.UserId && pn.PunchinTime.Date == DateTime.Today.Date select pn).FirstOrDefault();

                        if (p != null)
                        {
                            PunchinTime = p.PunchinTime.ToString("hh:mm:ss tt");
                            Punchinlatlong = p.PILatitudeLongitude;
                            Punchoutlatlong = p.POLatitudeLongitude;
                            PunchoutTime = p.PunchoutTime != null ? p.PunchoutTime.Value.ToString("hh:mm:ss tt") : "";
                            WorkingHour = p.PunchoutTime != null ?
                                                       (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Hours + ":" +
                                                       (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Minutes + ":" +
                                                       (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Seconds : "";
                            IsPunchinExist = true;
                            IsPunchinComplete = p.PunchoutTime == null ? false : true;
                            LatePunchinReason = p.LatePunchinReason;
                            PunchinLocImg = p.PunchinType == true ? (from l in context.Locations
                                                                     where l.LocationId == Convert.ToInt32(p.PILocationId)
                                                                     select l.LocationImage).FirstOrDefault() : " N.A.";
                            PunchoutLocImg = p.PunchinType == true ? (from l in context.Locations
                                                                      where l.LocationId == Convert.ToInt32(p.POLocationId)
                                                                      select l.LocationImage).FirstOrDefault() : " N.A.";
                            PunchinLocName = p.PunchinType == true ? (from l in context.Locations
                                                                      where l.LocationId == Convert.ToInt32(p.PILocationId)
                                                                      select l.PlaceName).FirstOrDefault() : " N.A.";
                            PunchoutLocName = p.PunchinType == true ? (from l in context.Locations
                                                                       where l.LocationId == Convert.ToInt32(p.POLocationId)
                                                                       select l.PlaceName).FirstOrDefault() : " N.A.";
                            PunchinOutsideLocationReason = p.PunchinOutsideLocationReason;
                            EarlyPunchoutReason = p.EarlyPunchoutReason;
                            PunchoutOutsideLocationReason = p.PunchoutOutsideLocationReason;


                            BDetail = (from b in context.Breaks
                                       where b.UserId == LD.UserId && b.BreakinTime.Date == p.PunchinTime.Date
                                       select new
                                       {
                                           BreakId = b.BreakId,
                                           BreakinTime = b.BreakinTime,
                                           BreakoutTime = b.BreakoutTime,
                                       }
                                       ).AsEnumerable()
                                       .Select(b => new BreakDetail()
                                       {
                                           BreakId = b.BreakId,
                                           BreakinTime = b.BreakinTime.ToString("hh:mm:ss tt"),
                                           BreakoutTime = b.BreakoutTime != null ? b.BreakoutTime.Value.ToString("hh:mm:ss tt") : "",
                                           BreakingHour = b.BreakoutTime != null ?
                                                           (b.BreakoutTime.Value.TimeOfDay - b.BreakinTime.TimeOfDay).Hours + ":" +
                                                           (b.BreakoutTime.Value.TimeOfDay - b.BreakinTime.TimeOfDay).Minutes + ":" +
                                                           (b.BreakoutTime.Value.TimeOfDay - b.BreakinTime.TimeOfDay).Seconds
                                                   : "",
                                       }).OrderBy(pr => pr.BreakId).ToList();

                            UncompletedBreakId = (from b in context.Breaks
                                                  where b.UserId == LD.UserId && b.BreakinTime.Date == p.PunchinTime.Date && b.BreakoutTime == null
                                                  select b.BreakId).FirstOrDefault();
                            if (IsPunchinComplete)
                            {
                                if (BDetail.Count() > 0)
                                {
                                    var TotalOfficeTime = p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay;
                                    var TotalWorkTime = (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == p.PunchinTime.Date).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks)));
                                    var total = new TimeSpan(TotalOfficeTime.Ticks - TotalWorkTime);
                                    TotalWorkHour = "Total Working Hour - " + total.Hours + " : " + total.Minutes + " : " + total.Seconds;
                                }
                                else
                                {
                                    TotalWorkHour = WorkingHour;
                                }
                            }

                            //  IsBreakComplete = TempResponse == null ? true : false;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + "Punchin Status" + ee.ToString();
                LD.FailureReason = "";
            }

            CheckUserLeaveForDay_Model obj_CheckUserLeaveForDay_Model = BAModel.BAModel.CheckUserLeaveForDay(DateTime.Now, LD.UserId);

            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                IsPunchinExist = IsPunchinExist,
                IsPunchinComplete = IsPunchinComplete,
                PunchinTime = PunchinTime,
                PunchoutTime = PunchoutTime,
                WorkingHour = WorkingHour,
                LatePunchinReason = LatePunchinReason,
                PunchinOutsideLocationReason = PunchinOutsideLocationReason,
                EarlyPunchoutReason = EarlyPunchoutReason,
                PunchoutOutsideLocationReason = PunchoutOutsideLocationReason,
                Punchinlatlong = Punchinlatlong,
                Punchoutlatlong = Punchoutlatlong,
                PunchinLocImg = PunchinLocImg,
                PunchoutLocImg = PunchoutLocImg,
                PunchinLocName = PunchinLocName,
                PunchoutLocName = PunchoutLocName,
                BDetail = BDetail,
                UncompletedBreakId = UncompletedBreakId,
                TotalWorkHour = TotalWorkHour,
                IsUserApplyForLeave = obj_CheckUserLeaveForDay_Model.IsUserApplyForLeave,
                LeaveType = obj_CheckUserLeaveForDay_Model.LeaveType,
                LeaveTypeName = obj_CheckUserLeaveForDay_Model.LeaveTypeName,
                MasterLeaveType = obj_CheckUserLeaveForDay_Model.MasterLeaveType,
                MasterLeaveTypeName = obj_CheckUserLeaveForDay_Model.MasterLeaveTypeName

            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string PunchIn(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, bool PunchInType, int LocationId, string LatitudeLongitude, string OutsideReason, string LateReason)
        {
            var PD = new PunchInDetail();
            var ValidForPunchin = false;
            var LatePunchinType = false;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var dbAppVersion = (from av in context.VersionMasters
                                            select DeviceType == "PC" ? av.PCVersion : DeviceType == "Android" ? av.AndroidVersion : av.IOSVersion).FirstOrDefault();

                        if (AppVersion == (dbAppVersion == null ? "0" : dbAppVersion.ToString()))
                        {
                            var CheckPunchCount = (from p in context.PunchIns
                                                   where p.UserId == LD.UserId && p.PunchinTime.Date == DateTime.Now.Date
                                                   select p.UserId).Count();
                            if (CheckPunchCount == 0)
                            {

                                // Changes will be here for timings 
                                var CheckLatePunchin = (from dt in context.Users
                                                        join wtt in context.WeeklyTimings on dt.UserId equals wtt.ObjectId
                                                        where dt.UserId == LD.UserId && wtt.TimingFor == 3
                                                        && wtt.TimeFrom.TimeOfDay < DateTime.Now.TimeOfDay && wtt.Day == Convert.ToString(DateTime.Now.DayOfWeek) && wtt.Status == true
                                                        select dt).Count();
                                if (CheckLatePunchin == 0)
                                {
                                    ValidForPunchin = true;
                                }
                                else
                                {
                                    if (LateReason.Trim() != null && LateReason.Trim() != "")
                                    {
                                        ValidForPunchin = true;
                                        LatePunchinType = true;
                                    }
                                    else
                                    {
                                        ValidForPunchin = false;
                                    }
                                }

                                if (ValidForPunchin)
                                {
                                    //START code for system punchout
                                    context.PunchIns.Where(p => p.UserId == LD.UserId && p.PunchinTime.Date < DateTime.Today.Date && p.PunchoutTime == null)
                                    .ToList()
                                    .ForEach(a =>
                                    {
                                        var bs = (from b in context.Breaks where b.UserId == LD.UserId && b.BreakinTime.Date == a.PunchinTime.Date && b.BreakoutTime == null select b);
                                        //System Breakout
                                        if (bs.Count() > 0)
                                            context.Breaks.Where(b => b.BreakId == bs.FirstOrDefault().BreakId).ToList().ForEach(p => { p.BreakoutTime = p.BreakinTime; p.SystemBreakout = true; });

                                        var df = (from u in context.WeeklyTimings where u.ObjectId == LD.UserId && u.TimingFor == 3 && u.Day == Convert.ToString(DateTime.Now.DayOfWeek) select u).FirstOrDefault();
                                        var halfwork = new TimeSpan(df.WorkingHours.Ticks / 2);
                                        a.PunchoutTime = bs.Count() > 0 ? a.PunchinTime.Date + bs.FirstOrDefault().BreakinTime.TimeOfDay :
                                            df.TimeUpto.TimeOfDay > a.PunchinTime.TimeOfDay + halfwork ? a.PunchinTime + halfwork : a.PunchinTime; //  a.PunchinTime.Date + df.DefaultTimeTo.TimeOfDay;
                                        a.PunchoutType = PunchInType;
                                        if (PunchInType)
                                            a.POLocationId = Convert.ToInt32(LocationId);
                                        a.POLatitudeLongitude = LatitudeLongitude;
                                        a.SystemPunchout = true;



                                        var TempWorkHour = a.PunchoutTime.Value.TimeOfDay - a.PunchinTime.TimeOfDay;

                                        if (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == a.PunchinTime.Date && w.BreakoutTime != null).Count() > 0)
                                        {
                                            var TotalBreakTime = (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == a.PunchinTime.Date && w.BreakoutTime != null).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks)));
                                            TempWorkHour = new TimeSpan(PD.WorkingHour.Ticks - TotalBreakTime);
                                        }

                                        //if (TempWorkHour <= new TimeSpan(context.Users.Where(w => w.UserId == LD.UserId).FirstOrDefault().WorkHour.Ticks / 2))
                                        //    a.IsHalfDay = true;
                                    });
                                    //END code for system punchout

                                    //START code for punchin
                                    var PIDetail = new PunchIn();
                                    PIDetail.UserId = LD.UserId;
                                    PIDetail.PunchinTime = DateTime.Now;
                                    PIDetail.PunchinDeviceId = DeviceId;
                                    PIDetail.PunchinType = PunchInType;
                                    if (PunchInType)
                                        PIDetail.PILocationId = LocationId;
                                    PIDetail.PILatitudeLongitude = LatitudeLongitude;
                                    PIDetail.PunchinOutsideLocationReason = ((OutsideReason.Trim() != "") ? (OutsideReason.Trim()) : null);
                                    PIDetail.LatePunchin = LatePunchinType;
                                    PIDetail.LatePunchinReason = ((LateReason.Trim() != "") ? (LateReason.Trim()) : null);
                                    PIDetail.PunchinOnHoliday = (context.Holidays.Where(p => p.HolidayDate == DateTime.Today && p.CompanyId == CompanyId).Count()) > 0 ? true : false;
                                    PIDetail.PunchinOnWeekoff = GF.TodayIsWeekoff(context.UserWeekOffDays.Where(w => w.UserId == LD.UserId && w.EndDate == null).ToList());
                                    context.PunchIns.InsertOnSubmit(PIDetail);
                                    context.SubmitChanges();

                                    BAModel.BAModel.CalculatePrevPunchInOutTiming(LD.UserId, CompanyId);
                                    //END code for punchin

                                    ////START code for notification
                                    PD.PId = PIDetail.PId;
                                    PD.PunchinTime = PIDetail.PunchinTime.ToString("hh:mm:ss tt");
                                    string Name = (from u in context.Users where u.UserId == LD.UserId select u.FirstName + " " + u.LastName).FirstOrDefault();
                                    var DataList = new
                                    {
                                        Username = Name,
                                        PType = "Punchin",
                                        IsPunchin = true,
                                        latlong = PIDetail.PILatitudeLongitude,
                                        outsidereason = PIDetail.PunchinOutsideLocationReason,
                                        PTime = PIDetail.PunchinTime.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                                        Whours = "",
                                        MobileNo = LD.Mobile,
                                        PTypeDesc = PunchInType == true ? "is now available at " + (from l in context.Locations
                                                                                                    where l.LocationId == Convert.ToInt32(LocationId)
                                                                                                    select l.PlaceName).FirstOrDefault() : " has punchin from outside",
                                        LatePunchinReason = PIDetail.LatePunchin == true ? PIDetail.LatePunchinReason : ""
                                    };
                                    JavaScriptSerializer jSearializer = new JavaScriptSerializer();
                                    var Data = jSearializer.Serialize(DataList);

                                    var RegIdlist = (from u in context.UserAccesses
                                                     join ud in context.UserDevices on u.UserID equals ud.UserId
                                                     where u.UserAccessID == LD.UserId && ud.AccessToken != null
                                                     select ud.AccessToken).ToList();


                                    //var RegIdlist = (from u in context.UserAccesses
                                    //                 join ud in context.UserDevices on u.UserID equals ud.UserId
                                    //                 where u.UserAccessID == LD.UserId && ud.AccessToken != null
                                    //                 select ud.AccessToken).ToList();

                                    // var RegIdlist = new List<string>() { "APA91bFyZHk2j0165odjgEI-naPTrYhuAEr099R2h5R1xjs5MJj01r6C4CnMEfGMNotW-LXRo_cjy0uH0LQSA94BufNWIkCppmLIRVhwFJbJw3x0TNXfsw64fFcwnhojL2BUC0UrOZuu" };
                                    var RegIds = jSearializer.Serialize(RegIdlist);

                                    // RegIds = new JSONArray(Arrays.asList(RegIdlist)));

                                    try
                                    {
                                        if (DeviceType.ToLower() == "android")
                                        {
                                            GF.AndroidPush(RegIds, Data, "Punchinout");
                                        }
                                        if (DeviceType.ToLower() == "ios")
                                        {
                                            GF.IOSPush(RegIds, Data, "Punchinout");
                                        }

                                    }
                                    catch (Exception ie)
                                    {
                                        LD.ReturnType = "success";
                                        LD.ReturnResult = "Server Is Not Responding..." + ie.ToString();
                                        LD.FailureReason = "";
                                    }

                                    ////END code for notification
                                }
                                else
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "Please enter reason for late punchin";
                                    LD.FailureReason = "late";
                                }
                            }
                            else
                            {
                                var CheckPunchinFromOtherDevice = (from p in context.PunchIns
                                                                   where p.UserId == LD.UserId && p.PunchinTime.Date == DateTime.Now.Date
                                                                   select p.PunchoutTime == null ? true : false).FirstOrDefault();
                                if (CheckPunchinFromOtherDevice)
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "You already punchin";
                                    LD.FailureReason = "device";
                                }
                                else
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "Punchin is already exists";
                                    LD.FailureReason = "device";
                                }
                            }
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "Please update this app";
                            LD.FailureReason = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string parameters = "UserName =" + UserName + ",Password = " + Password + ",DeviceId =" + DeviceId + ",DeviceType =" + DeviceType + ",CompanyId =" + CompanyId + "AppVersion = " + AppVersion + ",PunchInType = " + PunchInType + ",LocationId = " + LocationId + ",LatitudeLongitude = " + LatitudeLongitude + ",OutsideReason =" + OutsideReason + ",LateReason = " + LateReason;
                GF.SendEmail(e.ToString(), "Punchin", parameters);
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                Pid = PD.PId,
                PunchinTime = PD.PunchinTime,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string PunchOut(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, bool PunchInType, int LocationId, string LatitudeLongitude, string OutsideReason, string EarlyReason, string WorkHourReason)
        {
            var PD = new PunchInDetail();
            var ValidForPunchin = false;
            var EarlyPunchoutType = false;
            var TotalWorkHour = "";

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var dbAppVersion = (from av in context.VersionMasters
                                            select DeviceType == "PC" ? av.PCVersion : DeviceType == "Android" ? av.AndroidVersion : av.IOSVersion).FirstOrDefault();

                        if (AppVersion == (dbAppVersion == null ? "0" : dbAppVersion.ToString()))
                        {
                            if ((from p in context.PunchIns where p.UserId == LD.UserId && p.PunchinTime.Date == DateTime.Today.Date select p.PId).Count() > 0)
                            {
                                var CheckPunchoutFromOtherDevice = (from p in context.PunchIns
                                                                    where p.UserId == LD.UserId && p.PunchinTime.Date == DateTime.Now.Date
                                                                    select p.PunchoutTime == null ? false : true).FirstOrDefault();
                                if (!CheckPunchoutFromOtherDevice)
                                {
                                    var CheckEarlyPunchout = (from dt in context.Users
                                                              join wtt in context.WeeklyTimings on dt.UserId equals wtt.ObjectId
                                                              where dt.UserId == LD.UserId && wtt.Day == Convert.ToString(DateTime.Now.DayOfWeek) && wtt.TimeUpto.TimeOfDay > DateTime.Now.TimeOfDay && wtt.TimingFor == 3
                                                              select dt).Count();
                                    if (CheckEarlyPunchout == 0)
                                    {
                                        //var TotalBreakTime = (from b in context.Breaks where b.UserId ==LD.UserId && b.bre)
                                        //var CheckWorkHourComplete = (from p in context.PunchIns where p.);
                                        ValidForPunchin = true;
                                    }
                                    else
                                    {
                                        if (EarlyReason.Trim() != null && EarlyReason.Trim() != "")
                                        {
                                            ValidForPunchin = true;
                                            EarlyPunchoutType = true;
                                        }
                                        else
                                        {
                                            CheckUserLeaveForDay_Model obj_CheckUserLeaveForDay_Model = BAModel.BAModel.CheckUserLeaveForDay(DateTime.Now, LD.UserId);
                                            if (obj_CheckUserLeaveForDay_Model.IsUserApplyForLeave == false)
                                            {
                                                ValidForPunchin = false;
                                            }
                                            else
                                            {
                                                ValidForPunchin = true;
                                                EarlyPunchoutType = true;
                                            }
                                        }
                                    }

                                    if (ValidForPunchin)
                                    {
                                        long TotalBreakTimeTicks = 0;
                                        if (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == DateTime.Today.Date).Count() > 0)
                                        {
                                            TotalBreakTimeTicks = (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == DateTime.Today.Date).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks)));
                                        }

                                        var UserPunchinTime = (context.PunchIns.Where(w => w.UserId == LD.UserId && w.PunchinTime.Date == DateTime.Today.Date).Select(s => s.PunchinTime).FirstOrDefault());
                                        var UserWorkHour = (context.WeeklyTimings.Where(w => w.ObjectId == LD.UserId && w.TimingFor == 3 && w.Day == Convert.ToString(DateTime.Now.DayOfWeek)).Select(s => s.WorkingHours).FirstOrDefault());
                                        var WorkHourRemaining = new TimeSpan(UserWorkHour.Ticks - ((DateTime.Now.TimeOfDay - UserPunchinTime.TimeOfDay).Ticks - TotalBreakTimeTicks));

                                        if (UserWorkHour.Ticks < ((DateTime.Now.TimeOfDay - UserPunchinTime.TimeOfDay).Ticks - TotalBreakTimeTicks))
                                        {
                                            ValidForPunchin = true;
                                        }
                                        else
                                        {
                                            if (WorkHourReason.Trim() != null && WorkHourReason.Trim() != "")
                                            {
                                                ValidForPunchin = true;
                                            }
                                            else
                                            {
                                                ValidForPunchin = false;
                                            }
                                        }

                                        if (ValidForPunchin)
                                        {
                                            var PODetail = (from p in context.PunchIns
                                                            where p.UserId == LD.UserId && p.PunchinTime.Date == DateTime.Today.Date
                                                            select p).FirstOrDefault();

                                            if (PODetail.PunchinTime.Date == DateTime.Now.Date)
                                            {
                                                PODetail.PunchoutTime = DateTime.Now;
                                            }
                                            else
                                            {
                                                PODetail.PunchoutTime = PODetail.PunchinTime.Date + new TimeSpan(14, 0, 0);
                                            }

                                            PODetail.PunchoutDeviceId = DeviceId;
                                            PODetail.PunchoutType = PunchInType;
                                            if (PunchInType)
                                                PODetail.POLocationId = LocationId;
                                            PODetail.POLatitudeLongitude = LatitudeLongitude;
                                            PODetail.PunchoutOutsideLocationReason = ((OutsideReason.Trim() != "") ? (OutsideReason.Trim()) : null);
                                            PODetail.EarlyPunchout = EarlyPunchoutType;
                                            PODetail.EarlyPunchoutReason = ((EarlyReason.Trim() != "") ? (EarlyReason.Trim()) : null);
                                            PODetail.WorkHourReason = ((WorkHourReason.Trim() != "") ? (WorkHourReason.Trim()) : null);
                                            PD.WorkingHour = PODetail.PunchoutTime.Value.TimeOfDay - PODetail.PunchinTime.TimeOfDay;
                                            PD.PunchoutTime = PODetail.PunchoutTime.Value.ToString("hh:mm:ss tt");
                                            var TempWorkHour = PD.WorkingHour;

                                            if (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == PODetail.PunchinTime.Date && w.BreakoutTime != null).Count() > 0)
                                            {
                                                var TotalBreakTime = (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == PODetail.PunchinTime.Date && w.BreakoutTime != null).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks)));
                                                var total = new TimeSpan(PD.WorkingHour.Ticks - TotalBreakTime);
                                                TempWorkHour = total;
                                                TotalWorkHour = "Total Working Hour - " + total.Hours + " : " + total.Minutes + " : " + total.Seconds;
                                            }
                                            else
                                            {
                                                TotalWorkHour = "Total Working Hour - " + PD.WorkingHour.Hours + ":" + PD.WorkingHour.Minutes + ":" + PD.WorkingHour.Seconds;
                                            }

                                            WeeklyTiming obj_weeklytimming =  context.WeeklyTimings.Where(w => w.ObjectId == LD.UserId && w.TimingFor == 3 && w.Day == Convert.ToString(DateTime.Now.DayOfWeek)).FirstOrDefault();

                                            if (obj_weeklytimming != null)
                                            {
                                                if (TempWorkHour < new TimeSpan(context.WeeklyTimings.Where(w => w.ObjectId == LD.UserId && w.TimingFor == 3 && w.Day == Convert.ToString(DateTime.Now.DayOfWeek)).FirstOrDefault().WorkingHours.Ticks / 2))
                                                    PODetail.IsHalfDay = true;
                                            }

                                            PODetail.WorkingHour = Convert.ToString(TempWorkHour).Split('.')[0];
                                            PODetail.ExtraWorkingHour = Convert.ToString(new TimeSpan(TempWorkHour.Ticks - UserWorkHour.Ticks)).Split('.')[0];

                                            context.SubmitChanges();

                                            string Name = (from u in context.Users where u.UserId == LD.UserId select u.FirstName + " " + u.LastName).FirstOrDefault();

                                            var DataList = new
                                            {
                                                Username = Name,
                                                PType = "Punchout",
                                                IsPunchin = false,
                                                latlong = PODetail.POLatitudeLongitude,
                                                outsidereason = PODetail.PunchoutOutsideLocationReason,
                                                PTime = PODetail.PunchoutTime.Value.ToString("dd-MMM-yyyy hh:mm tt"),
                                                Whours = TotalWorkHour,
                                                MobileNo = LD.Mobile,
                                                PTypeDesc = PunchInType == true ? "is now leaving for the day from " + (from l in context.Locations
                                                                                                                        where l.LocationId == Convert.ToInt32(LocationId)
                                                                                                                        select l.PlaceName).FirstOrDefault() : " is now leaving for the day from outside",
                                                LatePunchinReason = PODetail.EarlyPunchout == true ? PODetail.WorkHourReason != "" ? PODetail.EarlyPunchoutReason + "\n" + PODetail.WorkHourReason : PODetail.EarlyPunchoutReason : ""
                                            };
                                            JavaScriptSerializer jSearializer = new JavaScriptSerializer();
                                            var Data = jSearializer.Serialize(DataList);

                                            var RegIdlist = (from u in context.UserAccesses
                                                             join ud in context.UserDevices on u.UserID equals ud.UserId
                                                             where u.UserAccessID == LD.UserId && ud.AccessToken != null
                                                             select ud.AccessToken).ToList();

                                            //var RegIdlist = new List<string>() { "APA91bFyZHk2j0165odjgEI-naPTrYhuAEr099R2h5R1xjs5MJj01r6C4CnMEfGMNotW-LXRo_cjy0uH0LQSA94BufNWIkCppmLIRVhwFJbJw3x0TNXfsw64fFcwnhojL2BUC0UrOZuu" };
                                            var RegIds = jSearializer.Serialize(RegIdlist);

                                            try
                                            {
                                                GF.AndroidPush(RegIds, Data, "Punchinout");
                                            }
                                            catch (Exception ie)
                                            {
                                                LD.ReturnType = "success";
                                                LD.ReturnResult = "Notification Server Is Not Responding..." + ie.ToString();
                                                LD.FailureReason = "";
                                            }
                                        }
                                        else
                                        {
                                            LD.ReturnType = "failure";
                                            LD.ReturnResult = "Your Work Hour is not Completed. " +
                                                                (WorkHourRemaining.Hours != 0 ? WorkHourRemaining.Hours + " Hours " : "") +
                                                                (WorkHourRemaining.Minutes != 0 ? WorkHourRemaining.Minutes + " Minutes " : "") +
                                                                "are remaining";
                                            LD.FailureReason = "workhour";
                                        }
                                    }
                                    else
                                    {
                                        LD.ReturnType = "failure";
                                        LD.ReturnResult = "Please enter reason for early punchout";
                                        LD.FailureReason = "late";
                                    }
                                }
                                else
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "You already punchout from another Device";
                                    LD.FailureReason = "device";
                                }
                            }
                            else
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "User is not punched in";
                                LD.FailureReason = "";
                            }
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "Please update this app";
                            LD.FailureReason = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
                GF.SendEmail(e.ToString(), "Punchout", "UserName = " + UserName + ", Password = " + Password + ", DeviceId = " + DeviceId + ", DeviceType = " + DeviceType + ", CompanyId = " + CompanyId + ", AppVersion = " + AppVersion + ", PunchInType = " + PunchInType + ", LocationId = " + LocationId + ", LatitudeLongitude = " + LatitudeLongitude + ", OutsideReason = " + OutsideReason + ", EarlyReason = " + EarlyReason + ", WorkHourReason = " + WorkHourReason);
            }

            var FinalResult = new
            {
                PunchoutTime = PD.PunchoutTime,
                WorkingHour = PD.WorkingHour.Hours + ":" + PD.WorkingHour.Minutes + ":" + PD.WorkingHour.Seconds,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                TotalWorkHour = TotalWorkHour,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string PunchInListDateWise(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedCompanyId, DateTime SelectedDate)
        {
            List<PunchInListDetail> PDetail = new List<PunchInListDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        PDetail = (from u in context.Users
                                   join temp in context.PunchIns on new { u.UserId, SelectedDate.Date } equals new { temp.UserId, temp.PunchinTime.Date } into tempjoin
                                   from p in tempjoin.DefaultIfEmpty()
                                   where u.Status == 1 && u.CompanyID == SelectedCompanyId
                                   select new PunchInListDetail()
                                   {
                                       UserName = u.UserName,
                                       PunchInTime = p.PunchinTime != null ? Convert.ToString(p.PunchinTime) : "N.A",
                                       PunchOutTime = p.PunchoutTime != null ? Convert.ToString(p.PunchoutTime.Value) : "N.A",
                                       WorkingHour = p.PunchoutTime != null ?
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Hours + ":" +
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Minutes + ":" +
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Seconds
                                                   : "N.A",

                                       PunchinType = (p.PunchinType != null ? p.PunchinType == true ? "Inside" : "Outside" : "N.A") +
                                                    " - " +
                                                    (p.PunchoutType != null ? p.PunchoutType == true ? "Inside" : "Outside" : "N.A"),
                                       PunchinAddress = (p.PunchinType != null ? p.PunchinType == true ? (from l in context.Locations where l.LocationId == p.PILocationId select l.LocationName).FirstOrDefault() : "N.A" : "N.A") +
                                                    " - " +
                                                    (p.PunchoutType != null ? p.PunchoutType == true ? (from l in context.Locations where l.LocationId == p.POLocationId select l.LocationName).FirstOrDefault() : "N.A" : "N.A")

                                       //PunchinType = (p.PunchInType != null ? p.PunchInType == true ? (from l in context.Locations where l.Id == p.PILocationId select l.Description).FirstOrDefault() : p.PunchInType : "N.A") +
                                       //             " - " +
                                       //             (p.PunchOutType != null ? p.PunchOutType == "Location" ? (from l in context.Locations where l.Id == p.POLocationId select l.Description).FirstOrDefault() : p.PunchOutType : "N.A")


                                   }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                PDetail = PDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        #endregion

        #region ~Callinreg

        [WebMethod]
        public string CheckCallinStatus(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            var IsCallinExist = false;
            var IsCallinComplete = false;
            var UncompletedBreakId = 0;
            var CallinTime = "";
            var CalloutTime = "";
            var WorkingHour = "";
            var TotalWorkHour = "";

            List<BreakDetail> BDetail = new List<BreakDetail>();
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var p = (from pn in context.CallIns where pn.UserId == LD.UserId && pn.CallinTime.Date == DateTime.Today.Date && pn.CalloutTime == null select pn).FirstOrDefault();

                        if (p != null)
                        {
                            CallinTime = p.CallinTime.ToString("hh:mm:ss tt");
                            CalloutTime = p.CalloutTime != null ? p.CalloutTime.Value.ToString("hh:mm:ss tt") : "";
                            WorkingHour = p.CalloutTime != null ?
                                                       (p.CalloutTime.Value.TimeOfDay - p.CallinTime.TimeOfDay).Hours + ":" +
                                                       (p.CalloutTime.Value.TimeOfDay - p.CallinTime.TimeOfDay).Minutes + ":" +
                                                       (p.CalloutTime.Value.TimeOfDay - p.CallinTime.TimeOfDay).Seconds : "";
                            IsCallinExist = true;
                            IsCallinComplete = p.CalloutTime == null ? false : true;

                            BDetail = (from b in context.Breaks
                                       where b.UserId == LD.UserId && b.BreakinTime.Date == p.CallinTime.Date
                                       select new
                                       {
                                           BreakId = b.BreakId,
                                           BreakinTime = b.BreakinTime,
                                           BreakoutTime = b.BreakoutTime,
                                       }
                                       ).AsEnumerable()
                                       .Select(b => new BreakDetail()
                                       {
                                           BreakId = b.BreakId,
                                           BreakinTime = b.BreakinTime.ToString("hh:mm:ss tt"),
                                           BreakoutTime = b.BreakoutTime != null ? b.BreakoutTime.Value.ToString("hh:mm:ss tt") : "",
                                           BreakingHour = b.BreakoutTime != null ?
                                                           (b.BreakoutTime.Value.TimeOfDay - b.BreakinTime.TimeOfDay).Hours + ":" +
                                                           (b.BreakoutTime.Value.TimeOfDay - b.BreakinTime.TimeOfDay).Minutes + ":" +
                                                           (b.BreakoutTime.Value.TimeOfDay - b.BreakinTime.TimeOfDay).Seconds
                                                   : "",
                                       }).OrderBy(pr => pr.BreakId).ToList();

                            UncompletedBreakId = (from b in context.Breaks
                                                  where b.UserId == LD.UserId && b.BreakinTime.Date == p.CallinTime.Date && b.BreakoutTime == null
                                                  select b.BreakId).FirstOrDefault();
                            if (IsCallinComplete)
                            {
                                if (BDetail.Count() > 0)
                                {
                                    var TotalOfficeTime = p.CalloutTime.Value.TimeOfDay - p.CallinTime.TimeOfDay;
                                    var TotalWorkTime = (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == p.CallinTime.Date).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks)));
                                    var total = new TimeSpan(TotalOfficeTime.Ticks - TotalWorkTime);
                                    TotalWorkHour = "Total Working Hour - " + total.Hours + " : " + total.Minutes + " : " + total.Seconds;
                                }
                                else
                                {
                                    TotalWorkHour = WorkingHour;
                                }
                            }

                            //  IsBreakComplete = TempResponse == null ? true : false;
                        }
                        else
                        {
                            var q = (from pn in context.CallIns where pn.UserId == LD.UserId && pn.CallinTime.Date == DateTime.Today.Date select pn).FirstOrDefault();

                            if (q != null)
                            {
                                IsCallinExist = true;
                                IsCallinComplete = true;
                            }


                        }
                    }
                }
            }
            catch (Exception ee)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                IsCallinExist = IsCallinExist,
                IsCallinComplete = IsCallinComplete,
                CallinTime = CallinTime,
                CalloutTime = CalloutTime,
                WorkingHour = WorkingHour,
                BDetail = BDetail,
                UncompletedBreakId = UncompletedBreakId,
                TotalWorkHour = TotalWorkHour,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string CallIn(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, bool CallInType, int LocationId, string LatitudeLongitude, string CallTitle)
        {
            var PD = new CallInDetail();
            var ValidForCallin = true;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var dbAppVersion = (from av in context.VersionMasters
                                            select DeviceType == "PC" ? av.PCVersion : DeviceType == "Android" ? av.AndroidVersion : av.IOSVersion).FirstOrDefault();

                        if (AppVersion == (dbAppVersion == null ? "0" : dbAppVersion.ToString()))
                        {
                            var CheckCallCount = (from p in context.CallIns
                                                  where p.UserId == LD.UserId && p.CallinTime.Date == DateTime.Now.Date && p.CalloutTime == null
                                                  select p.UserId).Count();
                            if (CheckCallCount == 0)
                            {
                                // Changes will be here for timings 
                                if (ValidForCallin)
                                {
                                    //START code for system Callout
                                    context.CallIns.Where(p => p.UserId == LD.UserId && p.CallinTime.Date < DateTime.Today.Date && p.CalloutTime == null)
                                    .ToList()
                                    .ForEach(a =>
                                    {
                                        var bs = (from b in context.Breaks where b.UserId == LD.UserId && b.BreakinTime.Date == a.CallinTime.Date && b.BreakoutTime == null select b);
                                        //System Breakout
                                        if (bs.Count() > 0)
                                            context.Breaks.Where(b => b.BreakId == bs.FirstOrDefault().BreakId).ToList().ForEach(p => { p.BreakoutTime = p.BreakinTime; p.SystemBreakout = true; });

                                        var df = (from u in context.WeeklyTimings where u.ObjectId == LD.UserId && u.TimingFor == 3 && u.Day == Convert.ToString(DateTime.Now.DayOfWeek) select u).FirstOrDefault();
                                        var halfwork = new TimeSpan(df.WorkingHours.Ticks / 2);
                                        a.CalloutTime = bs.Count() > 0 ? a.CallinTime.Date + bs.FirstOrDefault().BreakinTime.TimeOfDay :
                                            df.TimeUpto.TimeOfDay > a.CallinTime.TimeOfDay + halfwork ? a.CallinTime + halfwork : a.CallinTime; //  a.CallinTime.Date + df.DefaultTimeTo.TimeOfDay;
                                        a.CalloutType = CallInType;
                                        if (CallInType)
                                            a.COLocationId = Convert.ToInt32(LocationId);
                                        a.COLatitudeLongitude = LatitudeLongitude;
                                        a.SystemCallout = true;


                                        var TempWorkHour = a.CalloutTime.Value.TimeOfDay - a.CallinTime.TimeOfDay;

                                        if (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == a.CallinTime.Date && w.BreakoutTime != null).Count() > 0)
                                        {
                                            var TotalBreakTime = (context.Breaks.Where(w => w.UserId == LD.UserId && w.BreakinTime.Date == a.CallinTime.Date && w.BreakoutTime != null).Sum(s => (s.BreakoutTime.Value.TimeOfDay.Ticks - s.BreakinTime.TimeOfDay.Ticks)));
                                            TempWorkHour = new TimeSpan(PD.WorkingHour.Ticks - TotalBreakTime);
                                        }

                                        //if (TempWorkHour <= new TimeSpan(context.Users.Where(w => w.UserId == LD.UserId).FirstOrDefault().WorkHour.Ticks / 2))
                                        //    a.IsHalfDay = true;
                                    });
                                    //END code for system Callout
                                    //START code for Callin
                                    var PIDetail = new CallIn();
                                    PIDetail.UserId = LD.UserId;
                                    PIDetail.CallinTime = DateTime.Now;
                                    PIDetail.CallinDeviceId = DeviceId;
                                    PIDetail.CallinType = CallInType;
                                    PIDetail.CallTitle = CallTitle;
                                    if (CallInType)
                                        PIDetail.CILocationId = LocationId;
                                    PIDetail.CILatitudeLongitude = LatitudeLongitude;
                                    context.CallIns.InsertOnSubmit(PIDetail);
                                    context.SubmitChanges();
                                    //END code for Callin

                                    ////START code for notification
                                    PD.PId = PIDetail.CId;
                                    PD.CallinTime = PIDetail.CallinTime.ToString("hh:mm:ss tt");

                                    var DataList = new
                                    {
                                        Username = UserName,
                                        PType = "Callin",
                                        PTime = PIDetail.CallinTime.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                                        Whours = "",
                                        MobileNo = LD.Mobile,
                                        PTypeDesc = CallInType == true ? " at " + (from l in context.Locations
                                                                                   where l.LocationId == Convert.ToInt32(LocationId)
                                                                                   select l.PlaceName).FirstOrDefault() : " at outside",
                                        Reason = ""
                                    };
                                    JavaScriptSerializer jSearializer = new JavaScriptSerializer();
                                    var Data = jSearializer.Serialize(DataList);

                                    //var RegIdlist = (from u in context.Users
                                    //                 join ud in context.UserDevices on u.UserId equals ud.UserId
                                    //                 where u.RoleId == 1 && ud.AccessToken != null
                                    ////                 select ud.AccessToken).ToList();
                                    var RegIdlist = new List<string>() { "APA91bFyZHk2j0165odjgEI-naPTrYhuAEr099R2h5R1xjs5MJj01r6C4CnMEfGMNotW-LXRo_cjy0uH0LQSA94BufNWIkCppmLIRVhwFJbJw3x0TNXfsw64fFcwnhojL2BUC0UrOZuu" };
                                    var RegIds = jSearializer.Serialize(RegIdlist);

                                    try
                                    {
                                        GF.AndroidPush(RegIds, Data, "Callinout");
                                    }
                                    catch (Exception ie)
                                    {
                                        LD.ReturnType = "success";
                                        LD.ReturnResult = "Notification Server Is Not Responding...";
                                        LD.FailureReason = "";
                                    }

                                    ////END code for notification
                                }
                                else
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "Please enter reason for late Callin";
                                    LD.FailureReason = "late";

                                }
                            }
                            else
                            {

                                LD.ReturnType = "failure";
                                LD.ReturnResult = "You are already call In";
                                LD.FailureReason = "device";

                            }
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "Please update this app";
                            LD.FailureReason = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                Callid = PD.PId,
                CallinTime = PD.CallinTime,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string CallOut(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, bool CallInType, int LocationId, string LatitudeLongitude, string Description)
        {
            var PD = new CallInDetail();
            var ValidForCallin = true;
            var EarlyCalloutType = false;
            var TotalWorkHour = "";

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var dbAppVersion = (from av in context.VersionMasters
                                            select DeviceType == "PC" ? av.PCVersion : DeviceType == "Android" ? av.AndroidVersion : av.IOSVersion).FirstOrDefault();

                        if (AppVersion == (dbAppVersion == null ? "0" : dbAppVersion.ToString()))
                        {
                            if ((from p in context.CallIns where p.UserId == LD.UserId && p.CallinTime.Date == DateTime.Today.Date select p.CId).Count() > 0)
                            {
                                var CheckCalloutFromOtherDevice = (from p in context.CallIns
                                                                   where p.UserId == LD.UserId && p.CallinTime.Date == DateTime.Now.Date
                                                                   select p.CalloutTime == null ? false : true).FirstOrDefault();
                                if (!CheckCalloutFromOtherDevice)
                                {
                                    CheckCalloutFromOtherDevice = (from p in context.CallIns
                                                                   where p.UserId == LD.UserId && p.CallinTime.Date == DateTime.Now.Date
                                                                   select p.CalloutTime).Count() == 1 ? true : false;
                                }


                                if (CheckCalloutFromOtherDevice)
                                {

                                    if (ValidForCallin)
                                    {
                                        var PODetail = (from p in context.CallIns
                                                        where p.UserId == LD.UserId && p.CallinTime.Date == DateTime.Today.Date && p.CalloutTime == null
                                                        select p).FirstOrDefault();


                                        if (PODetail != null)
                                        {
                                            if (PODetail.CallinTime.Date == DateTime.Now.Date)
                                            {
                                                PODetail.CalloutTime = DateTime.Now;
                                            }
                                            else
                                            {
                                                PODetail.CalloutTime = PODetail.CallinTime.Date + new TimeSpan(14, 0, 0);
                                            }

                                            PODetail.CalloutDeviceId = DeviceId;
                                            PODetail.CalloutType = CallInType;
                                            if (CallInType)
                                                PODetail.COLocationId = LocationId;
                                            PODetail.COLatitudeLongitude = LatitudeLongitude;
                                            PODetail.CallConclusion = Description;


                                            PD.WorkingHour = PODetail.CalloutTime.Value.TimeOfDay - PODetail.CallinTime.TimeOfDay;
                                            PD.CalloutTime = PODetail.CalloutTime.Value.ToString("hh:mm:ss tt");
                                            var TempWorkHour = PD.WorkingHour;
                                            PODetail.CallTime = Convert.ToString(PD.WorkingHour).Split('.')[0];

                                            context.SubmitChanges();

                                            var DataList = new
                                            {
                                                Username = UserName,
                                                PType = "Callout",
                                                PTime = PODetail.CalloutTime.Value.ToString("dd-MMM-yyyy hh:mm tt"),
                                                Whours = TotalWorkHour,
                                                MobileNo = LD.Mobile,
                                                PTypeDesc = CallInType == true ? " at " + (from l in context.Locations
                                                                                           where l.LocationId == Convert.ToInt32(LocationId)
                                                                                           select l.PlaceName).FirstOrDefault() : " at outside",
                                                Reason = ""
                                            };
                                            JavaScriptSerializer jSearializer = new JavaScriptSerializer();
                                            var Data = jSearializer.Serialize(DataList);

                                            var RegIdlist = (from u in context.Users
                                                             join ud in context.UserDevices on u.UserId equals ud.UserId
                                                             where u.RoleId == 1 && ud.AccessToken != null
                                                             select ud.AccessToken).ToList();

                                            //   var RegIdlist = new List<string>() { "APA91bFyZHk2j0165odjgEI-naPTrYhuAEr099R2h5R1xjs5MJj01r6C4CnMEfGMNotW-LXRo_cjy0uH0LQSA94BufNWIkCppmLIRVhwFJbJw3x0TNXfsw64fFcwnhojL2BUC0UrOZuu" };
                                            var RegIds = jSearializer.Serialize(RegIdlist);

                                            try
                                            {
                                                GF.AndroidPush(RegIds, Data, "Callinout");
                                            }
                                            catch (Exception ie)
                                            {
                                                LD.ReturnType = "success";
                                                LD.ReturnResult = "Notification Server Is Not Responding...";
                                                LD.FailureReason = "";
                                            }
                                        }
                                        else
                                        {
                                            LD.ReturnType = "failure";
                                            LD.ReturnResult = "You already Callout.";
                                            LD.FailureReason = "device";
                                        }
                                    }
                                }
                                else
                                {
                                    LD.ReturnType = "failure";
                                    LD.ReturnResult = "You already Callout.";
                                    LD.FailureReason = "device";
                                }
                            }
                            else
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "User is not Called in";
                                LD.FailureReason = "";
                            }
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "Please update this app";
                            LD.FailureReason = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                CalloutTime = PD.CalloutTime,
                CallHour = PD.WorkingHour.Hours + ":" + PD.WorkingHour.Minutes + ":" + PD.WorkingHour.Seconds,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                TotalWorkHour = TotalWorkHour,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string CallListDateWise(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedCompanyId, string SelectedDate, int SelectedUserID)
        {

            //DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //DateTime date = start.AddMilliseconds(SelectedDate).ToLocalTime();

            DateTime date = DateTime.Parse(SelectedDate);

            List<CallInListDetail> CDetail = new List<CallInListDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        CDetail = (from u in context.Users
                                   join p in context.CallIns on u.UserId equals p.UserId
                                   where u.Status == 1 && u.CompanyID == SelectedCompanyId && u.UserId == SelectedUserID && p.CallinTime.Date == date.Date
                                   orderby p.CId descending
                                   select new CallInListDetail()
                                   {
                                       UserName = u.UserName,
                                       CallinDescription = p.CallConclusion,
                                       CallinTitle = p.CallTitle,
                                       CallInLatelong = p.CILatitudeLongitude,
                                       CallOutLatelong = p.COLatitudeLongitude,
                                       CallInTime = p.CallinTime != null ?
                                       (p.CallinTime).Date + " " +
                                       (p.CallinTime.TimeOfDay).Hours + ":" +
                                                           (p.CallinTime.TimeOfDay).Minutes + ":" +
                                                           (p.CallinTime.TimeOfDay).Seconds
                                                   : "N.A",
                                       CallOutTime = p.CallinTime != null ?
                                       (p.CalloutTime.Value).Date + " " +
                                       (p.CalloutTime.Value.TimeOfDay).Hours + ":" +
                                                           (p.CalloutTime.Value.TimeOfDay).Minutes + ":" +
                                                           (p.CalloutTime.Value.TimeOfDay).Seconds
                                                   : "N.A",
                                       WorkingHour = p.CalloutTime != null ?
                                                           (p.CalloutTime.Value.TimeOfDay - p.CallinTime.TimeOfDay).Hours + ":" +
                                                           (p.CalloutTime.Value.TimeOfDay - p.CallinTime.TimeOfDay).Minutes + ":" +
                                                           (p.CalloutTime.Value.TimeOfDay - p.CallinTime.TimeOfDay).Seconds
                                                   : "N.A",

                                       CallinType = (p.CallinType != null ? p.CallinType == true ? "Inside" : "Outside" : "N.A") +
                                                    " - " +
                                                    (p.CalloutType != null ? p.CalloutType == true ? "Inside" : "Outside" : "N.A"),
                                       CallinAddress = (p.CallinType != null ? p.CallinType == true ? (from l in context.Locations where l.LocationId == p.CILocationId select l.PlaceName).FirstOrDefault() : "N.A" : "N.A") +
                                                    " - " +
                                                    (p.CalloutType != null ? p.CalloutType == true ? (from l in context.Locations where l.LocationId == p.COLocationId select l.PlaceName).FirstOrDefault() : "N.A" : "N.A"),


                                   }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                PDetail = CDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        #endregion

        #region ~UserAccessreg

        [WebMethod]
        public string ListGroupForAccess(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int GroupID)
        {
            List<GroupDetail> GDetail = new List<GroupDetail>();
            List<GroupDetail> GRDetail = new List<GroupDetail>();
            List<GroupDetail> GADetail = new List<GroupDetail>();
            int FirstGp = 0;
            string result = "";
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success" && GroupID != 0)
                {
                    using (var context = new ELDBDataContext())
                    {
                        FirstGp = (from u in context.GroupMasters where u.CompanyID == CompanyId orderby u.ID ascending select u.ID).FirstOrDefault();
                        GDetail = (from g in context.GroupMasters
                                   where g.CompanyID == CompanyId && g.ID != GroupID && g.Status == true && g.ID != FirstGp
                                   select new GroupDetail
                                   {
                                       GroupID = g.ID,
                                       GroupName = g.GroupName
                                   }).ToList();
                        GADetail = (from g in context.GroupMasters
                                    join ga in context.GroupAccesses on g.ID equals ga.GroupAccessID
                                    where ga.GroupID == GroupID && g.Status == true
                                    select new GroupDetail
                                    {
                                        GroupID = g.ID,
                                        GroupName = g.GroupName
                                    }).ToList();

                        var aaa = (from g in context.GroupMasters
                                   join ga in context.GroupAccesses on g.ID equals ga.GroupAccessID
                                   where ga.GroupID == GroupID
                                   select g.ID).ToList();

                        if (GADetail.Count != 0)
                        {
                            result = String.Join(",", aaa);

                            foreach (var item in GDetail)
                            {
                                if (!result.Contains(Convert.ToString(item.GroupID)))
                                {
                                    GroupDetail gd = new GroupDetail();
                                    gd.GroupID = item.GroupID;
                                    gd.GroupName = item.GroupName;
                                    GRDetail.Add(gd);

                                }
                            }
                        }
                        else
                        {
                            GRDetail = GDetail;
                        }







                        //GADetail = (from ga in context.GroupAccesses
                        //            join u in context.GroupMasters on ga.GroupID equals u.ID
                        //            where u.CompanyID == CompanyId && ga.GroupID==GroupID
                        //            select new GroupDetail
                        //            {
                        //                GroupID = u.ID,
                        //                GroupName = u.GroupName
                        //            }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                GRDetail = GRDetail,
                GADetail = GADetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddGroupAccessID(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int GroupID, string GroupAccessIDs)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {

                        //var GroupAccessID = (from u in context.GroupMasters where u.CompanyID == CompanyId orderby u.ID ascending select u.ID).FirstOrDefault();


                        //if (GroupAccessID != null && GroupAccessID != GroupID)
                        //{
                        //    GroupAccessIDs = GroupAccessIDs + "," + GroupAccessID;
                        //}


                        DeleteUserAccessAutomatically(GroupID);
                        context.GroupAccesses.DeleteAllOnSubmit(context.GroupAccesses.Where(c => c.GroupID == GroupID));
                        context.SubmitChanges();

                        if (GroupAccessIDs.Length > 0)
                        {
                            string[] intarr = GroupAccessIDs.Split(',');



                            foreach (var item in intarr)
                            {
                                if (item != "")
                                {
                                    var GDetail = new GroupAccess();
                                    GDetail.GroupID = GroupID;
                                    GDetail.GroupAccessID = (long?)Convert.ToDouble(item);
                                    context.GroupAccesses.InsertOnSubmit(GDetail);
                                    context.SubmitChanges();
                                }

                            }


                            AllUserAccessAutomatically(GroupID);
                        }


                        LD.ReturnResult = "Group Access is  Updated sucessfully.";


                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        public string AllUserAccessAutomatically(int GroupID)
        {
            using (var context = new ELDBDataContext())
            {
                var aTeamDetail = (from u in context.GroupAccesses where u.GroupID == GroupID select u.GroupAccessID).ToList();



                foreach (var item in aTeamDetail)
                {
                    var DUserID = (from u in context.Users where u.GroupID == GroupID select u.UserId).ToList();

                    var DAccUserID = (from u in context.Users where u.GroupID == item select u.UserId).ToList();




                    foreach (var item2 in DUserID)
                    {
                        foreach (var item3 in DAccUserID)
                        {
                            var Ua = new UserAccess();
                            Ua.UserID = item2;
                            Ua.UserAccessID = item3;
                            context.UserAccesses.InsertOnSubmit(Ua);
                            context.SubmitChanges();
                        }
                    }
                }
            }

            return "";
        }

        public string DeleteUserAccessAutomatically(int GroupID)
        {
            using (var context = new ELDBDataContext())
            {
                var aTeamDetail = (from u in context.GroupAccesses where u.GroupID == GroupID select u.GroupAccessID).ToList();

                foreach (var item in aTeamDetail)
                {
                    var DUserID = (from u in context.Users where u.GroupID == GroupID select u.UserId).ToList();

                    var DAccUserID = (from u in context.Users where u.GroupID == item select u.UserId).ToList();




                    foreach (var item2 in DUserID)
                    {
                        foreach (var item3 in DAccUserID)
                        {
                            context.UserAccesses.DeleteAllOnSubmit(context.UserAccesses.Where(c => c.UserAccessID == item3 && c.UserID == item2));
                        }
                    }
                }
            }

            return "";
        }
        #endregion

        #region ~Userreg

        [WebMethod]
        public string AddUser(string UserName, string Password, string NewUserName, string NewPassword, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UserId, int GroupID, int RoleId, int TopId, string FirstName, string LastName, string MobileNoCmp, int Status, List<WeeklyTiming> Weektime, string WeekoffDays, string EmailID)
        {
            try
            {

                // List<WeeklyTiming> weektime = JsonConvert.DeserializeObject<List<WeeklyTiming>>(Weektime);
                using (TransactionScope ts = new TransactionScope())
                {
                    //check Username Password with device
                    LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                    if (LD.ReturnType == "success")
                    {

                        using (var context = new ELDBDataContext())
                        {
                            var IsExistUser = (from u in context.Users where u.UserName == NewUserName && u.CompanyID == CompanyId && u.Status == 1 select u);

                            var ValidUser = true;
                            if (IsExistUser.Count() > 0)
                            {
                                if (UserId == 0)
                                    ValidUser = false;
                                else if (IsExistUser.First().UserId != UserId)
                                    ValidUser = false;
                            }

                            if (ValidUser == false)
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "Username " + NewUserName + " is already exists.";
                                LD.FailureReason = "";
                            }
                            else
                            {
                                if (UserId == 0)
                                {
                                    var CurrentActiveUser = context.Users.Where(w => (w.CompanyID == CompanyId) && (w.Status == 1)).Count();
                                    var UserLimit = context.Companies.Where(w => (w.CompanyId == CompanyId)).FirstOrDefault().UserLimit;

                                    #region AddUser
                                    if (CurrentActiveUser < UserLimit)
                                    {
                                        User usr = new User();
                                        usr.UserName = NewUserName;
                                        usr.Password = NewPassword;
                                        usr.RoleId = RoleId;
                                        usr.TopId = TopId;
                                        usr.CompanyID = CompanyId;
                                        usr.Status = 1;
                                        usr.GroupID = GroupID;
                                        usr.FirstName = FirstName;
                                        usr.LastName = LastName;
                                        usr.EmailID = EmailID;
                                        usr.MobileNoCmp = MobileNoCmp;
                                        usr.Status = 1;
                                        usr.CreatedDate = DateTime.Now;
                                        usr.CreatedBy = LD.UserId;
                                        usr.CreatedDate = DateTime.Now;
                                        usr.UpdatedBy = LD.UserId;
                                        usr.UpdatedDate = DateTime.Now;
                                        context.Users.InsertOnSubmit(usr);
                                        context.SubmitChanges();
                                        #endregion

                                        var Userde = context.Users.Where(u => u.UserId == usr.UserId).FirstOrDefault();
                                        var cmpname = (from cm in context.Companies where cm.CompanyId == Userde.CompanyID select cm.CompanyName).FirstOrDefault();
                                        Userde.UserCode = cmpname.Length > 5 ? cmpname.Substring(0, 5).ToUpper() + Userde.UserId.ToString() : cmpname + Userde.UserId.ToString();
                                        context.SubmitChanges();





                                        #region AddWeekOff
                                        List<UserWeekOffDay> uwd = new List<UserWeekOffDay>();

                                        context.UserWeekOffDays.Where(x => (x.UserId == usr.UserId) && (x.EndDate == null)).ToList().ForEach(e => e.EndDate = DateTime.Now);

                                        JavaScriptSerializer js = new JavaScriptSerializer();
                                        var LstObj = js.Deserialize<List<List<string>>>(WeekoffDays);
                                        foreach (var LO in LstObj)
                                        {
                                            var AD = String.Join(",", LO.GetRange(2, LO.Count() - 2));
                                            var UpdateDetail = context.UserWeekOffDays.Where(x => (x.UserId == usr.UserId) && (x.EndDate == null) &&
                                                      (x.WeekOffDay == LO[0]) &&
                                                      (Convert.ToBoolean(LO[1]) == true ? ((x.IsAlternative == Convert.ToBoolean(LO[1])) && (x.AlternativeDay == AD)) : (x.IsAlternative == Convert.ToBoolean(LO[1]))));

                                            if (UpdateDetail.Count() > 0)
                                            {
                                                UpdateDetail.FirstOrDefault().EndDate = null;
                                            }
                                            else
                                            {
                                                uwd.Add(new UserWeekOffDay
                                                {
                                                    UserId = usr.UserId,
                                                    WeekOffDay = LO[0],
                                                    IsAlternative = Convert.ToBoolean(LO[1]),
                                                    AlternativeDay = AD != "" ? AD : null,
                                                    CreatedBy = LD.UserId,
                                                    CreatedDate = DateTime.Now,
                                                });
                                            }
                                        }
                                        context.UserWeekOffDays.InsertAllOnSubmit(uwd);


                                        context.SubmitChanges();


                                        #endregion

                                        #region AddWeekDay
                                        foreach (var item in Weektime)
                                        {
                                            var Wtime = new WeeklyTiming();
                                            Wtime.UserID = usr.UserId;
                                            Wtime.TimingFor = 3;
                                            Wtime.ObjectId = usr.UserId;
                                            Wtime.TimingType = "WorkHours";
                                            Wtime.Day = item.Day;
                                            Wtime.DayType = 1;
                                            Wtime.TimeFrom = Convert.ToDateTime(item.TimeFrom);
                                            Wtime.TimeUpto = Convert.ToDateTime(item.TimeUpto);
                                            Wtime.WorkingHours = item.WorkingHours;
                                            Wtime.Status = true;
                                            Wtime.Edit = false;
                                            Wtime.CreatedOn = DateTime.Now;
                                            Wtime.CreatedBy = UserId;
                                            Wtime.UpdatedOn = DateTime.Now;
                                            Wtime.UpdatedBy = UserId;
                                            Wtime.IPAddress = "";
                                            context.WeeklyTimings.InsertOnSubmit(Wtime);
                                            context.SubmitChanges();
                                        }
                                        #endregion

                                        #region AddUserAccess


                                        var GrpUsrID = (from u in context.Users
                                                        join ga in context.GroupAccesses on u.GroupID equals ga.GroupID
                                                        where ga.GroupAccessID == GroupID && u.UserId != UserId

                                                        select u.UserId).ToList();


                                        foreach (var item3 in GrpUsrID)
                                        {
                                            if ((from ua in context.UserAccesses where ua.UserID == item3 && ua.UserAccessID == UserId select ua.ID).ToList().Count == 0)
                                            {
                                                var Ua = new UserAccess();
                                                Ua.UserID = item3;
                                                Ua.UserAccessID = usr.UserId;
                                                context.UserAccesses.InsertOnSubmit(Ua);
                                                context.SubmitChanges();
                                            }


                                        }
                                        var GrpAccUsrID = (from u in context.Users
                                                           join ga in context.GroupAccesses on u.GroupID equals ga.GroupAccessID
                                                           where ga.GroupID == GroupID && u.UserId != UserId

                                                           select u.UserId).ToList();

                                        foreach (var item2 in GrpAccUsrID)
                                        {
                                            if ((from ua in context.UserAccesses where ua.UserID == UserId && ua.UserAccessID == item2 select ua.ID).ToList().Count == 0)
                                            {
                                                var Ua = new UserAccess();
                                                Ua.UserID = usr.UserId;
                                                Ua.UserAccessID = item2;
                                                context.UserAccesses.InsertOnSubmit(Ua);
                                                context.SubmitChanges();
                                            }
                                        }
                                        #endregion

                                        LD.ReturnResult = "Username " + NewUserName + " added sucessfully.";

                                        #region SendEmailUsr
                                        string BodyText = "";
                                        if (File.Exists(Server.MapPath("~\\MailTemplates\\Welcome.html")))
                                        {

                                            StreamReader sr = new StreamReader(Server.MapPath("~\\MailTemplates\\Welcome.html"));
                                            BodyText = sr.ReadToEnd();
                                            sr.Close();
                                            BodyText = BodyText.Replace("##Name##", usr.FirstName + " " + usr.LastName);
                                            BodyText = BodyText.Replace("##CompanyName##", (from c in context.Companies where c.CompanyId == usr.CompanyID select c.CompanyName).FirstOrDefault());
                                            BodyText = BodyText.Replace("##Username##", usr.UserName);
                                            BodyText = BodyText.Replace("##Password##", usr.Password);
                                            BodyText = BodyText.Replace("##ReportingPerson##", (from u in context.Users where u.UserId == usr.TopId select u.FirstName + " " + u.LastName).FirstOrDefault());
                                            BodyText = BodyText.Replace("##Department##", (from g in context.GroupMasters where g.ID == usr.GroupID select g.GroupName).FirstOrDefault());

                                            foreach (var item1 in Weektime)
                                            {
                                                BodyText = BodyText.Replace("##" + item1.Day + "TimeIn##", Convert.ToString(item1.TimeFrom.TimeOfDay));
                                                BodyText = BodyText.Replace("##" + item1.Day + "TimeOut##", Convert.ToString(item1.TimeUpto.TimeOfDay));
                                            }


                                            //GF.SendEmailNotification("NKTPL+ Support", Convert.ToString(usr.EmailID), BodyText,"[" + (from c in context.Companies where c.CompanyId == usr.CompanyID select c.CompanyName).FirstOrDefault() + "+] - Welcome " + usr.FirstName + " " + usr.LastName);
                                        }
                                        #endregion

                                    }
                                    else
                                    {
                                        LD.ReturnType = "failure";
                                        LD.ReturnResult = "Users limit is over.";
                                        LD.FailureReason = "";
                                    }
                                }
                                else
                                {
                                    var UserDetail = (from u in context.Users where u.UserId == UserId select u).FirstOrDefault();
                                    var TotalChild = (from u in context.Users where u.TopId == UserId select u).Count();
                                    var ValidForEdit = true;

                                    if (ValidForEdit)
                                    {
                                        #region UpdateUser

                                        UserDetail.UserName = NewUserName;
                                        UserDetail.Password = NewPassword;
                                        UserDetail.EmailID = EmailID;
                                        UserDetail.RoleId = RoleId;
                                        UserDetail.TopId = TopId;
                                        UserDetail.CompanyID = CompanyId;
                                        UserDetail.Status = 1;
                                        UserDetail.GroupID = GroupID;
                                        UserDetail.FirstName = FirstName;
                                        UserDetail.LastName = LastName;
                                        UserDetail.MobileNoCmp = MobileNoCmp;
                                        UserDetail.Status = 1;
                                        UserDetail.UpdatedBy = LD.UserId;
                                        UserDetail.UpdatedDate = DateTime.Now;


                                        context.SubmitChanges();

                                        #endregion



                                        #region UpdateWeekOff

                                        List<UserWeekOffDay> uwd = new List<UserWeekOffDay>();

                                        context.UserWeekOffDays.Where(x => (x.UserId == UserDetail.UserId) && (x.EndDate == null)).ToList().ForEach(e => e.EndDate = DateTime.Now);

                                        JavaScriptSerializer js = new JavaScriptSerializer();
                                        var LstObj = js.Deserialize<List<List<string>>>(WeekoffDays);
                                        foreach (var LO in LstObj)
                                        {
                                            var AD = String.Join(",", LO.GetRange(2, LO.Count() - 2));
                                            var UpdateDetail = context.UserWeekOffDays.Where(x => (x.UserId == UserDetail.UserId) && (x.EndDate == null) &&
                                                      (x.WeekOffDay == LO[0]) &&
                                                      (Convert.ToBoolean(LO[1]) == true ? ((x.IsAlternative == Convert.ToBoolean(LO[1])) && (x.AlternativeDay == AD)) : (x.IsAlternative == Convert.ToBoolean(LO[1]))));

                                            if (UpdateDetail.Count() > 0)
                                            {
                                                UpdateDetail.FirstOrDefault().EndDate = null;
                                            }
                                            else
                                            {
                                                uwd.Add(new UserWeekOffDay
                                                {
                                                    UserId = UserDetail.UserId,
                                                    WeekOffDay = LO[0],
                                                    IsAlternative = Convert.ToBoolean(LO[1]),
                                                    AlternativeDay = AD != "" ? AD : null,
                                                    CreatedBy = LD.UserId,
                                                    CreatedDate = DateTime.Now,
                                                });
                                            }
                                        }
                                        context.UserWeekOffDays.InsertAllOnSubmit(uwd);

                                        context.SubmitChanges();


                                        #endregion

                                        #region UpdateWeekTiming

                                        context.WeeklyTimings.DeleteAllOnSubmit(context.WeeklyTimings.Where(c => c.ObjectId == UserDetail.UserId && c.TimingFor == 3 && c.TimingType == "WorkHours"));
                                        context.SubmitChanges();

                                        foreach (var item in Weektime)
                                        {
                                            var Wtime = new WeeklyTiming();
                                            Wtime.UserID = UserDetail.UserId;
                                            Wtime.TimingFor = 3;
                                            Wtime.ObjectId = UserDetail.UserId;
                                            Wtime.TimingType = "WorkHours";
                                            Wtime.Day = item.Day;
                                            Wtime.DayType = 1;
                                            Wtime.TimeFrom = Convert.ToDateTime(item.TimeFrom);
                                            Wtime.TimeUpto = Convert.ToDateTime(item.TimeUpto);
                                            Wtime.WorkingHours = item.WorkingHours;
                                            Wtime.Status = true;
                                            Wtime.Edit = false;
                                            Wtime.CreatedOn = DateTime.Now;
                                            Wtime.CreatedBy = UserId;
                                            Wtime.UpdatedOn = DateTime.Now;
                                            Wtime.UpdatedBy = UserId;
                                            Wtime.IPAddress = "";
                                            context.WeeklyTimings.InsertOnSubmit(Wtime);
                                            context.SubmitChanges();
                                        }

                                        #endregion

                                        #region AddUserAccess

                                        context.UserAccesses.DeleteAllOnSubmit(context.UserAccesses.Where(c => c.UserID == UserDetail.UserId));
                                        context.UserAccesses.DeleteAllOnSubmit(context.UserAccesses.Where(c => c.UserAccessID == UserDetail.UserId));


                                        var GrpUsrID = (from u in context.Users
                                                        join ga in context.GroupAccesses on u.GroupID equals ga.GroupID
                                                        where ga.GroupAccessID == UserDetail.GroupID && u.UserId != UserId

                                                        select u.UserId).ToList();


                                        foreach (var item3 in GrpUsrID)
                                        {
                                            if ((from ua in context.UserAccesses where ua.UserID == item3 && ua.UserAccessID == UserId select ua.ID).ToList().Count == 0)
                                            {
                                                var Ua = new UserAccess();
                                                Ua.UserID = item3;
                                                Ua.UserAccessID = UserDetail.UserId;
                                                context.UserAccesses.InsertOnSubmit(Ua);
                                                context.SubmitChanges();
                                            }


                                        }
                                        var GrpAccUsrID = (from u in context.Users
                                                           join ga in context.GroupAccesses on u.GroupID equals ga.GroupAccessID
                                                           where ga.GroupID == UserDetail.GroupID && u.UserId != UserId

                                                           select u.UserId).ToList();

                                        foreach (var item2 in GrpAccUsrID)
                                        {
                                            if ((from ua in context.UserAccesses where ua.UserID == UserId && ua.UserAccessID == item2 select ua.ID).ToList().Count == 0)
                                            {
                                                var Ua = new UserAccess();
                                                Ua.UserID = UserDetail.UserId;
                                                Ua.UserAccessID = item2;
                                                context.UserAccesses.InsertOnSubmit(Ua);
                                                context.SubmitChanges();
                                            }
                                        }
                                        #endregion



                                        LD.ReturnResult = "Username " + NewUserName + " update sucessfully.";


                                    }
                                }
                            }
                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string AddUserComponent(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string ComponentStr, int UserId)
        {
            try
            {

                // List<WeeklyTiming> weektime = JsonConvert.DeserializeObject<List<WeeklyTiming>>(Weektime);

                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {

                    using (var context = new ELDBDataContext())
                    {

                        context.UserComponentRights.DeleteAllOnSubmit(context.UserComponentRights.Where(c => c.UserId == UserId));

                        string[] arryComponentStr = ComponentStr.Split(',');

                        foreach (var item in arryComponentStr)
                        {
                            UserComponentRight uc = new UserComponentRight();
                            uc.UserId = UserId;
                            uc.ComponentId = Convert.ToInt32(item);
                            context.UserComponentRights.InsertOnSubmit(uc);
                            context.SubmitChanges();
                        }

                        LD.ReturnResult = "User Rights Updated Successfully";
                    }
                }


            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }

            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        [WebMethod]
        public string ListMenu(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<MenuDetail> MDetail = new List<MenuDetail>();

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        MDetail = (from m in context.Menus
                                   select new MenuDetail()
                                   {
                                       MenuId = m.MId,
                                       MenuName = m.MenuName,
                                       FormName = m.FormName,
                                       SortNumber = m.SortNumber.Value,
                                       IconName = m.IconName,
                                       IsActive = m.IsActive,
                                       ParentMenuId = m.PMId,
                                       SubMenu = (from sm in context.Menus
                                                  where sm.PMId == m.MId
                                                  select new SubMenuDetail()
                                                  {
                                                      MenuId = m.MId,
                                                      MenuName = m.MenuName,
                                                      FormName = m.FormName,
                                                      SortNumber = m.SortNumber.Value,
                                                      IconName = m.IconName,
                                                      IsActive = m.IsActive,
                                                  }).ToList()
                                   }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                MDetail = MDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListComponent(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<MenuWithComponentDetail> CMPDetail = new List<MenuWithComponentDetail>();

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        CMPDetail = (from m in context.Pages
                                     select new MenuWithComponentDetail()
                                     {
                                         PageId = m.PageId,
                                         PageName = m.PageName,
                                         Comp = (from sm in context.Components
                                                 where sm.PageId == m.PageId
                                                 select new ComponentDetail()
                                                 {
                                                     ComponentId = sm.ComponentId,
                                                     ComponentName = sm.ComponentName,
                                                     PageId = m.PageId,
                                                     PageName = m.PageName
                                                 }).ToList()
                                     }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                CMPDetail = CMPDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        [WebMethod]
        public string ListComponentWithUserID(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int UserID)
        {
            string result = "";
            List<MenuWithComponentDetail> CMPDetail = new List<MenuWithComponentDetail>();
            List<ComponentDetail> CMPAccDetail = new List<ComponentDetail>();
            List<MenuWithComponentDetail> CMPRemDetail = new List<MenuWithComponentDetail>();
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        CMPDetail = (from m in context.Pages
                                     select new MenuWithComponentDetail()
                                     {
                                         PageId = m.PageId,
                                         PageName = m.PageName,
                                         Comp = (from sm in context.Components
                                                 where sm.PageId == m.PageId
                                                 select new ComponentDetail()
                                                 {
                                                     ComponentId = sm.ComponentId,
                                                     ComponentName = sm.ComponentName,
                                                     PageId = m.PageId,
                                                     PageName = m.PageName
                                                 }).ToList()
                                     }).ToList();

                        CMPAccDetail = (from uc in context.UserComponentRights
                                        join c in context.Components on uc.ComponentId equals c.ComponentId
                                        join p in context.Pages on c.PageId equals p.PageId
                                        where uc.UserId == UserID
                                        select new ComponentDetail()
                                        {
                                            PageId = p.PageId,
                                            PageName = p.PageName,
                                            ComponentId = c.ComponentId,
                                            ComponentName = c.ComponentName
                                        }).ToList();


                        var cmpdetail = (from c in context.Components
                                         join p in context.Pages on c.PageId equals p.PageId
                                         where c.Status == true
                                         select new ComponentDetail()
                                         {
                                             PageId = p.PageId,
                                             PageName = p.PageName,
                                             ComponentId = c.ComponentId,
                                             ComponentName = c.ComponentName
                                         }).ToList();



                        var cmpnentlst = (from uc in context.UserComponentRights where uc.UserId == UserID select uc.ComponentId).ToList();
                        int[] cmpnentarry = cmpnentlst.ToArray();




                        if (CMPAccDetail.Count != 0)
                        {

                            // result = String.Join(",", aaa);
                            CMPRemDetail = (from m in context.Pages
                                            select new MenuWithComponentDetail()
                                            {
                                                PageId = m.PageId,
                                                PageName = m.PageName,
                                                Comp = (from sm in context.Components
                                                        where !cmpnentarry.Contains(sm.ComponentId) && sm.PageId == m.PageId
                                                        select new ComponentDetail()
                                                        {
                                                            ComponentId = sm.ComponentId,
                                                            ComponentName = sm.ComponentName,
                                                            PageId = m.PageId,
                                                            PageName = m.PageName
                                                        }).ToList()
                                            }).ToList();


                        }
                        else
                        {
                            CMPRemDetail = CMPDetail;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                CMPDetail = CMPDetail,
                CMPAccDetail = CMPAccDetail,
                CMPRemDetail = CMPRemDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string UpdateComponent(string UserName, string Password, string DeviceId, string DeviceType, int UserId, string ComponentList)
        {


            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        string[] str_arr = ComponentList.Split(',').ToArray();
                        int[] int_arr = Array.ConvertAll(str_arr, Int32.Parse);

                        foreach (var item in int_arr)
                        {
                            var uc = new UserComponentRight();
                            uc.UserId = UserId;
                            uc.ComponentId = item;
                            context.UserComponentRights.InsertOnSubmit(uc);
                            context.SubmitChanges();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetTopUserAndLeaveApprovalType(int RoleId, int SelectedRoleId, int CompanyId, int UserId)
        {
            dynamic UDetail = null;
            List<LeaveApprovalTypeDetail> LATDetail = new List<LeaveApprovalTypeDetail>();
            try
            {
                //check Username Password with device
                LD.ReturnType = "success";
                using (var context = new ELDBDataContext())
                {
                    var ValidComapnyList = false;
                    var FinalCompanyId = CompanyId;

                    if (SelectedRoleId == 2)
                    {
                        var IsCompanyUserExist = (context.Users.Where(w => w.CompanyID == CompanyId && w.RoleId == 2));
                        if (IsCompanyUserExist.Count() == 0)
                        {
                            ValidComapnyList = true;
                            FinalCompanyId = CompanyId;
                        }
                        else
                        {
                            if (IsCompanyUserExist.FirstOrDefault().UserId == UserId)
                            {
                                ValidComapnyList = true;
                                FinalCompanyId = CompanyId;
                            }
                            else
                            {
                                ValidComapnyList = false;
                            }
                        }
                    }
                    else
                    {
                        ValidComapnyList = true;
                        FinalCompanyId = CompanyId;
                    }

                    if (ValidComapnyList)
                    {
                        UDetail = (from u in context.Users
                                   where u.RoleId == (SelectedRoleId - 1) && (u.CompanyID == FinalCompanyId) && (u.UserId != UserId) && (u.Status == 1)
                                   select new
                                   {
                                       UserId = u.UserId,
                                       UserName = u.UserName,
                                   }).ToList();

                        //var top = "Select Employee";
                        //if (UDetail.Count == 0)
                        //    top = "Employee not exist";
                        //UDetail.Insert(0, new { UserId = 0, UserName = top });

                        if ((SelectedRoleId - 1) < 2)
                        {
                            LATDetail = (from la in context.LeaveApprovalTypes
                                         where la.LeaveApprovalTypeId == 3
                                         select new LeaveApprovalTypeDetail()
                                         {
                                             LeaveApprovalTypeId = la.LeaveApprovalTypeId,
                                             LeaveApprovalTypeName = la.LeaveApprovalTypeName,
                                         }).ToList();
                        }
                        else if ((SelectedRoleId - 1) == 2)
                        {
                            LATDetail = (from la in context.LeaveApprovalTypes
                                         where la.LeaveApprovalTypeId != 2
                                         select new LeaveApprovalTypeDetail()
                                         {
                                             LeaveApprovalTypeId = la.LeaveApprovalTypeId,
                                             LeaveApprovalTypeName = la.LeaveApprovalTypeName,
                                         }).ToList();
                        }
                        else
                        {
                            LATDetail = (from la in context.LeaveApprovalTypes
                                         select new LeaveApprovalTypeDetail()
                                         {
                                             LeaveApprovalTypeId = la.LeaveApprovalTypeId,
                                             LeaveApprovalTypeName = la.LeaveApprovalTypeName,
                                         }).ToList();
                        }

                        //    LATDetail.Insert(0, new LeaveApprovalTypeDetail() { LeaveApprovalTypeId = 0, LeaveApprovalTypeName = "Select Leave Approval Type" });
                    }
                    else
                    {
                        LD.ReturnType = "failure";
                        LD.ReturnResult = "Company Admin already exist.";
                        LD.FailureReason = "lable";
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                UDetail = UDetail,
                LATDetail = LATDetail,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetTopUserList(int GroupId, int CompanyId)
        {
            dynamic UDetail = null;
            try
            {
                //check Username Password with device
                LD.ReturnType = "success";

                using (var context = new ELDBDataContext())
                {
                    string result = "";
                    var gpDetail = (from ga in context.GroupAccesses where ga.GroupAccessID == GroupId select ga.GroupID.ToString()).ToList();
                    if (gpDetail.Count != 0)
                    {
                        gpDetail.Add(GroupId.ToString());
                    }
                    else
                    {
                        result = Convert.ToString(GroupId);
                    }
                    result = String.Join(",", gpDetail);


                    UDetail = (from u in context.Users
                               where gpDetail.Contains(u.GroupID.ToString())
                               orderby GroupId descending
                               select new
                               {
                                   UserId = u.UserId,
                                   UserName = u.FirstName + " " + u.LastName,
                               }).ToList();

                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
                UDetail = UDetail,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListUsers(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int GroupID, string AppVersion)
        {
            List<UserDetail> URDetail = new List<UserDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {

                        if (GroupID != -1)
                        {
                            var UDetail = (from u in context.Users
                                           join ua in context.UserAccesses on u.UserId equals ua.UserAccessID
                                           where ua.UserID == LD.UserId && u.Status == 1
                                           select new
                                           {
                                               UserId = u.UserId,
                                               Name = u.FirstName + " " + u.LastName,
                                               UserName = u.UserName,
                                               Mobile = u.MobileNoCmp,
                                               TopId = u.TopId,
                                               EmailID = u.EmailID,
                                               Password = u.Password,
                                               GroupID = u.GroupID,
                                               ProfilePic = u.ProfilePic,
                                               TopName = (from u in context.Users where u.UserId == u.TopId select u.UserName).FirstOrDefault(),
                                               RoleName = (from r in context.RoleMasters where r.RoleId == u.RoleId select r.RoleName).FirstOrDefault(),
                                               DeviceCount = (from ud in context.UserDevices where ud.UserId == u.UserId && ud.IsDeleted == false select ud.UserId).Count(),
                                               CompanyName = (from ic in context.Companies where ic.CompanyId == u.CompanyID select ic.CompanyName).FirstOrDefault(),
                                               GroupName = (from g in context.GroupMasters where g.ID == u.GroupID select g.GroupName).FirstOrDefault(),
                                               Weekoff = string.Join(",", (context.UserWeekOffDays.Where(w => (w.UserId == u.UserId) && (w.EndDate == null)).Select(s => s.WeekOffDay + "(" + (s.AlternativeDay == null ? "All" : s.AlternativeDay) + ")"))),
                                               IsEmployeeAddress = (from ea in context.EmployeeAddresses where ea.UserID == u.UserId select ea.ID).Count() > 0 ? "True" : "False",
                                               IsEmployeeBankDetails = (from ea in context.EmployeeBankDetails where ea.UserID == u.UserId select ea.AccountId).Count() > 0 ? "True" : "False",
                                               IsEmployeeEducationDetails = (from ea in context.EmployeeEducationDetails where ea.UserID == u.UserId select ea.EduId).Count() > 0 ? "True" : "False",
                                               IsEmployeeFamily = (from ea in context.EmployeeFamilies where ea.UserID == u.UserId select ea.ID).Count() > 0 ? "True" : "False",
                                               IsEmployeePersonalIDs = (from ea in context.EmployeePersonalIDs where ea.UserID == u.UserId select ea.ID).Count() > 0 ? "True" : "False",
                                               IsPersonalInfo = (from ea in context.Users where ea.GenderTypeId != null && ea.DateOfBirth != null && ea.ProfilePic != null && ea.MaritalStatus != null && ea.City != null && ea.State != null && ea.Country != null && ea.Pincode != null && ea.EmailIDPer != null && ea.MobileNoCmp != null && ea.UserId == u.UserId select ea.UserId).Count() > 0 ? "True" : "False",
                                               wtimings = (from wt in context.WeeklyTimings where wt.UserID == u.UserId select wt).ToList(),
                                               WOffDay = (from wo in context.UserWeekOffDays where wo.UserId == u.UserId select wo).ToList(),
                                               FirstName = u.FirstName,
                                               LastName = u.LastName,
                                               // UsrDevice = (from ud in context.UserDevices where ud.UserId==u.UserId select ud).ToList(),
                                               Children = GF.GetChildren(context, u.UserId)
                                           })
                         .AsEnumerable()
                        .Select(c => new HierarchyUserDetail()
                        {
                            Name = c.Name,
                            EmailID = c.EmailID,
                            Password = c.Password,
                            FirstName = c.FirstName,
                            LatName = c.LastName,
                            Mobile = c.Mobile,
                            UserId = c.UserId,
                            UserName = c.UserName,
                            TopId = c.TopId,
                            TopName = c.TopName,
                            GroupId = c.GroupID,
                            RoleName = c.RoleName,
                            DeviceCount = c.DeviceCount,
                            CompanyName = c.CompanyName,
                            ProfilePic = c.ProfilePic,
                            GroupName = c.GroupName,
                            Weekoff = c.Weekoff == "" ? "Not Available" : c.Weekoff,
                            IsEmployeeAddress = c.IsEmployeeAddress,
                            IsEmployeeBankDetails = c.IsEmployeeBankDetails,
                            IsEmployeeEducationDetails = c.IsEmployeeEducationDetails,
                            IsEmployeeFamily = c.IsEmployeeFamily,
                            IsEmployeePersonalIDs = c.IsEmployeePersonalIDs,
                            IsPersonalInfo = c.IsPersonalInfo,
                            wtimings = c.wtimings,
                            WOffDay = c.WOffDay,
                            // UsrDevice = c.UsrDevice,
                            Children = c.Children,
                        }).ToList();
                            GF.HieararchyWalk(UDetail, URDetail);
                        }
                        else
                        {
                            var UDetail = context.Users
                          .Where(c => (c.UserId == LD.UserId) && (c.Status == 1))
                           .Select(c => new
                           {
                               UserId = c.UserId,
                               UserName = c.UserName,
                               Mobile = c.MobileNoCmp,
                               Name = c.FirstName + " " + c.LastName,
                               EmailID = c.EmailID,
                               Password = c.Password,
                               FirstName = c.FirstName,
                               LastName = c.LastName,
                               TopId = c.TopId,
                               GroupID = c.GroupID,
                               ProfilePic = c.ProfilePic,
                               GroupName = (from g in context.GroupMasters where g.ID == c.GroupID select g.GroupName).FirstOrDefault(),
                               TopName = (from u in context.Users where u.UserId == c.TopId select u.UserName).FirstOrDefault(),
                               RoleName = (from r in context.RoleMasters where r.RoleId == c.RoleId select r.RoleName).FirstOrDefault(),
                               DeviceCount = (from ud in context.UserDevices where ud.UserId == c.UserId && ud.IsDeleted == false select ud.UserId).Count(),
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
                               wtimings = (from wt in context.WeeklyTimings where wt.UserID == c.UserId select wt).ToList(),
                               WOffDay = (from wo in context.UserWeekOffDays where wo.UserId == c.UserId select wo).ToList(),
                               Children = GF.GetChildren(context, c.UserId)
                           })
                           .AsEnumerable()
                          .Select(c => new HierarchyUserDetail()
                          {
                              UserId = c.UserId,
                              UserName = c.UserName,
                              Name = c.Name,
                              Mobile = c.Mobile,
                              EmailID = c.EmailID,
                              Password = c.Password,
                              FirstName = c.FirstName,
                              LatName = c.LastName,
                              TopId = c.TopId,
                              TopName = c.TopName,
                              GroupId = c.GroupID,
                              RoleName = c.RoleName,
                              ProfilePic = c.ProfilePic,
                              DeviceCount = c.DeviceCount,
                              CompanyName = c.CompanyName,
                              GroupName = c.GroupName,
                              LeaveApprovalTypeName = c.LeaveApprovalTypeName,
                              LeaveApprovalSecondLevelName = c.LeaveApprovalSecondLevelName,
                              Weekoff = c.Weekoff == "" ? "Not Available" : c.Weekoff,
                              IsEmployeeAddress = c.IsEmployeeAddress,
                              IsEmployeeBankDetails = c.IsEmployeeBankDetails,
                              IsEmployeeEducationDetails = c.IsEmployeeEducationDetails,
                              IsEmployeeFamily = c.IsEmployeeFamily,
                              IsEmployeePersonalIDs = c.IsEmployeePersonalIDs,
                              IsPersonalInfo = c.IsPersonalInfo,
                              wtimings = c.wtimings,
                              WOffDay = c.WOffDay,
                              Children = c.Children,
                          }).ToList();
                            GF.HieararchyWalk(UDetail, URDetail);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
                //GF.SendEmail(e.ToString(), "ListUsers", "NA");
            }
            var FinalResult = new
            {
                URDetail = URDetail,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ListUserByID(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int UserId, string AppVersion)
        {
            List<UserDetail> URDetail = new List<UserDetail>();
            List<UserPersonaInfo> Usrpersonal = new List<UserPersonaInfo>();
            List<UserEducationInfo> UsrEducation = new List<UserEducationInfo>();
            List<UserFamilyDetail> UsrFamily = new List<UserFamilyDetail>();
            List<UserAddressDetail> UsrAddress = new List<UserAddressDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {

                        if (UserId != -1)
                        {
                            Usrpersonal = (from u in context.Users
                                           where u.UserId == UserId
                                           select new UserPersonaInfo
                                           {
                                               FirstName = u.FirstName,
                                               LastName = u.LastName,
                                               GenderTypeId = u.GenderTypeId,
                                               DateOfBirth = Convert.ToString(u.DateOfBirth),
                                               ProfilePic = u.ProfilePic,
                                               UserCode = u.UserCode,
                                               MaritalStatus = u.MaritalStatus,
                                               PhoneNoPer = u.PhoneNoPer,
                                               EmailIDPer = u.EmailIDPer
                                           }
                                          ).ToList();

                            UsrAddress = (from ua in context.EmployeeAddresses
                                          where ua.UserID == UserId
                                          select new UserAddressDetail
                                          {
                                              EAID = ua.ID,
                                              UserID = ua.UserID,
                                              Address = ua.Address,
                                              City = ua.City,
                                              State = ua.State,
                                              Pincode = ua.Pincode,
                                              CurrAddress = ua.CurrAddress,
                                              CurrCity = ua.CurrCity,
                                              CurrState = ua.CurrState,
                                              CurrPincode = ua.CurrPincode,
                                              ImergencyName = ua.ImergencyName,
                                              ImergencyCntNo = ua.ImergencyCntNo
                                          }).ToList();

                            UsrEducation = (from ue in context.EmployeeEducationDetails
                                            where ue.UserID == UserId
                                            select new UserEducationInfo
                                            {
                                                EduId = ue.EduId,
                                                UserID = ue.UserID,
                                                Degree = ue.Degree,
                                                Institute = ue.Institute,
                                                University = ue.University,
                                                YearOfPassing = ue.YearOfPassing,
                                                GradeType = ue.GradeType,
                                                Grade = ue.Grade,
                                                CourseType = ue.CourseType,
                                                Status = ue.Status,
                                                Edit = ue.Edit,
                                                CreatedOn = ue.CreatedOn,
                                                CreatedBy = (from u in context.Users where u.UserId == ue.CreatedBy select u.FirstName + " " + u.LastName).FirstOrDefault(),
                                                UpdatedOn = ue.UpdatedOn,
                                                UpdatedBy = (from u in context.Users where u.UserId == ue.UpdatedBy select u.FirstName + " " + u.LastName).FirstOrDefault(),


                                            }).ToList();

                            UsrFamily = (from uf in context.EmployeeFamilies
                                         where uf.UserID == UserId
                                         select new UserFamilyDetail
                                         {
                                             ID = uf.ID,
                                             UserID = uf.UserID,
                                             Name = uf.Name,
                                             DateOfBirth = Convert.ToString(uf.DateOfBirth),
                                             Relation = uf.Relation,
                                             Status = uf.Status,
                                             Edit = uf.Edit,
                                             CreatedOn = uf.CreatedOn,
                                             CreatedBy = (from u in context.Users where u.UserId == uf.CreatedBy select u.FirstName + " " + u.LastName).FirstOrDefault(),
                                             UpdatedOn = uf.UpdatedOn,
                                             UpdatedBy = (from u in context.Users where u.UserId == uf.UpdatedBy select u.FirstName + " " + u.LastName).FirstOrDefault(),
                                             IPAddress = uf.IPAddress
                                         }).ToList();


                        }
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                //LD.ReturnResult = e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                // URDetail = URDetail,
                UsrFamily = UsrFamily,
                UsrEducation = UsrEducation,
                Usrpersonal = Usrpersonal,
                UsrAddress = UsrAddress,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }



        [WebMethod]
        public string UserDetailsDateWise(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserID, int SelectedCompanyId, string SelectedDate)
        {
            List<PunchInListDetail> PDetail = new List<PunchInListDetail>();
            int CallCount = 0;
            DateTime date = DateTime.Parse(SelectedDate);
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        PDetail = (from u in context.Users
                                   join temp in context.PunchIns on new { u.UserId, date.Date } equals new { temp.UserId, temp.PunchinTime.Date } into tempjoin
                                   from p in tempjoin.DefaultIfEmpty()
                                   where u.Status == 1 && u.CompanyID == SelectedCompanyId && u.UserId == SelectedUserID
                                   select new PunchInListDetail()
                                   {
                                       UserName = u.UserName,
                                       PunchInTime = p.PunchinTime != null ? Convert.ToString(p.PunchinTime) : "N.A",
                                       PunchOutTime = p.PunchoutTime != null ? Convert.ToString(p.PunchoutTime.Value) : "N.A",
                                       WorkingHour = p.PunchoutTime != null ?
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Hours + ":" +
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Minutes + ":" +
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Seconds
                                                   : "N.A",
                                       ExtraWorkingHour = p.PunchoutTime != null ? Convert.ToString(p.ExtraWorkingHour) : "N.A",
                                       LatePunchin = (p.LatePunchin == true ? 1 : 0),
                                       EarlyPunchout = (p.EarlyPunchout == true ? 1 : 0),
                                       //ExtraWorkingHour = p.PunchoutTime != null ?
                                       //TimeSpan.Parse(p.ExtraWorkingHour).Hours + ":" +
                                       //                    TimeSpan.Parse(p.ExtraWorkingHour).Minutes + ":" +
                                       //                    TimeSpan.Parse(p.ExtraWorkingHour).Seconds
                                       //                    : "N.A",
                                       PunchinType = (p.PunchinType != null ? p.PunchinType == true ? "Inside" : "Outside" : "N.A"),
                                       PunchoutType = (p.PunchoutType != null ? p.PunchoutType == true ? "Inside" : "Outside" : "N.A"),
                                       PunchinAddress = (p.PunchinType != null ? p.PunchinType == true ? (from l in context.Locations where l.LocationId == p.PILocationId select l.LocationName).FirstOrDefault() : "N.A" : "N.A"),

                                       PunchoutAddress = (p.PunchoutType != null ? p.PunchoutType == true ? (from l in context.Locations where l.LocationId == p.POLocationId select l.LocationName).FirstOrDefault() : "N.A" : "N.A"),
                                       PunchinLatlong = p.PILatitudeLongitude,
                                       punchoutLatlong = p.POLatitudeLongitude

                                   }).ToList();


                        CallCount = (from u in context.Users
                                     join p in context.CallIns on u.UserId equals p.UserId
                                     where u.Status == 1 && u.CompanyID == SelectedCompanyId && u.UserId == SelectedUserID && p.CallinTime.Date == date.Date
                                     select p.CId).Count();



                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                PDetail = PDetail,
                CallCount = CallCount,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string UserDetailsMonth(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserID, int SelectedCompanyId, string SelectedMonth)
        {
            List<PunchInListDetail> PDetail = new List<PunchInListDetail>();
            int CallCount = 0;
            DateTime date = DateTime.Parse(SelectedMonth);

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        PDetail = (from u in context.Users
                                   join temp in context.PunchIns on new { u.UserId, date.Date } equals new { temp.UserId, temp.PunchinTime.Date } into tempjoin
                                   from p in tempjoin.DefaultIfEmpty()
                                   where u.Status == 1 && u.CompanyID == SelectedCompanyId && u.UserId == SelectedUserID
                                   select new PunchInListDetail()
                                   {
                                       UserName = u.UserName,
                                       PunchInTime = p.PunchinTime != null ? Convert.ToString(p.PunchinTime) : "N.A",
                                       PunchOutTime = p.PunchoutTime != null ? Convert.ToString(p.PunchoutTime.Value) : "N.A",
                                       WorkingHour = p.PunchoutTime != null ?
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Hours + ":" +
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Minutes + ":" +
                                                           (p.PunchoutTime.Value.TimeOfDay - p.PunchinTime.TimeOfDay).Seconds
                                                   : "N.A",
                                       ExtraWorkingHour = p.PunchoutTime != null ? Convert.ToString(p.ExtraWorkingHour) : "N.A",
                                       //ExtraWorkingHour = p.PunchoutTime != null ?
                                       //TimeSpan.Parse(p.ExtraWorkingHour).Hours + ":" +
                                       //                    TimeSpan.Parse(p.ExtraWorkingHour).Minutes + ":" +
                                       //                    TimeSpan.Parse(p.ExtraWorkingHour).Seconds
                                       //                    : "N.A",
                                       PunchinType = (p.PunchinType != null ? p.PunchinType == true ? "Inside" : "Outside" : "N.A"),
                                       PunchoutType = (p.PunchoutType != null ? p.PunchoutType == true ? "Inside" : "Outside" : "N.A"),
                                       PunchinAddress = (p.PunchinType != null ? p.PunchinType == true ? (from l in context.Locations where l.LocationId == p.PILocationId select l.LocationName).FirstOrDefault() : "N.A" : "N.A"),

                                       PunchoutAddress = (p.PunchoutType != null ? p.PunchoutType == true ? (from l in context.Locations where l.LocationId == p.POLocationId select l.LocationName).FirstOrDefault() : "N.A" : "N.A")

                                   }).ToList();

                        //   books.Where(book => book.Tags.Contains(genre))
                        var Myreport = PDetail.Where(a => a.PunchInTime == Convert.ToString(DateTime.Now.Date)).ToList();


                        CallCount = (from u in context.Users
                                     join p in context.CallIns on u.UserId equals p.UserId
                                     where u.Status == 1 && u.CompanyID == SelectedCompanyId && u.UserId == SelectedUserID && p.CallinTime.Date == date.Date
                                     select p.CId).Count();



                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                PDetail = PDetail,
                CallCount = CallCount,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        #endregion

        #region Common
        internal void SendEmailforDeviceApproval(UserDevice URDetail)
        {
            try
            {
                using (var context = new ELDBDataContext())
                {

                    #region SendEmailDeviceApproval
                    string BodyText = "";
                    if (File.Exists(Server.MapPath("~\\MailTemplates\\Registration-Request.html")))
                    {
                        string email = "";
                        string approvedby = "";
                        string subject = "";

                        string Name = (from u in context.Users where u.UserId == URDetail.UserId select u.FirstName + " " + u.LastName).FirstOrDefault();
                        string UserName = (from u in context.Users where u.UserId == URDetail.UserId select u.UserName).FirstOrDefault();

                        string company = (from c in context.Companies
                                          join u in context.Users on c.CompanyId equals u.CompanyID
                                          where u.UserId == URDetail.UserId
                                          select c.CompanyName).FirstOrDefault();


                        subject = "[" + company + " +] - " + Name + " - Device Approval";


                        StreamReader sr = new StreamReader(Server.MapPath("~\\MailTemplates\\Registration-Request.html"));
                        BodyText = sr.ReadToEnd();
                        sr.Close();
                        BodyText = BodyText.Replace("##Username##", (from u in context.Users where u.UserId == URDetail.UserId select u.FirstName + " " + u.LastName).FirstOrDefault());
                        BodyText = BodyText.Replace("##DeviceName##", URDetail.DeviceName);
                        BodyText = BodyText.Replace("##DeviceID##", URDetail.DeviceId);
                        BodyText = BodyText.Replace("##OSVersion##", URDetail.OSVersion);
                        BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
                        BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
                        BodyText = BodyText.Replace("##RegistrationDate##", URDetail.RequestDate.ToString());
                        BodyText = BodyText.Replace("##RegistrationTime##", URDetail.RequestDate.TimeOfDay.ToString());



                        // GF.SendEmailNotification("NKTPL+ Device Status", UserName, BodyText, subject);
                    }
                    #endregion
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        #endregion

        #region ~Passwordreg

        [WebMethod]
        public string ChangePassword(string UserName, string Password, string DeviceId, string DeviceType, string NewPassword, int UserID, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new ELDBDataContext())
                    {
                        var EIDdetail = (from wt in context.Users where wt.UserId == UserID select wt).FirstOrDefault();
                        EIDdetail.Password = NewPassword;
                        context.SubmitChanges();
                        LD.ReturnResult = "Password has been Updated sucessfully.";

                    }
                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ForgotPassword(string DeviceId, string DeviceType, string Str, string IPAddress, string latlong)
        {
            long OTPID = 0;
            try
            {
                //check Username Password with device
                using (var context = new ELDBDataContext())
                {

                    var chk = (from ot in context.UsersOTPs where ot.DeviceID == DeviceId && ot.DeviceType == DeviceType && ot.OTPSentTime.Date == DateTime.Now.Date && ot.IsVerified == false select ot).ToList();

                    if (chk.Count > 3)
                    {
                        LD.ReturnType = "failure";
                        LD.ReturnResult = "you have reached maximun attatemps for today.Please try tomorow...";
                        LD.FailureReason = "";
                    }
                    else
                    {

                        var EIDdetail = (from wt in context.Users where wt.EmailID == Str || wt.MobileNoCmp == Str select wt).FirstOrDefault();

                        if (EIDdetail != null)
                        {
                            Random generator = new Random();
                            string s = generator.Next(0, 10000000).ToString("D7");

                            UsersOTP ut = new UsersOTP();
                            ut.UserId = EIDdetail.UserId;
                            ut.OTP = Convert.ToInt64(s);
                            ut.IPAddress = IPAddress;
                            ut.OTPSentTime = DateTime.Now;
                            ut.DeviceID = DeviceId;
                            ut.DeviceType = DeviceType;
                            ut.LatLong = latlong;
                            ut.IsVerified = false;
                            ut.OTPSent = false;
                            context.UsersOTPs.InsertOnSubmit(ut);
                            context.SubmitChanges();
                            string msg = "call rem Your OTP is " + ut.OTP + ",,Assgn:,Remarks:";
                            //    var resp = GF.SMSSend(Convert.ToString(ConfigurationManager.AppSettings["SMSUid"]), Convert.ToString(ConfigurationManager.AppSettings["SMSKey"]), Convert.ToString(ConfigurationManager.AppSettings["SMSSenderId"]), EIDdetail.MobileNoCmp, msg);
                            var resp = "sms sent";
                            if (resp.ToLower().Contains("sms sent"))
                            {
                                OTPID = ut.ID;
                                var uut = (from uo in context.UsersOTPs where uo.ID == ut.ID select uo).FirstOrDefault();
                                uut.OTPSent = true;
                                ut.OTPSentTime = DateTime.Now;
                                context.SubmitChanges();
                                LD.ReturnType = "success";
                                LD.ReturnResult = "OTP has been sent to register number " + EIDdetail.MobileNoCmp.Substring(0, 2) + "######" + EIDdetail.MobileNoCmp.Substring(EIDdetail.MobileNoCmp.Length - 2);
                            }
                            else
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "Something went wrong...";
                                LD.FailureReason = "";
                            }

                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "please enter correct Email ID / Mobile Number...";
                            LD.FailureReason = "";
                        }


                    }
                }

            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                OTPID = OTPID,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        [WebMethod]
        public string ReceiveOTP(string DeviceId, string DeviceType, long OTP, string IPAddress, int OTPID)
        {
            long UserID = 0;
            try
            {
                using (var context = new ELDBDataContext())
                {
                    var EIDdetail = (from uo in context.UsersOTPs where uo.ID == OTPID select uo).FirstOrDefault();



                    if (EIDdetail != null)
                    {



                        if ((DateTime.Now - EIDdetail.OTPSentTime).TotalSeconds <= 180)
                        {
                            if (EIDdetail.OTP == OTP)
                            {
                                UserID = EIDdetail.UserId;
                                LD.ReturnType = "success";
                                LD.ReturnResult = "OTP has been verified...";
                            }
                            else
                            {
                                LD.ReturnType = "failure";
                                LD.ReturnResult = "you have entered wrong OTP...";
                                LD.FailureReason = "";
                            }
                        }
                        else
                        {
                            LD.ReturnType = "failure";
                            LD.ReturnResult = "Request timeout...";
                            LD.FailureReason = "";
                        }
                    }
                    else
                    {
                        LD.ReturnType = "failure";
                        LD.ReturnResult = "Something went wrong.try after some time...";
                        LD.FailureReason = "";
                    }

                    context.SubmitChanges();

                }
            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                UserID = UserID,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        [WebMethod]
        public string resetPassword(string NewPassword, int UserID)
        {
            try
            {
                //check Username Password with device


                using (var context = new ELDBDataContext())
                {
                    var EIDdetail = (from wt in context.Users where wt.UserId == UserID select wt).FirstOrDefault();

                    if (EIDdetail != null)
                    {
                        EIDdetail.Password = NewPassword;
                        context.SubmitChanges();
                        LD.ReturnResult = "Password has been Updated sucessfully.";
                        LD.ReturnType = "success";
                    }
                    else
                    {
                        LD.ReturnType = "failure";
                        LD.ReturnResult = "Something went wrong...kindly contact administrator...";
                        LD.FailureReason = "";
                    }


                }

            }
            catch
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }


        [WebMethod]
        public string SSendSMS(string uid, string pwd, string gsmsenderid, string mob, string msg)
        {
            var abc = GF.SMSSend(uid, pwd, gsmsenderid, mob, msg);
            return abc;
        }

        #endregion

        #region ~Leave Management 

        [WebMethod]
        public string CheckAppVersion(string VersionName, int VersionID, int DeviceType)
        {
            CheckAppVersion_Model result = new CheckAppVersion_Model();
            try
            {
                result = BAModel.BAModel.CheckAppVersion(VersionName, VersionID, DeviceType);
                if (result.response.status == false)
                {
                    LD.ReturnType = "failure";
                    LD.FailureReason = result.response.message;
                    LD.ReturnResult = "Server Is Not Responding..." + result.response.message;
                }

            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                records = result.record,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string ManageLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID, int UserID, long LeaveId, int LeaveTypeId, string Descirption, string StartDate, string EndDate, int MasterLeaveTypeId)
        {

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    ResultStatusModel response = BAModel.BAModel.ManageLeave(CompanyID, UserID, LeaveId, LeaveTypeId, Descirption, StartDate, EndDate, MasterLeaveTypeId);

                    if (response.status == false)
                    {
                        LD.ReturnType = "failure";
                        LD.FailureReason = response.message;
                        LD.ReturnResult = "Server Is Not Responding..." + response.message + StartDate + ":::" + EndDate;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLeavesListByUser(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID, int UserID)
        {
            GetLeavesListByUser_Model result = new GetLeavesListByUser_Model();
            try
            {
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    result = BAModel.BAModel.GetLeavesListByUser(CompanyID, UserID);

                    if (result.response.status == false)
                    {
                        LD.ReturnType = "failure";
                        LD.FailureReason = result.response.message;
                        LD.ReturnResult = "Server Is Not Responding..." + result.response.message;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                report = result.report,
                records = result.records,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLeavesListByCompany(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID)
        {
            GetLeavesListByCompany_Model result = new GetLeavesListByCompany_Model();
            try
            {
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    result = BAModel.BAModel.GetLeavesListByCompany(CompanyID);

                    if (result.response.status == false)
                    {
                        LD.ReturnType = "failure";
                        LD.FailureReason = result.response.message;
                        LD.ReturnResult = "Server Is Not Responding..." + result.response.message;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                report = result.report,
                records = result.records,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string UpdateLeaveStatus(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID, int UserID, int LeaveId, int LeaveStatusId)
        {

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    ResultStatusModel result = BAModel.BAModel.UpdateLeaveStatus(CompanyID, UserID, LeaveId, LeaveStatusId);

                    if (result.status == false)
                    {
                        LD.ReturnType = "failure";
                        LD.FailureReason = result.message;
                        LD.ReturnResult = "Server Is Not Responding..." + result.message;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLeaveDetail(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID, int UserID, int LeaveID)
        {
            GetLeaveDetail_Model result = new GetLeaveDetail_Model();
            try
            {
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    result = BAModel.BAModel.GetLeaveDetail(CompanyID, UserID, LeaveID);

                    if (result.response.status == false)
                    {
                        LD.ReturnType = "failure";
                        LD.FailureReason = result.response.message;
                        LD.ReturnResult = "Server Is Not Responding..." + result.response.message;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                records = result.record,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string GetLeavesReportByUser(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID, int UserID)
        {
            GetLeavesReportByUser_Model result = new GetLeavesReportByUser_Model();
            try
            {
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    result = BAModel.BAModel.GetLeavesReportByUser(CompanyID, UserID);

                    if (result.response.status == false)
                    {
                        LD.ReturnType = "failure";
                        LD.FailureReason = result.response.message;
                        LD.ReturnResult = "Server Is Not Responding..." + result.response.message;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                records = result.record,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        [WebMethod]
        public string SendEmployeePunchReport(string UserName, string Password, string DeviceId, string DeviceType, List<int> UserIDs, string StartDate, string EndDate, string SendEmailTo, string SendEmailCC)
        {
            ResultStatusModel result = new ResultStatusModel();
            try
            {
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    result = BAModel.BAModel.SendEmployeePunchReport(UserIDs, StartDate, EndDate, SendEmailTo, SendEmailCC);

                    if (result.status)
                    {
                        LD.ReturnType = "success";
                        LD.ReturnResult = "report send successfully!";
                        LD.FailureReason = "";
                    }
                    else
                    {
                        LD.ReturnType = "success";
                        LD.ReturnResult = "";
                        LD.FailureReason = "something went wrong!" + result.message;
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                //records = result.record,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }

        #endregion

        #region Common Service by Milan
        [WebMethod]
        public string ListUsersByGroupId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int GroupID, string AppVersion)
        {
            ListUsersByGroupId_Result_Model result = new ListUsersByGroupId_Result_Model();
            try
            {
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    result = BAModel.BAModel.ListUsersByGroupId(GroupID);

                    if (result.response.status)
                    {
                        LD.ReturnType = "success";
                        LD.ReturnResult = "report send successfully!";
                        LD.FailureReason = "";
                    }
                    else
                    {
                        LD.ReturnType = "success";
                        LD.ReturnResult = "";
                        LD.FailureReason = "something went wrong!";
                    }
                }
            }
            catch (Exception e)
            {
                LD.ReturnType = "failure";
                LD.ReturnResult = "Server Is Not Responding..." + e.ToString();
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                records = result.records,
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(FinalResult);
        }
        #endregion

    }
}