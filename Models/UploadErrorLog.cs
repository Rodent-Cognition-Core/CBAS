using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    public class UploadErrorLog
    {
        public int Id { get; set; }
        public int ExpID { get; set; }
        public int SubExpID { get; set; }
        public string UserFileName { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UploadDate { get; set; }
        public string SubExpNameAge { get; set; }
    }
}
