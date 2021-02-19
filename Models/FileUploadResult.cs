using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    public class FileUploadResult
    {
        public int UploadID { get; set; }
        public int ExpID { get; set; }
        public int TaskID { get; set; }
        public int SubExpID { get; set; }
        public int AnimalID { get; set; }
        public string SessionName { get; set; }
        public string UserFileName { get; set; }
        public string SysFileName { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsDismissed { get; set; }
        public DateTime? DateUpload { get; set; }
        public DateTime? DateFileCreated { get; set; }
        public int? FileSize { get; set; }
        public string FileUniqueID { get; set; }
        public string FileContent { get; set; }
        public bool IsQcPassed { get; set; }
        public bool IsIdentifierPassed { get; set; }
        public string UserAnimalID { get; set; }
        public string PermanentFilePath { get; set; }
        public Animal AnimalObj { get; set; }
        public string SubExpNameAge { get; set; }
        public bool IsDuplicateSession { get; set; }

    }
}
