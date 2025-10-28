using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class TaskAnalysisController : Controller
    {
        private readonly TaskAnalysisService _taskAnalysysService1;

        public TaskAnalysisController()
        {
            _taskAnalysysService1 = new TaskAnalysisService();

        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {

            //var qcp = new PostProcessingQcService();
            //var result = qcp.ScheduleCount_5C(16);

            // _taskAnalysysService.InsertTest();
            return new JsonResult(_taskAnalysysService1.GetAllTaskAnalysis());
        }

        [HttpPost("CreateTaskAnalysis")]
        public IActionResult CreateTaskAnalysis([FromBody]TaskAnalysis taskAnalysis)
        {
            return new JsonResult(_taskAnalysysService1.InsertTest(taskAnalysis));
        }

        [HttpPost("GetTaskAnalysisByID")]
        public IActionResult GetTaskAnalysisByID([FromBody]int taskAnalysisID)
        {
            string name = _taskAnalysysService1.GetTaskAnalysisByID(taskAnalysisID);

            return new JsonResult(name);
        }


    }
}
