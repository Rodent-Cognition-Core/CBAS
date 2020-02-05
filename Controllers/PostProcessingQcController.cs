using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
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
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class PostProcessingQcController : Controller
    {
        private readonly PostProcessingQcService _postProcessingQcService;

        public PostProcessingQcController()
        {
            _postProcessingQcService = new PostProcessingQcService();

        }

        [HttpPost("PostProcessSubExperiment")]
        public IActionResult PostProcessSubExperiment([FromBody]SubExperiment subExp)
        {
            var result = _postProcessingQcService.CheckPostProcessingQC(subExp);

            return new JsonResult(result);
        }

        [HttpGet("GetPostProcessingResult")]
        public IActionResult GetPostProcessingResult(int SubExpId)
        {
            var result = _postProcessingQcService.GetPostProcessingResult(SubExpId);

            return new JsonResult(result);
        }

    }
}
