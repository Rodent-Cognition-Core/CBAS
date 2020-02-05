using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class Mapping_SessionInfo
    {
        public int ID { get; set; }
        public string OriginalName { get; set; }
        public string MappedName { get; set; }

    }
}
