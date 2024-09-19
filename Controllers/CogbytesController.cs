using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

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

        [HttpPost("AddAuthor")]
        public async Task<IActionResult> AddAuthor([FromBody] PubScreenAuthor author)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_cogbytesService.AddAuthors(author, userEmail));
        }

        [HttpGet("GetAuthor")]
        [AllowAnonymous]
        public IActionResult GetAuthor()
        {
            return new JsonResult(_cogbytesService.GetAuthors());
        }

        [HttpPost("AddPI")]
        public async Task<IActionResult> AddPI([FromBody] Request request)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_cogbytesService.AddNewPI(request, userEmail));
        }

        [HttpGet("GetPI")]
        [AllowAnonymous]
        public IActionResult GetPI()
        {
            return new JsonResult(_cogbytesService.GetPIs());
        }

        [HttpPost("AddRepository")]
        public async Task<IActionResult> AddRepository([FromBody] Cogbytes repository)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_cogbytesService.AddRepository(repository, userEmail));
        }

        [HttpGet("GetRepositories")]
        public async Task<IActionResult> GetRepositories()
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_cogbytesService.GetRepositories(userEmail));
        }

        [HttpPost("EditRepository")]
        public async Task<IActionResult> EditRepository(int repositoryID, [FromBody] Cogbytes repository)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            return new JsonResult(_cogbytesService.EditRepository(repositoryID, repository, userEmail));
        }

        [HttpPost("AddUpload")]
        public IActionResult AddUpload([FromBody] CogbytesUpload upload)
        {
            return new JsonResult(_cogbytesService.AddUpload(upload));
        }

        //// Extracting Upload file list from Database (Cogbytes)
        [HttpGet("GetUploadFiles")]
        //[AllowAnonymous]
        public IActionResult GetUploadFiles(int uploadID)
        {
            return new JsonResult(_cogbytesService.GetUploadFiles(uploadID));
        }

        //// Extracting Upload list from Database (Cogbytes)
        [HttpGet("GetUploads")]
        //[AllowAnonymous]
        public IActionResult GetUploads(int repID)
        {
            return new JsonResult(_cogbytesService.GetUploads(repID));
        }

        // Edit an existing publication
        [HttpPost("EditUpload")]
        public IActionResult EditUpload(int uploadID, [FromBody] CogbytesUpload upload)
        {
            return new JsonResult(_cogbytesService.EditUpload(uploadID, upload));
        }

        // The main Upload function for uploading multiple files
        [EnableCors("CorsPolicy")]
        [HttpPost("AddFiles")]
        //[RequestSizeLimit(Int64.MaxValue)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> AddFiles()
        {
            var files = HttpContext.Request.Form.Files;
            var types = GetMimeTypes();

            int uploadID = Int16.Parse(HttpContext.Request.Form["uploadID"][0]);

            foreach (var file in files)
            {
                var fileType = Path.GetExtension(file.FileName);
                try
                {
                    var typeCheck = types[fileType];
                }
                catch (KeyNotFoundException)
                {
                    throw;
                }
            }

            bool result = await _cogbytesService.AddFiles(files, uploadID);
            // add a function to send an email to inform admin that new data added to the server

            return new JsonResult(result);   
        }

        [HttpGet("DownloadFile")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(string path)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.ms-excel"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".xml", "text/xml"} ,
                {".bak", "application/bak"},
                {".c", "text/plain" },
                {".zip", "application/x-zip-compressed" },
                {".7z", "application/x-7z-compressed" },
                {".tex", "application/x-tex" },
                {".tar", "application/x-tar" },
                {".rar", "application/octet-stream" },
                {".latex", "application/x-latex" },
                {".cpp", "text/plain" },
                {".r", "text/plain" },
                {".py", "text/plain" },
                {".avi", "video/x-msvideo"},
                {".mp3", "audio/mpeg"},
                {".mp4", "video/mp4"},
                {".mp4v", "video/mp4"},
                {".h5", "application/x-hdf5"},
            };
        }

        // Deleting file
        [HttpDelete("DeleteFile")]
        public IActionResult DeleteFile(int fileID, string path)
        {
            System.IO.File.Delete(path);
            _cogbytesService.DeleteFile(fileID);
            return new JsonResult("Done!");
        }

        // Deleting upload
        [HttpDelete("DeleteUpload")]
        public IActionResult DeleteUpload(int uploadID)
        {
            string pathString = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "COGBYTES_FILES", uploadID.ToString());
            if (System.IO.Directory.Exists(pathString))
            {
                System.IO.Directory.Delete(pathString, true);
            }
            _cogbytesService.DeleteUpload(uploadID);
            return new JsonResult("Done!");
        }

        // Deleting repository
        [HttpDelete("DeleteRepository")]
        public IActionResult DeleteRepository(int repID)
        {
            _cogbytesService.DeleteRepository(repID);
            return new JsonResult("Done!");
        }

        // searching repositories based on search criteria
        [HttpPost("SearchRepositories")]
        [AllowAnonymous]
        public IActionResult SearchRepositories([FromBody] CogbytesSearch cogSearch)
        {
            return new JsonResult(_cogbytesService.SearchRepositories(cogSearch));
        }

        // show all repositories 
        [HttpGet("ShowAllRepositories")]
        [AllowAnonymous]
        public IActionResult ShowAllRepositories()
        {
            return new JsonResult(_cogbytesService.ShowAllRepositories());
        }

        //// Extracting Repository list from Database (Cogbytes)
        [HttpGet("GetAllRepositories")]
        [AllowAnonymous]
        public IActionResult GetAllRepositories()
        {
            return new JsonResult(_cogbytesService.GetAllRepositories());
        }

        // Function Definition to get a repo based on its Guid
        [AllowAnonymous]
        [HttpGet("GetDataByLinkGuid")]
        public IActionResult GetDataByLinkGuid(Guid repoLinkGuid)
        {
            // extract data from database to show in the client
            return new JsonResult(_cogbytesService.GetRepoFromCogbytesByLinkGuid(repoLinkGuid));

        }

        // Function Definition to get repo's Guid based on Repo ID
        [AllowAnonymous]
        [HttpGet("GetGuidByRepID")]
        public IActionResult GetGuidByRepID(int repID)
        {
            // extract data from database to show in the client
            return new JsonResult(_cogbytesService.GetGuidByRepID(repID));

        }



    }
}
