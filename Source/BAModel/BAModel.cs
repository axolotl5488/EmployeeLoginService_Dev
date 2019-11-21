using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using BAModel.Common;
using BAModel.Model;
using System.Data.Entity;
using System.Diagnostics;
using System.Net.Mail;
using System.IO;

namespace BAModel
{
    public static class BAModel
    {
        public static ResultStatusModel ManageLeave(int CompanyID, int UserID, long LeaveId, int LeaveTypeId, string Descirption, string StartDate, string EndDate, int MasterLeaveTypeID)
        {
            ResultStatusModel response = new ResultStatusModel();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();
                AppUserLeave obj = new AppUserLeave();

                DateTime startdate = Convert.ToDateTime(StartDate);
                DateTime enddate = Convert.ToDateTime(EndDate);
                decimal TotalDayWantLeaves = enddate.Subtract(startdate).Days + 1;
                if (LeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                {
                    TotalDayWantLeaves = TotalDayWantLeaves / 2;
                }

                AppCompanyLeaveManagement Assignleaves = db.AppCompanyLeaveManagements.Where(x => x.CompanyID == CompanyID && x.UserID == UserID).FirstOrDefault();
                decimal TotalAllocatedLeaves = Assignleaves.TotalYearlyLeaves;
                decimal TotalUsedLeaves = GetUserTotalUsedLeaves(UserID);
                TotalUsedLeaves = TotalUsedLeaves + TotalDayWantLeaves;

                if (LeaveId == 0)
                {
                    List<AppUserLeave> appliedLeaves = db.AppUserLeaves.Where(x =>
                                                        (x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Sanctioned || x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Pending)
                                                     && (x.UserID == UserID) && (x.StartDate >= startdate && x.EndDate <= enddate)
                                                        ).ToList();

                    if (appliedLeaves.Count == 0)
                    {

                        bool IsAddLeave = true;
                        if (Assignleaves == null)
                        {
                            IsAddLeave = false;
                            response.message = "you don't have any pending leave";
                        }
                        //else if(TotalAllocatedLeaves <= TotalUsedLeave && (LeaveTypeId == (int)APIEnum.Leavetype.FirstHalfLeave || LeaveTypeId == (int)APIEnum.Leavetype.SecondHalfLeave))
                        //{
                        //    IsAddLeave = false;
                        //    response.message = "you have used your all half leaves for this year";
                        //}
                        //else if (TotalAllocatedLeaves <= TotalUsedLeave && (LeaveTypeId == (int)APIEnum.Leavetype.FirstHalfLeave))
                        //{
                        //    IsAddLeave = false;
                        //    response.message = "you have used your all full leaves for this year";
                        //}

                        if (IsAddLeave)
                        {
                            obj.AppLeaveApprovedStatusId = (int)APIEnum.LeaveApprovedStatus.Pending;
                            obj.AppLeaveTypeId = LeaveTypeId;
                            obj.AppMasterLeaveTypeID = MasterLeaveTypeID;
                            obj.DateCreated = DateTime.Now;
                            obj.DateModified = DateTime.Now;
                            obj.Description = Descirption;
                            obj.EndDate = Convert.ToDateTime(EndDate);
                            obj.IsActive = true;
                            obj.StartDate = Convert.ToDateTime(StartDate);
                            obj.UserID = UserID;
                            obj.CompanyID = CompanyID;
                            obj.TotalDays = enddate.Subtract(startdate).Days + 1;

                            if (LeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                            {
                                obj.TotalDays = obj.TotalDays / 2;
                            }

                            if (TotalUsedLeaves > TotalAllocatedLeaves)
                            {
                                obj.IsPaidLeave = false;
                            }
                            else
                            {
                                obj.IsPaidLeave = true;
                            }

                            db.AppUserLeaves.Add(obj);
                        }
                        else
                        {
                            response.status = false;
                        }
                    }
                    else
                    {
                        response.status = false;
                        response.message = "You have already applied leaves for this dates";
                    }
                }
                else
                {
                    obj = db.AppUserLeaves.Find(LeaveId);
                    if (obj != null)
                    {
                        obj.AppLeaveApprovedStatusId = (int)APIEnum.LeaveApprovedStatus.Pending;
                        obj.AppLeaveTypeId = LeaveTypeId;
                        obj.AppMasterLeaveTypeID = MasterLeaveTypeID;
                        obj.DateModified = DateTime.Now;
                        obj.Description = Descirption;
                        obj.EndDate = Convert.ToDateTime(EndDate);
                        obj.IsActive = true;
                        obj.StartDate = Convert.ToDateTime(StartDate);
                        obj.UserID = UserID;
                        obj.CompanyID = CompanyID;

                        if (LeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                        {
                            obj.TotalDays = obj.TotalDays / 2;
                        }

                        if (TotalUsedLeaves > TotalAllocatedLeaves)
                        {
                            obj.IsPaidLeave = false;
                        }
                        else
                        {
                            obj.IsPaidLeave = true;
                        }
                    }

                }

                db.SaveChanges();
                response.status = true;
                response.message = string.Empty;
            }
            catch (Exception ex)
            {
                response.status = false;
                response.message = ex.Message;
            }

            return response;
        }

        public static GetLeavesListByUser_Model GetLeavesListByUser(int CompanyID, int UserID)
        {
            GetLeavesListByUser_Model result = new GetLeavesListByUser_Model();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();

                List<AppUserLeave> list = db.AppUserLeaves.Where(x => x.UserID == UserID && x.IsActive).Include(x => x.User).Include(x => x.User1).OrderByDescending(x => x.StartDate).ToList();
                foreach (AppUserLeave obj in list)
                {
                    GetLeaveList_detail_Model map = new GetLeaveList_detail_Model();
                    map.leaveid = obj.ID;
                    map.comapnyid = CompanyID;
                    map.description = obj.Description;
                    map.enddate = Config.GetTimeStamp(obj.EndDate);
                    map.leaveapprovedstatusid = obj.AppLeaveApprovedStatusId;
                    map.leaveapprovedstatusname = Convert.ToString((APIEnum.LeaveApprovedStatus)obj.AppLeaveApprovedStatusId);
                    map.leavetypeid = obj.AppLeaveTypeId;
                    map.leavetypename = Convert.ToString((APIEnum.Leavetype)obj.AppLeaveTypeId);

                    map.masterleavetypeid = obj.AppMasterLeaveTypeID;
                    map.masterleavetypename = Convert.ToString((APIEnum.MasterLeavetype)obj.AppMasterLeaveTypeID);

                    map.startdate = Config.GetTimeStamp(obj.StartDate);
                    map.userid = obj.UserID;
                    map.username = obj.User.FirstName + " " + obj.User.LastName;
                    map.toaldays = obj.EndDate.Subtract(obj.StartDate).Days;
                    map.createddate = Config.GetTimeStamp(obj.DateCreated);
                    map.modifieddate = Config.GetTimeStamp(obj.DateModified);
                    if (obj.User1 != null)
                    {
                        map.approvedbyuserid = obj.ApprovedBy;
                        map.approvedbyusername = obj.User1.FirstName + " " + obj.User1.LastName;
                    }

                    result.records.Add(map);
                }

                result.report = GetLeaveStatatics(list);
                result.response.status = true;
                result.response.message = string.Empty;
            }
            catch (Exception ex)
            {
                result.response.status = false;
                result.response.message = ex.Message;
            }

            return result;
        }

        public static GetLeavesListByCompany_Model GetLeavesListByCompany(int CompanyID)
        {
            GetLeavesListByCompany_Model result = new GetLeavesListByCompany_Model();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();

                List<AppUserLeave> list = db.AppUserLeaves.Where(x => x.CompanyID == CompanyID && x.IsActive).Include(x => x.User).Include(x => x.User1).OrderByDescending(x => x.StartDate).ToList();
                foreach (AppUserLeave obj in list)
                {
                    GetLeaveList_detail_Model map = new GetLeaveList_detail_Model();
                    map.leaveid = obj.ID;
                    map.comapnyid = CompanyID;
                    map.description = obj.Description;
                    map.enddate = Config.GetTimeStamp(obj.EndDate);
                    map.leaveapprovedstatusid = obj.AppLeaveApprovedStatusId;
                    map.leaveapprovedstatusname = Convert.ToString((APIEnum.LeaveApprovedStatus)obj.AppLeaveApprovedStatusId);
                    map.leavetypeid = obj.AppLeaveTypeId;
                    map.leavetypename = Convert.ToString((APIEnum.Leavetype)obj.AppLeaveTypeId);
                    map.masterleavetypeid = obj.AppMasterLeaveTypeID;
                    map.masterleavetypename = Convert.ToString((APIEnum.MasterLeavetype)obj.AppMasterLeaveTypeID);
                    map.startdate = Config.GetTimeStamp(obj.StartDate);
                    map.userid = obj.UserID;
                    map.username = obj.User.FirstName + " " + obj.User.LastName;
                    map.toaldays = obj.EndDate.Subtract(obj.StartDate).Days;
                    map.createddate = Config.GetTimeStamp(obj.DateCreated);
                    map.modifieddate = Config.GetTimeStamp(obj.DateModified);
                    if (obj.User1 != null)
                    {
                        map.approvedbyuserid = obj.ApprovedBy;
                        map.approvedbyusername = obj.User1.FirstName + " " + obj.User1.LastName;
                    }

                    result.records.Add(map);
                }

                result.report = GetLeaveStatatics(list);
                result.response.status = true;
                result.response.message = string.Empty;
            }
            catch (Exception ex)
            {
                result.response.status = false;
                result.response.message = ex.Message;
            }

            return result;
        }

        public static ResultStatusModel UpdateLeaveStatus(int CompanyID, int UserID, long LeaveId, int LeaveStatusId)
        {
            ResultStatusModel response = new ResultStatusModel();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();
                AppUserLeave obj = new AppUserLeave();
                obj = db.AppUserLeaves.Find(LeaveId);
                if (obj != null)
                {
                    obj.AppLeaveApprovedStatusId = LeaveStatusId;
                    obj.DateModified = DateTime.Now;
                    obj.ApprovedBy = UserID;
                    db.SaveChanges();
                    response.status = true;
                    response.message = string.Empty;
                }
                else
                {
                    response.status = true;
                    response.message = "record does not found!";
                }


            }
            catch (Exception ex)
            {
                response.status = false;
                response.message = ex.Message;
            }

            return response;
        }

        public static GetLeaveDetail_Model GetLeaveDetail(int CompanyID, int UserID, int LeaveID)
        {
            GetLeaveDetail_Model result = new GetLeaveDetail_Model();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();

                AppUserLeave obj = db.AppUserLeaves.Where(x => x.ID == LeaveID).Include(x => x.User).Include(x => x.User1).FirstOrDefault();

                GetLeaveList_detail_Model map = new GetLeaveList_detail_Model();
                map.leaveid = obj.ID;
                map.comapnyid = CompanyID;
                map.description = obj.Description;
                map.enddate = Config.GetTimeStamp(obj.EndDate);
                map.leaveapprovedstatusid = obj.AppLeaveApprovedStatusId;
                map.leaveapprovedstatusname = Convert.ToString((APIEnum.LeaveApprovedStatus)obj.AppLeaveApprovedStatusId);
                map.leavetypeid = obj.AppLeaveTypeId;
                map.leavetypename = Convert.ToString((APIEnum.Leavetype)obj.AppLeaveTypeId);
                map.masterleavetypeid = obj.AppMasterLeaveTypeID;
                map.masterleavetypename = Convert.ToString((APIEnum.MasterLeavetype)obj.AppMasterLeaveTypeID);
                map.startdate = Config.GetTimeStamp(obj.StartDate);
                map.userid = obj.UserID;
                map.username = obj.User.FirstName + " " + obj.User.LastName;
                map.toaldays = obj.TotalDays;

                map.createddate = Config.GetTimeStamp(obj.DateCreated);
                map.modifieddate = Config.GetTimeStamp(obj.DateModified);

                if (obj.User1 != null)
                {
                    map.approvedbyuserid = obj.ApprovedBy;
                    map.approvedbyusername = obj.User1.FirstName + " " + obj.User1.LastName;
                }

                result.record = map;
                result.response.status = true;
                result.response.message = string.Empty;
            }
            catch (Exception ex)
            {
                result.response.status = false;
                result.response.message = ex.Message;
            }

            return result;
        }

        public static GetLeavesReportByUser_Model GetLeavesReportByUser(int CompanyID, int UserID)
        {
            GetLeavesReportByUser_Model result = new GetLeavesReportByUser_Model();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();

                AppCompanyLeaveManagement companyAllocatedLeaves = db.AppCompanyLeaveManagements.FirstOrDefault(x => x.CompanyID == CompanyID && x.UserID == UserID);
                List<AppUserLeave> list = db.AppUserLeaves.Where(x => x.UserID == UserID && x.IsActive).Include(x => x.User).Include(x => x.User1).ToList();

                result.record.TotalCanceled = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Canceled).Sum(x => x.TotalDays);
                result.record.TotalPending = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Pending).Sum(x => x.TotalDays);
                result.record.TotalRejected = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Rejected).Sum(x => x.TotalDays);
                result.record.TotalReverted = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Reverted).Sum(x => x.TotalDays);
                result.record.TotalSanctioned = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Sanctioned).Sum(x => x.TotalDays);

                result.record.TotalSanctionedhalfLeaves = list.Where(x => x.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave && x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Sanctioned).Sum(x => x.TotalDays);

                if (companyAllocatedLeaves != null)
                {
                    result.record.TotalRemainingFullLeaves = Math.Floor((companyAllocatedLeaves.TotalYearlyLeaves - (result.record.TotalSanctioned + result.record.TotalPending)));
                    result.record.TotalRemainingHalfLeaves = (companyAllocatedLeaves.TotalYearlyLeaves - (result.record.TotalSanctioned + result.record.TotalPending)) * 2;
                }

                result.response.status = true;
                result.response.message = string.Empty;
            }
            catch (Exception ex)
            {
                result.response.status = false;
                result.response.message = ex.Message;
            }

            return result;
        }

        public static void CalculatePrevPunchInOutTiming(int UserID, int CompanyId)
        {
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();
                PunchIn Prev_Punchin = db.PunchIns.Where(x => x.UserId == UserID && x.PunchoutTime != null).OrderByDescending(x => x.PId).LastOrDefault();

                if (Prev_Punchin != null)
                {
                    double PrevDayHours = Prev_Punchin.PunchoutTime.Value.Subtract(Prev_Punchin.PunchinTime).TotalHours;

                    List<WeeklyTiming> Users_weeklytimings = db.WeeklyTimings.Where(x => x.UserID == UserID && x.TimingType == "WorkHours" && x.Status == true).ToList();
                    WeeklyTiming obj_Users_weeklytiming = Users_weeklytimings.FirstOrDefault(x => x.Day == Prev_Punchin.PunchinTime.DayOfWeek.ToString());

                    double DayShiftHours = 8.30;
                    if (obj_Users_weeklytiming != null)
                    {
                        DayShiftHours = obj_Users_weeklytiming.TimeUpto.Subtract(obj_Users_weeklytiming.TimeFrom).TotalHours;
                    }

                    // Full Day Calculation
                    if (Prev_Punchin.IsHalfDay == false && Prev_Punchin.SystemPunchout == false)
                    {
                        if (PrevDayHours < DayShiftHours)
                        {
                            // Calculate Leave deduction
                            // Hours is less than 50% then deduct full Leave
                            if ((PrevDayHours / 2) < (DayShiftHours / 2))
                            {
                                // Full day Leave
                                ManageLeave(CompanyId, UserID, 0, (int)APIEnum.Leavetype.FullLeave, "Working hours are less than 50%", Prev_Punchin.PunchinTime.ToString(), Prev_Punchin.PunchinTime.ToString(), (int)APIEnum.MasterLeavetype.CasualLeave);
                            }
                            else
                            {
                                // Hours is greater than 50%  and less than 100% then deduct half Leave
                                // Half Leave
                                ManageLeave(CompanyId, UserID, 0, (int)APIEnum.Leavetype.FirstHalfLeave, "Working hours are greater than 50%", Prev_Punchin.PunchinTime.ToString(), Prev_Punchin.PunchinTime.ToString(), (int)APIEnum.MasterLeavetype.CasualLeave);
                            }

                        }
                    }
                    else
                    {
                        // Half Hours 
                        DayShiftHours = DayShiftHours / 2;
                        if (PrevDayHours < DayShiftHours)
                        {
                            // Calculate Leave deduction
                            // Hours is less than 50% then deduct full Leave
                            if ((PrevDayHours / 2) < DayShiftHours)
                            {
                                // Full day Leave
                                ManageLeave(CompanyId, UserID, 0, (int)APIEnum.Leavetype.FullLeave, "Working hours are less than 50%", Prev_Punchin.PunchinTime.ToString(), Prev_Punchin.PunchinTime.ToString(), (int)APIEnum.MasterLeavetype.CasualLeave);
                            }
                            else
                            {
                                // Hours is greater than 50%  and less than 100% then deduct half Leave
                                // Half Leave
                                ManageLeave(CompanyId, UserID, 0, (int)APIEnum.Leavetype.FirstHalfLeave, "Working hours are greater than 50%", Prev_Punchin.PunchinTime.ToString(), Prev_Punchin.PunchinTime.ToString(), (int)APIEnum.MasterLeavetype.CasualLeave);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static CheckUserLeaveForDay_Model CheckUserLeaveForDay(DateTime date, int UserID)
        {
            CheckUserLeaveForDay_Model map = new CheckUserLeaveForDay_Model();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();
                List<AppUserLeave> appUserLeaves = db.AppUserLeaves.Where(x => x.UserID == UserID && DbFunctions.TruncateTime(x.StartDate) == DbFunctions.TruncateTime(date)).ToList();
                if (appUserLeaves.Count > 0)
                {
                    map.IsUserApplyForLeave = true;
                    map.LeaveType = appUserLeaves.FirstOrDefault().AppLeaveTypeId;
                    map.LeaveTypeName = Convert.ToString((APIEnum.Leavetype)appUserLeaves.FirstOrDefault().AppLeaveTypeId);
                    map.MasterLeaveType = appUserLeaves.FirstOrDefault().AppMasterLeaveTypeID;
                    map.MasterLeaveTypeName = Convert.ToString((APIEnum.MasterLeavetype)appUserLeaves.FirstOrDefault().AppMasterLeaveTypeID);
                }
                else
                {
                    map.IsUserApplyForLeave = false;
                }
            }
            catch (Exception ex)
            {
                map.IsUserApplyForLeave = false;
            }
            return map;
        }

        //
        public static ResultStatusModel SendEmployeePunchReport(List<int> UserIDs, string StartDate, string EndDate, string EmailSendTo, string EmailSendCC)
        {
            ResultStatusModel map = new ResultStatusModel();
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();
                List<User> users = db.Users.Where(x => UserIDs.Contains(x.UserId)).ToList();
                DateTime startDate = Convert.ToDateTime(StartDate);
                DateTime endDate = Convert.ToDateTime(EndDate);
                DateTime MonthDate = startDate;
                int TotalWorkingDaysOfmonth = endDate.Subtract(startDate).Days + 1;
                List<Location> location_list = db.Locations.ToList();
                List<Holiday> holiday_list = db.Holidays.ToList();
                List<AppUserLeave> AppUserLeave_list = new List<AppUserLeave>();


                List<Report_Model> models = new List<Report_Model>();

                foreach (User obj in users)
                {
                    Report_Model model = MapEmployeePunchReport(MonthDate, startDate, endDate, obj.UserId);

                    if (model != null)
                    {
                        model.UserName = obj.FirstName + " " + obj.LastName;
                        models.Add(model);
                    }
                }

                byte[] bytes = ExportExcel(models, MonthDate, startDate, endDate, TotalWorkingDaysOfmonth, holiday_list, location_list);
                string name = "EmployeeMonthlyReport_" + startDate.ToString("dd_MMM_yy") + "_To_" + endDate.ToString("dd_MMM_yy") + ".xlsx";
                SendEmail(bytes, name, EmailSendTo, EmailSendCC);


                map.status = true;
                map.message = "";
            }
            catch (Exception ex)
            {
                map.status = false;
                map.message = ex.Message;
            }
            return map;
        }


        #region Window Service Methods

        public static void CheckAndSetEmployeePunchOutAsSystem()
        {
            try
            {
                EmployeeLoginEntities db = new EmployeeLoginEntities();
                List<PunchIn> punchInList = db.PunchIns.Where(x => x.PunchoutTime == null && DbFunctions.TruncateTime(x.PunchinTime) < DbFunctions.TruncateTime(DateTime.Now)).ToList();
                foreach (PunchIn obj in punchInList)
                {
                    obj.SystemPunchout = true;
                    obj.PunchoutTime = obj.PunchinTime;
                    obj.WorkHourReason = "User didn't Punch Out";

                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("CheckAndSetEmployeePunchOutAsSystem Error Start:: Datetime::" + DateTime.Now + ", Message:: " + ex.Message);
            }
        }

        #endregion

        #region common
        public static LeaveStatatics_Model GetLeaveStatatics(List<AppUserLeave> list)
        {
            LeaveStatatics_Model result = new LeaveStatatics_Model();

            result.TotalCanceled = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Canceled).Sum(x => x.TotalDays);
            result.TotalPending = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Pending).Sum(x => x.TotalDays);
            result.TotalRejected = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Rejected).Sum(x => x.TotalDays);
            result.TotalReverted = list.Where(x => x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Reverted).Sum(x => x.TotalDays);
            result.TotalSanctioned = list.Where(x => x.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave && x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Sanctioned).Sum(x => x.TotalDays);

            result.TotalSanctionedhalfLeaves = list.Where(x => x.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave && x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Sanctioned).Sum(x => x.TotalDays);

            return result;
        }

        public static decimal GetUserTotalUsedLeaves(int UserID)
        {
            EmployeeLoginEntities db = new EmployeeLoginEntities();
            List<AppUserLeave> usedLeaves = db.AppUserLeaves.Where(x => (x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Sanctioned ||
                                                                           x.AppLeaveApprovedStatusId == (int)APIEnum.LeaveApprovedStatus.Pending)).ToList();

            decimal TotalUsedLeaves = usedLeaves.Sum(x => x.TotalDays);

            return TotalUsedLeaves;
        }

        public static Report_Model MapEmployeePunchReport(DateTime MonthDate, DateTime startDate, DateTime endDate, int UserID)
        {
            Report_Model map = new Report_Model();

            try
            {

                EmployeeLoginEntities db = new EmployeeLoginEntities();
                List<Holiday> holidays_records = db.Holidays.Where(x => x.HolidayDate.Month == MonthDate.Month).ToList();
                List<PunchIn> records = db.PunchIns.Where(x => x.UserId == UserID && x.PunchinTime.Month == MonthDate.Month).OrderBy(x => x.PunchinTime).ToList();

                Report_Model model = new Report_Model();
                model.records = records;

                double AverageWorkingHours = records.Where(x => x.PunchoutTime != null).Average(x => x.PunchoutTime.Value.Subtract(x.PunchinTime).TotalHours);
                TimeSpan tp_AverageWorkingHours = TimeSpan.FromHours(AverageWorkingHours);
                model.AverageWorkingHours = tp_AverageWorkingHours.Hours + ":" + tp_AverageWorkingHours.Minutes + ":" + tp_AverageWorkingHours.Seconds;

                double TotalWorkingHours = records.Where(x => x.PunchoutTime != null).Sum(x => x.PunchoutTime.Value.Subtract(x.PunchinTime).TotalHours);
                TimeSpan tp_TotalWorkingHours = TimeSpan.FromHours(TotalWorkingHours);
                model.TotalWorkingHours = Convert.ToInt32(tp_TotalWorkingHours.TotalHours) + ":" + tp_TotalWorkingHours.Minutes + ":" + tp_TotalWorkingHours.Seconds;

                model.TotalLatePunch_In = records.Count;
                model.TotalEarlyPunch_Out = records.Where(x => x.PunchoutTime != null).Count();
                model.TotalPunchIn_Outside = records.Where(x => x.PunchinType == false).Count();
                model.TotalPunchOut_Outside = records.Where(x => x.PunchoutType == false).Count();
                model.TotalSystemPunchOut = records.Where(x => x.SystemPunchout == true).Count();

                model.TotalHolidays = holidays_records.Count;
                model.TotalWeekOffs = 0;
                model.TotalWorkingDays = 0;
                model.TotalWorkingDays = (endDate.Subtract(startDate).Days - (model.TotalHolidays + model.TotalWeekOffs));
                return model;
            }
            catch (Exception ex)
            {
                map = null;
            }
            return map;
        }


        public static Byte[] ExportExcel(List<Report_Model> models, DateTime MonthDate, DateTime startdateOfMonth, DateTime enddateOfMonth, int TotalWorkingDaysOfmonth, List<Holiday> holiday_list, List<Location> location_list)
        {
            try
            {
                using (OfficeOpenXml.ExcelPackage pckExport = new OfficeOpenXml.ExcelPackage())
                {
                    foreach (Report_Model model in models)
                    {
                        List<AppUserLeave> AppUserLeave_list = new List<AppUserLeave>();
                        #region  List
                        #region  Headers

                        OfficeOpenXml.ExcelWorksheet xlSheet = pckExport.Workbook.Worksheets.Add(model.UserName);
                        xlSheet.Name = model.UserName;
                        xlSheet.PrinterSettings.LeftMargin = 0.57M;
                        xlSheet.PrinterSettings.RightMargin = 0.41M;
                        xlSheet.PrinterSettings.HeaderMargin = 0.28M;
                        xlSheet.PrinterSettings.TopMargin = 1.28M;
                        xlSheet.PrinterSettings.BottomMargin = 0.7M;
                        xlSheet.PrinterSettings.FooterMargin = 0.3M;
                        xlSheet.PrinterSettings.Orientation = OfficeOpenXml.eOrientation.Portrait;

                        int padding = 1;
                        int miRow = 1 + padding;
                        int MinCol = 1 + padding;
                        int MaxCol = TotalWorkingDaysOfmonth + 1 + padding;
                        int TotalSearchColumn = TotalWorkingDaysOfmonth + 1 + padding;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "EMPLOYEE PUNCH IN-OUT MONTHLY REPORT";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        miRow = miRow + 1;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = model.UserName + " - Work Report";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 24;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));

                        miRow = miRow + 1;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Report generated on " + (DateTime.Now).ToString("MM/dd/yyyy") + " at " + (DateTime.Now).ToString("HH:mm tt").ToLower();
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#525252"));

                        miRow = miRow + 1;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Powered by NKTPL";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        miRow = miRow + 1;

                        // Add Time Range Search Filter - Tilte
                        //for (int c = 0; c <= TotalSearchColumn;)
                        for (int c = 0; c <= 30;)
                        {
                            if (c == 0)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Working Day";
                            else if (c == 3)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Working Hours";
                            else if (c == 6)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Average Workng Hours";
                            else if (c == 9)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Leaves";
                            else if (c == 12)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Late In";
                            else if (c == 15)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Early Out";
                            else if (c == 18)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total System Out";
                            else if (c == 21)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Outside Punch In";
                            else if (c == 24)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Outside Punch Out";


                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Merge = true;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Name = "Sitka Text";
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Size = 12;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Bold = true;
                            c = c + 3;
                        }

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                        miRow = miRow + 1;

                        // Add Time Range Search Filter - Data
                        for (int c = 0; c <= 30;)
                        {
                            {
                                if (c == 0) // Payroll
                                {
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalWorkingDays;
                                }
                                else if (c == 3) // Location
                                {
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalWorkingHours;
                                }
                                else if (c == 6) // Jobclass
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.AverageWorkingHours;
                                else if (c == 9)
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalLeaves;//"Total Leaves";
                                else if (c == 12)
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalLatePunch_In;//"Total Late In";
                                else if (c == 15)
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalEarlyPunch_Out;//"Total Early Out";
                                else if (c == 18)
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalSystemPunchOut;//"Total System Out";
                                else if (c == 21)
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalPunchIn_Outside;//"Total Outside Punch In";
                                else if (c == 24)
                                    xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalPunchOut_Outside; //"Total Outside Punch Out";

                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Merge = true;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Name = "Sitka Text";
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Size = 12;
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Bold = false;
                            }

                            c = c + 3;
                        }

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;
                        #endregion

                        #region Columns and Sheet Config

                        for (int i = 1 + padding; i <= MaxCol; i++)
                        {
                            xlSheet.Column(i).Width = 25.83;
                        }

                        miRow += 1;

                        //'header alignment
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        //'border
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                        //Font Bold
                        //xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                        //'Background Color
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));


                        //Font Name
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";


                        //Font Size
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;


                        //Font Color
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        int headercolnumber = 1;
                        xlSheet.Cells[miRow, headercolnumber + padding].Value = "Employee";
                        for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                        {
                            if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                            {
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                            }
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.WrapText = true;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.TextRotation = 90;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = startdateOfMonth.AddDays(i).ToString("dd-MM-yy") + Environment.NewLine + startdateOfMonth.AddDays(i).ToString("ddd");

                            // User Leave
                            AppUserLeave obj = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                            if (obj != null)
                            {
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                            }
                            headercolnumber++;
                        }


                        miRow += 1;

                        #endregion

                        #region Bind Data

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFF8E5"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 18;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#BD6427"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Size = 11;
                        xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, 1 + padding].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, 1 + padding].Value = "Prashan Vyash";


                        headercolnumber = 1;
                        for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Font.Size = 11;
                            PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                            if (obj == null || obj.PunchoutTime == null)
                            {
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = "0";
                            }
                            else
                            {
                                double hours = obj.PunchoutTime.Value.Subtract(obj.PunchinTime).TotalHours;
                                TimeSpan sp_hours = TimeSpan.FromHours(hours);
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = sp_hours.Hours + ":" + sp_hours.Minutes + ":" + sp_hours.Seconds;
                            }

                            if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                            {
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                            }
                            headercolnumber++;
                        }
                        miRow += 1;

                        for (int j = 0; j < 11; j++)
                        {

                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                            xlSheet.Cells[miRow, 1 + padding].Style.Font.Bold = true;
                            xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            xlSheet.Cells[miRow, 1 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, 1 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                            if (j == 0)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Puncn In";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj == null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchinTime.ToString("hh:mm tt");
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                }
                            }

                            if (j == 1)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Puncn Out";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {

                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj == null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                        //xlSheet.Cells[miRow + 2, i + 2 + padding].Value = "-";
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchoutTime == null ? " - " : obj.PunchoutTime.Value.ToString("hh:mm tt");
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                }
                            }

                            if (j == 2)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "In Type";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj == null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchinType == null ? " - " : (obj.PunchinType.Value ? "Inside" : "Outside");
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                }
                            }
                            if (j == 3)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "In Location";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj == null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                    }
                                    else
                                    {
                                        Location obj_location = location_list.FirstOrDefault(x => x.LocationId == obj.PILocationId);
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj_location == null ? " - " : obj_location.LocationName;
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                }
                            }
                            if (j == 4)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Late In";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj != null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.LatePunchin ? "Yes" : "No";
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                }
                            }
                            if (j == 5)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Late In Reason";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj != null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.LatePunchinReason;
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                    }
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                                }
                            }

                            if (j == 6)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Out Type";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj == null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchoutType == null ? " - " : (obj.PunchoutType.Value ? "Inside" : "Outside");
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                }
                            }
                            if (j == 7)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Out Location";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj == null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                    }
                                    else
                                    {
                                        Location obj_location = location_list.FirstOrDefault(x => x.LocationId == obj.POLocationId);
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj_location == null ? " - " : obj_location.PlaceName;
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                }
                            }
                            if (j == 8)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Early Out";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj != null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.EarlyPunchout ? "Yes" : "No";
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                }
                            }
                            if (j == 9)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "Early Out Reason";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj != null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.EarlyPunchoutReason;
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                                }
                            }

                            if (j == 10)
                            {
                                xlSheet.Cells[miRow, 1 + padding].Value = "System Punch Out";

                                for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                    PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                    if (obj != null)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = obj.SystemPunchout ? "Yes" : "No";
                                    }
                                    else
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                    }
                                    if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                    }

                                    // User Leave
                                    AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                    if (objleave != null)
                                    {
                                        if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                        }
                                        else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                        {
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                        }
                                    }
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                                }
                            }

                            miRow += 1;
                        }

                        ///Set Autofil columns
                        xlSheet.Cells.AutoFitColumns();



                        #endregion

                        #endregion
                        ///Set Autofil columns
                        xlSheet.Cells.AutoFitColumns();
                    }

                    byte[] bytes = pckExport.GetAsByteArray();
                    return bytes;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        private static void SendEmail(Byte[] bytes, string filename, string EmailSendTo, string EmailSendCC)
        {
            string[] emailtostr = EmailSendTo.Split(',');

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("milanttv13@gmail.com");

                foreach (string objemailaddress in emailtostr)
                {
                    mail.To.Add(objemailaddress);
                }
                if (EmailSendCC != "")
                {
                    string[] emailtostr_cc = EmailSendCC.Split(',');
                    foreach (string objemailaddress_cc in emailtostr)
                    {
                        mail.CC.Add(objemailaddress_cc);
                    }
                }

                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";

                System.Net.Mail.Attachment attachment;
                Stream stream = new MemoryStream(bytes);
                attachment = new System.Net.Mail.Attachment(stream, filename);
                mail.Attachments.Add(attachment);
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("inc.axolotl@gmail.com", "Nktpl@5488");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }
}
