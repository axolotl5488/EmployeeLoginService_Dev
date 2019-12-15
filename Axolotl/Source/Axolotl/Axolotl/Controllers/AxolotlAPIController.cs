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
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UpdateUserDeviceDetail(request,UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserDeviceToken(UpdateUserDeviceToken_request request)
        {
            long UserID = Convert.ToInt64(System.Security.Claims.ClaimsPrincipal.Current.Identity.GetUserId());
            return Request.CreateResponse(HttpStatusCode.OK, AppService.UpdateUserDeviceToken(request, UserID, Request.RequestUri.AbsoluteUri));
        }

        [HttpPost]
        public HttpResponseMessage ManageEmployeePunch(ManageEmployeePunch_request  request)
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
    }
}
