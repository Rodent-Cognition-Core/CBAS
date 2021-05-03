using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class PubScreenStrain
    {
        public int ID { get; set; }
        public string Strain { get; set; }
        public int SpeciesID { get; set; }
        public string username { get; set; }

    }
}

