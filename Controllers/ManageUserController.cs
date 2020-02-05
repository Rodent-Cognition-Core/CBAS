using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Controllers
{
    /// <summary>
    /// Resources Web API controller.
    /// </summary>
    [Route("api/[controller]")]
    // Authorization policy for this API.
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Manage Accounts")]
    public class ManageUserController : Controller
    {
        private readonly ManageUserService _manageUserService;
        
        //private readonly UserManager<ApplicationUser> _manager;

        public ManageUserController(UserManager<ApplicationUser> manager)
        {
            _manageUserService = new ManageUserService();

        }

        // Function definition to check if the Email of USer approved by Admin
        [AllowAnonymous]
        [HttpGet("IsEmailApproved")]
        public IActionResult IsEmailApproved(string username)
        {
            
            return new JsonResult(_manageUserService.IsUserEmailApproved(username));

        }
                

        [HttpGet("GetUserList")]
        public IActionResult GetUserList()
        {

            return new JsonResult(_manageUserService.GetAllUserList());

        }

        [HttpPost("ApproveUser")]
        public IActionResult ApproveUser([FromBody] Users user)
        {

            _manageUserService.ApproveSelectedUser(user.Email);
            // here should be a good place to send email.
            var strBody = $@"Hi {user.GivenName.ToUpper()} {user.FamilyName.ToUpper()}, <br /><br />
                                Thanks for registering with MouseBytes! <br /><br />
                                Your profile has been reviewed and approved. Please click on the following link and sign into MouseBytes.<br />
                                <a href=""http://mousebytes.ca/account/signin"">http://mousebytes.ca/account/signin</a>
                                <br /><br />

                                Kind regards,  <br />
                                Mousebytes";

            CBAS.Helpers.HelperService.SendEmail("", user.Email, "MouseBytes: Your Account Approved!", strBody);
            return new JsonResult("Done!");

        }

        [HttpGet("LockUser")]
        public IActionResult LockUser(string email)
        {

            _manageUserService.LockSelectedUser(email);
            return new JsonResult("Done!");

        }









    }
}
