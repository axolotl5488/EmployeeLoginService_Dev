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
    
    public partial class Leave
    {
        public int LeaveId { get; set; }
        public int UserId { get; set; }
        public int LeaveTypeId { get; set; }
        public System.DateTime LeaveFromDate { get; set; }
        public System.DateTime LeaveToDate { get; set; }
        public int LeaveDays { get; set; }
        public int WeekoffBetweenLeave { get; set; }
        public string WeekoffId { get; set; }
        public int HolidayBetweenLeave { get; set; }
        public string HolidayId { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}