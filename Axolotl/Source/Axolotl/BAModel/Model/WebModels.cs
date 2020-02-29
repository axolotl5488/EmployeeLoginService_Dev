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

        public string imageurl { get; set; }
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
        public int companyroleid { get; set; }
        public bool haveteam { get; set; }
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
        public string rolename { get; set; }
        public string reportingperson { get; set; }
        public int totalmember { get; set; }
    }

    public class EmployeePunchList_Request
    {
        public int companyid { get; set; }

        public int userid { get; set; }
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
        public List<GetEmployeeCallList_detail_Webportal> callrecords { get; set; }
        public EmployeePunchList_Detail()
        {
            taskrecords = new List<PunchTask_Model>();
            callrecords = new List<GetEmployeeCallList_detail_Webportal>();
        }
    }

    public class GetEmployeeCallList_detail_Webportal
    {
        public long id { get; set; }
        public int companyid { get; set; }
        public long punchid { get; set; }
        public double start_lat { get; set; }
        public double start_lng { get; set; }
        public string title { get; set; }
        public string callfor { get; set; }
        public string remarks { get; set; }

        public string start_datetime_timestamp { get; set; }

        public double? _end_lat { get; set; }
        public double? _end_lng { get; set; }
        public string _end_datetime_timestamp { get; set; }
    }

    public class PunchTask_Model
    {
        public long taskid { get; set; }
        public string Task { get; set; }
        public string TaskStatus { get; set; }
        public int TaskStatusID { get; set; }
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
        public bool IsCompanyhasAdmin { get; set; }
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

    public class MangeCompantHolidays_request
    {
        public long id { get; set; }

        public int companyid { get; set; }

        public string date { get; set; }

        public string Name { get; set; }

        public string description { get; set; }

    }

    public class GetCompanyHolidayDetail_request
    {
        public long id { get; set; }
    }


    public class GetCompanyHolidayDetail_response
    {
        public MangeCompantHolidays_request record { get; set; }

        public ResultStatus result { get; set; }

        public GetCompanyHolidayDetail_response()
        {
            result = new ResultStatus();
            record = new MangeCompantHolidays_request();
        }
    }

    public class GetCompanyHolidayList_request
    {
        public int companyid { get; set; }
    }

    public class GetCompanyHolidayList_response
    {
        public ResultStatus result { get; set; }

        public List<GetCompanyHolidayList_yeardetail> records { get; set; }

        public GetCompanyHolidayList_response()
        {
            result = new ResultStatus();
            records = new List<GetCompanyHolidayList_yeardetail>();
        }
    }
    public class GetCompanyHolidayList_yeardetail
    {
        public string year { get; set; }

        public List<GetCompanyHolidayList_detail> records { get; set; }

        public GetCompanyHolidayList_yeardetail()
        {
            records = new List<GetCompanyHolidayList_detail>();
        }
    }

    public class GetCompanyHolidayList_detail
    {
        public long id { get; set; }

        public int companyid { get; set; }

        public string companyname { get; set; }

        public string date { get; set; }


        public string year { get; set; }
        public string Name { get; set; }

        public string description { get; set; }

        public bool isactive { get; set; }
    }

    public class EmployeeleaveList_request
    {
        public int companyid { get; set; }

        public long userid { get; set; }
    }

    public class EmployeeleaveList_Response
    {
        public List<EmployeeleaveList_Detail> records { get; set; }
        public ResultStatus result { get; set; }

        public EmployeeleaveList_Response()
        {
            result = new ResultStatus();
            records = new List<EmployeeleaveList_Detail>();
        }
    }

    public class EmployeeleaveList_Detail
    {
        public long id { get; set; }

        public string companyname { get; set; }
        public int companyid { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public long userid { get; set; }
        public string username { get; set; }
        public int leavetypeid { get; set; }
        public string leavetype { get; set; }
        public int daytypeid { get; set; }
        public string daytype { get; set; }
        public int leavestatusid { get; set; }
        public string leavestatus { get; set; }
        public bool ispaidleave { get; set; }
        public string userremarkd { get; set; }
        public string approvalremarks { get; set; }
        public double totaldays { get; set; }

    }

    public class GetCompanyRolesList_request
    {
        public int companyid { get; set; }
    }

    public class GetCompanyRolesList_response
    {
        public List<GetCompanyRolesList_detail> records { get; set; }
        public ResultStatus result { get; set; }

        public GetCompanyRolesList_response()
        {
            result = new ResultStatus();
            records = new List<GetCompanyRolesList_detail>();
        }
    }

    public class GetCompanyRolesList_detail
    {
        public long id { get; set; }
        public string lastmodified { get; set; }
        public int companyid { get; set; }
        public int totalmembers { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool isactive { get; set; }
    }

    public class ManageCompanyRoles_request
    {
        public long id { get; set; }
        public int companyid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class GetCompanyRoleDetail_request
    {
        public long id { get; set; }
    }

    public class GetCompanyRoleDetail_response
    {
        public ManageCompanyRoles_request record { get; set; }

        public ResultStatus result { get; set; }

        public GetCompanyRoleDetail_response()
        {
            result = new ResultStatus();
            record = new ManageCompanyRoles_request();
        }
    }

    public class GetCompanyRolesPermissionList_request
    {
        public int companyid { get; set; }
    }

    public class GetCompanyRolesPermissionList_response
    {
        public List<GetCompanyRolesPermissionList_Role_detail> records { get; set; }

        public ResultStatus result { get; set; }

        public GetCompanyRolesPermissionList_response()
        {
            result = new ResultStatus();
            records = new List<GetCompanyRolesPermissionList_Role_detail>();
        }
    }

    public class GetCompanyRolesPermissionList_Role_detail
    {
        public long id { get; set; }
        public int companyid { get; set; }
        public string name { get; set; }

        public List<GetCompanyRolesPermissionList_RolePermission_detail> items { get; set; }

        public GetCompanyRolesPermissionList_Role_detail()
        {
            items = new List<GetCompanyRolesPermissionList_RolePermission_detail>();
        }
    }

    public class GetCompanyRolesPermissionList_RolePermission_detail
    {
        public long id { get; set; }
        public string lastmodified { get; set; }
        public string screenname { get; set; }
        public int screenid { get; set; }
        public int companyid { get; set; }
        public long companyroleid { get; set; }

        public bool isactive { get; set; }
    }
    public class ManageCompanyRolesPermission_request
    {
        public int companyid { get; set; }
        public long companyroleid { get; set; }
        public long companyrolepermissionid { get; set; }
        public int screenid { get; set; }
    }

    public class ManageTeamList_request
    {
        public int companyid { get; set; }
    }

    public class ManageTeamList_response
    {
        public List<ManageTeamList_detail> records { get; set; }
        public List<ManageTeamList_reporting_person_detail> reportingusers { get; set; }
        public List<ManageTeamList_reporting_role_detail> roles { get; set; }
        public ResultStatus result { get; set; }

        public ManageTeamList_response()
        {
            result = new ResultStatus();
            records = new List<ManageTeamList_detail>();
            reportingusers = new List<ManageTeamList_reporting_person_detail>();
            roles = new List<ManageTeamList_reporting_role_detail>();
        }
    }

    public class ManageTeamList_detail
    {
        public long userid { get; set; }
        public string name { get; set; }
        public long roleid { get; set; }
        public string rolename { get; set; }
        public int totalmember { get; set; }
        public long? reportingperson { get; set; }
        public string reportingpersonname { get; set; }

    }

    public class ManageTeamList_reporting_person_detail
    {
        public long userid { get; set; }
        public string name { get; set; }
    }

    public class ManageTeamList_reporting_role_detail
    {
        public long roleid { get; set; }
        public string name { get; set; }
    }

    public class ManageTeam_request
    {
        public long userid { get; set; }
        public long roleid { get; set; }
        public long? reportingpersonid { get; set; }
    }

    public class ReportingPerson_request
    {
        public long userid { get; set; }
        public int companyid { get; set; }
    }


    public class GetEmployeeWeekOffs_request
    {
        public long employeeid { get; set; }
    }

    public class GetEmployeeWeekOffs_response
    {
        public ResultStatus result { get; set; }
        public List<GetEmployeeWeekOffs_weekdetail> records { get; set; }
        public GetEmployeeWeekOffs_response()
        {
            result = new ResultStatus();
            records = new List<GetEmployeeWeekOffs_weekdetail>();
        }
    }
    public class GetEmployeeWeekOffs_weekdetail
    {
        public string week { get; set; }
        public List<GetEmployeeWeekOffs_detail> records { get; set; }

        public GetEmployeeWeekOffs_weekdetail()
        {
            records = new List<GetEmployeeWeekOffs_detail>();
        }
    }
        public class GetEmployeeWeekOffs_detail
    {
        public int weekno { get; set; }
        public string week { get; set; }
        public string day { get; set; }
        public long id { get; set; }
        public bool isadd { get; set; }
        public long employeeid { get; set; }
        public int companyid { get; set; }
        public int companyweekoffdays { get; set; }
    }

    public class UploadImage_Response
    {
        public string filename { get; set; }
        public ResultStatus response { get; set; }
        public UploadImage_Response()
        {
            response = new ResultStatus();
        }
    }
}
