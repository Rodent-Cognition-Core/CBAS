using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;

namespace AngularSPAWebAPI.Services
{

    public class PubScreenService
    {
        // Function Definition to get paper info from DOI
        // private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetPaperInfoByDoi(string doi)
        {
            // Submiy doi to get the pubmedkey

            HttpClient httpClient = new HttpClient();

            StringContent content = new System.Net.Http.StringContent(String.Empty);

            var response = await httpClient.PostAsync("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&WebEnv=1&usehistory=y&term=" + doi + "&rettype=Id", content);
            var responseString = await response.Content.ReadAsStringAsync();

            XElement incomingXml = XElement.Parse(responseString);
            var pubMedKey = incomingXml.Element("IdList").Element("Id").Value;

            //Send pubmedkey to another function to get Paper's info
            await GetPaperInfoByPubMedKey(pubMedKey);



            return pubMedKey;

        }

        //Function Definition to get some paper's info based on PubMedKey
        public async Task<string> GetPaperInfoByPubMedKey(string pubMedKey)
        {

            HttpClient httpClient = new HttpClient();

            var content = new StringContent(String.Empty, Encoding.UTF8, "application/xml");

            var response = await httpClient.PostAsync("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=" + pubMedKey + "&retmode=xml", content);
            var responseString = await response.Content.ReadAsStringAsync();
            XElement incomingXml = XElement.Parse(responseString);

            string result = "";
            return result;


        }



        // Function Definition to extract list of all Paper Types
        public List<PubScreenPaperType> GetPaperTypes()
        {
            List<PubScreenPaperType> PaperTypeList = new List<PubScreenPaperType>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From PaperType"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PaperTypeList.Add(new PubScreenPaperType
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        PaperType = Convert.ToString(dr["PaperType"].ToString()),


                    });
                }
            }

            return PaperTypeList;
        }


        // Function Definition to extract list of all Cognitive Tasks
        public List<PubScreenTask> GetTasks()
        {
            List<PubScreenTask> TaskList = new List<PubScreenTask>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Task"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TaskList.Add(new PubScreenTask
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Task = Convert.ToString(dr["Task"].ToString()),


                    });
                }
            }

            return TaskList;
        }

        // Function Definition to extract list of all Species
        public List<PubScreenSpecie> GetSpecies()
        {
            List<PubScreenSpecie> SpecieList = new List<PubScreenSpecie>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Species"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SpecieList.Add(new PubScreenSpecie
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Species = Convert.ToString(dr["Species"].ToString()),


                    });
                }
            }

            return SpecieList;
        }

        // Function Definition to extract list of all Sex
        public List<PubScreenSex> GetSex()
        {
            List<PubScreenSex> SexList = new List<PubScreenSex>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Sex"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SexList.Add(new PubScreenSex
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Sex = Convert.ToString(dr["Sex"].ToString()),


                    });
                }
            }

            return SexList;
        }

        // Function Definition to extract list of all Strains
        public List<PubScreenStrain> GetStrains()
        {
            List<PubScreenStrain> StrainList = new List<PubScreenStrain>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Strain"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StrainList.Add(new PubScreenStrain
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),


                    });
                }
            }

            return StrainList;
        }


        // Function Definition to extract list of all Disease Models
        public List<PubScreenDisease> GetDisease()
        {
            List<PubScreenDisease> DiseaseList = new List<PubScreenDisease>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From DiseaseModel"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DiseaseList.Add(new PubScreenDisease
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        DiseaseModel = Convert.ToString(dr["DiseaseModel"].ToString()),


                    });
                }
            }

            return DiseaseList;
        }

        // Function Definition to extract list of all Regions & Sub-regions
        public List<PubScreenRegion> GetAllRegions()
        {
            List<PubScreenRegion> RegionList = new List<PubScreenRegion>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select SubRegion.ID, SubRegion.RID, BrainRegion.BrainRegion, SubRegion.SubRegion 
                                                             From SubRegion
                                                             Inner join BrainRegion on BrainRegion.ID = SubRegion.RID"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RegionList.Add(new PubScreenRegion
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        RID = Int32.Parse(dr["RID"].ToString()),
                        BrainRegion = Convert.ToString(dr["BrainRegion"].ToString()),
                        SubRegion = Convert.ToString(dr["SubRegion"].ToString()),


                    });
                }
            }

            return RegionList;
        }

        // Function Definition to extract list of all Regions only
        public List<PubScreenRegion> GetRegions()
        {
            List<PubScreenRegion> RegionList = new List<PubScreenRegion>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From BrainRegion"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RegionList.Add(new PubScreenRegion
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        BrainRegion = Convert.ToString(dr["BrainRegion"].ToString()),

                    });
                }
            }

            return RegionList;
        }

        // Function Definition to extract list of Celltypes 
        public List<PubScreenCellType> GetCellTypes()
        {
            List<PubScreenCellType> CelltypeList = new List<PubScreenCellType>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From CellType"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    CelltypeList.Add(new PubScreenCellType
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        CellType = Convert.ToString(dr["CellType"].ToString()),

                    });
                }
            }

            return CelltypeList;
        }


        // Function Definition to extract list of Methods 
        public List<PubScreenMethod> GetMethods()
        {
            List<PubScreenMethod> MethodList = new List<PubScreenMethod>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Method"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    MethodList.Add(new PubScreenMethod
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Method = Convert.ToString(dr["Method"].ToString()),

                    });
                }
            }

            return MethodList;
        }

        // Function Definition to extract list of Neurotransmitter 
        public List<PubScreenNeuroTransmitter> GetNeurotransmitters()
        {
            List<PubScreenNeuroTransmitter> NeuroTransmitterList = new List<PubScreenNeuroTransmitter>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From NeuroTransmitter"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    NeuroTransmitterList.Add(new PubScreenNeuroTransmitter
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString()),

                    });
                }
            }

            return NeuroTransmitterList;
        }

        // Function defintion to add a new author to database
        public int AddAuthors(PubScreenAuthor author)
        {
            string sql = $@"Insert into Author (FirstName, LastName, Affiliation) Values ('{author.FirstName}', '{author.LastName}', '{author.Affiliation}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalarPub(sql).ToString());
        }


        // Function Definition to extract list of Authors 
        public List<PubScreenAuthor> GetAuthors()
        {
            List<PubScreenAuthor> AuthorList = new List<PubScreenAuthor>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Author"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AuthorList.Add(new PubScreenAuthor
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        FirstName = Convert.ToString(dr["FirstName"].ToString()),
                        LastName = Convert.ToString(dr["LastName"].ToString()),
                        Affiliation = Convert.ToString(dr["Affiliation"].ToString()),

                    });
                }
            }

            return AuthorList;
        }
        //*************************************************************************************************************************************************************************
        // Function Definition to add a new publication to database Pubscreen
        public int AddPublications(PubScreen publication)
        {
            string sqlPublication = $@"Insert into Publication (Title, Abstract, Keywords, DOI, Year) Values
                                    ('{publication.Title}', '{publication.Abstract}', '{publication.Keywords}', '{publication.DOI}', '{publication.Year}' ); SELECT @@IDENTITY AS 'Identity'; ";

            int PublicationID = Int32.Parse(Dal.ExecScalarPub(sqlPublication).ToString());

            //Adding to Publication_Author Table
            if (publication.AuthourID != null && publication.AuthourID.Length != 0)
            {
                string sqlAuthor = "";
                for (int i = 0; i < publication.AuthourID.Length; i++)
                {
                    sqlAuthor += $@"Insert into Publication_Author (AuthorID, PublicationID) Values ({publication.AuthourID[i]}, {PublicationID});";
                }
                if (sqlAuthor != "") { Dal.ExecuteNonQueryPub(sqlAuthor); };

            }

            //Adding to Publication_PaperType Table
            string sqlPaperType = "";
            sqlPaperType = $@"Insert into Publication_PaperType (PaperTypeID, PublicationID) Values ({publication.PaperTypeID}, {PublicationID});";
            Dal.ExecuteNonQueryPub(sqlPaperType);

            //Adding to Publication_Task
            if (publication.TaskID != null && publication.TaskID.Length != 0)
            {
                string sqlTask = "";
                for (int i = 0; i < publication.TaskID.Length; i++)
                {
                    sqlTask += $@"Insert into Publication_Task (TaskID, PublicationID) Values ({publication.TaskID[i]}, {PublicationID});";

                }
                if (sqlTask != "") { Dal.ExecuteNonQueryPub(sqlTask); };

            }

            //Adding to Publication_Specie
            if (publication.SpecieID != null && publication.SpecieID.Length != 0)
            {
                string sqlSpecie = "";
                for (int i = 0; i < publication.SpecieID.Length; i++)
                {
                    sqlSpecie += $@"Insert into Publication_Specie (SpecieID, PublicationID) Values ({publication.SpecieID[i]}, {PublicationID});";

                }
                if (sqlSpecie != "") { Dal.ExecuteNonQueryPub(sqlSpecie); };

            }

            //Adding to Publication_Sex
            if (publication.sexID != null && publication.sexID.Length != 0)
            {
                string sqlSex = "";
                for (int i = 0; i < publication.sexID.Length; i++)
                {
                    sqlSex += $@"Insert into Publication_Sex (sexID, PublicationID) Values ({publication.sexID[i]}, {PublicationID});";


                }
                if (sqlSex != "") { Dal.ExecuteNonQueryPub(sqlSex); };

            }

            //Adding to Publication_Strain
            if (publication.StrainID != null && publication.StrainID.Length != 0)
            {
                string sqlStrain = "";
                for (int i = 0; i < publication.StrainID.Length; i++)
                {
                    sqlStrain += $@"Insert into Publication_Strain (StrainID, PublicationID) Values ({publication.StrainID[i]}, {PublicationID});";

                }
                if (sqlStrain != "") { Dal.ExecuteNonQueryPub(sqlStrain); };

            }

            //Adding to Publication_Disease
            if (publication.DiseaseID != null && publication.DiseaseID.Length != 0)
            {
                string sqlDiease = "";
                for (int i = 0; i < publication.DiseaseID.Length; i++)
                {
                    sqlDiease += $@"Insert into Publication_Disease (DiseaseID, PublicationID) Values ({publication.DiseaseID[i]}, {PublicationID});";

                }
                if (sqlDiease != "") { Dal.ExecuteNonQueryPub(sqlDiease); };

            }

            //Adding to Publication_Region
            if (publication.RegionID != null && publication.RegionID.Length != 0)
            {
                string sqlRegion = "";
                for (int i = 0; i < publication.RegionID.Length; i++)
                {
                    sqlRegion += $@"Insert into Publication_Region (RegionID, PublicationID) Values ({publication.RegionID[i]}, {PublicationID});";

                }
                if (sqlRegion != "") { Dal.ExecuteNonQueryPub(sqlRegion); };

            }

            //Adding to Publication_SubRegion
            if (publication.SubRegionID != null && publication.SubRegionID.Length != 0)
            {
                string sqlSubRegion = "";
                for (int i = 0; i < publication.SubRegionID.Length; i++)
                {
                    sqlSubRegion += $@"Insert into Publication_SubRegion (SubRegionID, PublicationID) Values ({publication.SubRegionID[i]}, {PublicationID});";

                }
                if (sqlSubRegion != "") { Dal.ExecuteNonQueryPub(sqlSubRegion); };

            }

            //Adding to Publication_CellType
            if (publication.CellTypeID != null && publication.CellTypeID.Length != 0)
            {
                string sqlCelltype = "";
                for (int i = 0; i < publication.CellTypeID.Length; i++)
                {
                    sqlCelltype += $@"Insert into Publication_CellType (CellTypeID, PublicationID) Values ({publication.CellTypeID[i]}, {PublicationID});";

                }
                if (sqlCelltype != "") { Dal.ExecuteNonQueryPub(sqlCelltype); };

            }

            //Adding to Publication_Method
            if (publication.MethodID != null && publication.MethodID.Length != 0)
            {
                string sqlMethod = "";
                for (int i = 0; i < publication.MethodID.Length; i++)
                {
                    sqlMethod += $@"Insert into Publication_Method (MethodID, PublicationID) Values ({publication.MethodID[i]}, {PublicationID});";

                }
                if (sqlMethod != "") { Dal.ExecuteNonQueryPub(sqlMethod); };

            }

            //Adding to Publication_NeuroTransmitter
            if (publication.TransmitterID != null && publication.TransmitterID.Length != 0)
            {
                string sqlTransmitter = "";
                for (int i = 0; i < publication.TransmitterID.Length; i++)
                {
                    sqlTransmitter += $@"Insert into Publication_NeuroTransmitter (TransmitterID, PublicationID) Values ({publication.TransmitterID[i]}, {PublicationID});";

                }
                if (sqlTransmitter != "") { Dal.ExecuteNonQueryPub(sqlTransmitter); };

            }

            return PublicationID;

        }
        //*******************************************************************************************************************************************************************
        // Function definition to search publications in database
        public List<PubScreenSearch> SearchPublications(PubScreen pubScreen)
        {
            List<PubScreenSearch> lstPubScreen = new List<PubScreenSearch>();

            string sql = "Select * From SearchPub Where ";

            if (!string.IsNullOrEmpty(pubScreen.Title))
            {
                sql += $@"SearchPub.Title = '{pubScreen.Title}' OR ";
            }

            if (!string.IsNullOrEmpty(pubScreen.Keywords))
            {
                sql += $@"SearchPub.Keywords like '%{pubScreen.Keywords}%' OR ";
            }

            if (!string.IsNullOrEmpty(pubScreen.DOI))
            {
                sql += $@"SearchPub.DOI = '{pubScreen.DOI}' OR ";
            }

            // search query for Author
            if (pubScreen.AuthourID != null && pubScreen.AuthourID.Length != 0)
            {
                for (int i = 0; i < pubScreen.AuthourID.Length; i++)
                {
                    sql += $@"SearchPub.Author like '%'  + (Select CONCAT(Author.FirstName, '-', Author.LastName) From Author Where Author.ID = {pubScreen.AuthourID[i]}) +  '%' OR ";
                }
            }

            // search query for Year
            if (pubScreen.Years.Length != 0)
            {
                for (int i = 0; i < pubScreen.Years.Length; i++)
                {
                    sql += $@"SearchPub.Year = '{pubScreen.Years[i]}' OR ";
                }
            }

            // search query for PaperType
            if (pubScreen.PaperTypeID != null)
            {

                sql += $@"SearchPub.PaperType like '%'  + (Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeID}) +  '%' OR ";
            }

            // search query for Task
            if (pubScreen.TaskID != null && pubScreen.TaskID.Length != 0)
            {
                for (int i = 0; i < pubScreen.TaskID.Length; i++)
                {
                    sql += $@"SearchPub.Task like '%'  + (Select Task From Task Where Task.ID = {pubScreen.TaskID[i]}) +  '%' OR ";
                }
            }

            // search query for Species
            if (pubScreen.SpecieID != null && pubScreen.SpecieID.Length != 0)
            {
                for (int i = 0; i < pubScreen.SpecieID.Length; i++)
                {
                    sql += $@"SearchPub.Species like '%'  + (Select Species From Species Where Species.ID = {pubScreen.SpecieID[i]}) +  '%' OR ";
                }
            }

            // search query for Sex
            if (pubScreen.sexID != null && pubScreen.sexID.Length != 0)
            {
                for (int i = 0; i < pubScreen.sexID.Length; i++)
                {
                    sql += $@"SearchPub.Sex like '%'  + (Select Sex From Sex Where Sex.ID = {pubScreen.sexID[i]}) +  '%' OR ";
                }
            }

            // search query for Strain
            if (pubScreen.StrainID != null && pubScreen.StrainID.Length != 0)
            {
                for (int i = 0; i < pubScreen.StrainID.Length; i++)
                {
                    sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[i]}) +  '%' OR ";
                }
            }

            // search query for Disease
            if (pubScreen.DiseaseID != null && pubScreen.DiseaseID.Length != 0)
            {
                for (int i = 0; i < pubScreen.DiseaseID.Length; i++)
                {
                    sql += $@"SearchPub.DiseaseModel like '%'  + (Select DiseaseModel From DiseaseModel Where DiseaseModel.ID = {pubScreen.DiseaseID[i]}) +  '%' OR ";
                }
            }

            // search query for BrainRegion
            if (pubScreen.RegionID != null && pubScreen.RegionID.Length != 0)
            {
                for (int i = 0; i < pubScreen.RegionID.Length; i++)
                {
                    sql += $@"SearchPub.BrainRegion like '%'  + (Select BrainRegion From BrainRegion Where BrainRegion.ID = {pubScreen.RegionID[i]}) +  '%' OR ";
                }
            }

            // search query for SubRegion
            if (pubScreen.SubRegionID != null && pubScreen.SubRegionID.Length != 0)
            {
                for (int i = 0; i < pubScreen.SubRegionID.Length; i++)
                {
                    sql += $@"SearchPub.SubRegion like '%'  + (Select SubRegion From SubRegion Where SubRegion.ID = {pubScreen.SubRegionID[i]}) +  '%' OR ";
                }
            }

            // search query for CellType
            if (pubScreen.CellTypeID != null && pubScreen.CellTypeID.Length != 0)
            {
                for (int i = 0; i < pubScreen.CellTypeID.Length; i++)
                {
                    sql += $@"SearchPub.CellType like '%'  + (Select CellType From CellType Where CellType.ID = {pubScreen.CellTypeID[i]}) +  '%' OR ";
                }
            }

            // search query for Method
            if (pubScreen.MethodID != null && pubScreen.MethodID.Length != 0)
            {
                for (int i = 0; i < pubScreen.MethodID.Length; i++)
                {
                    sql += $@"SearchPub.Method like '%'  + (Select Method From Method Where Method.ID = {pubScreen.MethodID[i]}) +  '%' OR ";
                }
            }

            // search query for Neuro Transmitter
            if (pubScreen.TransmitterID != null && pubScreen.TransmitterID.Length != 0)
            {
                for (int i = 0; i < pubScreen.TransmitterID.Length; i++)
                {
                    sql += $@"SearchPub.Neurotransmitter like '%'  + (Select Neurotransmitter From Neurotransmitter Where Neurotransmitter.ID = {pubScreen.TransmitterID[i]}) +  '%' OR ";
                }
            }


            sql = sql.Substring(0, sql.Length - 3);



            using (DataTable dt = Dal.GetDataTablePub(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstPubScreen.Add(new PubScreenSearch
                    {

                        Title = Convert.ToString(dr["Title"].ToString()),
                        Abstract = Convert.ToString(dr["Abstract"].ToString()),
                        Keywords = Convert.ToString(dr["Keywords"].ToString()),
                        DOI = Convert.ToString(dr["DOI"].ToString()),
                        Year = Convert.ToString(dr["Year"].ToString()),
                        Author = Convert.ToString(dr["Author"].ToString()),
                        PaperType = Convert.ToString(dr["PaperType"].ToString()),
                        Task = Convert.ToString(dr["Task"].ToString()),
                        Species = Convert.ToString(dr["Species"].ToString()),
                        Sex = Convert.ToString(dr["Sex"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),
                        DiseaseModel = Convert.ToString(dr["DiseaseModel"].ToString()),
                        BrainRegion = Convert.ToString(dr["BrainRegion"].ToString()),
                        SubRegion = Convert.ToString(dr["SubRegion"].ToString()),
                        CellType = Convert.ToString(dr["CellType"].ToString()),
                        Method = Convert.ToString(dr["Method"].ToString()),
                        NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString()),

                    });
                }

            }

            return lstPubScreen;


        }

        // Function definition to get all year's values in database
        public List<PubScreenSearch> GetAllYears()
        {
            List<PubScreenSearch> YearList = new List<PubScreenSearch>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select distinct Year from Publication"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    YearList.Add(new PubScreenSearch
                    {

                        Year = Convert.ToString(dr["Year"].ToString()),
                    });

                }
            }

            return YearList;
        }


    }
}
