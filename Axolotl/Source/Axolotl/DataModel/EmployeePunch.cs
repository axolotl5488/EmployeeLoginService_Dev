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
    
    public partial class EmployeePunch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmployeePunch()
        {
            this.EmployeeTasks = new HashSet<EmployeeTask>();
        }
    
        public long ID { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
        public long UserID { get; set; }
        public System.DateTime ClockInTime { get; set; }
        public Nullable<System.DateTime> ClockOutTime { get; set; }
        public decimal ClockInLatitude { get; set; }
        public decimal ClockInLongitude { get; set; }
        public bool LateComer { get; set; }
        public bool EarlyOuter { get; set; }
        public string LateComerReason { get; set; }
        public string EarlyOuterReason { get; set; }
        public bool IsSystemClockOut { get; set; }
        public Nullable<decimal> ClockOutLatitude { get; set; }
        public Nullable<decimal> ClockOutLongitude { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeTask> EmployeeTasks { get; set; }
    }
}