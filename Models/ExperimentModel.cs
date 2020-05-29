using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    public class Experiment
    {
        public int ExpID { get; set; }
        public string UserID { get; set; }
        public int PUSID { get; set; }
        public int TaskID { get; set; }
        public string ExpName { get; set; }
        public string SubExperimentNames { get; set; }
        public string TaskName { get; set; }
        public string PISiteName { get; set; }
        public string PISiteUser { get; set; }
        public string UserName { get; set; }
        public DateTime StartExpDate { get; set; }
        public DateTime EndExpDate { get; set; }
        //public string ErrorMessage { get; set; }
        public string TaskDescription { get; set; }
        public string TaskBattery { get; set; }
        public string DOI { get; set; }
        public bool Status { get; set; }
        public int SpeciesID { get; set; }
        public string Species { get; set; }
        //public bool IsPostProcessingPass { get; set; }
        public System.Collections.Generic.List<SubExperiment> SubExpList {get; set; }
    }
}
