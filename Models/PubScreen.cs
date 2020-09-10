using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class PubScreen
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Keywords { get; set; }
        public string DOI { get; set; }
        public string Year { get; set; }
        public int?[] YearID { get; set; }
        public int?[] AuthourID { get; set; }
        public string AuthorString { get; set; }
        public int? PaperTypeID { get; set; }
        public int?[] PaperTypeIdSearch { get; set; }
        public string PaperType { get; set; }
        public int?[] TaskID { get; set; }
        public int?[] SpecieID { get; set; }
        public int?[] sexID { get; set; }
        public int?[] StrainID { get; set; }
        public int?[] DiseaseID { get; set; }
        public int?[] RegionID { get; set; }
        public int?[] SubRegionID { get; set; }
        public int?[] CellTypeID { get; set; }
        public int?[] MethodID { get; set; }
        public int?[] TransmitterID { get; set; }
        public string Reference { get; set; }
        public string Source { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }

        public System.Collections.Generic.List<PubScreenAuthor> Author { get; set; }


    }
}
