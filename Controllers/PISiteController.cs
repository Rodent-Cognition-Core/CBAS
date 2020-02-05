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
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class PISiteController : Controller
    {
        private readonly PISiteService _piSiteService;
        private readonly UserManager<ApplicationUser> _manager;

        public PISiteController(UserManager<ApplicationUser> manager)
        {
            _piSiteService = new PISiteService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }


        [HttpGet("GetPISite")]
        [AllowAnonymous]
        public IActionResult GetPISite()
        {
            return new JsonResult(_piSiteService.GetPISiteList());
        }

        [HttpGet("GetPISitebyUserID")]
        public IActionResult GetPISitebyUserID()
        {
            var user = GetCurrentUser();
            var userID = user.Result.Id;

            return new JsonResult(_piSiteService.GetPISitebyUserID(userID));
        }


    }
}
