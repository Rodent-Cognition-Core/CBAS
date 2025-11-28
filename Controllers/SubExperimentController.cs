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
    public class SubExperimentController : Controller
    {
        private readonly SubExperimentService _subexperimentService;
        private readonly UserManager<ApplicationUser> _manager;

        public SubExperimentController(UserManager<ApplicationUser> manager)
        {
            _subexperimentService = new SubExperimentService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        // Function definitin to extract list of all age ranges from table Age in DB
        [HttpGet("GetAllAge")]
        public IActionResult GetAllAge()
        {

            return new JsonResult(_subexperimentService.GetAgeList());
        }
        
        // Function definition to extract list of sub experiment based on the selected Experiment ID
        [HttpGet("GetAllSubExpbyExpID")]
        public IActionResult GetAllSubExpbyExpID(int expID)
        {

            return new JsonResult(_subexperimentService.GetAllSubExperimentsByExpID(expID));
        }

        [HttpGet("GetAllSubExpTimeSeriesbyExpID")]
        public IActionResult GetAllSubExpTimeSeriesbyExpID(int expID)
        {

            return new JsonResult(_subexperimentService.GetAllSubExperimentsTimeSeriesByExpID(expID));
        }

        [HttpPost("CreateSubExperiment")]
        public IActionResult CreateSubExperiment([FromBody]SubExperiment subexperiment)
        {


            // throw new Exception("This Experiment Name was already taken!");
            (bool flagSubExpName, bool flagSubexp) info = _subexperimentService.DoesSubExperimentNameExist(subexperiment, 0);


            if (info.flagSubExpName)
            {
                return new JsonResult("TakenSubExpName");
            }
            if (info.flagSubexp)
            {
                return new JsonResult("takenSubExp");
            }
            else
            {
                return new JsonResult(_subexperimentService.InsertSubExperiment(subexperiment));
            }
        }

        [HttpPost("UpdateSubExperiment")]
        public IActionResult UpdateSubExperiment([FromBody]SubExperiment subexperiment)
        {
            (bool flagSubExpName, bool flagSubexp) info = _subexperimentService.DoesSubExperimentNameExist(subexperiment, subexperiment.SubExpID);

            if (info.flagSubExpName)
            {
                return new JsonResult("TakenSubExpName");
            }
            if (info.flagSubexp)
            {
                return new JsonResult("takenSubExp");
            }
            else
            {
                _subexperimentService.UpdateSubExperiment(subexperiment);
                return new JsonResult("Done");
            }
            
        }

        [HttpDelete("DeleteSubExpById")]
        public IActionResult DeleteSubExpById(int subExpID)
        {
            _subexperimentService.DeleteSubExpBySubExpID(subExpID);
            return new JsonResult("Done!");

        }

        // Function to get all images from DB for PAL and PD
        [HttpGet("GetAllImages")]
        public IActionResult GetAllImages()
        {

            return new JsonResult(_subexperimentService.GetAllImagesList());
        }

        //[HttpDelete("DeleteFileById")]
        //public IActionResult DeleteFileById(int uploadID)
        //{
        //    _experimentService.DeleteExpByUploadID(uploadID);
        //    return new JsonResult("Done!");

        //}


    }
}
