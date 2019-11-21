using DataModel;
using System.Collections.Generic;

namespace EmployeeLoginService.BaseObject
{
    public class UserDetail
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

       
        public int DeviceCount { get; set; }

       
        public string Password { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int TopId { get; set; }

        public string TopName { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Weekoff { get; set; }

        public int WorkHour { get; set; }

        public string WorkHourInString { get; set; }

        public int WorkMinute { get; set; }

        public int GroupID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderTypeId { get; set; }
        public string DateOfBirth { get; set; }
        public string ProfilePic { get; set; }
        public string MaritalStatus { get; set; }
        public string PunchinStatus { get; set; }
        public int EmployeeTypeId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string EmailID { get; set; }
        public string EmailIDPer { get; set; }
        public string MobileNoCmp { get; set; }
        public string PhoneNoPer { get; set; }

        public string GroupName { get; set; }

        public string IsEmployeeAddress { get; set; }

        public string IsEmployeeBankDetails { get; set; }

        public string IsEmployeeEducationDetails { get; set; }

        public string IsEmployeeFamily { get; set; }

        public string IsEmployeePersonalIDs { get; set; }

        public string IsPersonalInfo { get; set; }
        public List<WeeklyTiming> wtimings { get; set; }

        public List<UserWeekOffDay> WOffDay { get; set; }
        public List<UserDevice> UsrDevice { get; set; }

    }

    public class HierarchyUserDetail
    {
        public List<HierarchyUserDetail> Children { get; set; }

        public int CompanyId { get; set; }

        public int? GroupId { get; set; }

        public string CompanyName { get; set; }

        public string DefaultFrom { get; set; }

        public string DefaultTo { get; set; }

        public string ProfilePic { get; set; }

        public int DeviceCount { get; set; }

     
      
        public string LeaveApprovalSecondLevelName { get; set; }

        

        public string LeaveApprovalTypeName { get; set; }

        public string Password { get; set; }

        public string EmailID { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string LatName { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int TopId { get; set; }

        public string TopName { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Weekoff { get; set; }

        public int WorkHour { get; set; }

        public string WorkHourInString { get; set; }

        public int WorkMinute { get; set; }

        public string GroupName { get; set; }

        public string IsEmployeeAddress { get; set; }

        public string IsEmployeeBankDetails { get; set; }

        public string IsEmployeeEducationDetails { get; set; }

        public string IsEmployeeFamily { get; set; }

        public string IsEmployeePersonalIDs { get; set; }

        public string IsPersonalInfo { get; set; }

        public List<WeeklyTiming> wtimings { get; set; }

        public List<UserWeekOffDay> WOffDay { get; set; }

        public List<UserDevice> UsrDevice { get; set; }
    }
}