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


        [HttpGet("GetPISite")]
        [AllowAnonymous]
        public IActionResult GetPISite()
        {
            return new JsonResult(_piSiteService.GetPISiteList());
        }

        [HttpGet("GetPISitebyUserID")]
        public async Task<IActionResult> GetPISitebyUserID()
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userID = user.Id;
            var res = await _piSiteService.GetPISitebyUserIDAsync(userID);
            return new JsonResult(res);
        }


    }
}
