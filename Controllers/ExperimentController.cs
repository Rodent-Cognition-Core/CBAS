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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class ExperimentController : Controller
    {
        private readonly ExperimentService _experimentService;
        private readonly UserManager<ApplicationUser> _manager;

        public ExperimentController(UserManager<ApplicationUser> manager)
        {
            _experimentService = new ExperimentService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        [HttpGet("GetAllExp")]
        public IActionResult GetAllExp()
        {
            var user = GetCurrentUser();
            var userID = user.Result.Id;

            return new JsonResult(_experimentService.GetAllExperiments(userID));
        }

        [HttpPost("CreateExperiment")]
        public IActionResult CreateExperiment([FromBody]Experiment experiment)
        {
            var user = GetCurrentUser();
            var userEmail = user.Result.UserName;
            var userID = user.Result.Id;

            // throw new Exception("This Experiment Name was already taken!");
            if (_experimentService.DoesExperimentNameExist(experiment.ExpName))
            {
                return new JsonResult("Taken");
            }
            else
            {

                return new JsonResult(_experimentService.InsertExperiment(experiment, userID, userEmail));
            }
        }

        [HttpPost("UpdateExperiment")]
        public IActionResult UpdateExperiment([FromBody]Experiment experiment)
        {
            var user = GetCurrentUser();
            var userEmail = user.Result.UserName;
            var userID = user.Result.Id;

            // throw new Exception("This Experiment Name was already taken!");
            if (_experimentService.DoesExperimentNameExistEdit(experiment.ExpName, experiment.ExpID))
            {
                return new JsonResult("Taken");
            }
            else
            {
                _experimentService.UpdateExp(experiment, userID, userEmail);
                return new JsonResult("Done!");
            }
        }

        [HttpDelete("DeleteExpById")]
        public IActionResult DeleteExpById(int expID)
        {
            _experimentService.DeleteExpByExpID(expID);
            return new JsonResult("Done!");

        }

        [HttpDelete("DeleteFileById")]
        public IActionResult DeleteFileById(int uploadID)
        {
            _experimentService.DeleteExpByUploadID(uploadID);
            return new JsonResult("Done!");

        }

        // Function to get all images from DB for PAL and PD
        [HttpGet("GetAllImages")]
        public IActionResult GetAllImages()
        {

            return new JsonResult(_experimentService.GetAllImagesList());
        }

        //Function to get all images from DB for PAL and PD
        [AllowAnonymous]
        [HttpGet("GetAllSpecies")]
        public IActionResult GetAllSpecies()
        {

            return new JsonResult(_experimentService.GetAllSpeciesList());
        }

    }
}
