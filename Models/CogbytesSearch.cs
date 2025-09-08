using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class CogbytesSearch
    {
        public int?[] RepID { get; set; }
        public string Keywords { get; set; }
        public string DOI { get; set; }
        public int?[] AuthorID { get; set; }
        public int?[] PIID { get; set; }

        public int?[] TaskID { get; set; }
        public int?[] SpecieID { get; set; }
        public int?[] SexID { get; set; }
        public int?[] StrainID { get; set; }
        public int?[] GenoID { get; set; }
        public int?[] AgeID { get; set; }

        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string Intervention { get; set; }

        public int?[] FileTypeID { get; set; }
        public int?[] DiseaseID { get; set; }
        public int?[] SubModelID { get; set; }
        public int?[] RegionID { get; set; }
        public int?[] SubRegionID { get; set; }
        public int?[] CellTypeID { get; set; }
        public int?[] MethodID { get; set; }
        public int?[] SubMethodID { get; set; }
        public int?[] TransmitterID { get; set; }
    }
}
