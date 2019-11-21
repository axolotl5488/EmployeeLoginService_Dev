using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class UserEducationInfo
    {

        public long? EduId { get; internal set; }
        public long? UserID { get; internal set; }
        public string Degree { get; internal set; }
        public string Institute { get; internal set; }
        public string University { get; internal set; }
        public int? YearOfPassing { get; internal set; }
        public int? GradeType { get; internal set; }
        public string Grade { get; internal set; }
        public byte? CourseType { get; internal set; }
        public bool? Status { get; internal set; }
        public bool? Edit { get; internal set; }
        public DateTime? CreatedOn { get; internal set; }
        public string CreatedBy { get; internal set; }
        public DateTime? UpdatedOn { get; internal set; }
        public string UpdatedBy { get; internal set; }
        public string IPAddress { get; internal set; }



    }
}