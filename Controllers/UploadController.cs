using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text.RegularExpressions;
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

        // The main Upload function for uploading multiple files
        [HttpPost("UploadFiles")]
        [RequestSizeLimit(900_000_000)]
        public async Task<IActionResult> UploadFiles()
        {
            var files = HttpContext.Request.Form.Files;
            int expID = Int16.Parse(HttpContext.Request.Form["expId"][0]);
            int subExpId = Int16.Parse(HttpContext.Request.Form["subExpId"][0]);
            string SessionName = HttpContext.Request.Form["sessionName"][0];
            int sessionID = Int16.Parse(HttpContext.Request.Form["sessionID"][0]);

            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            var userID = user.Id;

            string TaskName = await _uploadService.GetTaskNameAsync(expID);
            int TaskID = await _uploadService.GetTaskIDAsync(expID);
            string ExpName = await _uploadService.GetExpNameAsync(expID);

            List<FileUploadResult> result = await _uploadService.UploadFiles(files, TaskName, expID, subExpId, ExpName, userEmail, userID, SessionName, TaskID, sessionID);

            // add a function to send an email to inform admin that new data added to the server

            return new JsonResult(result);
        }

        [HttpPost("UploadTimeSerires")]
        [RequestSizeLimit(900_000_000)]
        public async Task<IActionResult> UploadTimeSerires()
        {
            var files = HttpContext.Request.Form.Files;

            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            var userID = user.Id;

            var result = await _uploadService.UploadTimeSeriesFiles(files, userEmail, userID);
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
        public async Task<IActionResult> SetUploadAsResolved(int uploadId)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            var userEmail = user.UserName;
            var userID = user.Id;
            var res = await _uploadService.SetUploadAsResolvedAsync(uploadId, userID);

            return new JsonResult(res);
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

            var fur = await _uploadService.GetUploadByUploadIDAsync(uploadId);
            var path = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){

                path = fur.PermanentFilePath + "\\" + fur.SysFileName;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {

                path = fur.PermanentFilePath + "/" + fur.SysFileName;
                // Checks for patterns like "C:\" or "D:\"
                if (Regex.IsMatch(fur.PermanentFilePath, @"^[A-Za-z]:\\"))
                {
                    string[] parts = Regex.Split(fur.PermanentFilePath, @"\\");

                    path = "/app/UPLOAD/";
                    Boolean afterUpload = false;
                    foreach (string part in parts) {
                        if (afterUpload) {
                            path += part + "/";
                        }
                        if (part.Equals("UPLOAD"))
                        {
                            afterUpload = true;
                        }
                    }
                    path += fur.SysFileName;
                }
            }

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
