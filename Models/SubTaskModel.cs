using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class SubTask
    {
        public int ID { get; set; }
        public int Task_ID { get; set; }
        public string Name{ get; set; }
        public string OriginalName { get; set; }
        public string SubTaskDescription { get; set; }

    }
}
