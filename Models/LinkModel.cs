using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    public class LinkModel
    {
        public int LinkID { get; set; }
        public Guid LinkGuid { get; set; }
        public string Description { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int SubTaskId { get; set; }
        public string SubTaskName { get; set; }
        public string ExpIdCsv { get; set; }
        public string AnimalAgeCsv { get; set; }
        public string AnimalSexCsv { get; set; }
        public string AnimalGenotypeCsv { get; set; }
        public string AnimalStrainCsv { get; set; }
        public string PiSiteIdsCsv { get; set; }
        public string SessionInfoNamesCsv { get; set; }
        public string MarkerInfoNamesCsv { get; set; }
        public string AggNamesCsv { get; set; }
        public bool IsTrialByTrials { get; set; }
        public string SubExpIDcsv { get; set; }
        public string SessionNameCsv { get; set; }

    }
}
