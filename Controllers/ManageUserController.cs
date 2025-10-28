using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Manage Accounts")]
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
        public async Task<IActionResult> IsEmailApproved(string username)
        {
            var result = await _manageUserService.IsUserEmailApprovedAsync(username);
            return new JsonResult(result);

        }
                

        [HttpGet("GetUserList")]
        public async Task<IActionResult> GetUserList()
        {
            var res = await _manageUserService.GetAllUserListAsync();
            return new JsonResult(res);

        }

        [HttpPost("ApproveUser")]
        public async Task<IActionResult> ApproveUser([FromBody] Users user)
        {

            await _manageUserService.ApproveSelectedUserAsync(user.Email);
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
        public async Task<IActionResult> LockUser(string email)
        {

            await _manageUserService.LockSelectedUserAsync(email);
            return new JsonResult("Done!");

        }









    }
}
