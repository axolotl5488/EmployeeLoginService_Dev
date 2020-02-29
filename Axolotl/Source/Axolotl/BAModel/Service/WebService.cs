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
using System.Web;
using System.IO;

namespace BAModel.Service
{
    public static class WebService
    {
        #region Sprint 1
        public static GetCompany_Response GetCompanyList()
        {
            GetCompany_Response response = new GetCompany_Response();
            try
            {

                AxolotlEntities db = new AxolotlEntities();
                List<Company> records = db.Companies.OrderByDescending(x => x.ID).ToList();
                foreach (Company obj in records)
                {
                    GetCompany_Detail map = new GetCompany_Detail();
                    map.address = obj.Address;
                    map.city = obj.City;
                    map.datecreated = obj.DateCreated.ToString("dd/MM/yyyy");
                    map.flexiblebufferminutes = obj.FlexibleBufferMinutes;
                    map.id = obj.ID;
                    map.isdelete = obj.IsDelete;
                    map.mobile = obj.Mobile;
                    map.name = obj.Name;
                    map.noofweekoffdays = obj.NoOfWeekOffDays;
                    map.state = obj.State;
                    map.totalusers = obj.AspNetUsers.Count;
                    map.workinghoursinminutes = obj.WorkingHoursInMinutes;
                    map.zipcode = obj.Zipcode;

                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetUserList_Response GetUserList()
        {
            GetUserList_Response response = new GetUserList_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<AspNetUser> records = db.AspNetUsers.Include(x => x.Company).OrderByDescending(x => x.Id).ToList();
                foreach (AspNetUser obj in records)
                {
                    GetUserList_Detail map = new GetUserList_Detail();
                    map.companyid = obj.CompanyID;
                    map.companyname = obj.Company.Name;
                    map.datecreated = obj.DateCreated.ToString("dd/MM/yyyy");
                    map.devicetype = obj.DeviceType == null ? "" : Convert.ToString((AppEnum.DeviceTypeEnum)obj.DeviceType);
                    map.fullname = obj.FirstName + " " + obj.LastName;
                    map.id = obj.Id;
                    map.isdeleted = obj.IsDeleted;
                    map.officeshifttype = new DateTime(obj.OfficeShiftType.Ticks).ToString("hh:mm");
                    map.phonenumber = obj.PhoneNumber;
                    map.totalearlyout = obj.EmployeePunches.Where(x => x.EarlyOuter).Count();
                    map.totallatecommer = obj.EmployeePunches.Where(x => x.LateComer).Count();
                    map.userphotourl = obj.UserPhotoURL;
                    map.rolename = obj.CompanyRole?.Name;

                    if (obj.ReportingUserID != null)
                    {
                        AspNetUser reportingperson = obj.AspNetUser1;

                        map.reportingperson = reportingperson.FirstName + " " + reportingperson.LastName;
                    }
                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static EmployeePunchList_Response EmployeePunchList(EmployeePunchList_Request model)
        {
            EmployeePunchList_Response response = new EmployeePunchList_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<EmployeePunch> records = db.EmployeePunches
                    .Where(x => (model.userid == 0 || x.UserID == model.userid))
                    .Include(x => x.AspNetUser).OrderByDescending(x => x.ID).ToList();

                if (model.companyid > 0)
                {
                    records = records.Where(x => x.AspNetUser.CompanyID == model.companyid).ToList();
                }

                foreach (EmployeePunch obj in records)
                {
                    EmployeePunchList_Detail map = new EmployeePunchList_Detail();
                    map.clockinlatitude = obj.ClockInLatitude;
                    map.clockinlongitude = obj.ClockInLongitude;
                    map.clockintime = obj.ClockInTime.ToString("hh:mm");
                    map.clockoutlatitude = obj.ClockOutLatitude;
                    map.clockoutlongitude = obj.ClockOutLongitude;
                    map.clockouttime = obj.ClockOutTime != null ? obj.ClockOutTime.Value.ToString("hh:mm") : "missing";
                    map.date = obj.ClockInTime.ToString("dd/MM/yyyy");
                    map.earlyouter = obj.EarlyOuter;
                    map.earlyouterreason = obj.EarlyOuterReason;
                    map.id = obj.ID;
                    map.issystemclockout = obj.IsSystemClockOut;
                    map.latecomer = obj.LateComer;
                    map.latecomerreason = obj.LateComerReason;
                    map.totaltasks = obj.EmployeeTasks.Count;
                    map.userid = obj.UserID;
                    map.username = obj.AspNetUser.FirstName + " " + obj.AspNetUser.LastName;
                    if (obj.ClockOutTime == null)
                    {
                        map.workinghours = "00:00";
                    }
                    else
                    {
                        TimeSpan tp = obj.ClockOutTime.Value.Subtract(obj.ClockInTime);
                        map.workinghours = new DateTime(tp.Ticks).ToString("HH:mm");
                    }
                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static ManageCompany_Response ManageCompany(ManageCompany_Request model)
        {
            ManageCompany_Response response = new ManageCompany_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                if (!db.Companies.Any(x => x.Name.ToLower() == model.name.ToLower() && x.ID != model.id))
                {
                    if (model.id == 0)
                    {
                        Company obj = new Company();
                        obj.Address = model.address;
                        obj.City = model.city;
                        obj.DateCreated = DateTime.UtcNow;
                        obj.DateModified = DateTime.UtcNow;
                        obj.FlexibleBufferMinutes = model.flexiblebufferminutes;
                        obj.IsDelete = false;
                        obj.Mobile = model.mobile;
                        obj.Name = model.name;
                        obj.NoOfWeekOffDays = model.noofweekOffdays;
                        obj.State = model.state;
                        obj.WorkingHoursInMinutes = model.workinghours;
                        obj.Zipcode = model.zipcode;
                        obj.PunchRangeInMeter = model.punchrangeinmeter;
                        obj.FincialStartYear = DateTime.UtcNow;
                        obj.FincialEndYear = obj.FincialStartYear.AddYears(1);
                        db.Companies.Add(obj);

                        // Default Admin Role
                        CompanyRole objRole = new CompanyRole();
                        objRole.CompanyID = obj.ID;
                        objRole.Company = obj;
                        objRole.DateCreated = DateTime.UtcNow;
                        objRole.DateModified = DateTime.UtcNow;
                        objRole.Description = "Default Admin Role";
                        objRole.IsActive = true;
                        objRole.Name = AppEnum.CompanyDefaultRole.Admin.ToString();
                        db.CompanyRoles.Add(objRole);

                        // Default Employee Role
                        CompanyRole objEmployeeRole = new CompanyRole();
                        objEmployeeRole.CompanyID = obj.ID;
                        objEmployeeRole.Company = obj;
                        objEmployeeRole.DateCreated = DateTime.UtcNow;
                        objEmployeeRole.DateModified = DateTime.UtcNow;
                        objEmployeeRole.Description = "Default Employee Role";
                        objEmployeeRole.IsActive = true;
                        objEmployeeRole.Name = AppEnum.CompanyDefaultRole.Employee.ToString();
                        db.CompanyRoles.Add(objEmployeeRole);

                        db.SaveChanges();

                        response.result.message = "";
                        response.result.status = true;
                    }
                    else
                    {
                        Company obj = db.Companies.Find(model.id);

                        if (obj != null)
                        {
                            obj.Address = model.address;
                            obj.City = model.city;
                            obj.DateModified = DateTime.UtcNow;
                            obj.FlexibleBufferMinutes = model.flexiblebufferminutes;
                            obj.IsDelete = false;
                            obj.Mobile = model.mobile;
                            obj.Name = model.name;
                            obj.NoOfWeekOffDays = model.noofweekOffdays;
                            obj.State = model.state;
                            obj.WorkingHoursInMinutes = model.workinghours;
                            obj.Zipcode = model.zipcode;
                            obj.PunchRangeInMeter = model.punchrangeinmeter;

                            if (obj.CompanyRoles.Count == 0)
                            {
                                // Default Admin Role
                                CompanyRole objRole = new CompanyRole();
                                objRole.CompanyID = obj.ID;
                                objRole.Company = obj;
                                objRole.DateCreated = DateTime.UtcNow;
                                objRole.DateModified = DateTime.UtcNow;
                                objRole.Description = "Default Admin Role";
                                objRole.IsActive = true;
                                objRole.Name = AppEnum.CompanyDefaultRole.Admin.ToString();
                                db.CompanyRoles.Add(objRole);

                                // Default Employee Role
                                CompanyRole objEmployeeRole = new CompanyRole();
                                objEmployeeRole.CompanyID = obj.ID;
                                objEmployeeRole.Company = obj;
                                objEmployeeRole.DateCreated = DateTime.UtcNow;
                                objEmployeeRole.DateModified = DateTime.UtcNow;
                                objEmployeeRole.Description = "Default Employee Role";
                                objEmployeeRole.IsActive = true;
                                objEmployeeRole.Name = AppEnum.CompanyDefaultRole.Employee.ToString();
                                db.CompanyRoles.Add(objEmployeeRole);
                            }

                            db.SaveChanges();

                            response.IsCompanyhasAdmin = db.AspNetUsers.Any(x => x.CompanyRoleID == (int)AppEnum.CompanyDefaultRole.Admin);
                            response.result.message = "";
                            response.result.status = true;
                        }
                        else
                        {
                            response.result.message = "record not found!";
                            response.result.status = false;
                        }
                    }
                }
                else
                {
                    response.result.message = "company name is already exists!";
                    response.result.status = false;
                }
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetCompanyDetail_Response GetCompanyDetail(GetCompanyDetail_Request model)
        {
            GetCompanyDetail_Response response = new GetCompanyDetail_Response();
            try
            {

                AxolotlEntities db = new AxolotlEntities();
                Company obj = db.Companies.Find(model.id);
                if (obj != null)
                {
                    ManageCompany_Request map = new ManageCompany_Request();
                    map.address = obj.Address;
                    map.city = obj.City;
                    map.flexiblebufferminutes = obj.FlexibleBufferMinutes;
                    map.id = obj.ID;
                    map.mobile = obj.Mobile;
                    map.name = obj.Name;
                    map.noofweekOffdays = obj.NoOfWeekOffDays;
                    map.state = obj.State;
                    map.workinghours = obj.WorkingHoursInMinutes;
                    map.zipcode = obj.Zipcode;
                    map.punchrangeinmeter = obj.PunchRangeInMeter;

                    response.record = map;

                    response.result.message = "";
                    response.result.status = true;

                }
                else
                {
                    response.result.message = "records not found!";
                    response.result.status = false;
                }
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetUserDetail_Response GetUserDetail(GetUserDetail_Request model)
        {
            GetUserDetail_Response response = new GetUserDetail_Response();
            try
            {

                AxolotlEntities db = new AxolotlEntities();
                AspNetUser obj = db.AspNetUsers.Find(model.id);
                if (obj != null)
                {
                    SignUp_request map = new SignUp_request();
                    map.companyid = obj.CompanyID;
                    map.firstname = obj.FirstName;
                    map.id = obj.Id;
                    map.lastname = obj.LastName;
                    map.phonenumber = obj.PhoneNumber;
                    map.Shifttype = Common.Common.GetShiftTimeBySpan(new DateTime(obj.OfficeShiftType.Ticks).ToString("hh:mm"));
                    map.username = obj.UserName;
                    map.haveteam = obj.AspNetUsers1.Any();

                    response.record = map;
                    response.result.message = "";
                    response.result.status = true;

                }
                else
                {
                    response.result.message = "records not found!";
                    response.result.status = false;
                }
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static ResultStatus UpdateUserDetail(SignUp_request model)
        {
            ResultStatus response = new ResultStatus();
            try
            {

                AxolotlEntities db = new AxolotlEntities();
                AspNetUser obj = db.AspNetUsers.Find(model.id);
                if (obj != null)
                {
                    if (!db.AspNetUsers.Any(x => x.PhoneNumber == model.phonenumber && x.Id != model.id))
                    {
                        obj.FirstName = model.firstname;
                        obj.LastName = model.lastname;
                        obj.PhoneNumber = model.phonenumber;
                        obj.UserName = model.username;
                        obj.OfficeShiftType = Common.Common.GetShiftTimeByType(model.Shifttype);
                        obj.DateModified = DateTime.UtcNow;

                        db.SaveChanges();
                        response.message = "";
                        response.status = true;
                    }
                    else
                    {
                        response.message = "phone number is already registered with other user!";
                        response.status = false;
                    }

                }
                else
                {
                    response.message = "records not found!";
                    response.status = false;
                }
            }
            catch (Exception ex)
            {
                response.message = "something went wrong!";
                response.status = false;
            }
            return response;
        }
        #endregion 

        #region Sprint 2
        public static GetCompanyLocaitonList_response GetCompanyLocaitonList(GetCompanyLocaitonList_request model)
        {
            GetCompanyLocaitonList_response response = new GetCompanyLocaitonList_response();
            try
            {

                AxolotlEntities db = new AxolotlEntities();
                List<CompanyLocation> records = db.CompanyLocations.Where(x => x.CompanyID == model.companyid).OrderByDescending(x => x.ID).ToList();
                foreach (CompanyLocation obj in records)
                {
                    GetCompanyLocaitonList_Detail map = new GetCompanyLocaitonList_Detail();
                    map.address = obj.Address;
                    map.city = obj.City;
                    map.id = obj.ID;
                    map.isdeleted = obj.IsDeleted;
                    map.lat = obj.Latitude;
                    map.lng = obj.Longitude;
                    map.state = obj.State;
                    map.zipcode = obj.Zipcode;
                    map.name = obj.Name;

                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static ManageCompanyLocation_response ManageCompanyLocation(ManageCompanyLocation_request model)
        {
            ManageCompanyLocation_response response = new ManageCompanyLocation_response();
            try
            {

                AxolotlEntities db = new AxolotlEntities();

                if (model.id == 0)
                {
                    CompanyLocation obj = new CompanyLocation();
                    obj.Address = model.address;
                    obj.City = model.city;
                    obj.CompanyID = model.companyid;
                    obj.DateCreated = DateTime.UtcNow;
                    obj.DateModified = DateTime.UtcNow;
                    obj.IsDeleted = false;
                    obj.Latitude = model.lat;
                    obj.Longitude = model.lng;
                    obj.Name = model.name;
                    obj.State = model.state;
                    obj.Zipcode = model.zipcode;
                    obj.ImageURL = model.imageurl;
                    db.CompanyLocations.Add(obj);
                    db.SaveChanges();
                }
                else
                {

                    CompanyLocation obj = db.CompanyLocations.Find(model.id);
                    obj.Address = model.address;
                    obj.City = model.city;
                    obj.CompanyID = model.companyid;
                    obj.DateModified = DateTime.UtcNow;
                    obj.IsDeleted = false;
                    obj.Latitude = model.lat;
                    obj.Longitude = model.lng;
                    obj.Name = model.name;
                    obj.State = model.state;
                    obj.Zipcode = model.zipcode;
                    obj.ImageURL = model.imageurl;

                    db.SaveChanges();
                }

                response.result.status = true;
                response.result.message = "";

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetCompanyLocationDetail_response GetCompanyLocationDetail(GetCompanyLocationDetail_request model)
        {
            GetCompanyLocationDetail_response response = new GetCompanyLocationDetail_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                CompanyLocation obj = db.CompanyLocations.Find(model.locationid);
                ManageCompanyLocation_request map = new ManageCompanyLocation_request();

                map.address = obj.Address;
                map.city = obj.City;
                map.id = obj.ID;
                map.lat = obj.Latitude;
                map.lng = obj.Longitude;
                map.state = obj.State;
                map.zipcode = obj.Zipcode;
                map.name = obj.Name;
                map.companyid = obj.CompanyID;
                map.imageurl = obj.ImageURL;
                response.record = map;

                response.result.status = true;
                response.result.message = "";

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static AppCommonResponse ActiveInActiveCompanyLocation(GetCompanyLocationDetail_request model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                CompanyLocation obj = db.CompanyLocations.Find(model.locationid);

                if (obj != null)
                {
                    obj.IsDeleted = !obj.IsDeleted;
                    obj.DateModified = DateTime.UtcNow;
                    db.SaveChanges();
                }

                response.result.status = true;
                response.result.message = "";

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetEmployeePunchDetailWeb_response GetEmployeePunchDetail(GetEmployeePunchDetailWeb_request model)
        {
            GetEmployeePunchDetailWeb_response response = new GetEmployeePunchDetailWeb_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                EmployeePunch obj = db.EmployeePunches.Find(model.punchid);

                if (obj != null)
                {
                    EmployeePunchList_Detail map = new EmployeePunchList_Detail();
                    map.clockinlatitude = obj.ClockInLatitude;
                    map.clockinlongitude = obj.ClockInLongitude;
                    map.clockintime = obj.ClockInTime.ToString("hh:mm");
                    map.clockoutlatitude = obj.ClockOutLatitude;
                    map.clockoutlongitude = obj.ClockOutLongitude;
                    map.clockouttime = obj.ClockOutTime != null ? obj.ClockOutTime.Value.ToString("hh:mm") : "no record";
                    map.date = obj.ClockInTime.ToString("dd/MM/yyyy");
                    map.earlyouter = obj.EarlyOuter;
                    map.earlyouterreason = obj.EarlyOuterReason;
                    map.id = obj.ID;
                    map.issystemclockout = obj.IsSystemClockOut;
                    map.latecomer = obj.LateComer;
                    map.latecomerreason = obj.LateComerReason;
                    map.totaltasks = obj.EmployeeTasks.Count;
                    map.userid = obj.UserID;
                    map.username = obj.AspNetUser.FirstName + " " + obj.AspNetUser.LastName;
                    if (obj.ClockOutTime == null)
                    {
                        map.workinghours = "00:00";
                    }
                    else
                    {
                        TimeSpan tp = obj.ClockOutTime.Value.Subtract(obj.ClockInTime);
                        map.workinghours = new DateTime(tp.Ticks).ToString("HH:mm");
                    }

                    foreach (EmployeeTask taskobj in obj.EmployeeTasks)
                    {
                        PunchTask_Model maptask = new PunchTask_Model();
                        maptask.Task = taskobj.Task;
                        maptask.taskid = taskobj.ID;
                        maptask.TaskStatusID = taskobj.Status;
                        maptask.TaskStatus = GetEnumDescription((AppEnum.EmployeeTaskStatus)taskobj.Status);
                        map.taskrecords.Add(maptask);
                    }

                    foreach (EmployeeCall objcall in obj.EmployeeCalls)
                    {
                        GetEmployeeCallList_detail_Webportal mapcall = new GetEmployeeCallList_detail_Webportal();
                        mapcall.callfor = objcall.CallFor;
                        mapcall.companyid = objcall.CompanyID;
                        mapcall.id = objcall.ID;
                        mapcall.punchid = objcall.EmployeePunchID;
                        mapcall.remarks = objcall.Remarks;
                        mapcall.start_datetime_timestamp = objcall.StartDateTime.ToString("dd/MM/yyyy hh:mm tt");
                        mapcall.start_lat = objcall.StartLatitude;
                        mapcall.start_lng = objcall.StartLongitude;
                        mapcall.title = objcall.Title;
                        if (objcall.EndDateTime != null)
                        {
                            mapcall._end_datetime_timestamp = objcall.EndDateTime.Value.ToString("dd/MM/yyyy hh:mm tt");
                            mapcall._end_lat = objcall.EndLatitude;
                            mapcall._end_lng = objcall.EndLongitude;
                        }

                        map.callrecords.Add(mapcall);
                    }
                    response.record = map;
                }

                response.result.status = true;
                response.result.message = "";

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static AppCommonResponse ManageCompanyHolidays(MangeCompantHolidays_request model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                DateTime holidaydate = Convert.ToDateTime(model.date);
                if (!db.CompanyHolidays.Any(x => x.Date == holidaydate && x.CompanyID == model.companyid && x.ID != model.id))
                {
                    CompanyHoliday obj = new CompanyHoliday();
                    if (model.id == 0)
                    {
                        obj.CompanyID = model.companyid;
                        obj.Date = holidaydate;
                        obj.DateCreated = DateTime.UtcNow;
                        obj.DateModified = DateTime.UtcNow;
                        obj.Description =  model.description;
                        obj.IsActive = true;
                        obj.Name = model.Name;

                        db.CompanyHolidays.Add(obj);
                    }
                    else
                    {
                        obj = db.CompanyHolidays.Find(model.id);
                        obj.CompanyID = model.companyid;
                        obj.Date = holidaydate;
                        obj.DateCreated = DateTime.UtcNow;
                        obj.DateModified = DateTime.UtcNow;
                        obj.Description = model.description;
                        obj.IsActive = true;
                        obj.Name = model.Name;
                    }
                    db.SaveChanges();
                    response.result.status = true;
                    response.result.message = "";
                }
                else
                {
                    response.result.status = false;
                    response.result.message = "Same date holiday is exists!";
                }



            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetCompanyHolidayDetail_response GetCompanyHolidayDetail(GetCompanyHolidayDetail_request model)
        {
            GetCompanyHolidayDetail_response response = new GetCompanyHolidayDetail_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                CompanyHoliday obj = db.CompanyHolidays.Find(model.id);
                response.record.companyid = obj.CompanyID;
                response.record.date = obj.Date.ToString("dd/MM/yyyy");
                response.record.description = obj.Description;
                response.record.Name = obj.Name;
                response.record.id = obj.ID;
                response.result.status = true;
                response.result.message = "";

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetCompanyHolidayList_response GetCompanyHolidayList(GetCompanyHolidayList_request model)
        {
            GetCompanyHolidayList_response response = new GetCompanyHolidayList_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<CompanyHoliday> holidays = db.CompanyHolidays.Where(x => x.CompanyID == model.companyid).ToList();

                List<DateTime> holidaysgroupbyyear = holidays.GroupBy(x => x.Date.Year).Select(x => x.First().Date).ToList();

                foreach (DateTime year in holidaysgroupbyyear)
                {
                    GetCompanyHolidayList_yeardetail record = new GetCompanyHolidayList_yeardetail();
                    record.year = year.Year.ToString();
                    foreach (CompanyHoliday obj in holidays.Where(x => x.Date.Year == year.Year))
                    {
                        GetCompanyHolidayList_detail item = new GetCompanyHolidayList_detail();
                        item.companyid = obj.CompanyID;
                        item.companyname = obj.Company.Name;
                        item.date = obj.Date.ToString("dd/MM/yyyy");
                        item.description = obj.Description;
                        item.id = obj.ID;
                        item.isactive = obj.IsActive;
                        item.Name = obj.Name;
                        item.year = obj.Date.ToString("yyyy");

                        record.records.Add(item);
                    }

                    response.records.Add(record);
                }
                response.result.status = true;
                response.result.message = "";

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static AppCommonResponse ActiveInActiveCompanyHolidays(GetCompanyHolidayDetail_request model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                CompanyHoliday obj = db.CompanyHolidays.Find(model.id);

                if (obj != null)
                {
                    obj.IsActive = !obj.IsActive;
                    obj.DateModified = DateTime.UtcNow;
                    db.SaveChanges();
                }

                response.result.status = true;
                response.result.message = "";

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }


        public static EmployeeleaveList_Response EmployeeLeaveList(EmployeeleaveList_request model)
        {
            EmployeeleaveList_Response response = new EmployeeleaveList_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<EmployeeLeaf> records = db.EmployeeLeaves.Where(x =>
                    (model.companyid == 0 || x.CompanyID == model.companyid) && (model.userid == 0 || x.UserID == model.userid)
                    ).Include(x => x.AspNetUser).OrderByDescending(x => x.ID).ToList();

                foreach (EmployeeLeaf obj in records)
                {
                    EmployeeleaveList_Detail map = new EmployeeleaveList_Detail();

                    map.approvalremarks = obj.ApprovaRemarks;
                    map.companyid = obj.CompanyID;
                    map.companyname = obj.Company.Name;
                    map.daytype = GetEnumDescription((AppEnum.DayTypeEnum)obj.DayTypeID);
                    map.daytypeid = obj.DayTypeID;
                    map.fromdate = obj.FromDate.ToString("dd/MM/yyyy");
                    map.id = obj.ID;
                    map.ispaidleave = obj.IsPaidLeave;
                    map.leavestatus = GetEnumDescription((AppEnum.LeaveStatusEnum)obj.LeaveStatus);
                    map.leavestatusid = obj.LeaveStatus;
                    map.leavetype = GetEnumDescription((AppEnum.LeaveTypeEnum)obj.LeaveTypeID);
                    map.leavetypeid = obj.LeaveTypeID;
                    map.todate = obj.ToDate.ToString("dd/MM/yyyy");
                    map.userid = obj.UserID;
                    map.username = obj.AspNetUser.FirstName + " " + obj.AspNetUser.LastName;
                    map.userremarkd = obj.ApplyRemarks;

                    double daydiff = obj.ToDate.Subtract(obj.FromDate).TotalDays;
                    if (map.daytypeid == (int)AppEnum.DayTypeEnum.HalfLeave)
                    {
                        map.totaldays = 0.5;
                    }
                    else
                    {
                        map.totaldays = Math.Round(daydiff, 2);
                    }
                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }
        #endregion

        #region Sprint #3/4
        public static GetCompanyRolesList_response GetCompanyRolesList(GetCompanyRolesList_request model)
        {
            GetCompanyRolesList_response response = new GetCompanyRolesList_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<CompanyRole> records = db.CompanyRoles.Where(x => x.CompanyID == model.companyid).ToList();

                foreach (CompanyRole obj in records)
                {
                    GetCompanyRolesList_detail map = new GetCompanyRolesList_detail();
                    map.companyid = obj.CompanyID;
                    map.description = obj.Description;
                    map.id = obj.ID;
                    map.isactive = obj.IsActive;
                    map.lastmodified = obj.DateModified.ToString("dd/MM/yyyy");
                    map.totalmembers = obj.AspNetUsers.Count;
                    map.name = obj.Name;

                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static AppCommonResponse ManageCompanyRoles(ManageCompanyRoles_request model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                if (!db.CompanyRoles.Any(x => x.CompanyID == model.companyid && x.ID != model.id && x.Name.ToLower() == model.name.ToLower()))
                {
                    CompanyRole obj = new CompanyRole();
                    if (model.id == 0)
                    {
                        obj.CompanyID = model.companyid;
                        obj.DateCreated = DateTime.UtcNow;
                        obj.DateModified = DateTime.UtcNow;
                        obj.Description = model.description;
                        obj.IsActive = true;
                        obj.Name = model.name;

                        db.CompanyRoles.Add(obj);
                    }
                    else
                    {
                        obj = db.CompanyRoles.Find(model.id);
                        if (obj != null)
                        {
                            obj.DateModified = DateTime.UtcNow;
                            obj.Description = model.description;
                            obj.IsActive = true;
                            obj.Name = model.name;
                        }
                    }
                    db.SaveChanges();

                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {
                    response.result.message = "role is already exist!";
                    response.result.status = false;
                }
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static AppCommonResponse RemoveCompanyRoles(GetCompanyRoleDetail_request model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                CompanyRole obj = db.CompanyRoles.Find(model.id);
                if (obj != null)
                {
                    // Convert all assinged employee role
                    CompanyRole employeeRole = db.CompanyRoles.FirstOrDefault(x => x.CompanyID == obj.CompanyID && x.Name == AppEnum.CompanyDefaultRole.Employee.ToString());
                    if (employeeRole != null)
                    {
                        List<AspNetUser> users = obj.AspNetUsers.ToList();
                        foreach (AspNetUser user in users)
                        {
                            user.CompanyRoleID = employeeRole.ID;
                            user.DateModified = DateTime.UtcNow;
                        }
                        db.SaveChanges();
                    }

                    // Remove Company Role Permission 
                    List<CompanyRolePermission> rolepermissionlist = obj.CompanyRolePermissions.ToList();

                    foreach (CompanyRolePermission objrolepermision in rolepermissionlist)
                    {
                        db.CompanyRolePermissions.Remove(objrolepermision);
                    }
                    db.SaveChanges();

                    // Remove Company Role
                    db.CompanyRoles.Remove(obj);
                    db.SaveChanges();
                }

                response.result.message = "";
                response.result.status = true;
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetCompanyRoleDetail_response GetCompanyRoleDetail(GetCompanyRoleDetail_request model)
        {
            GetCompanyRoleDetail_response response = new GetCompanyRoleDetail_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                CompanyRole obj = db.CompanyRoles.Find(model.id);
                if (obj != null)
                {
                    ManageCompanyRoles_request map = new ManageCompanyRoles_request();
                    map.companyid = obj.CompanyID;
                    map.description = obj.Description;
                    map.id = obj.ID;
                    map.name = obj.Name;

                    response.record = map;
                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {
                    response.result.message = "record does not exist!";
                    response.result.status = false;
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetCompanyRolesPermissionList_response GetCompanyRolesPermissionList(GetCompanyRolesPermissionList_request model)
        {
            GetCompanyRolesPermissionList_response response = new GetCompanyRolesPermissionList_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                List<CompanyRole> recordsgroupbyrole = db.CompanyRoles.Where(x => x.CompanyID == model.companyid).Include(x => x.CompanyRolePermissions).ToList();

                List<AppEnum.AppScreen> screenlist = Enum.GetValues(typeof(AppEnum.AppScreen)).Cast<AppEnum.AppScreen>().ToList();
                foreach (CompanyRole obj in recordsgroupbyrole)
                {
                    GetCompanyRolesPermissionList_Role_detail map = new GetCompanyRolesPermissionList_Role_detail();
                    map.companyid = obj.CompanyID;
                    map.id = obj.ID;
                    map.name = obj.Name;

                    foreach (AppEnum.AppScreen item in screenlist)
                    {
                        GetCompanyRolesPermissionList_RolePermission_detail mapitem = new GetCompanyRolesPermissionList_RolePermission_detail();

                        mapitem.companyid = obj.CompanyID;
                        mapitem.companyroleid = obj.ID;
                        mapitem.screenname = GetEnumDescription((AppEnum.AppScreen)item);
                        mapitem.screenid = (int)item;
                        mapitem.isactive = false;
                        CompanyRolePermission objrolepermision = obj.CompanyRolePermissions.FirstOrDefault(x => x.ScreenID == (int)item);
                        if (objrolepermision != null)
                        {
                            mapitem.id = objrolepermision.ID;
                            mapitem.lastmodified = objrolepermision.DateModified.ToString("dd/MM/yyyy");
                            mapitem.isactive = true;
                        }
                        map.items.Add(mapitem);
                    }

                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static AppCommonResponse ManageCompanyRolesPermission(ManageCompanyRolesPermission_request model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();

                if (model.companyrolepermissionid == 0)
                {
                    CompanyRolePermission obj = new CompanyRolePermission();
                    obj.CompanyID = model.companyid;
                    obj.CompanyRoleID = model.companyroleid;
                    obj.DateCreated = DateTime.UtcNow;
                    obj.DateModified = DateTime.UtcNow;
                    obj.ScreenID = model.screenid;
                    obj.ScreenName = GetEnumDescription((AppEnum.AppScreen)model.screenid);

                    db.CompanyRolePermissions.Add(obj);
                    db.SaveChanges();
                }
                else
                {
                    CompanyRolePermission obj = db.CompanyRolePermissions.Find(model.companyrolepermissionid);
                    if (obj != null)
                    {
                        db.CompanyRolePermissions.Remove(obj);
                        db.SaveChanges();
                    }
                }

                response.result.message = "";
                response.result.status = true;
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static ManageTeamList_response ManageTeamList(ManageTeamList_request model)
        {
            ManageTeamList_response response = new ManageTeamList_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<AspNetUser> users = db.AspNetUsers.Where(x => x.CompanyID == model.companyid).ToList();
                List<CompanyRole> roles = db.CompanyRoles.Where(x => x.CompanyID == model.companyid).ToList();
                CompanyRole employeerole = roles.FirstOrDefault(x => x.Name == AppEnum.CompanyDefaultRole.Employee.ToString());
                List<AspNetUser> reportingusers = new List<AspNetUser>();
                if (employeerole != null)
                {
                    reportingusers = users.Where(x => x.CompanyRoleID != employeerole.ID).ToList();
                }
                else
                {
                    reportingusers = users;
                }

                foreach (AspNetUser obj in users)
                {
                    int totalMember = users.Where(x => x.ReportingUserID == obj.Id).Count();
                    ManageTeamList_detail map = new ManageTeamList_detail();
                    map.name = obj.FirstName + " " + obj.LastName;
                    map.reportingperson = obj.ReportingUserID;
                    map.totalmember = totalMember;
                    map.userid = obj.Id;

                    map.roleid = obj.CompanyRoleID == null ? 0 : obj.CompanyRoleID.Value;
                    if (map.roleid > 0)
                        map.rolename = obj.CompanyRole.Name;
                    else
                        map.rolename = AppEnum.CompanyDefaultRole.Employee.ToString();

                    response.records.Add(map);
                }

                foreach (AspNetUser obj in reportingusers)
                {
                    ManageTeamList_reporting_person_detail map = new ManageTeamList_reporting_person_detail();
                    map.name = obj.FirstName + " " + obj.LastName;
                    map.userid = obj.Id;

                    response.reportingusers.Add(map);
                }

                foreach (CompanyRole obj in roles)
                {
                    ManageTeamList_reporting_role_detail map = new ManageTeamList_reporting_role_detail();
                    map.name = obj.Name;
                    map.roleid = obj.ID;

                    response.roles.Add(map);
                }
                response.result.message = "";
                response.result.status = true;
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static AppCommonResponse ManageTeam(ManageTeam_request model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                AspNetUser user = db.AspNetUsers.Find(model.userid);

                if (user != null)
                {
                    user.DateModified = DateTime.UtcNow;
                    user.CompanyRoleID = model.roleid;
                    user.ReportingUserID = model.reportingpersonid;
                    db.SaveChanges();
                }

                response.result.message = "";
                response.result.status = true;
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetUserList_Response GetMyteamEmployeeList(ReportingPerson_request model)
        {
            GetUserList_Response response = new GetUserList_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<AspNetUser> records = db.AspNetUsers.Where(x => x.ReportingUserID == model.userid).OrderByDescending(x => x.Id).ToList();
                foreach (AspNetUser obj in records)
                {
                    GetUserList_Detail map = new GetUserList_Detail();
                    map.companyid = obj.CompanyID;
                    map.companyname = obj.Company.Name;
                    map.datecreated = obj.DateCreated.ToString("dd/MM/yyyy");
                    map.devicetype = obj.DeviceType == null ? "" : Convert.ToString((AppEnum.DeviceTypeEnum)obj.DeviceType);
                    map.fullname = obj.FirstName + " " + obj.LastName;
                    map.id = obj.Id;
                    map.isdeleted = obj.IsDeleted;
                    map.officeshifttype = new DateTime(obj.OfficeShiftType.Ticks).ToString("hh:mm");
                    map.phonenumber = obj.PhoneNumber;
                    map.totalearlyout = obj.EmployeePunches.Where(x => x.EarlyOuter).Count();
                    map.totallatecommer = obj.EmployeePunches.Where(x => x.LateComer).Count();
                    map.userphotourl = obj.UserPhotoURL;
                    map.rolename = obj.CompanyRole?.Name;

                    if (obj.ReportingUserID != null)
                    {
                        AspNetUser reportingperson = obj.AspNetUser1;

                        map.reportingperson = reportingperson.FirstName + " " + reportingperson.LastName;
                    }
                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static EmployeePunchList_Response GetMyteamEmployeePunchList(ReportingPerson_request model)
        {
            EmployeePunchList_Response response = new EmployeePunchList_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<long> ids = db.AspNetUsers.Where(x => x.ReportingUserID == model.userid).Select(x => x.Id).ToList();
                List<EmployeePunch> records = db.EmployeePunches
                    .Where(x => ids.Contains(x.UserID))
                    .Include(x => x.AspNetUser).OrderByDescending(x => x.ID).ToList();

                if (model.companyid > 0)
                {
                    records = records.Where(x => x.AspNetUser.CompanyID == model.companyid).ToList();
                }

                foreach (EmployeePunch obj in records)
                {
                    EmployeePunchList_Detail map = new EmployeePunchList_Detail();
                    map.clockinlatitude = obj.ClockInLatitude;
                    map.clockinlongitude = obj.ClockInLongitude;
                    map.clockintime = obj.ClockInTime.ToString("hh:mm");
                    map.clockoutlatitude = obj.ClockOutLatitude;
                    map.clockoutlongitude = obj.ClockOutLongitude;
                    map.clockouttime = obj.ClockOutTime != null ? obj.ClockOutTime.Value.ToString("hh:mm") : "no record";
                    map.date = obj.ClockInTime.ToString("dd/MM/yyyy");
                    map.earlyouter = obj.EarlyOuter;
                    map.earlyouterreason = obj.EarlyOuterReason;
                    map.id = obj.ID;
                    map.issystemclockout = obj.IsSystemClockOut;
                    map.latecomer = obj.LateComer;
                    map.latecomerreason = obj.LateComerReason;
                    map.totaltasks = obj.EmployeeTasks.Count;
                    map.userid = obj.UserID;
                    map.username = obj.AspNetUser.FirstName + " " + obj.AspNetUser.LastName;
                    if (obj.ClockOutTime == null)
                    {
                        map.workinghours = "00:00";
                    }
                    else
                    {
                        TimeSpan tp = obj.ClockOutTime.Value.Subtract(obj.ClockInTime);
                        map.workinghours = new DateTime(tp.Ticks).ToString("HH:mm");
                    }
                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static EmployeeleaveList_Response GetMyTeamEmployeeLeaveList(ReportingPerson_request model)
        {
            EmployeeleaveList_Response response = new EmployeeleaveList_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<long> ids = db.AspNetUsers.Where(x => x.ReportingUserID == model.userid).Select(x => x.Id).ToList();
                List<EmployeeLeaf> records = db.EmployeeLeaves.Where(x =>
                    ids.Contains(x.UserID)
                    ).Include(x => x.AspNetUser).OrderByDescending(x => x.ID).ToList();

                foreach (EmployeeLeaf obj in records)
                {
                    EmployeeleaveList_Detail map = new EmployeeleaveList_Detail();

                    map.approvalremarks = obj.ApprovaRemarks;
                    map.companyid = obj.CompanyID;
                    map.companyname = obj.Company.Name;
                    map.daytype = GetEnumDescription((AppEnum.DayTypeEnum)obj.DayTypeID);
                    map.daytypeid = obj.DayTypeID;
                    map.fromdate = obj.FromDate.ToString("dd/MM/yyyy");
                    map.id = obj.ID;
                    map.ispaidleave = obj.IsPaidLeave;
                    map.leavestatus = GetEnumDescription((AppEnum.LeaveStatusEnum)obj.LeaveStatus);
                    map.leavestatusid = obj.LeaveStatus;
                    map.leavetype = GetEnumDescription((AppEnum.LeaveTypeEnum)obj.LeaveTypeID);
                    map.leavetypeid = obj.LeaveTypeID;
                    map.todate = obj.ToDate.ToString("dd/MM/yyyy");
                    map.userid = obj.UserID;
                    map.username = obj.AspNetUser.FirstName + " " + obj.AspNetUser.LastName;
                    map.userremarkd = obj.ApplyRemarks;

                    double daydiff = obj.ToDate.Subtract(obj.FromDate).TotalDays;
                    if (map.daytypeid == (int)AppEnum.DayTypeEnum.HalfLeave)
                    {
                        map.totaldays = 0.5;
                    }
                    else
                    {
                        map.totaldays = Math.Round(daydiff, 2);
                    }
                    response.records.Add(map);
                }

            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }
        #endregion

        #region Sprint #5

        public static AppCommonResponse ManageEmployeeWeekOffs(GetEmployeeWeekOffs_detail model)
        {
            AppCommonResponse response = new AppCommonResponse();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                if (model.companyweekoffdays > 0)
                {
                    if (model.isadd && model.companyweekoffdays > 0)
                    {
                        List<EmployeeWeekOff> records = db.EmployeeWeekOffs.Where(x => x.UserID == model.employeeid && x.WeekNo == model.weekno).ToList();
                        if (records.Count > 0 && records.Count >= model.companyweekoffdays)
                        {
                            EmployeeWeekOff objweekoff = records.OrderBy(x => x.ID).FirstOrDefault();
                            if (objweekoff != null)
                            {
                                db.EmployeeWeekOffs.Remove(objweekoff);
                                db.SaveChanges();
                            }
                        }

                        EmployeeWeekOff map = new EmployeeWeekOff();
                        map.CompanyID = model.companyid;
                        map.DateCreated = DateTime.UtcNow;
                        map.DateModified = DateTime.UtcNow;
                        map.Day = model.day;
                        map.UserID = model.employeeid;
                        map.WeekNo = model.weekno;
                        db.EmployeeWeekOffs.Add(map);
                        db.SaveChanges();
                    }
                    else
                    {
                        EmployeeWeekOff objweekoff = db.EmployeeWeekOffs.Find(model.id);
                        if (objweekoff != null)
                        {
                            db.EmployeeWeekOffs.Remove(objweekoff);
                            db.SaveChanges();
                        }
                    }

                    response.result.message = "";
                    response.result.status = true;
                }
                else
                {
                    response.result.message = "company's weekoff day is zero";
                    response.result.status = false;
                }
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static GetEmployeeWeekOffs_response GetEmployeeWeekOffs(GetEmployeeWeekOffs_request model)
        {
            GetEmployeeWeekOffs_response response = new GetEmployeeWeekOffs_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                
                List<EmployeeWeekOff> records = db.EmployeeWeekOffs.Where(x => x.UserID == model.employeeid).ToList();
                List<int> weeklist = Enum.GetValues(typeof(AppEnum.EmployeeWeekOff_Enum)).Cast<int>().ToList();
                List<string> daylist = Enum.GetNames(typeof(DayOfWeek)).Cast<string>().ToList();
                AspNetUser employee = db.AspNetUsers.Find(model.employeeid);
                Company objcompany = employee.Company;

                foreach(int week in weeklist)
                {
                    GetEmployeeWeekOffs_weekdetail weekmap = new GetEmployeeWeekOffs_weekdetail();
                    weekmap.week = GetEnumDescription((AppEnum.EmployeeWeekOff_Enum)week);
                    foreach (string day in daylist)
                    {
                        EmployeeWeekOff obj = records.FirstOrDefault(x => x.WeekNo == week && x.Day == day);
                        if (obj != null)
                        {
                            GetEmployeeWeekOffs_detail map = new GetEmployeeWeekOffs_detail();
                            map.companyid = objcompany.ID;
                            map.companyweekoffdays = objcompany.NoOfWeekOffDays;
                            map.day = day;
                            map.employeeid = model.employeeid;
                            map.id = obj.ID;
                            map.isadd = true;
                            map.week = GetEnumDescription((AppEnum.EmployeeWeekOff_Enum)week);
                            map.weekno = week;
                            weekmap.records.Add(map);
                        }
                        else
                        {
                            GetEmployeeWeekOffs_detail map = new GetEmployeeWeekOffs_detail();
                            map.companyid = objcompany.ID;
                            map.companyweekoffdays = objcompany.NoOfWeekOffDays;
                            map.day = day;
                            map.employeeid = model.employeeid;
                            map.id = 0;
                            map.isadd = false;
                            map.week = GetEnumDescription((AppEnum.EmployeeWeekOff_Enum)week);
                            map.weekno = week;

                            weekmap.records.Add(map);
                        }
                    }
                    response.records.Add(weekmap);

                }
                response.result.message = "";
                response.result.status = true;
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }
            return response;
        }

        public static UploadImage_Response UploadImage(HttpPostedFile file)
        {
            UploadImage_Response response = new UploadImage_Response();
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                string[] fileextension = file.FileName.Split('.');
                string fileext = fileextension[fileextension.Length - 1];
                string updatedfilename = "axolotlimage_" + DateTime.Now.ToString("MMddyyyyhhmmss") + "." + fileext;

                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Images")))
                {
                    System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Images"));
                }

                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Images/AxololtImages")))
                {
                    System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Images/AxololtImages"));
                }
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/AxololtImages"), updatedfilename);
                file.SaveAs(path);

                response.filename = updatedfilename;


                response.response.status = true;
                response.response.message = "";
            }
            catch (Exception ex)
            {
                response.response.status = false;
                response.response.message = ex.Message;
            }
            return response;
        }
        #endregion

    }
}
