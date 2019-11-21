using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace EmployeeLoginService
{
    public class OldFunctions
    {
        //        [WebMethod]
        //        public string ListUsers(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, int GroupID, string AppVersion)
        //        {
        //            List<UserDetail> URDetail = new List<UserDetail>();

        //            try
        //            {
        //                //check Username Password with device
        //                LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

        //                if (LD.ReturnType == "success")
        //                {
        //                    using (var context = new ELDBDataContext())
        //                    {
        //                        if (GroupID != -1)
        //                        {
        //                            var UDetail = context.Users
        //                        .Where(c => (c.UserId == LD.UserId) && (c.Status == 1) && c.GroupID == GroupID)
        //                         .Select(c => new
        //                         {
        //                             UserId = c.UserId,
        //                             UserName = c.FirstName+" "+c.LastName ,
        //                             TopId = c.TopId,
        //                             GroupID=c.GroupID,
        //                             TopName = (from u in context.Users where u.UserId == c.TopId select u.UserName).FirstOrDefault(),
        //                             RoleName = (from r in context.RoleMasters where r.RoleId == c.RoleId select r.RoleName).FirstOrDefault(),
        //                             DeviceCount = (from ud in context.UserDevices where ud.UserId == c.UserId && ud.IsDeleted == false select ud.UserId).Count(),
        //                             CompanyName = (from ic in context.Companies where ic.CompanyId == c.CompanyID select ic.CompanyName).FirstOrDefault(),
        //                             GroupName = (from g in context.GroupMasters where g.ID == c.GroupID select g.GroupName).FirstOrDefault(),
        //                             Weekoff = string.Join(",", (context.UserWeekOffDays.Where(w => (w.UserId == c.UserId) && (w.EndDate == null)).Select(s => s.WeekOffDay + "(" + (s.AlternativeDay == null ? "All" : s.AlternativeDay) + ")"))),
        //                             Children = GF.GetChildren(context, c.UserId)
        //                         })
        //                         .AsEnumerable()
        //                        .Select(c => new HierarchyUserDetail()
        //                        {
        //                            UserId = c.UserId,
        //                            UserName = c.UserName,
        //                            TopId = c.TopId,
        //                            TopName = c.TopName,
        //                            GroupId = c.GroupID,
        //                            RoleName = c.RoleName,
        //                            DeviceCount = c.DeviceCount,
        //                            CompanyName = c.CompanyName,
        //                            GroupName = c.GroupName,
        //                            Weekoff = c.Weekoff == "" ? "Not Available" : c.Weekoff,
        //                            Children = c.Children,
        //                        }).ToList();
        //                            GF.HieararchyWalk(UDetail, URDetail);
        //                        }
        //                        else
        //                        {
        //                            var UDetail = context.Users
        //                          .Where(c => (c.UserId == LD.UserId) && (c.Status == 1))
        //                           .Select(c => new
        //                           {
        //                               UserId = c.UserId,
        //                               UserName = c.FirstName+" "+c.LastName,
        //                               TopId = c.TopId,
        //                               GroupID = c.GroupID,
        //                               GroupName = (from g in context.GroupMasters where g.ID == c.GroupID select g.GroupName).FirstOrDefault(),
        //                               TopName = (from u in context.Users where u.UserId == c.TopId select u.UserName).FirstOrDefault(),
        //                               RoleName = (from r in context.RoleMasters where r.RoleId == c.RoleId select r.RoleName).FirstOrDefault(),
        //                               DeviceCount = (from ud in context.UserDevices where ud.UserId == c.UserId && ud.IsDeleted == false select ud.UserId).Count(),
        //                               CompanyName = (from ic in context.Companies where ic.CompanyId == c.CompanyID select ic.CompanyName).FirstOrDefault(),
        //                               LeaveApprovalTypeName = (from la in context.LeaveApprovalTypes where la.LeaveApprovalTypeId == c.LeaveApprovalTypeId select la.LeaveApprovalTypeName).FirstOrDefault(),
        //                               LeaveApprovalSecondLevelName = (from u in context.Users where u.UserId == c.LeaveApprovalSecondLevelId select u.UserName).FirstOrDefault(),
        //                               Weekoff = string.Join(",", (context.UserWeekOffDays.Where(w => (w.UserId == c.UserId) && (w.EndDate == null)).Select(s => s.WeekOffDay + "(" + (s.AlternativeDay == null ? "All" : s.AlternativeDay) + ")"))),
        //                               Children = GF.GetChildren(context, c.UserId)
        //                           })
        //                           .AsEnumerable()
        //                          .Select(c => new HierarchyUserDetail()
        //                          {
        //                              UserId = c.UserId,
        //                              UserName = c.UserName,
        //                              TopId = c.TopId,
        //                              TopName = c.TopName,
        //                              GroupId = c.GroupID,
        //                              RoleName = c.RoleName,
        //                              DeviceCount = c.DeviceCount,
        //                              CompanyName = c.CompanyName,
        //                              GroupName = c.GroupName,
        //                              LeaveApprovalTypeName = c.LeaveApprovalTypeName,
        //                              LeaveApprovalSecondLevelName = c.LeaveApprovalSecondLevelName,
        //                              Weekoff = c.Weekoff == "" ? "Not Available" : c.Weekoff,
        //                              Children = c.Children,
        //                          }).ToList();
        //                            GF.HieararchyWalk(UDetail, URDetail);
        //                        }


        ////                        EmployeeAddress
        ////EmployeeBankDetails
        ////EmployeeEducationDetails
        ////EmployeeFamily
        ////EmployeePersonalIDs


        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                LD.ReturnType = "failure";
        //                LD.ReturnResult = "Server Is Not Responding...";
        //                LD.FailureReason = "";
        //            }
        //            var FinalResult = new
        //            {
        //                URDetail = URDetail,
        //                type = LD.ReturnType,
        //                reason = LD.FailureReason,
        //                msg = LD.ReturnResult,
        //            };

        //            JavaScriptSerializer jss = new JavaScriptSerializer();
        //            return jss.Serialize(FinalResult);
        //        }

        // Code Done By Hardik Tur

        //[WebMethod]
        //public string AddEmployee(string FirstName,string MiddleName,string Lastname,string Gender,string DOB,string DOBFather,string DOBMother,string MaritalStatus,string AnniversaryDate,
        //    string SpouseName,string DOBSpouce, string DegreeName,string Grade,string Passingyear,string Degreedesc,string ProfileImg,string EmpType,string ReportingTo,string CompanyEmail,
        //    int CmpID,int BranchID,string CmpJoinDate,string CmpLeaveDate,string SalaryACNo,string BankBranch,string BankIFSC,string PANNO,string AdharCardNo,string DLNo,string PassportNo,
        //    string VoterID,int createdBy,string CreatedDate,int UpdatedBy,string Updateddate)

        //{
        //    //Changes Done By Hardik Tur
        //    try
        //    {
        //        using (TransactionScope ts = new TransactionScope())
        //        {
        //            //check Username Password with device
        //            LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

        //            if (LD.ReturnType == "success")
        //            {
        //                using (var context = new ELDBDataContext())
        //                {
        //                    var IsExistUser = (from u in context.Users where u.UserName == NewUserName && u.CompanyId == UserCompanyId && u.IsDeleted == false select u);

        //                    var ValidUser = true;
        //                    if (IsExistUser.Count() > 0)
        //                    {
        //                        if (UserId == 0)
        //                            ValidUser = false;
        //                        else if (IsExistUser.First().UserId != UserId)
        //                            ValidUser = false;
        //                    }

        //                    if (ValidUser == false)
        //                    {
        //                        LD.ReturnType = "failure";
        //                        LD.ReturnResult = "Username " + NewUserName + " is already exists.";
        //                        LD.FailureReason = "";
        //                    }
        //                    else
        //                    {
        //                        if (UserId == 0)
        //                        {
        //                            var CurrentActiveUser = context.Users.Where(w => (w.CompanyId == UserCompanyId) && (w.IsDeleted == false)).Count();
        //                            var UserLimit = context.Companies.Where(w => (w.CompanyId == UserCompanyId)).FirstOrDefault().UserLimit;
        //                            if (CurrentActiveUser < UserLimit)
        //                            {
        //                                User usr = new User();

        //                                usr.UserName = NewUserName;
        //                                usr.Password = NewPassword;
        //                                usr.RoleId = RoleId;
        //                                usr.TopId = TopId;
        //                                usr.CompanyId = UserCompanyId;
        //                                usr.IsDeleted = false;
        //                                usr.DefaultTimeFrom = Convert.ToDateTime(DefaultFrom);
        //                                usr.DefaultTimeTo = Convert.ToDateTime(DefaultTo);
        //                                usr.WorkHour = TimeSpan.FromHours(WorkHour) + TimeSpan.FromMinutes(WorkMinute);
        //                                usr.LeaveApprovalTypeId = LeaveApprovalTypeId;
        //                                usr.LeaveApprovalSecondLevelId = SecondLevelApprovalId != 0 ? SecondLevelApprovalId : (int?)null;
        //                                usr.CreatedBy = LD.UserId;
        //                                usr.CreatedDate = DateTime.Now;
        //                                usr.UpdatedBy = LD.UserId;
        //                                usr.UpdatedDate = DateTime.Now;
        //                                usr.EmployeeID = LD.EmployeeID;
        //                                context.Users.InsertOnSubmit(usr);
        //                                context.SubmitChanges();

        //                                List<UserWeekOffDay> uwd = new List<UserWeekOffDay>();

        //                                JavaScriptSerializer js = new JavaScriptSerializer();
        //                                var LstObj = js.Deserialize<List<List<string>>>(WeekoffDays);
        //                                foreach (var LO in LstObj)
        //                                {
        //                                    var AD = String.Join(",", LO.GetRange(2, LO.Count() - 2));

        //                                    uwd.Add(new UserWeekOffDay
        //                                    {
        //                                        UserId = usr.UserId,
        //                                        WeekOffDay = LO[0],
        //                                        IsAlternative = Convert.ToBoolean(LO[1]),
        //                                        AlternativeDay = AD != "" ? AD : null,
        //                                        CreatedBy = LD.UserId,
        //                                        CreatedDate = DateTime.Now,
        //                                    });
        //                                }
        //                                context.UserWeekOffDays.InsertAllOnSubmit(uwd);

        //                                List<UserLeaveTypeCount> cltc = new List<UserLeaveTypeCount>();

        //                                var clLstObj = js.Deserialize<List<List<string>>>(LeaveDays);
        //                                foreach (var LO in clLstObj)
        //                                {
        //                                    cltc.Add(new UserLeaveTypeCount
        //                                    {
        //                                        UserId = usr.UserId,
        //                                        LeaveTypeId = Convert.ToInt32(LO[0]),
        //                                        LeaveCountByType = Convert.ToInt32(LO[1]),
        //                                        CreatedBy = LD.UserId,
        //                                        CreatedDate = DateTime.Now,
        //                                        UpdatedBy = LD.UserId,
        //                                        UpdatedDate = DateTime.Now,
        //                                    });
        //                                }
        //                                context.UserLeaveTypeCounts.InsertAllOnSubmit(cltc);

        //                                List<int> UpdatedRight = new List<int>();
        //                                if (usr.RoleId == 2)
        //                                {
        //                                    UpdatedRight = new List<int>() { 1, 3, 4, 6, 8, 9, 10, 11, 12, 13, 14 };
        //                                }
        //                                else if (usr.RoleId == 3)
        //                                {
        //                                    UpdatedRight = new List<int>() { 9, 10, 11, 12, 14 };
        //                                }
        //                                else if (usr.RoleId == 4)
        //                                {
        //                                    UpdatedRight = new List<int>() { 9, 11, 12 };
        //                                }

        //                                var insertUR = (from p in UpdatedRight select new UserRight { UserId = usr.UserId, MenuId = p, });
        //                                context.UserRights.InsertAllOnSubmit(insertUR);

        //                                context.SubmitChanges();

        //                                LD.ReturnResult = "Username " + NewUserName + " added sucessfully.";
        //                            }
        //                            else
        //                            {
        //                                LD.ReturnType = "failure";
        //                                LD.ReturnResult = "Users limit is over.";
        //                                LD.FailureReason = "";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //code for when change role top to bottem side
        //                            var UserDetail = (from u in context.Users where u.UserId == UserId select u).FirstOrDefault();
        //                            var TotalChild = (from u in context.Users where u.TopId == UserId select u).Count();
        //                            var ValidForEdit = true;
        //                            if ((UserDetail.RoleId < RoleId) && (TotalChild > 0))
        //                            {
        //                                ValidForEdit = false;

        //                                LD.ReturnType = "failure";
        //                                LD.ReturnResult = TotalChild + " children available of " + UserDetail.UserName + ". Update children first.";
        //                                LD.FailureReason = "";
        //                            }

        //                            if (ValidForEdit)
        //                            {
        //                                UserDetail.UserName = NewUserName;
        //                                UserDetail.Password = NewPassword;
        //                                UserDetail.RoleId = RoleId;
        //                                UserDetail.TopId = TopId;
        //                                UserDetail.CompanyId = UserCompanyId;
        //                                UserDetail.DefaultTimeFrom = Convert.ToDateTime(DefaultFrom);
        //                                UserDetail.DefaultTimeTo = Convert.ToDateTime(DefaultTo);
        //                                UserDetail.UpdatedBy = LD.UserId;
        //                                UserDetail.UpdatedDate = DateTime.Now;
        //                                UserDetail.WorkHour = TimeSpan.FromHours(WorkHour) + TimeSpan.FromMinutes(WorkMinute);
        //                                UserDetail.LeaveApprovalTypeId = LeaveApprovalTypeId;
        //                                UserDetail.LeaveApprovalSecondLevelId = SecondLevelApprovalId != 0 ? SecondLevelApprovalId : (int?)null;
        //                                context.SubmitChanges();

        //                                List<UserWeekOffDay> uwd = new List<UserWeekOffDay>();

        //                                context.UserWeekOffDays.Where(x => (x.UserId == UserDetail.UserId) && (x.EndDate == null)).ToList().ForEach(e => e.EndDate = DateTime.Now);

        //                                JavaScriptSerializer js = new JavaScriptSerializer();
        //                                var LstObj = js.Deserialize<List<List<string>>>(WeekoffDays);
        //                                foreach (var LO in LstObj)
        //                                {
        //                                    var AD = String.Join(",", LO.GetRange(2, LO.Count() - 2));
        //                                    var UpdateDetail = context.UserWeekOffDays.Where(x => (x.UserId == UserDetail.UserId) && (x.EndDate == null) &&
        //                                              (x.WeekOffDay == LO[0]) &&
        //                                              (Convert.ToBoolean(LO[1]) == true ? ((x.IsAlternative == Convert.ToBoolean(LO[1])) && (x.AlternativeDay == AD)) : (x.IsAlternative == Convert.ToBoolean(LO[1]))));

        //                                    if (UpdateDetail.Count() > 0)
        //                                    {
        //                                        UpdateDetail.FirstOrDefault().EndDate = null;
        //                                    }
        //                                    else
        //                                    {
        //                                        uwd.Add(new UserWeekOffDay
        //                                        {
        //                                            UserId = UserDetail.UserId,
        //                                            WeekOffDay = LO[0],
        //                                            IsAlternative = Convert.ToBoolean(LO[1]),
        //                                            AlternativeDay = AD != "" ? AD : null,
        //                                            CreatedBy = LD.UserId,
        //                                            CreatedDate = DateTime.Now,
        //                                        });
        //                                    }
        //                                }
        //                                context.UserWeekOffDays.InsertAllOnSubmit(uwd);

        //                                var clLstObj = js.Deserialize<List<List<string>>>(LeaveDays);
        //                                foreach (var LO in clLstObj)
        //                                {
        //                                    var record = context.UserLeaveTypeCounts.Where(w => w.UserId == UserDetail.UserId && w.LeaveTypeId == Convert.ToInt32(LO[0]));
        //                                    if (record.Count() > 0)
        //                                    {
        //                                        var updateRecord = record.FirstOrDefault();
        //                                        updateRecord.LeaveCountByType = Convert.ToInt32(LO[1]);
        //                                        updateRecord.UpdatedBy = LD.UserId;
        //                                        updateRecord.UpdatedDate = DateTime.Now;
        //                                    }
        //                                    else
        //                                    {
        //                                        UserLeaveTypeCount cltc = new UserLeaveTypeCount();

        //                                        cltc.UserId = UserDetail.UserId;
        //                                        cltc.LeaveTypeId = Convert.ToInt32(LO[0]);
        //                                        cltc.LeaveCountByType = Convert.ToInt32(LO[1]);
        //                                        cltc.CreatedBy = cltc.UpdatedBy = LD.UserId;
        //                                        cltc.CreatedDate = cltc.UpdatedDate = DateTime.Now;
        //                                        context.UserLeaveTypeCounts.InsertOnSubmit(cltc);
        //                                    }
        //                                    context.SubmitChanges();
        //                                }

        //                                context.SubmitChanges();

        //                                LD.ReturnResult = "Username " + NewUserName + " update sucessfully.";
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            ts.Complete();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LD.ReturnType = "failure";
        //        LD.ReturnResult = "Server Is Not Responding...";
        //        LD.FailureReason = "";
        //    }

        //    var FinalResult = new
        //    {
        //        type = LD.ReturnType,
        //        reason = LD.FailureReason,
        //        msg = LD.ReturnResult,
        //    };

        //    JavaScriptSerializer jss = new JavaScriptSerializer();
        //    return jss.Serialize(FinalResult);
        //}

        //[WebMethod]
        //public string ImageTest(Stream f, string fileName)
        //{
        //    try
        //    {
        //        MemoryStream ms = new MemoryStream(f);

        //        FileStream fs = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath
        //        ("~/abc/") + fileName, FileMode.Create);

        //        ms.WriteTo(fs);

        //        ms.Close();
        //        fs.Close();
        //        fs.Dispose();

        //        return "OK";
        //    }
        //    catch (Exception ex)
        //    {
        //        // return the error message if the operation fails
        //        return ex.Message.ToString();
        //    }
        //}

        //[WebMethod]
        //public string BreakInOut(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion, int BreakId, bool BreakInOutType, int LocationId, string LatitudeLongitude, string OutsideReason)
        //{
        //    bool BreakComplete = true;
        //    var BD = new BreakDetail();
        //    var ValidBreak = true;
        //    var BreakTypeId = 3;
        //    try
        //    {
        //        //check Username Password with device
        //        LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

        //        if (LD.ReturnType == "success")
        //        {
        //            using (var context = new ELDBDataContext())
        //            {
        //                var dbAppVersion = (from av in context.VersionMasters
        //                                    select DeviceType == "PC" ? av.PCVersion : DeviceType == "Android" ? av.AndroidVersion : av.IOSVersion).FirstOrDefault();

        //                if (AppVersion == (dbAppVersion == null ? "0" : dbAppVersion.ToString()))
        //                {
        //                    if ((from p in context.PunchIns where p.UserId == LD.UserId && p.PunchinTime.Date == DateTime.Today.Date select p.PId).Count() > 0)
        //                    {
        //                        if (BreakId == 0)
        //                        {
        //                            var CheckBreakExist = (from b in context.Breaks
        //                                                   where b.UserId == LD.UserId && b.BreakinTime.Date == DateTime.Now.Date && b.BreakoutTime == null
        //                                                   select b.BreakId).Count();
        //                            if (CheckBreakExist == 0)
        //                            {
        //                                // break in approved location
        //                                if (BreakInOutType)
        //                                {
        //                                    var LocationDetail = (from l in context.Locations where l.LocationId == LocationId select l).FirstOrDefault();
        //                                    if (LocationDetail != null)
        //                                    {
        //                                        if (LocationDetail.BreakCategoryId == 1) // 1 - fix break
        //                                        {
        //                                            var breakCount = context.Breaks.Where(p => p.UserId == LD.UserId && p.BreakinTime.Date == DateTime.Today);

        //                                            //var breakCount = (from b in context.Breaks
        //                                            //                  where b.UserId == LD.UserId && b.BreakinTime.Date == DateTime.Today
        //                                            //                  select b);
        //                                            if (breakCount.Count() >= LocationDetail.TotalBreakCount)
        //                                            {
        //                                                ValidBreak = false;
        //                                                LD.ReturnType = "failure";
        //                                                LD.ReturnResult = "Your all break is over.";
        //                                                LD.FailureReason = "";
        //                                            }
        //                                            else
        //                                            {
        //                                                var lunchBreakCount = 0;
        //                                                var teaBreakCount = 0;

        //                                                if (breakCount.Count() > 0)
        //                                                {
        //                                                    lunchBreakCount = breakCount.Where(a => a.BreakTypeId == 1).Count();
        //                                                    teaBreakCount = breakCount.Where(a => a.BreakTypeId == 2).Count();
        //                                                }

        //                                                if (lunchBreakCount == 0 && DateTime.Now.TimeOfDay >= LocationDetail.LunchTimeFrom.TimeOfDay && DateTime.Now.TimeOfDay <= LocationDetail.LunchTimeTo.TimeOfDay)
        //                                                {
        //                                                    BreakTypeId = 1;
        //                                                }
        //                                                else if (teaBreakCount == 0 && DateTime.Now.TimeOfDay >= LocationDetail.TeaTimeFrom.TimeOfDay && DateTime.Now.TimeOfDay <= LocationDetail.TeaTimeTo.TimeOfDay)
        //                                                {
        //                                                    BreakTypeId = 2;
        //                                                }
        //                                                else
        //                                                {
        //                                                    if (lunchBreakCount == 1 && teaBreakCount == 1)
        //                                                    {
        //                                                        ValidBreak = false;
        //                                                        LD.ReturnType = "failure";
        //                                                        LD.ReturnResult = "Your all break are over";
        //                                                        LD.FailureReason = "";
        //                                                    }
        //                                                    else if (lunchBreakCount == 1 && DateTime.Now.TimeOfDay >= LocationDetail.LunchTimeFrom.TimeOfDay && DateTime.Now.TimeOfDay <= LocationDetail.LunchTimeTo.TimeOfDay)
        //                                                    {
        //                                                        ValidBreak = false;
        //                                                        LD.ReturnType = "failure";
        //                                                        LD.ReturnResult = "You already take lunch break";
        //                                                        LD.FailureReason = "";
        //                                                    }
        //                                                    else if (teaBreakCount == 1 && DateTime.Now.TimeOfDay >= LocationDetail.TeaTimeFrom.TimeOfDay && DateTime.Now.TimeOfDay <= LocationDetail.TeaTimeTo.TimeOfDay)
        //                                                    {
        //                                                        ValidBreak = false;
        //                                                        LD.ReturnType = "failure";
        //                                                        LD.ReturnResult = "You already take tea break";
        //                                                        LD.FailureReason = "";
        //                                                    }
        //                                                    else
        //                                                    {
        //                                                        ValidBreak = false;
        //                                                        LD.ReturnType = "failure";
        //                                                        LD.ReturnResult = "Your lunch break time between " + LocationDetail.LunchTimeFrom.ToString("hh:mm tt") + " and " + LocationDetail.LunchTimeTo.ToString("hh:mm tt") +
        //                                                                            "\n" + "and tea break time between " + LocationDetail.TeaTimeFrom.ToString("hh:mm tt") + " and " + LocationDetail.TeaTimeTo.ToString("hh:mm tt");
        //                                                        LD.FailureReason = "";
        //                                                    }
        //                                                }
        //                                            }

        //                                            if (breakCount.Count() == 0)
        //                                            {
        //                                                if (DateTime.Now >= LocationDetail.LunchTimeFrom && DateTime.Now <= LocationDetail.LunchTimeTo)
        //                                                {
        //                                                    BreakTypeId = 1;
        //                                                }
        //                                                else
        //                                                {
        //                                                }
        //                                            }
        //                                            else if (breakCount.Count() == 1 && breakCount.FirstOrDefault().BreakTypeId == 1)
        //                                            {
        //                                                if (DateTime.Now >= LocationDetail.TeaTimeFrom && DateTime.Now <= LocationDetail.TeaTimeFrom)
        //                                                {
        //                                                    BreakTypeId = 2;
        //                                                }
        //                                                else
        //                                                {
        //                                                }
        //                                            }
        //                                        }
        //                                        else if (LocationDetail.BreakCategoryId == 2) // 2 - limited break
        //                                        {
        //                                            var breakCount = (from b in context.Breaks
        //                                                              where b.UserId == LD.UserId && b.BreakinTime.Date == DateTime.Today
        //                                                              select b.BreakId).Count();
        //                                            if (breakCount >= LocationDetail.TotalBreakCount)
        //                                            {
        //                                                ValidBreak = false;
        //                                                LD.ReturnType = "failure";
        //                                                LD.ReturnResult = "Your break limit is finish.";
        //                                                LD.FailureReason = "";
        //                                            }
        //                                        }
        //                                        else if (LocationDetail.BreakCategoryId == 4) // 4 - fix and limited
        //                                        {
        //                                            var breakCount = (from b in context.Breaks
        //                                                              where b.UserId == LD.UserId && b.BreakinTime.Date == DateTime.Today
        //                                                              select b);
        //                                            if (breakCount.Count() >= LocationDetail.TotalBreakCount)
        //                                            {
        //                                                ValidBreak = false;
        //                                                LD.ReturnType = "failure";
        //                                                LD.ReturnResult = "Your break limit is finish.";
        //                                                LD.FailureReason = "";
        //                                            }
        //                                            else
        //                                            {
        //                                                var lunchBreakCount = 0;
        //                                                var teaBreakCount = 0;

        //                                                if (breakCount.Count() > 0)
        //                                                {
        //                                                    lunchBreakCount = breakCount.Where(a => a.BreakTypeId == 1).Count();
        //                                                    teaBreakCount = breakCount.Where(a => a.BreakTypeId == 2).Count();
        //                                                }

        //                                                if (lunchBreakCount == 0 && DateTime.Now >= LocationDetail.LunchTimeFrom && DateTime.Now <= LocationDetail.LunchTimeTo)
        //                                                {
        //                                                    BreakTypeId = 1;
        //                                                }
        //                                                else if (teaBreakCount == 0 && (DateTime.Now >= LocationDetail.TeaTimeFrom && DateTime.Now <= LocationDetail.TeaTimeFrom))
        //                                                {
        //                                                    BreakTypeId = 2;
        //                                                }
        //                                                else
        //                                                {
        //                                                    if (breakCount.Where(a => a.BreakTypeId == 3).Count() >= LocationDetail.TotalBreakCount - 2)
        //                                                    {
        //                                                        ValidBreak = false;
        //                                                        LD.ReturnType = "failure";
        //                                                        LD.ReturnResult = "Your other break limit is finish.";
        //                                                        LD.FailureReason = "";
        //                                                    }
        //                                                    //else
        //                                                    //{
        //                                                    //    //ValidBreak = false;
        //                                                    //    //LD.ReturnType = "failure";
        //                                                    //    //LD.ReturnResult = "Do u Want to take other break? ";
        //                                                    //    //LD.FailureReason = "other";
        //                                                    //}
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        ValidBreak = false;
        //                                        LD.ReturnType = "failure";
        //                                        LD.ReturnResult = "You are not in approved location.Please enter reason if you are outside from approved location.";
        //                                        LD.FailureReason = "location";
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (OutsideReason == "")
        //                                    {
        //                                        ValidBreak = false;
        //                                        LD.ReturnType = "failure";
        //                                        LD.ReturnResult = "You are not in approved location.Please enter reason if you are outside from approved location.";
        //                                        LD.FailureReason = "location";
        //                                    }
        //                                }

        //                                if (ValidBreak)
        //                                {
        //                                    var BDetail = new Break();
        //                                    BDetail.UserId = LD.UserId;
        //                                    BDetail.BreakinTime = DateTime.Now;
        //                                    BDetail.BreakinDeviceId = DeviceId;
        //                                    BDetail.BreakinType = BreakInOutType;
        //                                    if (BreakInOutType)
        //                                        BDetail.BILocationId = LocationId;
        //                                    BDetail.BILatitudeLongitude = LatitudeLongitude;
        //                                    BDetail.BreakinOutsideLocationReason = ((OutsideReason.Trim() != "") ? (OutsideReason.Trim()) : null);
        //                                    BDetail.BreakTypeId = BreakTypeId;
        //                                    context.Breaks.InsertOnSubmit(BDetail);
        //                                    context.SubmitChanges();

        //                                    BD.BreakId = BDetail.BreakId;
        //                                    BD.BreakinTime = BDetail.BreakinTime.ToString("hh:mm:ss tt");
        //                                    BreakComplete = false;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                LD.ReturnType = "failure";
        //                                LD.ReturnResult = "You already breakin from another Device";
        //                                LD.FailureReason = "device";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var CheckBreakExist = (from b in context.Breaks
        //                                                   where b.UserId == LD.UserId && b.BreakinTime.Date == DateTime.Now.Date && b.BreakoutTime == null
        //                                                   select b.BreakId).Count();
        //                            if (CheckBreakExist > 0)
        //                            {
        //                                var BDetail = (from b in context.Breaks
        //                                               where b.BreakId == BreakId
        //                                               select b).FirstOrDefault();

        //                                BDetail.BreakoutTime = DateTime.Now;
        //                                BDetail.BreakoutDeviceId = DeviceId;
        //                                BDetail.BreakoutType = BreakInOutType;
        //                                if (BreakInOutType)
        //                                    BDetail.BOLocationId = LocationId;
        //                                BDetail.BOLatitudeLongitude = LatitudeLongitude;
        //                                BDetail.BreakoutOutsideLocationReason = ((OutsideReason.Trim() != "") ? (OutsideReason.Trim()) : null);

        //                                context.SubmitChanges();

        //                                var TempWork = BDetail.BreakoutTime.Value.TimeOfDay - BDetail.BreakinTime.TimeOfDay;
        //                                BD.BreakinTime = BDetail.BreakinTime.ToString("hh:mm:ss tt");
        //                                BD.BreakingHour = TempWork.Hours + ":" + TempWork.Minutes + ":" + TempWork.Seconds;
        //                                BD.BreakoutTime = BDetail.BreakoutTime.Value.ToString("hh:mm:ss tt");
        //                                BreakComplete = true;
        //                            }
        //                            else
        //                            {
        //                                LD.ReturnType = "failure";
        //                                LD.ReturnResult = "You already breakout from another Device";
        //                                LD.FailureReason = "device";
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        LD.ReturnType = "failure";
        //                        LD.ReturnResult = "User is not punched in";
        //                        LD.FailureReason = "";
        //                    }
        //                }
        //                else
        //                {
        //                    LD.ReturnType = "failure";
        //                    LD.ReturnResult = "Please update this app";
        //                    LD.FailureReason = "";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LD.ReturnType = "failure";
        //        LD.ReturnResult = "Server Is Not Responding...";
        //        LD.FailureReason = "";
        //    }

        //    var FinalResult = new
        //    {
        //        BreakComplete = BreakComplete,
        //        BreakId = BD.BreakId,
        //        BreakinTime = BD.BreakinTime != null ? BD.BreakinTime : "",
        //        BreakoutTime = BD.BreakoutTime != null ? BD.BreakoutTime : "",
        //        BreakingHour = BD.BreakingHour != null ? BD.BreakingHour : "",
        //        type = LD.ReturnType,
        //        reason = LD.FailureReason,
        //        msg = LD.ReturnResult,
        //    };

        //    JavaScriptSerializer jss = new JavaScriptSerializer();
        //    return jss.Serialize(FinalResult);
        //}

        //    [WebMethod]
        //    public List<DateTime> GetWeeks(DayOfWeek weekStart, DateTime startDate)
        //    {
        //        List<DateTime> result = new List<DateTime>();

        //        DayOfWeek day = DateTime.Now.DayOfWeek;
        //        int days = day - DayOfWeek.Monday;
        //        DateTime start = DateTime.Now.AddDays(-days);
        //        DateTime end = start.AddDays(6);
        //        return result;
        //    }
        //    [WebMethod]
        //    public string GetTest1(int UserId, int GroupID)
        //    {

        //        try
        //        {
        //            using (var context = new ELDBDataContext())
        //            {

        //                User usr = context.Users.Where(u => u.UserId.Equals(UserId)).FirstOrDefault();

        //                List<WeeklyTiming> Weektime = context.WeeklyTimings.Where(u => u.UserID.Equals(UserId)).ToList();



        //                string BodyText = "";
        //                if (File.Exists(Server.MapPath("~\\MailTemplates\\Welcome.html")))
        //                {
        //                    string CompanyName = (from c in context.Companies where c.CompanyId == usr.CompanyID select c.CompanyName).FirstOrDefault();

        //                    StreamReader sr = new StreamReader(Server.MapPath("~\\MailTemplates\\Welcome.html"));
        //                    BodyText = sr.ReadToEnd();
        //                    sr.Close();

        //                    BodyText = BodyText.Replace("##Name##", usr.FirstName + " " + usr.LastName);
        //                    BodyText = BodyText.Replace("##CompanyName##", (from c in context.Companies where c.CompanyId == usr.CompanyID select c.CompanyName).FirstOrDefault());
        //                    BodyText = BodyText.Replace("##Username##", usr.UserName);
        //                    BodyText = BodyText.Replace("##Password##", usr.Password);
        //                    BodyText = BodyText.Replace("##ReportingPerson##", (from u in context.Users where u.UserId == usr.TopId select u.FirstName + " " + u.LastName).FirstOrDefault());
        //                    BodyText = BodyText.Replace("##Department##", (from g in context.GroupMasters where g.ID == usr.GroupID select g.GroupName).FirstOrDefault());

        //                    foreach (var item1 in Weektime)
        //                    {
        //                        BodyText = BodyText.Replace("##" + item1.Day + "TimeIn##", Convert.ToString(item1.TimeFrom.TimeOfDay));
        //                        BodyText = BodyText.Replace("##" + item1.Day + "TimeOut##", Convert.ToString(item1.TimeUpto.TimeOfDay));
        //                    }


        //                    GF.SendEmailNotification("NKTPL+ Support", Convert.ToString(usr.EmailID), BodyText,
        //                                                             "[" + (from c in context.Companies where c.CompanyId == usr.CompanyID select c.CompanyName).FirstOrDefault() + "+] - Welcome " + usr.FirstName + " " + usr.LastName);
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }

        //        return "Success";
        //    }

        //    [WebMethod]
        //    public string GetTest2(int UserId, string deviceid)
        //    {
        //        try
        //        {
        //            using (var context = new ELDBDataContext())
        //            {
        //                UserDevice URDetail = context.UserDevices.Where(ud => ud.UserId == UserId && ud.DeviceId == deviceid).FirstOrDefault();

        //                #region SendEmailDeviceApproval
        //                string BodyText = "";
        //                if (File.Exists(Server.MapPath("~\\MailTemplates\\Device-Approval.html")))
        //                {
        //                    string email = "";
        //                    string approvedby = "";
        //                    string subject = "";
        //                    email = (from u in context.Users where u.UserId == URDetail.UserId select u.UserName).FirstOrDefault(); ;
        //                    approvedby = (from u in context.Users where u.UserId == URDetail.ApprovedBy select u.FirstName + " " + u.LastName).FirstOrDefault();

        //                    string company = (from c in context.Companies
        //                                      join u in context.Users on c.CompanyId equals u.CompanyID
        //                                      where u.UserId == URDetail.UserId
        //                                      select c.CompanyName).FirstOrDefault();


        //                    if (URDetail.IsApproved == true)
        //                        subject = "[" + company + " +] - Congratulations, your device was approved";
        //                    else
        //                        subject = "[" + company + " +] - Apologies, your device was rejected";

        //                    StreamReader sr = new StreamReader(Server.MapPath("~\\MailTemplates\\Device-Approval.html"));
        //                    BodyText = sr.ReadToEnd();
        //                    sr.Close();
        //                    BodyText = BodyText.Replace("##Username##", (from u in context.Users where u.UserId == URDetail.UserId select u.FirstName + " " + u.LastName).FirstOrDefault());
        //                    BodyText = BodyText.Replace("##DeviceName##", URDetail.DeviceName);
        //                    BodyText = BodyText.Replace("##DeviceID##", URDetail.DeviceId);
        //                    BodyText = BodyText.Replace("##OSVersion##", URDetail.OSVersion);
        //                    BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
        //                    BodyText = BodyText.Replace("##RegistrationLocation##", URDetail.LocationName);
        //                    BodyText = BodyText.Replace("##RegistrationDate##", URDetail.RequestDate.ToString());
        //                    BodyText = BodyText.Replace("##RegistrationTime##", URDetail.RequestDate.TimeOfDay.ToString());
        //                    BodyText = BodyText.Replace("##ApprovedBy##", approvedby != "" ? approvedby : "-");
        //                    BodyText = BodyText.Replace("##ApprovedDate##", approvedby != "" ? URDetail.ApprovedDate.ToString() : "-");
        //                    BodyText = BodyText.Replace("##ApprovedTime##", approvedby != "" ? URDetail.ApprovedDate.ToString() : "-");
        //                    BodyText = BodyText.Replace("##Status##", URDetail.IsApproved == true ? "Approved" : "Rejected");
        //                    GF.SendEmailNotification("NKTPL+ Device Status", Convert.ToString(email), BodyText, subject);
        //                }
        //                #endregion
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }


        //        return "Success";
        //    }
        //}

        //internal void AndroidPushNeew(string RegIds, string Data)
        //{
        //    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //    tRequest.Method = "post";
        //    serverKey - Key from Firebase cloud messaging server
        //    tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAA0MaMILU:APA91bG_WicYOTgWO6oPJB5zQqM3zqfLNjIsPlRDPiSx3mVYvXQmeBPZUPfcSZbtiCNPZDGpSLOjs7B7xckBcfzop_o851WqYjht5UKtZa3W6nBcMfWAomdn5O_b6bkXYQhKsyokpyQM"));
        //    Sender Id -From firebase project setting
        //    tRequest.Headers.Add(string.Format("Sender: id={0}", "896684269749"));
        //    tRequest.ContentType = "application/json";
        //    var payload = new
        //    {
        //        to = RegIds,
        //        priority = "high",
        //        content_available = true,
        //        notification = new
        //        {
        //            message = Data,

        //        },
        //    };

        //    string postbody = JsonConvert.SerializeObject(payload).ToString();
        //    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
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
        //}

        // [WebMethod]
        //public string GetTest(int UserId)
        //{
        //    List<MonthHours> MonthDetail = new List<MonthHours>();


        //    var ConvertedSelectedMonth = Convert.ToDateTime(DateTime.Now);
        //    try
        //    { //check Username Password with device

        //        int year = DateTime.Now.Year;
        //        int month = DateTime.Now.Month;


        //        var ret = new List<DateTime>();
        //        for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
        //        {
        //            ret.Add(new DateTime(year, month, i));
        //        }


        //        using (var context = new ELDBDataContext())
        //        {

        //            foreach (var item in ret)
        //            {
        //                MonthHours mm = new MonthHours();
        //                mm.CurrMonthDate = Convert.ToString(item.Date);
        //                mm.Extra = ((from p in context.PunchIns where p.UserId == UserId && p.PunchinTime.Date == item.Date select p.ExtraWorkingHour).FirstOrDefault()) != null ? Math.Round((TimeSpan.Parse(((from p in context.PunchIns where p.UserId == UserId && p.PunchinTime.Date == item.Date select p.ExtraWorkingHour).FirstOrDefault()))).TotalHours, 2) : 0.00;
        //                MonthDetail.Add(mm);
        //            }








        //            TimeSpan _time = TimeSpan.Parse("07:16:46.5988761");

        //            var abc = Math.Round((TimeSpan.Parse("-07:16:46.5988761")).TotalHours, 2);



        //            DateTime dt1 = DateTime.Parse("11:55");
        //            DateTime dt2 = DateTime.Parse("9:35");

        //            double span = (dt2 - dt1).TotalHours;

        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        LD.ReturnType = "failure";
        //        LD.ReturnResult = "Server Is Not Responding...";
        //        LD.FailureReason = "";
        //    }
        //    var FinalResult = new
        //    {

        //        MonthDetail = MonthDetail,
        //        type = LD.ReturnType,
        //        reason = LD.FailureReason,
        //        msg = LD.ReturnResult,
        //    };

        //    JavaScriptSerializer jss = new JavaScriptSerializer();
        //    return jss.Serialize(FinalResult);

        //}

        //[WebMethod]
        //public string GetTest(string UserId)
        //{
        //    string[] userid = UserId.Split(',');
        //    List<MonthHours> MonthDetail = new List<MonthHours>();


        //    var ConvertedSelectedMonth = Convert.ToDateTime(DateTime.Now);
        //    try
        //    { //check Username Password with device


        //        foreach (var item in userid)
        //        {

        //            using (var context = new ELDBDataContext())
        //            {

        //                var PODetail = (from p in context.PunchIns where p.UserId == Convert.ToInt32(item) && p.PunchoutTime != null select p).ToList();
        //                PODetail.ForEach(a =>
        //                {
        //                    a.WorkingHour = Convert.ToString(a.PunchoutTime.Value.TimeOfDay - a.PunchinTime.TimeOfDay);
        //                    a.ExtraWorkingHour = Convert.ToString(new TimeSpan(TimeSpan.Parse(a.WorkingHour).Ticks - (context.WeeklyTimings.Where(w => w.ObjectId == Convert.ToInt32(item) && w.TimingFor == 3 && w.Day == Convert.ToString(a.PunchinTime.DayOfWeek)).Select(s => s.WorkingHours).FirstOrDefault()).Ticks));
        //                });

        //                context.SubmitChanges();


        //            }


        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        LD.ReturnType = "failure";
        //        LD.ReturnResult = "Server Is Not Responding...";
        //        LD.FailureReason = "";
        //    }
        //    var FinalResult = new
        //    {

        //        MonthDetail = MonthDetail,
        //        type = LD.ReturnType,
        //        reason = LD.FailureReason,
        //        msg = LD.ReturnResult,
        //    };

        //    JavaScriptSerializer jss = new JavaScriptSerializer();
        //    return jss.Serialize(FinalResult);

        //}
        //[WebMethod]
        //public string GetAssignLocationByUser(string UserName, string Password, string DeviceId, string DeviceType, int CompanyId, string AppVersion)
        //{
        //    dynamic LDetail = null;

        //    try
        //    {
        //        //check Username Password with device
        //        LD = GF.CheckValidUser(UserName, Password, DeviceId, DeviceType);

        //        if (LD.ReturnType == "success")
        //        {
        //            using (var context = new ELDBDataContext())
        //            {
        //                LDetail = (from ul in context.UserLocations
        //                           join l in context.Locations on ul.LocationId equals l.LocationId
        //                           join lpin in context.LocationPins on l.LocationId equals lpin.LocationID
        //                           where ul.UserId == LD.UserId && ul.UserDeviceId == LD.UDId
        //                           select new
        //                           {
        //                               LocationId = ul.LocationId,
        //                               LocacationName = l.PlaceName,
        //                               LocationImage = l.LocationImage,
        //                               Latitude = lpin.Lat,
        //                               Longitude = lpin.Long
        //                           }).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LD.ReturnType = "failure";
        //        LD.ReturnResult = "Server Is Not Responding...";
        //        LD.FailureReason = "";
        //    }
        //    var FinalResult = new
        //    {
        //        LDetail = LDetail,
        //        type = LD.ReturnType,
        //        reason = LD.FailureReason,
        //        msg = LD.ReturnResult,
        //    };

        //    JavaScriptSerializer jss = new JavaScriptSerializer();
        //    return jss.Serialize(FinalResult);
        //}

    }
}