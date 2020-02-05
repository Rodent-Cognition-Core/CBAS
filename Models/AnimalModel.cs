using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class Animal
    {
        public int AnimalID { get; set; }
        public int ExpID { get; set; }
        public int? GID { get; set; }
        public int? SID { get; set; }
        public string UserAnimalID { get; set; }
        public string Sex { get; set; }
        public string Genotype { get; set; }
        public string Strain { get; set; }

    }
}
