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
    
    public partial class EmployeeLeaf
    {
        public long ID { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public long UserID { get; set; }
        public int CompanyID { get; set; }
        public int LeaveTypeID { get; set; }
        public int DayTypeID { get; set; }
        public int LeaveStatus { get; set; }
        public bool IsPaidLeave { get; set; }
        public bool IsActive { get; set; }
        public string ApplyRemarks { get; set; }
        public string ApprovaRemarks { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Company Company { get; set; }
    }
}
