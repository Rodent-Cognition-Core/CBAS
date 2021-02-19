using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class PubScreenTask
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public string Task { get; set; }
        public string SubTask { get; set; }
        public string username { get; set; }

    }
}
