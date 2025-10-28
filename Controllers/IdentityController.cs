using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.AccountViewModels;
using AngularSPAWebAPI.Services;
using CBAS.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace AngularSPAWebAPI.Controllers
{
    /// <summary>
    /// Identity Web API controller.
    /// </summary>
    [Route("api/[controller]")]
    // Authorization policy for this API.
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Manage Accounts")]
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly PISiteService _piSiteService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public IdentityController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<IdentityController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _piSiteService = new PISiteService();
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        /// <summary>
        /// Gets all the users.
        /// </summary>
        /// <returns>Returns all the users</returns>
        // GET api/identity/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var role = await _roleManager.FindByNameAsync("user");
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
           
            return new JsonResult(users);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <returns>IdentityResult</returns>
        // POST: api/identity/Create
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody]CreateViewModel model)
        {
            var user = new ApplicationUser
            {
                GivenName = model.givenName,
                FamilyName = model.familyName,
                AccessFailedCount = 0,
                Email = model.username,
                EmailConfirmed = false,
                LockoutEnabled = true,
                NormalizedEmail = model.username.ToUpper(),
                NormalizedUserName = model.username.ToUpper(),
                TwoFactorEnabled = false,
                UserName = model.username,
                TermsConfirmed = model.termsConfirmed,
            };

            var result = await _userManager.CreateAsync(user, model.password);

            if (result.Succeeded)
            {
                await addToRole(model.username, "user");
                await addClaims(model.username);

                // here should be a good place to send email.
                var strBody = $@"Hi Admin, <br /><br />
                                New user with email address <b>{model.username}</b> has signed up to <b>mousebytes.ca</b> and needs to be approved. <br />
                                Please review the user’s profile for approval. <br /><br />

                                Thanks <br />
                                Mousebytes";
                HelperService.SendEmail("", "", "MouseBytes: New User Registration!", strBody);
            }

            return new JsonResult(result);
        }

        [AllowAnonymous]
        [HttpGet("GeneratePasswordResetToken")]
        public async Task<IActionResult> GeneratePasswordResetToken(string email)
        {
            var appUser = await _userManager.FindByNameAsync(email);
            if (appUser == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return new JsonResult("ret!");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var callbackUrl = Url.Action("reset", "account", new { userId = appUser.Id, code = HttpUtility.UrlEncode(token) }, protocol: HttpContext.Request.Scheme);
            string strBody = $"Hi there, <br/><br/> Please reset your password by clicking here: <a href='{callbackUrl}'>Reset Password!</a> <br/><br/> Thanks <br/> MouseBytes";
            HelperService.SendEmail("", email, "MouseBytes: Reset Password!", strBody);

            return new JsonResult("resetDone!");
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordModel model)
        {
            var appUser = await _userManager.FindByNameAsync(model.email);
            if (appUser == null)
            {
                return new JsonResult("Invalid Email!");
            }
            var result = await _userManager.ResetPasswordAsync(appUser, HttpUtility.UrlDecode(model.token), model.password);

            return new JsonResult(result);
            
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <returns>IdentityResult</returns>
        // POST: api/identity/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody]string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.DeleteAsync(user);

            return new JsonResult(result);
        }

        private async Task addToRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task addClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var claims = new List<Claim> {
                new Claim(type: JwtClaimTypes.GivenName, value: user.GivenName),
                new Claim(type: JwtClaimTypes.FamilyName, value: user.FamilyName),
            };
            await _userManager.AddClaimsAsync(user, claims);
        }


    }
}
