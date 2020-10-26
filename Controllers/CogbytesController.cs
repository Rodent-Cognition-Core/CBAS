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
    public class CogbytesController : Controller
    {
        private readonly CogbytesService _cogbytesService;

        private readonly UserManager<ApplicationUser> _manager;

        public CogbytesController(UserManager<ApplicationUser> manager)
        {
            _cogbytesService = new CogbytesService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        // Extracting paper type list
        [HttpGet("GetFileTypes")]
        [AllowAnonymous]
        public IActionResult GetFileTypes()
        {
            return new JsonResult(_cogbytesService.GetFileTypes());
        }

        // Extracting cognitive task list
        [HttpGet("GetTask")]
        [AllowAnonymous]
        public IActionResult GetTask()
        {
            return new JsonResult(_cogbytesService.GetTasks());
        }

        //// Extracting species list
        [HttpGet("GetSpecies")]
        [AllowAnonymous]
        public IActionResult GetSpecies()
        {
            return new JsonResult(_cogbytesService.GetSpecies());
        }

        //// Extracting sex list
        [HttpGet("GetSex")]
        [AllowAnonymous]
        public IActionResult GetSex()
        {
            return new JsonResult(_cogbytesService.GetSex());
        }

        //// Extracting strain list
        [HttpGet("GetStrain")]
        [AllowAnonymous]
        public IActionResult GetStrain()
        {
            return new JsonResult(_cogbytesService.GetStrains());
        }

        //// Extracting genotype list
        [HttpGet("GetGenos")]
        [AllowAnonymous]
        public IActionResult GetGenos()
        {
            return new JsonResult(_cogbytesService.GetGenos());
        }

        //// Extracting age list
        [HttpGet("GetAges")]
        [AllowAnonymous]
        public IActionResult GetAges()
        {
            return new JsonResult(_cogbytesService.GetAges());
        }

        //// Adding new author to database
        [HttpPost("AddAuthor")]
        public IActionResult AddAuthor([FromBody] PubScreenAuthor author)
        {
            var user = GetCurrentUser();
            var userEmail = user.Result.UserName;
            return new JsonResult(_cogbytesService.AddAuthors(author, userEmail));
        }


        //// Extracting Author list from Database (pubScreen)
        //[HttpGet("GetAuthor")]
        //[AllowAnonymous]
        //public IActionResult GetAuthor()
        //{
        //    //var a = _pubScreenService.getArticleFromPubMedByDoiAsync("10.1016/j.jns.2018.02.001").Result;
        //    //var b = a;

        //    return new JsonResult(_pubScreenService.GetAuthors());
        //}

        //// Adding new publication to database
        //[HttpPost("AddPublication")]
        ////[AllowAnonymous]
        //public IActionResult AddPublication([FromBody] PubScreen publication)
        //{
        //    var user = GetCurrentUser();
        //    var userEmail = user.Result.UserName;
        //    return new JsonResult(_pubScreenService.AddPublications(publication, userEmail));
        //}

        //// Edit an existing publication
        //[HttpPost("EditPublication")]
        //public IActionResult EditPublication(int publicationId, [FromBody] PubScreen publication)
        //{
        //    var user = GetCurrentUser();
        //    var userEmail = user.Result.UserName;
        //    return new JsonResult(_pubScreenService.EditPublication(publicationId, publication, userEmail));
        //}

        //// Deleting publication
        //[HttpDelete("DeletePublicationById")]
        //public IActionResult DeletePublicationById(int pubId)
        //{
        //    _pubScreenService.DeletePublicationById(pubId);
        //    return new JsonResult("Done!");
        //}


        //// searching Publications based on search criteria
        //[HttpPost("SearchPublication")]
        //[AllowAnonymous]
        //public IActionResult SearchPublication([FromBody] PubScreen publication)
        //{
        //    return new JsonResult(_pubScreenService.SearchPublications(publication));
        //}

        //// Getting list of all years from Database
        //[HttpGet("GetAllYear")]
        //[AllowAnonymous]
        //public IActionResult GetAllYear()
        //{
        //    return new JsonResult(_pubScreenService.GetAllYears());
        //}

        //// Getting some paper information based on Doi from pubMed
        //[HttpGet("GetPaperInfoByDOI")]
        //[AllowAnonymous]
        //public IActionResult GetPaperInfoByDOI(string DOI)
        //{
        //     return new JsonResult(_pubScreenService.GetPaperInfoByDoi(DOI));
        //}

        //// Getting some paper information based on pubMed key 
        //[HttpGet("GetPaperInfoByPubKey")]
        //[AllowAnonymous]
        //public IActionResult GetPaperInfoByPubKey(string PubKey)
        //{
        //    return new JsonResult(_pubScreenService.GetPaperInfoByPubMedKey(PubKey));
        //}

        //// Getting some paper information based on Doi from BioRxiv
        //[HttpGet("GetPaparInfoFromDOIBio")]
        //[AllowAnonymous]
        //public IActionResult GetPaparInfoFromDOIBio(string DOI)
        //{
        //    return new JsonResult(_pubScreenService.GetPaperInfoByDOIBIO(DOI));
        //}

        //[HttpGet("GetPaparInfoByID")]
        //public IActionResult GetPaparInfoByID(int ID)
        //{
        //    return new JsonResult(_pubScreenService.GetPaperInfoByID(ID));
        //}

    }
}
