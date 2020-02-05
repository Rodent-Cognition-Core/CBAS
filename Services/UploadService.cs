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
        public async Task<List<FileUploadResult>> UploadFiles(IFormFileCollection files, string TaskName, int expID, int subExpId, string ExpName, string Username, string userID, string SessionName)
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

                    // Extracting SubExpName , Age for the selected Subexperiment
                    string SubExpNameAge1 = GetSubExpNameAge(subExpId);
                    // Extracting Age of Animal for the selected Subexperiemnt
                    var AnimalAge = GetAnimalAge(subExpId);

                    // Check QC using the function defined in QualityControlService
                    bool IsUploaded1 = false;

                    (bool QC_IsQcPassed, bool QC_IsIdentifierPassed, string QC_FileUniqueID, string QC_ErrorMessage, string QC_WarningMessage, bool InsertToTblUpload, int SysAnimalID, int QC_UploadID, string QC_AnimalID) info =
                        _qualityControlService.QualityControl(TaskName, tempFileName, uploads, ExpName, expID, subExpId, AnimalAge, SessionName);

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
                            InsertFileData(tempFileName, pathString, uploadID, expID, userID, info.SysAnimalID, SessionName);

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

        public string GetExpName(int ExpID)
        {
            string sql = "Select ltrim(rtrim(ExpName)) As ExpName From Experiment where Experiment.ExpID =" + ExpID;
            return Dal.ExecScalar(sql).ToString();
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


        public bool SetUploadAsResolved(int uploadID, string userId)
        {
            FileUploadResult fur = GetUploadByUploadID(uploadID);

            var lstFur = GetListUploadsByAnimalIDErrorMessege(fur.AnimalID);

            foreach (var furInstance in lstFur)
            {
                InsertFileData(furInstance.SysFileName, furInstance.PermanentFilePath, furInstance.UploadID, furInstance.ExpID, userId, furInstance.AnimalID, furInstance.SessionName);

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
                InsertFileData(furInstance.SysFileName, furInstance.PermanentFilePath, furInstance.UploadID, furInstance.ExpID, userId, furInstance.AnimalID, furInstance.SessionName);

                string sql = $"UPDATE Upload " +
                     $"SET ErrorMessage = '', WarningMessage='', IsUploaded = 1, DateUpload = '{DateTime.UtcNow}', " +
                     $"IsQcPassed = 1, IsIdentifierPassed = 1 WHERE UploadID = {furInstance.UploadID} ;";

                Dal.ExecuteNonQuery(sql);

            }

            return true;
        }

        public FileUploadResult GetUploadByUploadID(int uploadID)
        {
            string sql = "select * from Upload where UploadID = " + uploadID;
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
            string sql = $"select * from Upload where AnimalID = {animalID} and ErrorMessage like 'Missing Animal Information:%' ";
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
            };
        }

        // ****************************************************************************************************************************************************************
        // ****************************************************************************************************************************************************************
        #region "Permanent File Data Operations"

        private void InsertFileData(string fileName, string filePath, int uploadID, int expID, string userID, int animalId, string SessionName)
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
            List<MarkerData> lstMD = ExtractMarkerData("MarkerData", "Marker", xdoc, sessionid);
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
        public List<MarkerData> ExtractMarkerData(string Tag1, string Tag2, XDocument xdoc1, int SessionInfoID)
        {
            string xpath = "/LiEvent/" + Tag1 + "/" + Tag2;
            var value = xdoc1.XPathSelectElements(xpath);

            List<MarkerData> lstMD = new List<MarkerData>();


            foreach (var val in value)
            {
                int source_type_id = -1;
                float? result = null;
                float? time = null;
                float? duration = null;
                int? count = null;

                string Attribute = ((System.Xml.Linq.XElement)val.FirstNode).Value.ToString(); // Feature Name
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
                        if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null )
                        {
                            count = Int32.Parse(((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode).Value.ToString());
                        }
                        else { count = null; }

                        break;

                    case "MEASURE":

                        source_type_id = 3;
                        if (((System.Xml.Linq.XElement)val.FirstNode.NextNode.NextNode) != null )
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

            using (DataTable dt = Dal.GetDataTable($@"SELECT Upload.UploadID, Upload.AnimalID, UserFileName, Animal.UserAnimalID, SessionName, ErrorMessage, WarningMessage, DateUpload, Upload.IsUploaded
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
                                TaskName= Convert.ToString(dr["TaskName"].ToString()),
                                SessionName = Convert.ToString(dr["SessionName"].ToString()),
                                SessionDescription = Convert.ToString(dr["SessionDescription"].ToString()),

                            });
                    }

                }

            return lstUploadSession;
        }

    }
}
