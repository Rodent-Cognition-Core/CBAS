using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class CogbytesUpload
    {
        public int? ID { get; set; }
        public int? RepID { get; set; }
        public int? FileTypeID { get; set; }
        public string Name { get; set; }
        public string DateUpload { get; set; }
        public string Description { get; set; }
        public string AdditionalNotes { get; set; }
        public bool IsIntervention { get; set; }
        public string InterventionDescription { get; set; }
        public string ImageIds { get; set; }
        public string ImageDescription { get; set; }
        public string Housing { get; set; }
        public string LightCycle { get; set; }
        public string TaskBattery { get; set; }
        public int?[] TaskID { get; set; }
        public int?[] SpecieID { get; set; }
        public int?[] SexID { get; set; }
        public int?[] StrainID { get; set; }
        public int?[] GenoID { get; set; }
        public int?[] AgeID { get; set; }
    }
}
