using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class TaskAnalysis
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string TaskDescription { get; set; }

    }
}
