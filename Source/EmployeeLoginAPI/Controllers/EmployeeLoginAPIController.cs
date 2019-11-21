using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using DataModel;
using EmployeeLoginAPI.Models;
using EmployeeLoginService.BaseObject;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmployeeLoginAPI.Controllers
{
    public class EmployeeLoginAPIController : ApiController
    {
        private GeneralFunction GF = new GeneralFunction();
        EmployeeLoginAPI.Models.Login LD = new EmployeeLoginAPI.Models.Login();

        #region APIs
        public HttpResponseMessage UpdateToken([FromBody]UpdateToken_request request)
        {
            try
            {

                string UserName = request.UserName;
                string Password = request.Password;
                string DeviceId = request.DeviceId;
                string DeviceType = request.DeviceType;
                int CompanyId = request.CompanyId;
                string AppVersion = request.AppVersion;
                int BreakCategoryId = request.BreakCategoryId;
                string BreakCategoryName = request.BreakCategoryName;
                bool IsActive = request.IsActive;
                bool HasTextbox = request.HasTextbox;
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (EmployeeLoginEntities context = new EmployeeLoginEntities())
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

                            context.BreakCategories.Add(bt);
                            context.SaveChanges();

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

                            context.SaveChanges();

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

            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        public HttpResponseMessage AddBreakCategory(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakCategoryId, string BreakCategoryName, bool IsActive, bool HasTextbox)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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



                            context.BreakCategories.Add(bt);
                            context.SaveChanges();

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

                            context.SaveChanges();

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

            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        public HttpResponseMessage AddCompany(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int NewCompanyId, string CompanyName, bool IsActive,
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
                        using (var context = new EmployeeLoginEntities())
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
                                    context.Companies.Add(cmp);
                                    context.SaveChanges();



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

                                    //context.SaveChanges();

                                    LD.ReturnResult = "Company " + CompanyName + " added sucessfully.";



                                    GroupMaster g = new GroupMaster();

                                    g.GroupName = "Super Admin";
                                    g.GroupCode = "SUPERADMIN";
                                    g.CreatedBy = LD.UserId;
                                    g.CompanyID = cmp.CompanyId;
                                    g.CreatedDate = Convert.ToDateTime(DateTime.Now);
                                    g.Status = IsActive;
                                    context.GroupMasters.Add(g);
                                    context.SaveChanges();


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
                                    context.Users.Add(usr);
                                    context.SaveChanges();

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
                                        context.WeeklyTimings.Add(Wtime);
                                        context.SaveChanges();
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
                                        context.SaveChanges();
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
                                        //context.SaveChanges();


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
                                        //    context.SaveChanges();




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
                                        //        context.CompanyLeaveTypeCounts.Add(cltc);
                                        //    }
                                        //    context.SaveChanges();
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

            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        public HttpResponseMessage AddHoliday(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, string HolidayDates, string Description)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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

                            context.Holidays.Add(hl);
                            context.SaveChanges();
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

            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        public HttpResponseMessage AddLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, string LeaveDateFrom, string LeaveDateTo, int LeaveTypeId, string Description, List<DateTime> ListHalfDay)
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
                    using (var context = new EmployeeLoginEntities())
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

                            context.Leaves.Add(l);
                            context.SaveChanges();

                            List<HalfDay> hflist = new List<HalfDay>();
                            foreach (var objhf in ListHalfDay)
                            {
                                hflist.Add(new HalfDay { LeaveID = l.LeaveId, HalfDayDate = objhf });

                            }

                            hflist.ForEach(x => context.HalfDays.Add(x));

                            context.SaveChanges();
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
                LD.ReturnResult = "Server Is Not Responding...";
                LD.FailureReason = "";
            }
            var FinalResult = new
            {
                type = LD.ReturnType,
                reason = LD.FailureReason,
                msg = LD.ReturnResult,
            };

            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddMenu(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int MenuId, string MenuName, string FormName, int SortNumber, bool IsActive, string IconName)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        if (MenuId == 0)
                        {
                            Menu m = new Menu();

                            m.MenuName = MenuName;
                            m.FormName = FormName;
                            m.SortNumber = SortNumber;
                            m.IconName = IconName;
                            m.IsActive = IsActive;
                            context.Menus.Add(m);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddUserLocations(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserId, int SelectedUserDeviceId, List<int> UpdatedLocation)
        {


            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        UpdatedLocation = (from l in context.Locations.Where(l => l.IsDeleted == false) select l.LocationId).ToList();

                        var deleteUL = context.UserLocations.Where(x => (x.UserId == SelectedUserId) && (x.UserDeviceId == SelectedUserDeviceId)).ToList();

                        deleteUL.ForEach(x => context.UserLocations.Remove(x));

                        var insertUL = (from p in UpdatedLocation select new UserLocation { UserId = SelectedUserId, UserDeviceId = SelectedUserDeviceId, LocationId = p, CreatedBy = LD.UserId, CreatedDate = DateTime.Now }).ToList();
                        insertUL.ForEach(x => context.UserLocations.Add(x));
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddUserRights(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserId, List<int> UpdatedRight)
        {
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var deleteUR = context.UserRights.Where(x => x.UserId == SelectedUserId).ToList();
                        deleteUR.ForEach(x => context.UserRights.Remove(x));

                        var insertUR = (from p in UpdatedRight select new UserRight { UserId = SelectedUserId, MenuId = p, }).ToList();
                        insertUR.ForEach(x => context.UserRights.Add(x));
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ApproveLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int ApprovalId, int LeaveId, int LeaveApprovalLevel, bool IsApprove, string Description)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                            context.UserLeaveApprovals.Add(ula);
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
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ApproveUserDevice(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UDId, int SelectedCompanyId, bool IsApproved)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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

                                context.SaveChanges();
                                LD.ReturnType = "success";
                                LD.ReturnResult = "Device has been Approved Sucessfully";
                                LD.FailureReason = "";



                            }
                            else
                            {
                                URDetail.ApprovedBy = -1;
                                URDetail.ApprovedDate = null;
                                context.SaveChanges();
                                LD.ReturnType = "success";
                                LD.ReturnResult = "Device has been disapproved Sucessfully";
                                LD.FailureReason = "";
                            }

                            try
                            {

                                #region SendEmailDeviceApproval
                                string BodyText = "";
                                if (File.Exists(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Device-Approval.html")))
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

                                    StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Device-Approval.html"));
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

                            context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage CheckCompanyStatus(string CompanyName)
        {
            var CompanyStatus = false;
            var CompanyId = 0;
            var CName = "";
            try
            {
                //check Username Password with device
                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage DeleteBreakCategory(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakCategoryId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var BGDetail = (from bt in context.BreakCategories
                                        where bt.BreakCategoryId == BreakCategoryId
                                        select bt).FirstOrDefault();

                        context.BreakCategories.Remove(BGDetail);
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public void DeleteDownloadExcel(string FileName)
        {
            var filepath = HttpContext.Current.Server.MapPath(FileName);
            System.IO.File.Delete(filepath);
        }


        public HttpResponseMessage DeleteHoliday(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int HolidayId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var HDetail = (from h in context.Holidays
                                       where h.Id.Equals(HolidayId)
                                       select h).FirstOrDefault();
                        context.Holidays.Remove(HDetail);
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage DeleteLocation(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int LocationId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var LDetail = (from l in context.Locations
                                       where l.LocationId.Equals(LocationId)
                                       select l).FirstOrDefault();
                        LDetail.IsDeleted = true;
                        //context.Locations.DeleteOnSubmit(LDetail);
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage DeleteMenu(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int MenuId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var MDetail = (from m in context.Menus
                                       where m.MId.Equals(MenuId)
                                       select m).FirstOrDefault();
                        context.Menus.Remove(MDetail);
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage DeleteUser(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UserId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var URDetail = (from c in context.Users
                                        where c.UserId == UserId
                                        select c).FirstOrDefault();

                        URDetail.Status = 1;
                        URDetail.UpdatedDate = DateTime.Now;
                        URDetail.UpdatedBy = LD.UserId;
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage DeleteUserDevice(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UDId)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var URDetail = (from c in context.UserDevices
                                        where c.UDId == UDId
                                        select c).FirstOrDefault();
                        URDetail.IsDeleted = true;
                        URDetail.IsApproved = false;
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetAssignLocationByUser(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            dynamic LDetail = null;

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetBreakCategoryAndLunchTimeAndTeaTime(bool BreakCategory, bool LunchAndTeaTime, int CompanyId)
        {
            List<BreakCategoryDetail> BGDetail = new List<BreakCategoryDetail>();
            dynamic LTTime = null;
            try
            {
                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetBreakCategoryDetailByBreakCategoryId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakCategoryId)
        {
            BreakCategoryDetail BGDetail = new BreakCategoryDetail();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetCompanyDefaultTimeAndLeaveDayAndWeekoffDay(int CompanyId)
        {
            CompanyDetail CMPDetail = new CompanyDetail();
            List<WeekoffDayDetail> UWDetail = new List<WeekoffDayDetail>();
            List<LeaveTypeCountDetail> LCDetail = new List<LeaveTypeCountDetail>();
            try
            {
                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetCompanyDetailByCompanyId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int EditCompanyId)
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
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetLeaveTypeCount(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            var LeaveTypeCountInText = "";
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetLeaveTypeDetail()
        {
            dynamic LTDetail = null;
            try
            {
                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetLocationByUserId(int UserId, int UserDeviceId, string DeviceType, int CompanyId)
        {
            List<LocationDetail> LDetail = new List<LocationDetail>();

            try
            {
                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetLocationDetailByLocationId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int LocationId)
        {
            LocationDetail LDetail = new LocationDetail();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetMenuDetailByMenuId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int EditMenuId)
        {
            MenuDetail MDetail = new MenuDetail();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetRoleAndCompany(int RoleId)
        {
            List<RoleDetail> RDetail = new List<RoleDetail>();
            dynamic CDetail = null;

            try
            {
                //check Username Password with device

                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetSecondLevelApprovalList(int SelectedRoleId, int CompanyId)
        {
            dynamic UDetail = null;
            try
            {
                //check Username Password with device

                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetUserDetailByUserId(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UserId)
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
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetUserHierachyWise(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<UserDetail> UDetail = new List<UserDetail>();
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetUserRights(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserId)
        {
            List<int> RDetail = new List<int>();
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListApprovalLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<LeaveDetail> LADetail = new List<LeaveDetail>();

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListBreakCategory(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<BreakCategoryDetail> BGDetail = new List<BreakCategoryDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListCompany(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<CompanyDetail> CMPDetail = new List<CompanyDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListHoliday(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<HolidayDetail> HDDetail = new List<HolidayDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListLeave(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<LeaveDetail> LDetail = new List<LeaveDetail>();
            int LeaveAprrovalTypeFlag = 3;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListRights(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<MenuDetail> MDetail = new List<MenuDetail>();
            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListUsersDevice(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<UserDeviceDetail> UDDetail = new List<UserDeviceDetail>();
            bool IsAllList = false;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage Login(string UserName, string Password, string DeviceId, string DeviceType, string AccessToken, string AppVersion)
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
                    using (var context = new EmployeeLoginEntities())
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
                                    context.SaveChanges();
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
                MDetail = MDetail,
                RoleId = RoleId,
                LDetail = LDetail,
                WTDetail = WTDetail,
                CompanyID = CompanyID,
                CompanyName = CompanyName

            };


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage RegistrationRequest(string UserName, string Password, string DeviceId, string DeviceType, string Description, string AccessToken, string OS, string DeviceName, string LocationName, string IPAddress)
        {
            var ReturnResult = "";
            var ReturnType = "success";
            try
            {
                var IsValidUser = 0;
                using (var context = new EmployeeLoginEntities())
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

                                context.UserDevices.Add(ud);
                                context.SaveChanges();
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

                                    context.SaveChanges();
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
                                        context.SaveChanges();

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

                                    context.UserDevices.Add(ud);
                                    context.SaveChanges();
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

                                        context.SaveChanges();

                                        ReturnType = "success";
                                        SendEmailforDeviceApproval(UDI);
                                        ReturnResult = "Registration request sent sucessfully. Contact Administrator to approve your device.";
                                    }
                                    else
                                    {
                                        UDI.RequestDate = DateTime.Now;
                                        UDI.Description = Description != "" ? Description : null;

                                        context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }



        public HttpResponseMessage AddGroup(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int GroupId, string GroupName, string Description, string GroupCode, bool IsActive)
        {
            GroupMaster mdetail = new GroupMaster();
            try
            {
                int UserID = -1;
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                            context.GroupMasters.Add(g);
                            context.SaveChanges();

                            LD.ReturnResult = "Group " + GroupName + " added sucessfully.";


                            int AdminGrpID = (from u in context.GroupMasters where (u.CompanyID == CompanyId) orderby u.ID ascending select u.ID).FirstOrDefault();

                            AddGroupAccessID(UserName, Password, DeviceId, DeviceType, CompanyId, AdminGrpID, Convert.ToString(g.ID));
                        }
                        else
                        {
                            mdetail = (from g in context.GroupMasters where g.ID == GroupId select g).FirstOrDefault();

                            mdetail.GroupName = GroupName;
                            mdetail.GroupCode = GroupCode;
                            mdetail.Status = IsActive;
                            mdetail.CompanyID = CompanyId;
                            mdetail.Description = Description;
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListGroup(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<GroupDetail> UDDetail = new List<GroupDetail>();
            bool IsAllList = false;
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        public HttpResponseMessage AddWeeklyTimings(string UserName, string Password, string DeviceId, string DeviceType, int UserID, byte TimingFor, int ObjectId, string TimingType, string Day, byte DayType, DateTime TimeFrom, DateTime TimeUpto, TimeSpan WorkingHours, int Status, int CreatedBy, string IPAddress, int WTID)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                            context.WeeklyTimings.Add(wt);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddCompanyBranch(string UserName, string Password, string DeviceId, string DeviceType, int CmpBrID, int CompanyID, string BranchName, string BranchAddress, string BranchCode, byte BranchTypeId, int ParentBranchId, string City, string State, string Country, string Pincode, string Location, int CretedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                            context.CompanyBranches.Add(CB);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        #region ~Employeereg


        public HttpResponseMessage AddEmployeeAddress(string UserName, string Password, string DeviceId, string DeviceType, int EAID, int EmployeeID, string Address, string City, string State, string Pincode, int CreatedBy, string ImergencyCntNo, string ImergencyName, string CurrAddress, string CurrCity, string CurrState, string CurrPincode)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


                            context.EmployeeAddresses.Add(EA);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddEmployeeBankDetails(string UserName, string Password, string DeviceId, string DeviceType, int AccountId, int UserID, string BankAccountNo, string BankName, string BranchCode, string IFSCode, byte Status, int CreatedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                            context.EmployeeBankDetails.Add(EB);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddEmployeeEducationDetails(string UserName, string Password, string DeviceId, string DeviceType, int EduId, int UserID, string Degree, string Institute, string University, int YearOfPassing, byte GradeType, string Grade, byte CourseType, int CreatedBy, string IpAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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

                            context.EmployeeEducationDetails.Add(Ed);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddEmployeeFamily(string UserName, string Password, string DeviceId, string DeviceType, int EFID, int UserID, string Name, DateTime DateOfBirth, string Relation, int CreatedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                            context.EmployeeFamilies.Add(EF);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddEmployeePersonalIDs(string UserName, string Password, string DeviceId, string DeviceType, int EFID, int UserID, int IDType, string IDNumber, string imageUrl, int CreatedBy, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                            context.EmployeePersonalIDs.Add(EP);
                            context.SaveChanges();

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
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddEmployeePersonaldetails(string UserName, string Password, string DeviceId, string DeviceType, int UserId, int CreatedBy, string IPAddress, UserPersonaInfo Usrpersonal)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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
                                var filepath = HttpContext.Current.Server.MapPath("Images\\UserProfile");

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
                        context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        #endregion


        public HttpResponseMessage ReportAllUsersMonthwiseExcel(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedCompanyId, string SelectedMonth)
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
                    using (var context = new EmployeeLoginEntities())
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
                        var filepath = HttpContext.Current.Server.MapPath("ExcelDownload");

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage CallReportAllUsersMonthwiseExcel(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedCompanyId, string SelectedMonth)
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
                    using (var context = new EmployeeLoginEntities())
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
                        var filepath = HttpContext.Current.Server.MapPath("ExcelDownload");

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }



        public HttpResponseMessage GetDashboardData(string UserName, string Password, string DeviceId, string DeviceType, int CompanyID, int UserID)
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

                    using (var context = new EmployeeLoginEntities())
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
                        UDetail.FinalRepPer = Accuracy;
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
                        PreviousUDetail.FinalRepPer = AccuracyPre;
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


                        TeamDetail.AddRange((from u in context.Users
                                             join ua in context.UserAccesses on u.UserId equals ua.UserID
                                             where ua.UserID == userID
                                             select new TeamAvailability()
                                             {
                                                 UserName = (from us in context.Users where us.UserId == ua.UserAccessID select us.FirstName + " " + us.LastName).FirstOrDefault(),
                                                 IsPunchin = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.UserId).Count() != 0 ? true : false,
                                                 LatePunchinReason = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.LatePunchinReason).FirstOrDefault(),
                                                 OutLocationReason = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinOutsideLocationReason).FirstOrDefault(),
                                                 PunchinTime = Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchinTime).FirstOrDefault()),
                                                 PunchoutTime = Convert.ToString((from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PunchoutTime).FirstOrDefault()),
                                                 Latlong = (from p in context.PunchIns where p.UserId == ua.UserAccessID && p.PunchinTime.Date == DateTime.Now.Date select p.PILatitudeLongitude).FirstOrDefault(),
                                                 DepartmentName = (from g in context.GroupMasters where g.ID == ((from uaa in context.Users where uaa.UserId == ua.UserAccessID select uaa.GroupID).FirstOrDefault()) select g.GroupName).FirstOrDefault(),
                                                 MobileNo = (from us in context.Users where us.UserId == ua.UserAccessID select us.MobileNoCmp).FirstOrDefault(),
                                             }).ToList());

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        //internal void SendEmailforDeviceApproval(UserDevice URDetail)
        //{
        //    try
        //    {
        //        using (var context = new EmployeeLoginEntities())
        //        {

        //            #region SendEmailDeviceApproval
        //            string BodyText = "";
        //            if (File.Exists(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Registration-Request.html")))
        //            {
        //                string email = "";
        //                string approvedby = "";
        //                string subject = "";

        //                string Name = (from u in context.Users where u.UserId == URDetail.UserId select u.FirstName + " " + u.LastName).FirstOrDefault();
        //                string UserName = (from u in context.Users where u.UserId == URDetail.UserId select u.UserName).FirstOrDefault();

        //                string company = (from c in context.Companies
        //                                  join u in context.Users on c.CompanyId equals u.CompanyID
        //                                  where u.UserId == URDetail.UserId
        //                                  select c.CompanyName).FirstOrDefault();


        //                subject = "[" + company + " +] - " + Name + " - Device Approval";


        //                StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Registration-Request.html"));
        //                BodyText = sr.ReadToEnd();
        //                sr.Close();
        //                BodyText = BodyText.Replace("##Username##", (from u in context.Users where u.UserId == URDetail.UserId select u.FirstName + " " + u.LastName).FirstOrDefault());
        //                BodyText = BodyText.Replace("##DeviceName##", URDetail.DeviceName);
        //                BodyText = BodyText.Replace("##DeviceID##", URDetail.DeviceId);
        //                BodyText = BodyText.Replace("##OSVersion##", URDetail.OSVersion);
        //                BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
        //                BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
        //                BodyText = BodyText.Replace("##RegistrationDate##", URDetail.RequestDate.ToString());
        //                BodyText = BodyText.Replace("##RegistrationTime##", URDetail.RequestDate.TimeOfDay.ToString());



        //                // GF.SendEmailNotification("NKTPL+ Device Status", UserName, BodyText, subject);
        //            }
        //            #endregion
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }


        //}

        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName, int j)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[j];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            return ws;
        }


        public HttpResponseMessage AddGroupAccessID(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int GroupID, string GroupAccessIDs)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {

                        //var GroupAccessID = (from u in context.GroupMasters where u.CompanyID == CompanyId orderby u.ID ascending select u.ID).FirstOrDefault();


                        //if (GroupAccessID != null && GroupAccessID != GroupID)
                        //{
                        //    GroupAccessIDs = GroupAccessIDs + "," + GroupAccessID;
                        //}


                        DeleteUserAccessAutomatically(GroupID);
                        var list = context.GroupAccesses.Where(c => c.GroupID == GroupID).ToList();

                        list.ForEach(x => context.GroupAccesses.Remove(x));
                        context.SaveChanges();

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
                                    context.GroupAccesses.Add(GDetail);
                                    context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        internal string AllUserAccessAutomatically(int GroupID)
        {
            using (var context = new EmployeeLoginEntities())
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
                            context.UserAccesses.Add(Ua);
                            context.SaveChanges();
                        }
                    }
                }
            }

            return "";
        }

        internal string DeleteUserAccessAutomatically(int GroupID)
        {
            using (var context = new EmployeeLoginEntities())
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
                            var list = context.UserAccesses.Where(c => c.UserAccessID == item3 && c.UserID == item2).ToList();
                            list.ForEach(x => context.UserAccesses.Remove(x));
                            context.SaveChanges();
                        }
                    }
                }
            }

            return "";
        }




        #region ~Userreg


        public HttpResponseMessage AddUser(string UserName, string Password, string NewUserName, string NewPassword, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int UserId, int GroupID, int RoleId, int TopId, string FirstName, string LastName, string MobileNoCmp, int Status, List<WeeklyTiming> Weektime, string WeekoffDays, string EmailID)
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

                        using (var context = new EmployeeLoginEntities())
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
                                        context.Users.Add(usr);
                                        context.SaveChanges();
                                        #endregion

                                        var Userde = context.Users.Where(u => u.UserId == usr.UserId).FirstOrDefault();
                                        var cmpname = (from cm in context.Companies where cm.CompanyId == Userde.CompanyID select cm.CompanyName).FirstOrDefault();
                                        Userde.UserCode = cmpname.Length > 5 ? cmpname.Substring(0, 5).ToUpper() + Userde.UserId.ToString() : cmpname + Userde.UserId.ToString();
                                        context.SaveChanges();





                                        #region AddWeekOff
                                        List<UserWeekOffDay> uwd = new List<UserWeekOffDay>();

                                        context.UserWeekOffDays.Where(x => (x.UserId == usr.UserId) && (x.EndDate == null)).ToList().ForEach(e => e.EndDate = DateTime.Now);


                                        var LstObj = JsonConvert.DeserializeObject<List<List<string>>>(WeekoffDays);
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
                                        //context.UserWeekOffDays.InsertAllOnSubmit(uwd);

                                        uwd.ToList().ForEach(x => context.UserWeekOffDays.Add(x));

                                        context.SaveChanges();


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
                                            context.WeeklyTimings.Add(Wtime);
                                            context.SaveChanges();
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
                                                context.UserAccesses.Add(Ua);
                                                context.SaveChanges();
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
                                                context.UserAccesses.Add(Ua);
                                                context.SaveChanges();
                                            }
                                        }
                                        #endregion

                                        LD.ReturnResult = "Username " + NewUserName + " added sucessfully.";

                                        #region SendEmailUsr
                                        string BodyText = "";
                                        if (File.Exists(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Welcome.html")))
                                        {

                                            StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Welcome.html"));
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


                                        context.SaveChanges();

                                        #endregion



                                        #region UpdateWeekOff

                                        List<UserWeekOffDay> uwd = new List<UserWeekOffDay>();

                                        context.UserWeekOffDays.Where(x => (x.UserId == UserDetail.UserId) && (x.EndDate == null)).ToList().ForEach(e => e.EndDate = DateTime.Now);


                                        var LstObj = JsonConvert.DeserializeObject<List<List<string>>>(WeekoffDays);
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
                                        //context.UserWeekOffDays.InsertAllOnSubmit(uwd);
                                        uwd.ForEach(x => context.UserWeekOffDays.Add(x));
                                        context.SaveChanges();


                                        #endregion

                                        #region UpdateWeekTiming

                                        //context.WeeklyTimings.DeleteAllOnSubmit(context.WeeklyTimings.Where(c => c.ObjectId == UserDetail.UserId && c.TimingFor == 3 && c.TimingType == "WorkHours"));
                                        context.WeeklyTimings.Where(c => c.ObjectId == UserDetail.UserId && c.TimingFor == 3 && c.TimingType == "WorkHours").ToList().ForEach(x => context.WeeklyTimings.Remove(x));
                                        context.SaveChanges();

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
                                            context.WeeklyTimings.Add(Wtime);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        #region AddUserAccess

                                        //context.UserAccesses.DeleteAllOnSubmit(context.UserAccesses.Where(c => c.UserID == UserDetail.UserId));
                                        //context.UserAccesses.DeleteAllOnSubmit(context.UserAccesses.Where(c => c.UserAccessID == UserDetail.UserId));

                                        context.UserAccesses.Where(c => c.UserID == UserDetail.UserId).ToList().ForEach(x => context.UserAccesses.Remove(x));
                                        context.SaveChanges();
                                        context.UserAccesses.Where(c => c.UserAccessID == UserDetail.UserId).ToList().ForEach(x => context.UserAccesses.Remove(x));
                                        context.SaveChanges();


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
                                                context.UserAccesses.Add(Ua);
                                                context.SaveChanges();
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
                                                context.UserAccesses.Add(Ua);
                                                context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage AddUserComponent(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string ComponentStr, int UserId)
        {
            try
            {

                // List<WeeklyTiming> weektime = JsonConvert.DeserializeObject<List<WeeklyTiming>>(Weektime);

                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {

                    using (var context = new EmployeeLoginEntities())
                    {

                        //context.UserComponentRights.DeleteAllOnSubmit(context.UserComponentRights.Where(c => c.UserId == UserId));
                        context.UserComponentRights.Where(c => c.UserId == UserId).ToList().ForEach(x => context.UserComponentRights.Remove(x));
                        context.SaveChanges();

                        string[] arryComponentStr = ComponentStr.Split(',');

                        foreach (var item in arryComponentStr)
                        {
                            UserComponentRight uc = new UserComponentRight();
                            uc.UserId = UserId;
                            uc.ComponentId = Convert.ToInt32(item);
                            context.UserComponentRights.Add(uc);
                            context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }



        public HttpResponseMessage ListMenu(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<MenuDetail> MDetail = new List<MenuDetail>();

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListComponent(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        {
            List<MenuWithComponentDetail> CMPDetail = new List<MenuWithComponentDetail>();

            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }



        public HttpResponseMessage ListComponentWithUserID(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int UserID)
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
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage UpdateComponent(string UserName, string Password, string DeviceId, string DeviceType, int UserId, string ComponentList)
        {


            try
            { //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        string[] str_arr = ComponentList.Split(',').ToArray();
                        int[] int_arr = Array.ConvertAll(str_arr, Int32.Parse);

                        foreach (var item in int_arr)
                        {
                            var uc = new UserComponentRight();
                            uc.UserId = UserId;
                            uc.ComponentId = item;
                            context.UserComponentRights.Add(uc);
                            context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetTopUserAndLeaveApprovalType(int RoleId, int SelectedRoleId, int CompanyId, int UserId)
        {
            dynamic UDetail = null;
            List<LeaveApprovalTypeDetail> LATDetail = new List<LeaveApprovalTypeDetail>();
            try
            {
                //check Username Password with device
                LD.ReturnType = "success";
                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage GetTopUserList(int GroupId, int CompanyId)
        {
            dynamic UDetail = null;
            try
            {
                //check Username Password with device
                LD.ReturnType = "success";

                using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListUsers(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int GroupID, string AppVersion)
        {
            List<UserDetail> URDetail = new List<UserDetail>();

            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ListUserByID(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int UserId, string AppVersion)
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
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }




        public HttpResponseMessage UserDetailsDateWise(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserID, int SelectedCompanyId, string SelectedDate)
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
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage UserDetailsMonth(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int SelectedUserID, int SelectedCompanyId, string SelectedMonth)
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
                    using (var context = new EmployeeLoginEntities())
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }

        #endregion

        #region Common
        internal void SendEmailforDeviceApproval(UserDevice URDetail)
        {
            try
            {
                using (var context = new EmployeeLoginEntities())
                {

                    #region SendEmailDeviceApproval
                    string BodyText = "";
                    if (File.Exists(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Registration-Request.html")))
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


                        StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~\\MailTemplates\\Registration-Request.html"));
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

        #region Passwordreg


        public HttpResponseMessage ChangePassword(string UserName, string Password, string DeviceId, string DeviceType, string NewPassword, int UserID, string IPAddress)
        {
            try
            {
                //check Username Password with device
                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

                if (LD.ReturnType == "success")
                {
                    using (var context = new EmployeeLoginEntities())
                    {
                        var EIDdetail = (from wt in context.Users where wt.UserName == UserName select wt).FirstOrDefault();
                        EIDdetail.Password = NewPassword;
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }


        public HttpResponseMessage ForgotPassword(string DeviceId, string DeviceType, string Str, string IPAddress, string latlong)
        {
            long OTPID = 0;
            try
            {
                //check Username Password with device



                using (var context = new EmployeeLoginEntities())
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
                            context.UsersOTPs.Add(ut);
                            context.SaveChanges();
                            string msg = "call rem Your OTP is " + ut.OTP + ",,Assgn:,Remarks:";
                            //    var resp = GF.SMSSend(Convert.ToString(ConfigurationManager.AppSettings["SMSUid"]), Convert.ToString(ConfigurationManager.AppSettings["SMSKey"]), Convert.ToString(ConfigurationManager.AppSettings["SMSSenderId"]), EIDdetail.MobileNoCmp, msg);
                            var resp = "sms sent";
                            if (resp.ToLower().Contains("sms sent"))
                            {
                                OTPID = ut.ID;
                                var uut = (from uo in context.UsersOTPs where uo.ID == ut.ID select uo).FirstOrDefault();
                                uut.OTPSent = true;
                                ut.OTPSentTime = DateTime.Now;
                                context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }



        public HttpResponseMessage ReceiveOTP(string DeviceId, string DeviceType, long OTP, string IPAddress, int OTPID)
        {
            long UserID = 0;
            try
            {
                using (var context = new EmployeeLoginEntities())
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

                    context.SaveChanges();

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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }



        public HttpResponseMessage resetPassword(string NewPassword, int UserID)
        {
            try
            {
                //check Username Password with device


                using (var context = new EmployeeLoginEntities())
                {
                    var EIDdetail = (from wt in context.Users where wt.UserId == UserID select wt).FirstOrDefault();

                    if (EIDdetail != null)
                    {
                        EIDdetail.Password = NewPassword;
                        context.SaveChanges();
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


            return Request.CreateResponse(HttpStatusCode.OK, FinalResult);
        }



        public string SSendSMS(string uid, string pwd, string gsmsenderid, string mob, string msg)
        {
            var abc = GF.SMSSend(uid, pwd, gsmsenderid, mob, msg);
            return abc;
        }

        #endregion

        #endregion 
    }

}
