using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularSPAWebAPI.Models.AccountViewModels;

namespace AngularSPAWebAPI.Controllers
{
    /// <summary>
    /// Resources Web API controller.
    /// </summary>
    [Route("api/[controller]")]
    // Authorization policy for this API.
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;
        private readonly UserManager<ApplicationUser> _manager;
        

        public ProfileController(UserManager<ApplicationUser> manager)
        {
            _profileService = new ProfileService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }


        
        [HttpGet("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            var user = GetCurrentUser();
            var userID = user.Result.Id;

            return new JsonResult(_profileService.GetUserInfoByID(userID));
        }

        [HttpPost("UpdateProfile")]
        public IActionResult UpdateProfile ([FromBody]CreateViewModel UserModel)
        {
            var user = GetCurrentUser();
            var userID = user.Result.Id;
            _profileService.UpdateProfile(UserModel, userID);
            return new JsonResult("Done!");
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
        {
            var user = GetCurrentUser();
            var userEmail = user.Result.UserName;
            // var userID = user.Result.Id;

            var appUser = await _manager.FindByNameAsync(userEmail);
            if (appUser == null)
            {
                return new JsonResult("Invalid Email!");
            }
            var result = await _manager.ChangePasswordAsync(appUser, model.currentPassword, model.newPassword);

            return new JsonResult(result);

        }


    }
}
