using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using MathNet;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Serilog;
using System.Data.SqlClient;

namespace AngularSPAWebAPI.Services
{

    public class UploadService
    {
        private readonly QualityControlService _qualityControlService;

        public UploadService()
        {
            _qualityControlService = new QualityControlService();
        }

        // Function Definition to read the file sent by the client, read the content, do some processing (e.g. Quality Control Rules), then insert it into Database
        public async Task<List<FileUploadResult>> UploadFiles(IFormFileCollection files, string TaskName, int expID, int subExpId, string ExpName,
                                                                string Username, string userID, string SessionName, int TaskID, int sessionID)
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

                    // Determining if multiple sessions for an animal in a single day are allowed
                    bool MultipleSessions = await GetMultipleSessionsAsync(expID);

                    // Extracting SubExpName , Age for the selected Subexperiment
                    string SubExpNameAge1 = await GetSubExpNameAgeAsync(subExpId);
                    // Extracting Age of Animal for the selected Subexperiemnt
                    var AnimalAge = await GetAnimalAgeAsync(subExpId);

                    // Check QC using the function defined in QualityControlService
                    bool IsUploaded1 = false;

                    (bool QC_IsQcPassed, bool QC_IsIdentifierPassed, string QC_FileUniqueID, string QC_ErrorMessage, string QC_WarningMessage, bool InsertToTblUpload, int SysAnimalID, int QC_UploadID, string QC_AnimalID) info =
                        _qualityControlService.QualityControl(TaskName, tempFileName, uploads, ExpName, expID, subExpId, AnimalAge, SessionName, MultipleSessions);

                    if (info.QC_IsQcPassed == true && info.QC_IsIdentifierPassed == true && info.InsertToTblUpload == true)
                    {
                        IsUploaded1 = true;
                    }
                    DateTime? DateUpload1 = null;
                    if (info.QC_IsIdentifierPassed == true && info.QC_IsQcPassed == true)
                    {
                        DateUpload1 = DateTime.UtcNow;
                    }

                    string pathString = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "UPLOAD", Username, expID.ToString());

                    FileUploadResult fur = new FileUploadResult
                    {
                        ExpID = expID,
                        SubExpID = subExpId,
                        AnimalID = info.SysAnimalID,
                        UserFileName = file.FileName,
                        SysFileName = tempFileName,
                        ErrorMessage = info.QC_ErrorMessage, // message from QC
                        WarningMessage = info.QC_WarningMessage, // message from QC
                        IsUploaded = IsUploaded1,
                        DateFileCreated = DateTime.UtcNow,
                        DateUpload = DateUpload1,
                        FileSize = HelperService.ConvertToNullableInt(file.Length.ToString()),
                        FileUniqueID = info.QC_FileUniqueID,
                        IsQcPassed = info.QC_IsQcPassed,
                        IsIdentifierPassed = info.QC_IsIdentifierPassed,
                        UserAnimalID = info.QC_AnimalID,
                        PermanentFilePath = pathString,
                        SubExpNameAge = SubExpNameAge1,
                        SessionName = SessionName, // this field is coming from client upload page

                    };

                    // Only objects whose errorMessage & warningMessage is not Null returned to client 
                    if (!string.IsNullOrEmpty(fur.ErrorMessage) || !string.IsNullOrEmpty(fur.WarningMessage))
                    {
                        uploadResult.Add(fur);
                    }

                    int uploadID = -1;
                    //Inserting to Upload Table in DB
                    if (info.InsertToTblUpload == true)
                    {

                        if (info.QC_UploadID == -1)
                        {
                            // Call insert function and return Upload ID if this fileinfo was not already inserted to tbl Upload
                            uploadID = await InsertUploadAsync(fur);

                        }

                        // If there is a duplicate, but multiple sessions for an animal in a day are allowed
                        else if (MultipleSessions)
                        {
                            uploadID = await InsertUploadAsync(fur);
                            await UpdateDuplicateSessionsAsync(info.QC_FileUniqueID);
                        }

                        else
                        {
                            //Update Upload table and return uploadID
                            await UpdateUploadAsync(fur, info.QC_UploadID);
                            uploadID = info.QC_UploadID;

                        }

                        // Copy the file to permanent path: UPLOAD/USERNAME/ExpID

                        System.IO.Directory.CreateDirectory(pathString);
                        if (file.Length > 0)
                        {
                            using (var fileStream1 = new FileStream(Path.Combine(pathString, tempFileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream1);
                            }
                        }

                        // Check again if IsUploaded1 is true then Insert into the following Tables:
                        //1) SessionInfo 2) SessionInfo_Dynamic 3)RBT_TouchScreen_Features
                        if (IsUploaded1 == true)
                        {
                            // write a function to get filename, filepath, uploadid as inputs and then insert to above mentioend tables
                            try
                            {
                                InsertFileData(tempFileName, pathString, uploadID, expID, userID, info.SysAnimalID, SessionName, TaskID, sessionID);
                            }
                            catch (InvalidOperationException e)
                            {
                                fur.IsUploaded = false;
                                fur.ErrorMessage = $"{e.Message} <br /><br />{e.InnerException.Message}<br />";
                                await UpdateUploadAsync(fur, uploadID);
                                uploadResult.Add(fur);
                            }
                        }
                    }
                }
            }

            return uploadResult;

        }

        public async Task<string> GetTaskNameAsync(int ExpID)
        {
            const string sql = "SELECT LTRIM(RTRIM(Task.Name)) AS TaskName FROM Task INNER JOIN Experiment ON Task.ID = Experiment.TaskID WHERE Experiment.ExpID = @ExpID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", ExpID)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return result?.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetTaskNameAsync");
                throw;
            }
        }

        public async Task<int> GetTaskIDAsync(int ExpID)
        {
            const string sql = "SELECT Task.ID AS TaskID FROM Task INNER JOIN Experiment ON Task.ID = Experiment.TaskID WHERE Experiment.ExpID = @ExpID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", ExpID)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetTaskIDAsync");
                throw;
            }
        }

        public async Task<string> GetExpNameAsync(int ExpID)
        {
            const string sql = "SELECT LTRIM(RTRIM(ExpName)) AS ExpName FROM Experiment WHERE Experiment.ExpID = @ExpID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", ExpID)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return result?.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetExpNameAsync");
                throw;
            }
        }

        // function to determine if multiple sessions of an animal in a single day are allowed
        public async Task<bool> GetMultipleSessionsAsync(int ExpID)
        {
            const string sql = "SELECT MultipleSessions FROM Experiment WHERE Experiment.ExpID = @ExpID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", ExpID)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return Convert.ToBoolean(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetMultipleSessionsAsync");
                throw;
            }
        }

        // Function to Insert To Upload Table in Database
        public async Task<int> InsertUploadAsync(FileUploadResult upload)
        {
            if (upload.FileUniqueID != null)
            {
                upload.FileUniqueID = upload.FileUniqueID.Trim();
            }

            const string sql = @"
                INSERT INTO Upload 
                (ExpID, AnimalID, SubExpID, UserFileName, SysFileName, SessionName, ErrorMessage, WarningMessage, IsUploaded, DateFileCreated, DateUpload, FileSize, FileUniqueID, IsQcPassed, IsIdentifierPassed, PermanentFilePath) 
                VALUES 
                (@ExpID, @AnimalID, @SubExpID, @UserFileName, @SysFileName, @SessionName, @ErrorMessage, @WarningMessage, @IsUploaded, @DateFileCreated, @DateUpload, @FileSize, @FileUniqueID, @IsQcPassed, @IsIdentifierPassed, @PermanentFilePath); 
                SELECT SCOPE_IDENTITY();";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", upload.ExpID),
                new SqlParameter("@AnimalID", upload.AnimalID),
                new SqlParameter("@SubExpID", upload.SubExpID),
                new SqlParameter("@UserFileName", upload.UserFileName),
                new SqlParameter("@SysFileName", upload.SysFileName),
                new SqlParameter("@SessionName", upload.SessionName),
                new SqlParameter("@ErrorMessage", upload.ErrorMessage),
                new SqlParameter("@WarningMessage", upload.WarningMessage),
                new SqlParameter("@IsUploaded", upload.IsUploaded),
                new SqlParameter("@DateFileCreated", upload.DateFileCreated),
                new SqlParameter("@DateUpload", upload.DateUpload),
                new SqlParameter("@FileSize", upload.FileSize),
                new SqlParameter("@FileUniqueID", upload.FileUniqueID),
                new SqlParameter("@IsQcPassed", upload.IsQcPassed),
                new SqlParameter("@IsIdentifierPassed", upload.IsIdentifierPassed),
                new SqlParameter("@PermanentFilePath", upload.PermanentFilePath)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in InsertUploadAsync");
                throw;
            }
        }

        //Function to update Upload Table
        public async Task UpdateUploadAsync(FileUploadResult upload, int uploadId)
        {
            if (upload.FileUniqueID != null)
            {
                upload.FileUniqueID = upload.FileUniqueID.Trim();
            }

            const string sql = @"
                UPDATE Upload 
                SET ExpID = @ExpID, AnimalID = @AnimalID, SubExpID = @SubExpID, UserFileName = @UserFileName, SysFileName = @SysFileName, 
                    SessionName = @SessionName, ErrorMessage = @ErrorMessage, WarningMessage = @WarningMessage, 
                    IsUploaded = @IsUploaded, DateFileCreated = @DateFileCreated, DateUpload = @DateUpload, FileSize = @FileSize, 
                    IsQcPassed = @IsQcPassed, IsIdentifierPassed = @IsIdentifierPassed 
                WHERE FileUniqueID = @FileUniqueID AND UploadID = @UploadID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", upload.ExpID),
                new SqlParameter("@AnimalID", upload.AnimalID),
                new SqlParameter("@SubExpID", upload.SubExpID),
                new SqlParameter("@UserFileName", upload.UserFileName),
                new SqlParameter("@SysFileName", upload.SysFileName),
                new SqlParameter("@SessionName", upload.SessionName),
                new SqlParameter("@ErrorMessage", HelperService.EscapeSql(upload.ErrorMessage)),
                new SqlParameter("@WarningMessage", HelperService.EscapeSql(upload.WarningMessage)),
                new SqlParameter("@IsUploaded", upload.IsUploaded),
                new SqlParameter("@DateFileCreated", upload.DateFileCreated),
                new SqlParameter("@DateUpload", upload.DateUpload),
                new SqlParameter("@FileSize", upload.FileSize),
                new SqlParameter("@IsQcPassed", upload.IsQcPassed),
                new SqlParameter("@IsIdentifierPassed", upload.IsIdentifierPassed),
                new SqlParameter("@FileUniqueID", upload.FileUniqueID),
                new SqlParameter("@UploadID", uploadId)
            };

            try
            {
                await Dal.ExecuteNonQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in UpdateUploadAsync");
                throw;
            }
        }


        // Function to flag duplicate fileUniqueIDs in the Upload Table
        public async Task UpdateDuplicateSessionsAsync(string fileUniqueID)
        {
            const string sql = @"
                UPDATE Upload 
                SET IsDuplicateSession = 1 
                WHERE FileUniqueID = @FileUniqueID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FileUniqueID", fileUniqueID)
            };

            try
            {
                await Dal.ExecuteNonQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in UpdateDuplicateSessionsAsync");
                throw;
            }
        }


        public async Task<bool> SetUploadAsResolvedAsync(int uploadID, string userId)
        {
            FileUploadResult fur;
            try
            {
                fur = await GetUploadByUploadIDAsync(uploadID);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetUploadByUploadIDAsync");
                return false;
            }

            List<FileUploadResult> lstFur;
            try
            {
                lstFur = await GetListUploadsByAnimalIDErrorMessegeAsync(fur.AnimalID);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetListUploadsByAnimalIDErrorMessegeAsync");
                return false;
            }

            foreach (var furInstance in lstFur)
            {
                int uploadSessionID;
                try
                {
                    uploadSessionID = await getUploadSessionIDbySessionNameAsync(furInstance.SessionName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in getUploadSessionIDbySessionNameAsync");
                    return false;
                }

                try
                {
                    InsertFileData(furInstance.SysFileName, furInstance.PermanentFilePath, furInstance.UploadID, furInstance.ExpID, userId, furInstance.AnimalID,
                                              furInstance.SessionName, furInstance.TaskID, uploadSessionID);
                }
                catch (InvalidOperationException e)
                {
                    furInstance.IsUploaded = false;
                    furInstance.ErrorMessage = $"{e.Message} <br /><br />{e.InnerException?.Message}<br />";
                    await UpdateUploadAsync(furInstance, uploadID);
                    return false;
                }

                const string sql = @"
                    UPDATE Upload 
                    SET ErrorMessage = '', WarningMessage = '', IsUploaded = 1, DateUpload = @DateUpload, 
                        IsQcPassed = 1, IsIdentifierPassed = 1 
                    WHERE UploadID = @UploadID";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DateUpload", DateTime.UtcNow),
                    new SqlParameter("@UploadID", furInstance.UploadID)
                };

                try
                {
                    await Dal.ExecuteNonQueryAsync(sql, parameters);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in ExecuteNonQueryAsync");
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> SetAsResolvedForEditedAnimalIdAsync(int animalId, string userId)
        {
            List<FileUploadResult> lstFur;
            try
            {
                lstFur = await GetListUploadsByAnimalIDErrorMessegeAsync(animalId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetListUploadsByAnimalIDErrorMessegeAsync");
                return false;
            }

            foreach (var furInstance in lstFur)
            {
                int uploadSessionID;
                try
                {
                    uploadSessionID = await getUploadSessionIDbySessionNameAsync(furInstance.SessionName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in getUploadSessionIDbySessionNameAsync");
                    return false;
                }

                try
                {
                    InsertFileData(furInstance.SysFileName, furInstance.PermanentFilePath, furInstance.UploadID, furInstance.ExpID, userId, furInstance.AnimalID,
                                              furInstance.SessionName, furInstance.TaskID, uploadSessionID);
                }
                catch (InvalidOperationException e)
                {
                    furInstance.IsUploaded = false;
                    furInstance.ErrorMessage = $"{e.Message} <br /><br />{e.InnerException?.Message}<br />";
                    await UpdateUploadAsync(furInstance, furInstance.UploadID);
                    return false;
                }

                const string sql = @"
                    UPDATE Upload 
                    SET ErrorMessage = '', WarningMessage = '', IsUploaded = 1, DateUpload = @DateUpload, 
                        IsQcPassed = 1, IsIdentifierPassed = 1 
                    WHERE UploadID = @UploadID";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DateUpload", DateTime.UtcNow),
                    new SqlParameter("@UploadID", furInstance.UploadID)
                };

                try
                {
                    await Dal.ExecuteNonQueryAsync(sql, parameters);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in ExecuteNonQueryAsync");
                    return false;
                }
            }

            return true;
        }

        private async Task<int> getUploadSessionIDbySessionNameAsync(string sessionName)
        {
            const string sql = "SELECT id FROM Upload_SessionInfo WHERE SessionName = @SessionName";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@SessionName", sessionName)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in getUploadSessionIDbySessionNameAsync");
                throw;
            }
        }

        public async Task<FileUploadResult> GetUploadByUploadIDAsync(int uploadID)
        {
            const string sql = @"
                SELECT Upload.*, Experiment.TaskID 
                FROM Upload
                INNER JOIN Experiment ON Experiment.ExpID = Upload.ExpID
                WHERE UploadID = @UploadID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UploadID", uploadID)
            };

            try
            {
                using (DataTable dt = await Dal.GetDataTableAsync(sql, parameters))
                {
                    if (dt.Rows.Count != 1)
                    {
                        throw new Exception("UploadID Not Found");
                    }

                    return ParseUploadRow(dt.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetUploadByUploadIDAsync");
                throw;
            }
        }

        public async Task<List<FileUploadResult>> GetListUploadsByAnimalIDErrorMessegeAsync(int animalID)
        {
            const string sql = @"
                SELECT Upload.*, Experiment.TaskID 
                FROM Upload
                INNER JOIN Experiment ON Experiment.ExpID = Upload.ExpID
                WHERE AnimalID = @AnimalID AND ErrorMessage LIKE 'Missing Animal Information:%'";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@AnimalID", animalID)
            };

            var retVal = new List<FileUploadResult>();

            try
            {
                using (DataTable dt = await Dal.GetDataTableAsync(sql, parameters))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        FileUploadResult uploadResult = ParseUploadRow(dr);
                        retVal.Add(uploadResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetListUploadsByAnimalIDErrorMessegeAsync");
                throw;
            }

            return retVal;
        }

        private FileUploadResult ParseUploadRow(DataRow dr)
        {
            return new FileUploadResult
            {
                UploadID = Int32.Parse(dr["UploadID"].ToString()),
                ExpID = Int32.Parse(dr["ExpID"].ToString()),
                SubExpID = Int32.Parse(dr["SubExpID"].ToString()),
                AnimalID = Int32.Parse(dr["AnimalID"].ToString()),
                UserFileName = Convert.ToString(dr["UserFileName"]),
                SysFileName = Convert.ToString(dr["SysFileName"]),
                ErrorMessage = Convert.ToString(dr["ErrorMessage"]),
                WarningMessage = Convert.ToString(dr["WarningMessage"]),
                IsUploaded = bool.Parse(dr["IsUploaded"].ToString()),
                IsDismissed = bool.Parse(dr["IsDismissed"].ToString()),
                DateUpload = HelperService.ConvertToNullableDateTime(dr["DateUpload"].ToString()),
                DateFileCreated = HelperService.ConvertToNullableDateTime(dr["DateFileCreated"].ToString()),
                FileSize = HelperService.ConvertToNullableInt(dr["FileSize"].ToString()),
                FileUniqueID = Convert.ToString(dr["FileUniqueID"]),
                FileContent = Convert.ToString(dr["FileContent"]),
                IsQcPassed = bool.Parse(dr["IsQcPassed"].ToString()),
                IsIdentifierPassed = bool.Parse(dr["IsIdentifierPassed"].ToString()),
                PermanentFilePath = Convert.ToString(dr["PermanentFilePath"]),
                SessionName = Convert.ToString(dr["SessionName"]),
                TaskID = Int32.Parse(dr["TaskID"].ToString()),
            };
        }

        // ****************************************************************************************************************************************************************
        // ****************************************************************************************************************************************************************
        #region "Permanent File Data Operations"

        private void InsertFileData(string fileName, string filePath, int uploadID, int expID, string userID, int animalId, string SessionName,
                                    int TaskID, int sessionIDUpload)
        {

            // Extract Marker Data
            List<MarkerData> lstMD = new List<MarkerData>();
            try
            {
                string path = filePath + "\\" + fileName;
                var xdoc = XDocument.Load(path);

                // Console.WriteLine(path);

                // Extract SessionInfo Data
                SessionInfo si = ExtractSessionInfo("SessionInformation", "Information", xdoc);
                si.ExpID = expID;
                si.UserID = userID;
                si.UploadID = uploadID;
                si.AnimalID = animalId;
                si.SessionName = SessionName;

                int sessionid = InsertSessionInfoToTable(si); // Should be sent to Marker Data Table in Database

                lstMD = ExtractMarkerData("MarkerData", "Marker", xdoc, sessionid, TaskID, SessionName, sessionIDUpload);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error in data.  Contents not added to database.  Please review then delete file.", e);
            }
            
            if (lstMD.Count > 0)
            {
                InsertMarketDataToTable(lstMD, SessionName);
            }

        }

        public SessionInfo ExtractSessionInfo(string Tag1, string Tag2, XDocument xdoc1)
        {

            string xpath = "/LiEvent/" + Tag1 + "/" + Tag2;
            var value = xdoc1.XPathSelectElements(xpath);

            SessionInfo si = new SessionInfo();
            List<SessionInfo_Dynamic> lstSid = new List<SessionInfo_Dynamic>();

            foreach (var val in value)
            {

                string Attribute = (((System.Xml.Linq.XContainer)val.FirstNode).LastNode).ToString();
                string AttributeVal = (((System.Xml.Linq.XElement)val.LastNode)).Value;

                switch (Attribute.ToUpper())
                {
                    case "DATABASE":
                        si.Database_Name = AttributeVal;
                        break;

                    case "DATE/TIME":

                        si.Date_Time = Convert.ToString(AttributeVal);

                        break;
                    case "ENVIRONMENT":
                        si.Environment = AttributeVal;
                        break;
                    case "MACHINE NAME":
                        si.Machine_Name = AttributeVal;
                        break;
                    case "ANALYSIS NAME":
                        si.Analysis_Name = AttributeVal;
                        break;
                    case "SCHEDULE NAME":
                        si.Schedule_Name = AttributeVal;
                        break;
                    case "GUID":
                        si.Guid = AttributeVal;
                        break;
                    case "SCHEDULE RUN ID":
                        si.Schedule_Run_ID = AttributeVal;
                        break;
                    case "VERSION":
                        si.Version = AttributeVal;
                        break;
                    case "VERSION NAME":
                        si.Version_Name = AttributeVal;
                        break;
                    case "APPLICATION_VERSION":
                        si.Application_Version = AttributeVal;
                        break;
                    case "MAX_NUMBER_TRIALS":
                        si.Max_Number_Trials = Int32.Parse(AttributeVal);
                        break;
                    case "MAX_SCHEDULE_TIME":
                        si.Max_Schedule_Time = AttributeVal;
                        break;
                    case "SCHEDULE_DESCRIPTION":
                        si.Schedule_Description = AttributeVal;
                        break;
                    case "SCHEDULE_START_TIME":
                        si.Schedule_Start_Time = AttributeVal;
                        break;
                    // Non Machine Generated Fields are Transferred to SessionInfo Dynamic Table in Datbase
                    default:
                        SessionInfo_Dynamic sid = new SessionInfo_Dynamic { Name = Attribute, Value = AttributeVal };
                        lstSid.Add(sid);
                        break;
                }

                si.SessionInfoDynamics = lstSid;

            }

            return si;

        }

        // Extrating Marker Data Info
        public List<MarkerData> ExtractMarkerData(string Tag1, string Tag2, XDocument xdoc1, int SessionInfoID, int TaskID,
                                                    string SessionName, int sessionIDUpload)
        {

            // Call function to get the list of features should be extracted from xml file and saved into the database
            List<string> lstFeatures = new List<string>();
            lstFeatures = GetlstFeature(sessionIDUpload);

            string xpath = "/LiEvent/" + Tag1 + "/" + Tag2;
            var value = xdoc1.XPathSelectElements(xpath);

            List<MarkerData> lstMD = new List<MarkerData>();

            // declare some lists only for CPT
            List<float?> lstSD = new List<float?>();
            List<int?> lstHits = new List<int?>();
            List<int?> lstMiss = new List<int?>();
            List<int?> lstMistake = new List<int?>();
            List<int?> lstcCorrectRejection = new List<int?>();

            List<float?> lstCorrectLatency = new List<float?>();
            List<float?> lstRewatdLatency = new List<float?>();
            List<float?> lstIncorLatency = new List<float?>();
            List<float?> lstCTLatency = new List<float?>();

            List<int?> lstCurrentImage = new List<int?>();
            List<int?> lstCTCorrej = new List<int?>();
            List<int?> lstCTMistakes = new List<int?>();
            List<float?> lstDistractor = new List<float?>();
            List<int?> lstITI = new List<int?>();

            Dictionary<string, float?> cptDictDistractorFeatures = new Dictionary<string, float?>();  // for sessionIDUpload==42 (cpt distractor)
            Dictionary<string, int?> SequenceDictFeatures = new Dictionary<string, int?>(); //  Extra features need to be calculated for sequence task
            Dictionary<string, float?> AutoshapeDictFeatures = new Dictionary<string, float?>(); // Extra features need to be calculated for autoshape task

            string end_summary_corrects = _qualityControlService.FeatureExtraction("MarkerData", "Marker", "End Summary - Corrects", "Results", xdoc1);

            foreach (var val in value)
            {

                int source_type_id = -1;
                float? result = null;
                float? time = null;
                float? duration = null;
                int? count = null;
                string Attribute = ((System.Xml.Linq.XElement)val.FirstNode).Value.ToString(); // Feature Name
                //  If condition to consider only features exist in lstFeatures (only important features of each task should be considered!)
                if (lstFeatures.Contains(Attribute) || lstFeatures.Count == 0)
                {
                    if (TaskID == 11) // Task is iCPT
                    {

                        if ((Attribute.Equals("End Summary - Stimulus Duration")) && (sessionIDUpload == 38 || sessionIDUpload == 39 || sessionIDUpload == 53))
                        {

                            if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                            {
                                result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                            }
                            else { result = null; }

                            lstSD.Add(result);

                        }

                        // find the values for features in the ?list when the task is cpt and session is probe3-distractor
                        if (sessionIDUpload == 42)
                        {
                            float? result_distractor = null;


                            string[] cptDistractorFeatures = {"End Summary - Hits during non-distractor trials - Count #1", "End Summary - Hits during congruent trials - Count #1",
                                                              "End Summary - Hits during incongruent trials - Count #1", "End Summary - Misses during non-distractor trials - Generic Counter",
                                                              "End Summary - Misses during congruent trials - Generic Counter", "End Summary - Misses during incongruent trials - Generic Counter",
                                                              "End Summary - Mistakes during non-distractor trials - Count #1", "End Summary - Mistakes during congruent trials - Count #1",
                                                              "End Summary - Mistakes during incongruent trials - Count #1", "End Summary - Correct Rejections during non-distractor trials - Count #1",
                                                              "End Summary - Correct Rejections during congruent trials - Count #1", "End Summary - Correct Rejections during incongruent trials - Count #1",
                                                              };
                            if (cptDistractorFeatures.Contains(Attribute))
                            {
                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result_distractor = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result_distractor = null; }

                                cptDictDistractorFeatures.Add(Attribute, result_distractor);

                            }
                        }

                        // add switch case to get the value for features occurring multiple times and save each in a separate list for icpt task
                        switch (Attribute)
                        {
                            case "trial by trial anal - Stimulus Duration":


                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }

                                lstSD.Add(result);

                                break;

                            case "trial by trial anal - Contrast":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }

                                lstSD.Add(result);

                                break;

                            case "trial by trial anal - Hits":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstHits.Add((int)result);
                                break;

                            case "trial by trial anal - Misses":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstMiss.Add((int)result);
                                break;

                            case "trial by trial anal - Mistakes":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstMistake.Add((int)result);
                                break;

                            case "trial by trial anal - Correct Rejections":
                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstcCorrectRejection.Add((int)result);
                                break;

                            case "trial by trial anal - Correct Choice Latency":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }
                                lstCorrectLatency.Add(result);

                                break;

                            case "trial by trial anal - Reward Retrieval Latency":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }
                                lstRewatdLatency.Add(result);
                                break;

                            case "trial by trial anal - Mistake Latency":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }
                                lstIncorLatency.Add(result);
                                break;

                            case "trial by trial anal - Correction Trial Mistake Latency":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }
                                lstCTLatency.Add(result);
                                break;

                            case "trial by trial anal - Current image":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstCurrentImage.Add((int)result);
                                break;

                            case "trial by trial anal - Correction Trial Correct Rejections":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstCTCorrej.Add((int)result);
                                break;

                            case "trial by trial anal - Correction Trial Mistakes":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstCTMistakes.Add((int)result);
                                break;

                            case "trial by trial anal - Distractor Time":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstDistractor.Add(result);
                                break;

                            case "trial by trial anal - ITI":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = 0; }

                                lstITI.Add((int)result);
                                break;
                        }  // End of switch case


                    }  // end of if(tskID==11) for iCPT task

                    // For Sequence Task some new features should be generated
                    if (TaskID == 15) // Task is Sequence
                    {
                        switch (Attribute)
                        {
                            case "Blank touches at grid specific - Grid 1":


                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    count = Int32.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                    count = count - Int32.Parse(end_summary_corrects);
                                }
                                else { count = null; }

                                SequenceDictFeatures.Add("True Blank Touches at Grid 1", count);

                                break;

                            case "Blank touches at grid specific - Grid 2":


                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    count = Int32.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                    count = count - Int32.Parse(end_summary_corrects);
                                }
                                else { count = null; }

                                SequenceDictFeatures.Add("True Blank Touches at Grid 2", count);

                                break;

                            case "Blank touches at grid specific - Grid 3":


                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    count = Int32.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                    count = count - Int32.Parse(end_summary_corrects);
                                }
                                else { count = null; }

                                SequenceDictFeatures.Add("True Blank Touches at Grid 3", count);

                                break;

                            case "Blank touches at grid specific - Grid 4":


                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    count = Int32.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                    count = count - Int32.Parse(end_summary_corrects);
                                }
                                else { count = null; }

                                SequenceDictFeatures.Add("True Blank Touches at Grid 4", count);

                                break;

                            case "Blank touches at grid specific - Grid 5":


                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                                {
                                    count = Int32.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                                    count = count - Int32.Parse(end_summary_corrects);
                                }
                                else { count = null; }

                                SequenceDictFeatures.Add("True Blank Touches at Grid 5", count);

                                break;
                        } // end of switch for sequence

                    } // end of if for sequecne

                    string SourceType = ((System.Xml.Linq.XElement)val.FirstNode.NextNode).Value.ToString(); // SourceType
                    switch (SourceType.ToUpper())
                    {
                        case "EVALUATION":

                            source_type_id = 1;
                            if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                            {
                                result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                            }
                            else { result = null; }

                            break;

                        case "COUNT":

                            source_type_id = 2;
                            if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                            {
                                count = Int32.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                            }
                            else { count = null; }

                            break;

                        case "MEASURE":

                            source_type_id = 3;
                            if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null)
                            {
                                time = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());


                                // if time is not null, then check for the value of duration
                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode) != null)
                                {
                                    duration = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { duration = null; }

                            }
                            else { time = null; }



                            break;

                    }

                    MarkerData MD = new MarkerData
                    {
                        SessionID = SessionInfoID,
                        SourceTypeID = source_type_id,
                        FeatureName = Attribute,
                        Results = result,
                        Time = time,
                        Duration = duration,
                        Count = count,

                    };


                    lstMD.Add(MD);

                } // end of if to consider only important features of each task


            } // and of foreach loop

            // Function call to get the dictionary with all the metrics calculated at each SD
            Dictionary<string, float?> cptFeatureDict = new Dictionary<string, float?>();  // to save all the metrics calculated at each SD, or contrast level or distractor
            if (sessionIDUpload == 40 || sessionIDUpload == 38 || sessionIDUpload == 39 || sessionIDUpload == 53)  // cpt with different SDs
            {
                cptFeatureDict = GetDictCPTFeatures(lstSD, lstHits, lstMiss, lstMistake, lstcCorrectRejection, lstCorrectLatency, lstRewatdLatency, lstIncorLatency, lstCTLatency, sessionIDUpload);
                // If distractor times are present, additional features must be added (stage 4 only for now)
                if (sessionIDUpload == 53 && lstDistractor.Any())
                {
                    cptFeatureDict = GetDictCPTFeaturesDistractorAnal(cptFeatureDict, lstCurrentImage, lstHits, lstMiss, lstMistake, lstcCorrectRejection, lstCTCorrej, lstCTMistakes,
                        lstCorrectLatency, lstIncorLatency, lstRewatdLatency, lstDistractor, lstITI);
                }
            }
            else if (sessionIDUpload == 41) // cpt with different contrast levels
            {
                cptFeatureDict = GetDictCPTFeatures_contrastLevel(lstSD, lstHits, lstMiss, lstMistake, lstcCorrectRejection, lstCorrectLatency, lstRewatdLatency, lstIncorLatency);
            }
            else if (sessionIDUpload == 42)  // cpt with distractor
            {
                cptFeatureDict = GetDictCPTFeatures_distractor(cptDictDistractorFeatures);
            }


            // For CPT task
            int sourceType = 1;
            float? resultVal = null;
            float? durationVal = null;
            foreach (KeyValuePair<string, float?> entry in cptFeatureDict)
            {
                //if (entry.Key.Contains("Correct Choice Latency") || entry.Key.Contains("Reward Retrieval Latency") || entry.Key.Contains("Incorrect Choice Latency")
                //    || entry.Key.Contains("Hit Latency") || entry.Key.Contains("False Alarm Latency") || entry.Key.Contains("Reward Latency"))
                if (entry.Key.Contains("Latency"))
                {
                    sourceType = 3;
                    durationVal = entry.Value;
                    resultVal = null;

                }
                else
                {
                    sourceType = 1;
                    resultVal = entry.Value;
                    durationVal = null;
                }

                MarkerData MD = new MarkerData
                {
                    SessionID = SessionInfoID,
                    SourceTypeID = sourceType,
                    FeatureName = entry.Key,
                    Results = resultVal,
                    Time = null,
                    Duration = durationVal,
                    Count = null,

                };

                lstMD.Add(MD);
            }

            // For Sequence Task
            int? countnVal = null;
            foreach (KeyValuePair<string, int?> entry in SequenceDictFeatures)
            {
                if (entry.Value != null)
                {
                    sourceType = 2;
                    countnVal = entry.Value;

                    MarkerData MD = new MarkerData
                    {
                        SessionID = SessionInfoID,
                        SourceTypeID = sourceType,
                        FeatureName = entry.Key,
                        Results = resultVal,
                        Time = null,
                        Duration = null,
                        Count = countnVal,

                    };

                    lstMD.Add(MD);

                }



            }

            //For Autoshape Task
            if(TaskID==13)
            {
                AutoshapeDictFeatures = CalcAutoshapeFeatures(lstMD);
            }

            // initialize some features
            int sourcetype = 0;
            float? resval = null;
            float? countval = null;
            float? timeval = null;

            string[] results_features = { "Normalized - End Summary - Touches to lit CS+", "Normalized - End Summary - Touches to lit CS-", "Normalized - End Summary - All CS+ touches", "Normalized - End Summary - All CS- touches" };
            string[] count_features = { "Normalized - End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+", "Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+",
                                        "Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-", "Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-" };
            string[] time_features = { "Normalized - End Summary - CS + Beam Breaking", "Normalized - End Summary - CS - Beam Breaking", "Normalized - End Summary - CS + Image Presentation Beam Breaking", "Normalized - End Summary - CS - Image Presentation Beam Breaking",
                                        "Normalized - End Summary - Tray Beam Breaking", "Normalized - End Summary - Tray CS + Beam Breaking", "Normalized - End Summary - Tray CS - Beam Breaking", "Normalized - End Summary - CS + Image Approach CS- Beam Breaking", "Normalized - End Summary - CS - Image Approach CS+ Beam Breaking" };

            foreach (KeyValuePair<string, float?> entry in AutoshapeDictFeatures)
            {
                if (results_features.Contains(entry.Key)) { sourcetype = 1; resval = entry.Value;  }
                if (count_features.Contains(entry.Key)) { sourcetype = 2; countval = entry.Value; }
                if (time_features.Contains(entry.Key)) { sourcetype = 3; timeval = entry.Value; }

                if (entry.Value != null)
                {
                    MarkerData MD = new MarkerData
                    {
                        SessionID = SessionInfoID,
                        SourceTypeID = sourcetype,
                        FeatureName = entry.Key,
                        Results = resval,
                        Time = null,
                        Duration = timeval,
                        Count = countval,

                    };

                    lstMD.Add(MD);
                }

            }
            // End of Autoshape

            return lstMD;
        }


        private int InsertSessionInfoToTable(SessionInfo si)
        {

            string sql = $@"select SessionID from SessionInfo where UploadID = {si.UploadID} ";
            //var sessionInfoId = Dal.ExecuteScalar(sql);
            //if (sessionInfoId != null)
            //{
            //    string sqlDelete = $@"
            //                delete from RBT_TouchScreen_Features where SessionID = {sessionInfoId}; 
            //                delete from SessionInfo_Dynamic where SessionID = {sessionInfoId};
            //                delete from SessionInfo where SessionId = {sessionInfoId}; ";
            //    Dal.ExecuteNonQuery(sqlDelete);
            //}


            sql = $@"insert into SessionInfo (
                          [ExpID]
                          ,[UploadID]
                          ,[AnimalID]
                          ,[UserID]
                          ,[Database_Name]
                          ,[Date_Time]
                          ,[Environment]
                          ,[Machine_Name]
                          ,[SessionName]
                          ,[Analysis_Name]
                          ,[Schedule_Name]
                          ,[Guid]
                          ,[Schedule_Run_ID]
                          ,[Version]
                          ,[Version_Name]
                          ,[Application_Version]
                          ,[Max_Number_Trials]
                          ,[Max_Schedule_Time]
                          ,[Schedule_Description]
                          ,[Schedule_Start_Time]
                           ) values (
                           {si.ExpID}
                           ,{si.UploadID}
                           ,{si.AnimalID}
                           ,'{si.UserID}'
                           ,'{si.Database_Name}'
                           ,'{si.Date_Time}'
                           ,'{si.Environment}'
                           ,'{si.Machine_Name}'
                           ,'{si.SessionName}'
                           ,'{si.Analysis_Name}'
                           ,'{si.Schedule_Name}'
                           ,'{si.Guid}'
                           ,'{si.Schedule_Run_ID}'
                           ,'{si.Version}'
                           ,'{si.Version_Name}'
                           ,'{si.Application_Version}'
                           ,{si.Max_Number_Trials}
                           ,'{si.Max_Schedule_Time}'
                           ,'{si.Schedule_Description}'
                           ,'{si.Schedule_Start_Time}'
                            ); SELECT @@IDENTITY AS 'Identity';";

            int sessionid = Int32.Parse(Dal.ExecScalar(sql).ToString());

            foreach (var sessionInfoDynamic in si.SessionInfoDynamics)
            {
                sql = $@"insert into SessionInfo_Dynamic (
                          [SessionID]
                          ,[Name]
                          ,[Value]
                           ) values (
                           {sessionid}
                           ,'{sessionInfoDynamic.Name}'
                           ,'{sessionInfoDynamic.Value}'
                            )";

                Dal.ExecuteNonQuery(sql);
            }

            return sessionid;

        }

        private void InsertMarketDataToTable(List<MarkerData> lstMD, string SessionName)
        {
            string sql = string.Empty;
            string stimulus_duration = string.Empty;
            string sql_stimulus = string.Empty;

            // If SessionName belongs to 5-choice and testing sessions, then extract stimulation duration and insert it to SessionInfo table in DB
            if (SessionName == "Probe" || SessionName == "Training" || SessionName == "Re_Baseline" || SessionName == "Intra_Probe")
            {

                float? stimulus1 = lstMD.Where(x => x.FeatureName == "Trial Analysis - Stimulus Duration").Average(x => x.Results);
                float? stimulus2 = lstMD.Where(x => x.FeatureName == "Threshold - Stimulus Duration").Average(x => x.Results);
                if (stimulus1 != 0 && stimulus1 != null)
                {
                    stimulus_duration = (stimulus1 * 1000).ToString();
                }
                else if (stimulus2 != 0 && stimulus2 != null)
                {
                    stimulus_duration = (stimulus2 * 1000).ToString();
                }

                if (!string.IsNullOrEmpty(stimulus_duration) && lstMD.Count > 0)
                {
                    // Insert it to table SessionInfo in DB
                    sql_stimulus = $@" Update SessionInfo Set Stimulus_Duration = '{stimulus_duration}'
                                      Where  SessionID = {lstMD[0].SessionID}";
                    Dal.ExecuteNonQuery(sql_stimulus);
                }

            }

            var table = Dal.GetDataTable($@"SELECT TOP 0 * FROM RBT_TouchScreen_Features");

            foreach (var markerData in lstMD)
            {
                var results = (markerData.Results == null) ? (object)DBNull.Value : markerData.Results.ToString();
                var time = (markerData.Time == null) ? (object)DBNull.Value : markerData.Time.ToString();
                var duration = (markerData.Duration == null) ? (object)DBNull.Value : markerData.Duration.ToString();
                var count = (markerData.Count == null) ? (object)DBNull.Value : markerData.Count.ToString();

                var row = table.NewRow();

                row["SessionID"] = markerData.SessionID;
                row["SourceTypeID"] = markerData.SourceTypeID;
                row["FeatureName"] = markerData.FeatureName;
                row["Results"] = results;
                row["Time"] = time;
                row["Duration"] = duration;
                row["Count"] = count;

                table.Rows.Add(row);

            }

            Dal.BulkInsert(table, "RBT_TouchScreen_Features");

            table.Dispose();

        }


        #endregion

        // Functions for extracting UploadLog and UploadErrorLog For Dashboard page**********************************
        public List<FileUploadResult> GetUploadInfoByExpID(int expId)
        {
            List<FileUploadResult> lstUploadLog = new List<FileUploadResult>();

            using (DataTable dt = Dal.GetDataTable($@"SELECT Upload.UploadID, Upload.AnimalID,  UserFileName, Animal.UserAnimalID, DateFileCreated, WarningMessage, 
                                                        Upload.ErrorMessage,  CONCAT(se.SubExpName, ', ' , Age.AgeInMonth, ' Months') as SubExpNameAge, se.SubExpID 
                                                        From Upload inner join Animal on Animal.AnimalID = Upload.AnimalID
                                                        inner join SubExperiment se on Upload.SubExpID = se.SubExpID
														inner join Age on Age.ID = se.AgeID
                                                        WHERE Upload.ExpID = {expId} and ((Upload.ErrorMessage!='' and Upload.ErrorMessage IS NOT NUll) OR (ISNULL(WarningMessage,'')!='')) Order By UserAnimalID;"))

            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstUploadLog.Add(new FileUploadResult
                    {
                        UploadID = Int32.Parse(dr["UploadID"].ToString()),
                        ExpID = expId,
                        SubExpID = Int32.Parse(dr["SubExpID"].ToString()),
                        AnimalID = Int32.Parse(dr["AnimalID"].ToString()),
                        UserFileName = Convert.ToString(dr["UserFileName"].ToString()),
                        UserAnimalID = Convert.ToString(dr["UserAnimalID"].ToString()),
                        DateFileCreated = HelperService.ConvertToNullableDateTime(dr["DateFileCreated"].ToString()),
                        ErrorMessage = Convert.ToString(dr["ErrorMessage"].ToString()),
                        WarningMessage = Convert.ToString(dr["WarningMessage"].ToString()),
                        SubExpNameAge = Convert.ToString(dr["SubExpNameAge"].ToString()),
                        AnimalObj = new Animal
                        {
                            ExpID = expId,
                            AnimalID = Int32.Parse(dr["AnimalID"].ToString()),
                            UserAnimalID = Convert.ToString(dr["UserAnimalID"].ToString()),


                        }

                    });
                }

            }

            return lstUploadLog;

        }

        public List<UploadErrorLog> GetUploadErrorLogByExpID(int expId)
        {
            List<UploadErrorLog> lstUploadErrorLog = new List<UploadErrorLog>();

            using (DataTable dt = Dal.GetDataTable($@"SELECT UploadErrorLog.* , CONCAT(se.SubExpName, ', ' , Age.AgeInMonth, ' Months') as SubExpNameAge  FROM UploadErrorLog
                                                        inner join SubExperiment se on UploadErrorLog.SubExpID = se.SubExpID
														inner join Age on Age.ID = se.AgeID
                                                        WHERE UploadErrorLog.ExpID = {expId};"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstUploadErrorLog.Add(new UploadErrorLog
                    {
                        ExpID = expId,
                        UserFileName = Convert.ToString(dr["UserFileName"].ToString()),
                        ErrorMessage = Convert.ToString(dr["ErrorMessage"].ToString()),
                        UploadDate = HelperService.ConvertToNullableDateTime(dr["UploadDate"].ToString()),
                        SubExpNameAge = Convert.ToString(dr["SubExpNameAge"].ToString()),
                    });
                }

            }

            return lstUploadErrorLog;

        }

        //*************************************************************************************************************************
        //**********Function Definition for extracting Data from Upload tbl and send it back to Experiment page********************
        public List<FileUploadResult> GetUploadInfoBySubExpIDForExperiemnt(int subExpId)
        {

            List<FileUploadResult> lstUploadLogForExp = new List<FileUploadResult>();

            using (DataTable dt = Dal.GetDataTable($@"SELECT Upload.UploadID, Upload.AnimalID, UserFileName, Animal.UserAnimalID, SessionName, ErrorMessage, WarningMessage, DateUpload, Upload.IsUploaded, Upload.IsDuplicateSession
                                                       From Upload inner join Animal on Animal.AnimalID = Upload.AnimalID
                                                       WHERE Upload.SubExpID = {subExpId} Order By DateUpload;"))

            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstUploadLogForExp.Add(new FileUploadResult
                    {
                        UploadID = Int32.Parse(dr["UploadID"].ToString()),
                        SubExpID = subExpId,
                        AnimalID = Int32.Parse(dr["AnimalID"].ToString()),
                        UserFileName = Convert.ToString(dr["UserFileName"].ToString()),
                        UserAnimalID = Convert.ToString(dr["UserAnimalID"].ToString()),
                        DateUpload = HelperService.ConvertToNullableDateTime(dr["DateUpload"].ToString()),
                        ErrorMessage = Convert.ToString(dr["ErrorMessage"].ToString()),
                        WarningMessage = Convert.ToString(dr["WarningMessage"].ToString()),
                        IsUploaded = bool.Parse(dr["IsUploaded"].ToString()),
                        SessionName = Convert.ToString(dr["SessionName"].ToString()),
                        IsDuplicateSession = Convert.ToBoolean(dr["IsDuplicateSession"].ToString()),
                    });
                }

            }

            return lstUploadLogForExp;

        }

        // Function Definition: Clear UploadErrorLog table
        public void ClearUploadLogTblbyID(int expID)
        {
            string sql = $@"Delete From UploadErrorLog Where ExpID={expID}";

            Dal.ExecuteNonQuery(sql);
        }

        //Function Definition to Extract SubExpNameAge From SubExperiment Table
        public async Task<string> GetSubExpNameAgeAsync(int subExpID)
        {
            const string sql = @"
                SELECT CONCAT(SubExpName, ', ', Age.AgeInMonth, ' Months') AS SubExpNameAge 
                FROM SubExperiment  
                INNER JOIN Age ON Age.ID = SubExperiment.AgeID  
                WHERE SubExpID = @SubExpID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@SubExpID", subExpID)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return result?.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetSubExpNameAgeAsync");
                throw;
            }
        }

        // Function Definition to Extract Age of Animal From the SubExperiment Table
        public async Task<string> GetAnimalAgeAsync(int subExpID)
        {
            const string sql = @"
                SELECT AgeInMonth 
                FROM SubExperiment 
                INNER JOIN Age ON Age.ID = SubExperiment.AgeID  
                WHERE SubExpID = @SubExpID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@SubExpID", subExpID)
            };

            try
            {
                var result = await Dal.ExecScalarAsync(sql, parameters);
                return result?.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetAnimalAgeAsync");
                throw;
            }
        }

        // Function definition to extract updload_sesssionInfo From DB
        public List<UploadSession> GetAllSessionInfo()
        {
            List<UploadSession> lstUploadSession = new List<UploadSession>();

            string sql = "Select * From Upload_SessionInfo;";

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstUploadSession.Add(new UploadSession
                    {
                        Id = Int32.Parse(dr["Id"].ToString()),
                        TaskID = Int32.Parse(dr["TaskID"].ToString()),
                        TaskName = Convert.ToString(dr["TaskName"].ToString()),
                        SessionName = Convert.ToString(dr["SessionName"].ToString()),
                        SessionDescription = Convert.ToString(dr["SessionDescription"].ToString()),

                    });
                }

            }

            return lstUploadSession;
        }

        // **********************Function definition to extract cpt features with different stimulation duration and stage 3 & 4
        private Dictionary<string, float?> GetDictCPTFeatures(List<float?> lstSD, List<int?> lstHits, List<int?> lstMiss, List<int?> lstMistake,
                       List<int?> lstcCorrectRejection, List<float?> lstCorrectLatency, List<float?> lstRewatdLatency, List<float?> lstIncorLatency,
                       List<float?> lstCTLatency, int uploadsessionID)
        {

            Dictionary<string, float?> cptFeatureDict = new Dictionary<string, float?>();

            var maxIndex = new List<int>() { lstSD.Count, lstHits.Count, lstMiss.Count, lstMistake.Count, lstcCorrectRejection.Count };
            var maxIndexValue = maxIndex.Max();
            var distSDValues = lstSD.Distinct();

            // if the file belongs to stage 3 and 4 for cpt task
            if (uploadsessionID == 38 || uploadsessionID == 39 || uploadsessionID == 53)
            {
                //float avgCorrectLatency = lstCorrectLatency.Average(x => Convert.ToSingle(x));
                //float avgRewardLatency = lstRewatdLatency.Average(x => Convert.ToSingle(x));
                float? avgCorrectLatency = lstCorrectLatency.Average();
                float? avgRewardLatency = lstRewatdLatency.Average();
                var titleCorrectLatency = $"Average - Correct Choice Latency at {distSDValues.ToList()[0].ToString()}s SD";
                var titleRewardLatency = $"Average - Reward Retrieval Latency at {distSDValues.ToList()[0].ToString()}s SD";

                //float avgIncorrectLatency = lstIncorLatency.Average(x => Convert.ToSingle(x));
                // var titleInCorrectLatency = $"Average - Incorrect Choice Latency at {distSDValues.ToList()[0].ToString()}s SD";
                float? avgIncorrectLatency = lstIncorLatency.Average();
                var titleInCorrectLatency = $"Average - Mistake Latency at {distSDValues.ToList()[0].ToString()}s SD";

                float? avgCTLatency = lstCTLatency.Average();
                var titleCTLatency = $"Average - Correction Trial Mistake Latency at {distSDValues.ToList()[0].ToString()}s SD";


                var sumHitNaive = lstHits.Sum(x => Convert.ToInt32(x));
                var sumHit = lstCorrectLatency.Count(x => x != null);
                var sumMiss = lstMiss.Sum(x => Convert.ToInt32(x));
                var sumMistakeNaive = lstMistake.Sum(x => Convert.ToInt32(x));
                var sumMistake = lstIncorLatency.Count(x => x != null);
                var sumCorrectRejection = lstcCorrectRejection.Sum(x => Convert.ToInt32(x));

                // Calculated features
                float hitRate, falseAlarmRate;
                if (sumMiss == 0)
                {
                    hitRate = 1.0f - 1.0f / (2.0f * sumHit);
                }
                else
                {
                    hitRate = (float)(sumHit) / (float)(sumHit + sumMiss);
                }
                if (sumMistake == 0)
                {
                    falseAlarmRate = 1.0f / (2.0f * sumCorrectRejection);
                }
                else
                {
                    falseAlarmRate = (float)sumMistake / (float)(sumMistake + sumCorrectRejection);
                }

                float? discriminationSensitivity = (float)(Normal.InvCDF(0d, 1d, hitRate) - Normal.InvCDF(0d, 1d, falseAlarmRate));
                discriminationSensitivity = double.IsInfinity((double)discriminationSensitivity) ? null : discriminationSensitivity;

                float? responseBias = (float)(-0.5 * (Normal.InvCDF(0d, 1d, hitRate) + Normal.InvCDF(0d, 1d, falseAlarmRate)));
                responseBias = double.IsInfinity((double)responseBias) ? null : responseBias;


                var titleHit = $"End Summary - Hits at {distSDValues.ToList()[0]}s SD";
                var titleMiss = $"End Summary - Miss at {distSDValues.ToList()[0]}s SD";
                var titleMistake = $"End Summary - Mistake at {distSDValues.ToList()[0]}s SD";
                var titleCorrectRejection = $"End Summary - CorrectRejection at {distSDValues.ToList()[0]}s SD";

                var titleHitRate = $"Hit Rate at {distSDValues.ToList()[0]}s";
                var titlefalseAlarmRate = $"False Alarm Rate at {distSDValues.ToList()[0]}s";
                var titleDiscriminationSensitivity = $"Discrimination Sensitivity at {distSDValues.ToList()[0]}s ";
                var titleResponseBias = $"Response Bias at {distSDValues.ToList()[0]}s";

                // Adding key/value pairs in cptFeatureDict 
                cptFeatureDict.Add(titleCorrectLatency, avgCorrectLatency);
                cptFeatureDict.Add(titleRewardLatency, avgRewardLatency);
                cptFeatureDict.Add(titleInCorrectLatency, avgIncorrectLatency);
                cptFeatureDict.Add(titleCTLatency, avgCTLatency);

                //cptFeatureDict.Add(titleHit, sumHit);
                //cptFeatureDict.Add(titleMiss, sumMiss);
                //cptFeatureDict.Add(titleMistake, sumMistake);
                //cptFeatureDict.Add(titleCorrectRejection, sumCorrectRejection);

                cptFeatureDict.Add(titleHitRate, hitRate);
                cptFeatureDict.Add(titlefalseAlarmRate, falseAlarmRate);
                cptFeatureDict.Add(titleDiscriminationSensitivity, discriminationSensitivity);
                cptFeatureDict.Add(titleResponseBias, (float?)responseBias);
            }

            else
            {



                // **********************Create a new data table based on dt datatable where  hit is greater than 0 for correct latency and reward latency
                DataTable dt_latency = new DataTable();
                dt_latency.Columns.Add("SD", typeof(float));
                dt_latency.Columns.Add("Hit", typeof(int));
                dt_latency.Columns.Add("CorrectLatency", typeof(int));
                dt_latency.Columns.Add("RewardLatency", typeof(int));

                // filling data table dt_latency
                // int cnt = 0;
                for (int j = 0; j < maxIndexValue; j++)
                {

                    //if (lstHits[j] > 0)
                    if (lstHits[j] > 0 && lstCorrectLatency[j] != null)
                    {
                        DataRow newRow = dt_latency.NewRow();
                        newRow["SD"] = lstSD.Count <= j ? (object)DBNull.Value : lstSD[j];
                        newRow["Hit"] = lstHits.Count <= j ? (object)DBNull.Value : lstHits[j];
                        // newRow["CorrectLatency"] = lstCorrectLatency.Count <= cnt ? (object)DBNull.Value : lstCorrectLatency[cnt];
                        // newRow["RewardLatency"] = lstRewatdLatency.Count <= cnt ? (object)DBNull.Value : lstRewatdLatency[cnt];
                        newRow["CorrectLatency"] = lstCorrectLatency.Count <= j ? (object)DBNull.Value : lstCorrectLatency[j];
                        newRow["RewardLatency"] = lstRewatdLatency.Count <= j ? (object)DBNull.Value : lstRewatdLatency[j];

                        dt_latency.Rows.Add(newRow);
                        // cnt = cnt + 1;
                    }

                }

                foreach (var sd in distSDValues)
                {
                    if (dt_latency.Select("SD = " + sd).Count() > 0)
                    {
                        float avgCorrectLatency = Convert.ToSingle(dt_latency.Compute("AVG(CorrectLatency)", "SD = " + sd));
                        float avgRewardLatency = Convert.ToSingle(dt_latency.Compute("AVG(RewardLatency)", "SD = " + sd));

                        var titleCorrectLatency = $"Average - Correct Choice Latency at {sd.ToString()}s SD";
                        var titleRewardLatency = $"Average - Reward Retrieval Latency at {sd.ToString()}s SD";

                        cptFeatureDict.Add(titleCorrectLatency, avgCorrectLatency);
                        cptFeatureDict.Add(titleRewardLatency, avgRewardLatency);
                    }

                }

                //************** Create a new data table based on dt datatable where  mistake is greater than 0 for incorrect Touch latency
                if (lstIncorLatency.Count > 0)
                {
                    DataTable dt_incorrect_latency = new DataTable();
                    dt_incorrect_latency.Columns.Add("SD", typeof(float));
                    dt_incorrect_latency.Columns.Add("Mistake", typeof(int));
                    dt_incorrect_latency.Columns.Add("IncorrectLatency", typeof(int));

                    // filling data table dt_latency
                    // int cnt2 = 0;
                    for (int j = 0; j < maxIndexValue; j++)
                    {

                        if (lstMistake[j] > 0 && lstIncorLatency[j] != null)
                        {
                            DataRow newRow = dt_incorrect_latency.NewRow();
                            newRow["SD"] = lstSD.Count <= j ? (object)DBNull.Value : lstSD[j];
                            newRow["Mistake"] = lstMistake.Count <= j ? (object)DBNull.Value : lstMistake[j];
                            // newRow["IncorrectLatency"] = lstIncorLatency.Count <= cnt2 ? (object)DBNull.Value : lstIncorLatency[cnt2];
                            newRow["IncorrectLatency"] = lstIncorLatency.Count <= j ? (object)DBNull.Value : lstIncorLatency[j];

                            dt_incorrect_latency.Rows.Add(newRow);
                            //cnt2 = cnt2 + 1;
                        }

                    }

                    foreach (var sd in distSDValues)
                    {
                        if (dt_incorrect_latency.Select("SD = " + sd).Count() > 0)
                        {
                            float avgIncorrectLatency = Convert.ToSingle(dt_incorrect_latency.Compute("AVG(IncorrectLatency)", "SD = " + sd));

                            var titleInCorrectLatency = $"Average - Mistake Latency at {sd.ToString()}s SD";

                            cptFeatureDict.Add(titleInCorrectLatency, avgIncorrectLatency);
                        }
                        else
                        {
                            // TODO: what needs to be done here?
                        }


                    }
                }


                //****************Create dataTable and put all the lists (hits, miss, mistake, correctRejection) inside it
                DataTable dt = new DataTable();
                dt.Columns.Add("SD", typeof(float));
                dt.Columns.Add("Hit", typeof(int));
                dt.Columns.Add("Miss", typeof(int));
                dt.Columns.Add("Mistake", typeof(int));
                dt.Columns.Add("CorrectRejection", typeof(int));


                for (int j = 0; j < maxIndexValue; j++)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["SD"] = lstSD.Count <= j ? (object)DBNull.Value : lstSD[j];
                    // newRow["Hit"] = lstHits.Count < j ? null : lstHits[j];
                    newRow["Miss"] = lstMiss.Count <= j ? (object)DBNull.Value : lstMiss[j];
                    // newRow["Mistake"] = lstMistake.Count < j ? null : lstMistake[j];
                    newRow["Hit"] = lstHits.Count <= j ? (object)DBNull.Value : (lstCorrectLatency != null ? lstHits[j] : 0);
                    newRow["Mistake"] = lstMistake.Count <= j ? (object)DBNull.Value : (lstIncorLatency != null ? lstMistake[j] : 0);
                    newRow["CorrectRejection"] = lstcCorrectRejection.Count <= j ? (object)DBNull.Value : lstcCorrectRejection[j];
                    dt.Rows.Add(newRow);
                }


                // This loop is used to extract features that occures the exact # of times that SD occures in the session. 
                foreach (var sd in distSDValues)
                {
                    if (dt.Select("SD = " + sd).Count() > 0)
                    {
                        var countSD = Convert.ToInt32(dt.Select("SD = " + sd).Count());

                        var sumHit = Convert.ToInt32(dt.Compute("Sum(Hit)", "SD = " + sd));
                        var sumMiss = Convert.ToInt32(dt.Compute("Sum(Miss)", "SD = " + sd));
                        var sumMistake = Convert.ToInt32(dt.Compute("Sum(Mistake)", "SD = " + sd));
                        var sumCorrectRejection = Convert.ToInt32(dt.Compute("Sum(CorrectRejection)", "SD = " + sd));

                        // Calculated features
                        float hitRate, falseAlarmRate;
                        if (sumMiss == 0)
                        {
                            hitRate = 1.0f - 1.0f / (2.0f * sumHit);
                        }
                        else
                        {
                            hitRate = (float)(sumHit) / (float)(sumHit + sumMiss);
                        }
                        if (sumMistake == 0)
                        {
                            falseAlarmRate = 1.0f / (2.0f * sumCorrectRejection);
                        }
                        else
                        {
                            falseAlarmRate = (float)sumMistake / (float)(sumMistake + sumCorrectRejection);
                        };

                        float? discriminationSensitivity = (float)(Normal.InvCDF(0d, 1d, hitRate) - Normal.InvCDF(0d, 1d, falseAlarmRate));
                        discriminationSensitivity = double.IsInfinity((double)discriminationSensitivity) ? null : discriminationSensitivity;

                        float? responseBias = (float)(-0.5 * (Normal.InvCDF(0d, 1d, hitRate) + Normal.InvCDF(0d, 1d, falseAlarmRate)));
                        responseBias = double.IsInfinity((double)responseBias) ? null : responseBias;

                        var titleSD = $"count at {sd.ToString()}s";
                        var titleHit = $"End Summary - Hits at {sd.ToString()}s SD";
                        var titleMiss = $"End Summary - Miss at {sd.ToString()}s SD";
                        var titleMistake = $"End Summary - Mistake at {sd.ToString()}s SD";
                        var titleCorrectRejection = $"End Summary - CorrectRejection at {sd.ToString()}s SD";

                        var titleHitRate = $"Hit Rate at {sd.ToString()}s";
                        var titlefalseAlarmRate = $"False Alarm Rate at {sd.ToString()}s";
                        var titleDiscriminationSensitivity = $"Discrimination Sensitivity at {sd.ToString()}s ";
                        var titleResponseBias = $"Response Bias at {sd.ToString()}s";


                        // Adding key/value pairs in cptFeatureDict 
                        cptFeatureDict.Add(titleSD, countSD);
                        cptFeatureDict.Add(titleHit, sumHit);
                        cptFeatureDict.Add(titleMiss, sumMiss);
                        cptFeatureDict.Add(titleMistake, sumMistake);
                        cptFeatureDict.Add(titleCorrectRejection, sumCorrectRejection);

                        cptFeatureDict.Add(titleHitRate, hitRate);
                        cptFeatureDict.Add(titlefalseAlarmRate, falseAlarmRate);
                        cptFeatureDict.Add(titleDiscriminationSensitivity, discriminationSensitivity);
                        cptFeatureDict.Add(titleResponseBias, (float?)responseBias);
                    }


                }
            }


            return cptFeatureDict;
        }

        enum DistractorState
        {
            NO_DISTRACTOR,
            DISTRACTOR_PRESENTATION,
            DISTRACTOR_0_5SEC_DELAY,
            DISTRACTOR_1SEC_DELAY
        }

        private Dictionary<string, float?> GetDictCPTFeaturesDistractorAnal(Dictionary<string, float?> cptFeatureDict, List<int?> lstCurrentImage,
                        List<int?> lstHits, List<int?> lstMiss, List<int?> lstMistake, List<int?> lstcCorrectRejection, List<int?> lstCTCorrej,
                        List<int?> lstCTMistakes, List<float?> lstCorrectLatency, List<float?> lstIncorLatency, List<float?> lstRewatdLatency,
                        List<float?> lstDistractor, List<int?> lstITI)
        {
            const double IQRRANGE = 1.5;
            List<double?> IQRVectorCheck(List<double?> dataVec, double IQRRange = IQRRANGE)
            {
                dataVec.RemoveAll(x => x == null);
                double data25 = Statistics.QuantileCustom(dataVec, 0.25, QuantileDefinition.R7);
                double data75 = Statistics.QuantileCustom(dataVec, 0.75, QuantileDefinition.R7);
                // double dataIQR = Statistics.InterquartileRange(dataVec) * IQRRange;
                double dataIQR = (data75 - data25) * IQRRange;
                dataVec.RemoveAll(x => x < (data25 - dataIQR));
                dataVec.RemoveAll(x => x > (data75 + dataIQR));
                return dataVec;
            }

            int[] sessionHit = { 0, 0, 0, 0 };
            int[] sessionMiss = { 0, 0, 0, 0 };
            int[] sessionMistake = { 0, 0, 0, 0 };
            int[] sessionCorrej = { 0, 0, 0, 0 };

            List<double?>[] sessionCorlat = new List<double?>[4];
            List<double?>[] sessionIncorlat = new List<double?>[4];
            List<double?>[] sessionRewlat = new List<double?>[4];
            for (int i = 0; i < 4; i++)
            {
                sessionCorlat[i] = new List<double?>();
                sessionIncorlat[i] = new List<double?>();
                sessionRewlat[i] = new List<double?>();
            }

            // int corlatMod = 0, incorlatMod = 0, rewlatMod = 0, preHit = 0;
            bool corrActive = false, preHitActive = true;

            int numTrials = lstCurrentImage.Count();

            int? sumHit = lstHits.Sum(); int? sumMistake = lstMistake.Sum(); int? sumCR = lstcCorrectRejection.Sum(); int? sumCTCR = lstCTCorrej.Sum(); int? sumCTMistakes = lstCTMistakes.Sum();
            int lthCorrectChoice = lstCorrectLatency.Count(); int lthIncorrectChoice = lstIncorLatency.Count(); int lthRewardLatency = lstRewatdLatency.Count();

            for (int i = 0; i < numTrials; i++)
            {
                if (lstDistractor[i] != null)
                {
                    if (!corrActive)
                    {
                        int distState = -1;
                        if (lstITI[i] == 3)
                        {
                            switch (lstDistractor[i])
                            {
                                case 0:
                                    distState = (int)DistractorState.NO_DISTRACTOR;
                                    break;

                                case 2:
                                    distState = (int)DistractorState.DISTRACTOR_1SEC_DELAY;
                                    break;

                                case 2.5f:
                                    distState = (int)DistractorState.DISTRACTOR_0_5SEC_DELAY;
                                    break;

                                case 3:
                                    distState = (int)DistractorState.DISTRACTOR_PRESENTATION;
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (lstDistractor[i])
                            {
                                case 0:
                                    distState = (int)DistractorState.NO_DISTRACTOR;
                                    break;

                                case 1:
                                    distState = (int)DistractorState.DISTRACTOR_1SEC_DELAY;
                                    break;

                                case 1.5f:
                                    distState = (int)DistractorState.DISTRACTOR_0_5SEC_DELAY;
                                    break;

                                case 2:
                                    distState = (int)DistractorState.DISTRACTOR_PRESENTATION;
                                    break;

                                default:
                                    break;
                            }
                        }

                        if (distState != -1)
                        {
                            if (lstCurrentImage[i] != 2)
                            {
                                if (lstMistake[i] == 1)
                                {
                                    if (lstIncorLatency[i] != null)
                                    {
                                        sessionMistake[distState]++;
                                        //sessionIncorlat[distState].Add(lstIncorLatency[incorlatMod++]);
                                        sessionIncorlat[distState].Add((double?)lstIncorLatency[i]);
                                        corrActive = true;
                                    }
                                }
                                else
                                {
                                    sessionCorrej[distState]++;
                                }
                            }
                            else
                            {
                                if (lstHits[i] == 1)
                                {
                                    if (lstCorrectLatency[i] != null)
                                    {
                                        sessionHit[distState]++;
                                        //sessionCorlat[distState].Add(lstCorrectLatency[corlatMod++]);
                                        //sessionRewlat[distState].Add(lstRewatdLatency[rewlatMod++]);
                                        sessionCorlat[distState].Add((double?)lstCorrectLatency[i]);
                                        sessionRewlat[distState].Add((double?)lstRewatdLatency[i]);
                                    }
                                }
                                else
                                {
                                    sessionMiss[distState]++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lstCTMistakes[i] != 1)
                        {
                            corrActive = false;
                        }
                    }

                }
            }

            for (int i = 0; i < 4; i++)
            {
                sessionCorlat[i] = IQRVectorCheck(sessionCorlat[i]);
                sessionIncorlat[i] = IQRVectorCheck(sessionIncorlat[i]);
                sessionRewlat[i] = IQRVectorCheck(sessionRewlat[i]);
            }

            double[] hitRate = new double[4];
            double[] falseAlarmRate = new double[4];
            double?[] sensitivity = new double?[4];
            double?[] responseBias = new double?[4];
            double?[] hitLatency = new double?[4];
            double?[] falseAlarmLatency = new double?[4];
            double?[] rewardLatency = new double?[4];

            for (int i = 0; i < 4; i++)
            {
                string[] distState = { "No Distractor ", "Distractor Presentation ", "Distractor 0.5s Delay ", "Distractor 1s Delay " };

                // Calculated features
                if (sessionMiss[i] == 0)
                {
                    hitRate[i] = 1.0 - 1.0 / (2.0 * sessionHit[i]);
                }
                else
                {
                    hitRate[i] = (double)sessionHit[i] / (double)(sessionHit[i] + sessionMiss[i]);
                }
                if (sessionMistake[i] == 0)
                {
                    falseAlarmRate[i] = 1.0 / (2.0 * sessionCorrej[i]);
                }
                else
                {
                    falseAlarmRate[i] = (double)sessionMistake[i] / (double)(sessionMistake[i] + sessionCorrej[i]);
                };
                //hitRate[i] = (double)sessionHit[i] / (double)(sessionHit[i] + sessionMiss[i]);
                //falseAlarmRate[i] = (double)sessionMistake[i] / (double)(sessionMistake[i] + sessionCorrej[i]);
                double? hrz = Normal.InvCDF(0, 1, hitRate[i]);
                hrz = double.IsInfinity((double)hrz) ? null : hrz;
                double? farz = Normal.InvCDF(0, 1, falseAlarmRate[i]);
                farz = double.IsInfinity((double)farz) ? null : farz;
                sensitivity[i] = hrz - farz;
                responseBias[i] = 0.5 * (hrz + farz);
                hitLatency[i] = sessionCorlat[i].Average();
                falseAlarmLatency[i] = sessionIncorlat[i].Average();
                rewardLatency[i] = sessionRewlat[i].Average();

                cptFeatureDict.Add(distState[i] + "Hit", sessionHit[i]);
                cptFeatureDict.Add(distState[i] + "Miss", sessionMiss[i]);
                cptFeatureDict.Add(distState[i] + "Mistake", sessionMistake[i]);
                cptFeatureDict.Add(distState[i] + "Correct Rejection", sessionCorrej[i]);
                cptFeatureDict.Add(distState[i] + "Hit Rate", (float?)hitRate[i]);
                cptFeatureDict.Add(distState[i] + "False Alarm Rate", (float?)falseAlarmRate[i]);
                cptFeatureDict.Add(distState[i] + "Sensitivity (d)", (float?)sensitivity[i]);
                cptFeatureDict.Add(distState[i] + "Response Bias", (float?)responseBias[i]);
                cptFeatureDict.Add(distState[i] + "Hit Latency", (float?)hitLatency[i]);
                cptFeatureDict.Add(distState[i] + "False Alarm Latency", (float?)falseAlarmLatency[i]);
                cptFeatureDict.Add(distState[i] + "Reward Latency", (float?)rewardLatency[i]);
            }

            return cptFeatureDict;
        }

        // Function definition to extract calculated metrics for cpt task when the contrast level is different
        private Dictionary<string, float?> GetDictCPTFeatures_contrastLevel(List<float?> lstSD, List<int?> lstHits, List<int?> lstMiss, List<int?> lstMistake,
                        List<int?> lstcCorrectRejection, List<float?> lstCorrectLatency, List<float?> lstRewatdLatency, List<float?> lstIncorLatency)
        {
            Dictionary<string, float?> cptFeatureDict = new Dictionary<string, float?>();

            var maxIndex = new List<int>() { lstSD.Count, lstHits.Count, lstMiss.Count, lstMistake.Count, lstcCorrectRejection.Count };
            var maxIndexValue = maxIndex.Max();
            var distSDValues = lstSD.Distinct();


            // **********************Create a new data table based on dt datatable where  hit is greater than 0 for correct latency and reward latency
            DataTable dt_latency = new DataTable();
            dt_latency.Columns.Add("SD", typeof(float));
            dt_latency.Columns.Add("Hit", typeof(int));
            dt_latency.Columns.Add("CorrectLatency", typeof(int));
            dt_latency.Columns.Add("RewardLatency", typeof(int));

            // filling data table dt_latency
            //int cnt = 0;
            for (int j = 0; j < maxIndexValue; j++)
            {

                if (lstHits[j] > 0)
                {
                    DataRow newRow = dt_latency.NewRow();
                    newRow["SD"] = lstSD.Count <= j ? (object)DBNull.Value : lstSD[j];
                    newRow["Hit"] = lstHits.Count <= j ? (object)DBNull.Value : lstHits[j];
                    //newRow["CorrectLatency"] = lstCorrectLatency.Count <= cnt ? (object)DBNull.Value : lstCorrectLatency[cnt];
                    //newRow["RewardLatency"] = lstRewatdLatency.Count <= cnt ? (object)DBNull.Value : lstRewatdLatency[cnt];
                    newRow["CorrectLatency"] = lstCorrectLatency.Count <= j ? (object)DBNull.Value : lstCorrectLatency[j];
                    newRow["RewardLatency"] = lstRewatdLatency.Count <= j ? (object)DBNull.Value : lstRewatdLatency[j];

                    dt_latency.Rows.Add(newRow);
                    //cnt = cnt + 1;
                }

            }

            double contrastLevel = 0;
            foreach (var sd in distSDValues)
            {
                if (dt_latency.Select("SD = " + sd).Count() > 0)
                {
                    float avgCorrectLatency = Convert.ToSingle(dt_latency.Compute("AVG(CorrectLatency)", "SD = " + sd));
                    float avgRewardLatency = Convert.ToSingle(dt_latency.Compute("AVG(RewardLatency)", "SD = " + sd));

                    switch (sd)
                    {
                        case 1:
                            contrastLevel = 100;
                            break;
                        case 2:
                            contrastLevel = 50;
                            break;
                        case 3:
                            contrastLevel = 25;
                            break;
                        case 4:
                            contrastLevel = 12.5;
                            break;

                    }
                    var titleCorrectLatency = $"Average - Correct Choice Latency at {contrastLevel.ToString()} contrast level";
                    var titleRewardLatency = $"Average - Reward Retrieval Latency at {contrastLevel.ToString()} contrast level";

                    cptFeatureDict.Add(titleCorrectLatency, avgCorrectLatency);
                    cptFeatureDict.Add(titleRewardLatency, avgRewardLatency);
                }

            }

            //************** Create a new data table based on dt datatable where  mistake is greater than 0 for incorrect Touch latency
            DataTable dt_incorrect_latency = new DataTable();
            dt_incorrect_latency.Columns.Add("SD", typeof(float));
            dt_incorrect_latency.Columns.Add("Mistake", typeof(int));
            dt_incorrect_latency.Columns.Add("IncorrectLatency", typeof(int));

            // filling data table dt_latency
            // int cnt2 = 0;
            for (int j = 0; j < maxIndexValue; j++)
            {

                if (lstMistake[j] > 0)
                {
                    DataRow newRow = dt_incorrect_latency.NewRow();
                    newRow["SD"] = lstSD.Count <= j ? (object)DBNull.Value : lstSD[j];
                    newRow["Mistake"] = lstMistake.Count <= j ? (object)DBNull.Value : lstMistake[j];
                    // newRow["IncorrectLatency"] = lstIncorLatency.Count <= cnt2 ? (object)DBNull.Value : lstIncorLatency[cnt2];
                    newRow["IncorrectLatency"] = lstIncorLatency.Count <= j ? (object)DBNull.Value : lstIncorLatency[j];

                    dt_incorrect_latency.Rows.Add(newRow);
                    // cnt2 = cnt2 + 1;
                }

            }

            foreach (var sd in distSDValues)
            {
                if (dt_incorrect_latency.Select("SD = " + sd).Count() > 0)
                {
                    float avgIncorrectLatency = Convert.ToSingle(dt_incorrect_latency.Compute("AVG(IncorrectLatency)", "SD = " + sd));

                    switch (sd)
                    {
                        case 1:
                            contrastLevel = 100;
                            break;
                        case 2:
                            contrastLevel = 50;
                            break;
                        case 3:
                            contrastLevel = 25;
                            break;
                        case 4:
                            contrastLevel = 12.5;
                            break;

                    }

                    var titleInCorrectLatency = $"Average - Incorrect Choice Latency at {contrastLevel.ToString()} contrast level";

                    cptFeatureDict.Add(titleInCorrectLatency, avgIncorrectLatency / 1000000);
                }


            }


            //****************Create dataTable and put all the lists (hits, miss, mistake, correctRejection) inside it
            DataTable dt = new DataTable();
            dt.Columns.Add("SD", typeof(float));
            dt.Columns.Add("Hit", typeof(int));
            dt.Columns.Add("Miss", typeof(int));
            dt.Columns.Add("Mistake", typeof(int));
            dt.Columns.Add("CorrectRejection", typeof(int));


            for (int j = 0; j < maxIndexValue; j++)
            {
                DataRow newRow = dt.NewRow();
                newRow["SD"] = lstSD.Count <= j ? null : lstSD[j];
                newRow["Hit"] = lstHits.Count <= j ? null : lstHits[j];
                newRow["Miss"] = lstMiss.Count <= j ? null : lstMiss[j];
                newRow["Mistake"] = lstMistake.Count <= j ? null : lstMistake[j];
                newRow["CorrectRejection"] = lstcCorrectRejection.Count <= j ? null : lstcCorrectRejection[j];
                dt.Rows.Add(newRow);
            }

            // This loop is used to extract features that occures the exact # of times that SD occures in the session. 
            foreach (var sd in distSDValues)
            {
                if (dt.Select("SD = " + sd).Count() > 0)
                {
                    var countSD = Convert.ToInt32(dt.Select("SD = " + sd).Count());
                    var sumHitNaive = lstHits.Sum(x => Convert.ToInt32(x));
                    var sumHit = lstCorrectLatency.Count(x => x != null);
                    var sumMiss = lstMiss.Sum(x => Convert.ToInt32(x));
                    var sumMistakeNaive = lstMistake.Sum(x => Convert.ToInt32(x));
                    var sumMistake = lstIncorLatency.Count(x => x != null);
                    var sumCorrectRejection = lstcCorrectRejection.Sum(x => Convert.ToInt32(x));

                    // Calculated features
                    float hitRate, falseAlarmRate;
                    if (sumMiss == 0)
                    {
                        hitRate = 1.0f - 1.0f / (2.0f * sumHit);
                    }
                    else
                    {
                        hitRate = (float)(sumHit) / (float)(sumHit + sumMiss);
                    }
                    if (sumMistake == 0)
                    {
                        falseAlarmRate = 1.0f / (2.0f * sumCorrectRejection);
                    }
                    else
                    {
                        falseAlarmRate = (float)sumMistake / (float)(sumMistake + sumCorrectRejection);
                    };

                    float? discriminationSensitivity = (float)(Normal.InvCDF(0d, 1d, hitRate) - Normal.InvCDF(0d, 1d, falseAlarmRate));
                    discriminationSensitivity = double.IsInfinity((double)discriminationSensitivity) ? null : discriminationSensitivity;

                    float? responseBias = (float)(-0.5 * (Normal.InvCDF(0d, 1d, hitRate) + Normal.InvCDF(0d, 1d, falseAlarmRate)));
                    responseBias = double.IsInfinity((double)responseBias) ? null : responseBias;

                    switch (sd)
                    {
                        case 1:
                            contrastLevel = 100;
                            break;
                        case 2:
                            contrastLevel = 50;
                            break;
                        case 3:
                            contrastLevel = 25;
                            break;
                        case 4:
                            contrastLevel = 12.5;
                            break;

                    }

                    var titleHitRate = $"Hit Rate at {contrastLevel.ToString()} contrast level";
                    var titlefalseAlarmRate = $"False Alarm Rate at {contrastLevel.ToString()} contrast level";
                    var titleDiscriminationSensitivity = $"Discrimination Sensitivity at {contrastLevel.ToString()} contrast level";
                    var titleResponseBias = $"Response Bias at {contrastLevel.ToString()} contrast level";


                    cptFeatureDict.Add(titleHitRate, hitRate);
                    cptFeatureDict.Add(titlefalseAlarmRate, falseAlarmRate);
                    cptFeatureDict.Add(titleDiscriminationSensitivity, discriminationSensitivity);
                    cptFeatureDict.Add(titleResponseBias, (float?)responseBias);


                }

            }

            return cptFeatureDict;
        }

        // function definiton to extract calculated metrics for cpt with different distractor
        private Dictionary<string, float?> GetDictCPTFeatures_distractor(Dictionary<string, float?> cptDictDistractorFeatures)
        {
            Dictionary<string, float?> cptFeatureDict = new Dictionary<string, float?>();

            // non-distractor trials
            float? nonDistHitRate, nonDisFalseALarmRate;
            if (cptDictDistractorFeatures["End Summary - Misses during non-distractor trials - Generic Counter"] == 0)
            {
                nonDistHitRate = 1.0f - 1.0f / (2.0f * cptDictDistractorFeatures["End Summary - Hits during non-distractor trials - Count #1"]);
            }
            else
            {
                nonDistHitRate = cptDictDistractorFeatures["End Summary - Hits during non-distractor trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during non-distractor trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during non-distractor trials - Generic Counter"]);
            }
            if (cptDictDistractorFeatures["End Summary - Mistakes during non-distractor trials - Count #1"] == 0)
            {
                nonDisFalseALarmRate = 1.0f / (2.0f * cptDictDistractorFeatures["End Summary - Correct Rejections during non-distractor trials - Count #1"]);
            }
            else
            {
                nonDisFalseALarmRate = (float)cptDictDistractorFeatures["End Summary - Mistakes during non-distractor trials - Count #1"] / (float)(cptDictDistractorFeatures["End Summary - Mistakes during non-distractor trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during non-distractor trials - Count #1"]);
            };

            //float? nonDistHitRate = cptDictDistractorFeatures["End Summary - Hits during non-distractor trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during non-distractor trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during non-distractor trials - Generic Counter"]);
            //float? nonDisFalseALarmRate = cptDictDistractorFeatures["End Summary - Mistakes during non-distractor trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Mistakes during non-distractor trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during non-distractor trials - Count #1"]);

            float? nonDisdiscriminationSensitivity = (float)(Normal.InvCDF(0d, 1d, (float)nonDistHitRate) - Normal.InvCDF(0d, 1d, (float)nonDisFalseALarmRate));
            nonDisdiscriminationSensitivity = double.IsInfinity((double)nonDisdiscriminationSensitivity) ? null : nonDisdiscriminationSensitivity;

            float? nonDistresponseBias = (float)(-0.5 * (Normal.InvCDF(0d, 1d, (float)nonDistHitRate) + Normal.InvCDF(0d, 1d, (float)nonDisFalseALarmRate)));
            nonDistresponseBias = double.IsInfinity((double)nonDistresponseBias) ? null : nonDistresponseBias;

            var nonDistTitleHitRate = $"Hit Rate during non-distractor trials";
            var nonDistTitlefalseAlarmRate = $"False Alarm Rate during non-distractor trials";
            var nonDistTitleDiscriminationSensitivity = $"Discrimination Sensitivity during non-distractor trials";
            var nonDistTitleResponseBias = $"Response Bias during non-distractor trials";

            cptFeatureDict.Add(nonDistTitleHitRate, (float)nonDistHitRate);
            cptFeatureDict.Add(nonDistTitlefalseAlarmRate, (float)nonDisFalseALarmRate);
            cptFeatureDict.Add(nonDistTitleDiscriminationSensitivity, (float)nonDisdiscriminationSensitivity);
            cptFeatureDict.Add(nonDistTitleResponseBias, (float)nonDistresponseBias);

            // congruent trials
            float? CongDistHitRate, CongFalseALarmRate;
            if (cptDictDistractorFeatures["End Summary - Misses during congruent trials - Generic Counter"] == 0)
            {
                CongDistHitRate = 1.0f - 1.0f / (2.0f * cptDictDistractorFeatures["End Summary - Hits during congruent trials - Count #1"]);
            }
            else
            {
                CongDistHitRate = cptDictDistractorFeatures["End Summary - Hits during congruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during congruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during congruent trials - Generic Counter"]);
            }
            if (cptDictDistractorFeatures["End Summary - Mistakes during congruent trials - Count #1"] == 0)
            {
                CongFalseALarmRate = 1.0f / (2.0f * cptDictDistractorFeatures["End Summary - Correct Rejections during congruent trials - Count #1"]);
            }
            else
            {
                CongFalseALarmRate = (float)cptDictDistractorFeatures["End Summary - Mistakes during congruent trials - Count #1"] / (float)(cptDictDistractorFeatures["End Summary - Mistakes during congruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during congruent trials - Count #1"]);
            };
            //float? CongDistHitRate = cptDictDistractorFeatures["End Summary - Hits during congruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during congruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during congruent trials - Generic Counter"]);
            //float? CongFalseALarmRate = cptDictDistractorFeatures["End Summary - Mistakes during congruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Mistakes during congruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during congruent trials - Count #1"]);
            float? CongdiscriminationSensitivity = (float)(Normal.InvCDF(0d, 1d, (float)CongDistHitRate) - Normal.InvCDF(0d, 1d, (float)CongFalseALarmRate));
            float? CongresponseBias = (float)(-0.5 * (Normal.InvCDF(0d, 1d, (float)CongDistHitRate) + Normal.InvCDF(0d, 1d, (float)CongFalseALarmRate)));

            var CongTitleHitRate = $"Hit Rate at during congruent trials";
            var CongTitlefalseAlarmRate = $"False Alarm Rate during congruent trials";
            var CongTitleDiscriminationSensitivity = $"Discrimination Sensitivity during congruent trials";
            var CongTitleResponseBias = $"Response Bias during congruent trials";

            cptFeatureDict.Add(CongTitleHitRate, (float)CongDistHitRate);
            cptFeatureDict.Add(CongTitlefalseAlarmRate, (float)CongFalseALarmRate);
            cptFeatureDict.Add(CongTitleDiscriminationSensitivity, (float)CongdiscriminationSensitivity);
            cptFeatureDict.Add(CongTitleResponseBias, (float)CongresponseBias);

            // incongruent trials
            float? inCongDistHitRate, inCongFalseALarmRate;
            if (cptDictDistractorFeatures["End Summary - Misses during incongruent trials - Generic Counter"] == 0)
            {
                inCongDistHitRate = 1.0f - 1.0f / (2.0f * cptDictDistractorFeatures["End Summary - Hits during incongruent trials - Count #1"]);
            }
            else
            {
                inCongDistHitRate = cptDictDistractorFeatures["End Summary - Hits during incongruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during incongruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during incongruent trials - Generic Counter"]);
            }
            if (cptDictDistractorFeatures["End Summary - Mistakes during incongruent trials - Count #1"] == 0)
            {
                inCongFalseALarmRate = 1.0f / (2.0f * cptDictDistractorFeatures["End Summary - Correct Rejections during incongruent trials - Count #1"]);
            }
            else
            {
                inCongFalseALarmRate = (float)cptDictDistractorFeatures["End Summary - Mistakes during incongruent trials - Count #1"] / (float)(cptDictDistractorFeatures["End Summary - Mistakes during incongruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during incongruent trials - Count #1"]);
            };
            //float? inCongDistHitRate = cptDictDistractorFeatures["End Summary - Hits during incongruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during incongruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during incongruent trials - Generic Counter"]);
            //float? inCongFalseALarmRate = cptDictDistractorFeatures["End Summary - Mistakes during incongruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Mistakes during incongruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during incongruent trials - Count #1"]);
            float? inCongdiscriminationSensitivity = (float)(Normal.InvCDF(0d, 1d, (float)inCongDistHitRate) - Normal.InvCDF(0d, 1d, (float)inCongFalseALarmRate));
            float? inCongresponseBias = (float)(-0.5 * (Normal.InvCDF(0d, 1d, (float)inCongDistHitRate) + Normal.InvCDF(0d, 1d, (float)inCongFalseALarmRate)));

            var inCongTitleHitRate = $"Hit Rate at during incongruent trials";
            var inCongTitlefalseAlarmRate = $"False Alarm Rate during incongruent trials";
            var inCongTitleDiscriminationSensitivity = $"Discrimination Sensitivity during incongruent trials";
            var inCongTitleResponseBias = $"Response Bias during incongruent trials";

            cptFeatureDict.Add(inCongTitleHitRate, (float)inCongDistHitRate);
            cptFeatureDict.Add(inCongTitlefalseAlarmRate, (float)inCongFalseALarmRate);
            cptFeatureDict.Add(inCongTitleDiscriminationSensitivity, (float?)inCongdiscriminationSensitivity);
            cptFeatureDict.Add(inCongTitleResponseBias, (float?)inCongresponseBias);

            return cptFeatureDict;
        }

        private Dictionary<string, float?> CalcAutoshapeFeatures(List<MarkerData> lstMD)
        {

            Dictionary<string, float?> autoshapeDictFeatures = new Dictionary<string, float?>(); // Extra features need to be calculated for autoshape task

            float? number_all_trials = lstMD.Where(x => x.FeatureName == "End Summary - Total Trials").Select(x => x.Results).FirstOrDefault();
            int? number_plus_trials = lstMD.Where(x => (x.FeatureName == "Trial Analysis - Reward Given - Condition") && (x.Count != null)).Count();
            float? total_length = lstMD.Where(x => x.FeatureName == "End Summary - Condition").Select(x => x.Results).FirstOrDefault();
            float? number_minus_trials = 0;

            float? End_Summary_Touches_lit_CS_plus = lstMD.Where(x => x.FeatureName == "End Summary - Touches to lit CS+").Select(x => x.Results).FirstOrDefault();
            float? End_Summary_Touches_lit_CS_minus = lstMD.Where(x => x.FeatureName == "End Summary - Touches to lit CS-").Select(x => x.Results).FirstOrDefault();
            float? End_Summary_All_CS_plus_touches = lstMD.Where(x => x.FeatureName == "End Summary - All CS+ touches").Select(x => x.Results).FirstOrDefault();
            float? End_Summary_All_CS_minus_touches = lstMD.Where(x => x.FeatureName == "End Summary - All CS- touches").Select(x => x.Results).FirstOrDefault();

            float? End_Summary_Tray_Entries_during_first_5s_CS_plus_lit = lstMD.Where(x => x.FeatureName == "End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+").Select(x => x.Count).FirstOrDefault();
            float? End_Summary_Tray_Entries_during_last_5s_CS_plus_lit = lstMD.Where(x => x.FeatureName == "End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+").Select(x => x.Count).FirstOrDefault();
            float? End_Summary_Tray_Entries_during_first_5s_CS_minus_lit = lstMD.Where(x => x.FeatureName == "End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-").Select(x => x.Count).FirstOrDefault();
            float? End_Summary_Tray_Entries_during_last_5s_CS_minus_lit = lstMD.Where(x => x.FeatureName == "End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-").Select(x => x.Count).FirstOrDefault();

           // For features occurring more than once in the xml file, first apply sum then divide it to the cs+/cs- trials OR total length
            float? End_Summary_CS_plus_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - CS + Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_CS_minus_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - CS - Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_CS_plus_Image_Presentation_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - CS + Image Presentation Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_CS_minus_Image_Presentation_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - CS - Image Presentation Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_Tray_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - Tray Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_Tray_CS_plus_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - Tray CS + Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_Tray_CS_minus_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - Tray CS - Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_CS_plus_Image_Approach_CS_minus_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - CS + Image Approach CS- Beam Breaking").Sum(x => x.Duration));
            float? End_Summary_CS_minus_Image_Approach_CS_plus_Beam_Breaking = Convert.ToSingle(lstMD.Where(x => x.FeatureName == "End Summary - CS - Image Approach CS+ Beam Breaking").Sum(x => x.Duration));

            if (number_all_trials !=null && number_plus_trials !=null)
            {
                number_minus_trials = number_all_trials - number_plus_trials;
                
                autoshapeDictFeatures.Add("Normalized - End Summary - Touches to lit CS+", End_Summary_Touches_lit_CS_plus/ number_plus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - Touches to lit CS-", End_Summary_Touches_lit_CS_minus / number_minus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - All CS+ touches", (End_Summary_All_CS_plus_touches / total_length) * 10);
                autoshapeDictFeatures.Add("Normalized - End Summary - All CS- touches", (End_Summary_All_CS_minus_touches / total_length) * 10);

                autoshapeDictFeatures.Add("Normalized - End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+", End_Summary_Tray_Entries_during_first_5s_CS_plus_lit / number_plus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+", End_Summary_Tray_Entries_during_last_5s_CS_plus_lit / number_plus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-", End_Summary_Tray_Entries_during_first_5s_CS_minus_lit / number_minus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-", End_Summary_Tray_Entries_during_last_5s_CS_minus_lit / number_minus_trials);

                autoshapeDictFeatures.Add("Normalized - End Summary - CS + Beam Breaking", (End_Summary_CS_plus_Beam_Breaking / total_length) * 10 );
                autoshapeDictFeatures.Add("Normalized - End Summary - CS - Beam Breaking", (End_Summary_CS_minus_Beam_Breaking / total_length) * 10);
                autoshapeDictFeatures.Add("Normalized - End Summary - CS + Image Presentation Beam Breaking", End_Summary_CS_plus_Image_Presentation_Beam_Breaking / number_plus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - CS - Image Presentation Beam Breaking", End_Summary_CS_minus_Image_Presentation_Beam_Breaking / number_minus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - Tray Beam Breaking", (End_Summary_Tray_Beam_Breaking / total_length) * 10);
                autoshapeDictFeatures.Add("Normalized - End Summary - Tray CS + Beam Breaking", End_Summary_Tray_CS_plus_Beam_Breaking / number_plus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - Tray CS - Beam Breaking", End_Summary_Tray_CS_minus_Beam_Breaking / number_minus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - CS + Image Approach CS- Beam Breaking", End_Summary_CS_plus_Image_Approach_CS_minus_Beam_Breaking / number_plus_trials);
                autoshapeDictFeatures.Add("Normalized - End Summary - CS - Image Approach CS+ Beam Breaking", End_Summary_CS_minus_Image_Approach_CS_plus_Beam_Breaking / number_minus_trials);
            }

            return autoshapeDictFeatures;

        }

        // function definition to get the list of features that should be extracted from xml file and saved into the database
        public List<string> GetlstFeature(int sessionIDUpload)
        {
            List<string> lstFeatures = new List<string>();

            switch (sessionIDUpload)
            {
                //***********Pre-training sessions
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    string[] preTraininglst = {"End Summary - Condition", "Reward Collection Latency", "End Summary - No. images", "End Summary - Corrects",
                                                "Correct touch latency", "Correct Reward Collection", "End Summary - Trials Completed", "End Summary - No Correction Trials",
                                                "End Summary - % Correct", "Trial Analysis - Reward Collection Latency"};
                    lstFeatures.AddRange(preTraininglst);
                    break;
                //***************5-choice
                case 7:
                case 8:
                case 9:
                case 10:
                    string[] fiveChoicelst = {"Threshold - Condition", "Threshold - Accuracy %", "Threshold - Omission %", "Threshold - Trials", "Trial Analysis - Accuracy%",
                                              "Trial Analysis - Omission%", "Trial Analysis - Correct", "Trial Analysis - Incorrect", "Trial Analysis - Omission",
                                              "Trial Analysis - Premature", "Threshold - Stimulus Duration", "Trial Analysis - Stimulus Duration", "Trial Analysis - Reward Collection Latency",
                                              "Trial Analysis - Correct Response Latency", "Trial Analysis - Incorrect Response Latency", "Omissions - Total", "Premature Responses - Total",
                                              "Premature Responses - Total", "Perseverative Correct - Total", "Perseverative Incorrect - Total", "Reward Collection Latency",
                                              "Trial Analysis - Time To Distraction", "Trial Analysis - Correct Resp Latency with No Distract"};

                    lstFeatures.AddRange(fiveChoicelst);
                    break;
                //*****************PAL
                case 11:
                case 12:
                    string[] PALlst = { "End Summary - Condition", "End Summary - Trials Completed", "End Summary - % Correct", "End Summary - Incorrect Touches", "Correct touch latency",
                                       "Incorrect Touch Latency", "Correct Reward Collection"};

                    lstFeatures.AddRange(PALlst);
                    break;
                //******************PD
                case 13:
                case 14:
                case 15:
                case 16:
                    string[] PDlst = { "End Summary - Condition", "End Summary - Trials Completed", "End Summary - No Correction Trials", "End Summary - % Correct",
                                       "Correct touch latency", "Incorrect Touch Latency", "Correct Reward Collection" };

                    lstFeatures.AddRange(PDlst);
                    break;
                //******************PR
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                    string[] PRlst = { "END SUMMARY - Schedule Length", "END SUMMARY - TRIALS COMPLETED", "END SUMMARY - Breakpoint", "END SUMMARY - Number of target touches",
                                       "END SUMMARY - REWARD COLLECTION LATENCY", "END SUMMARY - Revised total response time",
                                       "END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)",
                                       "END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)",
                                       "End Summary - Condition", "End Summary - Corrects", "End Summary - Blank Touches",
                                        "Correct touch latency", "Blank Touch Latency", "Correct Centre touch latency", "Correct Reward Collection"};
                    lstFeatures.AddRange(PRlst);
                    break;
                //******************PRL
                case 33:
                case 34:
                case 35:
                    string[] PRLlst = { "Whole session analysis - Condition", "Whole session analysis - Trials completed", "Whole session analysis - Optimal side chosen",
                                        "Whole session analysis - REWARD COLLECTION LATENCY", "Whole session analysis - Image response choice latency", "Whole session analysis - Number of Reversals",
                                        "Trial by trial analysis - One trial", "Trial by trial analysis - Trial type", "Trial by trial analysis - Spurious feedback given",
                                        "Trial by trial analysis - Spurious with milkshake", "Trial by trial analysis - Spurious no milkshake", "Trial by trial analysis - Optimal side chosen",
                                        "Trial by trial analysis - Optimal side chosen", "Trial by trial analysis - Optimal side chosen with milkshake given",
                                        "Trial by trial analysis - Optimal side chosen without milkshake given", "Trial by trial analysis - Left chosen", "Trial by trial analysis - Right chosen",
                                        "Trial by trial analysis - REWARD COLLECTION LATENCY", "Trial by trial analysis - Image response choice latency" };

                    lstFeatures.AddRange(PRLlst);
                    break;

                //*****************CPT
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 53: // NEW STAGE 4 FOR AUDIO DISTRACTOR!
                case 44:
                    string[] input_CPT = { "End Summary - Schedule length", "End Summary - No of non correction trials", "End Summary - Hits",
                                       "End Summary - Misses", "End Summary - Mistakes", "End Summary - Correct Rejections", "End Summary - Correct Image",
                                       // "Correct Choice Latency", "Incorrect Choice Latency", "Reward Retrieval Latency",
                                       "trial by trial anal - Current image",
                                       "trial by trial anal - Stimulus Duration", "trial by trial anal - Hits", "trial by trial anal - Misses", "trial by trial anal - Mistakes",
                                       "trial by trial anal - Correct Rejections", "End Summary - After Reward Pause", "trial by trial anal - ITI",
                                       "End Summary - Correction Trial Correct Rejections", "End Summary - Correction Trial Mistakes",
                                       "End Summary - Hits at 100% contrast - Count #1", "End Summary - Hits at 50% contrast - Count #1", "End Summary - Hits at 25% contrast - Count #1", "End Summary - Hits at 12.5% contrast - Count #1" ,
                                       "End Summary - Misses at 100% contrast - Generic Counter", "End Summary - Hits at 50% contrast - Count #1", "End Summary - Hits at 25% contrast - Count #1", "End Summary - Hits at 12.5% contrast - Count #1",
                                       "End Summary - Mistakes at 100% contrast - Count #1", "End Summary - Mistakes at 50% contrast - Count #1", "End Summary - Mistakes at 25% contrast - Count #1", "End Summary - Mistakes at 12.5% contrast - Count #1",
                                       "End Summary - Correct Rejections at 100% contrast - Count #1", "End Summary - Correct Rejections at 50% contrast - Count #1", "End Summary - Correct Rejections at 25% contrast - Count #1", "End Summary - Correct Rejections at 12.5% contrast - Count #1",
                                       "trial by trial anal - No of non correction trials", "End Summary - S+ Distractor touches", "End Summary - S- Distractor Touches",
                                       "End Summary - Hits during non-distractor trials - Count #1", "End Summary - Hits during congruent trials - Count #1", "End Summary - Hits during incongruent trials - Count #1",
                                       "End Summary - Misses during non-distractor trials - Generic Counter", "End Summary - Misses during congruent trials - Generic Counter",
                                       "End Summary - Misses during incongruent trials - Generic Counter", "End Summary - Mistakes during non-distractor trials - Count #1",
                                       "End Summary - Mistakes during congruent trials - Count #1", "End Summary - Mistakes during incongruent trials - Count #1",
                                       "End Summary - Correct Rejections during non-distractor trials - Count #1", "End Summary - Correct Rejections during congruent trials - Count #1",
                                       "End Summary - Correct Rejections during incongruent trials - Count #1", "trial by trial anal - Current image", "trial by trial anal - Distractor",
                                       "trial by trial anal - ITI", "trial by trial anal - Hits", "trial by trial anal - Misses",
                                       "trial by trial anal - Mistakes", "trial by trial anal - Correct Rejections",
                                       // "Correct Choice Latency non distractor trial", "Correct Choice Latency congruent trial", "Correct Choice Latency incongruent trial",
                                       // "Incorrect Choice Latency non distractor trial", "Incorrect Choice Latency congruent trial", "Incorrect Choice Latency incongruent trial",
                                       "End Summary - Centre ITI Touches", "End Summary - S+ Distractor touched during congruent trial -  Counter",
                                       "End Summary - S- Distractor touched during congruent trial -  Counter", "End Summary - S+ Distractor touched during incongruent trial -  Counter",
                                       "End Summary - S- Distractor touched during incongruent trial -  Counter", "trial by trial anal - Contrast", "End Summary - Stimulus Duration",
                                       "trial by trial anal - Correction Trial Correct Rejections", "trial by trial anal - Correction Trial Mistakes", "trial by trial anal - Distractor Time",
                                       "trial by trial anal - Correct Choice Latency", "trial by trial anal - Mistake Latency", "trial by trial anal - Correction Trial Mistake Latency", "trial by trial anal - Reward Retrieval Latency"};

                    lstFeatures.AddRange(input_CPT);
                    break;

                //***************VMCL
                case 45:
                case 46:
                case 47:

                    string[] input_VMCL = { "End Summary - Condition", "End Summary - Trials Completed", "End Summary - Correct Trials", "End Summary - Incorrect Trials",
                                            "End Summary - Missed Trials", "End Summary - No Correction Trials", "End Summary - % Correct", "End Summary - % Missed",
                                            "End Summary - Left Corrects - No Correct to Left", "End Summary - Right Correct - No. Corrects to Right",
                                            "Trial by Trial - Correct touch latency", "Trial by Trial - Incorrect Touch Latency", "Trial by Trial - Correct Reward Collection",
                                            "Correct touch latency", "Correct Reward Collection"};

                    lstFeatures.AddRange(input_VMCL);
                    break;

                //**************Autoshaoe
                case 48:
                case 49:
                    string[] input_autoshape = {"Trial Analysis - Reward Given - Condition", "Trial Analysis - Reward Collection Latency", "End Summary - Condition",
                             "End Summary - Trials completed", "End Summary - Touches to lit CS+", "End Summary - Touches to lit CS-", "End Summary - All CS+ touches",
                             "End Summary - All CS- touches", "End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+",
                             "End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+",
                             "End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-",
                             "End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-",
                             "End Summary - CS + Beam Breaking", "End Summary - CS - Beam Breaking", "End Summary - CS + Image Presentation Beam Breaking",
                             "End Summary - CS - Image Presentation Beam Breaking", "End Summary - Tray Beam Breaking", "End Summary - Tray CS + Beam Breaking",
                             "End Summary - Tray CS - Beam Breaking", "End Summary - CS + Image Approach CS- Beam Breaking",
                             "End Summary - CS - Image Approach CS+ Beam Breaking", "End Summary - Total Trials", "Trial Analysis - CS+ Approach Latency", "Trial Analysis - CS- Approach Latency",
                             "Trial Analysis - CS+ Touch Latency", "Trial Analysis - CS- Touch Latency"
                              };

                    lstFeatures.AddRange(input_autoshape);

                    break;

                //*****************Long Sequence
                case 52:
                    string[] input_sequence = { "End Summary - Condition", "End Summary - Corrects", "End Summary - Blank Touches", "Correct touch latency", "Correct Reward Collection",
                        "Correct latency grid 1", "Correct latency grid 2", "Correct latency grid 3", "Correct latency grid 4",  "Correct latency grid 5",
                        "Blank touches at grid specific - Grid 1", "Blank touches at grid specific - Grid 2", "Blank touches at grid specific - Grid 3", "Blank touches at grid specific - Grid 4",
                        "Blank touches at grid specific - Grid 5"};
                    lstFeatures.AddRange(input_sequence);
                    break;



            }

            return lstFeatures;
        }




    }
}
