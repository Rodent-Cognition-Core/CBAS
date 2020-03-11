using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AngularSPAWebAPI.Models
{
    public class DataExtraction
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public int[] ExpIDs { get; set; }
        public int SubTaskID { get; set; }
        public string SubTaskName { get; set; }
        public string[] SessionInfoNames { get; set; }
        public string[] MarkerInfoNames { get; set; }
        public string[] AggNames { get; set; }
        public int[] PiSiteIDS { get; set; }
        public int[] AgeVals { get; set; }
        public string[] SexVals { get; set; }
        public int[] GenotypeVals { get; set; }
        public int[] StrainVals { get; set; }
        public bool IsTrialByTrials { get; set; }
        public int[] SubExpID { get; set; }
        public string[] SessionName { get; set; }
        public int SpeciesID { get; set; }
        public string Species { get; set; }

    }

    public class GetDataResult
    {
        public Guid LinkGuid { get; set; }
        public String Description { get; set; }
        public int SubTaskID { get; set; }
        public IEnumerable<dynamic> ListOfRows { get; set; }
    }
}
