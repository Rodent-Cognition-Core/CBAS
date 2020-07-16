using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class JsonPubscreen
    {
        public JsonPubscreenMessage[] messages { get; set; }
        public JsonPubscreenFeature[] collection { get; set; }
        
    }

    public class JsonPubscreenFeature
    {
        public string doi { get; set; }
        public string title { get; set; }
        public string authors { get; set; }
        public string author_corresponding { get; set; }
        public string date { get; set; }
        public string type { get; set; }
        public string @abstract {get; set; }

    }

    public class JsonPubscreenMessage
    {
        public string status { get; set; }
    }

}
