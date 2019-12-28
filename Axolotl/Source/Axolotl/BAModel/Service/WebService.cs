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
    public static class WebService
    {
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

        public static EmployeePunchList_Response EmployeePunchList()
        {
            EmployeePunchList_Response response = new EmployeePunchList_Response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<EmployeePunch> records = db.EmployeePunches.Include(x => x.AspNetUser).OrderByDescending(x => x.ID).ToList();

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
                        obj.NoOfWeekOffDays = obj.NoOfWeekOffDays;
                        obj.State = model.state;
                        obj.WorkingHoursInMinutes = model.workinghours;
                        obj.Zipcode = model.zipcode;
                        obj.PunchRangeInMeter = model.punchrangeinmeter;

                        db.Companies.Add(obj);
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
                            obj.NoOfWeekOffDays = obj.NoOfWeekOffDays;
                            obj.State = model.state;
                            obj.WorkingHoursInMinutes = model.workinghours;
                            obj.Zipcode = model.zipcode;
                            obj.PunchRangeInMeter = model.punchrangeinmeter;

                            db.SaveChanges();

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
                if(obj != null)
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

        #region Sprint 2
        public static GetCompanyLocaitonList_response GetCompanyLocaitonList(GetCompanyLocaitonList_request model)
        {
            GetCompanyLocaitonList_response response = new GetCompanyLocaitonList_response();
            try
            {

                AxolotlEntities db = new AxolotlEntities();
                List<CompanyLocation> records = db.CompanyLocations.OrderByDescending(x => x.ID).ToList();
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

                if(model.id == 0)
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
               
                if(obj != null)
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

                    foreach(EmployeeTask taskobj in obj.EmployeeTasks)
                    {
                        PunchTask_Model maptask = new PunchTask_Model();
                        maptask.Task = taskobj.Task;
                        maptask.taskid = taskobj.ID;

                        map.taskrecords.Add(maptask);
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
        #endregion 
    }
}
