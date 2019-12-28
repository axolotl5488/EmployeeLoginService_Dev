using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAModel.Model;

namespace BAModel.Model
{
    public class GetEmployeeLeaves_response
    {
        public List<GetLeaveDetail_Model> records { get; set; }
        public ResultStatus result { get; set; }

        public GetEmployeeLeaves_response()
        {
            result = new ResultStatus();
            records = new List<GetLeaveDetail_Model>();
        }

    }
    public class GetLeaveDetail_request
    {
        public long id { get; set; }
    }

    public class GetLeaveDetail_response
    {
        public GetLeaveDetail_Model record { get; set; }
        public ResultStatus result { get; set; }

        public GetLeaveDetail_response()
        {
            result = new ResultStatus();
            record = new GetLeaveDetail_Model();
        }

    }

    public class GetLeaveDetail_Model
    {
        public long id { get; set; }
        public double from_timespan { get; set; }
        public double to_timespan { get; set; }
        public int daytype { get; set; }
        public string daytypename { get; set; }
        public string leavetypename { get; set; }
        public int leavetype { get; set; }
        public string remarks { get; set; }
        public string statusname { get; set; }
        public int statusid { get; set; }
    }

    public class UpdateLeaveStatus_request
    {
        public long id { get; set; }
        public int leavestatusid { get; set; }
    }
    public class AddEmployeeLeave_request
    {
        public long id { get; set; }
        public double from_timespan { get; set; }
        public double to_timespan { get; set; }
        public int daytype { get; set; }
        public int leavetype { get; set; }
        public string remarks { get; set; }
    }

    public class GetCompanyLocations_response
    {
        public List<GetCompanyLocations_detail> records { get; set; }
        public ResultStatus result { get; set; }

        public GetCompanyLocations_response()
        {
            result = new ResultStatus();
            records = new List<GetCompanyLocations_detail>();
        }

    }
    public class GetCompanyLocations_detail
    {
        public long locationid { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zipcode { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }
    public class APIVersionCheck_request
    {
        public string versioname { get; set; }

        public string versionid { get; set; }
        public int devicetype { get; set; }
    }
    public class APIVersionCheck_response
    {
        public bool isrequiredtoupdate { get; set; }
        public bool isneedtoupdate { get; set; }
        public ResultStatus result { get; set; }

        public APIVersionCheck_response()
        {
            result = new ResultStatus();
        }
    }

    public class ChangePassword_Request
    {
        public string oldpassword { get; set; }
        public string newpasswprd { get; set; }
    }

    public class ChangePassword_response
    {
        public ResultStatus result { get; set; }

        public ChangePassword_response()
        {
            result = new ResultStatus();
        }
    }
    public class GetEmployeePunchList_response
    {
        public ResultStatus result { get; set; }

        public List<GetEmployeeTodaysPunchDetail> records { get; set; }
        public GetEmployeePunchList_response()
        {
            result = new ResultStatus();
            records = new List<GetEmployeeTodaysPunchDetail>();
        }
    }

    public class GetEmployeePunchDetail_request
    {
        public long punchid { get; set; }
    }

    public class GetEmployeePunchDetail_response
    {
        public ResultStatus result { get; set; }

        public GetEmployeeTodaysPunchDetail record { get; set; }
        public GetEmployeePunchDetail_response()
        {
            result = new ResultStatus();
            record = new GetEmployeeTodaysPunchDetail();
        }
    }

    public class GetPunchTasks_request
    {
        public long punchid { get; set; }

    }

    public class GetPunchTasks_response
    {
        public ResultStatus result { get; set; }

        public List<GetEmployeeTodaysPunchDetail_TaskList> records { get; set; }

        public GetPunchTasks_response()
        {
            result = new ResultStatus();
            records = new List<GetEmployeeTodaysPunchDetail_TaskList>();
        }
    }

    public class AddPunchTask_request
    {
        public long punchid { get; set; }

        public string task { get; set; }
    }

    public class AddPunchTask_response
    {
        public ResultStatus result { get; set; }

        public AddPunchTask_response()
        {
            result = new ResultStatus();
        }
    }

    public class GetEmployeeTodaysPunchDetail_request
    {
        public double date_timestamp { get; set; }
    }

    public class GetEmployeeTodaysPunchDetail_response
    {
        public int punchtype { get; set; }
        public GetEmployeeTodaysPunchDetail record { get; set; }
        public ResultStatus result { get; set; }

        public GetEmployeeTodaysPunchDetail_response()
        {
            result = new ResultStatus();
            record = new GetEmployeeTodaysPunchDetail();
        }
    }

    public class GetEmployeeTodaysPunchDetail
    {
        public long punchid { get; set; }
        public double in_datetime_timestamp { get; set; }
        public double? out_datetime_timestamp { get; set; }
        public double in_lat { get; set; }
        public double in_lng { get; set; }
        public double? out_lat { get; set; }
        public double? out_lng { get; set; }
        public string latecomerreason { get; set; }
        public string earlyoutouterreason { get; set; }
        public bool islatecomer { get; set; }
        public bool isearlyouter { get; set; }

        public bool isoutside_punchin { get; set; }
        public bool isoutside_punchout { get; set; }
        public long? punchin_locationid { get; set; }
        public long? punchout_locationid { get; set; }

        public List<GetEmployeeTodaysPunchDetail_TaskList> tasks { get; set; }
        public GetEmployeeTodaysPunchDetail()
        {
            tasks = new List<GetEmployeeTodaysPunchDetail_TaskList>();
        }
    }

    public class GetEmployeeTodaysPunchDetail_TaskList
    {
        public long taskid { get; set; }
        public string Task { get; set; }

    }

    public class ManageEmployeePunch_request
    {
        public long punchid { get; set; }
        public int punchtype { get; set; }
        public double in_datetime_timestamp { get; set; }
        public double? out_datetime_timestamp { get; set; }
        public double in_lat { get; set; }
        public double in_lng { get; set; }
        public double? out_lat { get; set; }
        public double? out_lng { get; set; }
        public string latecomerreason { get; set; }
        public string earlyoutouterreason { get; set; }
        public bool islatecomer { get; set; }
        public bool isearlyouter { get; set; }

        public bool isoutside_punchin { get; set; }
        public bool isoutside_punchout { get; set; }
        public long? punchin_locationid { get; set; }
        public long? punchout_locationid { get; set; }
    }
    public class ManageEmployeePunch_response
    {
        public long punchid { get; set; }
        public ResultStatus result { get; set; }

        public ManageEmployeePunch_response()
        {
            result = new ResultStatus();
        }

    }

    public class UpdateUserDeviceToken_request
    {
        public string devicetoken { get; set; }
    }

    public class UpdateUserDeviceToken_response
    {
        public ResultStatus result { get; set; }

        public UpdateUserDeviceToken_response()
        {
            result = new ResultStatus();
        }
    }

    public class UpdateUserDeviceDetail_request
    {
        public string deviceid { get; set; }

        public int devicetype { get; set; }
    }
    public class UpdateUserDeviceDetail_response
    {
        public ResultStatus result { get; set; }

        public UpdateUserDeviceDetail_response()
        {
            result = new ResultStatus();
        }
    }
    public class GetUserProfile_response
    {
        public GetUserProfile_detail record { get; set; }
        public ResultStatus result { get; set; }

        public GetUserProfile_response()
        {
            result = new ResultStatus();
            record = new GetUserProfile_detail();
        }
    }
    public class GetUserProfile_detail
    {
        public long id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string devicetoken { get; set; }
        public bool shouldsendnotification { get; set; }
        public int? devicetype { get; set; }
        public string userphotourl { get; set; }
        public int companyid { get; set; }
        public string companyname { get; set; }
        public double  totalallowedleaves { get; set; }

        public int flexiblebufferminutes { get; set; }
        public int noofweekoffdays { get; set; }
        public int workinghoursinminutes { get; set; }
        public string phonenumber { get; set; }
        public System.TimeSpan officeshifttype { get; set; }
        public string deviceid { get; set; }
        public Nullable<decimal> latitude { get; set; }
        public Nullable<decimal> longitude { get; set; }
        public double punchrangeinmeter { get; set; }
        public double totalappliedleaves { get; set; }
    }
}
