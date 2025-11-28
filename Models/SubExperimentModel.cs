using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    public class SubExperiment
    {
        public int SubExpID { get; set; }
        public int ExpID { get; set; }
        public int AgeID { get; set; }

        public DateTime StartAge { get; set; }
        public DateTime EndAge { get; set; }
        public string AgeInMonth { get; set; }
        public string SubExpName { get; set; }
        public bool IsPostProcessingPass { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsIntervention { get; set; }
        public bool IsDrug { get; set; }
        public string DrugName { get; set; }
        public string DrugUnit { get; set; }
        public string DrugQuantity { get; set; }
        public string InterventionDescription { get; set; }
        public string ImageInfo { get; set; }
        public int[] ImageIds { get; set; }
        public string ImageDescription { get; set; }
        public string Housing { get; set; }
        public string LightCycle { get; set; }


    }
}
