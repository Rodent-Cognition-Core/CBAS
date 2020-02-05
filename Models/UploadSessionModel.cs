using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class UploadSession
    {
        public int Id { get; set; }
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public string SessionName { get; set; }
        public string SessionDescription { get; set; }
               
    }
}
