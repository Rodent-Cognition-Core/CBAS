using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class MarkerData
    {
        public int ID { get; set; }
        public int SessionID{ get; set; }
        public int SourceTypeID { get; set; }
        public string FeatureName { get; set; }
        public float? Results { get; set; }
        public float? Time { get; set; }
        public float? Duration { get; set; }
        public int? Count { get; set; }
       

    }
}
