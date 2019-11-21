﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EmployeeLoginEntities : DbContext
    {
        public EmployeeLoginEntities()
            : base("name=EmployeeLoginEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AppCompanyLeaveManagement> AppCompanyLeaveManagements { get; set; }
        public virtual DbSet<AppLeaveApprovedStatu> AppLeaveApprovedStatus { get; set; }
        public virtual DbSet<AppLeaveType> AppLeaveTypes { get; set; }
        public virtual DbSet<AppMasterLeaveType> AppMasterLeaveTypes { get; set; }
        public virtual DbSet<AppUserLeave> AppUserLeaves { get; set; }
        public virtual DbSet<BranchType> BranchTypes { get; set; }
        public virtual DbSet<Break> Breaks { get; set; }
        public virtual DbSet<BreakCategory> BreakCategories { get; set; }
        public virtual DbSet<BreakType> BreakTypes { get; set; }
        public virtual DbSet<CallIn> CallIns { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyBranch> CompanyBranches { get; set; }
        public virtual DbSet<CompanyLeaveTypeCount> CompanyLeaveTypeCounts { get; set; }
        public virtual DbSet<CompanyMaster> CompanyMasters { get; set; }
        public virtual DbSet<CompanyWeekOffDay> CompanyWeekOffDays { get; set; }
        public virtual DbSet<Component> Components { get; set; }
        public virtual DbSet<EmployeeAddress> EmployeeAddresses { get; set; }
        public virtual DbSet<EmployeeBankDetail> EmployeeBankDetails { get; set; }
        public virtual DbSet<EmployeeEducationDetail> EmployeeEducationDetails { get; set; }
        public virtual DbSet<EmployeeFamily> EmployeeFamilies { get; set; }
        public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }
        public virtual DbSet<EmployeePersonalID> EmployeePersonalIDs { get; set; }
        public virtual DbSet<GroupAccess> GroupAccesses { get; set; }
        public virtual DbSet<GroupMaster> GroupMasters { get; set; }
        public virtual DbSet<HalfDay> HalfDays { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<LeaveApprovalType> LeaveApprovalTypes { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
        public virtual DbSet<LocationPin> LocationPins { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<OfficialLeaf> OfficialLeaves { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PunchIn> PunchIns { get; set; }
        public virtual DbSet<RoleMaster> RoleMasters { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
        public virtual DbSet<UserAccess> UserAccesses { get; set; }
        public virtual DbSet<UserComponentRight> UserComponentRights { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<UserLeaveApproval> UserLeaveApprovals { get; set; }
        public virtual DbSet<UserLeaveTypeCount> UserLeaveTypeCounts { get; set; }
        public virtual DbSet<UserLocation> UserLocations { get; set; }
        public virtual DbSet<UserRight> UserRights { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersOTP> UsersOTPs { get; set; }
        public virtual DbSet<UserWeekOffDay> UserWeekOffDays { get; set; }
        public virtual DbSet<VersionMaster> VersionMasters { get; set; }
        public virtual DbSet<WeeklyTiming> WeeklyTimings { get; set; }
    
        public virtual ObjectResult<GetEmployeeHeirarchy_Result> GetEmployeeHeirarchy(string p_Command, Nullable<int> p_UserID, Nullable<int> p_CompanyID)
        {
            var p_CommandParameter = p_Command != null ?
                new ObjectParameter("p_Command", p_Command) :
                new ObjectParameter("p_Command", typeof(string));
    
            var p_UserIDParameter = p_UserID.HasValue ?
                new ObjectParameter("p_UserID", p_UserID) :
                new ObjectParameter("p_UserID", typeof(int));
    
            var p_CompanyIDParameter = p_CompanyID.HasValue ?
                new ObjectParameter("p_CompanyID", p_CompanyID) :
                new ObjectParameter("p_CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetEmployeeHeirarchy_Result>("GetEmployeeHeirarchy", p_CommandParameter, p_UserIDParameter, p_CompanyIDParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> GetEmployeeHeirarchyBOTTOMTOTOP(string p_Command, Nullable<int> p_UserID, Nullable<int> p_CompanyID)
        {
            var p_CommandParameter = p_Command != null ?
                new ObjectParameter("p_Command", p_Command) :
                new ObjectParameter("p_Command", typeof(string));
    
            var p_UserIDParameter = p_UserID.HasValue ?
                new ObjectParameter("p_UserID", p_UserID) :
                new ObjectParameter("p_UserID", typeof(int));
    
            var p_CompanyIDParameter = p_CompanyID.HasValue ?
                new ObjectParameter("p_CompanyID", p_CompanyID) :
                new ObjectParameter("p_CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetEmployeeHeirarchyBOTTOMTOTOP", p_CommandParameter, p_UserIDParameter, p_CompanyIDParameter);
        }
    
        public virtual ObjectResult<GetUserHierarchyWise_Result> GetUserHierarchyWise(Nullable<int> p_UserId)
        {
            var p_UserIdParameter = p_UserId.HasValue ?
                new ObjectParameter("p_UserId", p_UserId) :
                new ObjectParameter("p_UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetUserHierarchyWise_Result>("GetUserHierarchyWise", p_UserIdParameter);
        }
    }
}
