using CBAS.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    public class CogBytesMetaData
    {
        public int RepoID { get; set; }
        public Guid RepoLinkGuid { get; set; }
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
        public System.Collections.Generic.List<Experiment> Experiment { get; set; }
        public PubScreenSearch Paper { get; set; }
    }
}