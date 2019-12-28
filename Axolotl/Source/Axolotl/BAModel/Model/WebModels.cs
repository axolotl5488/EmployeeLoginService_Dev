using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAModel.Model;

namespace BAModel.Model
{
    public class GetEmployeePunchDetailWeb_request
    {
        public long punchid { get; set; }
    }

    public class GetEmployeePunchDetailWeb_response
    {
        public ResultStatus result { get; set; }

        public EmployeePunchList_Detail record { get; set; }

        public GetEmployeePunchDetailWeb_response()
        {
            result = new ResultStatus();
            record = new EmployeePunchList_Detail();
        }
    }


    public class GetCompanyLocationDetail_request
    {
        public long locationid { get; set; }
    }

    public class GetCompanyLocationDetail_response
    {
        public ResultStatus result { get; set; }

        public ManageCompanyLocation_request record { get; set; }
        public GetCompanyLocationDetail_response()
        {
            result = new ResultStatus();
            record = new ManageCompanyLocation_request();
        }
    }
    public class ManageCompanyLocation_request
    {
        public int companyid { get; set; }
        public long id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }

    }

    public class ManageCompanyLocation_response
    {
        public ResultStatus result { get; set; }

        public ManageCompanyLocation_response()
        {
            result = new ResultStatus();
        }
    }
    public class GetCompanyLocaitonList_request
    {
        public int companyid { get; set; }
    }

    public class GetCompanyLocaitonList_response
    {
        public ResultStatus result { get; set; }

        public List<GetCompanyLocaitonList_Detail> records { get; set; }
        public GetCompanyLocaitonList_response()
        {
            result = new ResultStatus();
            records = new List<GetCompanyLocaitonList_Detail>();
        }
    }

    public class GetCompanyLocaitonList_Detail
    {
        public long id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public bool isdeleted { get; set; }
    }

    #region Sprint #1
    public class SignUp_request
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string phonenumber { get; set; }
        public string password { get; set; }
        public int companyid { get; set; }
        public int Shifttype { get; set; }
        public long id { get; set; }
    }

    public class SignUp_response
    {

        public ResultStatus result { get; set; }

        public SignUp_response()
        {
            result = new ResultStatus();
        }
    }

    public class GetCompany_Response
    {
        public List<GetCompany_Detail> records { get; set; }
        public ResultStatus result { get; set; }

        public GetCompany_Response()
        {
            result = new ResultStatus();
            records = new List<GetCompany_Detail>();
        }
    }

    public class GetCompany_Detail
    {
        public int id { get; set; }

        public int totalusers { get; set; }
        public string datecreated { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public int flexiblebufferminutes { get; set; }
        public int noofweekoffdays { get; set; }
        public int workinghoursinminutes { get; set; }
        public bool isdelete { get; set; }
    }


    public class GetUserList_Response
    {
        public List<GetUserList_Detail> records { get; set; }
        public ResultStatus result { get; set; }

        public GetUserList_Response()
        {
            result = new ResultStatus();
            records = new List<GetUserList_Detail>();
        }
    }

    public class GetUserList_Detail
    {
        public long id { get; set; }
        public string datecreated { get; set; }
        public string fullname { get; set; }
        public string devicetype { get; set; }
        public string userphotourl { get; set; }
        public int companyid { get; set; }
        public int totallatecommer { get; set; }
        public int totalearlyout { get; set; }
        public string companyname { get; set; }
        public bool isdeleted { get; set; }
        public string phonenumber { get; set; }
        public string officeshifttype { get; set; }
    }

    public class EmployeePunchList_Response
    {
        public List<EmployeePunchList_Detail> records { get; set; }
        public ResultStatus result { get; set; }

        public EmployeePunchList_Response()
        {
            result = new ResultStatus();
            records = new List<EmployeePunchList_Detail>();
        }
    }

    public class EmployeePunchList_Detail
    {
        public long id { get; set; }
        public string date { get; set; }
        public long userid { get; set; }
        public int totaltasks { get; set; }
        public string workinghours { get; set; }
        public string username { get; set; }
        public string clockintime { get; set; }
        public string clockouttime { get; set; }
        public double clockinlatitude { get; set; }
        public double clockinlongitude { get; set; }
        public bool latecomer { get; set; }
        public bool earlyouter { get; set; }
        public string latecomerreason { get; set; }
        public string earlyouterreason { get; set; }
        public bool issystemclockout { get; set; }
        public Nullable<double> clockoutlatitude { get; set; }
        public Nullable<double> clockoutlongitude { get; set; }

        public List<PunchTask_Model> taskrecords { get; set; }

        public EmployeePunchList_Detail()
        {
            taskrecords = new List<PunchTask_Model>();
        }
    }

    public class PunchTask_Model
    {
        public long taskid { get; set; }
        public string Task { get; set; }
    }

    public class ManageCompany_Request
    {
        public int id { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public int flexiblebufferminutes { get; set; }
        public int noofweekOffdays { get; set; }
        public int workinghours { get; set; }
        public double punchrangeinmeter { get; set; }
    }

    public class ManageCompany_Response
    {
        public ResultStatus result { get; set; }

        public ManageCompany_Response()
        {
            result = new ResultStatus();
        }
    }

    public class GetCompanyDetail_Request
    {
        public int id { get; set; }
    }

    public class GetCompanyDetail_Response
    {
        public ManageCompany_Request record { get; set; }

        public ResultStatus result { get; set; }

        public GetCompanyDetail_Response()
        {
            result = new ResultStatus();
            record = new ManageCompany_Request();
        }
    }

    public class GetUserDetail_Request
    {
        public long id { get; set; }
    }

    public class GetUserDetail_Response
    {
        public SignUp_request record { get; set; }
        public ResultStatus result { get; set; }

        public GetUserDetail_Response()
        {
            result = new ResultStatus();
            record = new SignUp_request();
        }
    }
    #endregion 

}
