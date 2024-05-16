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
        public string Title { get; set; }
        public string Date { get; set; }
        public string DOI { get; set; }
        public string Keywords { get; set; }
        public bool PrivacyStatus { get; set; }
        public string Description { get; set; }
        public string AdditionalNotes { get; set; }
        public string Link { get; set; }
        public string Username { get; set; }
        public string DateRepositoryCreated { get; set; }
        public string Author { get; set; }
        public string PI { get; set; }
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
        public System.Collections.Generic.List<Experiment> Experiment { get; set; }
        public PubScreenSearch Paper { get; set; }

        public System.Collections.Generic.List<FileUploadResult> UploadFileList { get; set; }


    }
}
