using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Controllers
{
    /// <summary>
    /// Resources Web API controller.
    /// </summary>
    [Route("api/[controller]")]
    // Authorization policy for this API.
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class SearchExpController : Controller
    {
        private readonly SearchExpService _searchExperiment;

        private readonly UserManager<ApplicationUser> _manager;

        private readonly IHostingEnvironment _hostingEnvironment;



        public SearchExpController(UserManager<ApplicationUser> manager, IHostingEnvironment hostingEnvironment)
        {
            _searchExperiment = new SearchExpService();
            _manager = manager;
            _hostingEnvironment = hostingEnvironment;

        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        [AllowAnonymous]
        [HttpGet("GetSearchList")]
        public IActionResult GetSearchList()
        {

            return new JsonResult(_searchExperiment.GetAllExpList(0));

        }

        [AllowAnonymous]
        [HttpGet("GetSearchByExpID")]
        public IActionResult GetSearchByExpID(int expID)
        {

            return new JsonResult(_searchExperiment.GetAllExpList(expID));

        }

        [HttpGet("DownloadExpDs")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadExpDs(string expDsFileName)
        {
            var expId = expDsFileName.Replace("ds_", "");
            var exp = _searchExperiment.GetAllExpList(int.Parse(expId));
            if (exp[0].Status != "Public") {
                return new JsonResult("");
            }
            var expDsFilePath = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                expDsFilePath = _hostingEnvironment.ContentRootPath + "\\UPLOAD\\datasets\\" + expDsFileName + ".csv";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                expDsFilePath = _hostingEnvironment.ContentRootPath + "/UPLOAD/datasets/" + expDsFileName + ".csv";
            }
            var memory = new MemoryStream();
            using (var stream = new FileStream(expDsFilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(expDsFilePath), expDsFileName + ".csv");
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
    }
}
