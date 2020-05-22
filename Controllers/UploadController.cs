using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Controllers
{
    /// <summary>
    /// Resources Web API controller.
    /// </summary>
    [Route("api/[controller]")]
    // Authorization policy for this API.
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class UploadController : Controller
    {
        private readonly UploadService _uploadService;
        private readonly UserManager<ApplicationUser> _manager;

        // constructor
        public UploadController(UserManager<ApplicationUser> manager)
        {
            _uploadService = new UploadService();
            _manager = manager;

        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        // The main Upload function for uploading multiple files
        [EnableCors("CorsPolicy")]
        [HttpPost("UploadFiles")]
        [RequestSizeLimit(900_000_000)]
        public async Task<IActionResult> UploadFiles()
        {
            var files = HttpContext.Request.Form.Files;
            int expID = Int16.Parse(HttpContext.Request.Form["expId"][0]);
            int subExpId = Int16.Parse(HttpContext.Request.Form["subExpId"][0]);
            string SessionName = HttpContext.Request.Form["sessionName"][0];
            int sessionID = Int16.Parse(HttpContext.Request.Form["sessionID"][0]);

            var user = GetCurrentUser();
            var userEmail = user.Result.UserName;
            var userID = user.Result.Id;

            string TaskName = _uploadService.GetTaskName(expID);
            int TaskID = _uploadService.GetTaskID(expID);
            string ExpName = _uploadService.GetExpName(expID);

            List<FileUploadResult> result = await _uploadService.UploadFiles(files, TaskName, expID, subExpId, ExpName, userEmail, userID, SessionName, TaskID, sessionID);

            return new JsonResult(result);
        }

        [HttpGet("GetUploadInfoByID")]
        public IActionResult GetUploadInfoByID(int expId)
        {

            return new JsonResult(_uploadService.GetUploadInfoByExpID(expId));
        }

        [HttpGet("GetUploadErrorLogByID")]
        public IActionResult GetUploadErrorLogByID(int expId)
        {

            return new JsonResult(_uploadService.GetUploadErrorLogByExpID(expId));
        }

        [HttpGet("SetUploadAsResolved")]
        public IActionResult SetUploadAsResolved(int uploadId)
        {
            var user = GetCurrentUser();
            var userEmail = user.Result.UserName;
            var userID = user.Result.Id;
            // Console.WriteLine("it is here!!!" + DateTime.Now.ToString("hh:mm:ss"));

            return new JsonResult(_uploadService.SetUploadAsResolved(uploadId, userID));
        }

        //Uploadinfo for Experiment page
        [HttpGet("GetUploadInfoBySubExpId")]
        public IActionResult GetUploadInfoBySubExpId(int subExpId)
        {

            return new JsonResult(_uploadService.GetUploadInfoBySubExpIDForExperiemnt(subExpId));
        }         

        [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(int uploadId)
        {

            var fur = _uploadService.GetUploadByUploadID(uploadId);
            var path = fur.PermanentFilePath + "\\" + fur.SysFileName;

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
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".xml", "text/xml"}
            };
        }

        [HttpDelete("DelUploadLogTblbyId")]
        public IActionResult DelUploadLogTblbyId(int expID)
        {
            _uploadService.ClearUploadLogTblbyID(expID);
            return new JsonResult("Done!");

        }

        [AllowAnonymous]
        [HttpGet("GetSessionInfo")]
        public IActionResult GetSessionInfo()
        {

            return new JsonResult(_uploadService.GetAllSessionInfo());
        }

    }
}
