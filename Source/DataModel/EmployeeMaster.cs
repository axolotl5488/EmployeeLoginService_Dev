//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeMaster
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOBFather { get; set; }
        public Nullable<System.DateTime> DOBMother { get; set; }
        public Nullable<int> MaritalStatus { get; set; }
        public Nullable<System.DateTime> AnniversaryDate { get; set; }
        public string SpouseName { get; set; }
        public Nullable<System.DateTime> DOBSpouse { get; set; }
        public string DegreeName { get; set; }
        public string Grade { get; set; }
        public Nullable<int> YearOfPassing { get; set; }
        public string DegreeDesc { get; set; }
        public string ProfileImg { get; set; }
        public Nullable<int> EmpType { get; set; }
        public Nullable<int> ReportingPerson { get; set; }
        public string CompanyEmail { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<System.DateTime> CompanyJoinDate { get; set; }
        public Nullable<System.DateTime> CompanyLeaveDate { get; set; }
        public string SalaryACNumber { get; set; }
        public string SalaryBank { get; set; }
        public string SalaryBankBranch { get; set; }
        public string SalaryBankIFSC { get; set; }
        public string PANNo { get; set; }
        public string AdharNo { get; set; }
        public string DLNo { get; set; }
        public string PassportNo { get; set; }
        public string VoterID { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
