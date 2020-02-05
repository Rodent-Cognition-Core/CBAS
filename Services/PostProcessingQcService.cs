using AngularSPAWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AngularSPAWebAPI.Services
{

    public class PostProcessingQcService
    {

        public string CheckPostProcessingQC(SubExperiment subExp)
        {
            if (string.IsNullOrEmpty(subExp.ErrorMessage))
            {
                return RunPostProcessing(subExp);
            }
            else
            {
                // connect to DB, empty Erroressage field in SubExsp table, then call RunPostProcessin Function
                string sql = $@"UPDATE SubExperiment
                            SET ErrorMessage = ''
                            WHERE ExpID = {subExp.SubExpID} ;";

                Dal.ExecuteNonQuery(sql);
                // Run post processing
                return RunPostProcessing(subExp);
            }

        }

        public string RunPostProcessing(SubExperiment subExp)
        {
            string errorMessage = "";
            string errorGeneral = "";

            // Extracting the TaskName of the selected experiment 
            int TaskID = GetTaskName(subExp.ExpID);
            // Extracting minAge of each experiment
            int minAge = getminAge(subExp.ExpID);

            // post processing QC for Standard Schedules which is applied only on the SubExp with minimum age
            if (subExp.AgeID == minAge && !subExp.IsIntervention)
            {
                // check the post QC for schedule names common in all tasks e.g. habituation 1
                errorGeneral = ScheduleCount(subExp.SubExpID);
            }

            string error = "";
            // Switch Case .... based on taskName of the experiment
            switch (TaskID)
            {

                case 2:

                    // Post Processing QC for cam 5 choice experiment
                    error = ScheduleCount_5C(subExp.SubExpID);
                    break;

                case 3:

                    //Post Processing QC for PD experiment
                    error = ScheduleCount_PD(subExp.SubExpID);
                    break;

                case 4:

                    // Post Processing QC for PAL experiment
                    error = ScheduleCount_PAL(subExp.SubExpID);
                    break;

                case 5:

                    error = ScheduleCount_LD(subExp.SubExpID);
                    break;

                case 9:
                    // post Processing QC for PR Task
                    error = ScheduleCount_PR(subExp.SubExpID);
                    break;
                case 10:
                    // post Processing QC for PRL Task
                    error = ScheduleCount_PRL(subExp.SubExpID);
                    break; 


            }
            // Post Processing QC for 5 choice experiment
            //string error = ScheduleCount_5C(expID);
            // error += other tests...

            if (error == string.Empty && errorGeneral == string.Empty)
            {
                SetPostProcessingPassed(subExp.SubExpID);
            }
            else
            {
                // add header
                errorMessage = $@"<table><tr style=""text-align: center; color:#566573;"">
                        <th style=""text-align:center; padding: 15px 0px; text-transform: uppercase; color:#34495E;"" class=""docs-markdown-th"">Animal Id</th>
                        <th style=""text-align:center; padding: 15px 0px; text-transform: uppercase; color:#34495E;"" class=""docs-markdown-th"">Error Message</th>
                        <th style=""text-align:center; padding: 15px 0px; text-transform: uppercase; color:#34495E;"" class=""docs-markdown-th"">File Names</th>
                        <th style=""text-align:center; padding: 15px 0px; text-transform: uppercase; color:#34495E;"" class=""docs-markdown-th"">Dates</th>
                        </tr>
                        {errorGeneral}
                        {error}
                        </table>";

                SetPostProcessingError(subExp.SubExpID, errorMessage);

            }

            // after all tests done, update IsPostProcessingPass and errorMessage
            return errorMessage;
        }

        private void SetPostProcessingError(int SubxpID, string error)
        {
            string sql = $@"UPDATE SubExperiment
                            SET IsPostProcessingPass = 0, ErrorMessage = '{error}'
                            WHERE SubExpID = {SubxpID} ;";

            Dal.ExecuteNonQuery(sql);

        }

        private void SetPostProcessingPassed(int SubxpID)
        {
            string sql = $@"UPDATE SubExperiment
                            SET IsPostProcessingPass = 1, ErrorMessage = ''
                            WHERE SubExpID = {SubxpID} ;";

            Dal.ExecuteNonQuery(sql);

        }

        public string GetPostProcessingResult(int subExpID)
        {
            string sql = $@"SELECT ISNULL(ErrorMessage, '') AS ErrorMessage FROM SubExperiment
                            
                            WHERE SubExpID = {subExpID} ;";

            return Dal.ExecScalar(sql).ToString();

        }

        #region "5C"

        // Function Definition: Post Processing QC for the standard schedules

        public string ScheduleCount(int expID)
        {
            string error1 = "";
            error1 = GetScheduleCount_Error(expID, "SessionInfo.SessionName= 'Habituation_1'", "<b>Habituation_1</b>", 1, "!=", "");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName= 'Habituation_2'", "<b>Habituation_2</b>", 3, "!=", "");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName= 'Initial_Touch'", "<b>Initial_Touch</b>", 1, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName= 'Must_Touch'", "<b>Must_Touch</b>", 1, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName= 'Must_Initiate'", "<b>Must_Initiate</b>", 1, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName= 'Punish_Incorrect'", "<b>Punish_Incorrect</b>", 1, "<", "at least");

            return error1;

        }

        // Function Definition: Post Processing QC for 5 choice experiment
        public string ScheduleCount_5C(int expID)
        {
            string error1 = "";
            error1 = GetScheduleCount_Error(expID, "stimulus_duration = '4000'", "<b>4s</b> or <b>4000ms</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '8000'", "<b>8s</b> or <b>8000ms</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '1500'", "<b>1.5s</b> or <b>1500ms</b>", 2, "!=", "");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '1000'", "<b>1s</b> or <b>1000ms</b>", 2, "!=", "");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '600'", "<b>0.6s</b> or <b>600ms</b>", 2, "!=", "");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '800'", "<b>0.8s</b> or <b>800ms</b>", 2, "!=", "");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '400'", "<b>0.4s</b> or <b>400ms</b>", 2, "!=", "");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '200'", "<b>0.2s</b> or <b>200ms</b>", 2, "!=", "");

            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '2000' and SessionInfo.SessionName='Training'", "<b>2000ms Training</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "stimulus_duration = '2000' and SessionInfo.SessionName='Intra_Probe'", "<b>2000ms INTRAPROBE</b>", 6, "<", "at least");

            return error1;

        }


        // Function Defintion: Post Processing QC for PAL experiment
        public string ScheduleCount_PAL(int expID)
        {
            string error1 = "";
            error1 = GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Acquisition'", "<b>Acquisition</b>", 3, ">", "at most");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Mouse_dpal_spal'", "<b>Mouse dPAL</b> or <b>Mouse sPAL</b>", 45, "<", "at least");
            //error1 += GetScheduleCount_Error(expID, "Schedule_Name like '%retention%'", "<b>Mouse dPAL RETENTION</b> or <b>Mouse sPAL RETENTION</b>", 5, "<", "at least");
            return error1;

        }

        // Function Definition: Post Processing QC for PAL experiment
        public string ScheduleCount_PD(int expID)
        {
            string error1 = "";

            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Acquisition'", "<b>Acquisition</b>", 2, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Baseline'", "<b>Baseline</b>", 2, "!=", "");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Reversal'", "<b>Reversal</b>", 10, "!=", "");
            //error1 += GetScheduleCount_Error(expID, "Schedule_Name like '%RetentionReversal%'", "<b>Retention Reversal</b>", 2, "!=", "");
            return error1;
        }

        // Function Definition: Post Processing QC for LD experiment
        public string ScheduleCount_LD(int expID)
        {
            string error1 = "";
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'LD_1_Choice_Training'", "<b>LD_1_Choice_Training</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'LD_1_Choice_Probe_Easy'", "<b>LD_1_Choice_Probe_Easy</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'LD_1_Choice_Probe_Hard'", "<b>LD_1_Choice_Probe_Hard</b>", 3, "<", "at least");

            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'LD_1_Choice_Reversal_Training'", "<b>LD_1_Choice_Reversal_Training</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'LD_1_Choice_Reversal_Probe_Easy'", "<b>LD_1_Choice_Reversal_Probe_Easy</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'LD_1_Choice_Reversal_Probe_Hard'", "<b>LD_1_Choice_Reversal_Probe_Hard</b>", 3, "<", "at least");
            return error1;
        }

        // Function Definition: Post Processing QC for PR experiment
        public string ScheduleCount_PR(int expID)
        {
            string error1 = "";
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Multiple Responding Training FR-1'", "<b>Multiple Responding Training FR-1</b>", 1, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Multiple Responding Training FR-2'", "<b>Multiple Responding Training FR-2</b>", 1, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Multiple Responding Training FR-3'", "<b>Multiple Responding Training FR-3</b>", 1, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Multiple Responding Training FR-5' OR SessionInfo.SessionName ='Baseline FR-5'", "<b>Multiple Responding Training FR-5 or Baseline FR-5</b>", 4, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Basic PR (PR4)'", "<b>Basic PR (PR4)</b>", 6, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'High Demand PR (PR8)'", "<b>High Demand PR (PR8)</b>", 2, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'High Demand PR (PR12)'", "<b>High Demand PR (PR12)</b>", 2, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Uncapped FR-5'", "<b>Uncapped FR-5</b>", 3, "<", "at least");

            return error1;
        }

        // Function Definition: Post Processing QC for PR experiment
        public string ScheduleCount_PRL(int expID)
        {
            string error1 = "";
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Deterministic Reversal Training'", "<b>Deterministic Reversal Training</b>", 3, "<", "at least");
            error1 += GetScheduleCount_Error(expID, "SessionInfo.SessionName = 'Probabilistic Feedback'", "<b>Probabilistic Feedback</b>", 3, "<", "at least");
            return error1;
        }

        public string GetScheduleCount_Error(int expID, string sessionNameFilter, string SessionName, int minCount, string operand, string str)
        {

            string retVal = string.Empty;

            string sql = GetScheduleNameQuery_MinCount(sessionNameFilter, expID, minCount, operand);
            string sql2 = GetScheduleNameQuery(sessionNameFilter, expID);
            int cntSchedule = Int32.Parse(Dal.ExecScalar(sql2).ToString());
            if (cntSchedule == 0)
            {

                retVal += $@"<tr style=""text-align: center; color:#566573;"">
                                   <td style=""text-align: center; color:#909497;"" class=""docs-markdown-td"">No file with Schedule {SessionName} has been uploaded to the database</td>";

                retVal += "</tr>";
            }
            else
            {
                using (DataTable dt = Dal.GetDataTable(sql))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sqlInner = GetScheduleNameDetailQuery_MinCount(expID, Int16.Parse(dr["animalId"].ToString()), sessionNameFilter);
                        using (DataTable dtInner = Dal.GetDataTable(sqlInner))
                        {
                            string strFileNames = string.Empty;
                            string strUserAnimalId = string.Empty;

                            IList<string> lstStrFileNames = new List<string>();
                            IList<string> lstStrDates = new List<string>();

                            foreach (DataRow drInner in dtInner.Rows)
                            {
                                lstStrFileNames.Add(drInner["UserFileName"].ToString());
                                lstStrDates.Add(DateTime.Parse(drInner["DateField"].ToString()).ToString("dd/MM/yyyy"));
                                strUserAnimalId = drInner["UserAnimalId"].ToString();
                            }

                            string fileNameJoined = string.Join(", ", lstStrFileNames);
                            string dateJoined = string.Join(", ", lstStrDates);



                            retVal += $@"<tr style=""text-align: center; color:#566573;"">
                                        <td style=""text-align: center; color:#909497;"" class=""docs-markdown-td"">{strUserAnimalId}</td>
                                        <td style=""text-align: center; color:#909497;"" class=""docs-markdown-td"">Each animal should have {str} {minCount} {SessionName} in this experiment!</td>
                                        <td style=""text-align: center; color:#909497;"" class=""docs-markdown-td"">{fileNameJoined}</td>
                                        <td style=""text-align: center; color:#909497;"" class=""docs-markdown-td"">{dateJoined}</td>";
                            retVal += "</tr>";
                        }

                    }
                }

            }


            return retVal;

        }

        private string GetScheduleNameQuery_MinCount(string sessionNameFilter, int subExpId, int minCount, string operand)
        {

            return $@"select count(*) cnt, SessionInfo.AnimalID
                    from SessionInfo
                    inner join upload on SessionInfo.UploadID = Upload.UploadID
                    where upload.subExpID = {subExpId} and ({sessionNameFilter})
                    group by SessionInfo.AnimalID
                    having  count(*) {operand} {minCount}";

        }

        private string GetScheduleNameQuery(string sessionNameFilter, int subExpId)
        {
            return $@"select count(*) cnt
                    from SessionInfo
                    inner join upload on SessionInfo.UploadID = Upload.UploadID
                    where upload.subExpID = {subExpId} and ({sessionNameFilter})";

        }

        private string GetScheduleNameDetailQuery_MinCount(int subExpId, int animalId, string sessionNameFilter)
        {

            return $@"select Schedule_Name, Animal.UserAnimalID , CAST(Date_Time AS DATE) as DateField, upload.UserFileName 
                        from SessionInfo 
                        inner join Upload on SessionInfo.UploadID = Upload.UploadID
                        inner join Animal on Animal.AnimalID = SessionInfo.AnimalID
                        where ({sessionNameFilter}) and  upload.subExpID = {subExpId} and SessionInfo.AnimalID = {animalId}

            ";
        }



        #endregion

        // extrcting taskname of the experiment
        public int GetTaskName(int expID)
        {
            string sql = $@"Select task.id as TaskID from Experiment inner join task on task.id = Experiment.taskID WHERE experiment.ExpID = {expID};";
            int taskID = Int32.Parse(Dal.ExecScalar(sql).ToString());
            return taskID;

        }

        public int getminAge(int expID)
        {
            string sql = $@"Select Min(AgeID) as MinAge From SubExperiment Where ExpID ={expID}";
            int minAge = Int32.Parse(Dal.ExecScalar(sql).ToString());
            return minAge;
        }

    }


}
