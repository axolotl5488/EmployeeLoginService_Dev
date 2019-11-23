using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAModel.Model
{
    public class ResultStatusModel
    {
        public bool status { get; set; }

        public string message { get; set; }
    }

    public class ListUsersByGroupId_Result_Model
    {
        public ResultStatusModel response { get; set; }

        public List<ListUsersByGroupId_Model> records { get; set; }

        public ListUsersByGroupId_Result_Model()
        {
            response = new ResultStatusModel();
            records = new List<ListUsersByGroupId_Model>();
        }
    }

    public class ListUsersByGroupId_Model
    {
        public int UserID { get; set; }

        public string UserName { get; set; }
    }

        public class CheckAppVersion_Detail_Model
    {
        public bool MustUpdate { get; set; }
        public bool ShouldUpdate { get; set; }
        public string message { get; set; }

    }

    public class CheckAppVersion_Model
    {
        public CheckAppVersion_Detail_Model record { get; set; }

        public ResultStatusModel response{ get;set; }

        public CheckAppVersion_Model()
        {
            response = new ResultStatusModel();
            record = new CheckAppVersion_Detail_Model();
        }
    }

    public class GetLeavesListByUser_Model
    {
        public List<GetLeaveList_detail_Model> records { get; set; }

        public LeaveStatatics_Model report { get; set; }

        public ResultStatusModel response { get; set; }
        public GetLeavesListByUser_Model()
        {
            records = new List<GetLeaveList_detail_Model>();
            response = new ResultStatusModel();
            report = new LeaveStatatics_Model();
        }
    }

    public class LeaveStatatics_Model
    {
        public decimal TotalPending { get; set; }
        public decimal TotalSanctioned { get; set; }
        public decimal TotalSanctionedhalfLeaves { get; set; }
        public decimal TotalCanceled { get; set; }
        public decimal TotalReverted { get; set; }
        public decimal TotalRejected { get; set; }
    }

    public class GetLeavesListByCompany_Model
    {
        public List<GetLeaveList_detail_Model> records { get; set; }

        public LeaveStatatics_Model report { get; set; }
        public ResultStatusModel response { get; set; }
        public GetLeavesListByCompany_Model()
        {
            records = new List<GetLeaveList_detail_Model>();
            response = new ResultStatusModel();
            report = new LeaveStatatics_Model();
        }
    }

    public class GetLeaveList_detail_Model
    {
        public long leaveid { get; set; }
        public int userid { get; set; }

        public string username { get; set; }

        public int comapnyid { get; set; }

        public string description { get; set; }

        public int leavetypeid { get; set; }

        public string leavetypename { get; set; }

        public int masterleavetypeid { get; set; }

        public string masterleavetypename { get; set; }

        public int leaveapprovedstatusid { get; set; }



        public string leaveapprovedstatusname { get; set; }

        public long startdate { get; set; }

        public long enddate { get; set; }

        public decimal toaldays { get; set; }

        public int? approvedbyuserid { get; set; }

        public string approvedbyusername { get; set; }

        public long createddate { get; set; }

        public long modifieddate { get; set; }
    }


    


    public class GetLeaveDetail_Model
    {
        public GetLeaveList_detail_Model record { get; set; }

        public ResultStatusModel response { get; set; }
        public GetLeaveDetail_Model()
        {
            record = new GetLeaveList_detail_Model();
            response = new ResultStatusModel();
        }
    }

    public class GetLeavesReportByUser_Model
    {
      
        public GetLeavesReportByUserDetail_Model record { get; set; }
        public ResultStatusModel response { get; set; }

        public GetLeavesReportByUser_Model()
        {
           
            response = new ResultStatusModel();
            record = new GetLeavesReportByUserDetail_Model();
        }

    }

    public class GetLeavesReportByUserDetail_Model
    {
        public decimal TotalRemainingFullLeaves { get; set; }
        public decimal TotalRemainingHalfLeaves { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalSanctioned { get; set; }
        public decimal TotalSanctionedhalfLeaves { get; set; }
        public decimal TotalCanceled { get; set; }
        public decimal TotalReverted { get; set; }
        public decimal TotalRejected { get; set; }
    }


}
