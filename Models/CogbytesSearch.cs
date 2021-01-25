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

    }
}
