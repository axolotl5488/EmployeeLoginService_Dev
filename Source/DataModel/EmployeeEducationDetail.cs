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
    
    public partial class EmployeeEducationDetail
    {
        public long EduId { get; set; }
        public Nullable<long> UserID { get; set; }
        public string Degree { get; set; }
        public string Institute { get; set; }
        public string University { get; set; }
        public Nullable<int> YearOfPassing { get; set; }
        public Nullable<byte> GradeType { get; set; }
        public string Grade { get; set; }
        public Nullable<byte> CourseType { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> Edit { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string IpAddress { get; set; }
    }
}