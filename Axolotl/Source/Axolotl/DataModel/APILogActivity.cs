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
    
    public partial class APILogActivity
    {
        public long ID { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime StarteTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public string API { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
        public bool IsSuccess { get; set; }
    }
}