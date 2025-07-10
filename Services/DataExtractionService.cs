using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{

    public class DataExtractionService
    {
        // Extracting list of all Task from Database
        public List<TaskAnalysis> GetAllTaskAnalysis()
        {
            List<TaskAnalysis> lstTaskAnalyses = new List<TaskAnalysis>();

            using (DataTable dt = Dal.GetDataTable("select * from tsd.Task Where ID not in (1,6)"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstTaskAnalyses.Add(new TaskAnalysis
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Name = Convert.ToString(dr["Name"].ToString()),
                        OriginalName = Convert.ToString(dr["OriginalName"].ToString()),
                        TaskDescription = Convert.ToString(dr["TaskDescription"].ToString()),
                    });
                }

            }

            return lstTaskAnalyses;

        }

        public List<Experiment> GetAllExperimentByTaskId(int TaskId, string userID, string isFullDataAccess, int speciesId)
        {
            List<Experiment> lstExp = new List<Experiment>();

            var userIdCondition = (string.IsNullOrEmpty(userID)) ? "" : $"OR Experiment.UserID ='{userID}'";
            var isFullAccessCondition = (isFullDataAccess.ToUpper() == "TRUE") ? "" : $"and (Experiment.Status = 1 {userIdCondition} )";

            using (DataTable dt = Dal.GetDataTable($@"select Experiment.*, task.name as TaskName, tt2.PISiteName, tt2.UserName, CONCAT(tt2.PISiteName, ' - ', tt2.UserName) as PISiteUser from tsd.Experiment 

                                                        inner join tsd.task on task.ID = Experiment.TaskID
                                                        inner join
                                                        
                                                        (Select PUSID, PIUserSite.PSID, tt.PISiteName, CONCAT(AspNetUsers.GivenName, '-', AspNetUsers.FamilyName) as UserName  From tsd.PIUserSite
                                                        inner join
                                                        (Select PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) as PISiteName From tsd.PISite
                                                        inner join tsd.PI on PI.PID = PISite.PID
                                                        inner join tsd.Site on Site.SiteID = PISite.SiteID
														
														) as tt on tt.PSID = PIUserSite.PSID
														inner join dbo.AspNetUsers on AspNetUsers.Id = PIUserSite.UserID) 
														 as tt2 on tt2.PUSID = Experiment.PUSID
                                                        WHERE TaskID ={TaskId} and SpeciesID = {speciesId} {isFullAccessCondition}"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstExp.Add(new Experiment
                    {
                        ExpID = Int32.Parse(dr["ExpID"].ToString()),
                        UserID = Convert.ToString(dr["UserID"].ToString()),
                        PUSID = Int32.Parse(dr["PUSID"].ToString()),
                        TaskID = TaskId,
                        ExpName = Convert.ToString(dr["ExpName"].ToString()),
                        StartExpDate = Convert.ToDateTime(dr["StartExpDate"].ToString()),
                        EndExpDate = Convert.ToDateTime(dr["EndExpDate"].ToString()),
                        TaskName = Convert.ToString(dr["TaskName"].ToString()),
                        TaskDescription = Convert.ToString(dr["TaskDescription"].ToString()),
                        //ErrorMessage = Convert.ToString(dr["ErrorMessage"].ToString()),
                        DOI = Convert.ToString(dr["DOI"].ToString()),
                        // IsPostProcessingPass = bool.Parse(dr["IsPostProcessingPass"].ToString()),
                        PISiteName = Convert.ToString(dr["PISiteName"].ToString()),
                        PISiteUser = Convert.ToString(dr["PISiteUser"].ToString()),
                        UserName = Convert.ToString(dr["UserName"].ToString()),

                    });

                }
            }

            return lstExp;
        }

        public List<Experiment> GetAllExperimentsByExpIdsCsv(string expIdCsv)
        {
            List<Experiment> lstExp = new List<Experiment>();
            if (expIdCsv == "")
            {
                return lstExp;
            }

            using (DataTable dt = Dal.GetDataTable($@"select * from tsd.Experiment 
                                                        WHERE ExpID in({expIdCsv})"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstExp.Add(new Experiment
                    {
                        ExpID = Int32.Parse(dr["ExpID"].ToString()),
                        ExpName = Convert.ToString(dr["ExpName"].ToString()),
                    });

                }
            }

            return lstExp;
        }

        public string GetPiSiteNamesByPiIdsCsv(string piSiteIdsCsv, List<int> ExpId, bool returnIdPi)
        {
            string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());

            var strCondition = $"Where PUSID in ({piSiteIdsCsv})";

            // get all if empty
            if (piSiteIdsCsv == "")
            {
                strCondition = "";
            }

            var retVal = string.Empty;
            var lstReVal = new List<string>();

            using (DataTable dt = Dal.GetDataTable($@"Select  CONCAT(tt2.PISiteName, ' - ', tt2.UserName) as PISiteUser, tt2.PUSID From tsd.Experiment

                                                        inner join 
                                                        (Select PUSID, PIUserSite.PSID, tt.PISiteName, CONCAT(AspNetUsers.GivenName, '-', AspNetUsers.FamilyName) as UserName  From tsd.PIUserSite
                                                        inner join
                                                        (Select PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) as PISiteName From tsd.PISite
                                                        inner join tsd.PI on PI.PID = PISite.PID
                                                        inner join tsd.Site on Site.SiteID = PISite.SiteID
														
														) as tt on tt.PSID = PIUserSite.PSID
														inner join dbo.AspNetUsers on AspNetUsers.Id = PIUserSite.UserID {strCondition}) as tt2 on tt2.PUSID = Experiment.PUSID

                                                        Where Experiment.ExpID in ({ExpIDcsv}) "))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (returnIdPi)
                    {
                        var PUSID = Convert.ToString(dr["PUSID"]);
                        if (!string.IsNullOrEmpty(PUSID))
                        {
                            lstReVal.Add(PUSID);
                        }

                    }
                    else
                    {
                        var PiSiteUser = Convert.ToString(dr["PISiteUser"]);
                        if (!string.IsNullOrEmpty(PiSiteUser))
                        {
                            lstReVal.Add(PiSiteUser);
                        }

                    }
                }
            }

            return String.Join(",", lstReVal.Select(x => x.ToString()));

        }


        // Function Defintion: To get List of features for SessionInfo (on hold)
        public List<SessionInfo> GetSessionInfoDatabyExpID(int ExpId)
        {
            List<SessionInfo> SessionInfolst = new List<SessionInfo>();

            using (DataTable dt = Dal.GetDataTable($"Select * From tsd.SessionInfo Where ExpID = {ExpId}"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SessionInfolst.Add(new SessionInfo
                    {
                        SessionID = Int32.Parse(dr["SessionID"].ToString()),
                        ExpID = Int32.Parse(dr["ExpID"].ToString()),
                        UploadID = Int32.Parse(dr["UploadID"].ToString()),
                        AnimalID = Int32.Parse(dr["AnimalID"].ToString()),
                        UserID = Convert.ToString(dr["UserID"].ToString()),
                        Database_Name = Convert.ToString(dr["Database_Name"].ToString()),
                        Date_Time = Convert.ToString(dr["Date_Time"]),
                        Environment = Convert.ToString(dr["Environment"].ToString()),
                        Machine_Name = Convert.ToString(dr["Machine_Name"].ToString()),
                        Analysis_Name = Convert.ToString(dr["Analysis_Name"].ToString()),
                        Schedule_Name = Convert.ToString(dr["Schedule_Name"].ToString()),
                        Schedule_Run_ID = Convert.ToString(dr["Schedule_Run_ID"].ToString()),
                        Version = Convert.ToString(dr["Version"].ToString()),
                        Version_Name = Convert.ToString(dr["Version_Name"].ToString()),
                        Application_Version = Convert.ToString(dr["Application_Version"].ToString()),
                        Max_Number_Trials = Int32.Parse(dr["Max_Number_Trials"].ToString()),
                        Max_Schedule_Time = Convert.ToString(dr["Max_Schedule_Time"].ToString()),
                        Schedule_Description = Convert.ToString(dr["Schedule_Description"].ToString()),
                        Schedule_Start_Time = Convert.ToString(dr["Schedule_Start_Time"].ToString()),


                    });

                }
            }

            return SessionInfolst;
        }

        // Function Definition: Extracting list of all Schedules' name for the  selected Task ID
        public List<SubTask> GetScheduleListbyTaskID(int taskID)
        {
            List<SubTask> Schedulelst = new List<SubTask>();

            using (DataTable dt = Dal.GetDataTable($"select * from tsd.Sub_Task where Task_ID={taskID} and show=1"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Schedulelst.Add(new SubTask
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Task_ID = Int32.Parse(dr["Task_ID"].ToString()),
                        Name = Convert.ToString(dr["Name"].ToString()),
                        OriginalName = Convert.ToString(dr["OriginalName"].ToString()),
                        SubTaskDescription = Convert.ToString(dr["SubTaskDescription"].ToString()),

                    });

                }
            }

            return Schedulelst;
        }

        // Function Defintion: To get List of features for MarkerInfo (on hold)
        public List<String> GetMarkerInfoDatabySubTaskIdExpID(int subTaskId, List<int> ExpId)
        {
            List<string> markerInfoFeatureslst = new List<string>();

            string str = GetQuery_markerInfoList(subTaskId);
            if (!string.IsNullOrEmpty(str))
            {
                string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());
                string sql = $@"Select DISTINCT(FeatureName) From tsd.rbt_cached
                Where SessionID in (Select SessionID From tsd.SessionInfo Where {str} and (ExpID in ({ExpIDcsv})) );";

                using (DataTable dt = Dal.GetDataTable(sql))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        markerInfoFeatureslst.Add(

                            Convert.ToString(dr["FeatureName"].ToString())

                        );

                    }
                }
            }

            return markerInfoFeatureslst;
        }

        // Function Definition: To get List of Animal Age
        public List<SubExperiment> GetAnimalAgeDatabyExpIDs(List<int> ExpId)
        {
            List<SubExperiment> animalAgelst = new List<SubExperiment>();
            string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());
            string sql = $@"Select DISTINCT(LTRIM(RTRIM(Age.AgeInMonth))) as AgeInMonth,  AgeID From tsd.SubExperiment
                inner join tsd.Age on Age.ID = SubExperiment.AgeID
                 Where ExpID in ({ExpIDcsv}) and AgeInMonth IS NOT NULL and AgeInMonth !=''";


            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    animalAgelst.Add(new SubExperiment
                    {
                        AgeInMonth = Convert.ToString(dr["AgeInMonth"].ToString()),
                        AgeID = Int32.Parse(dr["AgeID"].ToString()),

                    });

                }

            }


            return animalAgelst;
        }

        // Function Definition: To get List of Animal Sex
        public List<Animal> GetAnimalSexDatabyExpIDs(List<int> ExpId)
        {
            List<Animal> animalSexlst = new List<Animal>();
            string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());
            string sql = $"Select DISTINCT(Sex) From tsd.Animal Where ExpID in ({ExpIDcsv}) and Sex IS NOT NULL and Sex !=''";


            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    animalSexlst.Add(new Animal
                    {
                        Sex = Convert.ToString(dr["Sex"].ToString()),

                    });

                }

            }


            return animalSexlst;
        }


        // Function Definition: to get List of Animal Genotype for Data Lab page
        // Function Definition: To get List of Animal Genotype if both strain and genotype are empty
        public List<Geno> GetAnimalGenotypeDatabyGenoIDList(List<int> GenoID)
        {
            List<Geno> animalGenotypelst = new List<Geno>();
            string GenoIDcsv = String.Join(",", GenoID.Select(x => x.ToString()).ToArray());
            if (!String.IsNullOrEmpty(GenoIDcsv))
            {
                string sql = $@"Select DISTINCT(Genotype.Genotype), Genotype.ID, Genotype.Link, Genotype.Description From tsd.Genotype
                            
                            Where Genotype.ID in ({GenoIDcsv}) and Genotype.Genotype IS NOT NULL and Genotype.Genotype !=''";


                using (DataTable dt = Dal.GetDataTable(sql))
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        animalGenotypelst.Add(new Geno
                        {
                            ID = Int32.Parse(dr["ID"].ToString()),
                            Genotype = Convert.ToString(dr["Genotype"].ToString()),
                            Link = Convert.ToString(dr["Link"].ToString()),
                            Description = Convert.ToString(dr["Description"].ToString()),

                        });

                    }

                }
            }

            return animalGenotypelst;
        }




        // Function Definition: To get List of Animal Genotype if both strain and genotype are empty
        public List<Geno> GetAnimalGenotypeDatabyExpIDs(List<int> ExpId)
        {
            List<Geno> animalGenotypelst = new List<Geno>();
            string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());
            string sql = $@"Select DISTINCT(Genotype.Genotype), Genotype.ID, Genotype.Link, Genotype.Description From tsd.Animal
                            inner join tsd.Genotype on Genotype.ID = Animal.GID
                            Where Animal.ExpID in ({ExpIDcsv}) and Genotype.Genotype IS NOT NULL and Genotype.Genotype !=''";


            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    animalGenotypelst.Add(new Geno
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Genotype = Convert.ToString(dr["Genotype"].ToString()),
                        Link = Convert.ToString(dr["Link"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),

                    });

                }

            }


            return animalGenotypelst;
        }

        // Function Definition: To get List of Animal  if genotype list is empty and based on the items selected in the strain
        public List<Geno> GetAnimalGenotypeDatabyExpIDsStrainList(List<int> ExpId, string AnimalStrainCsv)
        {
            List<Geno> animalGenotypelst = new List<Geno>();
            string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());
            string sql = $@"Select DISTINCT(Genotype.Genotype), Genotype.ID, Genotype.Link, Genotype.Description, Strain.Strain From tsd.Animal
                            inner join tsd.Genotype on Genotype.ID = Animal.GID
							inner join tsd.Strain on Strain.ID = Animal.SID
                            Where ExpID in ({ExpIDcsv}) and Genotype.Genotype IS NOT NULL and Genotype.Genotype !='' and Strain.ID in ({AnimalStrainCsv}) ";


            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    animalGenotypelst.Add(new Geno
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Genotype = Convert.ToString(dr["Genotype"].ToString()),
                        Link = Convert.ToString(dr["Link"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),

                    });

                }

            }


            return animalGenotypelst;
        }

        // Function Definition: To get List of Animal Strain
        public List<Strains> GetAnimalStrainDatabyExpIDs(List<int> ExpId)
        {
            List<Strains> animalStrainlst = new List<Strains>();
            string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());
            string sql = $"Select DISTINCT(Strain.Strain), Strain.ID, Strain.Link From tsd.Animal " +
                $" inner join tsd.Strain on Strain.ID = Animal.SID Where ExpID in ({ExpIDcsv}) and Strain IS NOT NULL and Strain.Strain !=''";


            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    animalStrainlst.Add(new Strains
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),
                        Link = Convert.ToString(dr["Link"].ToString()),

                    });

                }

            }


            return animalStrainlst;
        }

        // Function Definition to extract list of all interventions
        public List<SubExperiment> GetInterventionbyExpIDs(List<int> ExpId)
        {
            List<SubExperiment> InterventionList = new List<SubExperiment>();
            string ExpIDcsv = String.Join(",", ExpId.Select(x => x.ToString()).ToArray());

            if (!string.IsNullOrEmpty(ExpIDcsv))
            {
                string sql = $@"Select * From tsd.SubExperiment where expID in ({ExpIDcsv}) and IsIntervention=1";


                using (DataTable dt = Dal.GetDataTable(sql))
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        InterventionList.Add(new SubExperiment
                        {
                            SubExpID = Int32.Parse(dr["SubExpID"].ToString()),
                            IsIntervention = bool.Parse(dr["IsIntervention"].ToString()),
                            IsDrug = bool.Parse(dr["IsDrug"].ToString()),
                            DrugName = Convert.ToString(dr["DrugName"].ToString()),
                            DrugUnit = Convert.ToString(dr["DrugUnit"].ToString()),
                            DrugQuantity = Convert.ToString(dr["DrugQuantity"].ToString()),
                            InterventionDescription = Convert.ToString(dr["InterventionDescription"].ToString()),

                        });

                    }

                }
            }

            return InterventionList;
        }

        // Function Definition to extract data using User's Query
        public GetDataResult GetDataFromDB(DataExtraction data_extraction)
        {
            var isTrialByTrialEnabledFromClient = data_extraction.IsTrialByTrials;

            var decoded = DecodeDataExtraction(data_extraction);
            SaveDataExtractionLink(decoded.linkModel);

            // Inserting the code for Trial by Trial case
            var dtSummaryResult = Dal.GetDataTable(decoded.sql);
            DataTable dtFinalResult = new DataTable();

            if (isTrialByTrialEnabledFromClient)
            {
                //dtFinalResult.Columns.Add("SessionID");
                dtFinalResult.Columns.Add("AnimalID");
                dtFinalResult.Columns.Add("Age");
                dtFinalResult.Columns.Add("Sex");
                dtFinalResult.Columns.Add("GenoType");
                dtFinalResult.Columns.Add("Strain");
                dtFinalResult.Columns.Add("ExpName");
                dtFinalResult.Columns.Add("Housing");
                dtFinalResult.Columns.Add("LightCycle");
                dtFinalResult.Columns.Add("PISiteUser");
                dtFinalResult.Columns.Add("SessionName");

                dtFinalResult.Columns.Add("Image");
                dtFinalResult.Columns.Add("Image_Description");

                //if (data_extraction.TaskID == 3 || data_extraction.TaskID == 4)
                //{
                //}

                if (data_extraction.SubTaskID == 21 || data_extraction.SubTaskID == 22 || data_extraction.SubTaskID == 23 || data_extraction.SubTaskID == 27)
                {
                    dtFinalResult.Columns.Add("Stimulus_Duration");
                }

                List<string> lstSessionInfoNames = data_extraction.SessionInfoNames.ToList();
                foreach (string name in lstSessionInfoNames)
                {

                    dtFinalResult.Columns.Add(name);


                }

                foreach (DataRow drSummary in dtSummaryResult.Rows)
                {
                    SetRowsToFinalResult(drSummary, ref dtFinalResult, data_extraction.MarkerInfoNames, lstSessionInfoNames, data_extraction.SubTaskID);
                }

                // set first few columns

                dtFinalResult.Columns["AnimalID"].SetOrdinal(0);
                dtFinalResult.Columns["Age"].SetOrdinal(1);
                dtFinalResult.Columns["Sex"].SetOrdinal(2);
                dtFinalResult.Columns["GenoType"].SetOrdinal(3);
                dtFinalResult.Columns["Strain"].SetOrdinal(4);
                dtFinalResult.Columns["ExpName"].SetOrdinal(5);
                dtFinalResult.Columns["Housing"].SetOrdinal(6);
                dtFinalResult.Columns["LightCycle"].SetOrdinal(7);
                dtFinalResult.Columns["PISiteUser"].SetOrdinal(8);
                dtFinalResult.Columns["SessionName"].SetOrdinal(9);
                //dtFinalResult.Columns["SessionID"].SetOrdinal(8);
                dtFinalResult.Columns["Image"].SetOrdinal(10);
                dtFinalResult.Columns["Image_Description"].SetOrdinal(11);


                if (data_extraction.SubTaskID == 21 || data_extraction.SubTaskID == 22 || data_extraction.SubTaskID == 23 || data_extraction.SubTaskID == 24)
                {
                    dtFinalResult.Columns["Stimulus_Duration"].ColumnName = "Stimulus_Duration";
                }

                //DataColumnCollection columns = dtFinalResult.Columns;
                //if (columns.Contains("Schedule_Name"))
                //{
                //    dtFinalResult.Columns.Remove("ScheduleName");
                //}
                //else
                //{
                //    dtFinalResult.Columns["ScheduleName"].ColumnName = "Schedule_Name";
                //}

            }
            else
            {
                dtFinalResult = dtSummaryResult;
                //dtFinalResult.Columns.Remove("ScheduleName");

            }

            var DataExtractionList = dtFinalResult.AsDynamicEnumerable();

            var retVal = new GetDataResult
            {
                ListOfRows = DataExtractionList,
                LinkGuid = decoded.linkModel.LinkGuid,
            };

            return retVal;

        }

        // Inserting the code for Trial by Trial case
        private DataTable SetRowsToFinalResult(DataRow drSummary, ref DataTable dtResult, string[] featureNames, List<string> lstSessionInfoNames, int Subtaks)
        {
            var sessionId = Int32.Parse(drSummary["SessionID"].ToString());

            var strFeatureNamesCondition = string.Empty;
            if (featureNames.Length > 0)
            {
                var strOrCondition = string.Join(" OR ", featureNames.Select(x => " FeatureName = '" + x + "' "));
                strFeatureNamesCondition = $"AND ({strOrCondition})";
            }
            // order by FeatureName so we don't need to sort the output
            var sql = $"SELECT * from tsd.RBT_TouchScreen_Features WHERE SessionID = {sessionId} {strFeatureNamesCondition} ORDER BY FeatureName, ID";
            var dt = Dal.GetDataTable(sql);
            var index = 0;

            // track the count of columns
            var dictColNames = new Dictionary<string, int>();

            var drResultRow = dtResult.NewRow();

            foreach (DataRow dr in dt.Rows)
            {
                index += 1;

                var featureName = dr["FeatureName"].ToString().ToUpper();

                if (dictColNames.ContainsKey(featureName))
                {
                    dictColNames[featureName] += 1;
                }
                else
                {
                    dictColNames[featureName] = 1;
                }

                index = dictColNames[featureName];

                featureName = dr["FeatureName"].ToString().ToUpper() + " _" + index;


                var SourceTypeID = Int16.Parse(dr["SourceTypeID"].ToString());
                float value = 0;

                switch (SourceTypeID)
                {
                    case 1:
                        value = dr["Results"].ToString() == "" ? 0 : float.Parse(dr["Results"].ToString());
                        break;
                    case 2:
                        value = dr["Count"].ToString() == "" ? 0 : float.Parse(dr["Count"].ToString());
                        break;
                    case 3:
                        value = dr["Duration"].ToString() == "" ? 0 : (float.Parse(dr["Duration"].ToString()) / 1000000);
                        break;

                    default:
                        break;
                }


                //if column exist, add the value, if doesn't exist add column and set the value

                // TODO: need to insert new column to the correct location
                if (dtResult.Columns.IndexOf(featureName) == -1)
                {
                    drResultRow.Table.Columns.Add(featureName);
                }

                drResultRow[featureName] = value;

            }

            //drResultRow["SessionID"] = sessionId;
            drResultRow["AnimalID"] = Convert.ToString(drSummary["AnimalID"]);
            drResultRow["Age"] = Convert.ToString(drSummary["Age"]);
            drResultRow["Sex"] = Convert.ToString(drSummary["Sex"]);
            drResultRow["GenoType"] = Convert.ToString(drSummary["GenoType"]);
            drResultRow["Strain"] = Convert.ToString(drSummary["Strain"]);
            drResultRow["ExpName"] = Convert.ToString(drSummary["ExpName"]);
            drResultRow["Housing"] = Convert.ToString(drSummary["Housing"]);
            drResultRow["LightCycle"] = Convert.ToString(drSummary["LightCycle"]);
            drResultRow["PISiteUser"] = Convert.ToString(drSummary["PISiteUser"]);
            drResultRow["SessionName"] = Convert.ToString(drSummary["SessionName"]);
            drResultRow["Image"] = Convert.ToString(drSummary["Image"]); //ExperimentService.GetImagePathFromImageIdCsv(Convert.ToString(drSummary["Image"]));
            drResultRow["Image_Description"] = Convert.ToString(drSummary["Image_Description"]);

            if (Subtaks == 21 || Subtaks == 22 || Subtaks == 23 || Subtaks == 27)
            {
                drResultRow["Stimulus_Duration"] = Convert.ToString(drSummary["Stimulus_Duration"]);
            }



            foreach (string name in lstSessionInfoNames)
            {
                string TempName = name.Trim();
                if (drSummary.Table.Columns.Contains(TempName))
                {
                    drResultRow[TempName] = drSummary[TempName].ToString();
                }

            }

            dtResult.Rows.Add(drResultRow);

            return dtResult;
        }



        public async Task <GetDataResult> GetDataFromDbByLinkGuid(Guid linkGuid)
        {
            //var linkModel = new LinkModel();
            string interventionDescription = string.Empty;
            string SessionNameDescription = string.Empty;
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@LinkGuid", linkGuid));

            string selectQuery = "select top 1 * from tsd.Links where LinkGuid=@LinkGuid";

            var linkModels = await Dal.GetReader(selectQuery, reader => new LinkModel
            {
                TaskId = reader.GetInt32("TaskID"),
                SpeciesId = reader.GetInt32("SpeciesID"),
                SubTaskId = reader.GetInt32("SubTaskId"),
                ExpIdCsv = reader.GetString("ExpIdCsv"),
                AnimalAgeCsv = reader.GetString("AnimalAgeCsv"),
                AnimalSexCsv = reader.GetString("AnimalSexCsv"),
                AnimalGenotypeCsv = reader.GetString("AnimalGenotypeCsv"),
                AnimalStrainCsv = reader.GetString("AnimalStrainCsv"),
                PiSiteIdsCsv = reader.GetString("PiSiteIdsCsv"),
                SessionInfoNamesCsv = reader.GetString("SessionInfoNamesCsv"),
                MarkerInfoNamesCsv = reader.GetString("MarkerInfoNamesCsv"),
                AggNamesCsv = reader.GetString("AggNamesCsv"),
                IsTrialByTrials = reader.GetBoolean("IsTrialByTrial"),
                SubExpIDcsv = reader.GetString("SubExpIDcsv"),
                SessionNameCsv = reader.GetString("SessionNameCsv")
            }, parameters);


            var linkModel = linkModels.FirstOrDefault(); 

            if (!string.IsNullOrEmpty(linkModel.SubExpIDcsv))
            {
                interventionDescription = $"Intervention: <b>{GetInterventionFromIdCsv(linkModel.SubExpIDcsv)}</b><br />";
            }

            if (!string.IsNullOrEmpty(linkModel.SessionNameCsv))
            {
                SessionNameDescription = $"SessionName: <b>{linkModel.SessionNameCsv}</b><br />";
            }

            linkModel.Description = $@"Species: <b>{linkModel.Species}</b><br />
                                        Task Name: <b>{linkModel.TaskName}</b><br />
                                        Sub Task Name: <b>{linkModel.SubTaskName}</b><br /> 
                                        Experiment Name: <b>{String.Join(", ", GetAllExperimentsByExpIdsCsv(linkModel.ExpIdCsv).Select(x => x.ExpName.ToString()).ToArray())}</b><br />
                                        Animal Age: <b>{GetAgeCsvFromIdCsv(linkModel.AnimalAgeCsv)}</b><br /> 
                                        Animal Sex: <b>{linkModel.AnimalSexCsv}</b><br />
                                        Animal Genotype: <b>{GetGenotypeCsvFromIdCsv(linkModel.AnimalGenotypeCsv)}</b><br /> 
                                        Animal Strain: <b>{GetStrainCsvFromIdCsv(linkModel.AnimalStrainCsv)}</b><br />
                                        PI/Site Name: <b>{GetPiSiteNamesByPiIdsCsv(linkModel.PiSiteIdsCsv, linkModel.ExpIdCsv.Split(',').Select(int.Parse).ToList(), false)}</b><br /> 
                                        Session Info Names: <b>{linkModel.SessionInfoNamesCsv.Replace("SessionInfo.", "")}</b><br />
                                        Marker Info Names: <b>{linkModel.MarkerInfoNamesCsv.Replace("§", ", ")}</b><br /> 
                                        Aggregate functions: <b>{linkModel.AggNamesCsv.Replace("§", ", ")}</b><br />
                                        {interventionDescription}
                                        {SessionNameDescription} 
                                            ";
            

            var sql = DataExtractionQuery(linkModel.TaskId, linkModel.SpeciesId, linkModel.SubTaskId, linkModel.ExpIdCsv, linkModel.AnimalAgeCsv,
                linkModel.AnimalSexCsv, linkModel.AnimalGenotypeCsv, linkModel.AnimalStrainCsv,
                linkModel.PiSiteIdsCsv, linkModel.SessionInfoNamesCsv,
                linkModel.MarkerInfoNamesCsv.Split('§'),
                linkModel.AggNamesCsv, linkModel.IsTrialByTrials, linkModel.SubExpIDcsv, linkModel.SessionNameCsv);

            var dtSummaryResult = Dal.GetDataTable(sql);
            DataTable dtFinalResult = new DataTable();

            if (linkModel.IsTrialByTrials)
            {
                //dtFinalResult.Columns.Add("SessionID");
                dtFinalResult.Columns.Add("AnimalID");
                dtFinalResult.Columns.Add("Age");
                dtFinalResult.Columns.Add("Sex");
                dtFinalResult.Columns.Add("GenoType");
                dtFinalResult.Columns.Add("Strain");
                dtFinalResult.Columns.Add("ExpName");
                dtFinalResult.Columns.Add("Housing");
                dtFinalResult.Columns.Add("Lightcycle");
                dtFinalResult.Columns.Add("PISiteUser");
                dtFinalResult.Columns.Add("SessionName");
                dtFinalResult.Columns.Add("Image");
                dtFinalResult.Columns.Add("Image_Description");


                //dtFinalResult.Columns.Add("Schedule_Name");

                if (linkModel.SubTaskId == 21 || linkModel.SubTaskId == 22 || linkModel.SubTaskId == 23 || linkModel.SubTaskId == 27)
                {
                    dtFinalResult.Columns.Add("Stimulus_Duration");
                }

                List<string> lstSessionInfoNames = linkModel.SessionInfoNamesCsv.Replace("SessionInfo.", "").Split(',').Select(s => s.Trim()).ToList();

                foreach (string name in lstSessionInfoNames)
                {

                    dtFinalResult.Columns.Add(name);


                }

                foreach (DataRow drSummary in dtSummaryResult.Rows)
                {
                    SetRowsToFinalResult(drSummary, ref dtFinalResult, linkModel.MarkerInfoNamesCsv.Split('§'), lstSessionInfoNames, linkModel.SubTaskId);
                }


                // set first few columns

                dtFinalResult.Columns["AnimalID"].SetOrdinal(0);
                dtFinalResult.Columns["Age"].SetOrdinal(1);
                dtFinalResult.Columns["Sex"].SetOrdinal(2);
                dtFinalResult.Columns["GenoType"].SetOrdinal(3);
                dtFinalResult.Columns["Strain"].SetOrdinal(4);
                dtFinalResult.Columns["ExpName"].SetOrdinal(5);
                dtFinalResult.Columns["Housing"].SetOrdinal(6);
                dtFinalResult.Columns["LightCycle"].SetOrdinal(7);
                dtFinalResult.Columns["PISiteUser"].SetOrdinal(8);
                dtFinalResult.Columns["SessionName"].SetOrdinal(9);
                dtFinalResult.Columns["Image"].SetOrdinal(10);
                dtFinalResult.Columns["Image_Description"].SetOrdinal(11);

                //dtFinalResult.Columns["SessionID"].SetOrdinal(8);

                if (linkModel.SubTaskId == 21 || linkModel.SubTaskId == 22 || linkModel.SubTaskId == 23 || linkModel.SubTaskId == 27)
                {
                    dtFinalResult.Columns["Stimulus_Duration"].ColumnName = "Stimulus_Duration";
                }

            }
            else
            {
                dtFinalResult = dtSummaryResult;
                //dtFinalResult.Columns.Remove("ScheduleName");
            }

            var DataExtractionList = dtFinalResult.AsDynamicEnumerable();

            var retVal = new GetDataResult
            {
                ListOfRows = DataExtractionList,
                Description = linkModel.Description,
                SubTaskID = linkModel.SubTaskId,
            };

            return retVal;

        }

        private string GetAgeCsvFromIdCsv(string ageIdCsv)
        {
            if (string.IsNullOrEmpty(ageIdCsv))
            {
                return "";
            }

            string sql = $@"Select * From tsd.Age Where Id in ({ageIdCsv}) ";

            List<string> ageInMonthList = new List<string>();

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    ageInMonthList.Add(
                        Convert.ToString(dr["AgeInMonth"].ToString())
                    );
                }

            }

            return string.Join(", ", ageInMonthList);
        }

        private string GetGenotypeCsvFromIdCsv(string genotypeIdCsv)
        {
            if (string.IsNullOrEmpty(genotypeIdCsv))
            {
                return "";
            }

            string sql = $@"Select * From tsd.genotype Where Id in ({genotypeIdCsv}) ";

            List<string> genoypeList = new List<string>();

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    genoypeList.Add(
                        Convert.ToString(dr["genotype"].ToString())
                    );
                }

            }

            return string.Join(", ", genoypeList);
        }

        private string GetStrainCsvFromIdCsv(string strainIdCsv)
        {
            if (string.IsNullOrEmpty(strainIdCsv))
            {
                return "";
            }

            string sql = $@"Select * From tsd.strain Where Id in ({strainIdCsv}) ";

            List<string> strainList = new List<string>();

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    strainList.Add(
                        Convert.ToString(dr["strain"].ToString())
                    );
                }

            }

            return string.Join(", ", strainList);
        }


        // Function definition to get the name of intervention based in subexpID for data linkc page
        private string GetInterventionFromIdCsv(string subExpIdCsv)
        {
            if (string.IsNullOrEmpty(subExpIdCsv))
            {
                return "";
            }

            string sql = $@"Select 	CASE 
							WHEN SubExperiment.IsDrug = 1 THEN CONCAT(SubExperiment.DrugName, '-', SubExperiment.DrugQuantity, '-', SubExperiment.DrugUnit)
							WHEN SubExperiment.IsDrug = 0 THEN SubExperiment.InterventionDescription
							END as Intervention
							From tsd.SubExperiment Where SubExperiment.SubExpID in ({subExpIdCsv}) and SubExperiment.IsIntervention=1 ";

            List<string> InterventionList = new List<string>();

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    InterventionList.Add(
                        Convert.ToString(dr["Intervention"].ToString())
                    );
                }

            }
            if (InterventionList.Count > 0)
            {
                return string.Join(", ", InterventionList);
            }
            else
            {
                return "";
            }


        }

        public (string sql, LinkModel linkModel) DecodeDataExtraction(DataExtraction data_extraction)
        {

            int SpeciesID = data_extraction.SpeciesID;
            int TaskID = data_extraction.TaskID;
            string ExpIDcsv = String.Join(",", data_extraction.ExpIDs.Select(x => x.ToString()).ToArray());
            int SubTaskID = data_extraction.SubTaskID;
            string[] MarkerInfoNames = data_extraction.MarkerInfoNames;
            string AggNames = data_extraction.AggNames;
            string AnimalAgeCsv = "";
            string AnimalSexCsv = "";
            string AnimalGenotypeCsv = "";
            string AnimalStrainCsv = "";
            string PiSiteIDsCsv = "";
            string SessionInfoNamesCsv = "";
            bool isTrialByTrial = data_extraction.IsTrialByTrials;
            //string InterventionQuery = "";
            string SubExpIDcsv = "";
            string SessionNamesCsv = "";

            if (data_extraction.SessionName != null && data_extraction.SessionName.Any())
            {
                //SessionNamesCsv = String.Join(",", data_extraction.SessionName.Select(x => x.ToString()).ToArray());
                SessionNamesCsv = String.Join(",", data_extraction.SessionName.Select(x => "'" + x.ToString() + "' ").ToArray());

            }

            if (data_extraction.SubExpID != null && data_extraction.SubExpID.Length != 0)
            {

                SubExpIDcsv = String.Join(",", data_extraction.SubExpID.Select(x => x.ToString()).ToArray());

            }
            if (data_extraction.AgeVals != null && data_extraction.AgeVals.Length != 0)
            {
                AnimalAgeCsv = String.Join(",", data_extraction.AgeVals.Select(x => x.ToString()).ToArray());
            }

            if (data_extraction.SexVals != null && data_extraction.SexVals.Length != 0)
            {
                AnimalSexCsv = String.Join(",", data_extraction.SexVals.Select(x => "'" + x.ToString() + "'").ToArray());
            }

            if (data_extraction.GenotypeVals != null && data_extraction.GenotypeVals.Length != 0)
            {
                AnimalGenotypeCsv = String.Join(",", data_extraction.GenotypeVals.Select(x => x.ToString()).ToArray());
            }

            if (data_extraction.StrainVals != null && data_extraction.StrainVals.Length != 0)
            {
                AnimalStrainCsv = String.Join(",", data_extraction.StrainVals.Select(x => x.ToString()).ToArray());
            }

            if (data_extraction.PiSiteIDS != null && data_extraction.PiSiteIDS.Length != 0)
            {
                PiSiteIDsCsv = String.Join(",", data_extraction.PiSiteIDS.Select(x => x.ToString()).ToArray());
            }

            List<string> lstSessionInfoNames = data_extraction.SessionInfoNames.Select(c => { c = "SessionInfo." + c; return c; }).ToList();
            SessionInfoNamesCsv = string.Join(", ", lstSessionInfoNames);

            var linkModel = new LinkModel
            {
                LinkGuid = Guid.NewGuid(),
                TaskId = TaskID,
                SpeciesId = SpeciesID,
                Species = data_extraction.Species,
                TaskName = data_extraction.TaskName,
                SubTaskId = SubTaskID,
                SubTaskName = data_extraction.SubTaskName,
                ExpIdCsv = ExpIDcsv,
                AnimalAgeCsv = AnimalAgeCsv,
                AnimalSexCsv = AnimalSexCsv,
                AnimalGenotypeCsv = AnimalGenotypeCsv,
                AnimalStrainCsv = AnimalStrainCsv,
                PiSiteIdsCsv = PiSiteIDsCsv,
                SessionInfoNamesCsv = SessionInfoNamesCsv,
                MarkerInfoNamesCsv = MarkerInfoNames.Length > 0 ? string.Join("§", MarkerInfoNames) : "", // § is separator char
                //AggNamesCsv = AggNames.Length > 0 ? string.Join("§", AggNames) : "",
                AggNamesCsv = AggNames,
                IsTrialByTrials = data_extraction.IsTrialByTrials,
                SubExpIDcsv = SubExpIDcsv,
                SessionNameCsv = SessionNamesCsv,

            };

            var sql = string.Empty;

            sql = DataExtractionQuery(TaskID, SpeciesID, SubTaskID, ExpIDcsv, AnimalAgeCsv, AnimalSexCsv, AnimalGenotypeCsv, AnimalStrainCsv,
                                      PiSiteIDsCsv, SessionInfoNamesCsv, MarkerInfoNames, AggNames, isTrialByTrial, SubExpIDcsv, SessionNamesCsv);

            return (sql, linkModel);
        }

        // Function Definition for dataextractionQuery
        public string DataExtractionQuery(int TaskID, int SpeciesID, int subTaskId, string expIDcsv, string animalAgeCsv, string animalSexCsv,
            string animalGenotypeCsv, string animalStrainCsv, string piSiteIdsCsv, string sessionInfoNamesCsv, string[] markerInfoNames,
            string aggNames, bool isTrialByTrial, string SubExpIDcsv, string SessionNamesCsv)
        {
            string sql = "";
            string AgeCondition = "";
            string SexCondition = "";
            string GenotypeCondition = "";
            string StrainCondition = "";
            string PiSiteCondition = "";
            string InterventionCondition = "";
            string InterventionQuery = "";
            string SessionNameCondition = "";
            string StimulusDurationCondition1 = "";
            string StimulusDurationCondition2 = "";

            string str = ""; // GetQuery_markerInfoList(subTaskId);

            //  Check to see Animal Info and PiSite info is Null or Not for including them to subQuery1
            if (animalAgeCsv != "")
            {
                AgeCondition = $"and Age.ID in ({animalAgeCsv}) ";
            }

            if (animalSexCsv != "")
            {
                SexCondition = $"and Animal.Sex in ({animalSexCsv}) ";
            }

            if (animalGenotypeCsv != "")
            {
                GenotypeCondition = $"and Genotype.ID in ({animalGenotypeCsv}) ";
            }

            if (animalStrainCsv != "")
            {
                StrainCondition = $"and Strain.ID in ({animalStrainCsv}) ";
            }

            if (piSiteIdsCsv != "")
            {
                PiSiteCondition = $"and tt2.PUSID in ({piSiteIdsCsv}) ";
            }
            if (SubExpIDcsv != "")
            {
                InterventionCondition = $"or ss.SubExpID in ({SubExpIDcsv})";
                InterventionQuery = @",
						            CASE 
						            WHEN ss.IsDrug = 1 THEN CONCAT(ss.DrugName, '-', ss.DrugQuantity, '-', ss.DrugUnit)
						            WHEN ss.IsDrug = 0 THEN ss.InterventionDescription
						            END as Intervention";
            }

            if (SessionNamesCsv != "")
            {
                SessionNameCondition = $"and SessionInfo.SessionName in ({SessionNamesCsv}) ";
            }
            else // IF no sessionName was sent from Client to server (SessionNameCsv is Null), then get the conditoin from GetQuery_markerInfoList Function
            {
                str = GetQuery_markerInfoList(subTaskId);
                str = str + " and ";
            }

            // Including Stimulus duration in Data extraction Query if 5-chice testing sessions/subtasks are selected
            if (subTaskId == 21 || subTaskId == 22 || subTaskId == 23 || subTaskId == 27)
            {
                StimulusDurationCondition1 = ", Stimulus_Duration ";

                //StimulusDurationCondition2 = "and stimulus_Duration is not null ";

            }

            string subQuery1 = $@"Select Animal.UserAnimalID as AnimalID, Age.AgeInMonth as Age, Animal.Sex as Sex, Genotype.Genotype,
                                 Strain.Strain, SessionInfo.ExpID, SessionInfo.SessionID, Experiment.ExpName, ss.Housing, ss.LightCycle, CONCAT(tt2.PISiteName, ' - ', tt2.UserName) as PISiteUser, SessionName,
                                 ss.SubExpID,

								STUFF(( SELECT distinct ';' + CONCAT('<img src =""', imagepath, '"" width=''30'' height=''30'' style=''margin-top:15px;''/>') as path
                                From tsd.image
                                Where image.ID in (SELECT * FROM dbo.CSVToTable(ss.ImageIds))
                                FOR XML PATH(''), type
                                ).value('.', 'nvarchar(max)'),1,1,'') as Image,

                                ss.ImageDescription as Image_Description,

                                 {sessionInfoNamesCsv} as sessionInfoName
                                 {StimulusDurationCondition1}
                                 {InterventionQuery}
                                 From tsd.SessionInfo
                                 inner join tsd.Animal on Animal.AnimalID = SessionInfo.AnimalID
                                 inner join tsd.Experiment on Experiment.ExpID = SessionInfo.ExpID
                                 inner join tsd.Genotype on Genotype.ID = Animal.GID
                                 inner join tsd.Strain on Strain.ID = Animal.SID

								 inner join (
								 Select PUSID, PIUserSite.PSID, tt.PISiteName , CONCAT(AspNetUsers.GivenName, '-', AspNetUsers.FamilyName) as UserName From tsd.PIUserSite
                                                        inner join
                                                        (Select PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) as PISiteName From tsd.PISite
                                                        inner join tsd.PI on PI.PID = PISite.PID
                                                        inner join tsd.Site on Site.SiteID = PISite.SiteID) as tt on tt.PSID = PIUserSite.PSID
                                                        inner join dbo.AspNetUsers on AspNetUsers.Id = PIUserSite.UserID

								 ) as tt2 on  tt2.PUSID = Experiment.PUSID

                                 inner join (
								 Select UploadID, SubExperiment.ExpID, SubExperiment.SubExpID, SubExperiment.AgeID, 
                                        SubExperiment.IsDrug, SubExperiment.DrugQuantity, SubExperiment.DrugName, SubExperiment.DrugUnit, SubExperiment.IsIntervention,
                                        SubExperiment.InterventionDescription, SubExperiment.ImageIds, SubExperiment.ImageDescription, SubExperiment.Housing, SubExperiment.LightCycle
                                        From tsd.SubExperiment 
											inner join tsd.Upload on SubExperiment.SubExpID = Upload.SubExpID
								 ) as ss on  ss.UploadID = SessionInfo.UploadID
                                inner join tsd.Age on Age.ID = ss.AgeID

                                 Where {str}  (SessionInfo.ExpID in ({expIDcsv}) {AgeCondition} {SexCondition} {GenotypeCondition} {StrainCondition} {PiSiteCondition} {SessionNameCondition})";

            // Creating subQuery 2 from RBT_TouchScreen_Features

            List<string> FeaturesLst = new List<string>();
            List<string> ConditionLst = new List<string>();


            if (!isTrialByTrial)

            {
                string[] aggNamesList = aggNames.Split('§');

                for (int i = 0; i < markerInfoNames.Length; i++)
                {

                    for (int j = 0; j < aggNamesList.Length; j++)
                    {
                        if (aggNamesList[j] == "MEAN") { aggNamesList[j] = "AVG"; };
                        string subString = $"[{aggNamesList[j]}_{markerInfoNames[i]}]";
                        FeaturesLst.Add(subString);

                    }
                }
            }

            var strFeatureLst = string.Join(", ", FeaturesLst);

            string strSessionIdDrop = "";
            string subQuery2 = "Select * From ( Select SessionID as S_ID ";
            if (isTrialByTrial)
            {
                subQuery2 += @" From tsd.RBT_TouchScreen_Features
                            group by SessionID
                            ) tmp ";

            }
            else
            {
                subQuery2 += ", " + strFeatureLst;
                switch(aggNames)
                {
                    case "MEAN":
                        subQuery2 += @" From tsd.rbt_data_cached_avg
                                ) tmp ";
                        break;
                    case "STDEV":
                        subQuery2 += @" From tsd.rbt_data_cached_std
                                ) tmp ";
                        break;
                    case "COUNT":
                        subQuery2 += @" From tsd.rbt_data_cached_cnt
                                ) tmp ";
                        break;
                    case "SUM":
                        subQuery2 += @" From tsd.rbt_data_cached_sum
                                ) tmp ";
                        break;
                    default:
                        subQuery2 += @" From tsd.rbt_data_cached
                                ) tmp ";
                        break;
                }

                //subQuery2 += @" From rbt_data_cached
                //                ) tmp ";

                strSessionIdDrop = ", SessionID";

            }

            // Making the main Query
            sql = $@"SELECT * INTO #TempTable
                    From ( Select SessionInfoTmp.*, MarkerInfoTmp.* From (
                    ({subQuery1}) as SessionInfoTmp inner join ({subQuery2}) as MarkerInfoTmp on SessionInfoTmp.SessionID = MarkerInfoTmp.S_ID ) ) as MainTable
                    ALTER TABLE #TempTable
                    DROP COLUMN S_ID, ExpID, SubExpID {strSessionIdDrop}

                    Select * From #TempTable
                    DROP TABLE #TempTable ";


            return sql;
        }

        private string ReplaceBadChars(string subString)
        {
            var retval = subString;
            retval = retval.Replace(".", "_");
            retval = retval.Replace("@", "at");
            retval = retval.Replace("%", "percent");
            retval = retval.Replace("+", "plus");
            retval = retval.Replace(" - ", "_");
            retval = retval.Replace("-", "minus");
            retval = retval.Replace("#", "_");
            retval = retval.Replace("/", "_");
            retval = retval.Replace(" ", "_");

            return retval;
        }

        private void SaveDataExtractionLink(LinkModel linkModel)
        {

            string sql = $@"insert into tsd.Links
                            (
                                LinkGuid, TaskId, TaskName, SpeciesID, Species, SubTaskId, SubTaskName, ExpIdCsv, AnimalAgeCsv, AnimalSexCsv, AnimalGenotypeCsv,
                                AnimalStrainCsv, PiSiteIdsCsv, SessionInfoNamesCsv, MarkerInfoNamesCsv, AggNamesCsv, IsTrialByTrial, SubExpIDcsv, SessionNameCsv
                            ) Values 
                            (
                                @LinkGuid, @TaskId, @TaskName, @SpeciesID, @Species, @SubTaskId, @SubTaskName, @ExpIdCsv, @AnimalAgeCsv, @AnimalSexCsv, @AnimalGenotypeCsv,
                                @AnimalStrainCsv, @PiSiteIdsCsv, @SessionInfoNamesCsv, @MarkerInfoNamesCsv, @AggNamesCsv, @IsTrialByTrial, @SubExpIDcsv, @SessionNameCsv
                            )";

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@LinkGuid", linkModel.LinkGuid));
            parameters.Add(new SqlParameter("@TaskId", linkModel.TaskId));
            parameters.Add(new SqlParameter("@TaskName", linkModel.TaskName));
            parameters.Add(new SqlParameter("@SpeciesID", linkModel.SpeciesId));
            parameters.Add(new SqlParameter("@Species", linkModel.Species));
            parameters.Add(new SqlParameter("@SubTaskId", linkModel.SubTaskId));
            parameters.Add(new SqlParameter("@SubTaskName", linkModel.SubTaskName));
            parameters.Add(new SqlParameter("@ExpIdCsv", string.IsNullOrEmpty(linkModel.ExpIdCsv) ? "" : linkModel.ExpIdCsv));
            parameters.Add(new SqlParameter("@AnimalAgeCsv", string.IsNullOrEmpty(linkModel.AnimalAgeCsv) ? "" : linkModel.AnimalAgeCsv));
            parameters.Add(new SqlParameter("@AnimalSexCsv", string.IsNullOrEmpty(linkModel.AnimalSexCsv) ? "" : linkModel.AnimalSexCsv));
            parameters.Add(new SqlParameter("@AnimalGenotypeCsv", string.IsNullOrEmpty(linkModel.AnimalGenotypeCsv) ? "" : linkModel.AnimalGenotypeCsv));
            parameters.Add(new SqlParameter("@AnimalStrainCsv", string.IsNullOrEmpty(linkModel.AnimalStrainCsv) ? "" : linkModel.AnimalStrainCsv));
            parameters.Add(new SqlParameter("@PiSiteIdsCsv", string.IsNullOrEmpty(linkModel.PiSiteIdsCsv) ? "" : linkModel.PiSiteIdsCsv));
            parameters.Add(new SqlParameter("@SessionInfoNamesCsv", string.IsNullOrEmpty(linkModel.SessionInfoNamesCsv) ? "" : linkModel.SessionInfoNamesCsv));
            parameters.Add(new SqlParameter("@MarkerInfoNamesCsv", string.IsNullOrEmpty(linkModel.MarkerInfoNamesCsv) ? "" : linkModel.MarkerInfoNamesCsv));
            parameters.Add(new SqlParameter("@AggNamesCsv", string.IsNullOrEmpty(linkModel.AggNamesCsv) ? "" : linkModel.AggNamesCsv));
            parameters.Add(new SqlParameter("@IsTrialByTrial", linkModel.IsTrialByTrials));
            parameters.Add(new SqlParameter("@SubExpIDcsv", linkModel.SubExpIDcsv));
            parameters.Add(new SqlParameter("@SessionNameCsv", string.IsNullOrEmpty(linkModel.SessionNameCsv) ? "" : linkModel.SessionNameCsv));



            Dal.ExecScalar(CommandType.Text, sql, parameters.ToArray());

        }

        // if generate link clicked the status of IsSaved changed to true.
        public async Task<bool> MarkLinkAsSaved(Guid linkGuid)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@LinkGuid", linkGuid));
            string selectQuery = "select top 1 * from tsd.Links where LinkGuid=@LinkGuid";
            // fill Links table data (if not already)
            var savedLink = (await Dal.GetReader(selectQuery, reader => new LinkModel
            {
                ExpIdCsv = reader.GetString("ExpIdCsv"),
                PiSiteIdsCsv = reader.GetString("PiSiteIdsCsv"),
                AnimalGenotypeCsv = reader.GetString("AnimalgenotypeCsv"),
                AnimalAgeCsv = reader.GetString("AnimalAgeCsv"),
                AnimalSexCsv = reader.GetString("AnimalSexCsv"),
                AnimalStrainCsv =reader.GetString("AnimalStrainCsv")

            },  parameters)).FirstOrDefault();

            var ExpIdCsv = savedLink.ExpIdCsv;
            var PiSiteIdsCsv = savedLink.PiSiteIdsCsv;
            var AnimalgenotypeCsv = savedLink.AnimalGenotypeCsv;
            var ExpNameCsv = string.Empty;
            var AnimalAgeCsv = savedLink.AnimalAgeCsv;
            var AnimalSexCsv = savedLink.AnimalSexCsv;
            var AnimalStrainCsv = savedLink.AnimalStrainCsv;
            var PiSiteUserIdCsv = string.Empty;
            var SubExpIdCsv = string.Empty;


            List<int> expIds = ExpIdCsv.Split(',').Select(int.Parse).ToList();
            var lstExps = GetAllExperimentsByExpIdsCsv(ExpIdCsv);
            ExpNameCsv = String.Join(", ", lstExps.Select(x => x.ExpName.ToString()).ToArray());
            PiSiteUserIdCsv = GetPiSiteNamesByPiIdsCsv(PiSiteIdsCsv, expIds, true);

            // if both strain & genotype are empty get all the result based on all the available Genotype and Strain in the selected Experiments
            if (AnimalStrainCsv == string.Empty && AnimalgenotypeCsv == string.Empty)
            {
                // get all genotypes
                var lstAnimals = GetAnimalGenotypeDatabyExpIDs(expIds);
                AnimalgenotypeCsv = String.Join(",", lstAnimals.Select(x => x.ID.ToString()).ToArray());
            }

            if (AnimalgenotypeCsv == string.Empty)
            {
                var lstAnimals = GetAnimalGenotypeDatabyExpIDsStrainList(expIds, AnimalStrainCsv);
                AnimalgenotypeCsv = String.Join(",", lstAnimals.Select(x => x.ID.ToString()).ToArray());
            }



            if (AnimalAgeCsv == string.Empty)
            {
                var lstAnimals = GetAnimalAgeDatabyExpIDs(expIds);
                AnimalAgeCsv = String.Join(",", lstAnimals.Select(x => x.AgeID.ToString()).ToArray());
            }

            if (AnimalStrainCsv == string.Empty)
            {
                var lstAnimals = GetAnimalStrainDatabyExpIDs(expIds);
                AnimalStrainCsv = String.Join(",", lstAnimals.Select(x => x.ID.ToString()).ToArray());
            }

            if (AnimalSexCsv == string.Empty)
            {
                var lstAnimals = GetAnimalSexDatabyExpIDs(expIds);
                AnimalSexCsv = String.Join(",", lstAnimals.Select(x => "'" + x.Sex.ToString() + "'").ToArray());
            }
            parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@LinkGuid", linkGuid));
            parameters.Add(new SqlParameter("@AnimalgenotypeCsv", AnimalgenotypeCsv));
            parameters.Add(new SqlParameter("@ExpNameCsv", ExpNameCsv));
            parameters.Add(new SqlParameter("@AnimalAgeCsv", AnimalAgeCsv));
            parameters.Add(new SqlParameter("@AnimalStrainCsv", AnimalStrainCsv));
            parameters.Add(new SqlParameter("@AnimalSexCsv", AnimalSexCsv));
            parameters.Add(new SqlParameter("@PiSiteIdsCsv", PiSiteUserIdCsv));

            string sql = $@"update tsd.Links
                            set IsSaved = 1, AnimalgenotypeCsv = @AnimalgenotypeCsv, ExpNameCsv = @ExpNameCsv, AnimalAgeCsv = @AnimalAgeCsv,
                            AnimalStrainCsv = @AnimalStrainCsv, AnimalSexCsv = @AnimalSexCsv, PiSiteIdsCsv = @PiSiteIdsCsv
                            where LinkGuid = @LinkGuid ";

            return (Dal.ExecuteNonQueryAsync(sql, parameters.ToArray()).Result == 1);
        }

        public void IncreaseDLCounter(string buttonName)
        {
            string sql = $@"Insert into tsd.Metrics (ButtonName) Values('{buttonName}')";

            Dal.ExecuteNonQuery(sql);
        }


        #region Getting the Query for making MarkerInfo List

        public string GetQuery_markerInfoList(int SubtaskID)
        {
            string str = "";

            switch (SubtaskID)
            {
                case 1:
                case 14:
                case 34:
                case 42:
                case 51:
                case 65:
                case 70:
                case 76:
                case 82:
                case 89:
                case 92:
                case 100:

                    {
                        str = "(SessionName='Habituation_1')";
                        break;
                    }

                case 2:
                case 15:
                case 35:
                case 43:
                case 52:
                case 60:
                case 66:
                case 71:
                case 77:
                case 83:
                case 90:
                case 93:
                case 101:
                    {
                        str = "(SessionName='Habituation_2')";
                        break;
                    }
                case 4:
                case 17:
                case 36:
                case 44:
                case 53:
                case 61:
                case 67:
                case 72:
                case 84:
                case 95:
                case 102:
                    {
                        str = "(SessionName='Initial_Touch')";
                        break;
                    }
                case 5:
                case 18:
                case 38:
                case 46:
                case 55:
                case 63:
                case 68:
                case 73:
                case 85:
                case 96:
                case 103:
                    {
                        str = "(SessionName='Must_Touch')";
                        break;
                    }
                case 16:
                case 37:
                case 45:
                case 54:
                case 62:
                case 86:
                case 94:
                
                    {
                        str = "(SessionName='Must_Initiate')";
                        break;
                    }
                case 6:
                case 19:
                case 39:
                case 47:
                case 56:
                case 64:
                case 87:
                case 97:
                    {
                        str = "((SessionName='Punish_Incorrect') OR (SessionName='Punish Incorrect II'))";
                        break;
                    }
                case 74:
                    {
                        str = "(SessionName='PRL Punish Incorrect')";
                        break;
                    }
                case 21:
                case 22:
                case 23:
                case 27:

                    {
                        str = @"((SessionName = 'Probe') or (SessionName = 'Training') or (SessionName = 'Re_Baseline') or (SessionName = 'Intra_Probe'))";
                        break;
                    }
                case 31:
                    {
                        str = @"((SessionName = 'PD-Acquisition') or (SessionName = 'Baseline') or (SessionName = 'Reversal') or (SessionName = 'Re_reversal'))";
                        break;
                    }
                case 40:
                    {
                        str = @"((Analysis_Name like '%pal analysis%') or (SessionName = 'Mouse_dpal_spal'))";
                        break;
                    }
                case 41:
                    {
                        str = "((Analysis_Name like '%pal acquisition%') or (SessionName = 'PAL-Acquisition'))";
                        break;
                    }

                case 48:
                    {
                        str = "((Analysis_Name = 'ld 1 choice rev') or (SessionName = 'LD_1_choice_reversal'))";
                        break;
                    }
                case 49:
                    {
                        str = "((Analysis_Name = 'ld 1 choice') or (SessionName = 'LD_1_choice'))";
                        break;
                    }
                case 50:
                    {
                        str = "((Analysis_Name = 'ld 2 or 3 choice') or (Analysis_Name = 'ld 2 or 3 choice single trial block') or (SessionName = 'LD_block_2_choice_EH_V2'))";
                        break;
                    }
                case 69: // PR Analysis
                    {
                        str = @"( (Analysis_Name like '%PR analysis%') OR (SessionName = 'Multiple Responding Training FR-1') OR
                                (SessionName = 'Multiple Responding Training FR-2') OR
                                (SessionName = 'Multiple Responding Training FR-3') OR
                                (SessionName = 'Multiple Responding Training FR-5') OR
                                (SessionName = 'Basic PR (PR4)') OR
                                (SessionName = 'Baseline FR-5') OR
                                (SessionName = 'High Demand PR (PR4)') OR
                                (SessionName = 'High Demand PR (PR8)') OR
                                (SessionName = 'High Demand PR (PR12)') OR
                                (SessionName = 'Uncapped FR-5') )";
                        break;
                    }
                case 75: // PRL Analysis
                    {
                        str = "( (Analysis_Name like '%PRL analysis%') OR (SessionName = 'Deterministic Reversal Training') OR (SessionName = 'Probabilistic Feedback') )";
                        break;
                    }
                case 78: // ICPT Analysis for training stages (stage 1 to satge 4)
                    {
                        str = "(  (Analysis_Name like '%ICPT Analysis%') OR (SessionName ='Stage 1 - Stimulus Touch') OR (SessionName ='Stage 2 - Target Stimulus Touch') OR (SessionName ='Stage 3 - One Target and one non-target') OR (SessionName ='Stage 4 - One Target and four non-targets') )";
                        break;
                    }
                case 79:
                    {
                        str = "( (Analysis_Name like '%ICPT SD Probe 1 Analysis%') OR (SessionName = 'Probe-1 Variable Stimulus Duration') )";
                        break;
                    }
                case 80:
                    {
                        str = "( (Analysis_Name like '%ICPT Contrast Probe Analysis%') OR (SessionName = 'Probe-2 Variable Contrast levels') )";
                        break;
                    }
                case 81:
                    {
                        str = "( (Analysis_Name like '%ICPT Distractor Probe Analysis%') OR (SessionName = 'Probe-3 Variable Flanking Distractor')  )";
                        break;
                    }
                case 88:
                    {
                        str = "( (Analysis_Name like '%VMCL Analysis%') OR (SessionName = 'VMCL Train') OR (SessionName ='VMCL Test') )";
                        break;
                    }
                case 91:
                    {
                        str = "( (Analysis_Name like '%Autoshaping Analysis%') OR (SessionName = 'Autoshape_Acquisition') OR (SessionName = 'Autoshape_Reversal') )";
                        break;
                    }
                case 98:
                    {
                        str = "(SessionName = 'Extinction_Training')";
                        break;
                    }
                case 99:
                    {
                        str = "(SessionName = 'Extinction_Probe')";
                        break;
                    }
                case 104:
                    {
                        str = "(SessionName = 'Long_Sequence')";
                        break;
                    }

            }

            return str;
        }

        #endregion

    }
}
