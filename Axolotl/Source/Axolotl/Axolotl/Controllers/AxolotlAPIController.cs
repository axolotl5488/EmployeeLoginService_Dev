using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BAModel.Service;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using BAModel.Model;
using System.Web.Http.Cors;

namespace Axolotl.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class AxolotlAPIController : ApiController
    {
        #region Authentication 
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AxolotlAPIController()
        {
        }

        public AxolotlAPIController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
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

        #region Sprint #1
        [HttpPost]
        public HttpResponseMessage GetUserProfile()
        {

            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetUserProfile(UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserDeviceDetail(UpdateUserDeviceDetail_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UpdateUserDeviceDetail(request, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserDeviceToken(UpdateUserDeviceToken_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UpdateUserDeviceToken(request, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage ManageEmployeePunch(ManageEmployeePunch_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.ManageEmployeePunch(request, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetEmployeeTodaysPunchDetail(GetEmployeeTodaysPunchDetail_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetEmployeeTodaysPunchDetail(request, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage AddPunchTask(AddPunchTask_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.AddPunchTask(request, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetPunchTasks(GetPunchTasks_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetPunchTasks(request, UserID, Request.RequestUri.AbsoluteUri));
        }


        [HttpPost]
        public HttpResponseMessage GetEmployeePunchDetail(GetEmployeePunchDetail_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetEmployeePunchDetail(request, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetEmployeePunchList()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetEmployeePunchList(UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage Logout()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.Logout(UserID, Request.RequestUri.AbsoluteUri));
        }
        #endregion

        #region Sprint #2

        [HttpPost]
        public async Task<HttpResponseMessage> ChangePassword(ChangePassword_Request model)
        {
            ChangePassword_response response = new ChangePassword_response();
            try
            {
                long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
                var user = await UserManager.FindByIdAsync(UserID);
                if (await UserManager.CheckPasswordAsync(user, model.oldpassword))
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, model.oldpassword, model.newpasswprd);
                    if (result.Succeeded)
                    {
                        response.result.status = true;
                        response.result.message = "";
                    }
                    else
                    {
                        response.result.status = false;
                        response.result.message = result.Errors.FirstOrDefault();
                    }
                }
                else
                {
                    response.result.status = false;
                    response.result.message = "Invalid current password";
                }

            }
            catch (Exception ex)
            {
                response.result.status = false;
                response.result.message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage APIVersionCheck(APIVersionCheck_request request)
        {
            return Request.CreateResponse(HttpStatusCode.OK, AppService.APIVersionCheck(request, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetCompanyLocations()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetCompanyLocations(UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage AddEmployeeLeave(AddEmployeeLeave_request model)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.AddEmployeeLeave(model, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UpdateLeaveStatus(UpdateLeaveStatus_request model)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UpdateLeaveStatus(model, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetLeaveDetail(GetLeaveDetail_request model)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetLeaveDetail(model, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetEmployeeLeaves()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetEmployeeLeaves(UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetMyTeamEmployeeLeaves()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetMyTeamEmployeeLeaves(UserID, Request.RequestUri.AbsoluteUri));
        }
        #endregion

        #region Sprint #3-4
        [HttpPost]
        public HttpResponseMessage GetMyTeam()
        {

            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetMyTeam(UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UpdateEmployeeTaskStatus(UpdateEmployeeTaskStatus_request model)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UpdateEmployeeTaskStatus(model, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage AddUpdateEmployeeCalls(AddUpdateEmployeeCalls_request model)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.AddUpdateEmployeeCalls(model, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage EndEmployeeCalls(EndEmployeeCalls_request model)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.EndEmployeeCalls(model, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage GetEmployeeCallList()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.GetEmployeeCallList(UserID, Request.RequestUri.AbsoluteUri));
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage MarkNotificationAsRead(MarkNotificationAsRead_request model)
        {

            return Request.CreateResponse(HttpStatusCode.OK, AppService.MarkNotificationAsRead(model, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UserDashboardStatics()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UserDashboardStatics(UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UserLeaveStatics()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UserLeaveStatics(UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UserCallStatics()
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UserCallStatics(UserID, Request.RequestUri.AbsoluteUri));
        }
        #endregion
    }
}
