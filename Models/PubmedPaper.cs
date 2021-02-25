using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class PubmedPaper
    {
        public PubScreen Paper { get; set; }
        public int? PubmedID { get; set; }
        public string PubDate { get; set; }
        public string QueueDate { get; set; }
        public string DOI { get; set; }
    }
}
