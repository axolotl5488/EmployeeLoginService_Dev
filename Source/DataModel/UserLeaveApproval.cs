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
    
    public partial class UserLeaveApproval
    {
        public int ApprovalId { get; set; }
        public int LeaveId { get; set; }
        public bool IsApprovedFirst { get; set; }
        public Nullable<bool> IsApprovedSecond { get; set; }
        public int FirstApprovedBy { get; set; }
        public Nullable<int> SecondApprovedBy { get; set; }
        public string FirstDescription { get; set; }
        public string SecondDescription { get; set; }
        public System.DateTime FirstCreatedDate { get; set; }
        public System.DateTime FirstUpdatedDate { get; set; }
        public Nullable<System.DateTime> SecondCreatedDate { get; set; }
        public Nullable<System.DateTime> SecondUpdatedDate { get; set; }
    }
}
