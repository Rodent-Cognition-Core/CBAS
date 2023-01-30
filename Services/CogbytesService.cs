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
using Microsoft.AspNetCore.Http;

namespace AngularSPAWebAPI.Services
{

    public class CogbytesService
    {
        // Function Definition to get paper info from DOI
        // private static readonly HttpClient client = new HttpClient();

        public List<CogbytesFileType> GetFileTypes()
        {
            List<CogbytesFileType> FileTypeList = new List<CogbytesFileType>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From FileType"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    FileTypeList.Add(new CogbytesFileType
                    {
                        ID = Int32.Parse(dr["FileTypeID"].ToString()),
                        FileType = Convert.ToString(dr["FileType"].ToString()),

                    });
                }
            }

            return FileTypeList;
        }

        // Function Definition to extract list of all Cognitive Tasks
        public List<PubScreenTask> GetTasks()
        {
            List<PubScreenTask> TaskList = new List<PubScreenTask>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Task"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TaskList.Add(new PubScreenTask
                    {
                        ID = Int32.Parse(dr["TaskID"].ToString()),
                        Task = Convert.ToString(dr["Name"].ToString()),


                    });
                }
            }

            return TaskList;
        }

        // Function Definition to extract list of all Species
        public List<PubScreenSpecie> GetSpecies()
        {
            List<PubScreenSpecie> SpecieList = new List<PubScreenSpecie>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Species"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SpecieList.Add(new PubScreenSpecie
                    {
                        ID = Int32.Parse(dr["SpeciesID"].ToString()),
                        Species = Convert.ToString(dr["Species"].ToString()),


                    });
                }
            }

            return SpecieList;
        }

        //// Function Definition to extract list of all Sex
        public List<PubScreenSex> GetSex()
        {
            List<PubScreenSex> SexList = new List<PubScreenSex>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Sex"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SexList.Add(new PubScreenSex
                    {
                        ID = Int32.Parse(dr["SexID"].ToString()),
                        Sex = Convert.ToString(dr["Sex"].ToString()),


                    });
                }
            }

            return SexList;
        }

        //// Function Definition to extract list of all Strains
        public List<PubScreenStrain> GetStrains()
        {
            List<PubScreenStrain> StrainList = new List<PubScreenStrain>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Strain"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StrainList.Add(new PubScreenStrain
                    {
                        ID = Int32.Parse(dr["StrainID"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),


                    });
                }
            }

            return StrainList;
        }

        //// Function Definition to extract list of all Genotypes
        public List<Geno> GetGenos()
        {
            List<Geno> GenoList = new List<Geno>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Genotype"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GenoList.Add(new Geno
                    {
                        ID = Int32.Parse(dr["GenoID"].ToString()),
                        Genotype = Convert.ToString(dr["Genotype"].ToString()),


                    });
                }
            }

            return GenoList;
        }

        //// Function Definition to extract list of all ages
        public List<Age> GetAges()
        {
            List<Age> AgeList = new List<Age>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Age"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AgeList.Add(new Age
                    {
                        ID = Int32.Parse(dr["AgeID"].ToString()),
                        AgeInMonth = Convert.ToString(dr["AgeInMonth"].ToString()),


                    });
                }
            }

            return AgeList;
        }


        //// Function defintion to add a new author to database
        public int AddAuthors(PubScreenAuthor author, string userEmail)
        {
            // Check if author is in DB
            string sqlCount = $@"Select Count(AuthorID) From Author Where FirstName = '{author.FirstName.Trim()}' AND LastName = '{author.LastName.Trim()}'";
            int isAuthorAdded = Int32.Parse(Dal.ExecScalarCog(sqlCount).ToString());
            if (isAuthorAdded > 0)
            {
                return 0;
            }

            string sql = $@"Insert into Author (FirstName, LastName, Affiliation, Username) Values
                            ('{author.FirstName.Trim()}', '{author.LastName.Trim()}', '{author.Affiliation.Trim()}', '{userEmail}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalarCog(sql).ToString());
        }


        //// Function Definition to extract list of Authors 
        public List<PubScreenAuthor> GetAuthors()
        {
            List<PubScreenAuthor> AuthorList = new List<PubScreenAuthor>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Author Order By LastName"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AuthorList.Add(new PubScreenAuthor
                    {
                        ID = Int32.Parse(dr["AuthorID"].ToString()),
                        FirstName = Convert.ToString(dr["FirstName"].ToString()),
                        LastName = Convert.ToString(dr["LastName"].ToString()),
                        Affiliation = Convert.ToString(dr["Affiliation"].ToString()),

                    });
                }
            }

            return AuthorList;
        }

        public int AddNewPI(Request request, string userEmail)
        {
            // Check if author is in DB
            string sqlCount = $@"Select Count(PIID) From PI Where FullName = '{request.PIFullName.Trim()}'";
            int isPIAdded = Int32.Parse(Dal.ExecScalarCog(sqlCount).ToString());
            if (isPIAdded > 0)
            {
                return 0;
            }

            string sql = $@"Insert into PI (Username, FullName, Email, Affiliation) Values
                            ('{userEmail}', '{HelperService.EscapeSql(request.PIFullName)}',
                             '{HelperService.EscapeSql(request.PIEmail)}', '{HelperService.EscapeSql(request.PIInstitution)}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalarCog(sql).ToString());

        }

        //// Function Definition to extract list of PIs 
        public List<Request> GetPIs()
        {
            List<Request> PIList = new List<Request>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From PI Order By PIID"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PIList.Add(new Request
                    {
                        ID = Int32.Parse(dr["PIID"].ToString()),
                        PIFullName = Convert.ToString(dr["FullName"].ToString()),
                        PIEmail = Convert.ToString(dr["Email"].ToString()),
                        PIInstitution = Convert.ToString(dr["Affiliation"].ToString()),

                    });
                }
            }

            return PIList;
        }


        ////************************************************************************************Adding Repository*************************************************************************************
        // Function Definition to add a new repository to database Cogbytes
        public int? AddRepository(Cogbytes repository, string Username)
        {

            string sqlRepository = $@"Insert into UserRepository (RepoLinkGuid, Title, Date, DOI, Keywords, PrivacyStatus, Description, AdditionalNotes, Link, Username, DateRepositoryCreated) Values
                                    ('{Guid.NewGuid()}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(repository.Title)).Trim())}',
                                     '{repository.Date}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(repository.DOI)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(repository.Keywords)).Trim())}',
                                     '{repository.PrivacyStatus}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(repository.Description)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(repository.AdditionalNotes)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(repository.Link)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(Username)))}',
                                     '{repository.DateRepositoryCreated}'
                                      ); SELECT @@IDENTITY AS 'Identity'; ";

            int RepositoryID = Int32.Parse(Dal.ExecScalarCog(sqlRepository).ToString());

            // Adding Author **********************************************************************************************************************

            string sqlAuthor = "";
            for (int i = 0; i < repository.AuthourID.Length; i++)
            {
                sqlAuthor += $@"Insert into RepAuthor (AuthorID, RepID) Values ({repository.AuthourID[i]}, {RepositoryID});";
            }
            if (sqlAuthor != "") { Dal.ExecuteNonQueryCog(sqlAuthor); };

            // Adding PI

            string sqlPI = "";
            for (int i = 0; i < repository.PIID.Length; i++)
            {
                sqlPI += $@"Insert into RepPI (PIID, RepID) Values ({repository.PIID[i]}, {RepositoryID});";
            }
            if (sqlPI != "") { Dal.ExecuteNonQueryCog(sqlPI); };

            // Send email for new repository
            string emailMsg = $"Repository Title: {repository.Title}\n\nUser: {Username}";
            HelperService.SendEmail("", "", "New Complementary Data Reposiotry in MouseBytes+", emailMsg.Replace("\n", "<br \\>"));

            return RepositoryID;

        }

        //// Function Definition to extract a user's repositories 
        public List<Cogbytes> GetRepositories(string userEmail)
        {
            List<Cogbytes> RepList = new List<Cogbytes>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From UserRepository Where Username='{userEmail}' Order By DateRepositoryCreated"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int repID = Int32.Parse(dr["RepID"].ToString());
                    string doi = Convert.ToString(dr["DOI"].ToString());
                    PubScreenSearch publication = GetPubScreenPaper(doi);

                    RepList.Add(new Cogbytes
                    {
                        ID = repID,
                        RepoLinkGuid = Guid.Parse(dr["repoLinkGuid"].ToString()),
                        Title = Convert.ToString(dr["Title"].ToString()),
                        Date = Convert.ToString(dr["Date"].ToString()),
                        Keywords = Convert.ToString(dr["Keywords"].ToString()),
                        DOI = doi,
                        Link = Convert.ToString(dr["Link"].ToString()),
                        PrivacyStatus = Boolean.Parse(dr["PrivacyStatus"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),
                        AdditionalNotes = Convert.ToString(dr["AdditionalNotes"].ToString()),
                        AuthourID = FillCogbytesItemArray($"Select AuthorID From RepAuthor Where RepID={repID}", "AuthorID"),
                        PIID = FillCogbytesItemArray($"Select PIID From RepPI Where RepID={repID}", "PIID"),
                        Experiment = GetCogbytesExperimentList(Guid.Parse(dr["repoLinkGuid"].ToString())),
                        Paper = publication
                    });
                }
            }

            return RepList;
        }

        // Function Definition to edit a respository in database Cogbytes
        public bool EditRepository(int repositoryID, Cogbytes repository, string Username)
        {

            string sqlRepository = $@"Update UserRepository set Title = @title, Date = @date, DOI = @doi, Keywords = @keywords, PrivacyStatus = @privacyStatus,
                                                                Description = @description, AdditionalNotes = @additionalNotes, Link = @link
                                                                where RepID = {repositoryID}";

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@title", HelperService.NullToString(HelperService.EscapeSql(repository.Title)).Trim()));
            parameters.Add(new SqlParameter("@date", HelperService.NullToString(HelperService.EscapeSql(repository.Date)).Trim()));
            parameters.Add(new SqlParameter("@doi", HelperService.NullToString(HelperService.EscapeSql(repository.DOI)).Trim()));
            parameters.Add(new SqlParameter("@keywords", HelperService.NullToString(HelperService.EscapeSql(repository.Keywords)).Trim()));
            parameters.Add(new SqlParameter("@privacyStatus", HelperService.NullToString(repository.PrivacyStatus).Trim()));
            parameters.Add(new SqlParameter("@description", HelperService.NullToString(HelperService.EscapeSql(repository.Description)).Trim()));
            parameters.Add(new SqlParameter("@additionalNotes", HelperService.NullToString(HelperService.EscapeSql(repository.AdditionalNotes)).Trim()));
            parameters.Add(new SqlParameter("@link", HelperService.NullToString(HelperService.EscapeSql(repository.Link)).Trim()));

            Int32.Parse(Dal.ExecuteNonQueryCog(CommandType.Text, sqlRepository, parameters.ToArray()).ToString());

            string sqlAuthor = "";

            string sqlDelete = $"DELETE From RepAuthor where RepID = {repositoryID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < repository.AuthourID.Length; i++)
            {
                sqlAuthor += $@"Insert into RepAuthor (AuthorID, RepID) Values ({repository.AuthourID[i]}, {repositoryID});";
            }

            Dal.ExecuteNonQueryCog(sqlAuthor);

            string sqlPI = "";
            sqlDelete = $"DELETE From RepPI where RepID = {repositoryID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < repository.PIID.Length; i++)
            {
                sqlPI += $@"Insert into RepPI (PIID, RepID) Values ({repository.PIID[i]}, {repositoryID});";
            }

            if (sqlPI != "") Dal.ExecuteNonQueryCog(sqlPI);

            return true;

        }

        // Function Definition to add a new repository to database Cogbytes
        public int? AddUpload(CogbytesUpload upload)
        {
            string sqlNumSubjects = "";
            if (upload.NumSubjects == null)
            {
                sqlNumSubjects = "null";
            }
            else
            {
                sqlNumSubjects = upload.NumSubjects.ToString();
            }

            string sqlUpload = $@"Insert into Upload (RepID, FileTypeID, Name, DateUpload, Description, AdditionalNotes, IsIntervention, InterventionDescription, ImageIds, ImageDescription, Housing, LightCycle, TaskBattery, NumSubjects) Values
                                    ('{upload.RepID}',
                                     '{upload.FileTypeID}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.Name)).Trim())}',
                                     '{upload.DateUpload}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.Description)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.AdditionalNotes)).Trim())}',
                                     '{upload.IsIntervention}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.InterventionDescription)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.ImageIds)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.ImageDescription)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.Housing)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.LightCycle)).Trim())}',
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.TaskBattery)).Trim())}',
                                     {sqlNumSubjects}
                                      ); SELECT @@IDENTITY AS 'Identity'; ";

            int UploadID = Int32.Parse(Dal.ExecScalarCog(sqlUpload).ToString());

            // Adding Tasks and other Features **********************************************************************************************************************

            string sqlCmd = "";
            for (int i = 0; i < upload.TaskID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetTask (TaskID, UploadID) Values ({upload.TaskID[i]}, {UploadID});";
            }
            if (sqlCmd != "") { Dal.ExecuteNonQueryCog(sqlCmd); };

            sqlCmd = "";
            for (int i = 0; i < upload.SpecieID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetSpecies (SpeciesID, UploadID) Values ({upload.SpecieID[i]}, {UploadID});";
            }
            if (sqlCmd != "") { Dal.ExecuteNonQueryCog(sqlCmd); };

            sqlCmd = "";
            for (int i = 0; i < upload.SexID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetSex (SexID, UploadID) Values ({upload.SexID[i]}, {UploadID});";
            }
            if (sqlCmd != "") { Dal.ExecuteNonQueryCog(sqlCmd); };

            sqlCmd = "";
            for (int i = 0; i < upload.StrainID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetStrain (StrainID, UploadID) Values ({upload.StrainID[i]}, {UploadID});";
            }
            if (sqlCmd != "") { Dal.ExecuteNonQueryCog(sqlCmd); };

            sqlCmd = "";
            for (int i = 0; i < upload.GenoID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetGeno (GenoID, UploadID) Values ({upload.GenoID[i]}, {UploadID});";
            }
            if (sqlCmd != "") { Dal.ExecuteNonQueryCog(sqlCmd); };

            sqlCmd = "";
            for (int i = 0; i < upload.AgeID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetAge (AgeID, UploadID) Values ({upload.AgeID[i]}, {UploadID});";
            }
            if (sqlCmd != "") { Dal.ExecuteNonQueryCog(sqlCmd); };


            return UploadID;

        }

        public List<FileUploadResult> GetUploadFiles(int uploadID)
        {
            List<FileUploadResult> FileList = new List<FileUploadResult>();

            using (DataTable ft = Dal.GetDataTableCog($@"Select * From UploadFile Where UploadID='{uploadID}' Order By DateUploaded"))
            {
                foreach (DataRow fr in ft.Rows)
                {
                    FileList.Add(new FileUploadResult
                    {
                        ExpID = Int32.Parse(fr["ID"].ToString()), // Hijaking ExpID for the primary key
                        UserFileName = Convert.ToString(fr["UserFileName"].ToString()),
                        SysFileName = Convert.ToString(fr["SystemFileName"].ToString()),
                        DateUpload = DateTime.Parse(fr["DateUploaded"].ToString()),
                        DateFileCreated = DateTime.Parse(fr["DateFileCreated"].ToString()),
                        FileSize = Int32.Parse(fr["FileSize"].ToString()),
                        PermanentFilePath = Convert.ToString(fr["PermanentFilePath"].ToString()),
                    });
                }
            }

            return FileList;
        }

        //// Function Definition to extract a repositories' uploads 
        public List<CogbytesUpload> GetUploads(int repID)
        {
            List<CogbytesUpload> Uploadlist = new List<CogbytesUpload>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Upload Where RepID='{repID}' Order By DateUpload"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int uploadID = Int32.Parse(dr["UploadID"].ToString());
                    int fileTypeID = Int32.Parse(dr["FileTypeID"].ToString());
                    int? numSubjects = null;
                    int num;
                    if (Int32.TryParse(dr["NumSubjects"].ToString(), out num))
                    {
                        numSubjects = num;
                    };


                    List<FileUploadResult> FileList = new List<FileUploadResult>();

                    using (DataTable ft = Dal.GetDataTableCog($@"Select * From UploadFile Where UploadID='{uploadID}' Order By DateUploaded"))
                    {
                        foreach (DataRow fr in ft.Rows)
                        {
                            FileList.Add(new FileUploadResult
                            {
                                ExpID = Int32.Parse(fr["ID"].ToString()), // Hijaking ExpID for the primary key
                                UserFileName = Convert.ToString(fr["UserFileName"].ToString()),
                                SysFileName = Convert.ToString(fr["SystemFileName"].ToString()),
                                DateUpload = DateTime.Parse(fr["DateUploaded"].ToString()),
                                DateFileCreated = DateTime.Parse(fr["DateFileCreated"].ToString()),
                                FileSize = Int32.Parse(fr["FileSize"].ToString()),
                                PermanentFilePath = Convert.ToString(fr["PermanentFilePath"].ToString()),
                            });
                        }
                    }

                    Uploadlist.Add(new CogbytesUpload
                    {
                        ID = uploadID,
                        RepID = repID,
                        FileTypeID = fileTypeID,
                        Name = Convert.ToString(dr["Name"].ToString()),
                        DateUpload = Convert.ToString(dr["DateUpload"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),
                        AdditionalNotes = Convert.ToString(dr["AdditionalNotes"].ToString()),
                        IsIntervention = Boolean.Parse(dr["IsIntervention"].ToString()),
                        InterventionDescription = Convert.ToString(dr["InterventionDescription"].ToString()),
                        ImageIds = Convert.ToString(dr["ImageIds"].ToString()),
                        ImageDescription = Convert.ToString(dr["ImageDescription"].ToString()),
                        Housing = Convert.ToString(dr["Housing"].ToString()),
                        LightCycle = Convert.ToString(dr["LightCycle"].ToString()),
                        TaskBattery = Convert.ToString(dr["TaskBattery"].ToString()),
                        TaskID = FillCogbytesItemArray($"Select TaskID From DatasetTask Where UploadID={uploadID}", "TaskID"),
                        SpecieID = FillCogbytesItemArray($"Select SpeciesID From DatasetSpecies Where UploadID={uploadID}", "SpeciesID"),
                        SexID = FillCogbytesItemArray($"Select SexID From DatasetSex Where UploadID={uploadID}", "SexID"),
                        StrainID = FillCogbytesItemArray($"Select StrainID From DatasetStrain Where UploadID={uploadID}", "StrainID"),
                        GenoID = FillCogbytesItemArray($"Select GenoID From DatasetGeno Where UploadID={uploadID}", "GenoID"),
                        AgeID = FillCogbytesItemArray($"Select AgeID From DatasetAge Where UploadID={uploadID}", "AgeID"),
                        NumSubjects = numSubjects,
                        UploadFileList = FileList
                    });
                }
            }

            return Uploadlist;
        }

        // Function Definition to edit a respository in database Cogbytes
        public bool EditUpload(int uploadID, CogbytesUpload upload)
        {
            //string sqlNumSubjects = "";
            //if (upload.NumSubjects == null)
            //{
            //    sqlNumSubjects = "null";
            //}
            //else
            //{
            //    sqlNumSubjects = upload.NumSubjects.ToString();
            //}

            string sqlUpload = $@"Update Upload set Name = @name, Description = @description, AdditionalNotes = @additionalNotes, IsIntervention = @isIntervention, InterventionDescription=@interventionDescription,
                                                                    ImageIds = @imageIds, ImageDescription=@imageDescription, Housing=@housing, LightCycle = @lightCycle, TaskBattery=@taskBattery, NumSubjects=@numSubjects
                                                                where UploadID = {uploadID}";

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@name", HelperService.NullToString(HelperService.EscapeSql(upload.Name)).Trim()));
            parameters.Add(new SqlParameter("@description", HelperService.NullToString(HelperService.EscapeSql(upload.Description)).Trim()));
            parameters.Add(new SqlParameter("@additionalNotes", HelperService.NullToString(HelperService.EscapeSql(upload.AdditionalNotes)).Trim()));
            parameters.Add(new SqlParameter("@isIntervention", HelperService.NullToString(upload.IsIntervention).Trim()));
            parameters.Add(new SqlParameter("@interventionDescription", HelperService.NullToString(HelperService.EscapeSql(upload.InterventionDescription)).Trim()));
            parameters.Add(new SqlParameter("@imageIds", HelperService.NullToString(HelperService.EscapeSql(upload.ImageIds)).Trim()));
            parameters.Add(new SqlParameter("@imageDescription", HelperService.NullToString(HelperService.EscapeSql(upload.ImageDescription)).Trim()));
            parameters.Add(new SqlParameter("@housing", HelperService.NullToString(HelperService.EscapeSql(upload.Housing)).Trim()));
            parameters.Add(new SqlParameter("@lightCycle", HelperService.NullToString(HelperService.EscapeSql(upload.LightCycle)).Trim()));
            parameters.Add(new SqlParameter("@taskBattery", HelperService.NullToString(HelperService.EscapeSql(upload.TaskBattery)).Trim()));
            parameters.Add(new SqlParameter("@numSubjects", (object)upload.NumSubjects ?? DBNull.Value));

            Int32.Parse(Dal.ExecuteNonQueryCog(CommandType.Text, sqlUpload, parameters.ToArray()).ToString());

            string sqlCmd = "";
            string sqlDelete = $"DELETE From DatasetTask where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < upload.TaskID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetTask (TaskID, UploadID) Values ({upload.TaskID[i]}, {uploadID});";
            }

            if (sqlCmd != "") Dal.ExecuteNonQueryCog(sqlCmd);

            sqlCmd = "";
            sqlDelete = $"DELETE From DatasetSpecies where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < upload.SpecieID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetSpecies (SpeciesID, UploadID) Values ({upload.SpecieID[i]}, {uploadID});";
            }

            if (sqlCmd != "") Dal.ExecuteNonQueryCog(sqlCmd);

            sqlCmd = "";
            sqlDelete = $"DELETE From DatasetSex where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < upload.SexID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetSex (SexID, UploadID) Values ({upload.SexID[i]}, {uploadID});";
            }

            if (sqlCmd != "") Dal.ExecuteNonQueryCog(sqlCmd);

            sqlCmd = "";
            sqlDelete = $"DELETE From DatasetStrain where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < upload.StrainID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetStrain (StrainID, UploadID) Values ({upload.StrainID[i]}, {uploadID});";
            }

            if (sqlCmd != "") Dal.ExecuteNonQueryCog(sqlCmd);

            sqlCmd = "";
            sqlDelete = $"DELETE From DatasetGeno where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < upload.GenoID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetGeno (GenoID, UploadID) Values ({upload.GenoID[i]}, {uploadID});";
            }

            if (sqlCmd != "") Dal.ExecuteNonQueryCog(sqlCmd);

            sqlCmd = "";
            sqlDelete = $"DELETE From DatasetAge where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            for (int i = 0; i < upload.AgeID.Length; i++)
            {
                sqlCmd += $@"Insert into DatasetAge (AgeID, UploadID) Values ({upload.AgeID[i]}, {uploadID});";
            }

            if (sqlCmd != "") Dal.ExecuteNonQueryCog(sqlCmd);

            return true;

        }
        ////*******************************************************************************************************************************************************************
        public async Task<bool> AddFiles(IFormFileCollection files, int uploadID)
        {
            List<FileUploadResult> uploadResult = new List<FileUploadResult>();

            foreach (IFormFile file in files)
            {

                string tempFileName = Guid.NewGuid().ToString() + "-" + file.FileName;

                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "TempUpload");

                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, tempFileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    DateTime? DateUpload1 = DateTime.UtcNow;

                    string pathString = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "COGBYTES_FILES", uploadID.ToString());

                    FileUploadResult fur = new FileUploadResult
                    {
                        UploadID = uploadID,
                        UserFileName = file.FileName,
                        SysFileName = tempFileName,
                        DateFileCreated = DateTime.UtcNow,
                        DateUpload = DateUpload1,
                        FileSize = HelperService.ConvertToNullableInt(file.Length.ToString()),
                        PermanentFilePath = pathString,
                    };

                    //Inserting to Upload Table in DB

                    // Call insert function and return Upload ID if this fileinfo was not already inserted to tbl Upload
                    int fileID = InsertFile(fur);

                    // Copy the file to permanent path: COGBYTES_FILES/uploadID

                    System.IO.Directory.CreateDirectory(pathString);
                    if (file.Length > 0)
                    {
                        using (var fileStream1 = new FileStream(Path.Combine(pathString, tempFileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream1);
                        }
                    }

                }
            }

            return true;

        }

        // Function to Insert File in Database
        public int InsertFile(FileUploadResult upload)
        {
            string sql = $"Insert into UploadFile " +
              $"(UploadID, UserFileName, SystemFileName, DateFileCreated, DateUploaded, FileSize, PermanentFilePath ) Values " +
              $"({upload.UploadID}, '{upload.UserFileName}', '{upload.SysFileName}', '{upload.DateFileCreated}', " +
              $"'{upload.DateUpload}', '{upload.FileSize}', '{upload.PermanentFilePath}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalarCog(sql).ToString());
        }

        public void DeleteFile(int fileID)
        {
            string sql = $@"Delete from UploadFile where ID = {fileID}";
            Dal.ExecuteNonQueryCog(sql);
        }

        public void DeleteUpload(int uploadID)
        {
            string sqlDelete = $"DELETE From DatasetTask where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            sqlDelete = $"DELETE From DatasetSpecies where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            sqlDelete = $"DELETE From DatasetSex where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            sqlDelete = $"DELETE From DatasetStrain where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            sqlDelete = $"DELETE From DatasetGeno where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            sqlDelete = $"DELETE From DatasetAge where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            sqlDelete = $"DELETE From UploadFile where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);

            sqlDelete = $"DELETE From Upload where UploadID = {uploadID}";
            Dal.ExecuteNonQueryCog(sqlDelete);
        }

        public void DeleteRepository(int repID)
        {
            // Delete RepGuidLink from Experiments linked to the repository
            Cogbytes rep = GetGuidByRepID(repID);
            Dal.ExecuteNonQuery($"UPDATE Experiment SET RepoGuid = null WHERE RepoGuid = '{rep.RepoLinkGuid}'");

            using (DataTable dt = Dal.GetDataTableCog($@"Select UploadID From Upload Where RepID = {repID}"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int uploadID = Int32.Parse(dr["UploadID"].ToString());
                    string pathString = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "COGBYTES_FILES", uploadID.ToString());
                    if (System.IO.Directory.Exists(pathString))
                    {
                        System.IO.Directory.Delete(pathString, true);
                    }
                    DeleteUpload(uploadID);
                }
            }

            string sqlDelete = $"DELETE From RepAuthor where RepID = {repID}";
            Dal.ExecuteNonQueryCog(sqlDelete);
            sqlDelete = $"DELETE From RepPI where RepID = {repID}";
            Dal.ExecuteNonQueryCog(sqlDelete);
            sqlDelete = $"DELETE From UserRepository where RepID = {repID}";
            Dal.ExecuteNonQueryCog(sqlDelete);
        }


        public int?[] FillCogbytesItemArray(string sql, string fieldName)
        {

            var retVal = new int?[0];
            using (DataTable dt = Dal.GetDataTableCog(sql))
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


        // Function definition to search repositories in database
        public List<CogbytesUpload> SearchRepositories(CogbytesSearch cogbytesSearch)
        {
            List<CogbytesUpload> Uploadlist = new List<CogbytesUpload>();

            string sql = "Select UploadID, Name, DateUpload, Description, AdditionalNotes, IsIntervention, InterventionDescription, " +
                "ImageIds, ImageDescription, Housing, LightCycle, TaskBattery, NumSubjects, RepID, FileTypeID " +
                "From SearchCog Where ";

            // Title
            if (cogbytesSearch.RepID != null && cogbytesSearch.RepID.Length != 0)
            {

                //if (pubScreen.TaskID.Length == 1)
                //{
                //    sql += $@"SearchPub.Task like '%'  + (Select Task From Task Where Task.ID = {pubScreen.TaskID[0]}) +  '%' AND ";
                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.RepID.Length; i++)
                {
                    sql += $@"RepID = {cogbytesSearch.RepID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}

            }

            //Keywords
            if (!string.IsNullOrEmpty(cogbytesSearch.Keywords))
            {
                sql += $@"Keywords like '%{HelperService.EscapeSql(cogbytesSearch.Keywords)}%' AND ";
            }

            // DOI
            if (!string.IsNullOrEmpty(cogbytesSearch.DOI))
            {
                sql += $@"DOI = '{HelperService.EscapeSql(cogbytesSearch.DOI)}' AND ";
            }



            // search query for Author
            if (cogbytesSearch.AuthorID != null && cogbytesSearch.AuthorID.Length != 0)
            {
                //if (cogbytesSearch.AuthorID.Length == 1)
                //{
                //    sql += $@"SearchPub.Author like '%'  + (Select CONCAT(Author.FirstName, '-', Author.LastName) From Author Where Author.ID = {pubScreen.AuthourID[0]}) +  '%' AND ";
                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.AuthorID.Length; i++)
                {
                    sql += $@"AuthorID = {cogbytesSearch.AuthorID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}

            }

            // search query for Year
            if (cogbytesSearch.YearFrom != null && cogbytesSearch.YearTo != null)
            {
                sql += $@"(Date >= '{cogbytesSearch.YearFrom}-01-01' AND Date <= '{cogbytesSearch.YearTo}-12-31') AND ";
            }

            else if (cogbytesSearch.YearFrom != null && cogbytesSearch.YearTo == null)
            {
                sql += $@"(Date >= '{cogbytesSearch.YearFrom}-01-01') AND ";
            }

            else if (cogbytesSearch.YearTo != null && cogbytesSearch.YearFrom == null)
            {
                sql += $@"(Date <= '{cogbytesSearch.YearTo}-12-31') AND ";
            }

            // search query for File Type
            if (cogbytesSearch.FileTypeID != null && cogbytesSearch.FileTypeID.Length != 0)
            {
                sql += "(";
                for (int i = 0; i < cogbytesSearch.FileTypeID.Length; i++)
                {
                    sql += $@"FileTypeID = {cogbytesSearch.FileTypeID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}

            }

            // search query for Task
            if (cogbytesSearch.TaskID != null && cogbytesSearch.TaskID.Length != 0)
            {

                //if (pubScreen.TaskID.Length == 1)
                //{
                //    sql += $@"SearchPub.Task like '%'  + (Select Task From Task Where Task.ID = {pubScreen.TaskID[0]}) +  '%' AND ";
                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.TaskID.Length; i++)
                {
                    sql += $@"TaskID = {cogbytesSearch.TaskID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}

            }

            // search query for Species
            if (cogbytesSearch.SpecieID != null && cogbytesSearch.SpecieID.Length != 0)
            {
                //if (cogbytesSearch.SpecieID.Length == 1)
                //{
                //    sql += $@"SearchPub.Species like '%'  + (Select Species From Species Where Species.ID = {pubScreen.SpecieID[0]}) +  '%' AND ";

                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.SpecieID.Length; i++)
                {
                    sql += $@"SpeciesID = {cogbytesSearch.SpecieID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}
            }

            // search query for Sex
            if (cogbytesSearch.SexID != null && cogbytesSearch.SexID.Length != 0)
            {
                //if (pubScreen.sexID.Length == 1)
                //{
                //    sql += $@"SearchPub.Sex like '%'  + (Select Sex From Sex Where Sex.ID = {pubScreen.sexID[0]}) +  '%' AND ";
                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.SexID.Length; i++)
                {
                    sql += $@"SexID = {cogbytesSearch.SexID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}
            }

            // search query for Strain
            if (cogbytesSearch.StrainID != null && cogbytesSearch.StrainID.Length != 0)
            {
                //if (cogbytesSearch.StrainID.Length == 1)
                //{
                //    sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[0]}) +  '%' AND ";
                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.StrainID.Length; i++)
                {
                    sql += $@"StrainID = {cogbytesSearch.StrainID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}
            }

            // search query for Geno
            if (cogbytesSearch.GenoID != null && cogbytesSearch.GenoID.Length != 0)
            {
                //if (cogbytesSearch.StrainID.Length == 1)
                //{
                //    sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[0]}) +  '%' AND ";
                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.GenoID.Length; i++)
                {
                    sql += $@"GenoID = {cogbytesSearch.GenoID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}
            }

            // search query for Age
            if (cogbytesSearch.AgeID != null && cogbytesSearch.AgeID.Length != 0)
            {
                //if (cogbytesSearch.StrainID.Length == 1)
                //{
                //    sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[0]}) +  '%' AND ";
                //}
                //else
                //{
                sql += "(";
                for (int i = 0; i < cogbytesSearch.AgeID.Length; i++)
                {
                    sql += $@"AgeID = {cogbytesSearch.AgeID[i]} OR ";
                }
                sql = sql.Substring(0, sql.Length - 3);
                sql += ") AND ";
                //}
            }

            // filter for intervention

            if (cogbytesSearch.Intervention == "Only")
            {
                sql += $@"IsIntervention = 1 AND ";
            }

            else if (cogbytesSearch.Intervention == "No")
            {
                sql += $@"IsIntervention = 0 AND ";
            }

            // if no search entries, do not execute query and return empty list
            if (sql.Substring(sql.Length - 4) != "AND ")
            {
                return Uploadlist;
            }

            sql = sql.Substring(0, sql.Length - 4); // to remvoe the last NAD from the query
            sql += "GROUP BY UploadID, Name, DateUpload, Description, AdditionalNotes, IsIntervention, InterventionDescription, " +
                "ImageIds, ImageDescription, Housing, LightCycle, TaskBattery, NumSubjects, RepID, FileTypeID " +
                "ORDER BY RepID";
            string sqlMB = "";

            using (DataTable dt = Dal.GetDataTableCog(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int uploadID = Int32.Parse(dr["UploadID"].ToString());
                    int fileTypeID = Int32.Parse(dr["FileTypeID"].ToString());
                    int? numSubjects = null;
                    int num;
                    if (Int32.TryParse(dr["NumSubjects"].ToString(), out num))
                    {
                        numSubjects = num;
                    };

                    List<FileUploadResult> FileList = new List<FileUploadResult>();

                    using (DataTable ft = Dal.GetDataTableCog($@"Select * From UploadFile Where UploadID='{uploadID}' Order By DateUploaded"))
                    {
                        foreach (DataRow fr in ft.Rows)
                        {
                            FileList.Add(new FileUploadResult
                            {
                                ExpID = Int32.Parse(fr["ID"].ToString()), // Hijacking ExpID for the primary key
                                UserFileName = Convert.ToString(fr["UserFileName"].ToString()),
                                SysFileName = Convert.ToString(fr["SystemFileName"].ToString()),
                                DateUpload = DateTime.Parse(fr["DateUploaded"].ToString()),
                                DateFileCreated = DateTime.Parse(fr["DateFileCreated"].ToString()),
                                FileSize = Int32.Parse(fr["FileSize"].ToString()),
                                PermanentFilePath = Convert.ToString(fr["PermanentFilePath"].ToString()),
                            });
                        }
                    }

                    Uploadlist.Add(new CogbytesUpload
                    {
                        ID = uploadID,
                        RepID = Int32.Parse(dr["RepID"].ToString()),
                        FileTypeID = fileTypeID,
                        Name = Convert.ToString(dr["Name"].ToString()),
                        DateUpload = Convert.ToString(dr["DateUpload"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),
                        AdditionalNotes = Convert.ToString(dr["AdditionalNotes"].ToString()),
                        IsIntervention = Boolean.Parse(dr["IsIntervention"].ToString()),
                        InterventionDescription = Convert.ToString(dr["InterventionDescription"].ToString()),
                        ImageIds = Convert.ToString(dr["ImageIds"].ToString()),
                        ImageDescription = Convert.ToString(dr["ImageDescription"].ToString()),
                        Housing = Convert.ToString(dr["Housing"].ToString()),
                        LightCycle = Convert.ToString(dr["LightCycle"].ToString()),
                        TaskBattery = Convert.ToString(dr["TaskBattery"].ToString()),
                        TaskID = FillCogbytesItemArray($"Select TaskID From DatasetTask Where UploadID={uploadID}", "TaskID"),
                        SpecieID = FillCogbytesItemArray($"Select SpeciesID From DatasetSpecies Where UploadID={uploadID}", "SpeciesID"),
                        SexID = FillCogbytesItemArray($"Select SexID From DatasetSex Where UploadID={uploadID}", "SexID"),
                        StrainID = FillCogbytesItemArray($"Select StrainID From DatasetStrain Where UploadID={uploadID}", "StrainID"),
                        GenoID = FillCogbytesItemArray($"Select GenoID From DatasetGeno Where UploadID={uploadID}", "GenoID"),
                        AgeID = FillCogbytesItemArray($"Select AgeID From DatasetAge Where UploadID={uploadID}", "AgeID"),
                        NumSubjects = numSubjects,
                        UploadFileList = FileList
                    });
                }

            }

            // search MouseBytes database to see if the dataset exists********************************************


            return Uploadlist;


        }

        public List<Cogbytes> ShowAllRepositories()
        {
            List<Cogbytes> RepList = new List<Cogbytes>();
            //using (DataTable dt = Dal.GetDataTableCog($@"Select * From UserRepository Where PrivacyStatus = 1 Order By DateRepositoryCreated"))
            using (DataTable dt = Dal.GetDataTableCog($@"SELECT        UserRepository.RepID, RepoLinkGuid, Title, Date, DOI, Keywords, PrivacyStatus, UserRepository.Description, UserRepository.AdditionalNotes, Link, Username, DateRepositoryCreated, UserRepository.DataCiteURL, 
                         STUFF
                             ((SELECT        ', ' + CONCAT(Author.FirstName, '-', Author.LastName)
                                 FROM            RepAuthor INNER JOIN
                                                          Author ON Author.AuthorID = RepAuthor.AuthorID
                                 WHERE        RepAuthor.RepID = UserRepository.RepID
                                 ORDER BY AuthorOrder FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Author, STUFF
                             ((SELECT        ', ' + PI.FullName
                                 FROM            RepPI INNER JOIN
                                                          PI ON PI.PIID = RepPI.PIID
                                 WHERE        RepPI.RepID = UserRepository.RepID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS PI

                            FROM            UserRepository "))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int repID = Int32.Parse(dr["RepID"].ToString());
                    string doi = Convert.ToString(dr["DOI"].ToString());
                    RepList.Add(new Cogbytes
                    {
                        ID = repID,
                        RepoLinkGuid = Guid.Parse(dr["repoLinkGuid"].ToString()),
                        Title = Convert.ToString(dr["Title"].ToString()),
                        Date = Convert.ToString(dr["Date"].ToString()),
                        Keywords = Convert.ToString(dr["Keywords"].ToString()),
                        DOI = doi,
                        Link = Convert.ToString(dr["Link"].ToString()),
                        PrivacyStatus = Boolean.Parse(dr["PrivacyStatus"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),
                        AdditionalNotes = Convert.ToString(dr["AdditionalNotes"].ToString()),
                        AuthorString = Convert.ToString(dr["Author"].ToString()),
                        PIString = Convert.ToString(dr["PI"].ToString()),
                        Experiment = GetCogbytesExperimentList(Guid.Parse(dr["repoLinkGuid"].ToString())),
                        DataCiteURL = Convert.ToString(dr["DataCiteURL"].ToString()),
                    });
                }
            }

            return RepList;


        }


        //// Function Definition to extract all repositories 
        public List<Cogbytes> GetAllRepositories()
        {
            List<Cogbytes> RepList = new List<Cogbytes>();
            //using (DataTable dt = Dal.GetDataTableCog($@"Select * From UserRepository Where PrivacyStatus = 1 Order By DateRepositoryCreated"))
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From UserRepository Order By DateRepositoryCreated"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int repID = Int32.Parse(dr["RepID"].ToString());
                    string doi = Convert.ToString(dr["DOI"].ToString());
                    PubScreenSearch publication = GetPubScreenPaper(doi);
                    RepList.Add(new Cogbytes
                    {
                        ID = repID,
                        RepoLinkGuid = Guid.Parse(dr["repoLinkGuid"].ToString()),
                        Title = Convert.ToString(dr["Title"].ToString()),
                        Date = Convert.ToString(dr["Date"].ToString()),
                        Keywords = Convert.ToString(dr["Keywords"].ToString()),
                        DOI = doi,
                        Link = Convert.ToString(dr["Link"].ToString()),
                        PrivacyStatus = Boolean.Parse(dr["PrivacyStatus"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),
                        AdditionalNotes = Convert.ToString(dr["AdditionalNotes"].ToString()),
                        AuthourID = FillCogbytesItemArray($"Select AuthorID From RepAuthor Where RepID={repID}", "AuthorID"),
                        PIID = FillCogbytesItemArray($"Select PIID From RepPI Where RepID={repID}", "PIID"),
                        Experiment = GetCogbytesExperimentList(Guid.Parse(dr["repoLinkGuid"].ToString())),
                        Paper = publication
                    });
                }
            }

            return RepList;
        }

        public List<Experiment> GetCogbytesExperimentList(Guid repoLinkGuid)
        {

            string sqlMB = $@"Select Experiment.*, Task.Name as TaskName From Experiment
                       Inner join Task on Task.ID = Experiment.TaskID
                       Where RepoGuid = '{repoLinkGuid}'";

            var lstExperiment = new List<Experiment>();
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
            return lstExperiment;
        }

        public PubScreenSearch GetPubScreenPaper(string doi)
        {
            if (string.IsNullOrEmpty(doi))
            {
                return null;
            }

            var pubscreenService = new PubScreenService();
            var pub = new PubScreen { DOI = doi };
            var result = pubscreenService.SearchPublications(pub);
            if (result != null && result.Any())
            {
                return result[0];
            }
            return null;
        }

        public List<CogbytesSearch2> GetRepoFromCogbytesByLinkGuid(Guid repoLinkGuid)
        {
            List<CogbytesSearch2> RepList = new List<CogbytesSearch2>();
            List<Experiment> lstExperiment = new List<Experiment>();
            //List<FileUploadResult> FileList = new List<FileUploadResult>();
            string sqlMB = "";

            using (DataTable dt = Dal.GetDataTableCog($@"Select * From searchCog2 Where RepoLinkGuid = '{repoLinkGuid}'"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int repID = Int32.Parse(dr["RepID"].ToString());
                    int UploadID = Int32.Parse(dr["UploadID"].ToString());
                    int? numSubjects = null;
                    int num;
                    if (Int32.TryParse(dr["NumSubjects"].ToString(), out num))
                    {
                        numSubjects = num;
                    };

                    string doi = Convert.ToString(dr["DOI"].ToString());
                    PubScreenSearch publication = GetPubScreenPaper(doi);

                    // Loop through table UploadFile to get list of all files uploaded to each Upload section in a Repo
                    List<FileUploadResult> FileList = new List<FileUploadResult>();
                    using (DataTable ft = Dal.GetDataTableCog($@"Select * From UploadFile Where UploadID={UploadID} Order By DateUploaded"))
                    {
                        foreach (DataRow fr in ft.Rows)
                        {
                            FileList.Add(new FileUploadResult
                            {
                                UploadID = UploadID,
                                ExpID = Int32.Parse(fr["ID"].ToString()), // Hijacking ExpID for the primary key
                                UserFileName = Convert.ToString(fr["UserFileName"].ToString()),
                                SysFileName = Convert.ToString(fr["SystemFileName"].ToString()),
                                DateUpload = DateTime.Parse(fr["DateUploaded"].ToString()),
                                DateFileCreated = DateTime.Parse(fr["DateFileCreated"].ToString()),
                                FileSize = Int32.Parse(fr["FileSize"].ToString()),
                                PermanentFilePath = Convert.ToString(fr["PermanentFilePath"].ToString()),
                            });
                        }
                    }

                    RepList.Add(new CogbytesSearch2
                    {
                        RepoID = repID,
                        RepoLinkGuid = repoLinkGuid,
                        UploadID = UploadID,
                        Title = Convert.ToString(dr["Title"].ToString()),
                        Date = Convert.ToString(dr["Date"].ToString()),
                        DOI = doi,
                        Keywords = Convert.ToString(dr["Keywords"].ToString()),
                        PrivacyStatus = Boolean.Parse(dr["PrivacyStatus"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),
                        AdditionalNotes = Convert.ToString(dr["AdditionalNotes"].ToString()),
                        Link = Convert.ToString(dr["Link"].ToString()),
                        Username = Convert.ToString(dr["Username"].ToString()),
                        DateRepositoryCreated = Convert.ToString(dr["DateRepositoryCreated"].ToString()),
                        Author = Convert.ToString(dr["Author"].ToString()),
                        PI = Convert.ToString(dr["PI"].ToString()),
                        UploadName = Convert.ToString(dr["UploadName"].ToString()),
                        DateUpload = Convert.ToString(dr["DateUpload"].ToString()),
                        UploadDescription = Convert.ToString(dr["UploadDescription"].ToString()),
                        UploadAdditionalNotes = Convert.ToString(dr["UploadAdditionalNotes"].ToString()),
                        IsIntervention = Boolean.Parse(dr["IsIntervention"].ToString()),
                        InterventionDescription = Convert.ToString(dr["InterventionDescription"].ToString()),
                        Housing = Convert.ToString(dr["Housing"].ToString()),
                        LightCycle = Convert.ToString(dr["LightCycle"].ToString()),
                        TaskBattery = Convert.ToString(dr["TaskBattery"].ToString()),
                        FileType = Convert.ToString(dr["FileType"].ToString()),
                        Task = Convert.ToString(dr["Task"].ToString()),
                        Species = Convert.ToString(dr["Species"].ToString()),
                        Sex = Convert.ToString(dr["Sex"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),
                        GenoType = Convert.ToString(dr["GenoType"].ToString()),
                        Age = Convert.ToString(dr["Age"].ToString()),
                        UploadFileList = FileList,
                        Experiment = GetCogbytesExperimentList(repoLinkGuid),
                        NumSubjects = numSubjects,
                        Paper = publication
                    });

                }
            }

            return RepList;

        }


        public Cogbytes GetGuidByRepID(int repID)
        {
            Cogbytes cogbytesRepo = new Cogbytes();
            string sql = $"Select * From UserRepository Where RepID = {repID} ";
            using (IDataReader dr = Dal.GetReaderCog(CommandType.Text, sql, null))
            {
                if (dr.Read())
                {
                    cogbytesRepo.RepoLinkGuid = Guid.Parse(dr["RepoLinkGuid"].ToString());

                }

            }

            return cogbytesRepo;
        }


    }

}
