using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class SessionInfo_Dynamic
    {
        public int ID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

    }
}
