using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class SearchExp
    {
        public string ExpName { get; set; }
        public string CognitiveTask { get; set; }
        public string Status { get; set; }
        public string Age { get; set; }
        public string Strain { get; set; }
        public string Genotype { get; set; }
        public string TaskDescription { get; set; }
        public string Period { get; set; }
        public string PISiteName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DOI { get; set; }

    } 
}
