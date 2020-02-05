using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class PubScreenRegion
    {
        public int ID { get; set; }
        public int RID { get; set; }
        public string BrainRegion { get; set; }
        public string SubRegion { get; set; }

    }
}
