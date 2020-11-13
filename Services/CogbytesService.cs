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
            string sql = $@"Insert into Author (FirstName, LastName, Affiliation, Username) Values
                            ('{author.FirstName}', '{author.LastName}', '{author.Affiliation}', '{userEmail}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalarCog(sql).ToString());
        }


        //// Function Definition to extract list of Authors 
        public List<PubScreenAuthor> GetAuthors()
        {
            List<PubScreenAuthor> AuthorList = new List<PubScreenAuthor>();
            using (DataTable dt = Dal.GetDataTableCog($@"Select * From Author Order By AuthorID"))
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
        //// Delete publication
        //public void DeletePublicationById(int pubId)
        //{
        //    string sql = $@" 
        //                     Delete From Publication_Author Where PublicationID = {pubId};
        //                     Delete From Publication_CellType Where PublicationID = {pubId};
        //                     Delete From Publication_Disease Where PublicationID = {pubId};
        //                     Delete From Publication_Method Where PublicationID = {pubId};
        //                     Delete From Publication_NeuroTransmitter Where PublicationID = {pubId};
        //                     Delete From Publication_PaperType Where PublicationID = {pubId};
        //                     Delete From Publication_Region Where PublicationID = {pubId};
        //                     Delete From Publication_Sex Where PublicationID = {pubId};
        //                     Delete From Publication_Specie Where PublicationID = {pubId};
        //                     Delete From Publication_Strain Where PublicationID = {pubId};
        //                     Delete From Publication_SubRegion Where PublicationID = {pubId};
        //                     Delete From Publication_Task Where PublicationID = {pubId};
        //                     Delete From Publication Where id = { pubId};";

        //    Dal.ExecuteNonQueryPub(sql);
        //}

        ////************************************************************************************Adding Repository*************************************************************************************
        // Function Definition to add a new repository to database Cogbytes
        public int? AddRepository(Cogbytes repository, string Username)
        {

            string sqlRepository = $@"Insert into UserRepository (Title, Date, DOI, Keywords, PrivacyStatus, Description, AdditionalNotes, Link, Username, DateRepositoryCreated) Values
                                    ('{HelperService.EscapeSql((HelperService.NullToString(repository.Title)).Trim())}',
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
                    RepList.Add(new Cogbytes
                    {
                        ID = repID,
                        Title = Convert.ToString(dr["Title"].ToString()),
                        Date = Convert.ToString(dr["Date"].ToString()),
                        Keywords = Convert.ToString(dr["Keywords"].ToString()),
                        DOI = Convert.ToString(dr["DOI"].ToString()),
                        Link = Convert.ToString(dr["Link"].ToString()),
                        PrivacyStatus = Boolean.Parse(dr["PrivacyStatus"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),
                        AdditionalNotes = Convert.ToString(dr["AdditionalNotes"].ToString()),
                        AuthourID = FillCogbytesItemArray($"Select AuthorID From RepAuthor Where RepID={repID}", "AuthorID"),
                        PIID = FillCogbytesItemArray($"Select PIID From RepPI Where RepID={repID}", "PIID")
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

            string sqlUpload = $@"Insert into Upload (RepID, FileTypeID, Name, DateUpload, Description, AdditionalNotes, IsIntervention, InterventionDescription, ImageIds, ImageDescription, Housing, LightCycle, TaskBattery) Values
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
                                     '{HelperService.EscapeSql((HelperService.NullToString(upload.TaskBattery)).Trim())}'
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
                        AgeID = FillCogbytesItemArray($"Select AgeID From DatasetAge Where UploadID={uploadID}", "AgeID")

                    });
                }
            }

            return Uploadlist;
        }

        // Function Definition to edit a respository in database Cogbytes
        public bool EditUpload(int uploadID, CogbytesUpload upload)
        {

            string sqlUpload = $@"Update Upload set Name = @name, Description = @description, AdditionalNotes = @additionalNotes, IsIntervention = @isIntervention, InterventionDescription=@interventionDescription,
                                                                    ImageIds = @imageIds, ImageDescription=@imageDescription, Housing=@housing, LightCycle = @lightCycle, TaskBattery=@taskBattery
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
                sqlCmd += $@"Insert into DatasetSpecies (SexID, UploadID) Values ({upload.SexID[i]}, {uploadID});";
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

        //// Function definition to search publications in database
        //public List<PubScreenSearch> SearchPublications(PubScreen pubScreen)
        //{
        //    List<PubScreenSearch> lstPubScreen = new List<PubScreenSearch>();

        //    string sql = "Select * From SearchPub Where ";

        //    // Title
        //    if (!string.IsNullOrEmpty(pubScreen.Title))
        //    {
        //        sql += $@"SearchPub.Title like '%{(HelperService.EscapeSql(pubScreen.Title)).Trim()}%' AND ";
        //    }

        //    //Keywords
        //    if (!string.IsNullOrEmpty(pubScreen.Keywords))
        //    {
        //        sql += $@"SearchPub.Keywords like '%{HelperService.EscapeSql(pubScreen.Keywords)}%' AND ";
        //    }

        //    // DOI
        //    if (!string.IsNullOrEmpty(pubScreen.DOI))
        //    {
        //        sql += $@"SearchPub.DOI = '{HelperService.EscapeSql(pubScreen.DOI)}' AND ";
        //    }



        //    // search query for Author
        //    if (pubScreen.AuthourID != null && pubScreen.AuthourID.Length != 0)
        //    {
        //        if (pubScreen.AuthourID.Length == 1)
        //        {
        //            sql += $@"SearchPub.Author like '%'  + (Select CONCAT(Author.FirstName, '-', Author.LastName) From Author Where Author.ID = {pubScreen.AuthourID[0]}) +  '%' AND ";
        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.AuthourID.Length; i++)
        //            {
        //                sql += $@"SearchPub.Author like '%'  + (Select CONCAT(Author.FirstName, '-', Author.LastName) From Author Where Author.ID = {pubScreen.AuthourID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }

        //    }

        //    // search query for Year
        //    if (pubScreen.YearFrom != null && pubScreen.YearTo!=null)
        //    {
        //        sql += $@"(SearchPub.Year >= {pubScreen.YearFrom} AND SearchPub.Year <= {pubScreen.YearTo}) AND ";
        //    }

        //    if (pubScreen.YearFrom != null && pubScreen.YearTo == null)
        //    {
        //        sql += $@"(SearchPub.Year >= {pubScreen.YearFrom}) AND ";
        //    }

        //    if(pubScreen.YearTo != null && pubScreen.YearFrom == null)
        //    {
        //        sql += $@"(SearchPub.Year <= {pubScreen.YearTo}) AND ";
        //    }

        //    // search query for PaperType
        //    if (pubScreen.PaperTypeID != null)
        //    {
        //        sql += $@"SearchPub.PaperType like '%'  + (Select PaperType From PaperType Where PaperType.ID = {pubScreen.PaperTypeID}) +  '%' AND ";
        //    }

        //    // search query for Task
        //    if (pubScreen.TaskID != null && pubScreen.TaskID.Length != 0)
        //    {

        //        if (pubScreen.TaskID.Length == 1)
        //        {
        //            sql += $@"SearchPub.Task like '%'  + (Select Task From Task Where Task.ID = {pubScreen.TaskID[0]}) +  '%' AND ";
        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.TaskID.Length; i++)
        //            {
        //                sql += $@"SearchPub.Task like '%'  + (Select Task From Task Where Task.ID = {pubScreen.TaskID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }

        //    }

        //    // search query for Species
        //    if (pubScreen.SpecieID != null && pubScreen.SpecieID.Length != 0)
        //    {
        //        if (pubScreen.SpecieID.Length == 1)
        //        {
        //            sql += $@"SearchPub.Species like '%'  + (Select Species From Species Where Species.ID = {pubScreen.SpecieID[0]}) +  '%' AND ";

        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.SpecieID.Length; i++)
        //            {
        //                sql += $@"SearchPub.Species like '%'  + (Select Species From Species Where Species.ID = {pubScreen.SpecieID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }
        //    }

        //    // search query for Sex
        //    if (pubScreen.sexID != null && pubScreen.sexID.Length != 0)
        //    {
        //        if (pubScreen.sexID.Length == 1)
        //        {
        //            sql += $@"SearchPub.Sex like '%'  + (Select Sex From Sex Where Sex.ID = {pubScreen.sexID[0]}) +  '%' AND ";
        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.sexID.Length; i++)
        //            {
        //                sql += $@"SearchPub.Sex like '%'  + (Select Sex From Sex Where Sex.ID = {pubScreen.sexID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }
        //    }

        //    // search query for Strain
        //    if (pubScreen.StrainID != null && pubScreen.StrainID.Length != 0)
        //    {
        //        if (pubScreen.StrainID.Length == 1)
        //        {
        //            sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[0]}) +  '%' AND ";
        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.StrainID.Length; i++)
        //            {
        //                sql += $@"SearchPub.Strain like '%'  + (Select Strain From Strain Where Strain.ID = {pubScreen.StrainID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }
        //    }

        //    // search query for Disease
        //    if (pubScreen.DiseaseID != null && pubScreen.DiseaseID.Length != 0)
        //    {
        //        if (pubScreen.DiseaseID.Length == 1)
        //        {
        //            sql += $@"SearchPub.DiseaseModel like '%'  + (Select DiseaseModel From DiseaseModel Where DiseaseModel.ID = {pubScreen.DiseaseID[0]}) +  '%' AND ";

        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.DiseaseID.Length; i++)
        //            {
        //                sql += $@"SearchPub.DiseaseModel like '%'  + (Select DiseaseModel From DiseaseModel Where DiseaseModel.ID = {pubScreen.DiseaseID[i]}) +  '%' OR ";

        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }
        //    }

        //    // search query for BrainRegion
        //    if (pubScreen.RegionID != null && pubScreen.RegionID.Length != 0)
        //    {
        //        if (pubScreen.RegionID.Length == 1)
        //        {
        //            sql += $@"SearchPub.BrainRegion like '%'  + (Select BrainRegion From BrainRegion Where BrainRegion.ID = {pubScreen.RegionID[0]}) +  '%' AND ";

        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.RegionID.Length; i++)
        //            {
        //                sql += $@"SearchPub.BrainRegion like '%'  + (Select BrainRegion From BrainRegion Where BrainRegion.ID = {pubScreen.RegionID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }
        //    }

        //    // search query for SubRegion
        //    if (pubScreen.SubRegionID != null && pubScreen.SubRegionID.Length != 0)
        //    {
        //        if (pubScreen.SubRegionID.Length == 1)
        //        {
        //            sql += $@"SearchPub.SubRegion like '%'  + (Select SubRegion From SubRegion Where SubRegion.ID = {pubScreen.SubRegionID[0]}) +  '%' AND ";
        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.SubRegionID.Length; i++)
        //            {
        //                sql += $@"SearchPub.SubRegion like '%'  + (Select SubRegion From SubRegion Where SubRegion.ID = {pubScreen.SubRegionID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }

        //    }

        //    // search query for CellType
        //    if (pubScreen.CellTypeID != null && pubScreen.CellTypeID.Length != 0)
        //    {
        //        if (pubScreen.CellTypeID.Length == 1)
        //        {
        //            sql += $@"SearchPub.CellType like '%'  + (Select CellType From CellType Where CellType.ID = {pubScreen.CellTypeID[0]}) +  '%' AND ";

        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.CellTypeID.Length; i++)
        //            {
        //                sql += $@"SearchPub.CellType like '%'  + (Select CellType From CellType Where CellType.ID = {pubScreen.CellTypeID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }
        //    }

        //    // search query for Method
        //    if (pubScreen.MethodID != null && pubScreen.MethodID.Length != 0)
        //    {
        //        if (pubScreen.MethodID.Length == 1)
        //        {
        //            sql += $@"SearchPub.Method like '%'  + (Select Method From Method Where Method.ID = {pubScreen.MethodID[0]}) +  '%' AND ";
        //        }

        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.MethodID.Length; i++)
        //            {
        //                sql += $@"SearchPub.Method like '%'  + (Select Method From Method Where Method.ID = {pubScreen.MethodID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }
        //    }
        //    // search query for Neuro Transmitter
        //    if (pubScreen.TransmitterID != null && pubScreen.TransmitterID.Length != 0)
        //    {
        //        if (pubScreen.TransmitterID.Length == 1)
        //        {
        //            sql += $@"SearchPub.Neurotransmitter like '%'  + (Select Neurotransmitter From Neurotransmitter Where Neurotransmitter.ID = {pubScreen.TransmitterID[0]}) +  '%' AND ";
        //        }
        //        else
        //        {
        //            sql += "(";
        //            for (int i = 0; i < pubScreen.TransmitterID.Length; i++)
        //            {
        //                sql += $@"SearchPub.Neurotransmitter like '%'  + (Select Neurotransmitter From Neurotransmitter Where Neurotransmitter.ID = {pubScreen.TransmitterID[i]}) +  '%' OR ";
        //            }
        //            sql = sql.Substring(0, sql.Length - 3);
        //            sql += ") AND ";
        //        }

        //    }

        //    sql = sql.Substring(0, sql.Length - 4); // to remvoe the last NAD from the query
        //    string sqlMB = "";
        //    List<Experiment> lstExperiment = new List<Experiment>();
        //    using (DataTable dt = Dal.GetDataTablePub(sql))
        //    {

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            sqlMB = $@"Select Experiment.*, Task.Name as TaskName From Experiment
        //                       Inner join Task on Task.ID = Experiment.TaskID
        //                       Where DOI = '{Convert.ToString(dr["DOI"].ToString())}'";

        //            // empty lstExperiment list
        //            lstExperiment.Clear();
        //            using (DataTable dtExp = Dal.GetDataTable(sqlMB))
        //            {
        //                foreach (DataRow drExp in dtExp.Rows)
        //                {

        //                    lstExperiment.Add(new Experiment
        //                    {
        //                        ExpID = Int32.Parse(drExp["ExpID"].ToString()),
        //                        ExpName = Convert.ToString(drExp["ExpName"].ToString()),
        //                        StartExpDate = Convert.ToDateTime(drExp["StartExpDate"].ToString()),
        //                        TaskName = Convert.ToString(drExp["TaskName"].ToString()),
        //                        DOI = Convert.ToString(drExp["DOI"].ToString()),
        //                        Status = Convert.ToBoolean(drExp["Status"]),
        //                        TaskBattery = Convert.ToString(drExp["TaskBattery"].ToString()),

        //                    });
        //                }

        //            }

        //            lstPubScreen.Add(new PubScreenSearch
        //            {
        //                ID = Int32.Parse(dr["ID"].ToString()),
        //                Title = Convert.ToString(dr["Title"].ToString()),
        //                Keywords = Convert.ToString(dr["Keywords"].ToString()),
        //                DOI = Convert.ToString(dr["DOI"].ToString()),
        //                Year = Convert.ToString(dr["Year"].ToString()),
        //                Author = Convert.ToString(dr["Author"].ToString()),
        //                PaperType = Convert.ToString(dr["PaperType"].ToString()),
        //                Task = Convert.ToString(dr["Task"].ToString()),
        //                Species = Convert.ToString(dr["Species"].ToString()),
        //                Sex = Convert.ToString(dr["Sex"].ToString()),
        //                Strain = Convert.ToString(dr["Strain"].ToString()),
        //                DiseaseModel = Convert.ToString(dr["DiseaseModel"].ToString()),
        //                BrainRegion = Convert.ToString(dr["BrainRegion"].ToString()),
        //                SubRegion = Convert.ToString(dr["SubRegion"].ToString()),
        //                CellType = Convert.ToString(dr["CellType"].ToString()),
        //                Method = Convert.ToString(dr["Method"].ToString()),
        //                NeuroTransmitter = Convert.ToString(dr["NeuroTransmitter"].ToString()),
        //                Reference = Convert.ToString(dr["Reference"].ToString()),
        //                Experiment = new List<Experiment>(lstExperiment),


        //            });
        //            //lstExperiment.Clear();
        //        }

        //    }

        //    // search MouseBytes database to see if the dataset exists********************************************


        //    return lstPubScreen;


        //}

        //// Function definition to get all year's values in database
        //public List<PubScreenSearch> GetAllYears()
        //{
        //    List<PubScreenSearch> YearList = new List<PubScreenSearch>();
        //    using (DataTable dt = Dal.GetDataTablePub($@"Select distinct Year from Publication"))
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            YearList.Add(new PubScreenSearch
        //            {

        //                Year = Convert.ToString(dr["Year"].ToString()),
        //            });

        //        }
        //    }

        //    return YearList;
        //}

        //public PubScreen GetPaperInfoByID(int id)
        //{
        //    var pubScreen = new PubScreen();

        //    string sql = $"Select AuthorID From Publication_Author Where PublicationID ={id}";
        //    pubScreen.AuthourID = FillPubScreenItemArray(sql, "AuthorID");

        //    sql = $"Select CelltypeID From Publication_CellType Where PublicationID ={id}";
        //    pubScreen.CellTypeID = FillPubScreenItemArray(sql, "CelltypeID");

        //    sql = $"Select DiseaseID From Publication_Disease Where PublicationID ={id}";
        //    pubScreen.DiseaseID = FillPubScreenItemArray(sql, "DiseaseID");

        //    sql = $"Select MethodID From Publication_Method Where PublicationID ={id}";
        //    pubScreen.MethodID = FillPubScreenItemArray(sql, "MethodID");

        //    sql = $"Select TransmitterID From Publication_NeuroTransmitter Where PublicationID ={id}";
        //    pubScreen.TransmitterID = FillPubScreenItemArray(sql, "TransmitterID");

        //    sql = $"Select RegionID From Publication_Region Where PublicationID ={id}";
        //    pubScreen.RegionID = FillPubScreenItemArray(sql, "RegionID");

        //    sql = $"Select SexID From Publication_Sex Where PublicationID ={id}";
        //    pubScreen.sexID = FillPubScreenItemArray(sql, "SexID");

        //    sql = $"Select SpecieID From Publication_Specie Where PublicationID ={id}";
        //    pubScreen.SpecieID = FillPubScreenItemArray(sql, "SpecieID");

        //    sql = $"Select StrainID From Publication_Strain Where PublicationID ={id}";
        //    pubScreen.StrainID = FillPubScreenItemArray(sql, "StrainID");

        //    sql = $"Select SubRegionID From Publication_SubRegion Where PublicationID ={id}";
        //    pubScreen.SubRegionID = FillPubScreenItemArray(sql, "SubRegionID");

        //    sql = $"Select TaskID From Publication_Task Where PublicationID ={id}";
        //    pubScreen.TaskID = FillPubScreenItemArray(sql, "TaskID");

        //    //sql = $"Select PaperTypeID From Publication_PaperType Where PublicationID ={id}";
        //    sql = $"Select top 1 pt.PaperType From Publication_PaperType ppt inner join PaperType pt on ppt.PaperTypeID = pt.ID where ppt.PublicationID = {id}";
        //    var paperTypeVal = Dal.ExecScalarPub(sql);
        //    pubScreen.PaperType = paperTypeVal == null ? "" : paperTypeVal.ToString();

        //    sql = $"Select * From Publication Where ID ={id}";
        //    using (DataTable dt = Dal.GetDataTablePub(sql))
        //    {
        //        pubScreen.DOI = dt.Rows[0]["DOI"].ToString();
        //        pubScreen.Keywords = dt.Rows[0]["Keywords"].ToString();
        //        pubScreen.Title = dt.Rows[0]["Title"].ToString();
        //        pubScreen.Abstract = dt.Rows[0]["Abstract"].ToString();
        //        pubScreen.Year = dt.Rows[0]["Year"].ToString();
        //        pubScreen.Reference = dt.Rows[0]["Reference"].ToString();
        //        pubScreen.Source = dt.Rows[0]["Source"].ToString();
        //    }

        //    return pubScreen;
        //}

        private int?[] FillCogbytesItemArray(string sql, string fieldName)
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

        //public void ProcessOther(string inputOther, string tableOther, string fieldOther, string tblPublication,
        //                        string tblPublicationField, int PublicationID, string Username)
        //{
        //    if (!String.IsNullOrEmpty(inputOther))
        //    {
        //        List<string> ItemList = inputOther.Split(';').Select(p => p.Trim()).ToList();
        //        foreach (string item in ItemList)
        //        {
        //            string sqlOther = $@"Select ID From {tableOther} Where ltrim(rtrim({fieldOther})) = '{HelperService.NullToString(HelperService.EscapeSql(item)).Trim()}';";
        //            var IDOther = Dal.ExecScalarPub(sqlOther);

        //            if (IDOther == null)
        //            {
        //                string sqlOther2 = $@"Insert Into {tableOther} ({fieldOther}, Username) Values
        //                                    ('{HelperService.NullToString(HelperService.EscapeSql(item)).Trim()}', '{Username}'); SELECT @@IDENTITY AS 'Identity';";
        //                int IDOther2 = Int32.Parse((Dal.ExecScalarPub(sqlOther2).ToString()));

        //                string sqlOther3 = $@"Insert into {tblPublication} ({tblPublicationField}, PublicationID) Values ({IDOther2}, {PublicationID}); ";
        //                Dal.ExecuteNonQueryPub(sqlOther3);
        //            }

        //        }

        //    }
        //}


    }
}
