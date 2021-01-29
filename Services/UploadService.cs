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
                    bool MultipleSessions = GetMultipleSessions(expID);

                    // Extracting SubExpName , Age for the selected Subexperiment
                    string SubExpNameAge1 = GetSubExpNameAge(subExpId);
                    // Extracting Age of Animal for the selected Subexperiemnt
                    var AnimalAge = GetAnimalAge(subExpId);

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
                            uploadID = InsertUpload(fur);

                        }

                        // If there is a duplicate, but multiple sessions for an animal in a day are allowed
                        else if (MultipleSessions)
                        {
                            uploadID = InsertUpload(fur);
                            UpdateDuplicateSessions(info.QC_FileUniqueID);
                        }

                        else
                        {
                            //Update Upload table and return uploadID
                            UpdateUpload(fur, info.QC_UploadID);
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
                            InsertFileData(tempFileName, pathString, uploadID, expID, userID, info.SysAnimalID, SessionName, TaskID, sessionID);

                        }


                    }



                }




            }

            return uploadResult;

        }

        public string GetTaskName(int ExpID)
        {
            string sql = "Select ltrim(rtrim(Task.Name)) as TaskName From Task inner join Experiment on task.ID = Experiment.taskID where Experiment.ExpID =" + ExpID;
            return Dal.ExecScalar(sql).ToString();
        }

        public int GetTaskID(int ExpID)
        {
            string sql = "Select Task.ID as TaskID From Task inner join Experiment on task.ID = Experiment.taskID where Experiment.ExpID =" + ExpID;
            return Convert.ToInt32(Dal.ExecScalar(sql).ToString());
        }

        public string GetExpName(int ExpID)
        {
            string sql = "Select ltrim(rtrim(ExpName)) As ExpName From Experiment where Experiment.ExpID =" + ExpID;
            return Dal.ExecScalar(sql).ToString();
        }

        // function to determine if multiple sessions of an animal in a single day are allowed
        public bool GetMultipleSessions(int ExpID)
        {
            string sql = "Select MultipleSessions From Experiment where Experiment.ExpID =" + ExpID;
            return Convert.ToBoolean(Dal.ExecScalar(sql).ToString());
        }

        // Function to Insert To Upload Table in Database
        public int InsertUpload(FileUploadResult upload)
        {
            if (upload.FileUniqueID != null) { upload.FileUniqueID = upload.FileUniqueID.Trim(); }

            string sql = $"insert into Upload " +
              $"(ExpID, AnimalID, SubExpID, UserFileName, SysFileName, SessionName, ErrorMessage, WarningMessage, IsUploaded, DateFileCreated, DateUpload, FileSize, FileUniqueID, IsQcPassed, IsIdentifierPassed, PermanentFilePath ) Values " +
              $"({upload.ExpID}, {upload.AnimalID}, {upload.SubExpID}, '{upload.UserFileName}', '{upload.SysFileName}', '{upload.SessionName}', '{upload.ErrorMessage}', '{upload.WarningMessage}', '{upload.IsUploaded}', '{upload.DateFileCreated}', " +
              $"'{upload.DateUpload}', '{upload.FileSize}', '{upload.FileUniqueID}', '{upload.IsQcPassed}',  '{upload.IsIdentifierPassed}', '{upload.PermanentFilePath}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalar(sql).ToString());
        }

        //Function to update Upload Table
        public void UpdateUpload(FileUploadResult upload, int UploadId)
        {
            if (upload.FileUniqueID != null) { upload.FileUniqueID = upload.FileUniqueID.Trim(); }

            string sql = $"UPDATE Upload " +
                 $"SET ExpID = {upload.ExpID} , AnimalID = {upload.AnimalID}, SubExpID = {upload.SubExpID} , UserFileName = '{upload.UserFileName}', SysFileName = '{upload.SysFileName}', SessionName ='{upload.SessionName}', ErrorMessage = '{upload.ErrorMessage}'," +
                 $"WarningMessage= '{upload.WarningMessage}', IsUploaded = '{upload.IsUploaded}', DateFileCreated = '{upload.DateFileCreated}', DateUpload = '{upload.DateUpload}', FileSize= '{upload.FileSize}', " +
                 $" IsQcPassed = '{upload.IsQcPassed}', IsIdentifierPassed = '{upload.IsIdentifierPassed}' WHERE FileUniqueID = '{upload.FileUniqueID}' and UploadID = {UploadId};";


            Dal.ExecuteNonQuery(sql);
        }

        // Function to flag duplicate fileUniqueIDs in the Upload Table
        public void UpdateDuplicateSessions(string FileUniqueID)
        {
            string sql = $"UPDATE Upload " +
                $"SET IsDuplicateSession = 1 WHERE FileUniqueID = '{FileUniqueID}'";

            Dal.ExecuteNonQuery(sql);
        }

        public bool SetUploadAsResolved(int uploadID, string userId)
        {
            FileUploadResult fur = GetUploadByUploadID(uploadID);

            var lstFur = GetListUploadsByAnimalIDErrorMessege(fur.AnimalID);

            foreach (var furInstance in lstFur)
            {
                // Get Upload session ID from "Upload_SessionInfo" Table using its SessionName
                int UploadSessionID = getUploadSessionIDbySessionName(furInstance.SessionName);

                InsertFileData(furInstance.SysFileName, furInstance.PermanentFilePath, furInstance.UploadID, furInstance.ExpID, userId, furInstance.AnimalID,
                               furInstance.SessionName, furInstance.TaskID, UploadSessionID);

                string sql = $"UPDATE Upload " +
                     $"SET ErrorMessage = '', WarningMessage='', IsUploaded = 1, DateUpload = '{DateTime.UtcNow}', " +
                     $"IsQcPassed = 1, IsIdentifierPassed = 1 WHERE UploadID = {furInstance.UploadID} ;";

                Dal.ExecuteNonQuery(sql);

            }

            return true;
        }


        public bool SetAsResolvedForEditedAnimalId(int animalId, string userId)
        {
            var lstFur = GetListUploadsByAnimalIDErrorMessege(animalId);

            foreach (var furInstance in lstFur)
            {
                int UploadSessionID = getUploadSessionIDbySessionName(furInstance.SessionName);
                InsertFileData(furInstance.SysFileName, furInstance.PermanentFilePath, furInstance.UploadID, furInstance.ExpID, userId, furInstance.AnimalID,
                               furInstance.SessionName, furInstance.TaskID, UploadSessionID);

                string sql = $"UPDATE Upload " +
                     $"SET ErrorMessage = '', WarningMessage='', IsUploaded = 1, DateUpload = '{DateTime.UtcNow}', " +
                     $"IsQcPassed = 1, IsIdentifierPassed = 1 WHERE UploadID = {furInstance.UploadID} ;";

                Dal.ExecuteNonQuery(sql);

            }

            return true;
        }

        private int getUploadSessionIDbySessionName(string sessionName)
        {
            string sql = $"Select id from Upload_SessionInfo Where SessionName='{sessionName}'";

            return Int32.Parse(Dal.ExecScalar(sql).ToString());
        }

        public FileUploadResult GetUploadByUploadID(int uploadID)
        {
            string sql = $@"select Upload.*, Experiment.TaskID from Upload
                            Inner join Experiment on Experiment.ExpID = Upload.ExpID
                            where UploadID ={uploadID} ";
            FileUploadResult retVal;

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                if (dt.Rows.Count != 1)
                {
                    throw new Exception("UploadID Not Found");
                }

                retVal = ParseUploadRow(dt.Rows[0]);

            }

            return retVal;

        }

        public List<FileUploadResult> GetListUploadsByAnimalIDErrorMessege(int animalID)
        {
            string sql = $@"select Upload.*, Experiment.TaskID from Upload
                          Inner join Experiment on Experiment.ExpID = Upload.ExpID
                          where AnimalID = {animalID} and ErrorMessage like 'Missing Animal Information:%' ";
            var retVal = new List<FileUploadResult>();

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    FileUploadResult uploadResult = ParseUploadRow(dr);
                    retVal.Add(uploadResult);
                }

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

            // Extract Marker Data
            List<MarkerData> lstMD = ExtractMarkerData("MarkerData", "Marker", xdoc, sessionid, TaskID, SessionName, sessionIDUpload);
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
            Dictionary<string, float?> cptDictDistractorFeatures = new Dictionary<string, float?>();  // for sessionIDUpload==42 (cpt distractor)

            foreach (var val in value)
            {

                int source_type_id = -1;
                float? result = null;
                float? time = null;
                float? duration = null;
                int? count = null;
                string Attribute = ((System.Xml.Linq.XElement)val.FirstNode).Value.ToString(); // Feature Name
                //  If condition to consider only features exist in lstFeatures (only important features of each task should be considered!)
                if (lstFeatures.Contains(Attribute) || lstFeatures == null)
                {


                    if (TaskID == 11) // Task is iCPT
                    {

                        if ((Attribute.Equals("End Summary - Stimulus Duration")) && (sessionIDUpload == 38 || sessionIDUpload == 39))
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

                            case "Correct Choice Latency":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }
                                lstCorrectLatency.Add((float)result);

                                break;

                            case "Reward Retrieval Latency":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }
                                lstRewatdLatency.Add((float)result);
                                break;

                            case "Incorrect Choice Latency":

                                if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode) != null)
                                {
                                    result = float.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode.NextNode).Value.ToString());
                                }
                                else { result = null; }
                                lstIncorLatency.Add((float)result);
                                break;

                        }  // End of switch case


                    }  // end of if(tskID==11) for iCPT task

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
            if (sessionIDUpload == 40 || sessionIDUpload == 38 || sessionIDUpload == 39)  // cpt with different SDs
            {
                cptFeatureDict = GetDictCPTFeatures(lstSD, lstHits, lstMiss, lstMistake, lstcCorrectRejection, lstCorrectLatency, lstRewatdLatency, lstIncorLatency, sessionIDUpload);
            }
            else if (sessionIDUpload == 41) // cpt with different contrast levels
            {
                cptFeatureDict = GetDictCPTFeatures_contrastLevel(lstSD, lstHits, lstMiss, lstMistake, lstcCorrectRejection, lstCorrectLatency, lstRewatdLatency, lstIncorLatency);
            }
            else if (sessionIDUpload == 42)  // cpt with distractor
            {
                cptFeatureDict = GetDictCPTFeatures_distractor(cptDictDistractorFeatures);
            }

            int sourceType = 1;
            float? resultVal = null;
            float? durationVal = null;
            foreach (KeyValuePair<string, float?> entry in cptFeatureDict)
            {
                if (entry.Key.Contains("Correct Choice Latency") || entry.Key.Contains("Reward Retrieval Latency") || entry.Key.Contains("Incorrect Choice Latency"))
                {
                    sourceType = 3;
                    durationVal = entry.Value;

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
                        IsDuplicateSession = bool.Parse(dr["IsDuplicateSession"].ToString()),
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
        public string GetSubExpNameAge(int subExpID)
        {
            string sql = $@"Select CONCAT(SubExpName, ', ' , Age.AgeInMonth, ' Months') as SubExpNameAge From SubExperiment  Inner Join Age On Age.ID = SubExperiment.AgeID  Where SubExpID = {subExpID}";
            string getValue = Dal.ExecScalar(sql).ToString();

            return getValue.ToString();

        }

        // Function Definition to Extract Age of Animal From the SubExperiment Table
        public string GetAnimalAge(int subExpID)
        {
            string sql = $@"Select AgeInMonth From SubExperiment Inner Join Age On Age.ID = SubExperiment.AgeID  Where SubExpID = {subExpID}";
            var age = Convert.ToString(Dal.ExecScalar(sql));
            return age;
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
                       List<int?> lstcCorrectRejection, List<float?> lstCorrectLatency, List<float?> lstRewatdLatency, List<float?> lstIncorLatency, int uploadsessionID)
        {

            Dictionary<string, float?> cptFeatureDict = new Dictionary<string, float?>();

            var maxIndex = new List<int>() { lstSD.Count, lstHits.Count, lstMiss.Count, lstMistake.Count, lstcCorrectRejection.Count };
            var maxIndexValue = maxIndex.Max();
            var distSDValues = lstSD.Distinct();

            // if the file belongs to stage 3 and 4 for cpt task
            if (uploadsessionID == 38 || uploadsessionID == 39)
            {
                float avgCorrectLatency = lstCorrectLatency.Average(x => Convert.ToSingle(x));
                float avgRewardLatency = lstRewatdLatency.Average(x => Convert.ToSingle(x));
                var titleCorrectLatency = $"Average - Correct Choice Latency at {distSDValues.ToList()[0].ToString()}s SD";
                var titleRewardLatency = $"Average - Reward Retrieval Latency at {distSDValues.ToList()[0].ToString()}s SD";

                float avgIncorrectLatency = lstIncorLatency.Average(x => Convert.ToSingle(x));
                var titleInCorrectLatency = $"Average - Incorrect Choice Latency at {distSDValues.ToList()[0].ToString()}s SD";

                var sumHit = lstHits.Sum(x => Convert.ToInt32(x));
                var sumMiss = lstMiss.Sum(x => Convert.ToInt32(x));
                var sumMistake = lstMistake.Sum(x => Convert.ToInt32(x));
                var sumCorrectRejection = lstcCorrectRejection.Sum(x => Convert.ToInt32(x));
                // Calculated features
                float hitRate = (float)(sumHit) / (float)(sumHit + sumMiss);
                float falseAlarmRate = (float)sumMistake / (float)(sumMistake + sumCorrectRejection);

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
                cptFeatureDict.Add(titleCorrectLatency, avgCorrectLatency / 1000000);
                cptFeatureDict.Add(titleRewardLatency, avgRewardLatency / 1000000);
                cptFeatureDict.Add(titleInCorrectLatency, avgIncorrectLatency / 1000000);

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
                int cnt = 0;
                for (int j = 0; j < maxIndexValue; j++)
                {

                    if (lstHits[j] > 0)
                    {
                        DataRow newRow = dt_latency.NewRow();
                        newRow["SD"] = lstSD.Count < j ? null : lstSD[j];
                        newRow["Hit"] = lstHits.Count < j ? null : lstHits[j];
                        newRow["CorrectLatency"] = lstCorrectLatency.Count <= cnt ? (object)DBNull.Value : lstCorrectLatency[cnt];
                        newRow["RewardLatency"] = lstRewatdLatency.Count <= cnt ? (object)DBNull.Value : lstRewatdLatency[cnt];  

                        dt_latency.Rows.Add(newRow);
                        cnt = cnt + 1;
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

                        cptFeatureDict.Add(titleCorrectLatency, avgCorrectLatency / 1000000);
                        cptFeatureDict.Add(titleRewardLatency, avgRewardLatency / 1000000);
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
                    int cnt2 = 0;
                    for (int j = 0; j < maxIndexValue; j++)
                    {

                        if (lstMistake[j] > 0)
                        {
                            DataRow newRow = dt_incorrect_latency.NewRow();
                            newRow["SD"] = lstSD.Count < j ? null : lstSD[j];
                            newRow["Mistake"] = lstMistake.Count < j ? null : lstMistake[j];
                            newRow["IncorrectLatency"] = lstIncorLatency.Count <= cnt2 ? (object)DBNull.Value : lstIncorLatency[cnt2]; 

                            dt_incorrect_latency.Rows.Add(newRow);
                            cnt2 = cnt2 + 1;
                        }

                    }

                    foreach (var sd in distSDValues)
                    {
                        if (dt_incorrect_latency.Select("SD = " + sd).Count() > 0)
                        {
                            float avgIncorrectLatency = Convert.ToSingle(dt_incorrect_latency.Compute("AVG(IncorrectLatency)", "SD = " + sd));

                            var titleInCorrectLatency = $"Average - Incorrect Choice Latency at {sd.ToString()}s SD";

                            cptFeatureDict.Add(titleInCorrectLatency, avgIncorrectLatency / 1000000);
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
                    newRow["SD"] = lstSD.Count < j ? null : lstSD[j];
                    newRow["Hit"] = lstHits.Count < j ? null : lstHits[j];
                    newRow["Miss"] = lstMiss.Count < j ? null : lstMiss[j];
                    newRow["Mistake"] = lstMistake.Count < j ? null : lstMistake[j];
                    newRow["CorrectRejection"] = lstcCorrectRejection.Count < j ? null : lstcCorrectRejection[j];
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
                        float hitRate = (float)(sumHit) / (float)(sumHit + sumMiss);
                        float falseAlarmRate = (float)sumMistake / (float)(sumMistake + sumCorrectRejection);

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
            int cnt = 0;
            for (int j = 0; j < maxIndexValue; j++)
            {

                if (lstHits[j] > 0)
                {
                    DataRow newRow = dt_latency.NewRow();
                    newRow["SD"] = lstSD.Count < j ? null : lstSD[j];
                    newRow["Hit"] = lstHits.Count < j ? null : lstHits[j];
                    newRow["CorrectLatency"] = lstCorrectLatency.Count <= cnt ? (object)DBNull.Value : lstCorrectLatency[cnt];
                    newRow["RewardLatency"] = lstRewatdLatency.Count <= cnt ? (object)DBNull.Value : lstRewatdLatency[cnt];

                    dt_latency.Rows.Add(newRow);
                    cnt = cnt + 1;
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

                    cptFeatureDict.Add(titleCorrectLatency, avgCorrectLatency / 1000000);
                    cptFeatureDict.Add(titleRewardLatency, avgRewardLatency / 1000000);
                }

            }

            //************** Create a new data table based on dt datatable where  mistake is greater than 0 for incorrect Touch latency
            DataTable dt_incorrect_latency = new DataTable();
            dt_incorrect_latency.Columns.Add("SD", typeof(float));
            dt_incorrect_latency.Columns.Add("Mistake", typeof(int));
            dt_incorrect_latency.Columns.Add("IncorrectLatency", typeof(int));

            // filling data table dt_latency
            int cnt2 = 0;
            for (int j = 0; j < maxIndexValue; j++)
            {

                if (lstMistake[j] > 0)
                {
                    DataRow newRow = dt_incorrect_latency.NewRow();
                    newRow["SD"] = lstSD.Count < j ? null : lstSD[j];
                    newRow["Mistake"] = lstMistake.Count < j ? null : lstMistake[j];
                    newRow["IncorrectLatency"] = lstIncorLatency.Count <= cnt2 ? (object)DBNull.Value : lstIncorLatency[cnt2];

                    dt_incorrect_latency.Rows.Add(newRow);
                    cnt2 = cnt2 + 1;
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
                newRow["SD"] = lstSD.Count < j ? null : lstSD[j];
                newRow["Hit"] = lstHits.Count < j ? null : lstHits[j];
                newRow["Miss"] = lstMiss.Count < j ? null : lstMiss[j];
                newRow["Mistake"] = lstMistake.Count < j ? null : lstMistake[j];
                newRow["CorrectRejection"] = lstcCorrectRejection.Count < j ? null : lstcCorrectRejection[j];
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
                    float hitRate = (float)(sumHit) / (float)(sumHit + sumMiss);
                    float falseAlarmRate = (float)sumMistake / (float)(sumMistake + sumCorrectRejection);

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
            float? nonDistHitRate = cptDictDistractorFeatures["End Summary - Hits during non-distractor trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during non-distractor trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during non-distractor trials - Generic Counter"]);
            float? nonDisFalseALarmRate = cptDictDistractorFeatures["End Summary - Mistakes during non-distractor trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Mistakes during non-distractor trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during non-distractor trials - Count #1"]);

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
            float? CongDistHitRate = cptDictDistractorFeatures["End Summary - Hits during congruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during congruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during congruent trials - Generic Counter"]);
            float? CongFalseALarmRate = cptDictDistractorFeatures["End Summary - Mistakes during congruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Mistakes during congruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during congruent trials - Count #1"]);
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
            float? inCongDistHitRate = cptDictDistractorFeatures["End Summary - Hits during incongruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Hits during incongruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Misses during incongruent trials - Generic Counter"]);
            float? inCongFalseALarmRate = cptDictDistractorFeatures["End Summary - Mistakes during incongruent trials - Count #1"] / (cptDictDistractorFeatures["End Summary - Mistakes during incongruent trials - Count #1"] + cptDictDistractorFeatures["End Summary - Correct Rejections during incongruent trials - Count #1"]);
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
                                                "End Summary - % Correct"};
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
                                       "END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)"};
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
                case 44:
                    string[] input_CPT = { "End Summary - Schedule length", "End Summary - No of non correction trials", "End Summary - Hits",
                                       "End Summary - Misses", "End Summary - Mistakes", "End Summary - Correct Rejections", "End Summary - Correct Image",
                                       "Correct Choice Latency", "Incorrect Choice Latency", "Reward Retrieval Latency", "trial by trial anal - Current image",
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
                                       "Correct Choice Latency non distractor trial", "Correct Choice Latency congruent trial", "Correct Choice Latency incongruent trial",
                                       "Incorrect Choice Latency non distractor trial", "Incorrect Choice Latency congruent trial", "Incorrect Choice Latency incongruent trial",
                                       "End Summary - Centre ITI Touches", "End Summary - S+ Distractor touched during congruent trial -  Counter",
                                       "End Summary - S- Distractor touched during congruent trial -  Counter", "End Summary - S+ Distractor touched during incongruent trial -  Counter",
                                       "End Summary - S- Distractor touched during incongruent trial -  Counter", "trial by trial anal - Contrast", "End Summary - Stimulus Duration"};

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


            }

            return lstFeatures;
        }




    }
}
