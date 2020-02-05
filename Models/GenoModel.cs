using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class Geno
    {
        public int ID { get; set; }
        public string Genotype { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        
    }
}
