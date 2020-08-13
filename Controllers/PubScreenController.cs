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
    public class PubScreenController : Controller
    {
        private readonly PubScreenService _pubScreenService;

        private readonly UserManager<ApplicationUser> _manager;

        public PubScreenController(UserManager<ApplicationUser> manager)
        {
            _pubScreenService = new PubScreenService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        // Extracting paper type list
        [HttpGet("GetPaperType")]
        [AllowAnonymous]
        public IActionResult GetPaperType()
        {
            return new JsonResult(_pubScreenService.GetPaperTypes());
        }

        // Extracting cognitive task list
        [HttpGet("GetTask")]
        [AllowAnonymous]
        public IActionResult GetTask()
        {
            return new JsonResult(_pubScreenService.GetTasks());
        }

        // Extracting species list
        [HttpGet("GetSpecie")]
        [AllowAnonymous]
        public IActionResult GetSpecie()
        {
            return new JsonResult(_pubScreenService.GetSpecies());
        }

        // Extracting sex list
        [HttpGet("GetSex")]
        [AllowAnonymous]
        public IActionResult GetSex()
        {
            return new JsonResult(_pubScreenService.GetSex());
        }

        // Extracting strain list
        [HttpGet("GetStrain")]
        [AllowAnonymous]
        public IActionResult GetStrain()
        {
            return new JsonResult(_pubScreenService.GetStrains());
        }

        // Extracting Disease Models list
        [HttpGet("GetDisease")]
        [AllowAnonymous]
        public IActionResult GetDisease()
        {
            return new JsonResult(_pubScreenService.GetDisease());
        }

        // Extracting Regions  list
        [HttpGet("GetRegion")]
        [AllowAnonymous]
        public IActionResult GetRegion()
        {
            return new JsonResult(_pubScreenService.GetRegions());
        }

        // Extracting Regions & Sub-Regions list
        [HttpGet("GetRegionSubRegion")]
        [AllowAnonymous]
        public IActionResult GetRegionSubRegion()
        {
            return new JsonResult(_pubScreenService.GetAllRegions());
        }

        // Extracting Celltype list
        [HttpGet("GetCellType")]
        [AllowAnonymous]
        public IActionResult GetCellType()
        {
            return new JsonResult(_pubScreenService.GetCellTypes());
        }

        // Extracting Method list
        [HttpGet("GetMethod")]
        [AllowAnonymous]
        public IActionResult GetMethod()
        {
            return new JsonResult(_pubScreenService.GetMethods());
        }

        // Extracting NeuroTransmitter list
        [HttpGet("GetNeurotransmitter")]
        [AllowAnonymous]
        public IActionResult GetNeurotransmitter()
        {
            return new JsonResult(_pubScreenService.GetNeurotransmitters());
        }

        // Adding new author to database
        [HttpPost("AddAuthor")]
        [AllowAnonymous]
        public IActionResult AddAuthor([FromBody] PubScreenAuthor author)
        {
            return new JsonResult(_pubScreenService.AddAuthors(author));
        }


        // Extracting Author list from Database (pubScreen)
        [HttpGet("GetAuthor")]
        [AllowAnonymous]
        public IActionResult GetAuthor()
        {
            //var a = _pubScreenService.getArticleFromPubMedByDoiAsync("10.1016/j.jns.2018.02.001").Result;
            //var b = a;

            return new JsonResult(_pubScreenService.GetAuthors());
        }

        // Adding new author to database
        [HttpPost("AddPublication")]
        [AllowAnonymous]
        public IActionResult AddPublication([FromBody] PubScreen publication)
        {
            return new JsonResult(_pubScreenService.AddPublications(publication));
        }

        // searching Publications based on search criteria
        [HttpPost("SearchPublication")]
        [AllowAnonymous]
        public IActionResult SearchPublication([FromBody] PubScreen publication)
        {
            return new JsonResult(_pubScreenService.SearchPublications(publication));
        }

        // Getting list of all years from Database
        [HttpGet("GetAllYear")]
        [AllowAnonymous]
        public IActionResult GetAllYear()
        {
            return new JsonResult(_pubScreenService.GetAllYears());
        }

        // Getting some paper information based on Doi from pubMed
        [HttpGet("GetPaperInfoByDOI")]
        [AllowAnonymous]
        public IActionResult GetPaperInfoByDOI(string DOI)
        {
             return new JsonResult(_pubScreenService.GetPaperInfoByDoi(DOI));
        }

        // Getting some paper information based on pubMed key 
        [HttpGet("GetPaperInfoByPubKey")]
        [AllowAnonymous]
        public IActionResult GetPaperInfoByPubKey(string PubKey)
        {
            return new JsonResult(_pubScreenService.GetPaperInfoByPubMedKey(PubKey));
        }

        // Getting some paper information based on Doi from BioRxiv
        [HttpGet("GetPaparInfoFromDOIBio")]
        [AllowAnonymous]
        public IActionResult GetPaparInfoFromDOIBio(string DOI)
        {
            return new JsonResult(_pubScreenService.GetPaperInfoByDOIBIO(DOI));
        }

    }
}
