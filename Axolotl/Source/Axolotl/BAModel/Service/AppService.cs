using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAModel.Common;
using BAModel.Model;
using DataModel;
using Newtonsoft.Json;
using System.Data.Entity;
using static BAModel.Common.Common;

namespace BAModel.Service
{
    public static class AppService
    {
        public static GetUserProfile_response GetUserProfile(long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetUserProfile_response response = new GetUserProfile_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                AspNetUser obj = db.AspNetUsers.Find(UserID);
                if (obj != null)
                {
                    Company obj_company = obj.Company;
                    GetUserProfile_detail map = new GetUserProfile_detail();

                    DateTime currentdatetime = DateTime.UtcNow;
                    DateTime start_finicialyear = obj_company.FincialStartYear;
                    DateTime end_finicialyear = obj_company.FincialEndYear;

                    if (end_finicialyear < currentdatetime)
                    {
                        start_finicialyear = start_finicialyear.AddYears(1);
                        end_finicialyear = end_finicialyear.AddYears(1);
                    }

                    map.companyid = obj.CompanyID;
                    map.companyname = obj_company.Name;
                    map.deviceid = obj.DeviceID;
                    map.devicetoken = obj.DeviceToken;
                    map.devicetype = obj.DeviceType;
                    map.firstname = obj.FirstName;
                    map.id = obj.Id;
                    map.lastname = obj.LastName;
                    map.latitude = obj.Latitude;
                    map.longitude = obj.Longitude;
                    map.officeshifttype = obj.OfficeShiftType;
                    map.phonenumber = obj.PhoneNumber;
                    map.shouldsendnotification = obj.ShouldSendNotification;
                    map.userphotourl = obj.UserPhotoURL;
                    map.flexiblebufferminutes = obj_company.FlexibleBufferMinutes;
                    map.noofweekoffdays = obj_company.NoOfWeekOffDays;
                    map.workinghoursinminutes = obj_company.WorkingHoursInMinutes;
                    map.punchrangeinmeter = obj_company.PunchRangeInMeter;

                    map.totalallowedleaves = obj_company.AllowedLeaves;


                    List<EmployeeLeaf> leaves = obj.EmployeeLeaves.Where(x =>
                                                                          (x.LeaveStatus == (int)AppEnum.LeaveStatusEnum.Pending
                                                                         || x.LeaveStatus == (int)AppEnum.LeaveStatusEnum.Sanctioned)
                                                                         &&
                                                                         (x.FromDate >= start_finicialyear && x.ToDate <= end_finicialyear)
                                                                          ).ToList();
                    map.totalappliedleaves = leaves.Where(x => x.DayTypeID == (int)AppEnum.DayTypeEnum.FullLeave).Sum(x => x.ToDate.Subtract(x.FromDate).TotalDays);
                    map.totalappliedleaves = map.totalappliedleaves + leaves.Where(x => x.DayTypeID == (int)AppEnum.DayTypeEnum.HalfLeave).Count() / 2;

                    response.record = map;
                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {
                    response.result.message = "record not found!";
                    response.result.status = false;
                }

                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }


            return response;
        }

        public static UpdateUserDeviceDetail_response UpdateUserDeviceDetail(UpdateUserDeviceDetail_request request, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            UpdateUserDeviceDetail_response response = new UpdateUserDeviceDetail_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                AspNetUser obj = db.AspNetUsers.Find(UserID);
                if (obj != null)
                {
                    if (string.IsNullOrEmpty(obj.DeviceID))
                    {
                        if (!db.AspNetUsers.Any(x => x.DeviceID == request.deviceid && x.Id != obj.Id))
                        {
                            obj.DateModified = DateTime.UtcNow;
                            obj.DeviceID = request.deviceid;
                            obj.DeviceType = request.devicetype;

                            db.SaveChanges();

                            response.result.message = "";
                            response.result.status = true;
                        }
                        else
                        {

                            response.result.message = "device is already registered with other user!";
                            response.result.status = false;
                        }
                    }
                    else
                    {
                        response.result.message = "device is already registered!";
                        response.result.status = false;
                    }
                }
                else
                {
                    response.result.message = "record not found!";
                    response.result.status = false;
                }

                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }


            return response;
        }

        public static UpdateUserDeviceToken_response UpdateUserDeviceToken(UpdateUserDeviceToken_request request, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            UpdateUserDeviceToken_response response = new UpdateUserDeviceToken_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                AspNetUser obj = db.AspNetUsers.Find(UserID);
                if (obj != null)
                {

                    List<AspNetUser> users = db.AspNetUsers.Where(x => x.DeviceToken == request.devicetoken).ToList();
                    foreach (AspNetUser objuser in users)
                    {
                        objuser.DateModified = DateTime.UtcNow;
                        objuser.DeviceToken = null;
                    }
                    db.SaveChanges();

                    obj.DateModified = DateTime.UtcNow;
                    obj.DeviceToken = request.devicetoken;
                    db.SaveChanges();
                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {
                    response.result.message = "record not found!";
                    response.result.status = false;
                }

                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }


            return response;
        }

        public static ManageEmployeePunch_response ManageEmployeePunch(ManageEmployeePunch_request request, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            ManageEmployeePunch_response response = new ManageEmployeePunch_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                EmployeePunch obj = new EmployeePunch();
                if (request.punchid > 0)
                {
                    // Out Type
                    obj = db.EmployeePunches.Find(request.punchid);
                    if (obj != null)
                    {
                        if (request.punchtype == (int)AppEnum.PunchTypeEnum.In)
                        {
                            response.result.message = "you have already punched In";
                            response.result.status = false;
                        }

                        if (request.punchtype == (int)AppEnum.PunchTypeEnum.Out)
                        {
                            if (obj.ClockOutTime == null)
                            {
                                if (!obj.IsSystemClockOut)
                                {
                                    if (request.out_datetime_timestamp != null && request.out_lat != null && request.out_lng != null)
                                    {
                                        obj.DateModified = DateTime.UtcNow;
                                        obj.ClockOutLatitude = request.out_lat;
                                        obj.ClockOutLongitude = request.out_lng;
                                        obj.ClockOutTime = Common.Common.GetDateTimeFromTimeStamp(request.out_datetime_timestamp.Value);
                                        obj.EarlyOuter = request.isearlyouter;
                                        obj.EarlyOuterReason = request.earlyoutouterreason;
                                        obj.IsOutSidePunchOut = request.isoutside_punchout;
                                        obj.PunchOut_LocationID = request.punchout_locationid;
                                        db.SaveChanges();

                                        response.result.message = "";
                                        response.result.status = true;
                                    }
                                    else
                                    {
                                        response.result.message = "request is not valid!";
                                        response.result.status = false;
                                    }
                                }
                                else
                                {
                                    response.result.message = "system has already punched Out";
                                    response.result.status = false;
                                }
                            }
                            else
                            {
                                response.result.message = "you have already punched Out";
                                response.result.status = false;
                            }
                        }
                    }
                    else
                    {
                        response.result.message = "record not found!";
                        response.result.status = false;
                    }
                }
                else
                {
                    // In Type
                    if (request.punchtype == (int)AppEnum.PunchTypeEnum.Out)
                    {
                        response.result.message = "Invalid Punch";
                        response.result.status = false;
                    }

                    if (request.punchtype == (int)AppEnum.PunchTypeEnum.In)
                    {
                        DateTime intime = Common.Common.GetDateTimeFromTimeStamp(request.in_datetime_timestamp);
                        if (db.EmployeePunches.Any(x => x.UserID == UserID && DbFunctions.TruncateTime(x.ClockInTime) == DbFunctions.TruncateTime(intime)))
                        {
                            response.result.message = "you have already punched In";
                            response.result.status = false;
                        }
                        else
                        {
                            obj = new EmployeePunch();
                            obj.ClockInLatitude = request.in_lat;
                            obj.ClockInLongitude = request.in_lng;
                            obj.EarlyOuterReason = request.earlyoutouterreason;
                            obj.ClockInTime = intime;
                            obj.DateCreated = DateTime.UtcNow;
                            obj.IsSystemClockOut = false;
                            obj.LateComer = request.islatecomer;
                            obj.LateComerReason = request.latecomerreason;
                            obj.DateModified = DateTime.UtcNow;
                            obj.UserID = UserID;
                            obj.IsOutSidePunchIn = request.isoutside_punchin;
                            obj.PunchIn_LocationID = request.punchin_locationid;

                            db.EmployeePunches.Add(obj);
                            db.SaveChanges();
                            response.result.message = "";
                            response.result.status = true;
                        }
                    }
                }

                response.punchid = obj.ID;
                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }


            return response;
        }

        public static GetEmployeeTodaysPunchDetail_response GetEmployeeTodaysPunchDetail(GetEmployeeTodaysPunchDetail_request request, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetEmployeeTodaysPunchDetail_response response = new GetEmployeeTodaysPunchDetail_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                DateTime todaydate = GetDateTimeFromTimeStamp(request.date_timestamp);
                EmployeePunch obj = db.EmployeePunches.FirstOrDefault(x => x.UserID == UserID && DbFunctions.TruncateTime(x.ClockInTime) == DbFunctions.TruncateTime(todaydate));

                if (obj != null)
                {
                    if (obj.ClockOutTime != null)
                    {
                        response.punchtype = (int)AppEnum.PunchTypeEnum.Out;
                        response.record.earlyoutouterreason = obj.EarlyOuterReason;
                        response.record.out_datetime_timestamp = ConvertToTimestamp(obj.ClockOutTime.Value);
                        response.record.isearlyouter = obj.EarlyOuter;
                        response.record.out_lat = obj.ClockOutLatitude;
                        response.record.out_lng = obj.ClockOutLongitude;
                    }
                    else
                    {
                        response.punchtype = (int)AppEnum.PunchTypeEnum.In;
                    }
                    response.record.in_datetime_timestamp = ConvertToTimestamp(obj.ClockInTime);
                    response.record.in_lat = obj.ClockInLatitude;
                    response.record.in_lng = obj.ClockInLongitude;
                    response.record.islatecomer = obj.LateComer;
                    response.record.latecomerreason = obj.LateComerReason;
                    response.record.punchid = obj.ID;

                    response.record.isoutside_punchin = obj.IsOutSidePunchIn;
                    response.record.isoutside_punchout = obj.IsOutSidePunchOut;
                    response.record.punchin_locationid = obj.PunchIn_LocationID;
                    response.record.punchout_locationid = obj.PunchOut_LocationID;
                    foreach (EmployeeTask objtask in obj.EmployeeTasks)
                    {
                        GetEmployeeTodaysPunchDetail_TaskList map = new GetEmployeeTodaysPunchDetail_TaskList();
                        map.Task = objtask.Task;
                        map.taskid = objtask.ID;

                        response.record.tasks.Add(map);
                    }
                }
                else
                {
                    response.punchtype = (int)AppEnum.PunchTypeEnum.In;
                }

                response.result.message = "";
                response.result.status = true;
                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }


            return response;
        }

        public static AddPunchTask_response AddPunchTask(AddPunchTask_request request, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            AddPunchTask_response response = new AddPunchTask_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                EmployeePunch obj = db.EmployeePunches.Find(request.punchid);

                if (obj != null)
                {
                    EmployeeTask objtask = new EmployeeTask();
                    objtask.DateCreated = DateTime.UtcNow;
                    objtask.DateModified = DateTime.UtcNow;
                    objtask.EmployeePunchID = obj.ID;
                    objtask.IsDeleted = false;
                    objtask.Task = request.task;
                    objtask.UserID = UserID;

                    db.EmployeeTasks.Add(objtask);
                    db.SaveChanges();

                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {
                    response.result.message = "record not found!";
                    response.result.status = false;
                }


                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static GetPunchTasks_response GetPunchTasks(GetPunchTasks_request request, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetPunchTasks_response response = new GetPunchTasks_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                EmployeePunch obj = db.EmployeePunches.Find(request.punchid);

                if (obj != null)
                {
                    foreach (EmployeeTask objtask in obj.EmployeeTasks)
                    {
                        GetEmployeeTodaysPunchDetail_TaskList map = new GetEmployeeTodaysPunchDetail_TaskList();
                        map.Task = objtask.Task;
                        map.taskid = objtask.ID;

                        response.records.Add(map);
                    }
                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {
                    response.result.message = "record not found!";
                    response.result.status = false;
                }


                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static GetEmployeePunchDetail_response GetEmployeePunchDetail(GetEmployeePunchDetail_request request, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetEmployeePunchDetail_response response = new GetEmployeePunchDetail_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                EmployeePunch obj = db.EmployeePunches.Find(request.punchid);

                if (obj != null)
                {
                    if (obj.ClockOutTime != null)
                    {
                        response.record.earlyoutouterreason = obj.EarlyOuterReason;
                        response.record.out_datetime_timestamp = ConvertToTimestamp(obj.ClockOutTime.Value);
                        response.record.isearlyouter = obj.EarlyOuter;
                        response.record.out_lat = obj.ClockOutLatitude;
                        response.record.out_lng = obj.ClockOutLongitude;
                    }

                    response.record.in_datetime_timestamp = ConvertToTimestamp(obj.ClockInTime);
                    response.record.in_lat = obj.ClockInLatitude;
                    response.record.in_lng = obj.ClockInLongitude;
                    response.record.islatecomer = obj.LateComer;
                    response.record.latecomerreason = obj.LateComerReason;
                    response.record.punchid = obj.ID;

                    response.record.isoutside_punchin = obj.IsOutSidePunchIn;
                    response.record.isoutside_punchout = obj.IsOutSidePunchOut;
                    response.record.punchin_locationid = obj.PunchIn_LocationID;
                    response.record.punchout_locationid = obj.PunchOut_LocationID;

                    foreach (EmployeeTask objtask in obj.EmployeeTasks)
                    {
                        GetEmployeeTodaysPunchDetail_TaskList map = new GetEmployeeTodaysPunchDetail_TaskList();
                        map.Task = objtask.Task;
                        map.taskid = objtask.ID;

                        response.record.tasks.Add(map);
                    }
                }
                response.result.message = "";
                response.result.status = true;
                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static GetEmployeePunchList_response GetEmployeePunchList(long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetEmployeePunchList_response response = new GetEmployeePunchList_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                List<EmployeePunch> records = db.EmployeePunches.Where(x => x.UserID == UserID).OrderByDescending(x => x.ID).ToList();

                foreach (EmployeePunch obj in records)
                {
                    GetEmployeeTodaysPunchDetail record = new GetEmployeeTodaysPunchDetail();
                    if (obj.ClockOutTime != null)
                    {
                        record.earlyoutouterreason = obj.EarlyOuterReason;
                        record.out_datetime_timestamp = ConvertToTimestamp(obj.ClockOutTime.Value);
                        record.isearlyouter = obj.EarlyOuter;
                        record.out_lat = obj.ClockOutLatitude;
                        record.out_lng = obj.ClockOutLongitude;
                    }

                    record.in_datetime_timestamp = ConvertToTimestamp(obj.ClockInTime);
                    record.in_lat = obj.ClockInLatitude;
                    record.in_lng = obj.ClockInLongitude;
                    record.islatecomer = obj.LateComer;
                    record.latecomerreason = obj.LateComerReason;
                    record.punchid = obj.ID;

                    foreach (EmployeeTask objtask in obj.EmployeeTasks)
                    {
                        GetEmployeeTodaysPunchDetail_TaskList map = new GetEmployeeTodaysPunchDetail_TaskList();
                        map.Task = objtask.Task;
                        map.taskid = objtask.ID;

                        record.tasks.Add(map);
                    }

                    response.records.Add(record);
                }

                response.result.message = "";
                response.result.status = true;
                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static GetEmployeePunchList_response Logout(long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetEmployeePunchList_response response = new GetEmployeePunchList_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                AspNetUser user = db.AspNetUsers.Find(UserID);

                user.DeviceToken = null;
                user.DateModified = DateTime.UtcNow;
                db.SaveChanges();

                response.result.message = "";
                response.result.status = true;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        #region Sprint #2

        public static APIVersionCheck_response APIVersionCheck(APIVersionCheck_request request, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            APIVersionCheck_response response = new APIVersionCheck_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<AppVersionHistory> list = db.AppVersionHistories.Where(x => x.DeviceType == request.devicetype).ToList();
                Version currentVersion = new Version((request.versioname.Count(x => x == '.') == 0 ? request.versioname + ".0.0" : request.versioname.Count(x => x == '.') == 1 ? request.versioname + ".0" : request.versioname) + "." + request.versionid);
                if (list != null && list.Count > 0)
                {
                    foreach (AppVersionHistory appVersion in list)
                    {

                        Version appVersionToCheck = new
                        Version((appVersion.VersionName.Count(x => x == '.') == 0 ? appVersion.VersionName +
                        ".0.0" : appVersion.VersionName.Count(x => x == '.') == 1 ? appVersion.VersionName + ".0"
                        : appVersion.VersionName) + "." + appVersion.VersionID);
                        if (appVersionToCheck.CompareTo(currentVersion) > 0)
                        {
                            response.isneedtoupdate = true;
                            response.isrequiredtoupdate = appVersion.IsRequiredToUpdate;
                            break;
                        }
                    }
                }
                else
                {
                    response.isneedtoupdate = false;
                    response.isrequiredtoupdate = false;
                }

                response.result.status = true;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(""), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(""), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static GetCompanyLocations_response GetCompanyLocations(long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetCompanyLocations_response response = new GetCompanyLocations_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                AspNetUser user = db.AspNetUsers.Find(UserID);

                List<CompanyLocation> records = db.CompanyLocations.Where(x => x.CompanyID == user.CompanyID && x.IsDeleted == false).ToList();
                foreach (CompanyLocation obj in records)
                {
                    GetCompanyLocations_detail map = new GetCompanyLocations_detail();
                    map.address = obj.Address;
                    map.city = obj.City;
                    map.country = "";
                    map.latitude = obj.Latitude;
                    map.locationid = obj.ID;
                    map.longitude = obj.Longitude;
                    map.name = obj.Name;
                    map.state = obj.State;
                    map.zipcode = obj.Zipcode;

                    response.records.Add(map);
                }

                response.result.message = "";
                response.result.status = true;
                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static AppCommonResponse AddEmployeeLeave(AddEmployeeLeave_request model, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                AspNetUser user = db.AspNetUsers.Find(UserID);

                DateTime fromdate = Common.Common.GetDateTimeFromTimeStamp(model.from_timespan);
                DateTime todate = Common.Common.GetDateTimeFromTimeStamp(model.to_timespan);

                List<EmployeeLeaf> appliedleaves = user.EmployeeLeaves.Where(x =>
                (x.LeaveStatus == (int)AppEnum.LeaveStatusEnum.Pending || x.LeaveStatus == (int)AppEnum.LeaveStatusEnum.Sanctioned)
                &&
                x.FromDate >= fromdate && x.ToDate <= todate).ToList();

                if (appliedleaves.Count == 0)
                {
                    EmployeeLeaf obj = new EmployeeLeaf();
                    obj.ApplyRemarks = model.remarks;
                    obj.CompanyID = user.CompanyID;
                    obj.DateCreated = DateTime.UtcNow;
                    obj.DateModified = DateTime.UtcNow;
                    obj.DayTypeID = model.daytype;
                    obj.FromDate = fromdate;
                    obj.IsActive = true;
                    obj.IsPaidLeave = false;
                    obj.LeaveStatus = (int)AppEnum.LeaveStatusEnum.Pending;
                    obj.LeaveTypeID = model.leavetype;
                    obj.ToDate = todate;
                    obj.UserID = user.Id;

                    db.EmployeeLeaves.Add(obj);
                    db.SaveChanges();

                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {

                    response.result.message = "your have already applied leaves for this time periods";
                    response.result.status = false;
                }


                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static AppCommonResponse UpdateLeaveStatus(UpdateLeaveStatus_request model, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                EmployeeLeaf obj = db.EmployeeLeaves.Find(model.id);

                if (obj != null)
                {
                    obj.DateModified = DateTime.UtcNow;
                    obj.LeaveStatus = model.leavestatusid;
                    db.SaveChanges();

                    response.result.status = true;
                    response.result.message = "";
                }

                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static GetLeaveDetail_response GetLeaveDetail(GetLeaveDetail_request model, long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetLeaveDetail_response response = new GetLeaveDetail_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                EmployeeLeaf obj = db.EmployeeLeaves.Find(model.id);
                if(obj != null)
                {
                    GetLeaveDetail_Model map = new GetLeaveDetail_Model();
                    map.daytype = obj.DayTypeID;
                    map.daytypename = GetEnumDescription((AppEnum.DayTypeEnum)obj.DayTypeID);
                    map.from_timespan = ConvertToTimestamp(obj.FromDate);
                    map.id = obj.ID;
                    map.leavetype = obj.LeaveTypeID;
                    map.leavetypename = GetEnumDescription((AppEnum.LeaveTypeEnum)obj.LeaveTypeID);
                    map.remarks = !string.IsNullOrEmpty(obj.ApplyRemarks)?obj.ApplyRemarks:obj.ApplyRemarks;
                    map.statusname = GetEnumDescription((AppEnum.LeaveStatusEnum)obj.LeaveStatus);
                    map.statusid = obj.LeaveStatus;
                    map.to_timespan = ConvertToTimestamp(obj.ToDate);

                    response.record = map;
                }

                response.result.status = true;
                response.result.message = "";


                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }

        public static GetEmployeeLeaves_response GetEmployeeLeaves(long UserID, string url)
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetEmployeeLeaves_response response = new GetEmployeeLeaves_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<EmployeeLeaf> objs = db.EmployeeLeaves.Where(x => x.UserID == UserID).OrderByDescending(x => x.ID).ToList();
               foreach(EmployeeLeaf obj in objs)
                {
                    GetLeaveDetail_Model map = new GetLeaveDetail_Model();
                    map.daytype = obj.DayTypeID;
                    map.daytypename = GetEnumDescription((AppEnum.DayTypeEnum)obj.DayTypeID);
                    map.from_timespan = ConvertToTimestamp(obj.FromDate);
                    map.id = obj.ID;
                    map.leavetype = obj.LeaveTypeID;
                    map.leavetypename = GetEnumDescription((AppEnum.LeaveTypeEnum)obj.LeaveTypeID);
                    map.remarks = !string.IsNullOrEmpty(obj.ApplyRemarks) ? obj.ApplyRemarks : obj.ApplyRemarks;
                    map.statusname = GetEnumDescription((AppEnum.LeaveStatusEnum)obj.LeaveStatus);
                    map.statusid = obj.LeaveStatus;
                    map.to_timespan = ConvertToTimestamp(obj.ToDate);

                    response.records.Add(map);
                }

                response.result.status = true;
                response.result.message = "";


                endtime = DateTime.UtcNow;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                endtime = DateTime.UtcNow;
                response.result.message = "something went wrong!";
                response.result.status = false;
                Common.Common.AddAPIActivityLog(url, starttime, endtime, JsonConvert.SerializeObject(UserID), JsonConvert.SerializeObject(response), ex, false);
            }
            return response;
        }
        #endregion
    }
}
