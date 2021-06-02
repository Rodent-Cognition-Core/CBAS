using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AngularSPAWebAPI.Models
{
    public class Request
    {

        public int ID { get; set; }
        public string Type { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string taskName { get; set; }
        public string ScheduleName { get; set; }
        public string PIFullName { get; set; }
        public string PIEmail { get; set; }
        public string PIInstitution { get; set; }
        public string Age { get; set; }
        public string MouseStrain { get; set; }
        public string GeneticModification { get; set; }
        public string StrainReference { get; set; }
        public string ControlSuggestion { get; set; }
        public string TaskCategory { get; set; }
        public string DOI { get; set; }
        public string Model { get; set; }
        public string SubModel { get; set; }
        public string GeneralRequest { get; set; }


    }

    
}
