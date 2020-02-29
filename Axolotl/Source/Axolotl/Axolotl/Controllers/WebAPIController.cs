using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Axolotl.Models;
using BAModel.Common;
using BAModel.Model;
using BAModel.Service;
using DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;

namespace Axolotl.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class WebAPIController : ApiController
    {
        #region Authentication 
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public WebAPIController()
        {
        }

        public WebAPIController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }
        #endregion

        #region Global App
        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyList_drp()
        {
            return Request.CreateResponse(HttpStatusCode.OK, GlobalService.GetCompanyList_drp());
        }
        #endregion 

        #region Sprint #1

        // #1 SignUp
        // Creates New User Account.
        // Parameters : firstName,lastName,email,dateOfBirth,password
        // Zeplin - 2.0 : https://app.zeplin.io/project/5d1d12f850c6a5736f73b866/screen/5d1d134056a5a74b67054b8d
        [AllowAnonymous]
        [HttpPost]
        public async Task<HttpResponseMessage> SignUp([FromBody]SignUp_request request)
        {
            SignUp_response response = new SignUp_response();
            try
            {
                // Create Account
                ApplicationUser user = new ApplicationUser();
                user = new ApplicationUser();

                user.FirstName = request.firstname;
                user.LastName = request.lastname;
                user.Email = request.username;
                user.UserName = request.phonenumber;
                user.PhoneNumber = request.phonenumber;
                user.BirthDate = DateTime.UtcNow;
                user.DateCreated = DateTime.UtcNow;
                user.DateModified = DateTime.UtcNow;
                user.PhoneNumberConfirmed = true;
                user.ShouldSendNotification = true;
                user.OfficeShiftType = Common.GetShiftTimeByType(request.Shifttype);
                user.CompanyID = request.companyid;

                CompanyRole employeeRole = new AxolotlEntities().CompanyRoles.FirstOrDefault(x => x.CompanyID == user.CompanyID
                && x.Name == AppEnum.CompanyDefaultRole.Employee.ToString());
                if(employeeRole != null)
                {
                    user.CompanyRoleID = employeeRole.ID;
                }

                IdentityResult result = await UserManager.CreateAsync(user, request.password);
                if (result.Succeeded)
                {
                    // await this.UserManager.AddToRoleAsync(user.Id, HonestAbeEnum.UserRoleEnum.Technician.ToString());
                    response.result.status = true;
                    response.result.message = "";
                }
                else
                {
                    response.result.status = false;
                    response.result.message = result.Errors.FirstOrDefault();
                }
                Common.AddAPIActivityLog("SignUp", DateTime.UtcNow, DateTime.UtcNow, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), null, true);
            }
            catch (Exception ex)
            {
                response.result.status = false;
                response.result.message = "something went wrong!";

                Common.AddAPIActivityLog("SignUp", DateTime.UtcNow, DateTime.UtcNow, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex, false);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyList()
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyList());
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetUserList()
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetUserList());
        }

        [HttpPost]
        public async Task<HttpResponseMessage> EmployeePunchList(EmployeePunchList_Request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.EmployeePunchList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ManageCompany([FromBody]ManageCompany_Request model)
        {
            ManageCompany_Response response =  WebService.ManageCompany(model);
            if (response.IsCompanyhasAdmin)
            {
                //SignUp_request
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyDetail([FromBody]GetCompanyDetail_Request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyDetail(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetUserDetail([FromBody]GetUserDetail_Request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetUserDetail(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UpdateUserDetail([FromBody]SignUp_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.UpdateUserDetail(model));
        }
        #endregion

        #region Sprint #2
        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyLocaitonList([FromBody]GetCompanyLocaitonList_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyLocaitonList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ManageCompanyLocation([FromBody]ManageCompanyLocation_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageCompanyLocation(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyLocationDetail([FromBody]GetCompanyLocationDetail_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyLocationDetail(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ActiveInActiveCompanyLocation([FromBody]GetCompanyLocationDetail_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ActiveInActiveCompanyLocation(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetEmployeePunchDetail([FromBody]GetEmployeePunchDetailWeb_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetEmployeePunchDetail(model));
        }


        [HttpPost]
        public async Task<HttpResponseMessage> ManageCompanyHolidays([FromBody]MangeCompantHolidays_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageCompanyHolidays(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyHolidayDetail([FromBody]GetCompanyHolidayDetail_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyHolidayDetail(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyHolidayList([FromBody]GetCompanyHolidayList_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyHolidayList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ActiveInActiveCompanyHolidays([FromBody]GetCompanyHolidayDetail_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ActiveInActiveCompanyHolidays(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> EmployeeLeaveList(EmployeeleaveList_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.EmployeeLeaveList(model));
        }
        #endregion

        #region Sprint #3/4
        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyRolesList([FromBody]GetCompanyRolesList_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyRolesList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ManageCompanyRoles([FromBody]ManageCompanyRoles_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageCompanyRoles(model));
        }

        
        [HttpPost]
        public async Task<HttpResponseMessage> RemoveCompanyRoles([FromBody]GetCompanyRoleDetail_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.RemoveCompanyRoles(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyRoleDetail([FromBody]GetCompanyRoleDetail_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyRoleDetail(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyRolesPermissionList([FromBody]GetCompanyRolesPermissionList_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyRolesPermissionList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ManageCompanyRolesPermission([FromBody]ManageCompanyRolesPermission_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageCompanyRolesPermission(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ManageTeamList([FromBody]ManageTeamList_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageTeamList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ManageTeam([FromBody]ManageTeam_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageTeam(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetMyteamEmployeeList([FromBody]ReportingPerson_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetMyteamEmployeeList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetMyteamEmployeePunchList([FromBody]ReportingPerson_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetMyteamEmployeePunchList(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetMyTeamEmployeeLeaveList([FromBody]ReportingPerson_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetMyTeamEmployeeLeaveList(model));
        }
        #endregion

        #region Sprint #5
        [HttpPost]
        public async Task<HttpResponseMessage> ManageEmployeeWeekOffs([FromBody]GetEmployeeWeekOffs_detail model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageEmployeeWeekOffs(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetEmployeeWeekOffs([FromBody]GetEmployeeWeekOffs_request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetEmployeeWeekOffs(model));
        }
        #endregion 
    }
}
