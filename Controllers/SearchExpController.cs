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
    public class SearchExpController : Controller
    {
        private readonly SearchExpService _searchExperiment;

        private readonly UserManager<ApplicationUser> _manager;

        public SearchExpController(UserManager<ApplicationUser> manager)
        {
            _searchExperiment = new SearchExpService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        [AllowAnonymous]
        [HttpGet("GetSearchList")]
        public IActionResult GetSearchList()
        {

            return new JsonResult(_searchExperiment.GetAllExpList());

        }










    }
}
