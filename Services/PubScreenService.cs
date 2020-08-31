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
using MathNet.Numerics;
using System.Data.SqlClient;
using Remotion.Linq.Clauses;

namespace AngularSPAWebAPI.Services
{

    public class PubScreenService
    {
        // Function Definition to get paper info from DOI
        // private static readonly HttpClient client = new HttpClient();

        public async Task<PubScreen> GetPaperInfoByDoi(string doi)
        {
            // Submit doi to get the pubmedkey
            if (doi.ToLower().Contains("https://doi.org/"))
            {
                doi = doi.Replace("https://doi.org/", "", StringComparison.OrdinalIgnoreCase);
            }

            HttpClient httpClient = new HttpClient();

            StringContent content = new System.Net.Http.StringContent(String.Empty);

            var response = await httpClient.PostAsync("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&WebEnv=1&usehistory=y&term=" + doi + "&rettype=Id", content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString.ToLower().IndexOf("<OutputMessage>No items found.</OutputMessage>".ToLower()) > -1)
            {
                return null;
            }

            XElement incomingXml = XElement.Parse(responseString);
            string pubMedKey = incomingXml.Element("IdList").Element("Id").Value;

            //If pubmedkey is available, Send it to another function to get Paper's info
            if (!string.IsNullOrEmpty(pubMedKey))
            {
                return await GetPaperInfoByPubMedKey(pubMedKey);
            }

            return null;

        }

        //Function Definition to get some paper's info based on PubMedKey
        public async Task<PubScreen> GetPaperInfoByPubMedKey(string pubMedKey)
        {
            // if IskeyPumbed is true then get doi and add it to object
            HttpClient httpClient = new HttpClient();

            var content = new StringContent(String.Empty, Encoding.UTF8, "application/xml");

            var response = await httpClient.PostAsync("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=" + pubMedKey + "&retmode=xml", content);
            var responseString = await response.Content.ReadAsStringAsync();
            XElement incomingXml = XElement.Parse(responseString);

            string articleAbstract = "";
            string articleYear = "";

            // journal name
            string articleName = "";
            if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("Journal").Element("Title") != null)
            {
                articleName = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("Journal").Element("Title").Value;
            }

            //title
            string articleTitle = "";
            if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ArticleTitle") != null)
            {
                articleTitle = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ArticleTitle").Value;
            }

            //abstract
            if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("Abstract") != null)
            {
                articleAbstract = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("Abstract").Element("AbstractText").Value;
            }

            //articletype
            string articleType = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("PublicationTypeList").Element("PublicationType").Value;

            //year
            if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ArticleDate") != null)
            {
                articleYear = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ArticleDate").Element("Year").Value;
            }
            else if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("Journal").Element("JournalIssue").Element("PubDate") != null)
            {
                articleYear = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("Journal").Element("JournalIssue").Element("PubDate").Element("Year").Value;

            }

            //string keywords = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("KeywordList").Value;
            XmlDocument xml = new XmlDocument();

            // Extracting list of keywords
            string keyWordString = "";

            if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("KeywordList") != null)
            {
                string xmlStringKeywords = Convert.ToString(incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("KeywordList"));
                xml.LoadXml(xmlStringKeywords);
                XmlNodeList keyList = xml.SelectNodes("/KeywordList/Keyword");
                List<string> keyWordList = new List<string>();
                foreach (XmlNode keyword in keyList)
                {
                    keyWordList.Add(keyword.InnerText);
                }

                keyWordString = string.Join(", ", keyWordList);
            }

            // Extracting list of Authors and add them to author object list
            List<PubScreenAuthor> authorList = new List<PubScreenAuthor>();
            List<string> authorListString = new List<string>();
            string xmlStringAuthor = Convert.ToString(incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("AuthorList"));
            xml.LoadXml(xmlStringAuthor);
            XmlNodeList xnList = xml.SelectNodes("/AuthorList/Author");

            foreach (XmlNode xn in xnList)
            {
                if(xn["ForeName"] !=null && xn["LastName"]!=null)
                {
                    authorListString.Add(xn["ForeName"].InnerText + "-" + xn["LastName"].InnerText);

                    authorList.Add(new PubScreenAuthor
                    {
                        FirstName = xn["ForeName"].InnerText,
                        LastName = xn["LastName"].InnerText,
                        Affiliation = xn["AffiliationInfo"] == null ? "" : (xn["AffiliationInfo"].InnerText).Split(',')[0],

                    });
                }
            }

            //doi
            string doi = "";
            if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ELocationID") != null)
            {
                doi = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ELocationID").Value;
            }




            string authorString = string.Join(", ", authorListString);

            // initialize PubScreenSearch object and fill it
            var pubscreenObj = new PubScreen
            {
                Title = articleTitle,
                Abstract = articleAbstract,
                Keywords = keyWordString,
                Year = articleYear,
                AuthorString = authorString,
                PaperType = articleType,
                Author = authorList,
                Reference = articleName,
                DOI = doi,


            };

            // Actually two outputs should be returned (authorList should be also returned)

            return pubscreenObj;


        }

        //Function Definition to get some paper's info based on DOI from BioRxiv
        public async Task<PubScreen> GetPaperInfoByDOIBIO(string doi)
        {
            // Submiy doi to get the pubmedkey

            HttpClient httpClient = new HttpClient();

            StringContent content = new System.Net.Http.StringContent(String.Empty);

            var response = await httpClient.PostAsync("https://api.biorxiv.org/details/biorxiv/" + doi + "/json", content);
            var responseString = await response.Content.ReadAsStringAsync();

            JsonPubscreen jsonPubscreen = JsonConvert.DeserializeObject<JsonPubscreen>(responseString);

            if (jsonPubscreen.collection == null || jsonPubscreen.collection.Length == 0)
            {
                return null;
            }

            List<string> authorTempList = (jsonPubscreen.collection[0].authors).Split(';').ToList<string>();
            List<string> authorListString = new List<string>();
            List<PubScreenAuthor> authorList = new List<PubScreenAuthor>();
            foreach (var name in authorTempList)
            {
                authorListString.Add(name.Split(',')[1] + '-' + name.Split(',')[0]);

                authorList.Add(new PubScreenAuthor
                {
                    FirstName = name.Split(',')[1],
                    LastName = name.Split(',')[0],

                });

            }

            string authorString = string.Join(", ", authorListString);

            var pubscreenObj = new PubScreen
            {
                Title = jsonPubscreen.collection[0].title,
                Abstract = jsonPubscreen.collection[0].@abstract,
                Year = (jsonPubscreen.collection[jsonPubscreen.collection.Count() - 1].date).Split('-')[0],
                AuthorString = authorString,
                PaperType = jsonPubscreen.collection[0].@type,
                Author = authorList,
                Reference = "bioRxiv",
                DOI = doi,

            };

            return pubscreenObj;

        }

        // Function Definition to extract list of all Paper Types
        public List<PubScreenPaperType> GetPaperTypes()
        {
            List<PubScreenPaperType> PaperTypeList = new List<PubScreenPaperType>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select distinct(PaperType) From PaperType"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PaperTypeList.Add(new PubScreenPaperType
                    {
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

        // Delete publication
        public void DeletePublicationById(int pubId)
        {
            string sql = $@" 
                             Delete From Publication_Author Where PublicationID = {pubId};
                             Delete From Publication_CellType Where PublicationID = {pubId};
                             Delete From Publication_Disease Where PublicationID = {pubId};
                             Delete From Publication_Method Where PublicationID = {pubId};
                             Delete From Publication_NeuroTransmitter Where PublicationID = {pubId};
                             Delete From Publication_PaperType Where PublicationID = {pubId};
                             Delete From Publication_Region Where PublicationID = {pubId};
                             Delete From Publication_Sex Where PublicationID = {pubId};
                             Delete From Publication_Specie Where PublicationID = {pubId};
                             Delete From Publication_Strain Where PublicationID = {pubId};
                             Delete From Publication_SubRegion Where PublicationID = {pubId};
                             Delete From Publication_Task Where PublicationID = {pubId};
                             Delete From Publication Where id = { pubId};";

            Dal.ExecuteNonQueryPub(sql);
        }

        //************************************************************************************Adding Publication*************************************************************************************
        // Function Definition to add a new publication to database Pubscreen
        public int? AddPublications(PubScreen publication, string Username)
        {
            // Check for duplication based on the DOI
            string sqlDOI = $@"Select ID From Publication Where DOI = '{publication.DOI}'";
            object ID = Dal.ExecScalarPub(sqlDOI);
            if (ID != null)
            {
                return null;
            }


            string sqlPublication = $@"Insert into Publication (Title, Abstract, Keywords, DOI, Year, Reference, Username, Source) Values
                                    ('{HelperService.EscapeSql((HelperService.NullToString(publication.Title)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(publication.Abstract)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(publication.Keywords)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(publication.DOI)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(publication.Year)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(publication.Reference)))}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(Username)))}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(publication.Source)))}'

                                      ); SELECT @@IDENTITY AS 'Identity'; ";

            int PublicationID = Int32.Parse(Dal.ExecScalarPub(sqlPublication).ToString());

            // Adding Author **********************************************************************************************************************
            //Adding to Publication_Author Table if Author ID is not null or empty (it happens when DOI or pubmedID is not available)
            if (publication.AuthourID != null && publication.AuthourID.Length != 0)
            {
                string sqlAuthor = "";
                for (int i = 0; i < publication.AuthourID.Length; i++)
                {
                    sqlAuthor += $@"Insert into Publication_Author (AuthorID, PublicationID) Values ({publication.AuthourID[i]}, {PublicationID});";
                }
                if (sqlAuthor != "") { Dal.ExecuteNonQueryPub(sqlAuthor); };

            }

            //When pubmedID or DOI is avaialble, add Authors to Publication_Author Table and also to "Author" table if the author in publication.Author does not already exist in Author table in DB
            if (publication.Author != null && publication.Author.Count() != 0)

            {
                // Get list of all autohrs from DB in the following format "firstname-lastname"
                List<string> allAuthorList = new List<string>();
                using (DataTable dt = Dal.GetDataTablePub($@"Select * From Author"))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        allAuthorList.Add((Convert.ToString(dr["FirstName"].ToString())).ToLower() + '-' + (Convert.ToString(dr["LastName"].ToString()).ToLower()));

                    }
                }

                // loop through  publication.Author if author does not exist in allAuthorList then add it to DB
                string sqlAuthor = "";
                for (int i = 0; i < publication.Author.Count(); i++)
                {

                    if (!allAuthorList.Contains((publication.Author[i].FirstName).ToLower() + '-' + (publication.Author[i].LastName).ToLower()))
                    {
                        sqlAuthor += $@"Insert into Author (FirstName, LastName, Affiliation) Values ('{publication.Author[i].FirstName}',
                                                                                                      '{publication.Author[i].LastName}',
                                                                                                      '{publication.Author[i].Affiliation}');";
                    }

                }

                if (sqlAuthor != "") { Dal.ExecuteNonQueryPub(sqlAuthor).ToString(); };

                //Add all authors to publication-author table in DB
                List<string> AuthorList = publication.AuthorString.Split(',').ToList();
                int? authorID = 0;
                string sqlauthor2 = "";
                string sqlauthor3 = "";
                int j = 1;
                foreach (string author in AuthorList)
                {
                    sqlauthor2 = $@"Select ID From Author Where CONCAT(LOWER(Author.FirstName), '-', LOWER(Author.LastName))= '{author.Trim()}';";
                    authorID = Int32.Parse((Dal.ExecScalarPub(sqlauthor2).ToString()));

                    sqlauthor3 = $@"Insert into Publication_Author (AuthorID, PublicationID, AuthorOrder) Values ({authorID}, {PublicationID}, {j});";
                    Dal.ExecuteNonQueryPub(sqlauthor3).ToString();
                    j++;
                }



            } // End of if statement when DOI OR Pubmed is available


            //Adding to Publication_PaperType Table**********************************************************************************************

            // When DOI or Pubmedkey is not available
            if (publication.PaperTypeID != null)
            {
                string sqlPaperType = "";
                sqlPaperType = $@"Insert into Publication_PaperType (PaperTypeID, PublicationID) Values ({publication.PaperTypeID}, {PublicationID});";
                Dal.ExecuteNonQueryPub(sqlPaperType);
            }

            // When DOI or Pubmedkey is available
            if (publication.PaperType != null && publication.PaperType.Length != 0)
            {
                // check to see if papertype exist in DB, if so just insert it into Publication_PaperType; otherwise, insert it into both PrperType and Publication_PaperType tables in DB.

                //Get list of all paperType form DB
                List<string> allPTList = new List<string>();
                using (DataTable dt = Dal.GetDataTablePub($@"Select PaperType From PaperType"))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        allPTList.Add((Convert.ToString(dr["PaperType"].ToString())).ToLower());

                    }
                }

                // if paper type is new and is not available in DB, insert it to DB
                if (!allPTList.Contains(publication.PaperType))
                {
                    // Insert into paper type table in DB
                    string sqlPT = $@"Insert into PaperType (PaperType) Values ('{publication.PaperType}');";
                    if (sqlPT != "") { Dal.ExecuteNonQueryPub(sqlPT); };
                }

                // Get the ID of new or existing paperType
                string sqlPT2 = $@"Select ID from PaperType Where PaperType = '{publication.PaperType}'";
                int PaperTypeID = Int32.Parse(Dal.ExecScalarPub(sqlPT2).ToString());

                //Insert paperTypeID itno Publication_PaperType tbl in DB
                string sqlPT3 = $@"Insert into Publication_PaperType (PaperTypeID, PublicationID) Values ({PaperTypeID}, {PublicationID});";
                Dal.ExecuteNonQueryPub(sqlPT3);

            }


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

        // Edit publication
        public bool EditPublication(int publicationId, PubScreen publication, string Username)
        {


            string sqlPublication = $@"Update Publication set Title = @title, Abstract = @abstract, Keywords = @keywords,
                                                               DOI = @doi, Year = @year where id = {publicationId}";

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@title", HelperService.NullToString(HelperService.EscapeSql(publication.Title)).Trim()));
            parameters.Add(new SqlParameter("@abstract", HelperService.NullToString(HelperService.EscapeSql(publication.Abstract)).Trim()));
            parameters.Add(new SqlParameter("@keywords", HelperService.NullToString(HelperService.EscapeSql(publication.Keywords)).Trim()));
            parameters.Add(new SqlParameter("@doi", HelperService.NullToString(HelperService.EscapeSql(publication.DOI)).Trim()));
            parameters.Add(new SqlParameter("@year", HelperService.NullToString(HelperService.EscapeSql(publication.Year)).Trim()));

            Int32.Parse(Dal.ExecuteNonQueryPub(CommandType.Text, sqlPublication, parameters.ToArray()).ToString());

            string sqlDelete = "";

            // Editing Author **********************************************************************************************************************
            // Edit Publication_Author Table 
            if (publication.AuthourID != null && publication.AuthourID.Length != 0)
            {
                string sqlAuthor = "";

                sqlDelete = $"DELETE From Publication_Author where PublicationID = {publicationId}";
                Dal.ExecuteNonQueryPub(sqlDelete);

                for (int i = 0; i < publication.AuthourID.Length; i++)
                {
                    sqlAuthor += $@"Insert into Publication_Author (AuthorID, PublicationID) Values ({publication.AuthourID[i]}, {publicationId});";
                }

                if (sqlAuthor != "") { Dal.ExecuteNonQueryPub(sqlAuthor); };

            }

            //Edit Publication_PaperType Table**********************************************************************************************

            // When DOI or Pubmedkey is not available
            var paperTypeId = 0;
            if (!string.IsNullOrEmpty(publication.PaperType))
            {
                var sql = $"select id from PaperType where ltrim(rtrim(papertype)) = '{HelperService.NullToString(HelperService.EscapeSql(publication.PaperType)).Trim()}'";
                var retPaperType = Dal.ExecScalarPub(sql);
                if (retPaperType != null)
                {
                    paperTypeId = Int32.Parse(Dal.ExecScalarPub(sql).ToString());

                }
            }

            if (paperTypeId != 0)
            {
                sqlDelete = $"DELETE From Publication_PaperType where PublicationID = {publicationId}";
                Dal.ExecuteNonQueryPub(sqlDelete);

                string sqlPaperType = "";
                sqlPaperType = $@"Insert into Publication_PaperType (PaperTypeID, PublicationID) Values ({paperTypeId}, {publicationId});";
                Dal.ExecuteNonQueryPub(sqlPaperType);
            }

            //Editing to Publication_Task
            if (publication.TaskID != null && publication.TaskID.Length != 0)
            {
                sqlDelete = $"DELETE From Publication_Task where PublicationID = {publicationId}";
                Dal.ExecuteNonQueryPub(sqlDelete);

                string sqlTask = "";
                for (int i = 0; i < publication.TaskID.Length; i++)
                {
                    sqlTask += $@"Insert into Publication_Task (TaskID, PublicationID) Values ({publication.TaskID[i]}, {publicationId});";

                }
                if (sqlTask != "") { Dal.ExecuteNonQueryPub(sqlTask); };

            }

            //Editing to Publication_Specie
            sqlDelete = $"DELETE From Publication_Specie where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.SpecieID != null && publication.SpecieID.Length != 0)
            {
                string sqlSpecie = "";
                for (int i = 0; i < publication.SpecieID.Length; i++)
                {
                    sqlSpecie += $@"Insert into Publication_Specie (SpecieID, PublicationID) Values ({publication.SpecieID[i]}, {publicationId});";

                }
                if (sqlSpecie != "") { Dal.ExecuteNonQueryPub(sqlSpecie); };
            }

            //Editing to Publication_Sex
            sqlDelete = $"DELETE From Publication_Sex where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.sexID != null && publication.sexID.Length != 0)
            {
                string sqlSex = "";
                for (int i = 0; i < publication.sexID.Length; i++)
                {
                    sqlSex += $@"Insert into Publication_Sex (sexID, PublicationID) Values ({publication.sexID[i]}, {publicationId});";


                }
                if (sqlSex != "") { Dal.ExecuteNonQueryPub(sqlSex); };

            }

            //Editing to Publication_Strain
            sqlDelete = $"DELETE From Publication_Strain where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.StrainID != null && publication.StrainID.Length != 0)
            {
                string sqlStrain = "";
                for (int i = 0; i < publication.StrainID.Length; i++)
                {
                    sqlStrain += $@"Insert into Publication_Strain (StrainID, PublicationID) Values ({publication.StrainID[i]}, {publicationId});";

                }
                if (sqlStrain != "") { Dal.ExecuteNonQueryPub(sqlStrain); };

            }

            //Editing to Publication_Disease
            sqlDelete = $"DELETE From Publication_Disease where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.DiseaseID != null && publication.DiseaseID.Length != 0)
            {
                string sqlDiease = "";
                for (int i = 0; i < publication.DiseaseID.Length; i++)
                {
                    sqlDiease += $@"Insert into Publication_Disease (DiseaseID, PublicationID) Values ({publication.DiseaseID[i]}, {publicationId});";

                }
                if (sqlDiease != "") { Dal.ExecuteNonQueryPub(sqlDiease); };

            }

            //Editing to Publication_Region
            sqlDelete = $"DELETE From Publication_Region where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.RegionID != null && publication.RegionID.Length != 0)
            {
                string sqlRegion = "";
                for (int i = 0; i < publication.RegionID.Length; i++)
                {
                    sqlRegion += $@"Insert into Publication_Region (RegionID, PublicationID) Values ({publication.RegionID[i]}, {publicationId});";

                }
                if (sqlRegion != "") { Dal.ExecuteNonQueryPub(sqlRegion); };

            }

            //Editing to Publication_SubRegion
            sqlDelete = $"DELETE From Publication_SubRegion where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.SubRegionID != null && publication.SubRegionID.Length != 0)
            {
                string sqlSubRegion = "";
                for (int i = 0; i < publication.SubRegionID.Length; i++)
                {
                    sqlSubRegion += $@"Insert into Publication_SubRegion (SubRegionID, PublicationID) Values ({publication.SubRegionID[i]}, {publicationId});";

                }
                if (sqlSubRegion != "") { Dal.ExecuteNonQueryPub(sqlSubRegion); };

            }

            //Editing to Publication_CellType
            sqlDelete = $"DELETE From Publication_CellType where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.CellTypeID != null && publication.CellTypeID.Length != 0)
            {
                string sqlCelltype = "";
                for (int i = 0; i < publication.CellTypeID.Length; i++)
                {
                    sqlCelltype += $@"Insert into Publication_CellType (CellTypeID, PublicationID) Values ({publication.CellTypeID[i]}, {publicationId});";

                }
                if (sqlCelltype != "") { Dal.ExecuteNonQueryPub(sqlCelltype); };

            }

            //Editing to Publication_Method
            sqlDelete = $"DELETE From Publication_Method where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.MethodID != null && publication.MethodID.Length != 0)
            {
                string sqlMethod = "";
                for (int i = 0; i < publication.MethodID.Length; i++)
                {
                    sqlMethod += $@"Insert into Publication_Method (MethodID, PublicationID) Values ({publication.MethodID[i]}, {publicationId});";

                }
                if (sqlMethod != "") { Dal.ExecuteNonQueryPub(sqlMethod); };

            }

            //Editing to Publication_NeuroTransmitter
            sqlDelete = $"DELETE From Publication_NeuroTransmitter where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.TransmitterID != null && publication.TransmitterID.Length != 0)
            {
                string sqlTransmitter = "";
                for (int i = 0; i < publication.TransmitterID.Length; i++)
                {
                    sqlTransmitter += $@"Insert into Publication_NeuroTransmitter (TransmitterID, PublicationID) Values ({publication.TransmitterID[i]}, {publicationId});";

                }
                if (sqlTransmitter != "") { Dal.ExecuteNonQueryPub(sqlTransmitter); };

            }

            return true;

        }

        // Function definition to search publications in database
        public List<PubScreenSearch> SearchPublications(PubScreen pubScreen)
        {
            List<PubScreenSearch> lstPubScreen = new List<PubScreenSearch>();

            string sql = "Select * From SearchPub Where ";

            if (!string.IsNullOrEmpty(pubScreen.Title))
            {
                sql += $@"SearchPub.Title like '%{HelperService.EscapeSql(pubScreen.Title)}%' AND ";
            }

            if (!string.IsNullOrEmpty(pubScreen.Keywords))
            {
                sql += $@"SearchPub.Keywords like '%{HelperService.EscapeSql(pubScreen.Keywords)}%' AND ";
            }

            if (!string.IsNullOrEmpty(pubScreen.DOI))
            {
                sql += $@"SearchPub.DOI = '{HelperService.EscapeSql(pubScreen.DOI)}' AND ";
            }



            // search query for Author
            if (pubScreen.AuthourID != null && pubScreen.AuthourID.Length != 0)
            {
                if (pubScreen.AuthourID.Length == 1)
                {
                    sql += $@"SearchPub.Author like '%'  + (Select CONCAT(Author.FirstName, '-', Author.LastName) From Author Where Author.ID = {pubScreen.AuthourID[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.AuthourID.Length; i++)
                    {
                        sql += $@"SearchPub.Author like '%'  + (Select CONCAT(Author.FirstName, '-', Author.LastName) From Author Where Author.ID = {pubScreen.AuthourID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }

            }

            // search query for Year
            if (pubScreen.YearID != null && pubScreen.YearID.Length != 0)
            {
                for (int i = 0; i < pubScreen.YearID.Length; i++)
                {
                    sql += $@"SearchPub.Year = '{pubScreen.YearID[i]}' AND ";
                }
            }

            // search query for PaperType
            if (pubScreen.PaperTypeID != null)
            {
                sql += $@"SearchPub.PaperType like '%'  + (Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeID}) +  '%' AND ";
            }

            // search query for Task
            if (pubScreen.TaskID != null && pubScreen.TaskID.Length != 0)
            {

                if (pubScreen.TaskID.Length == 1)
                {
                    sql += $@"SearchPub.Task like '%'  + (Select Task From Task Where Task.ID = {pubScreen.TaskID[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.TaskID.Length; i++)
                    {
                        sql += $@"SearchPub.Task like '%'  + (Select Task From Task Where Task.ID = {pubScreen.TaskID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }

            }

            // search query for Species
            if (pubScreen.SpecieID != null && pubScreen.SpecieID.Length != 0)
            {
                if (pubScreen.SpecieID.Length == 1)
                {
                    sql += $@"SearchPub.Species like '%'  + (Select Species From Species Where Species.ID = {pubScreen.SpecieID[0]}) +  '%' AND ";

                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.SpecieID.Length; i++)
                    {
                        sql += $@"SearchPub.Species like '%'  + (Select Species From Species Where Species.ID = {pubScreen.SpecieID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }
            }

            // search query for Sex
            if (pubScreen.sexID != null && pubScreen.sexID.Length != 0)
            {
                if (pubScreen.sexID.Length == 1)
                {
                    sql += $@"SearchPub.Sex like '%'  + (Select Sex From Sex Where Sex.ID = {pubScreen.sexID[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.sexID.Length; i++)
                    {
                        sql += $@"SearchPub.Sex like '%'  + (Select Sex From Sex Where Sex.ID = {pubScreen.sexID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }
            }

            // search query for Strain
            if (pubScreen.StrainID != null && pubScreen.StrainID.Length != 0)
            {
                if (pubScreen.StrainID.Length == 1)
                {
                    sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.StrainID.Length; i++)
                    {
                        sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }
            }

            // search query for Disease
            if (pubScreen.DiseaseID != null && pubScreen.DiseaseID.Length != 0)
            {
                if (pubScreen.DiseaseID.Length == 1)
                {
                    sql += $@"SearchPub.DiseaseModel like '%'  + (Select DiseaseModel From DiseaseModel Where DiseaseModel.ID = {pubScreen.DiseaseID[0]}) +  '%' AND ";

                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.DiseaseID.Length; i++)
                    {
                        sql += $@"SearchPub.DiseaseModel like '%'  + (Select DiseaseModel From DiseaseModel Where DiseaseModel.ID = {pubScreen.DiseaseID[i]}) +  '%' OR ";

                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }
            }

            // search query for BrainRegion
            if (pubScreen.RegionID != null && pubScreen.RegionID.Length != 0)
            {
                if (pubScreen.RegionID.Length == 1)
                {
                    sql += $@"SearchPub.BrainRegion like '%'  + (Select BrainRegion From BrainRegion Where BrainRegion.ID = {pubScreen.RegionID[0]}) +  '%' AND ";

                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.RegionID.Length; i++)
                    {
                        sql += $@"SearchPub.BrainRegion like '%'  + (Select BrainRegion From BrainRegion Where BrainRegion.ID = {pubScreen.RegionID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }
            }

            // search query for SubRegion
            if (pubScreen.SubRegionID != null && pubScreen.SubRegionID.Length != 0)
            {
                if (pubScreen.SubRegionID.Length == 1)
                {
                    sql += $@"SearchPub.SubRegion like '%'  + (Select SubRegion From SubRegion Where SubRegion.ID = {pubScreen.SubRegionID[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.SubRegionID.Length; i++)
                    {
                        sql += $@"SearchPub.SubRegion like '%'  + (Select SubRegion From SubRegion Where SubRegion.ID = {pubScreen.SubRegionID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }

            }

            // search query for CellType
            if (pubScreen.CellTypeID != null && pubScreen.CellTypeID.Length != 0)
            {
                if (pubScreen.CellTypeID.Length == 1)
                {
                    sql += $@"SearchPub.CellType like '%'  + (Select CellType From CellType Where CellType.ID = {pubScreen.CellTypeID[0]}) +  '%' AND ";

                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.CellTypeID.Length; i++)
                    {
                        sql += $@"SearchPub.CellType like '%'  + (Select CellType From CellType Where CellType.ID = {pubScreen.CellTypeID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }
            }

            // search query for Method
            if (pubScreen.MethodID != null && pubScreen.MethodID.Length != 0)
            {
                if (pubScreen.MethodID.Length == 1)
                {
                    sql += $@"SearchPub.Method like '%'  + (Select Method From Method Where Method.ID = {pubScreen.MethodID[0]}) +  '%' AND ";
                }

                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.MethodID.Length; i++)
                    {
                        sql += $@"SearchPub.Method like '%'  + (Select Method From Method Where Method.ID = {pubScreen.MethodID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }
            }
            // search query for Neuro Transmitter
            if (pubScreen.TransmitterID != null && pubScreen.TransmitterID.Length != 0)
            {
                if (pubScreen.TransmitterID.Length == 1)
                {
                    sql += $@"SearchPub.Neurotransmitter like '%'  + (Select Neurotransmitter From Neurotransmitter Where Neurotransmitter.ID = {pubScreen.TransmitterID[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.TransmitterID.Length; i++)
                    {
                        sql += $@"SearchPub.Neurotransmitter like '%'  + (Select Neurotransmitter From Neurotransmitter Where Neurotransmitter.ID = {pubScreen.TransmitterID[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }

            }

            sql = sql.Substring(0, sql.Length - 4); // to remvoe the last NAD from the query
            string sqlMB = "";
            List<Experiment> lstExperiment = new List<Experiment>();
            using (DataTable dt = Dal.GetDataTablePub(sql))
            {

                foreach (DataRow dr in dt.Rows)
                {
                    sqlMB = $@"Select Experiment.*, Task.Name as TaskName From Experiment
                               Inner join Task on Task.ID = Experiment.TaskID
                               Where DOI = '{Convert.ToString(dr["DOI"].ToString())}'";

                    // empty lstExperiment list
                    lstExperiment.Clear();
                    using (DataTable dtExp = Dal.GetDataTable(sqlMB))
                    {
                        foreach (DataRow drExp in dtExp.Rows)
                        {

                            lstExperiment.Add(new Experiment
                            {
                                ExpID = Int32.Parse(drExp["ExpID"].ToString()),
                                ExpName = Convert.ToString(drExp["ExpName"].ToString()),
                                StartExpDate = Convert.ToDateTime(drExp["StartExpDate"].ToString()),
                                TaskName = Convert.ToString(drExp["TaskName"].ToString()),
                                DOI = Convert.ToString(drExp["DOI"].ToString()),
                                Status = Convert.ToBoolean(drExp["Status"]),
                                TaskBattery = Convert.ToString(drExp["TaskBattery"].ToString()),

                            });
                        }

                    }

                    lstPubScreen.Add(new PubScreenSearch
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Title = Convert.ToString(dr["Title"].ToString()),
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
                        Reference = Convert.ToString(dr["Reference"].ToString()),
                        Experiment = new List<Experiment>(lstExperiment),


                    });
                    //lstExperiment.Clear();
                }

            }

            // search MouseBytes database to see if the dataset exists********************************************


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

        public PubScreen GetPaperInfoByID(int id)
        {
            var pubScreen = new PubScreen();

            string sql = $"Select AuthorID From Publication_Author Where PublicationID ={id}";
            pubScreen.AuthourID = FillPubScreenItemArray(sql, "AuthorID");

            sql = $"Select CelltypeID From Publication_CellType Where PublicationID ={id}";
            pubScreen.CellTypeID = FillPubScreenItemArray(sql, "CelltypeID");

            sql = $"Select DiseaseID From Publication_Disease Where PublicationID ={id}";
            pubScreen.DiseaseID = FillPubScreenItemArray(sql, "DiseaseID");

            sql = $"Select MethodID From Publication_Method Where PublicationID ={id}";
            pubScreen.MethodID = FillPubScreenItemArray(sql, "MethodID");

            sql = $"Select TransmitterID From Publication_NeuroTransmitter Where PublicationID ={id}";
            pubScreen.TransmitterID = FillPubScreenItemArray(sql, "TransmitterID");

            sql = $"Select RegionID From Publication_Region Where PublicationID ={id}";
            pubScreen.RegionID = FillPubScreenItemArray(sql, "RegionID");

            sql = $"Select SexID From Publication_Sex Where PublicationID ={id}";
            pubScreen.sexID = FillPubScreenItemArray(sql, "SexID");

            sql = $"Select SpecieID From Publication_Specie Where PublicationID ={id}";
            pubScreen.SpecieID = FillPubScreenItemArray(sql, "SpecieID");

            sql = $"Select StrainID From Publication_Strain Where PublicationID ={id}";
            pubScreen.StrainID = FillPubScreenItemArray(sql, "StrainID");

            sql = $"Select SubRegionID From Publication_SubRegion Where PublicationID ={id}";
            pubScreen.SubRegionID = FillPubScreenItemArray(sql, "SubRegionID");

            sql = $"Select TaskID From Publication_Task Where PublicationID ={id}";
            pubScreen.TaskID = FillPubScreenItemArray(sql, "TaskID");

            //sql = $"Select PaperTypeID From Publication_PaperType Where PublicationID ={id}";
            sql = $"Select top 1 pt.PaperType From Publication_PaperType ppt inner join PaperType pt on ppt.PaperTypeID = pt.ID where ppt.PublicationID = {id}";
            var paperTypeVal = Dal.ExecScalarPub(sql);
            pubScreen.PaperType = paperTypeVal == null ? "" : paperTypeVal.ToString();

            sql = $"Select * From Publication Where ID ={id}";
            using (DataTable dt = Dal.GetDataTablePub(sql))
            {
                pubScreen.DOI = dt.Rows[0]["DOI"].ToString();
                pubScreen.Keywords = dt.Rows[0]["Keywords"].ToString();
                pubScreen.Title = dt.Rows[0]["Title"].ToString();
                pubScreen.Abstract = dt.Rows[0]["Abstract"].ToString();
                pubScreen.Year = dt.Rows[0]["Year"].ToString();
                pubScreen.Reference = dt.Rows[0]["Reference"].ToString();
                pubScreen.Source = dt.Rows[0]["Source"].ToString();
            }

            return pubScreen;
        }

        private int?[] FillPubScreenItemArray(string sql, string fieldName)
        {

            var retVal = new int?[0];
            using (DataTable dt = Dal.GetDataTablePub(sql))
            {
                retVal = new int?[dt.Rows.Count];
                var i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    retVal[i] = Int32.Parse(dr[fieldName].ToString());
                    i++;
                }
            }

            return retVal;
        }


    }
}
