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
                user.OfficeShiftType = Common.GetShiftTimeByType((int)AppEnum.OfficeShiftEnum.Shift_1);
                user.CompanyID = request.companyid;
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
                    response.result.message = "something went wrong!";
                }
                Common.AddAPIActivityLog("SignUp", DateTime.UtcNow, DateTime.UtcNow,JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response),null, true);
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
        public async Task<HttpResponseMessage> EmployeePunchList()
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.EmployeePunchList());
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ManageCompany([FromBody]ManageCompany_Request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.ManageCompany(model));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCompanyDetail([FromBody]GetCompanyDetail_Request model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebService.GetCompanyDetail(model));
        }
        #endregion 
    }
}
