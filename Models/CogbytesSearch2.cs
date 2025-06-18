using CBAS.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    public class CogbytesSearch2
    {

        public int RepoID { get; set; }
        public Guid RepoLinkGuid { get; set; }
        public int UploadID { get; set; }
        public string UploadName { get; set; }
        public string DateUpload { get; set; }
        public string UploadDescription { get; set; }
        public string UploadAdditionalNotes { get; set; }
        public bool IsIntervention { get; set; }
        public string InterventionDescription { get; set; }
        public string Housing { get; set; }
        public string LightCycle { get; set; }
        public string TaskBattery { get; set; }
        public string FileType { get; set; }
        public string Task { get; set; }
        public string Species { get; set; }
        public string Sex { get; set; }
        public string Strain { get; set; }
        public string GenoType { get; set; }
        public string Age { get; set; }
        public int? NumSubjects { get; set; }

        public System.Collections.Generic.List<FileUploadResult> UploadFileList { get; set; }


    }
}
