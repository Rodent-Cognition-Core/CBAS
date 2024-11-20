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
        public async Task<IActionResult> GetPaperType()
        {
            var res = await _pubScreenService.GetPaperTypesAsync();
            return new JsonResult(res);
        }

        // Extracting cognitive task list
        [HttpGet("GetTask")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTask()
        {
            var res = await _pubScreenService.GetTasksAsync();
            return new JsonResult(res);
        }

        // Extracting Regions & Sub-Regions list
        [HttpGet("GetTaskSubTask")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTaskSubTask()
        {
            var res = await _pubScreenService.GetAllTasksAsync();
            return new JsonResult(res);
        }

        // Extracting species list
        [HttpGet("GetSpecie")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSpecie()
        {
            var res = await _pubScreenService.GetSpeciesAsync();
            return new JsonResult(res);
        }

        // Extracting sex list
        [HttpGet("GetSex")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSex()
        {
            var res = await _pubScreenService.GetSexAsync();
            return new JsonResult(res);
        }

        // Extracting strain list
        [HttpGet("GetStrain")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStrain()
        {
            var res = await _pubScreenService.GetStrainsAsync();
            return new JsonResult(res);
        }

        // Extracting Disease Models list
        [HttpGet("GetDisease")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDisease()
        {
            var res = await _pubScreenService.GetDiseaseAsync();
            return new JsonResult(res);
        }

        // Extracting Disease Models list
        [HttpGet("GetSubModels")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubModels()
        {
            var res = await _pubScreenService.GetSubModelsAsync();
            return new JsonResult(res);
        }

        // Extracting Regions  list
        [HttpGet("GetRegion")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegion()
        {
            var res = await _pubScreenService.GetRegionsAsync();
            return new JsonResult(res);
        }

        // Extracting Regions & Sub-Regions list
        [HttpGet("GetRegionSubRegion")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegionSubRegion()
        {
            var res = await _pubScreenService.GetAllRegionsAsync();
            return new JsonResult(res);
        }

        // Extracting Celltype list
        [HttpGet("GetCellType")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCellType()
        {
            var res = await _pubScreenService.GetCellTypesAsync();
            return new JsonResult(res);
        }

        // Extracting Method list
        [HttpGet("GetMethod")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMethod()
        {
            var res = await _pubScreenService.GetMethodsAsync();
            return new JsonResult(res);
        }

        // Extracting SubMethod list
        [HttpGet("GetSubMethod")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubMethod()
        {
            var res = await _pubScreenService.GetSubMethodsAsync();
            return new JsonResult(res);
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
