using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using CBAS.Helpers;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class RequestController : Controller
    {
        private readonly RequestService _requestService;

        private readonly UserManager<ApplicationUser> _manager;

        public RequestController(UserManager<ApplicationUser> manager)
        {
            _requestService = new RequestService();
            
        }

        [AllowAnonymous]
        [HttpPost("AddTask")]

        public IActionResult AddTask([FromBody]Request request)
        {

            _requestService.AddNewTask(request);

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> added new cognitive task to the system! <br /><br /> Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Cognitive Task Added to MouseBytes!", strBody);

            return new JsonResult("Done!");

        }

        [AllowAnonymous]
        [HttpPost("AddPI")]

        public IActionResult AddPI([FromBody]Request request)
        {

            _requestService.AddNewPI(request);

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> added new PI to the system! <br /><br /> Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New PI Added to MouseBytes!", strBody);

            return new JsonResult("Done!");

        }


        [AllowAnonymous]
        [HttpPost("AddAge")]

        public IActionResult AddAge([FromBody]Request request)
        {

            _requestService.AddNewAge(request);

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> added new Age to the system! <br /><br /> Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Age Added to MouseBytes!", strBody);

            return new JsonResult("Done!");

        }


        [AllowAnonymous]
        [HttpPost("AddMouseLine")]

        public IActionResult AddMouseLine([FromBody]Request request)
        {

            _requestService.AddNewMouseLine(request);

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> added new Mouse Line to the system! <br /><br /> Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Mouse Line Added to MouseBytes!", strBody);

            return new JsonResult("Done!");

        }

        [AllowAnonymous]
        [HttpPost("AddGeneral")]
        public IActionResult AddGeneral([FromBody] Request request)
        {

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> has a general request for MouseBytes: <br /><br /> {request.GeneralRequest.Replace("\n", "<br />")} <br /><br />Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Request for MouseBytes!", strBody);

            return new JsonResult("Done!");

        }

        [AllowAnonymous]
        [HttpPost("AddPubTask")]
        public IActionResult AddPubTask([FromBody] Request request)
        {

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> has a request for a new Pubscreen Task: <br /><br />";
            strBody += $@"<b>Paper in question:</b> http://www.doi.org/{request.DOI.Trim()}<br />";
            strBody += $@"<b>Task Category:</b> {request.TaskCategory}<br />";
            strBody += $@"<b>New Task:</b> {request.taskName} <br /><br />Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Task Request for Pubscreen!", strBody);

            return new JsonResult("Done!");

        }

        [AllowAnonymous]
        [HttpPost("AddPubSubModel")]
        public IActionResult AddPubSubModel([FromBody] Request request)
        {

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> has a request for a new Pubscreen sub-rodent model: <br /><br />";
            strBody += $@"<b>Paper in question:</b> http://www.doi.org/{request.DOI.Trim()}<br />";
            strBody += $@"<b>Rodent Model:</b> {request.Model}<br />";
            strBody += $@"<b>New Sub-Model:</b> {request.SubModel} <br /><br />Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Sub-Model Request for Pubscreen!", strBody);

            return new JsonResult("Done!");

        }
        [AllowAnonymous]
        [HttpPost("AddPubSubMethod")]
        public IActionResult AddPubSubMethod([FromBody] Request request)
        {

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{request.Email}</b> has a request for a new Pubscreen sub-method: <br /><br />";
            strBody += $@"<b>Paper in question:</b> http://www.doi.org/{request.DOI.Trim()}<br />";
            strBody += $@"<b>Method:</b> {request.Method}<br />";
            strBody += $@"<b>New Sub-Method:</b> {request.SubMethod} <br /><br />Thanks, <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Sub-Method Request for Pubscreen!", strBody);

            return new JsonResult("Done!");

        }

    }
}
