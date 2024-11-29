using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AngularSPAWebAPI.Models
{
    public class PubScreen
    {
        public int? ID { get; set; } = null;

        public Guid PaperLinkGuid { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string DOI { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public int?[] YearID { get; set; } = new int?[0];
        public int?[] AuthourID { get; set; } = new int?[0];
        public string AuthorString { get; set; } = string.Empty;
        public int? PaperTypeID { get; set; } = null;
        public int?[] PaperTypeIdSearch { get; set; } = new int?[0];
        public string PaperType { get; set; } = string.Empty;
        public int?[] TaskID { get; set; } = new int?[0];
        public int?[] SubTaskID { get; set; } = new int?[0];
        public int?[] SpecieID { get; set; } = new int?[0];
        public int?[] sexID { get; set; } = new int?[0];
        public int?[] StrainID { get; set; } = new int?[0];
        public int?[] DiseaseID { get; set; } = new int?[0];
        public int?[] SubModelID { get; set; } = new int?[0];
        public int?[] RegionID { get; set; } = new int?[0];
        public int?[] SubRegionID { get; set; } = new int?[0];
        public int?[] CellTypeID { get; set; } = new int?[0];
        public int?[] MethodID { get; set; } = new int?[0];
        public int?[] SubMethodID { get; set; } = new int?[0];
        public int?[] TransmitterID { get; set; } = new int?[0];
        public string Reference { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int? YearFrom { get; set; } = null;
        public int? YearTo { get; set; } = null;
        public string TaskOther { get; set; } = string.Empty;
        public string SpecieOther { get; set; } = string.Empty;
        public string StrainMouseOther { get; set; } = string.Empty;
        public string StrainRatOther { get; set; } = string.Empty;
        public string DiseaseOther { get; set; } = string.Empty;
        public string CelltypeOther { get; set; } = string.Empty;
        public string MethodOther { get; set; } = string.Empty;
        public string NeurotransOther { get; set; } = string.Empty;
        public string search { get; set; } = string.Empty;


        public System.Collections.Generic.List<PubScreenAuthor> Author { get; set; } = new List<PubScreenAuthor>();


    }
}
