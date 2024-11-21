using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Nest;
using Microsoft.AspNetCore.Http;



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
        private readonly IElasticClient _elasticClient;
        public PubScreenController(UserManager<ApplicationUser> manager, IElasticClient client)
        {
            _elasticClient = client;
            _pubScreenService = new PubScreenService(_elasticClient);
            _manager = manager;
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

        // Extracting Regions & Sub-Regions list
        [HttpGet("GetTaskSubTask")]
        [AllowAnonymous]
        public IActionResult GetTaskSubTask()
        {
            return new JsonResult(_pubScreenService.GetAllTasks());
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

        // Extracting Disease Models list
        [HttpGet("GetSubModels")]
        [AllowAnonymous]
        public IActionResult GetSubModels()
        {
            return new JsonResult(_pubScreenService.GetSubModels());
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

        // Extracting SubMethod list
        [HttpGet("GetSubMethod")]
        [AllowAnonymous]
        public IActionResult GetSubMethod()
        {
            return new JsonResult(_pubScreenService.GetSubMethods());
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
        public async Task<IActionResult> AddAuthor([FromBody] PubScreenAuthor author)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_pubScreenService.AddAuthors(author, userEmail));
        }


        // Extracting Author list from Database (pubScreen)
        [HttpGet("GetAuthor")]
        [AllowAnonymous]
        public IActionResult GetAuthor()
        {
            return new JsonResult(_pubScreenService.GetAuthors());
        }

        // Adding new publication to database
        [HttpPost("AddPublication")]
        //[AllowAnonymous]
        public async Task<IActionResult> AddPublication([FromBody] PubScreen publication)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_pubScreenService.AddPublications(publication, userEmail));
        }

        // Edit an existing publication
        [HttpPost("EditPublication")]
        public async Task<IActionResult> EditPublication(int publicationId, [FromBody] PubScreen publication)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_pubScreenService.EditPublication(publicationId, publication, userEmail));
        }

        // Edit an existing publication
        [HttpPost("EditPublicationPublic")]
        [AllowAnonymous]
        public IActionResult EditPublicationPublic(int publicationId, [FromBody] PubScreen publication)
        {
            return new JsonResult(_pubScreenService.EditPublication(publicationId, publication, "Public"));
        }

        // Deleting publication
        [HttpDelete("DeletePublicationById")]
        public IActionResult DeletePublicationById(int pubId)
        {
            _pubScreenService.DeletePublicationById(pubId);
            return new JsonResult("Done!");
        }


        // searching Publications based on search criteria
        [HttpPost("SearchPublication")]
        [AllowAnonymous]
        public IActionResult SearchPublication([FromBody] PubScreen publication)
        {
            return new JsonResult(_pubScreenService.ElasticSearchPublications(publication));
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
        public async Task<IActionResult> GetPaperInfoByDOI(string DOI)
        {
            var result = await _pubScreenService.GetPaperInfoByDoi(DOI);
            return new JsonResult(result);
        }

        // Getting some paper information based on pubMed key 
        [HttpGet("GetPaperInfoByPubKey")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaperInfoByPubKey(string PubKey)
        {
            var result = await _pubScreenService.GetPaperInfoByPubMedKey(PubKey);
            return new JsonResult(result);
        }

        // Getting some paper information based on Doi from BioRxiv
        [HttpGet("GetPaparInfoFromDOIBio")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaparInfoFromDOIBio(string DOI)
        {
            var result = await _pubScreenService.GetPaperInfoByDOIBIO(DOI);
            return new JsonResult(result);
        }

        // Getting some paper information based on Doi from Crossref
        [HttpGet("GetPaparInfoFromDOICrossref")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaperInfoByDOICrossref(string DOI)
        {
            var result = await _pubScreenService.GetPaperInfoByDOICrossref(DOI);
            return new JsonResult(result);
        }

        [HttpGet("GetPaparInfoByID")]
        [AllowAnonymous]
        public IActionResult GetPaparInfoByID(int ID)
        {
            return new JsonResult(_pubScreenService.GetPaperInfoByID(ID));
        }

        [HttpGet("GetPubmedQueue")]
        public IActionResult GetPubmedQueue()
        {
            return new JsonResult(_pubScreenService.GetPubmedQueue());
        }

        [HttpGet("AddQueuePaper")] //HttpPost results in failed authentication
        public async Task<IActionResult> AddQueuePaper(int pubmedID, string doi)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_pubScreenService.AddQueuePaper(pubmedID, doi, userEmail));
        }

        [HttpDelete("RejectQueuePaper")]
        public IActionResult RejectQueuePaper(int pubmedID, string doi)
        {

            _pubScreenService.ProcessQueuePaper(pubmedID, doi);
            return new JsonResult("Done!");
        }

        [HttpGet("GetPubCount")]
        [AllowAnonymous]
        public IActionResult GetPubCount()
        {
            var result = _pubScreenService.GetPubCount();
            //  When the Value property of JsonResult is set to a tuple, it might not be properly serialized into JSON, leading to a null object on the client side.
            var tempObject = new
            {
                pubCount = result.Item1,
                featureCount = result.Item2,
            };
            return new JsonResult(tempObject);
        }

        [HttpGet("AddCSVPapers")]
        public async Task<IActionResult> AddCSVPapers()
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_pubScreenService.AddCSVPapers(userEmail));
        }

        [AllowAnonymous]
        [HttpGet("GetDataByLinkGuid")]
        public IActionResult GetDataByLinkGuid(Guid paperLinkGuid)
        {
            // extract data from database to show in the client
            return new JsonResult(_pubScreenService.GetDataFromPubScreenByLinkGuid(paperLinkGuid));

        }

        [AllowAnonymous]
        [HttpGet("GetGuidByDoi")]
        public IActionResult GetGuidByDoi(string doi)
        {
            // extract data from database to show in the client
            return new JsonResult(_pubScreenService.GetGuidByDoi(doi));

        }

    }
}
