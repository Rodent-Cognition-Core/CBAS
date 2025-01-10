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
using System.Net;
using Nest;
using IdentityServer4.Models;
using CBAS.Models;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using Elasticsearch.Net;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using Serilog;

namespace AngularSPAWebAPI.Services
{

    public class PubScreenService
    {
        const int MOUSEID = 2;
        const int RATID = 1;
        private readonly HttpClient httpClient;
        private static string[] MULTISEARCHFIELDS = { "title", "keywords", "author", "abstract" };
        private static HashSet<string> MULTISELCETFIELD = new HashSet<string> {"Author", "Task", "SubTask", "PaperType", "Species", "Sex", "Strain", "DiseaseModel", "SubModel", "BrainRegion", "SubRegion", "CellType", "Method", "SubMethod", "NeuroTransmitter", };
        private const int SEARCHRESULTSIZE = 10000;
        private readonly IElasticClient _elasticClient;
        // Function Definition to get paper info from DOI
        // private static readonly HttpClient client = new HttpClient();
        public PubScreenService(IElasticClient client){
            var PROXY_ADDR = Environment.GetEnvironmentVariable("PROXY_ADDR");
            HttpClientHandler handler = new HttpClientHandler()
            {
                //Proxy = new WebProxy(PROXY_ADDR, 8080),
                //UseProxy = true
            };
            httpClient = new HttpClient(handler);
            _elasticClient = client;
        }

        public async Task<PubScreen> GetPaperInfoByDoi(string doi)
        {
            // Submit doi to get the pubmedkey
            if (doi.StartsWith("https://doi.org/", StringComparison.OrdinalIgnoreCase))
            {
                doi = doi.Substring("https://doi.org/".Length);
            }

            StringContent content = new StringContent(string.Empty);
            string responseString = "";
            var incomingXml = new XElement("newXML");
            bool isSuccess = false;
            int retryCount = 0;
            const int maxRetries = 10;
            const int delayMilliseconds = 300;

            while (!isSuccess && retryCount < maxRetries)
            {
                try
                {
                    var response = await httpClient.PostAsync($"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&WebEnv=1&usehistory=y&term={doi}&rettype=Id", content);
                    responseString = await response.Content.ReadAsStringAsync();
                    if (responseString.Contains("<OutputMessage>No items found.</OutputMessage>", StringComparison.OrdinalIgnoreCase) ||
                        responseString.Contains("<ErrorList>", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                    incomingXml = XElement.Parse(responseString);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Exception: {ex.Message} responseString: {responseString}");
                    if (responseString.Contains("API rate limit exceeded", StringComparison.OrdinalIgnoreCase))
                    {
                        retryCount++;
                        await Task.Delay(delayMilliseconds);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (!isSuccess)
            {
                throw new Exception("Failed to retrieve data from GetPaperInfoByDoi PubMed API after multiple attempts.");
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
            // HttpClient httpClient = new HttpClient();

            var content = new StringContent(String.Empty, Encoding.UTF8, "application/xml");
            var incomingXml = new XElement("newXML");
            string responseString = "";

            // Pubmed API is rate-limited. If the rate limit is exceeded, the response string from the HTTP Post will indicate that.
            // When that is the case, we wait 0.3s and attempt again, until we obtain our desired result or another error occurs
            bool isSuccess = false;
            int retryCount = 0;
            const int maxRetries = 10;
            const int delayMilliseconds = 300;
            while (!isSuccess && retryCount < maxRetries)
            {
                try
                {
                    var response = await httpClient.PostAsync($"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id={pubMedKey}&retmode=xml", content);
                    responseString = await response.Content.ReadAsStringAsync();
                    incomingXml = XElement.Parse(responseString);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Exception: {ex.Message} responseString: {responseString}");
                    if (responseString.Contains("API rate limit exceeded"))
                    {
                        retryCount++;
                        await Task.Delay(delayMilliseconds);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (!isSuccess)
            {
                throw new Exception("Failed to retrieve data from GetPaperInfoByPubMedKey PubMed API after multiple attempts.");
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
            catch (Exception ex)
            {
                Log.Error(ex.Message);
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
            catch (Exception ex)
            {
                Log.Error(ex.Message);
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
                DOI = doi
            };

            // Actually two outputs should be returned (authorList should be also returned)

            return pubscreenObj;
        }

        //Function Definition to get some paper's info based on DOI from BioRxiv
        public async Task<PubScreen> GetPaperInfoByDOIBIO(string doi)
        {
            // Submiy doi to get the pubmedkey

            // HttpClient httpClient = new HttpClient();

            StringContent content = new System.Net.Http.StringContent(String.Empty);
            try
            {
                var response = await httpClient.PostAsync("https://api.biorxiv.org/details/biorxiv/" + doi + "/na/json", content);
                var responseString = await response.Content.ReadAsStringAsync();
                Log.Information($"responseString: {responseString}");
                JsonPubscreen jsonPubscreen = JsonConvert.DeserializeObject<JsonPubscreen>(responseString);
                string jsonPubscreenString = JsonConvert.SerializeObject(jsonPubscreen, Newtonsoft.Json.Formatting.Indented);
                Log.Information($"jsonPubscreen: {jsonPubscreenString}");
                if (jsonPubscreen.messages != null && jsonPubscreen.messages.Length > 0)
                {
                    foreach (var message in jsonPubscreen.messages)
                    {
                        Log.Warning($"jsonPubscreen message status: {message.status}");
                    }
                }
                if (jsonPubscreen.collection == null || jsonPubscreen.collection.Length == 0)
                {
                    Log.Warning($"jsonPubscreen.collection is null");
                    return null;
                }
                else
                {
                    foreach (var jsonPubscreenFeature in jsonPubscreen.collection)
                    {
                        Log.Information($"author_corresponding: {jsonPubscreenFeature.author_corresponding} - authors: {jsonPubscreenFeature.authors} - year: {jsonPubscreenFeature.date}");
                    }
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
                        LastName = name.Split(',')[0].Trim()
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
                    DOI = doi
                };

                return pubscreenObj;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetPaperInfoByDOIBIO");
            }
            return null;
        }

        //Function Definition to get some paper's info based on PubMedKey
        public async Task<PubScreen> GetPaperInfoByDOICrossref(string doi)
        {
            // if IskeyPumbed is true then get doi and add it to object
            // HttpClient httpClient = new HttpClient();

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
                catch (Exception ex)
                {
                    // log responseString and exception message
                    Log.Error($"responseString: {responseString}, exception message: {ex.Message}");

                    if (responseString.IndexOf("API rate limit exceeded") == -1)
                    {
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
            bool isBook = false;

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
            else if (incomingXml.Element("doi_record").Element("crossref").Element("book") != null)
            {
                crossrefElement = incomingXml.Element("doi_record").Element("crossref").Element("book").Element("book_series_metadata");
                isBook = true;
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
                var tempCrossRefElement = crossrefElement;
                if(isBook)
                {
                    crossrefElement = incomingXml.Element("doi_record").Element("crossref").Element("book").Element("content_item");
                }
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
                crossrefElement = tempCrossRefElement;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
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
                DOI = doi
            };

            // Actually two outputs should be returned (authorList should be also returned)

            return pubscreenObj;


        }

        public async Task<List<PubScreenPaperType>> GetPaperTypesAsync()
        {
            var paperTypeList = new List<PubScreenPaperType>();
            string query = "SELECT * FROM PaperType";

            try
            {
                using (var dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        paperTypeList.Add(new PubScreenPaperType
                        {
                            PaperType = dr["PaperType"].ToString(),
                            ID = int.Parse(dr["ID"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting paper types");
            }

            return paperTypeList;
        }

        public async Task<List<PubScreenTask>> GetTasksAsync()
        {
            var taskList = new List<PubScreenTask>();
            string query = @"SELECT * FROM Task ORDER BY (CASE WHEN Task NOT LIKE '%None%' THEN 1 ELSE 2 END), Task";

            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        taskList.Add(new PubScreenTask
                        {
                            ID = int.Parse(dr["ID"].ToString()),
                            Task = dr["Task"].ToString()
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while getting tasks.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while getting tasks.");
            }

            return taskList;
        }

        public async Task<List<PubScreenTask>> GetAllTasksAsync()
        {
            var taskList = new List<PubScreenTask>();
            string query = @"SELECT SubTask.ID, SubTask.TaskID, Task.Task, SubTask.SubTask 
                     FROM SubTask
                     INNER JOIN Task ON Task.ID = SubTask.TaskID
                     ORDER BY (CASE WHEN SubTask NOT LIKE '%None%' THEN 1 ELSE 2 END), Task, SubTask";

            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        taskList.Add(new PubScreenTask
                        {
                            ID = int.Parse(dr["ID"].ToString()),
                            TaskID = int.Parse(dr["TaskID"].ToString()),
                            Task = dr["Task"].ToString(),
                            SubTask = dr["SubTask"].ToString()
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while getting all tasks.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while getting all tasks.");
            }

            return taskList;
        }

        public async Task<List<PubScreenSpecie>> GetSpeciesAsync()
        {
            var specieList = new List<PubScreenSpecie>();
            string query = @"SELECT * FROM Species 
                     ORDER BY (CASE WHEN Species NOT LIKE '%Other%' AND Species <> LTRIM(RTRIM('none')) THEN 1 ELSE 2 END), Species";

            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        specieList.Add(new PubScreenSpecie
                        {
                            ID = int.Parse(dr["ID"].ToString()),
                            Species = dr["Species"].ToString()
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while getting species.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while getting species.");
            }

            return specieList;
        }

        public async Task<List<PubScreenSex>> GetSexAsync()
        {
            var sexList = new List<PubScreenSex>();
            string query = @"SELECT * FROM Sex ORDER BY Sex";

            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        sexList.Add(new PubScreenSex
                        {
                            ID = int.Parse(dr["ID"].ToString()),
                            Sex = dr["Sex"].ToString()
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while getting sex.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while getting sex.");
            }

            return sexList;
        }

        public async Task<List<PubScreenStrain>> GetStrainsAsync()
        {
            var strainList = new List<PubScreenStrain>();
            string query = @"SELECT Strain.ID, Strain, SpeciesID
                     FROM Strain 
                     INNER JOIN Species ON Strain.SpeciesID = Species.ID
                     ORDER BY (CASE WHEN Strain NOT LIKE '%Other%' AND Strain <> LTRIM(RTRIM('none')) THEN 1 ELSE 2 END), Species, Strain";

            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        strainList.Add(new PubScreenStrain
                        {
                            ID = int.Parse(dr["ID"].ToString()),
                            Strain = dr["Strain"].ToString(),
                            SpeciesID = int.Parse(dr["SpeciesID"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while getting strains.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while getting strains.");
            }

            return strainList;
        }

        public async Task<List<PubScreenDisease>> GetDiseaseAsync()
        {
            var diseaseList = new List<PubScreenDisease>();
            string query = @"SELECT * FROM DiseaseModel 
                     ORDER BY (CASE WHEN DiseaseModel NOT LIKE '%Other%' AND DiseaseModel <> LTRIM(RTRIM('none')) THEN 1 ELSE 2 END), DiseaseModel";

            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        diseaseList.Add(new PubScreenDisease
                        {
                            ID = int.Parse(dr["ID"].ToString()),
                            DiseaseModel = dr["DiseaseModel"].ToString()
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while getting diseases.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while getting diseases.");
            }

            return diseaseList;
        }

        public async Task<List<PubScreenSubModel>> GetSubModelsAsync()
        {
            var subModelList = new List<PubScreenSubModel>();
            string query = @"SELECT SubModel.ID, SubModel, ModelID
                     FROM SubModel 
                     INNER JOIN DiseaseModel ON SubModel.ModelID = DiseaseModel.ID
                     ORDER BY (CASE WHEN SubModel NOT LIKE '%Other%' AND SubModel <> LTRIM(RTRIM('none')) THEN 1 ELSE 2 END), DiseaseModel, SubModel";

            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        subModelList.Add(new PubScreenSubModel
                        {
                            ID = int.Parse(dr["ID"].ToString()),
                            SubModel = dr["SubModel"].ToString(),
                            ModelID = int.Parse(dr["ModelID"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while getting submodels.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while getting submodels.");
            }

            return subModelList;
        }

        public async Task<List<PubScreenRegion>> GetAllRegionsAsync()
        {
            var regionList = new List<PubScreenRegion>();
            var query = $@"Select SubRegion.ID, SubRegion.RID, BrainRegion.BrainRegion, SubRegion.SubRegion 
                    From SubRegion
                    Inner join BrainRegion on BrainRegion.ID = SubRegion.RID
                    Order By (Case When SubRegion not like '%Other%'  and  SubRegion <> ltrim(rtrim('none'))  Then 1 Else 2 End), RID, SubRegion";
            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        regionList.Add(new PubScreenRegion
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            RID = Int32.Parse(dr["RID"].ToString()),
                            BrainRegion = Convert.ToString(dr["BrainRegion"].ToString()),
                            SubRegion = Convert.ToString(dr["SubRegion"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while fetching all regions.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while fetching all regions.");
                throw;
            }

            return regionList;
        }

        public async Task<List<PubScreenRegion>> GetRegionsAsync()
        {
            var regionList = new List<PubScreenRegion>();
            var query = $@"Select * From BrainRegion Order By (Case When BrainRegion not like '%Other%'  and  BrainRegion <> ltrim(rtrim('none'))  Then 1 Else 2 End), BrainRegion";
            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        regionList.Add(new PubScreenRegion
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            BrainRegion = Convert.ToString(dr["BrainRegion"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while fetching regions.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while fetching regions.");
                throw;
            }

            return regionList;
        }

        public async Task<List<PubScreenCellType>> GetCellTypesAsync()
        {
            var cellTypeList = new List<PubScreenCellType>();
            string query = @"Select * From CellType Order By (Case When CellType not like '%Other%'  and  CellType <> ltrim(rtrim('none'))  Then 1 Else 2 End), CellType";
            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        cellTypeList.Add(new PubScreenCellType
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            CellType = Convert.ToString(dr["CellType"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while fetching cell types.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while fetching cell types.");
                throw;
            }

            return cellTypeList;
        }

        public async Task<List<PubScreenMethod>> GetMethodsAsync()
        {
            var methodList = new List<PubScreenMethod>();
            string query = @"Select * From Method Order By (Case When Method not like '%Other%'   and  Method <> ltrim(rtrim('none'))  Then 1 Else 2 End), Method";
            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        methodList.Add(new PubScreenMethod
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            Method = Convert.ToString(dr["Method"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while fetching methods.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while fetching methods.");
                throw;
            }

            return methodList;
        }

        public async Task<List<PubScreenSubMethod>> GetSubMethodsAsync()
        {
            var subMethodList = new List<PubScreenSubMethod>();
            string query = @"Select SubMethod.ID, SubMethod, MethodID
                     From SubMethod Inner Join Method On SubMethod.MethodID = Method.ID
                     Order By (Case When SubMethod not like '%Other%'  and  SubMethod <> ltrim(rtrim('none'))  Then 1 Else 2 End), Method, SubMethod";
            try
            {
                using (DataTable dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        subMethodList.Add(new PubScreenSubMethod
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            SubMethod = Convert.ToString(dr["SubMethod"].ToString()),
                            MethodID = Int32.Parse(dr["MethodID"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while fetching sub-methods.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while fetching sub-methods.");
                throw;
            }

            return subMethodList;
        }

        public async Task<List<PubScreenNeuroTransmitter>> GetNeurotransmittersAsync()
        {
            var neuroTransmitterList = new List<PubScreenNeuroTransmitter>();
            string query = @"Select * From NeuroTransmitter Order By 
                     (Case When NeuroTransmitter not like '%Other%' and NeuroTransmitter <> ltrim(rtrim('none')) Then 1 Else 2 End), 
                     NeuroTransmitter";
            try
            {
                using (DataTable dt = await Task.Run(() => Dal.GetDataTablePub(query)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        neuroTransmitterList.Add(new PubScreenNeuroTransmitter
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while fetching neurotransmitters.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while fetching neurotransmitters.");
                throw;
            }

            return neuroTransmitterList;
        }

        public async Task<List<PubScreenAuthor>> GetAuthorsAsync()
        {
            var authorList = new List<PubScreenAuthor>();
            string query = "Select * From Author Order By LastName";
            try
            {
                using (DataTable dt = await Task.Run(() => Dal.GetDataTablePub(query)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        authorList.Add(new PubScreenAuthor
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            FirstName = Convert.ToString(dr["FirstName"].ToString()),
                            LastName = Convert.ToString(dr["LastName"].ToString()),
                            Affiliation = Convert.ToString(dr["Affiliation"].ToString())
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while fetching authors.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while fetching authors.");
                throw;
            }

            return authorList;
        }

        public async Task<int> AddAuthorsAsync(PubScreenAuthor author, string userEmail)
        {
            string sql = @"Insert into Author (FirstName, LastName, Affiliation, username) 
                   Values (@FirstName, @LastName, @Affiliation, @Username); 
                   SELECT CAST(scope_identity() AS int);";
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FirstName", author.FirstName),
                    new SqlParameter("@LastName", author.LastName),
                    new SqlParameter("@Affiliation", author.Affiliation),
                    new SqlParameter("@Username", userEmail)
                };

                var result = await Dal.ExecScalarPubAsync(sql, parameters);
                return (int)result;
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while adding author: {Author}", author);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while adding author: {Author}", author);
                throw;
            }
        }

        public async Task DeletePublicationByIdAsync(int pubId)
        {
            string sql = @"
                DELETE FROM Publication_Author WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_CellType WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_Disease WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_SubModel WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_Method WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_SubMethod WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_NeuroTransmitter WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_PaperType WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_Region WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_Sex WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_Specie WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_Strain WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_SubRegion WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_Task WHERE PublicationID = @PublicationID;
                DELETE FROM Publication_SubTask WHERE PublicationID = @PublicationID;
                DELETE FROM EditLog WHERE PubID = @PublicationID;
                DELETE FROM Publication WHERE id = @PublicationID;";

            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@PublicationID", pubId)
                };

                await Dal.ExecuteNonQueryAsync(sql, parameters);
                await DeleteFromElasticSearchAsync(pubId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting publication with ID {PublicationID}", pubId);
            }
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
                return (int)ID;
            }

            var guid = Guid.NewGuid();
            string sqlPublication = $@"Insert into Publication (PaperLinkGuid, Title, Abstract, Keywords, DOI, Year, Reference, Username, Source) Values
                                    ('{guid}',
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

            //Adding to Publication_SubModel
            if (publication.SubMethodID != null && publication.SubMethodID.Length != 0)
            {
                string sqlSubMethod = "";
                for (int i = 0; i < publication.SubMethodID.Length; i++)
                {
                    sqlSubMethod += $@"Insert into Publication_SubMethod (SubMethodID, PublicationID) Values ({publication.SubMethodID[i]}, {PublicationID});";

                }
                if (sqlSubMethod != "") { Dal.ExecuteNonQueryPub(sqlSubMethod); };

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
            publication.ID = PublicationID;
            publication.PaperLinkGuid = guid;
            AddPublicationsToElasticSearch(publication);
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
                    origPub.SubMethod = Convert.ToString(dr["SubMethod"].ToString());
                    origPub.NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString());
                    origPub.Reference = Convert.ToString(dr["Reference"].ToString());
                }
            };

            string sqlPublication = $@"Update Publication set Title = @title, Abstract = @abstract, Keywords = @keywords,
                                                               DOI = @doi, Year = @year where id = {publicationId}";

            var parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("@title", HelperService.NullToString(HelperService.EscapeSql(publication.Title)).Trim()));
            //parameters.Add(new SqlParameter("@abstract", HelperService.NullToString(HelperService.EscapeSql(publication.Abstract)).Trim()));
            //parameters.Add(new SqlParameter("@keywords", HelperService.NullToString(HelperService.EscapeSql(publication.Keywords)).Trim()));
            //parameters.Add(new SqlParameter("@doi", HelperService.NullToString(HelperService.EscapeSql(publication.DOI)).Trim()));
            //parameters.Add(new SqlParameter("@year", HelperService.NullToString(HelperService.EscapeSql(publication.Year)).Trim()));

            parameters.Add(new SqlParameter("@title", HelperService.NullToString(publication.Title).Trim()));
            parameters.Add(new SqlParameter("@abstract", HelperService.NullToString(publication.Abstract).Trim()));
            parameters.Add(new SqlParameter("@keywords", HelperService.NullToString(publication.Keywords).Trim()));
            parameters.Add(new SqlParameter("@doi", HelperService.NullToString(publication.DOI).Trim()));
            parameters.Add(new SqlParameter("@year", HelperService.NullToString(publication.Year).Trim()));

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

            //Editing to Publication_SubMethod
            sqlDelete = $"DELETE From Publication_SubMethod where PublicationID = {publicationId}";
            Dal.ExecuteNonQueryPub(sqlDelete);
            if (publication.SubMethodID != null && publication.SubMethodID.Length != 0)
            {
                string sqlSubMethod = "";
                for (int i = 0; i < publication.SubMethodID.Length; i++)
                {
                    sqlSubMethod += $@"Insert into Publication_SubMethod (SubMethodID, PublicationID) Values ({publication.SubMethodID[i]}, {publicationId});";

                }
                if (sqlSubMethod != "") { Dal.ExecuteNonQueryPub(sqlSubMethod); };

            }

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
                    newPub.SubMethod = Convert.ToString(dr["SubMethod"].ToString());
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

            UpdatePublicationToElasticSearch(newPub.ID, publication);
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
            try
            {
                string sql = "Select * From SearchPub Where ";

                if (!string.IsNullOrEmpty(pubScreen.search))
                {
                    sql += $@"(SearchPub.Title like '%{(HelperService.EscapeSql(pubScreen.search)).Trim()}%' or
                           SearchPub.Keywords like '%{(HelperService.EscapeSql(pubScreen.search)).Trim()}%' or
                           SearchPub.Author like '%{(HelperService.EscapeSql(pubScreen.search)).Trim()}%') AND ";
                }
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

                // search query for Sub Method
                if (pubScreen.SubMethodID != null && pubScreen.SubMethodID.Length != 0)
                {
                    if (pubScreen.SubMethodID.Length == 1)
                    {
                        sql += $@"SearchPub.SubMethod like '%'  + (Select SubMethod From SubMethod Where SubMethod.ID = {pubScreen.SubMethodID[0]}) +  '%' AND ";

                    }
                    else
                    {
                        sql += "(";
                        for (int i = 0; i < pubScreen.SubMethodID.Length; i++)
                        {
                            sql += $@"SearchPub.SubMethod like '%'  + (Select SubMethod From SubMethod Where SubMethod.ID = {pubScreen.SubMethodID[i]}) +  '%' OR ";

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
                        string doi = Convert.ToString(dr["DOI"].ToString());
                        if (String.IsNullOrEmpty(doi) == false)
                        {
                            sqlMB = $@"Select Experiment.*, Task.Name as TaskName From Experiment
                                   Inner join Task on Task.ID = Experiment.TaskID
                                   Where DOI = '{doi}'";

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

                            sqlCog = $"Select * From UserRepository Where DOI = '{doi}'";
                            lstRepo.Clear();
                            using (DataTable dtCog = Dal.GetDataTableCog(sqlCog))
                            {
                                var cogbytesService = new CogbytesService();
                                foreach (DataRow drCog in dtCog.Rows)
                                {
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
                            SubMethod = Convert.ToString(dr["SubMethod"].ToString()),
                            NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString()),
                            Reference = Convert.ToString(dr["Reference"].ToString()),
                            Experiment = new List<Experiment>(lstExperiment),
                            Repo = new List<Cogbytes>(lstRepo)

                        });
                        //lstExperiment.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in SearchPublications");
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

        public async Task<PubScreen> GetPaperInfoByIDAsync(int id)
        {
            var pubScreen = new PubScreen();
            try
            {
                pubScreen = await Dal.GetPaperInfoByIDAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetPaperInfoByIDAsync");
                throw;
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

        public async Task<List<PubmedPaper>> GetPubmedQueueAsync()
        {
            var pubmedQueue = new List<PubmedPaper>();
            const string query = "SELECT * FROM PubmedQueue WHERE IsProcessed = 0 ORDER BY QueueDate, PubDate";

            try
            {
                using (var dt = await Dal.GetDataTablePubAsync(query))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        pubmedQueue.Add(new PubmedPaper
                        {
                            Title = dr["Title"].ToString(),
                            PubmedID = int.Parse(dr["PubmedID"].ToString()),
                            PubDate = dr["PubDate"].ToString(),
                            QueueDate = dr["QueueDate"].ToString(),
                            DOI = dr["DOI"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetPubmedQueue");
                throw;
            }

            return pubmedQueue;
        }

        public async Task<int?> AddQueuePaper(int pubmedID, string doi, string userName)
        {
            try
            {
                PubScreen paper;
                if (pubmedID == -1)
                {
                    paper = await GetPaperInfoByDOICrossref(doi);
                }
                else
                {
                    paper = await GetPaperInfoByPubMedKey(pubmedID.ToString());
                }

                if (paper == null)
                {
                    Log.Warning("No paper found for PubMed ID: {PubMedID} or DOI: {DOI}", pubmedID, doi);
                    return null;
                }

                paper.DOI = doi;
                int? pubID = AddPublications(paper, userName);
                await ProcessQueuePaperAsync(pubmedID, doi);

                return pubID;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in AddQueuePaper for PubMed ID: {PubMedID} or DOI: {DOI}", pubmedID, doi);
                throw;
            }
        }

        public async Task ProcessQueuePaperAsync(int pubmedID, string doi = null)
        {
            try
            {
                string query;
                SqlParameter[] parameters;

                if (pubmedID == -1)
                {
                    query = "UPDATE PubmedQueue SET IsProcessed = 1 WHERE DOI = @doi";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@doi", SqlDbType.NVarChar) { Value = doi }
                    };
                }
                else
                {
                    query = "UPDATE PubmedQueue SET IsProcessed = 1 WHERE PubmedID = @pubmedID";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@pubmedID", SqlDbType.Int) { Value = pubmedID }
                    };
                }

                await Dal.ExecuteNonQueryPubAsync(query, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ProcessQueuePaperAsync");
                throw;
            }
        }

        public async Task<(int, int)> GetPubCountAsync()
        {
            const string pubCountQuery = "SELECT COUNT(ID) FROM SearchPub";
            const string featureCountQuery = @"
                SELECT COUNT(*) 
                FROM SearchPub 
                WHERE species IS NOT NULL 
                   OR sex IS NOT NULL 
                   OR Strain IS NOT NULL 
                   OR DiseaseModel IS NOT NULL 
                   OR BrainRegion IS NOT NULL 
                   OR CellType IS NOT NULL 
                   OR method IS NOT NULL 
                   OR Neurotransmitter IS NOT NULL 
                   OR task IS NOT NULL";

            try
            {
                var pubCountTask = await Dal.ExecScalarPubAsync(pubCountQuery, null);
                var featureCountTask = await Dal.ExecScalarPubAsync(featureCountQuery, null);

                return ((int)pubCountTask, (int)featureCountTask);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetPubCountAsync");
                throw;
            }
        }

        //public async Task<List<string>> AddCSVPapers(string userName)
        public bool AddCSVPapers(string userName)
        {
            List<string> doiList = new List<string>();

            // THIS SECTION: REMOVE EXTRA APOSTROPHES
            using (DataTable dt = Dal.GetDataTablePub($@"Select ID, Title, Abstract, Keywords From Publication Where Title like '%''''%' or Abstract like '%''''%' or Keywords like '%''''%'"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int id = Int32.Parse(dr["ID"].ToString());
                    string oldTitle = Convert.ToString(dr["Title"].ToString());
                    string oldAbstract = Convert.ToString(dr["Abstract"].ToString());
                    string oldKeywords = Convert.ToString(dr["Keywords"].ToString());

                    string newTitle = removeApostrophes(oldTitle);
                    string newAbstract = removeApostrophes(oldAbstract);
                    string newKeywords = removeApostrophes(oldKeywords);

                    string sql = $"Update Publication Set Title = '{newTitle}', Abstract = '{newAbstract}', Keywords = '{newKeywords}' Where ID = {id}";
                    Dal.ExecuteNonQueryPub(sql);

                    string removeApostrophes(string input)
                    {
                        while (input.Contains("''"))
                        {
                            input = input.Replace("''", "'");
                        }
                        return HelperService.EscapeSql(input);
                    }
                }
            }

            // THIS SECTION: ADD CROSSREF DOI'S FROM PAPERS WITH EMPTY ENTRIES
            //List<string> doiList = new List<string>();

            //using (DataTable dt = Dal.GetDataTablePub($@"Select * From Publication Where Title like ''"))
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        int id = Int32.Parse(dr["ID"].ToString());
            //        string doi = Convert.ToString(dr["DOI"].ToString());
            //        try
            //        {
            //            PubScreen paper =  await GetPaperInfoByDOICrossref(doi);
            //            if (paper != null)
            //            {
            //                EditPublication(id, paper, userName);
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }
            //}

            // THIS SECTION: ADD PAPERS FROM CSV LIST
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

            //return doiList;
            return true;
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
                    pubScreenPublication.SubMethod = Convert.ToString(dr["SubMethod"].ToString());
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
        private QueryContainer ApplyQuery(PubScreenElasticSearchModel pubscreen, QueryContainerDescriptor<PubScreenElasticSearchModel> query)
        {
            return query.Bool(boolQ => boolQ.Should(boolQM => boolQM
                                                .DisMax(dxq => dxq
                                                .Queries(JoinAllQuries(pubscreen, query).ToArray())))
                                            .Filter(AddFilter(pubscreen).ToArray()));
        }
        private List<QueryContainer> JoinAllQuries(PubScreenElasticSearchModel pubscreen, QueryContainerDescriptor<PubScreenElasticSearchModel> query)
        {
            var container = new List<QueryContainer>();


            var multiMatchQuery = MultiMatchSearchField(pubscreen, query);
            if (multiMatchQuery.Count > 0)
            {
                foreach (var multiQuery in multiMatchQuery)
                {
                    container.Add(multiQuery);
                }
            }
            return container;
        }

        private List<Func<QueryContainerDescriptor<PubScreenElasticSearchModel>, QueryContainer>> AddFilter(PubScreenElasticSearchModel pubscreen)
        {
            var filterQuery = new List<Func<QueryContainerDescriptor<PubScreenElasticSearchModel>, QueryContainer>>();
            foreach (PropertyInfo pi in pubscreen.GetType().GetProperties())
            {
                if (pi.Name.Equals("search") || pi.Name.Equals("ID") || pi.Name.Equals("PaperLinkGuid"))
                {
                    continue;
                }


                string value = "";

                Array listOfValue = null;

                if (pi.PropertyType == typeof(string[]))
                {
                    listOfValue = (Array)pi.GetValue(pubscreen);
                }
                else
                {
                    value = Convert.ToString(pi.GetValue(pubscreen, null));
                }

                if ((string.IsNullOrEmpty(value) || value.Equals("0")) && listOfValue == null)
                {
                    continue;
                }

                if (pi.Name == "YearFrom")
                {
                    double yearFrom = 0;
                    if (double.TryParse(value, out yearFrom))
                    {
                        filterQuery.Add(fq => fq
                        .Range(range => range
                            .Field(f => f.Year)
                        .GreaterThanOrEquals(yearFrom))
                        );
                    }
                    else
                    {
                        Console.WriteLine("Unable to parse Year From '{0}'", value);
                    }

                }

                else if (pi.Name == "YearTo")
                {
                    double yearTo = 0;
                    if (double.TryParse(value, out yearTo))
                    {
                        filterQuery.Add(fq => fq
                        .Range(range => range
                            .Field(f => f.Year)
                            .LessThanOrEquals(yearTo))
                        );
                    }
                    else
                    {
                        Console.WriteLine("Unable to parse Year To '{0}", value);
                    }
                }
                else if (pi.Name == "DOI")
                {
                    filterQuery.Add(fq => fq
                                .Bool(f => f
                                    .Must(boolSHould => boolSHould
                                        .Term(t => t
                                            .Field(feild => feild.DOI.Suffix("keyword"))
                                             .Value(value)))));
                }

                else if (MULTISELCETFIELD.Contains(pi.Name))
                {
                    var listOfItem = HelperService.ConvertArrayToStringList(listOfValue);
                    filterQuery.Add(fq => fq
                        .Bool(f => f
                        .Should(listOfItem.Select(item => ApplyORQuery(fq, pi.Name, item)).ToArray())));
                }

                else if (pi.PropertyType == typeof(string))
                {
                    filterQuery.Add(fq => fq
                            .Bool(dxqm => dxqm
                                 .Must(boolShould => boolShould
                               .MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
                               .Field(new Nest.Field(pi.Name).ToString().ToLower())
                               .Query(value.ToString().ToLower())))));
                }
                else
                {
                    filterQuery.Add(fq => fq.Terms(t => t.Field(new Nest.Field(pi.Name)).Terms(value)));
                }
            }
            return filterQuery;
        }

        public List<PubScreenElasticSearchModel> ElasticSearchPublications(PubScreen pubScreen)
        {
            PubScreenElasticSearchModel pubScreenForElasticSearch = ConvertToElasticSearchModel(pubScreen);

            return Search(pubScreenForElasticSearch);
        }

        public PubScreenElasticSearchModel ConvertToElasticSearchModel(PubScreen pubScreen)
        {
            List<PubScreenElasticSearchModel> lstPubScreen = new List<PubScreenElasticSearchModel>();
            PubScreenElasticSearchModel pubScreenForElasticSearch = new PubScreenElasticSearchModel();

            if (pubScreen.ID != null)
            {
                pubScreenForElasticSearch.ID = (int)pubScreen.ID;
            }
            if (!string.IsNullOrEmpty(pubScreen.search))
            {
                pubScreenForElasticSearch.search = (HelperService.EscapeSql(pubScreen.search)).Trim();
            }

            // Title
            if (!string.IsNullOrEmpty(pubScreen.Title))
            {
                pubScreenForElasticSearch.Title = (HelperService.EscapeSql(pubScreen.Title)).Trim();
            }

            //Keywords
            if (!string.IsNullOrEmpty(pubScreen.Keywords))
            {
                pubScreenForElasticSearch.Keywords = (HelperService.EscapeSql(pubScreen.Keywords)).Trim();
            }

            // DOI
            if (!string.IsNullOrEmpty(pubScreen.DOI))
            {
                pubScreenForElasticSearch.DOI = (HelperService.EscapeSql(pubScreen.DOI)).Trim();
            }

            // search query for Author
            if (pubScreen.AuthourID != null && pubScreen.AuthourID.Length != 0)
            {
                List<string> authorName = new List<string>();
                if (pubScreen.AuthourID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select CONCAT(Author.FirstName, '-', Author.LastName) as Name From Author Where Author.ID = {pubScreen.AuthourID[0]}"))
                    {
                        authorName.Add(dt.Rows[0]["name"].ToString());
                    }

                }
                else
                {
                    for (int i = 0; i < pubScreen.AuthourID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select CONCAT(Author.FirstName, '-', Author.LastName) as Name From Author Where Author.ID = {pubScreen.AuthourID[i]}"))
                        {
                            authorName.Add(dt.Rows[0]["name"].ToString());
                        }
                    }
                }

                pubScreenForElasticSearch.Author = authorName.ToArray();

            }

            if(!string.IsNullOrEmpty(pubScreen.AuthorString) && pubScreen.AuthorString.Length != 0)
            {
                pubScreenForElasticSearch.Author = pubScreen.AuthorString.Split(",").ToArray();
            }

            if (!string.IsNullOrEmpty(pubScreen.Abstract))
            {
                pubScreenForElasticSearch.Abstract = (HelperService.EscapeSql(pubScreen.Abstract)).Trim();
            }

            // search query for Paper type
            if (pubScreen.PaperTypeIdSearch != null && pubScreen.PaperTypeIdSearch.Length != 0)
            {

                List<string> listOfPaperType = new List<string>();


                if (pubScreen.PaperTypeIdSearch.Length == 1)
                {

                    using (DataTable dt = Dal.GetDataTablePub($@"Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeIdSearch[0]}"))
                    {
                        listOfPaperType.Add(dt.Rows[0]["PaperType"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.PaperTypeIdSearch.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeIdSearch[i]}"))
                        {
                            listOfPaperType.Add(dt.Rows[0]["PaperType"].ToString());
                        }
                    }
                }

                pubScreenForElasticSearch.PaperType = listOfPaperType.ToArray();
            }
            else if(pubScreen.PaperTypeID != null)
            {
                List<string> listOfPaperType = new List<string>();
                using (DataTable dt = Dal.GetDataTablePub($@"Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeID}"))
                {
                    listOfPaperType.Add(dt.Rows[0]["PaperType"].ToString());
                }
                pubScreenForElasticSearch.PaperType = listOfPaperType.ToArray();
            }

            // search query for Species
            if (pubScreen.SpecieID != null && pubScreen.SpecieID.Length != 0)
            {
                List<string> listOfSpecies = new List<string>();
                if (pubScreen.SpecieID.Length == 1)
                {

                    using (DataTable dt = Dal.GetDataTablePub($@"Select Species From Species Where Species.ID = {pubScreen.SpecieID[0]}"))
                    {
                        listOfSpecies.Add(dt.Rows[0]["Species"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.SpecieID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select Species From Species Where Species.ID = {pubScreen.SpecieID[i]}"))
                        {
                            listOfSpecies.Add(dt.Rows[0]["Species"].ToString());
                        }
                    }
                }

                pubScreenForElasticSearch.Species = listOfSpecies.ToArray();
            }

            // search query for Task
            if (pubScreen.TaskID != null && pubScreen.TaskID.Length != 0)
            {
                List<string> listOfTask = new List<string>();
                if (pubScreen.TaskID.Length == 1)
                {

                    using (DataTable dt = Dal.GetDataTablePub($@"Select Task From Task Where Task.ID = {pubScreen.TaskID[0]}"))
                    {
                        listOfTask.Add(dt.Rows[0]["Task"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.TaskID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select Task From Task Where Task.ID = {pubScreen.TaskID[i]}"))
                        {
                            listOfTask.Add(dt.Rows[0]["Task"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.Task = listOfTask.ToArray();
            }

            // search query for SubTask
            if (pubScreen.SubTaskID != null && pubScreen.SubTaskID.Length != 0)
            {
                List<string> listOfSubTask = new List<string>();
                if (pubScreen.SubTaskID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select SubTask From SubTask Where SubTask.ID = {pubScreen.SubTaskID[0]}"))
                    {
                        listOfSubTask.Add(dt.Rows[0]["SubTask"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.SubTaskID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select SubTask From SubTask Where SubTask.ID = {pubScreen.SubTaskID[i]}"))
                        {
                            listOfSubTask.Add(dt.Rows[0]["SubTask"].ToString());
                        }
                    }
                }

                pubScreenForElasticSearch.SubTask = listOfSubTask.ToArray();

            }

            // search query for Sex
            if (pubScreen.sexID != null && pubScreen.sexID.Length != 0)
            {
                List<string> listOfSex = new List<string>();
                if (pubScreen.sexID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select Sex From Sex Where Sex.ID = {pubScreen.sexID[0]}"))
                    {
                        listOfSex.Add(dt.Rows[0]["Sex"].ToString());
                    }
                }
                else
                {

                    for (int i = 0; i < pubScreen.sexID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select Sex From Sex Where Sex.ID = {pubScreen.sexID[i]}"))
                        {
                            listOfSex.Add(dt.Rows[0]["Sex"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.Sex = listOfSex.ToArray();
            }

            // search query for Strain
            if (pubScreen.StrainID != null && pubScreen.StrainID.Length != 0)
            {
                List<string> listOfStrain = new List<string>();
                if (pubScreen.StrainID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[0]}"))
                    {
                        listOfStrain.Add(dt.Rows[0]["Strain"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.StrainID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[i]}"))
                        {
                            listOfStrain.Add(dt.Rows[0]["Strain"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.Strain = listOfStrain.ToArray();
            }

            // search query for Disease
            if (pubScreen.DiseaseID != null && pubScreen.DiseaseID.Length != 0)
            {
                List<string> listOfDisease = new List<string>();
                if (pubScreen.DiseaseID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select DiseaseModel From DiseaseModel Where DiseaseModel.ID = {pubScreen.DiseaseID[0]}"))
                    {
                        listOfDisease.Add(dt.Rows[0]["DiseaseModel"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.DiseaseID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select DiseaseModel From DiseaseModel Where DiseaseModel.ID = {pubScreen.DiseaseID[i]}"))
                        {
                            listOfDisease.Add(dt.Rows[0]["DiseaseModel"].ToString());
                        }
                    }

                }
                pubScreenForElasticSearch.DiseaseModel = listOfDisease.ToArray();
            }

            // search query for Sub Model
            if (pubScreen.SubModelID != null && pubScreen.SubModelID.Length != 0)
            {
                List<string> listOfSubModel = new List<string>();
                if (pubScreen.SubModelID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select SubModel From SubModel Where SubModel.ID = {pubScreen.SubModelID[0]}"))
                    {
                        listOfSubModel.Add(dt.Rows[0]["SubModel"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.SubModelID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select SubModel From SubModel Where SubModel.ID = {pubScreen.SubModelID[i]}"))
                        {
                            listOfSubModel.Add(dt.Rows[0]["SubModel"].ToString());
                        }

                    }
                }

                pubScreenForElasticSearch.SubModel = listOfSubModel.ToArray();
            }

            // search query for BrainRegion
            if (pubScreen.RegionID != null && pubScreen.RegionID.Length != 0)
            {
                List<string> listOfRegion = new List<string>();
                if (pubScreen.RegionID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select BrainRegion From BrainRegion Where BrainRegion.ID = {pubScreen.RegionID[0]}"))
                    {
                        listOfRegion.Add(dt.Rows[0]["BrainRegion"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.RegionID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select BrainRegion From BrainRegion Where BrainRegion.ID = {pubScreen.RegionID[i]}"))
                        {
                            listOfRegion.Add(dt.Rows[0]["BrainRegion"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.BrainRegion = listOfRegion.ToArray();
            }

            // search query for SubRegion
            if (pubScreen.SubRegionID != null && pubScreen.SubRegionID.Length != 0)
            {
                List<string> listOfSubRegion = new List<string>();
                if (pubScreen.SubRegionID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select SubRegion From SubRegion Where SubRegion.ID = {pubScreen.SubRegionID[0]}"))
                    {
                        listOfSubRegion.Add(dt.Rows[0]["SubRegion"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.SubRegionID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select SubRegion From SubRegion Where SubRegion.ID = {pubScreen.SubRegionID[i]}"))
                        {
                            listOfSubRegion.Add(dt.Rows[0]["SubRegion"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.SubRegion = listOfSubRegion.ToArray();
            }

            // search query for CellType
            if (pubScreen.CellTypeID != null && pubScreen.CellTypeID.Length != 0)
            {
                List<string> listOfCellType = new List<string>();
                if (pubScreen.CellTypeID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select CellType From CellType Where CellType.ID = {pubScreen.CellTypeID[0]}"))
                    {
                        listOfCellType.Add(dt.Rows[0]["CellType"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.CellTypeID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select CellType From CellType Where CellType.ID = {pubScreen.CellTypeID[i]}"))
                        {
                            listOfCellType.Add(dt.Rows[0]["CellType"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.CellType = listOfCellType.ToArray();
            }

            // search query for Method
            if (pubScreen.MethodID != null && pubScreen.MethodID.Length != 0)
            {
                List<string> listOfMethod = new List<string>();
                if (pubScreen.MethodID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select Method From Method Where Method.ID = {pubScreen.MethodID[0]}"))
                    {
                        listOfMethod.Add(dt.Rows[0]["Method"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.MethodID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select Method From Method Where Method.ID = {pubScreen.MethodID[i]}"))
                        {
                            listOfMethod.Add(dt.Rows[0]["Method"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.Method = listOfMethod.ToArray();
            }

            // search query for Sub Method
            if (pubScreen.SubMethodID != null && pubScreen.SubMethodID.Length != 0)
            {
                List<string> listOfSubMethod = new List<string>();
                if (pubScreen.SubMethodID.Length == 1)
                {
                    using (DataTable dt = Dal.GetDataTablePub($@"Select SubMethod From SubMethod Where SubMethod.ID = {pubScreen.SubMethodID[0]}"))
                    {
                        listOfSubMethod.Add(dt.Rows[0]["SubMethod"].ToString());
                    }

                }
                else
                {
                    for (int i = 0; i < pubScreen.SubMethodID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select SubMethod From SubMethod Where SubMethod.ID = {pubScreen.SubMethodID[i]}"))
                        {
                            listOfSubMethod.Add(dt.Rows[0]["SubMethod"].ToString());
                        }

                    }
                }
                pubScreenForElasticSearch.SubMethod = listOfSubMethod.ToArray();
            }

            // search query for Neuro Transmitter
            if (pubScreen.TransmitterID != null && pubScreen.TransmitterID.Length != 0)
            {
                List<string> listOfTransmitter = new List<string>();
                if (pubScreen.TransmitterID.Length == 1)
                {

                    using (DataTable dt = Dal.GetDataTablePub($@"Select Neurotransmitter From Neurotransmitter Where Neurotransmitter.ID = {pubScreen.TransmitterID[0]}"))
                    {
                        listOfTransmitter.Add(dt.Rows[0]["Neurotransmitter"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < pubScreen.TransmitterID.Length; i++)
                    {
                        using (DataTable dt = Dal.GetDataTablePub($@"Select Neurotransmitter From Neurotransmitter Where Neurotransmitter.ID = {pubScreen.TransmitterID[i]}"))
                        {
                            listOfTransmitter.Add(dt.Rows[0]["Neurotransmitter"].ToString());
                        }
                    }
                }
                pubScreenForElasticSearch.NeuroTransmitter = listOfTransmitter.ToArray();
            }

            if (pubScreen.YearFrom != null)
            {
                pubScreenForElasticSearch.YearFrom = pubScreen.YearFrom;
            }

            if (pubScreen.YearTo != null)
            {
                pubScreenForElasticSearch.YearTo = pubScreen.YearTo;
            }
            if(pubScreen.Year != null)
            {
                pubScreenForElasticSearch.Year = Int32.Parse(pubScreen.Year);
            }
            return pubScreenForElasticSearch;

        }
        public List<PubScreenElasticSearchModel> Search(PubScreenElasticSearchModel pubScreen)
        {
            var results = new List<PubScreenElasticSearchModel>();
            try
            {
                var queryContainer = new QueryContainerDescriptor<PubScreenElasticSearchModel>();
                var searchResult = _elasticClient.Search<PubScreenElasticSearchModel>(s => s.Index("pubscreen")
                    .Size(SEARCHRESULTSIZE)
                    .Query(q => ApplyQuery(pubScreen, q))
                    .Fields(f => f.Fields("*")));

                if (!searchResult.ApiCall.Success)
                {
                    Log.Error($@"Failed to get results using ElasticSearch the following error occured:{searchResult.OriginalException.Message}");
                }
                results = searchResult.Hits.Select(hit => hit.Source).ToList();

                var lstExperiment = new List<Experiment>();
                var lstRepo = new List<Cogbytes>();
                string sqlMB = string.Empty;
                string sqlCog = string.Empty;
                foreach (var paper in results)
                {
                    if (!String.IsNullOrEmpty(paper.DOI))
                    {
                        sqlMB = $@"Select Experiment.*, Task.Name as TaskName From Experiment
                                   Inner join Task on Task.ID = Experiment.TaskID
                                   Where DOI = '{paper.DOI}'";

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

                        sqlCog = $"Select * From UserRepository Where DOI = '{paper.DOI}'";
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

                        paper.PaperLinkGuid = GetGuidByDoi(paper.DOI).PaperLinkGuid;

                    }
                    paper.Experiment = lstExperiment;
                    paper.Repo = lstRepo;
                }

            }
            catch (Exception ex)
            {
                Log.Error($@"Failed to get results using ElasticSearch the following error occured:{ex.Message}");
                if(ex.InnerException != null)
                {
                    Log.Error($@"The following inner excetipn occured: {ex.InnerException}");
                }
            }
            return results;
        }

        private QueryContainer ApplyORQuery(QueryContainerDescriptor<PubScreenElasticSearchModel> query, string field, string value)
        {
            switch (field)
            {
                case ("Author"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.Author)
               .Query(value));

                case ("Task"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.Task)
               .Query(value));

                case ("SubTask"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
                .Field(f => f.SubTask)
                .Query(value));

                case ("PaperType"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
                .Field(f => f.PaperType)
                .Query(value));

                case ("Species"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.Species)
               .Query(value));

                case ("Sex"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.Sex)
               .Query(value));

                case ("Strain"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.Strain)
               .Query(value));

                case ("DiseaseModel"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.DiseaseModel)
               .Query(value));

                case ("SubModel"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.SubModel)
               .Query(value));
                case ("BrainRegion"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.BrainRegion)
               .Query(value));

                case ("SubRegion"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.SubRegion)
               .Query(value));

                case ("CellType"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.CellType)
               .Query(value));

                case ("Method"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.Method)
               .Query(value));

                case ("SubMethod"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.SubMethod)
               .Query(value));

                case ("NeuroTransmitter"):
                    return query.MatchPhrasePrefix(matchPhrasePrefix => matchPhrasePrefix
               .Field(f => f.NeuroTransmitter)
               .Query(value));
            }

            return query;
        }
        public List<QueryContainer> MultiMatchSearchField(PubScreenElasticSearchModel pubscreen, QueryContainerDescriptor<PubScreenElasticSearchModel> query) =>
            string.IsNullOrEmpty(pubscreen.search) ? new List<QueryContainer>() : ApplyMatchQuery(pubscreen.search, query);

        private List<QueryContainer> ApplyMatchQuery(string searchingFor, QueryContainerDescriptor<PubScreenElasticSearchModel> query)
        {
            var queryContainer = new List<QueryContainer>();
            foreach (var field in MULTISEARCHFIELDS)
            {
                var listOfSearchQuery = MatchRelevance(searchingFor, query, field);
                foreach (var searchQuery in listOfSearchQuery)
                {
                    queryContainer.Add(searchQuery);
                }
            }
            return queryContainer;
        }

        private List<QueryContainer> MatchRelevance(object searchingFor, QueryContainerDescriptor<PubScreenElasticSearchModel> query, string fieldName)
        {
            var queryContainer = new List<QueryContainer>();
            queryContainer.Add(MatchWithFuzziness(searchingFor, query, fieldName));
            queryContainer.Add(MatchWithWildCard(searchingFor, query, fieldName));
            return queryContainer;
        }

        private QueryContainer MatchWithFuzziness(object searchingFor, QueryContainerDescriptor<PubScreenElasticSearchModel> query, string fieldName)
        {
            var queryField = new Nest.Field(fieldName);
            return query
                        .Match(dxqm => dxqm
                            .Field(queryField)
                            .Query(searchingFor.ToString().ToLower())
                            .Fuzziness(Nest.Fuzziness.EditDistance(0)));
        }


        private QueryContainer MatchWithWildCard(object searchingFor, QueryContainerDescriptor<PubScreenElasticSearchModel> query, string fieldName) => query
        .Bool(boolq => boolq
                        .Should(boolShould => boolShould
                            .Wildcard(dxqm => dxqm
                            .Field(new Nest.Field(fieldName))
                            .Value("*" + searchingFor.ToString().ToLower() + "*"))));


        private async Task DeleteFromElasticSearchAsync(int pubId)
        {
            try
            {
                var pubScreenId = pubId.ToString();
                var response = await _elasticClient.DeleteAsync<PubScreenElasticSearchModel>(pubScreenId, delete => delete.Index("pubscreen"));
                if (!response.IsValid)
                {
                    Log.Error("Failed to delete publication from Elasticsearch with ID {PublicationID}: {Error}", pubScreenId, response.OriginalException.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting publication from Elasticsearch with ID {PublicationID}", pubId);
            }
        }
        private IndexResponse AddPublicationsToElasticSearch(PubScreen publication)
        {
            var pubScreen = ConvertToElasticSearchModel(publication);
            
            var response = _elasticClient.Index(pubScreen, p => p.Index("pubscreen"));
            return response;
        }

        private UpdateResponse<PubScreenElasticSearchModel> UpdatePublicationToElasticSearch(int Id, PubScreen publication)
        {
            publication.ID = Id;
            var pubScreen = ConvertToElasticSearchModel(publication);
            
            var rsponse = _elasticClient.Update<PubScreenElasticSearchModel>(Id,
                    f => f
                        .Index("pubscreen")
                        .Doc(pubScreen)
                        .Refresh(Refresh.True));
            return rsponse;
        }
    }

}
