using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
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
using System.Reflection;


namespace AngularSPAWebAPI.Services
{

    public class PubScreenService
    {
        const int MOUSEID = 2;
        const int RATID = 1;

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

            string responseString = "";
            var incomingXml = new XElement("newXML");
            bool isSuccess = false;
            while (!isSuccess)
            {
                try
                {
                    var response = await httpClient.PostAsync("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&WebEnv=1&usehistory=y&term=" + doi + "&rettype=Id", content);
                    responseString = await response.Content.ReadAsStringAsync();
                    if ((responseString.ToLower().IndexOf("<OutputMessage>No items found.</OutputMessage>".ToLower()) > -1) ||
                        (responseString.ToLower().IndexOf("<ErrorList>".ToLower()) > -1))
                    {
                        return null;
                    }
                    incomingXml = XElement.Parse(responseString);
                    isSuccess = true;
                }
                catch
                {
                    Console.WriteLine(responseString);
                    if (responseString.IndexOf("API rate limit exceeded") == -1)
                    {
                        Console.WriteLine("Unknown Error in GetPaperInfoByDoi!");
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(300);
                    }
                }
            }

            string pubMedKey = null;
            if (incomingXml.Element("IdList").Element("Id") != null)
            {
                pubMedKey = incomingXml.Element("IdList").Element("Id").Value;
            }

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
            var incomingXml = new XElement("newXML");
            string responseString = "";

            // Pubmed API is rate-limited. If the rate limit is exceeded, the response string from the HTTP Post will indicate that.
            // When that is the case, we wait 0.3s and attempt again, until we obtain our desired result or another error occurs
            bool isSuccess = false;
            while (!isSuccess)
            {
                try
                {
                    var response = await httpClient.PostAsync("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=" + pubMedKey + "&retmode=xml", content);
                    responseString = await response.Content.ReadAsStringAsync();
                    incomingXml = XElement.Parse(responseString);
                    isSuccess = true;
                }
                catch
                {
                    Console.WriteLine(responseString);
                    if (responseString.IndexOf("API rate limit exceeded") == -1)
                    {
                        Console.WriteLine("Unknown Error in GetPaperInfoByPubMedKey!");
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(300);
                    }
                }
            }

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
            //string articleType = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("PublicationTypeList").Element("PublicationType").Value;

            //year
            if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ArticleDate") != null)
            {
                articleYear = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("ArticleDate").Element("Year").Value;
            }
            else if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("Journal").Element("JournalIssue").Element("PubDate").Element("Year") != null)
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
                if (xn["ForeName"] != null && xn["LastName"] != null)
                {
                    authorListString.Add(xn["ForeName"].InnerText.Trim() + "-" + xn["LastName"].InnerText.Trim());

                    authorList.Add(new PubScreenAuthor
                    {
                        FirstName = xn["ForeName"].InnerText.Trim(),
                        LastName = xn["LastName"].InnerText.Trim(),
                        Affiliation = xn["AffiliationInfo"] == null ? "" : (xn["AffiliationInfo"].InnerText).Split(',')[0],

                    });
                }
            }

            //doi
            string doi = "";
            try
            {
                if (incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Elements("ELocationID").Where(id => id.Attribute("EIdType").Value == "doi").Any())
                {
                    doi = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Elements("ELocationID").Where(id => id.Attribute("EIdType").Value == "doi").FirstOrDefault().Value;
                }
            }
            catch (ArgumentNullException)
            {

            }

            try
            {
                if (String.IsNullOrEmpty(doi))
                {
                    if (incomingXml.Element("PubmedArticle").Element("PubmedData").Element("ArticleIdList").Elements("ArticleId").Where(id => id.Attribute("IdType").Value == "doi").Any())
                    {
                        doi = incomingXml.Element("PubmedArticle").Element("PubmedData").Element("ArticleIdList").Elements("ArticleId").Where(id => id.Attribute("IdType").Value == "doi").FirstOrDefault().Value;
                    }

                }
            }
            catch (ArgumentNullException)
            {

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
                //PaperType = articleType,
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
                authorListString.Add(name.Split(',')[1].Trim() + '-' + name.Split(',')[0].Trim());

                authorList.Add(new PubScreenAuthor
                {
                    FirstName = name.Split(',')[1].Trim(),
                    LastName = name.Split(',')[0].Trim(),

                });

            }

            string authorString = string.Join(", ", authorListString);

            var pubscreenObj = new PubScreen
            {
                Title = jsonPubscreen.collection[0].title,
                Abstract = jsonPubscreen.collection[0].@abstract,
                Year = (jsonPubscreen.collection[jsonPubscreen.collection.Count() - 1].date).Split('-')[0],
                AuthorString = authorString,
                //PaperType = jsonPubscreen.collection[0].@type,
                Author = authorList,
                Reference = "bioRxiv",
                DOI = doi,

            };

            return pubscreenObj;

        }

        //Function Definition to get some paper's info based on PubMedKey
        public async Task<PubScreen> GetPaperInfoByDOICrossref(string doi)
        {
            // if IskeyPumbed is true then get doi and add it to object
            HttpClient httpClient = new HttpClient();

            var content = new StringContent(String.Empty, Encoding.UTF8, "application/xml");
            var incomingXml = new XElement("newXML");
            string responseString = "";

            // Pubmed API is rate-limited. If the rate limit is exceeded, the response string from the HTTP Post will indicate that.
            // When that is the case, we wait 0.3s and attempt again, until we obtain our desired result or another error occurs
            bool isSuccess = false;
            while (!isSuccess)
            {
                try
                {
                    var response = await httpClient.PostAsync("https://doi.crossref.org/servlet/query?pid=mousebyt@uwo.ca&format=unixref&id=" + doi, content);
                    responseString = await response.Content.ReadAsStringAsync();
                    incomingXml = XElement.Parse(responseString);
                    isSuccess = true;
                }
                catch
                {
                    Console.WriteLine(responseString);
                    if (responseString.IndexOf("API rate limit exceeded") == -1)
                    {
                        Console.WriteLine("Unknown Error in GetPaperInfoByDOICrossref!");
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(300);
                    }
                }
            }

            string articleAbstract = "";
            string articleYear = "";
            string articleName = "";

            // Determine type of content (conference, journal, or posted content)
            XElement crossrefElement = null;
            if (incomingXml.Element("doi_record").Element("crossref").Element("journal") != null)
            {
                crossrefElement = incomingXml.Element("doi_record").Element("crossref").Element("journal").Element("journal_article");

                // journal name
                if (incomingXml.Element("doi_record").Element("crossref").Element("journal").Element("journal_metadata").Element("full_title") != null)
                {
                    articleName = incomingXml.Element("doi_record").Element("crossref").Element("journal").Element("journal_metadata").Element("full_title").Value;
                }
            }
            else if (incomingXml.Element("doi_record").Element("crossref").Element("conference") != null)
            {
                crossrefElement = incomingXml.Element("doi_record").Element("crossref").Element("conference").Element("conference_paper");

                // conference name
                if (incomingXml.Element("doi_record").Element("crossref").Element("conference").Element("event_metadata").Element("conference_name") != null)
                {
                    articleName = incomingXml.Element("doi_record").Element("crossref").Element("conference").Element("event_metadata").Element("conference_name").Value;
                }
            }
            else if (incomingXml.Element("doi_record").Element("crossref").Element("posted_content") != null)
            {
                crossrefElement = incomingXml.Element("doi_record").Element("crossref").Element("posted_content");
                //if (incomingXml.Element("doi_record").Element("crossref").Element("posted_content").Element("group_title") != null)
                //{
                //    articleName = incomingXml.Element("doi_record").Element("crossref").Element("posted_content").Element("group_title").Value;
                //}
            }
            else
            {
                return null;
            }

            //title
            string articleTitle = "";
            if (crossrefElement.Element("titles").Element("title") != null)
            {
                articleTitle = crossrefElement.Element("titles").Element("title").Value;
            }

            //abstract
            if (crossrefElement.Element("abstract") != null)
            {
                articleAbstract = crossrefElement.Element("abstract").Value;
            }

            //articletype
            //string articleType = incomingXml.Element("PubmedArticle").Element("MedlineCitation").Element("Article").Element("PublicationTypeList").Element("PublicationType").Value;

            //year
            if (crossrefElement.Element("publication_date") != null)
            {
                articleYear = crossrefElement.Element("publication_date").Element("year").Value;
            }
            else if (crossrefElement.Element("acceptance_date") != null)
            {
                articleYear = crossrefElement.Element("acceptance_date").Element("year").Value;
            }

            // Extracting list of Authors and add them to author object list
            List<PubScreenAuthor> authorList = new List<PubScreenAuthor>();
            List<string> authorListString = new List<string>();
            try
            {
                foreach (var authorElement in crossrefElement.Element("contributors").Elements("person_name").Where(id => id.Attribute("contributor_role").Value == "author"))
                {
                    string givenName = null, surName = null, affiliation = null;
                    if (authorElement.Element("given_name") != null)
                    {
                        givenName = authorElement.Element("given_name").Value;
                    }
                    if (authorElement.Element("surname") != null)
                    {
                        surName = authorElement.Element("surname").Value;
                    }
                    if (authorElement.Element("affiliation") != null)
                    {
                        affiliation = authorElement.Element("affiliation").Value;
                    }
                    if (givenName != null && surName != null)
                    {
                        authorListString.Add(givenName.Trim() + "-" + surName.Trim());
                        authorList.Add(new PubScreenAuthor
                        {
                            FirstName = givenName.Trim(),
                            LastName = surName.Trim(),
                            Affiliation = affiliation == null ? "" : affiliation.Split(',')[0],
                        });
                    }
                }

            }
            catch (ArgumentNullException)
            {

            }

            

            if (crossrefElement.Element("doi_data").Element("doi") != null)
            {
                doi = crossrefElement.Element("doi_data").Element("doi").Value;
            }

            string authorString = string.Join(", ", authorListString);

            // initialize PubScreenSearch object and fill it
            var pubscreenObj = new PubScreen
            {
                Title = articleTitle,
                Abstract = articleAbstract,
                Year = articleYear,
                AuthorString = authorString,
                //PaperType = articleType,
                Author = authorList,
                Reference = articleName,
                DOI = doi,


            };

            // Actually two outputs should be returned (authorList should be also returned)

            return pubscreenObj;


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
                        PaperType = Convert.ToString(dr["PaperType"].ToString()),
                        ID = Int32.Parse(dr["ID"].ToString()),

                    });
                }
            }

            return PaperTypeList;
        }


        // Function Definition to extract list of all Cognitive Tasks
        public List<PubScreenTask> GetTasks()
        {
            List<PubScreenTask> TaskList = new List<PubScreenTask>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Task Order By (Case When Task not like '%None%' Then 1 Else 2 End), Task"))
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

        // Function Definition to extract list of all Task & Sub-Tasks
        public List<PubScreenTask> GetAllTasks()
        {
            List<PubScreenTask> TaskList = new List<PubScreenTask>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select SubTask.ID, SubTask.TaskID, Task.Task, SubTask.SubTask 
                                                             From SubTask
                                                             Inner join Task on Task.ID = SubTask.TaskID
                                                             Order By (Case When SubTask not like '%None%' Then 1 Else 2 End), TaskID, SubTask"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TaskList.Add(new PubScreenTask
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        TaskID = Int32.Parse(dr["TaskID"].ToString()),
                        Task = Convert.ToString(dr["Task"].ToString()),
                        SubTask = Convert.ToString(dr["SubTask"].ToString()),


                    });
                }
            }

            return TaskList;
        }

        // Function Definition to extract list of all Species
        public List<PubScreenSpecie> GetSpecies()
        {
            List<PubScreenSpecie> SpecieList = new List<PubScreenSpecie>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Species Order By (Case When Species not like '%Other%' Then 1 Else 2 End), Species"))
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
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Sex Order By Sex"))
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
            using (DataTable dt = Dal.GetDataTablePub($@"Select Strain.ID, Strain, SpeciesID
                                                            From Strain Inner Join Species On Strain.SpeciesID = Species.ID
                                                            Order By (Case When Strain not like '%Other%' Then 1 Else 2 End), Species, Strain"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StrainList.Add(new PubScreenStrain
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),
                        SpeciesID = Int32.Parse(dr["SpeciesID"].ToString()),
                    });
                }
            }

            return StrainList;
        }


        // Function Definition to extract list of all Disease Models
        public List<PubScreenDisease> GetDisease()
        {
            List<PubScreenDisease> DiseaseList = new List<PubScreenDisease>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From DiseaseModel Order By (Case When DiseaseModel not like '%Other%' Then 1 Else 2 End), DiseaseModel"))
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

        // Function Definition to extract list of all Submodels
        public List<PubScreenSubModel> GetSubModels()
        {
            List<PubScreenSubModel> SubModelList = new List<PubScreenSubModel>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select SubModel.ID, SubModel, ModelID
                                                            From SubModel Inner Join DiseaseModel On SubModel.ModelID = DiseaseModel.ID
                                                            Order By (Case When SubModel not like '%Other%' Then 1 Else 2 End), DiseaseModel, SubModel"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SubModelList.Add(new PubScreenSubModel
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        SubModel = Convert.ToString(dr["SubModel"].ToString()),
                        ModelID = Int32.Parse(dr["ModelID"].ToString()),
                    });
                }
            }

            return SubModelList;
        }

        // Function Definition to extract list of all Regions & Sub-regions
        public List<PubScreenRegion> GetAllRegions()
        {
            List<PubScreenRegion> RegionList = new List<PubScreenRegion>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select SubRegion.ID, SubRegion.RID, BrainRegion.BrainRegion, SubRegion.SubRegion 
                                                             From SubRegion
                                                             Inner join BrainRegion on BrainRegion.ID = SubRegion.RID
                                                             Order By (Case When SubRegion not like '%Other%' Then 1 Else 2 End), RID, SubRegion"))
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
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From BrainRegion Order By (Case When BrainRegion not like '%Other%' Then 1 Else 2 End), BrainRegion"))
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
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From CellType Order By (Case When CellType not like '%Other%' Then 1 Else 2 End), CellType"))
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
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Method Order By (Case When Method not like '%Other%' Then 1 Else 2 End), Method"))
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
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From NeuroTransmitter Order By (Case When NeuroTransmitter not like '%Other%' Then 1 Else 2 End), NeuroTransmitter"))
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
        public int AddAuthors(PubScreenAuthor author, string userEmail)
        {
            string sql = $@"Insert into Author (FirstName, LastName, Affiliation, username) Values
                            ('{author.FirstName}', '{author.LastName}', '{author.Affiliation}', '{userEmail}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalarPub(sql).ToString());
        }


        // Function Definition to extract list of Authors 
        public List<PubScreenAuthor> GetAuthors()
        {
            List<PubScreenAuthor> AuthorList = new List<PubScreenAuthor>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Author Order By LastName"))
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
                             Delete From Publication_SubTask Where PublicationID = {pubId};
                             Delete From Publication Where id = { pubId};";

            Dal.ExecuteNonQueryPub(sql);
        }

        //************************************************************************************Adding Publication*********************************
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

            string sqlPublication = $@"Insert into Publication (PaperLinkGuid, Title, Abstract, Keywords, DOI, Year, Reference, Username, Source) Values
                                    ('{Guid.NewGuid()}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(publication.Title)).Trim())}',
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
                        sqlAuthor += $@"Insert into Author (FirstName, LastName, Affiliation) Values ('{HelperService.EscapeSql(publication.Author[i].FirstName.Trim())}',
                                                                                                      '{HelperService.EscapeSql(publication.Author[i].LastName.Trim())}',
                                                                                                      '{HelperService.EscapeSql(HelperService.NullToString(publication.Author[i].Affiliation).Trim())}');";
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
                    sqlauthor2 = $@"Select ID From Author Where CONCAT(LOWER(Author.FirstName), '-', LOWER(Author.LastName))= '{HelperService.EscapeSql(author).Trim()}';";
                    authorID = Int32.Parse((Dal.ExecScalarPub(sqlauthor2).ToString()));

                    sqlauthor3 = $@"Insert into Publication_Author (AuthorID, PublicationID, AuthorOrder) Values ({authorID}, {PublicationID}, {j});";
                    Dal.ExecuteNonQueryPub(sqlauthor3).ToString();
                    j++;
                }



            } // End of if statement when DOI OR Pubmed is available


            //Adding to Publication_PaperType Table**********************************************************************************************


            if (publication.PaperTypeID != null)
            {
                string sqlPaperType = "";
                sqlPaperType = $@"Insert into Publication_PaperType (PaperTypeID, PublicationID) Values ({publication.PaperTypeID}, {PublicationID});";
                Dal.ExecuteNonQueryPub(sqlPaperType);
            }

            //******************************Key Features**************
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

            //Adding to Publication_SubTask
            if (publication.SubTaskID != null && publication.SubTaskID.Length != 0)
            {
                string sqlSubTask = "";
                for (int i = 0; i < publication.SubTaskID.Length; i++)
                {

                    sqlSubTask += $@"Insert into Publication_SubTask (SubTaskID, PublicationID) Values ({publication.SubTaskID[i]}, {PublicationID});";


                }
                if (sqlSubTask != "") { Dal.ExecuteNonQueryPub(sqlSubTask); };

            }

            //Adding to Publication_Specie
            // Handling other for species
            ProcessOther(publication.SpecieOther, "Species", "Species", "Publication_Specie", "SpecieID", PublicationID, Username);

            if (publication.SpecieID != null && publication.SpecieID.Length != 0)
            {
                string sqlSpecie = "";
                for (int i = 0; i < publication.SpecieID.Length; i++)
                {
                    if (publication.SpecieID[i] != 3)
                    {
                        sqlSpecie += $@"Insert into Publication_Specie (SpecieID, PublicationID) Values ({publication.SpecieID[i]}, {PublicationID});";
                    }

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
            // handling other for strain
            //ProcessOther(publication.StrainMouseOther, "Strain", "Strain", "Publication_Strain", "StrainID", PublicationID, Username);
            ProcessOtherStrain(publication.StrainMouseOther, MOUSEID, PublicationID, Username);
            ProcessOtherStrain(publication.StrainRatOther, RATID, PublicationID, Username);

            // Get 'Other' entries in Strain table
            List<int?> otherStrainID = new List<int?>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select ID from Strain Where Strain like '%Other%'"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    otherStrainID.Add(Int32.Parse(dr["ID"].ToString()));
                }
            }

            if (publication.StrainID != null && publication.StrainID.Length != 0)
            {
                string sqlStrain = "";
                for (int i = 0; i < publication.StrainID.Length; i++)
                {
                    //if (publication.StrainID[i] != 19 && publication.StrainID[i] != 4019)
                    if (!otherStrainID.Contains(publication.StrainID[i]))
                    {
                        sqlStrain += $@"Insert into Publication_Strain (StrainID, PublicationID) Values ({publication.StrainID[i]}, {PublicationID});";
                    }

                }
                if (sqlStrain != "") { Dal.ExecuteNonQueryPub(sqlStrain); };

            }

            //Adding to Publication_Disease
            // handling other for disease model
            ProcessOther(publication.DiseaseOther, "DiseaseModel", "DiseaseModel", "Publication_Disease", "DiseaseID", PublicationID, Username);

            if (publication.DiseaseID != null && publication.DiseaseID.Length != 0)
            {
                string sqlDiease = "";
                for (int i = 0; i < publication.DiseaseID.Length; i++)
                {
                    if (publication.DiseaseID[i] != 14)
                    {
                        sqlDiease += $@"Insert into Publication_Disease (DiseaseID, PublicationID) Values ({publication.DiseaseID[i]}, {PublicationID});";
                    }

                }
                if (sqlDiease != "") { Dal.ExecuteNonQueryPub(sqlDiease); };

            }

            //Adding to Publication_SubModel
            if (publication.SubModelID != null && publication.SubModelID.Length != 0)
            {
                string sqlSub = "";
                for (int i = 0; i < publication.SubModelID.Length; i++)
                {
                    sqlSub += $@"Insert into Publication_SubModel (SubModelID, PublicationID) Values ({publication.SubModelID[i]}, {PublicationID});";

                }
                if (sqlSub != "") { Dal.ExecuteNonQueryPub(sqlSub); };

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
            // handling other for cell type
            ProcessOther(publication.CelltypeOther, "CellType", "CellType", "Publication_CellType", "CelltypeID", PublicationID, Username);

            if (publication.CellTypeID != null && publication.CellTypeID.Length != 0)
            {
                string sqlCelltype = "";
                for (int i = 0; i < publication.CellTypeID.Length; i++)
                {
                    if (publication.CellTypeID[i] != 4)
                    {
                        sqlCelltype += $@"Insert into Publication_CellType (CellTypeID, PublicationID) Values ({publication.CellTypeID[i]}, {PublicationID});";
                    }

                }
                if (sqlCelltype != "") { Dal.ExecuteNonQueryPub(sqlCelltype); };

            }

            //Adding to Publication_Method
            // handling other for method
            ProcessOther(publication.MethodOther, "Method", "Method", "Publication_Method", "MethodID", PublicationID, Username);

            if (publication.MethodID != null && publication.MethodID.Length != 0)
            {
                string sqlMethod = "";
                for (int i = 0; i < publication.MethodID.Length; i++)
                {
                    if (publication.MethodID[i] != 23)
                    {
                        sqlMethod += $@"Insert into Publication_Method (MethodID, PublicationID) Values ({publication.MethodID[i]}, {PublicationID});";
                    }

                }
                if (sqlMethod != "") { Dal.ExecuteNonQueryPub(sqlMethod); };

            }

            //Adding to Publication_NeuroTransmitter
            //hanlding other for NeuroTransmitter
            ProcessOther(publication.NeurotransOther, "Neurotransmitter", "NeuroTransmitter", "Publication_NeuroTransmitter", "TransmitterID", PublicationID, Username);

            if (publication.TransmitterID != null && publication.TransmitterID.Length != 0)
            {
                string sqlTransmitter = "";
                for (int i = 0; i < publication.TransmitterID.Length; i++)
                {
                    if (publication.TransmitterID[i] != 8)
                    {
                        sqlTransmitter += $@"Insert into Publication_NeuroTransmitter (TransmitterID, PublicationID) Values ({publication.TransmitterID[i]}, {PublicationID});";
                    }

                }
                if (sqlTransmitter != "") { Dal.ExecuteNonQueryPub(sqlTransmitter); };

            }

            return PublicationID;

        }
        //*******************************************************************************************************************************************************************

        // Edit publication
        public bool EditPublication(int publicationId, PubScreen publication, string Username)
        {

            // Get original paper information
            var origPub = new PubScreenSearch();

            using (DataTable dt = Dal.GetDataTablePub($"Select * From SearchPub Where ID = {publicationId}"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    origPub.ID = Int32.Parse(dr["ID"].ToString());
                    origPub.Title = Convert.ToString(dr["Title"].ToString());
                    origPub.Abstract = Convert.ToString(dr["Abstract"].ToString());
                    origPub.Keywords = Convert.ToString(dr["Keywords"].ToString());
                    origPub.DOI = Convert.ToString(dr["DOI"].ToString());
                    origPub.Year = Convert.ToString(dr["Year"].ToString());
                    origPub.Author = Convert.ToString(dr["Author"].ToString());
                    origPub.PaperType = Convert.ToString(dr["PaperType"].ToString());
                    origPub.Task = Convert.ToString(dr["Task"].ToString());
                    origPub.SubTask = Convert.ToString(dr["SubTask"].ToString());
                    origPub.Species = Convert.ToString(dr["Species"].ToString());
                    origPub.Sex = Convert.ToString(dr["Sex"].ToString());
                    origPub.Strain = Convert.ToString(dr["Strain"].ToString());
                    origPub.DiseaseModel = Convert.ToString(dr["DiseaseModel"].ToString());
                    origPub.BrainRegion = Convert.ToString(dr["BrainRegion"].ToString());
                    origPub.SubRegion = Convert.ToString(dr["SubRegion"].ToString());
                    origPub.CellType = Convert.ToString(dr["CellType"].ToString());
                    origPub.Method = Convert.ToString(dr["Method"].ToString());
                    origPub.NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString());
                    origPub.Reference = Convert.ToString(dr["Reference"].ToString());
                }
            };

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
            //var paperTypeId = 0;
            //if (!string.IsNullOrEmpty(publication.PaperType))
            //{
            //    var sql = $"select id from PaperType where ltrim(rtrim(papertype)) = '{HelperService.NullToString(HelperService.EscapeSql(publication.PaperType)).Trim()}'";
            //    var retPaperType = Dal.ExecScalarPub(sql);
            //    if (retPaperType != null)
            //    {
            //        paperTypeId = Int32.Parse(Dal.ExecScalarPub(sql).ToString());

            //    }
            //}

            if (publication.PaperTypeID != null)
            {
                sqlDelete = $"DELETE From Publication_PaperType where PublicationID = {publicationId}";
                Dal.ExecuteNonQueryPub(sqlDelete);

                string sqlPaperType = "";
                sqlPaperType = $@"Insert into Publication_PaperType (PaperTypeID, PublicationID) Values ({publication.PaperTypeID}, {publicationId});";
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

            //Editing to Publication_SubTask
            if (publication.SubTaskID != null && publication.SubTaskID.Length != 0)
            {
                sqlDelete = $"DELETE From Publication_SubTask where PublicationID = {publicationId}";
                Dal.ExecuteNonQueryPub(sqlDelete);

                string sqlSubTask = "";
                for (int i = 0; i < publication.SubTaskID.Length; i++)
                {

                    sqlSubTask += $@"Insert into Publication_SubTask (SubTaskID, PublicationID) Values ({publication.SubTaskID[i]}, {publicationId});";


                }
                if (sqlSubTask != "") { Dal.ExecuteNonQueryPub(sqlSubTask); };

            }

            //Editing to Publication_Specie
            sqlDelete = $"DELETE From Publication_Specie where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.SpecieID != null && publication.SpecieID.Length != 0)
            {
                string sqlSpecie = "";
                for (int i = 0; i < publication.SpecieID.Length; i++)
                {
                    if (publication.SpecieID[i] != 3)
                    {
                        sqlSpecie += $@"Insert into Publication_Specie (SpecieID, PublicationID) Values ({publication.SpecieID[i]}, {publicationId});";
                    }

                }
                if (sqlSpecie != "") { Dal.ExecuteNonQueryPub(sqlSpecie); };
            }

            // Handling Other for Species
            ProcessOther(publication.SpecieOther, "Species", "Species", "Publication_Specie", "SpecieID", publicationId, Username);

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

            // Get 'Other' entries in Strain table
            List<int?> otherStrainID = new List<int?>();
            using (DataTable dt = Dal.GetDataTablePub($@"Select ID from Strain Where Strain like '%Other%'"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    otherStrainID.Add(Int32.Parse(dr["ID"].ToString()));
                }
            }

            if (publication.StrainID != null && publication.StrainID.Length != 0)
            {
                string sqlStrain = "";
                for (int i = 0; i < publication.StrainID.Length; i++)
                {
                    //if (publication.StrainID[i] != 19)
                    if (!otherStrainID.Contains(publication.StrainID[i]))
                    {
                        sqlStrain += $@"Insert into Publication_Strain (StrainID, PublicationID) Values ({publication.StrainID[i]}, {publicationId});";
                    }

                }
                if (sqlStrain != "") { Dal.ExecuteNonQueryPub(sqlStrain); };

            }

            // Handking Other for Strain
            //ProcessOther(publication.StrainMouseOther, "Strain", "Strain", "Publication_Strain", "StrainID", publicationId, Username);
            ProcessOtherStrain(publication.StrainMouseOther, MOUSEID, publicationId, Username);
            ProcessOtherStrain(publication.StrainRatOther, RATID, publicationId, Username);

            //Editing to Publication_Disease
            sqlDelete = $"DELETE From Publication_Disease where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.DiseaseID != null && publication.DiseaseID.Length != 0)
            {
                string sqlDiease = "";
                for (int i = 0; i < publication.DiseaseID.Length; i++)
                {
                    if (publication.DiseaseID[i] != 14)
                    {
                        sqlDiease += $@"Insert into Publication_Disease (DiseaseID, PublicationID) Values ({publication.DiseaseID[i]}, {publicationId});";
                    }

                }
                if (sqlDiease != "") { Dal.ExecuteNonQueryPub(sqlDiease); };

            }

            // Handling Other for Disease
            ProcessOther(publication.DiseaseOther, "DiseaseModel", "DiseaseModel", "Publication_Disease", "DiseaseID", publicationId, Username);

            //Editing to Publication_SubModel
            sqlDelete = $"DELETE From Publication_SubModel where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);
            if (publication.SubModelID != null && publication.SubModelID.Length != 0)
            {
                string sqlSub = "";
                for (int i = 0; i < publication.SubModelID.Length; i++)
                {
                    sqlSub += $@"Insert into Publication_SubModel (SubModelID, PublicationID) Values ({publication.SubModelID[i]}, {publicationId});";

                }
                if (sqlSub != "") { Dal.ExecuteNonQueryPub(sqlSub); };

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
                    if (publication.CellTypeID[i] != 4)
                    {
                        sqlCelltype += $@"Insert into Publication_CellType (CellTypeID, PublicationID) Values ({publication.CellTypeID[i]}, {publicationId});";
                    }

                }
                if (sqlCelltype != "") { Dal.ExecuteNonQueryPub(sqlCelltype); };

            }

            //Handling Other for Cell Type
            ProcessOther(publication.CelltypeOther, "CellType", "CellType", "Publication_CellType", "CelltypeID", publicationId, Username);

            //Editing to Publication_Method
            sqlDelete = $"DELETE From Publication_Method where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.MethodID != null && publication.MethodID.Length != 0)
            {
                string sqlMethod = "";
                for (int i = 0; i < publication.MethodID.Length; i++)
                {
                    if (publication.MethodID[i] != 23)
                    {
                        sqlMethod += $@"Insert into Publication_Method (MethodID, PublicationID) Values ({publication.MethodID[i]}, {publicationId});";
                    }

                }
                if (sqlMethod != "") { Dal.ExecuteNonQueryPub(sqlMethod); };

            }

            //Handling Other for method
            ProcessOther(publication.MethodOther, "Method", "Method", "Publication_Method", "MethodID", publicationId, Username);

            //Editing to Publication_NeuroTransmitter
            sqlDelete = $"DELETE From Publication_NeuroTransmitter where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);

            if (publication.TransmitterID != null && publication.TransmitterID.Length != 0)
            {
                string sqlTransmitter = "";
                for (int i = 0; i < publication.TransmitterID.Length; i++)
                {
                    if (publication.TransmitterID[i] != 8)
                    {
                        sqlTransmitter += $@"Insert into Publication_NeuroTransmitter (TransmitterID, PublicationID) Values ({publication.TransmitterID[i]}, {publicationId});";
                    }

                }
                if (sqlTransmitter != "") { Dal.ExecuteNonQueryPub(sqlTransmitter); };

            }

            // Handling Other for NeuroTransmitter
            ProcessOther(publication.NeurotransOther, "Neurotransmitter", "NeuroTransmitter", "Publication_NeuroTransmitter", "TransmitterID", publicationId, Username);

            // Get new paper information

            // Get updated paper information
            var newPub = new PubScreenSearch();

            using (DataTable dt = Dal.GetDataTablePub($"Select * From SearchPub Where ID = {publicationId}"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    newPub.ID = Int32.Parse(dr["ID"].ToString());
                    newPub.Title = Convert.ToString(dr["Title"].ToString());
                    newPub.Abstract = Convert.ToString(dr["Abstract"].ToString());
                    newPub.Keywords = Convert.ToString(dr["Keywords"].ToString());
                    newPub.DOI = Convert.ToString(dr["DOI"].ToString());
                    newPub.Year = Convert.ToString(dr["Year"].ToString());
                    newPub.Author = Convert.ToString(dr["Author"].ToString());
                    newPub.PaperType = Convert.ToString(dr["PaperType"].ToString());
                    newPub.Task = Convert.ToString(dr["Task"].ToString());
                    newPub.SubTask = Convert.ToString(dr["SubTask"].ToString());
                    newPub.Species = Convert.ToString(dr["Species"].ToString());
                    newPub.Sex = Convert.ToString(dr["Sex"].ToString());
                    newPub.Strain = Convert.ToString(dr["Strain"].ToString());
                    newPub.DiseaseModel = Convert.ToString(dr["DiseaseModel"].ToString());
                    newPub.BrainRegion = Convert.ToString(dr["BrainRegion"].ToString());
                    newPub.SubRegion = Convert.ToString(dr["SubRegion"].ToString());
                    newPub.CellType = Convert.ToString(dr["CellType"].ToString());
                    newPub.Method = Convert.ToString(dr["Method"].ToString());
                    newPub.NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString());
                    newPub.Reference = Convert.ToString(dr["Reference"].ToString());
                }
            };

            // Log changes made
            string changeLog = "";
            PropertyInfo[] pubProperties = typeof(PubScreenSearch).GetProperties();
            foreach (PropertyInfo pubProperty in pubProperties)
            {
                if (!Object.Equals(pubProperty.GetValue(origPub), pubProperty.GetValue(newPub)))
                {
                    changeLog += $"{pubProperty.Name} changed.\n" +
                        $"Original: {pubProperty.GetValue(origPub)}\n" +
                        $"Edited: {pubProperty.GetValue(newPub)}\n\n";
                }
            }

            // Log edit to database
            if (!string.IsNullOrEmpty(changeLog))
            {
                DateTime today = DateTime.Today;
                Dal.ExecuteNonQueryPub($"Insert Into EditLog (PubID, Username, EditDate, ChangeLog) Values " +
                    $"({publicationId}, '{Username}', '{today.ToString().Split('T')[0]}', '{changeLog.Replace("'", "''")}')");

                // Send email detailing paper edits
                string emailMsg = $"Pubscreen paper '{newPub.Title}' (ID {newPub.ID}), was edited by {Username}.\n\n" +
                    $"The following changes were made:\n\n{changeLog}";
                HelperService.SendEmail("", "", "PUBSCREEN: Edit made", emailMsg.Replace("\n", "<br \\>"));
            }



            return true;
        }

        // Function definition to search publications in database
        public List<PubScreenSearch> SearchPublications(PubScreen pubScreen)
        {
            List<PubScreenSearch> lstPubScreen = new List<PubScreenSearch>();

            string sql = "Select * From SearchPub Where ";

            // Title
            if (!string.IsNullOrEmpty(pubScreen.Title))
            {
                sql += $@"SearchPub.Title like '%{(HelperService.EscapeSql(pubScreen.Title)).Trim()}%' AND ";
            }

            //Keywords
            if (!string.IsNullOrEmpty(pubScreen.Keywords))
            {
                sql += $@"SearchPub.Keywords like '%{HelperService.EscapeSql(pubScreen.Keywords)}%' AND ";
            }

            // DOI
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
            if (pubScreen.YearFrom != null && pubScreen.YearTo != null)
            {
                sql += $@"(SearchPub.Year >= {pubScreen.YearFrom} AND SearchPub.Year <= {pubScreen.YearTo}) AND ";
            }

            if (pubScreen.YearFrom != null && pubScreen.YearTo == null)
            {
                sql += $@"(SearchPub.Year >= {pubScreen.YearFrom}) AND ";
            }

            if (pubScreen.YearTo != null && pubScreen.YearFrom == null)
            {
                sql += $@"(SearchPub.Year <= {pubScreen.YearTo}) AND ";
            }

            // search query for PaperType
            //if (pubScreen.PaperTypeID != null)
            //{
            //    sql += $@"SearchPub.PaperType like '%'  + (Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeID}) +  '%' AND ";
            //}

            // search query for Paper type
            if (pubScreen.PaperTypeIdSearch != null && pubScreen.PaperTypeIdSearch.Length != 0)
            {

                if (pubScreen.PaperTypeIdSearch.Length == 1)
                {
                    sql += $@"SearchPub.PaperType like '%'  + (Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeIdSearch[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.PaperTypeIdSearch.Length; i++)
                    {
                        sql += $@"SearchPub.PaperType like '%'  + (Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeIdSearch[i]}) +  '%' OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") AND ";
                }

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

            // search query for SubTask
            if (pubScreen.SubTaskID != null && pubScreen.SubTaskID.Length != 0)
            {
                if (pubScreen.SubTaskID.Length == 1)
                {
                    sql += $@"SearchPub.SubTask like '%'  + (Select SubTask From SubTask Where SubTask.ID = {pubScreen.SubTaskID[0]}) +  '%' AND ";
                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.SubTaskID.Length; i++)
                    {
                        sql += $@"SearchPub.SubTask like '%'  + (Select SubTask From SubTask Where SubTask.ID = {pubScreen.SubTaskID[i]}) +  '%' OR ";
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

            // search query for Sub Model
            if (pubScreen.SubModelID != null && pubScreen.SubModelID.Length != 0)
            {
                if (pubScreen.SubModelID.Length == 1)
                {
                    sql += $@"SearchPub.SubModel like '%'  + (Select SubModel From SubModel Where SubModel.ID = {pubScreen.SubModelID[0]}) +  '%' AND ";

                }
                else
                {
                    sql += "(";
                    for (int i = 0; i < pubScreen.SubModelID.Length; i++)
                    {
                        sql += $@"SearchPub.SubModel like '%'  + (Select SubModel From SubModel Where SubModel.ID = {pubScreen.SubModelID[i]}) +  '%' OR ";

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
            //sql += "ORDER BY Year DESC";

            string sqlMB = "";
            string sqlCog = "";
            List<Experiment> lstExperiment = new List<Experiment>();
            List<Cogbytes> lstRepo = new List<Cogbytes>();
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

                    sqlCog = $"Select * From UserRepository Where DOI = '{Convert.ToString(dr["DOI"].ToString())}'";
                    lstRepo.Clear();
                    using (DataTable dtCog = Dal.GetDataTableCog(sqlCog))
                    {
                        foreach (DataRow drCog in dtCog.Rows)
                        {
                            var cogbytesService = new CogbytesService();
                            int repID = Int32.Parse(drCog["RepID"].ToString());
                            lstRepo.Add(new Cogbytes
                            {
                                ID = repID,
                                RepoLinkGuid = Guid.Parse(drCog["repoLinkGuid"].ToString()),
                                Title = Convert.ToString(drCog["Title"].ToString()),
                                Date = Convert.ToString(drCog["Date"].ToString()),
                                Keywords = Convert.ToString(drCog["Keywords"].ToString()),
                                DOI = Convert.ToString(drCog["DOI"].ToString()),
                                Link = Convert.ToString(drCog["Link"].ToString()),
                                PrivacyStatus = Boolean.Parse(drCog["PrivacyStatus"].ToString()),
                                Description = Convert.ToString(drCog["Description"].ToString()),
                                AdditionalNotes = Convert.ToString(drCog["AdditionalNotes"].ToString()),
                                AuthourID = cogbytesService.FillCogbytesItemArray($"Select AuthorID From RepAuthor Where RepID={repID}", "AuthorID"),
                                PIID = cogbytesService.FillCogbytesItemArray($"Select PIID From RepPI Where RepID={repID}", "PIID"),
                            });
                        }

                    }

                    lstPubScreen.Add(new PubScreenSearch
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        PaperLinkGuid = Guid.Parse(dr["PaperLinkGuid"].ToString()),
                        Title = Convert.ToString(dr["Title"].ToString()),
                        Keywords = Convert.ToString(dr["Keywords"].ToString()),
                        DOI = Convert.ToString(dr["DOI"].ToString()),
                        Year = Convert.ToString(dr["Year"].ToString()),
                        Author = Convert.ToString(dr["Author"].ToString()),
                        PaperType = Convert.ToString(dr["PaperType"].ToString()),
                        Task = Convert.ToString(dr["Task"].ToString()),
                        SubTask = Convert.ToString(dr["SubTask"].ToString()),
                        Species = Convert.ToString(dr["Species"].ToString()),
                        Sex = Convert.ToString(dr["Sex"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),
                        DiseaseModel = Convert.ToString(dr["DiseaseModel"].ToString()),
                        SubModel = Convert.ToString(dr["SubModel"].ToString()),
                        BrainRegion = Convert.ToString(dr["BrainRegion"].ToString()),
                        SubRegion = Convert.ToString(dr["SubRegion"].ToString()),
                        CellType = Convert.ToString(dr["CellType"].ToString()),
                        Method = Convert.ToString(dr["Method"].ToString()),
                        NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString()),
                        Reference = Convert.ToString(dr["Reference"].ToString()),
                        Experiment = new List<Experiment>(lstExperiment),
                        Repo = new List<Cogbytes>(lstRepo)

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

            sql = $"Select SubModelID From Publication_SubModel Where PublicationID ={id}";
            pubScreen.SubModelID = FillPubScreenItemArray(sql, "SubModelID");

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

            sql = $"Select SubTaskID From Publication_SubTask Where PublicationID ={id}";
            pubScreen.SubTaskID = FillPubScreenItemArray(sql, "SubTaskID");

            sql = $"Select PaperTypeID From Publication_PaperType Where PublicationID ={id}";
            object papertypeID = Dal.ExecScalarPub(sql);
            if (papertypeID == null)
            {
                pubScreen.PaperTypeID = null;
            }
            else
            {
                pubScreen.PaperTypeID = Int32.Parse(papertypeID.ToString());
            }

            sql = $"Select * From Publication Where ID ={id}";
            using (DataTable dt = Dal.GetDataTablePub(sql))
            {
                pubScreen.PaperLinkGuid = Guid.Parse(dt.Rows[0]["PaperLinkGuid"].ToString());
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

        public void ProcessOther(string inputOther, string tableOther, string fieldOther, string tblPublication,
                                string tblPublicationField, int PublicationID, string Username)
        {
            if (!String.IsNullOrEmpty(inputOther))
            {
                List<string> ItemList = inputOther.Split(';').Select(p => p.Trim()).ToList();
                foreach (string item in ItemList)
                {
                    string sqlOther = $@"Select ID From {tableOther} Where ltrim(rtrim({fieldOther})) = '{HelperService.NullToString(HelperService.EscapeSql(item)).Trim()}';";
                    var IDOther = Dal.ExecScalarPub(sqlOther);

                    if (IDOther == null)
                    {
                        string sqlOther2 = $@"Insert Into {tableOther} ({fieldOther}, Username) Values
                                            ('{HelperService.NullToString(HelperService.EscapeSql(item)).Trim()}', '{Username}'); SELECT @@IDENTITY AS 'Identity';";
                        int IDOther2 = Int32.Parse((Dal.ExecScalarPub(sqlOther2).ToString()));

                        string sqlOther3 = $@"Insert into {tblPublication} ({tblPublicationField}, PublicationID) Values ({IDOther2}, {PublicationID}); ";
                        Dal.ExecuteNonQueryPub(sqlOther3);
                    }

                }

            }
        }

        public void ProcessOtherStrain(string inputOther, int speciesID, int PublicationID, string Username)
        {
            if (!String.IsNullOrEmpty(inputOther))
            {
                List<string> ItemList = inputOther.Split(';').Select(p => p.Trim()).ToList();
                foreach (string item in ItemList)
                {
                    string sqlOther = $@"Select ID From Strain Where Strain = '{HelperService.NullToString(HelperService.EscapeSql(item)).Trim()}' AND SpeciesID = {speciesID};";
                    var IDOther = Dal.ExecScalarPub(sqlOther);

                    if (IDOther == null)
                    {
                        string sqlOther2 = $@"Insert Into Strain (Strain, SpeciesID, Username) Values
                                            ('{HelperService.NullToString(HelperService.EscapeSql(item)).Trim()}', {speciesID}, '{Username}'); SELECT @@IDENTITY AS 'Identity';";
                        int IDOther2 = Int32.Parse((Dal.ExecScalarPub(sqlOther2).ToString()));

                        string sqlOther3 = $@"Insert into Publication_Strain (StrainID, PublicationID) Values ({IDOther2}, {PublicationID}); ";
                        Dal.ExecuteNonQueryPub(sqlOther3);
                    }

                }

            }
        }

        public List<PubmedPaper> GetPubmedQueue()
        {
            List<PubmedPaper> PubmedQueue = new List<PubmedPaper>();

            using (DataTable dt = Dal.GetDataTablePub($@"Select * from PubmedQueue Where IsProcessed = 0 Order By QueueDate, PubDate"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PubmedQueue.Add(new PubmedPaper
                    {
                        // Paper = await GetPaperInfoByPubMedKey(Convert.ToString(dr["PubmedID"].ToString())),
                        Title = Convert.ToString(dr["Title"].ToString()),
                        PubmedID = Int32.Parse(dr["PubmedID"].ToString()),
                        PubDate = Convert.ToString(dr["PubDate"].ToString()),
                        QueueDate = Convert.ToString(dr["QueueDate"].ToString()),
                        DOI = Convert.ToString(dr["DOI"].ToString()),
                    });
                }
            }

            return PubmedQueue;
        }

        public async Task<int?> AddQueuePaper(int pubmedID, string doi, string userName)
        {
            PubScreen paper = await GetPaperInfoByPubMedKey(pubmedID.ToString());
            paper.DOI = doi;
            int? PubID = AddPublications(paper, userName);

            ProcessQueuePaper(pubmedID);

            return PubID;
        }

        public void ProcessQueuePaper(int pubmedID)
        {
            Dal.ExecuteNonQueryPub($"Update PubmedQueue Set IsProcessed = 1 Where PubmedID = {pubmedID}");
        }

        public (int, int) GetPubCount()
        {
            int pubCount = (int)Dal.ExecScalarPub("Select Count(ID) From SearchPub");
            int featureCount = (int)Dal.ExecScalarPub("Select Count(Task) From SearchPub");
            return (pubCount, featureCount);
        }

        public async Task<List<string>> AddCSVPapers(string userName)
        {
            List<string> doiList = new List<string>();

            using (DataTable dt = Dal.GetDataTablePub($@"Select * From Publication Where Title like ''"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int id = Int32.Parse(dr["ID"].ToString());
                    string doi = Convert.ToString(dr["DOI"].ToString());
                    try
                    {
                        PubScreen paper =  await GetPaperInfoByDOICrossref(doi);
                        if (paper != null)
                        {
                            EditPublication(id, paper, userName);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            //var reader = new StreamReader("TSPapers.csv");

            //while (!reader.EndOfStream)
            //{
            //    var line = reader.ReadLine();
            //    Console.WriteLine(line);
            //    var values = line.Split(',');

            //    string doi = values[0].Trim().TrimEnd('.').Split(' ')[0].TrimEnd('.');
            //    string pubmedID = values[1].Trim();

            //    int? ID = null;
            //    // Check for duplication based on the DOI

            //    if (!string.IsNullOrEmpty(doi))
            //    {
            //        string sqlDOI = $@"Select ID From Publication Where DOI = '{doi}'";
            //        ID = (int?)Dal.ExecScalarPub(sqlDOI);
            //    }

            //    if (ID == null)
            //    {
            //        int result;
            //        bool pubmedSuccess = false;
            //        var paper = new PubScreen();

            //        if (Int32.TryParse(pubmedID, out result))
            //        {
            //            try
            //            {
            //                paper = await GetPaperInfoByPubMedKey(pubmedID);
            //                pubmedSuccess = true;
            //            }
            //            catch
            //            {

            //            }
            //        }

            //        if (!pubmedSuccess)
            //        {
            //            if (!string.IsNullOrEmpty(doi))
            //            {
            //                // Attempt to get read paper based on DOI and Pubmed
            //                paper = await GetPaperInfoByDoi(doi);
            //                if (paper == null)
            //                {
            //                    // Attempt to read paper based on bioRxiv
            //                    paper = await GetPaperInfoByDOIBIO(doi);

            //                    if (paper == null)
            //                    {
            //                        // If all else fails, just add DOI to database
            //                        paper = new PubScreen { DOI = doi, };
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                paper = null;
            //            }
            //        }

            //        if (paper != null)
            //        {
            //            if (paper.DOI == "")
            //            {
            //                paper.DOI = doi;
            //            }
            //            AddPublications(paper, userName);
            //            doiList.Add(doi);
            //        }
            //    }

            //}

            return doiList;
        }

        // Function definition to get a publication from searchPub based Guid
        public PubScreenSearch GetDataFromPubScreenByLinkGuid(Guid paperLinkGuid)
        {
            PubScreenSearch pubScreenPublication = new PubScreenSearch();

            string sql = $"Select * From SearchPub Where PaperLinkGuid = '{paperLinkGuid}'";

            using (IDataReader dr = Dal.GetReaderPub(CommandType.Text, sql, null))
            {
                string sqlMB = "";
                string sqlCog = "";
                List<Experiment> lstExperiment = new List<Experiment>();
                List<Cogbytes> lstRepo = new List<Cogbytes>();

                if (dr.Read())
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

                    sqlCog = $"Select * From UserRepository Where DOI = '{Convert.ToString(dr["DOI"].ToString())}'";
                    lstRepo.Clear();
                    using (DataTable dtCog = Dal.GetDataTableCog(sqlCog))
                    {
                        foreach (DataRow drCog in dtCog.Rows)
                        {
                            var cogbytesService = new CogbytesService();
                            int repID = Int32.Parse(drCog["RepID"].ToString());
                            lstRepo.Add(new Cogbytes
                            {
                                ID = repID,
                                RepoLinkGuid = Guid.Parse(drCog["repoLinkGuid"].ToString()),
                                Title = Convert.ToString(drCog["Title"].ToString()),
                                Date = Convert.ToString(drCog["Date"].ToString()),
                                Keywords = Convert.ToString(drCog["Keywords"].ToString()),
                                DOI = Convert.ToString(drCog["DOI"].ToString()),
                                Link = Convert.ToString(drCog["Link"].ToString()),
                                PrivacyStatus = Boolean.Parse(drCog["PrivacyStatus"].ToString()),
                                Description = Convert.ToString(drCog["Description"].ToString()),
                                AdditionalNotes = Convert.ToString(drCog["AdditionalNotes"].ToString()),
                                AuthourID = cogbytesService.FillCogbytesItemArray($"Select AuthorID From RepAuthor Where RepID={repID}", "AuthorID"),
                                PIID = cogbytesService.FillCogbytesItemArray($"Select PIID From RepPI Where RepID={repID}", "PIID"),
                            });
                        }

                    }
                    pubScreenPublication.DOI = Convert.ToString(dr["DOI"]);
                    pubScreenPublication.Keywords = Convert.ToString(dr["Keywords"]);
                    pubScreenPublication.Title = Convert.ToString(dr["Title"]);
                    pubScreenPublication.Abstract = Convert.ToString(dr["Abstract"]);
                    pubScreenPublication.Year = Convert.ToString(dr["Year"]);
                    pubScreenPublication.Reference = Convert.ToString(dr["Reference"]);
                    pubScreenPublication.ID = Int32.Parse(dr["ID"].ToString());
                    pubScreenPublication.Author = Convert.ToString(dr["Author"].ToString());
                    pubScreenPublication.PaperType = Convert.ToString(dr["PaperType"].ToString());
                    pubScreenPublication.Task = Convert.ToString(dr["Task"].ToString());
                    pubScreenPublication.SubTask = Convert.ToString(dr["SubTask"].ToString());
                    pubScreenPublication.Species = Convert.ToString(dr["Species"].ToString());
                    pubScreenPublication.Sex = Convert.ToString(dr["Sex"].ToString());
                    pubScreenPublication.Strain = Convert.ToString(dr["Strain"].ToString());
                    pubScreenPublication.DiseaseModel = Convert.ToString(dr["DiseaseModel"].ToString());
                    pubScreenPublication.SubModel = Convert.ToString(dr["SubModel"].ToString());
                    pubScreenPublication.BrainRegion = Convert.ToString(dr["BrainRegion"].ToString());
                    pubScreenPublication.SubRegion = Convert.ToString(dr["SubRegion"].ToString());
                    pubScreenPublication.CellType = Convert.ToString(dr["CellType"].ToString());
                    pubScreenPublication.Method = Convert.ToString(dr["Method"].ToString());
                    pubScreenPublication.NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString());
                    pubScreenPublication.Experiment = new List<Experiment>(lstExperiment);
                    pubScreenPublication.Repo = new List<Cogbytes>(lstRepo);

                }

            }

            return pubScreenPublication;
        }

        public PubScreen GetGuidByDoi(string doi)
        {
            PubScreen pubScreenPublication = new PubScreen();
            string sql = $"Select * From Publication Where DOI = '{doi}' ";
            using (IDataReader dr = Dal.GetReaderPub(CommandType.Text, sql, null))
            {
                if (dr.Read())
                {
                    pubScreenPublication.PaperLinkGuid = Guid.Parse(dr["PaperLinkGuid"].ToString());

                }

            }

            return pubScreenPublication;
        }
    }

}
