using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAModel.Model;

namespace BAModel.Model
{
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
        public decimal in_lat { get; set; }
        public decimal in_lng { get; set; }
        public decimal? out_lat { get; set; }
        public decimal? out_lng { get; set; }
        public string latecomerreason { get; set; }
        public string earlyoutouterreason { get; set; }
        public bool islatecomer { get; set; }
        public bool isearlyouter { get; set; }

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
        public decimal in_lat { get; set; }
        public decimal in_lng { get; set; }
        public decimal? out_lat { get; set; }
        public decimal? out_lng { get; set; }
        public string latecomerreason { get; set; }
        public string earlyoutouterreason { get; set; }
        public bool islatecomer { get; set; }
        public bool isearlyouter { get; set; }
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
        public int flexiblebufferminutes { get; set; }
        public int noofweekoffdays { get; set; }
        public int workinghoursinminutes { get; set; }
        public string phonenumber { get; set; }
        public System.TimeSpan officeshifttype { get; set; }
        public string deviceid { get; set; }
        public Nullable<decimal> latitude { get; set; }
        public Nullable<decimal> longitude { get; set; }
    }
}
