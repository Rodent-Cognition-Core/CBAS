using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.AccountViewModels;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
   // [AllowAnonymous]
    public class DataExtractionController : Controller
    {
        private readonly DataExtractionService _DataExtractionService;

        private readonly UserManager<ApplicationUser> _userManager;
        public DataExtractionController(UserManager<ApplicationUser> userManager)
        {
            _DataExtractionService = new DataExtractionService();
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        // Function Definition to Extract all Task
        [AllowAnonymous]
        [HttpGet("GetAllTask")]
        public IActionResult GetAllGetAllTask()
        {
            return new JsonResult(_DataExtractionService.GetAllTaskAnalysis());
        }
               

        [HttpGet("GetUserGuid")]
        public IActionResult GetUserGuid()
        {
            var user = GetCurrentUser();
            string userID = (user.Result.Id).ToString();

            return new JsonResult(userID);
        }

        // Function Definition to extract ExpList and SubTasklst using TaskID

        [AllowAnonymous]
        [HttpGet("GetExpByTaskID")]
        public IActionResult GetExpByTaskID(int taskId, string userGuid)
        {
            return new JsonResult(_DataExtractionService.GetAllExperimentByTaskId(taskId, userGuid));
        }


        // Function Definition for extracting list of Sub Tasks
        [AllowAnonymous]
        [HttpGet("GetScheduleNameByTaskID")]
        public IActionResult GetScheduleNameByTaskID(int taskID)
        {
            return new JsonResult(_DataExtractionService.GetScheduleListbyTaskID(taskID));
        }


        // Function Definition to extract Market_info features using the ExpID & TaskID & SubTaskID
        [AllowAnonymous]
        [HttpPost("GetMarketInfoBySubTaskIdExpId")]
        public IActionResult GetMarketInfoBySubTaskIdExpId([FromQuery] int subtaskID, [FromBody] List<int> expids)
        {
            return new JsonResult(_DataExtractionService.GetMarkerInfoDatabySubTaskIdExpID(subtaskID, expids));
        }

        // Function Definition to extract animal age using the selected ExpIDs
        [AllowAnonymous]
        [HttpPost("GetAnimalAgeByExpIDs")]
        public IActionResult GetAnimalAgeByExpIDs([FromBody] List<int> expids)
        {
            return new JsonResult(_DataExtractionService.GetAnimalAgeDatabyExpIDs(expids));
        }

        // Function Definition to extract animal sex using the selected ExpIDs
        [AllowAnonymous]
        [HttpPost("GetAnimalSexByExpIDs")]
        public IActionResult GetAnimalSexByExpIDs([FromBody] List<int> expids)
        {
            return new JsonResult(_DataExtractionService.GetAnimalSexDatabyExpIDs(expids));
        }

        // Function Definition to extract animal genotype using the selected ExpIDs
        [AllowAnonymous]
        [HttpPost("GetAnimalGenotypeByExpIDs")]
        public IActionResult GetAnimalGenotypeByExpIDs([FromBody] List<int> genoids)
        {
            return new JsonResult(_DataExtractionService.GetAnimalGenotypeDatabyGenoIDList(genoids));
        }

        // Function Definition to extract animal strain using the selected ExpIDs
        [AllowAnonymous]
        [HttpPost("GetAnimalStrainByExpIDs")]
        public IActionResult GetAnimalStrainByExpIDs([FromBody] List<int> expids)
        {
            return new JsonResult(_DataExtractionService.GetAnimalStrainDatabyExpIDs(expids));
        }

        // Function Definition to extract interventions for the selected experiments
        [AllowAnonymous]
        [HttpPost("GetInterventionByExpIDs")]
        public IActionResult GetInterventionByExpIDs([FromBody] List<int> expids)
        {
            return new JsonResult(_DataExtractionService.GetInterventionbyExpIDs(expids));
        }
        
        // Function Definition to extract data from database
        [AllowAnonymous]
        [HttpPost("GetData")]
        public IActionResult GetData([FromBody] DataExtraction data_extraction)
        {
            // extract data from database to show in the client
            return new JsonResult(_DataExtractionService.GetDataFromDB(data_extraction));

        }

        [AllowAnonymous]
        [HttpGet("GetDataByLinkGuid")]
        public IActionResult GetDataByLinkGuid(Guid linkGuid)
        {
            // extract data from database to show in the client
            return new JsonResult(_DataExtractionService.GetDataFromDbByLinkGuid(linkGuid));

        }

        //function definition when Generate Link button is clicked
        [AllowAnonymous]
        [HttpGet("SaveLink")]
        public IActionResult SaveLink(Guid linkGuid)
        {
            return new JsonResult(_DataExtractionService.MarkLinkAsSaved(linkGuid));
        }

        //[HttpGet("GetAnimalInfoByExpID")]
        //public IActionResult GetAnimalInfoByExpID(int expId)
        //{
        //    var user = GetCurrentUser();
        //    var userID = user.Result.Id;

        //    return new JsonResult(_animalService.GetAnimalByExpID(expId));
        //}








    }
}
