using CBAS.Models;
using Microsoft.AspNetCore.Identity;
using System;
namespace AngularSPAWebAPI.Models
{
    public class Cogbytes
    {
        public int? ID { get; set; }
        public Guid RepoLinkGuid { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Keywords { get; set; }
        public string DOI { get; set; }
        public int?[] AuthourID { get; set; }
        public string AuthorString { get; set; }
        public string CountryString { get; set; }
        public int?[] PIID { get; set; }
        public string PIString { get; set; }
        public string Link { get; set; }
        public int?[] TaskID { get; set; }
        public int?[] SubTaskID { get; set; }
        public int?[] SpecieID { get; set; }
        public int?[] SexID { get; set; }
        public int?[] StrainID { get; set; }
        public int?[] GenoID { get; set; }
        public int?[] DiseaseID { get; set; }
        public int?[] SubModelID { get; set; }
        public int?[] RegionID { get; set; }
        public int?[] SubRegionID { get; set; }
        public int?[] CellTypeID { get; set; }
        public int?[] MethodID { get; set; }
        public int?[] SubMethodID { get; set; }
        public int?[] TransmitterID { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? StartAge { get; set; }
        public int? EndAge { get; set; }
        public string TaskOther { get; set; }
        public string SpecieOther { get; set; }
        public string StrainMouseOther { get; set; }
        public string StrainRatOther { get; set; }
        public string DiseaseOther { get; set; }
        public string CelltypeOther { get; set; }
        public string MethodOther { get; set; }
        public string NeurotransOther { get; set; }
        public bool PrivacyStatus { get; set; }
        public string Description { get; set; }
        public string AdditionalNotes { get; set; }
        public string DateRepositoryCreated { get; set; }
        public System.Collections.Generic.List<Experiment> Experiment { get; set; }
        public PubScreenSearch Paper { get; set; }
        public string DataCiteURL { get; set; }
    }
}
