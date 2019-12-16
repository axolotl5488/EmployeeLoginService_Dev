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
                   if(string.IsNullOrEmpty(obj.DeviceID))
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

                                        db.SaveChanges();
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
                        obj = new EmployeePunch();
                        obj.ClockInLatitude = request.in_lat;
                        obj.ClockInLongitude = request.in_lng;
                        obj.EarlyOuterReason = request.earlyoutouterreason;
                        obj.ClockInTime = Common.Common.GetDateTimeFromTimeStamp(request.in_datetime_timestamp);
                        obj.DateCreated = DateTime.UtcNow;
                        obj.IsSystemClockOut = false;
                        obj.LateComer = request.islatecomer;
                        obj.LateComerReason = request.latecomerreason;
                        obj.DateModified = DateTime.UtcNow;

                        db.EmployeePunches.Add(obj);
                        db.SaveChanges();

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

                    foreach (EmployeeTask objtask in obj.EmployeeTasks)
                    {
                        GetEmployeeTodaysPunchDetail_TaskList map = new GetEmployeeTodaysPunchDetail_TaskList();
                        map.Task = objtask.Task;
                        map.taskid = obj.ID;

                        response.record.tasks.Add(map);
                    }
                }
                else
                {
                    response.punchtype = (int)AppEnum.PunchTypeEnum.In;
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
                        map.taskid = obj.ID;

                        response.records.Add(map);
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

                    foreach (EmployeeTask objtask in obj.EmployeeTasks)
                    {
                        GetEmployeeTodaysPunchDetail_TaskList map = new GetEmployeeTodaysPunchDetail_TaskList();
                        map.Task = objtask.Task;
                        map.taskid = obj.ID;

                        response.record.tasks.Add(map);
                    }
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
                        map.taskid = obj.ID;

                        record.tasks.Add(map);
                    }

                    response.records.Add(record);
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
    }
}
