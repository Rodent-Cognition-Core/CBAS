using AngularSPAWebAPI.Models;
using CBAS.Helpers;
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

    public class QualityControlService
    {

        // This service checks the file against the quality control rules (Pre-processing)
        // First thing to check is to make sure each file is uploaded to the right Experiment considering the TaskName of the Experiment
        public (bool IsQcPassed, bool IsIdentifierPassed, string FileUniqueID, string ErrorMessage, string WarningMessage, bool InsertToTblUpload, int SysAnimalID, int UploadId, string AnimalID)
            QualityControl(string TaskName, string FileName, string Filepath, string ExpName, int expID, int subExpID, string AnimalAge, string SessionName)
        {

            //variable initialization
            bool IsQcPassed1 = false;
            bool IsIdentifierPassed1 = false;
            string FileUniqueID1 = "";
            string ErrorMessage1 = "";
            string WarningMessage1 = "";
            bool InsertToTblUpload1 = false;
            int SysAnimalID1 = -1;
            string Analysis_Name;
            string Schedule_Name;
            string Max_Number_Trials;
            string Max_Schedule_Time;
            string Date;
            string Animal_ID;
            string End_Summary_Condition;
            string End_Summary_No_Images;
            string End_Summary_Corrects;
            string End_Summary_Trials_Completed;
            string End_Summary_Schedule_Length;
            string PR_End_Summary_Trials_Completed;
            string Whole_Session_Analysis_Condition;
            string Whole_Session_Analysis_Trials_Completed;
            string Threshold_Condition;
            string Threshold_Trials;
            string End_Summary_Hits;


            string path = Filepath + "\\" + FileName;
            XDocument xdoc;
            using (var reader = XmlReader.Create(path))
            {
                //var xdoc = XDocument.Load(path);

                xdoc = XDocument.Load(reader);

                // *********************Extracting the Required Fields From XML Files **********************

                //******************************SessionInformation***************

                //*******Analysis Name **************
                Analysis_Name = FeatureExtraction("SessionInformation", "Information", "Analysis Name", "Value", xdoc);
                //*******Schedule Name **************
                Schedule_Name = FeatureExtraction("SessionInformation", "Information", "Schedule Name", "Value", xdoc);
                //*******Max_Number_Trials***********
                Max_Number_Trials = FeatureExtraction("SessionInformation", "Information", "Max_Number_Trials", "Value", xdoc);
                //******Max_Schedule_Time***********
                Max_Schedule_Time = FeatureExtraction("SessionInformation", "Information", "Max_Schedule_Time", "Value", xdoc);
                //*********Date/Time****************
                Date = FeatureExtraction("SessionInformation", "Information", "Date/Time", "Value", xdoc);
                //******Animal ID******************
                Animal_ID = FeatureExtraction("SessionInformation", "Information", "Animal ID", "Value", xdoc);

                //******************************MarkerData**********************

                //***********End Summary - Condition********
                End_Summary_Condition = FeatureExtraction("MarkerData", "Marker", "End Summary - Condition", "Results", xdoc);
                //**********End Summary - No. images*******
                End_Summary_No_Images = FeatureExtraction("MarkerData", "Marker", "End Summary - No. images", "Results", xdoc);
                //*********End Summary - Corrects**********
                End_Summary_Corrects = FeatureExtraction("MarkerData", "Marker", "End Summary - Corrects", "Results", xdoc);
                //*********End Summary - Trials Completed*******
                End_Summary_Trials_Completed = FeatureExtraction("MarkerData", "Marker", "End Summary - Trials Completed", "Results", xdoc);
                //*********Threshold - Condition **************
                Threshold_Condition = FeatureExtraction("MarkerData", "Marker", "Threshold - Condition", "Results", xdoc);
                //********Threshold - Trials*******************
                Threshold_Trials = FeatureExtraction("MarkerData", "Marker", "Threshold - Trials", "Results", xdoc);
                //********Progressive Ratio*******************
                End_Summary_Schedule_Length = FeatureExtraction("MarkerData", "Marker", "END SUMMARY - Schedule Length", "Results", xdoc);  // Also CPT task
                PR_End_Summary_Trials_Completed = FeatureExtraction("MarkerData", "Marker", "END SUMMARY - TRIALS COMPLETED", "Results", xdoc);
                //********Probabilistic reversal learning (PRL)*******************
                Whole_Session_Analysis_Condition = FeatureExtraction("MarkerData", "Marker", "Whole session analysis - Condition", "Results", xdoc);
                Whole_Session_Analysis_Trials_Completed = FeatureExtraction("MarkerData", "Marker", "Whole session analysis - Trials completed", "Results", xdoc);
                //********Continous performance Task (CPT)*******************
                End_Summary_Hits = FeatureExtraction("MarkerData", "Marker", "End Summary - Hits", "Results", xdoc);

            }

            // Checking Analysis_Name in the uploaded xml file to be the same as the Selected Task_Name selected in Upload Page
            switch (TaskName)
            {

                case "5_CSRTT (5C)":
                    if (!(Analysis_Name.Trim().ToLower().Contains("5c") || Analysis_Name.Trim().ToLower().Contains("5-choice") || Analysis_Name.Trim().ToLower().Contains("5-c")
                        || Analysis_Name.Trim().ToLower().Contains("touch mousetraining")))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Pairwise Visual Discrimination (PVD)":
                    if (!(Analysis_Name.Trim().ToLower().Contains("pd") || Analysis_Name.Trim().ToLower().Contains("pd") || Analysis_Name.Trim().ToLower().Contains("pairwise")))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Paired-Associates Learning (PAL)":
                    if (!Analysis_Name.Trim().ToLower().Contains("pal"))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Location Discrimination (LD)":
                    if (!Analysis_Name.Trim().ToLower().Contains("ld"))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Progressive Ratio (PR)":
                    if (!Analysis_Name.Trim().ToLower().Contains("pr"))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Probabilistic reversal learning (PRL)":
                    if (!Analysis_Name.Trim().ToLower().Contains("prl"))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Image Continuous Performance Task (iCPT)":
                    if(!Analysis_Name.Trim().ToLower().Contains("icpt"))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Visuomotor Conditional Learning (VMCL)":
                    if (!Analysis_Name.Trim().ToLower().Contains("vmcl"))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;
                case "Autoshaping":
                    if (!Analysis_Name.Trim().ToLower().Contains("autoshap"))
                    {
                        ErrorMessage1 = $@"Task name of experiment <b>{ExpName}</b> is <b>{TaskName}</b> and the uploaded file does not belong to this experiment <br/>";
                    }
                    break;


            }

            // If Xml File does have Age then this age value should be checked with the age of Subexperiment for matching purpose
            //if (!string.IsNullOrEmpty(Age))
            //{
            //    string[] splittedAge = AnimalAge.Split("-");

            //    ErrorMessage1 += (Int32.Parse(Age) >= Int32.Parse(splittedAge[0]) && Int32.Parse(Age) <= Int32.Parse(splittedAge[1])) ? "" : "Animal age in the uploaded file does not match the Animal age defined for this sub-experiment";
            //}

            // Check to make sure animalID is not NULL in xml file
            if (string.IsNullOrEmpty(Animal_ID))
            {
                ErrorMessage1 += "No Animal ID was found in the uploaded file <br/>";
            }

            // Make FileUniqueID to handle Duplication
            string Date1 = "";
            if (!string.IsNullOrEmpty(Date))
            {
                Date1 = Date.Split(" ")[0];
            }

            FileUniqueID1 = expID + "_" + Date1 + "_" + Schedule_Name + "_" + Animal_ID;
            FileUniqueID1 = FileUniqueID1.Replace(" ", "_");
            FileUniqueID1 = FileUniqueID1.Replace("-", "_");
            FileUniqueID1 = FileUniqueID1.Replace("/", "_");

            // Check FileUniqueID in DB to see if the file already exist in DB. If so retrun its uploadID, FileUniqueID, and IsQCpassed 
            var duplicateFileUniqueID = GetFileUniqueIDIsQcPassed(FileUniqueID1, expID);
            //var duplicateFileUniqueID = FileUniqueIsQcPassed.Where(x => x.FileUniqueID == FileUniqueID1).FirstOrDefault();
            int uploadId1 = (duplicateFileUniqueID != null) ? duplicateFileUniqueID.UploadID : -1;

            //Check if file already exist, its errorMessage is Null & its IsQCpassed is true then it is the duplication
            if (string.IsNullOrEmpty(ErrorMessage1))
            {

                if (duplicateFileUniqueID != null)
                {
                    if (duplicateFileUniqueID.IsQcPassed == true)
                    {
                        ErrorMessage1 += " This File already uploaded to the database";
                    }

                }


            }
            // if errorMessage was not null, insert to table uploadErrorLog and return, else go ahead with QC Rules checking
            if (!string.IsNullOrEmpty(ErrorMessage1))
            {
                // Insert to table UploadErrorLog
                UploadErrorLog uploadErrorLog = new UploadErrorLog
                {
                    ExpID = expID,
                    SubExpID = subExpID,
                    UserFileName = FileName.Split("-").Last(),
                    ErrorMessage = ErrorMessage1,
                    UploadDate = DateTime.UtcNow,

                };

                insertToUploadErrorLog(uploadErrorLog);

                return (IsQcPassed: IsQcPassed1, IsIdentifierPassed: IsIdentifierPassed1, FileUniqueID: FileUniqueID1, ErrorMessage: ErrorMessage1, WarningMessage: WarningMessage1,
                    InsertToTblUpload: InsertToTblUpload1, SysAnimalID: SysAnimalID1, UploadId: uploadId1, AnimalID: Animal_ID);
            }
            else

            {
                // set the Variable InsertToTblUpload
                InsertToTblUpload1 = true;
            }

            //***************************"Habituation 1 (5C, PAL, PD, LD, PR, PRL, iCPT, VMCL)"*************************

            if (Analysis_Name.Trim().ToLower().Contains("habit 1"))
            {

                if (!(SessionName.Trim().ToLower().Contains("habituation_1")))
                {
                    ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                }


                (bool flag, string ErrMsg) info = Check_Habit_1(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition);


                if (info.flag)
                { }

                else
                {
                    // type a message 
                    ErrorMessage1 += info.ErrMsg;
                }
            }

            //***************************"Habituation 2 (5C, PAL, PD, LD, PR, PRL, iCPT, VMCL)"*************************

            else if (Analysis_Name.Trim().ToLower().Contains("habit 2"))
            {

                if (!(SessionName.Trim().ToLower().Contains("habituation_2")))
                {
                    ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                }

                (bool flag, string ErrMsg) info = Check_Habit_2(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition);


                if (info.flag)
                { }


                else
                {
                    // type a message 
                    ErrorMessage1 += info.ErrMsg;
                }
            }

            //************************"Initial Touch" --->"Initial Train" for (5C, PAL, PD, LD, PR, PRL,VMCL)************************

            else if (Analysis_Name.Trim().ToLower().Contains("initial train"))
            {
                if (!(SessionName.Trim().ToLower().Contains("initial_touch")))
                {
                    ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                }

                (bool flag, string ErrMsg) info = Check_Initial_Touch(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_No_Images);
                
                if (info.flag)
                { }

                else
                {
                    // type a message 
                    ErrorMessage1 += info.ErrMsg;
                }
            }

            //************************"Must Touch" for (5C, PAL, PD, LD, PR, PRL, VMCL)************************

            else if (Analysis_Name.Trim().ToLower().Contains("must touch"))
            {
                if (!(SessionName.Trim().ToLower().Contains("must_touch")))
                {
                    ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                }

                (bool flag, string ErrMsg) info = Check_Must_Touch(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_Corrects);

                if (info.flag)
                { }

                else
                {
                    // type a message 
                    ErrorMessage1 += info.ErrMsg;
                }
            }

            //************************"Must Initiate" for (5C, PAL, PD, LD, VMCL)************************

            else if (Analysis_Name.Trim().ToLower().Contains("must initiate"))
            {
                if (!(SessionName.Trim().ToLower().Contains("must_initiate")))
                {
                    ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                }

                (bool flag, string ErrMsg) info = Check_Must_Touch(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_Corrects);

                if (info.flag)
                { }

                else
                {
                    // type a message 
                    ErrorMessage1 += info.ErrMsg;
                }
            }

            //**********************Punish Incorrect for (5C, PAL, PD, LD, PRL, VMCL)****************

            else if (Analysis_Name.Trim().ToLower().Contains("punish"))
            {

                if (!(SessionName.Trim().ToLower().Contains("punish_incorrect")))
                {
                    ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                }

                (bool flag, string ErrMsg) info = Check_Punish_Incorrect(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_Trials_Completed);

                if (info.flag)
                { }


                else
                {
                    // type a message 
                    ErrorMessage1 += info.ErrMsg;
                }
            }

            ///*************End of Pretraining Sessions**************
            ///*************Testing sessions for all Existing Tasks is MB

            else
            {

                switch (TaskName)
                {

                    case "5_CSRTT (5C)":
                        if (Analysis_Name.Trim().ToLower().Contains("5c"))
                        {
                            if (!(SessionName.Trim().ToLower().Contains("training") || SessionName.Trim().ToLower().Contains("probe") || SessionName.Trim().ToLower().Contains("re_baseline")
                             || SessionName.Trim().ToLower().Contains("intra_probe")))
                            {
                                ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                            }

                            (bool flag, string ErrMsg) info = Check_Mouse5C_Touch_Var1(Max_Number_Trials, Max_Schedule_Time, Threshold_Condition, Threshold_Trials);


                            if (info.flag)
                            { }

                            else
                            {
                                // type a message 
                                ErrorMessage1 += info.ErrMsg;
                            }
                        }

                        break;

                    case "Pairwise Visual Discrimination (PVD)":

                        if (Analysis_Name.Trim().ToLower().Contains("analysis"))
                        {
                            if (!(SessionName.Trim().ToLower().Contains("acquisition") || SessionName.Trim().ToLower().Contains("baseline") || SessionName.Trim().ToLower().Contains("reversal")
                             || SessionName.Trim().ToLower().Contains("re_reversal")))
                            {
                                ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                            }

                            (bool flag, string ErrMsg) info = Check_PD(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_Trials_Completed);


                            if (info.flag)
                            { }

                            else
                            {
                                // type a message 
                                ErrorMessage1 += info.ErrMsg;
                            }
                        }

                        break;

                    case "Paired-Associates Learning (PAL)":

                        if (Analysis_Name.Trim().ToLower().Contains("pal analysis") || Analysis_Name.Trim().ToLower().Contains("pal aquisition"))  // pal analysis or pal acquisition ???
                        {
                            switch (Analysis_Name.Trim().ToLower())
                            {
                                case "pal analysis":
                                    if (!(Analysis_Name.Trim().ToLower().Contains("pal analysis") && (SessionName.Trim().ToLower().Contains("dpal") || SessionName.Trim().ToLower().Contains("spal"))))
                                    {
                                        ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                                    }
                                    break;
                                case "pal aquisition":

                                    if (!(Analysis_Name.Trim().ToLower().Contains("pal aquisition") && (SessionName.Trim().ToLower().Contains("acquisition"))))
                                    {
                                        ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                                    }
                                    break;
                            }
                            (bool flag, string ErrMsg) info = Check_PAL_Analysis_Acquisition(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_Trials_Completed);


                            if (info.flag)
                            { }

                            else
                            {
                                // type a message 
                                ErrorMessage1 += info.ErrMsg;
                            }
                        }

                        break;

                    case "Location Discrimination (LD)":

                        if (Analysis_Name.Trim().ToLower().Contains("ld"))
                        {
                            switch (Analysis_Name.Trim().ToLower())
                            {
                                case "ld 1 choice":
                                case "ld 2 or 3 choice":
                                    if (!((SessionName.Trim().ToLower().Contains("ld_1_choice_training")) || (SessionName.Trim().ToLower().Contains("LD_1_Choice_Probe_Easy")) || (SessionName.Trim().ToLower().Contains("LD_1_Choice_Probe_Hard"))))
                                    {
                                        ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>.<br/>";
                                    }
                                    break;
                                case "ld 1 choice rev":
                                    if (!((SessionName.Trim().ToLower().Contains("ld_1_choice_reversal_training") || SessionName.Trim().ToLower().Contains("ld_1_choice_reversal_probe_easy") || SessionName.Trim().ToLower().Contains("ld_1_choice_reversal_probe_hard"))))
                                    {
                                        ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                                    }
                                    break;

                            }

                            (bool flag, string ErrMsg) info = Check_LD1_Choice(Max_Number_Trials, Max_Schedule_Time, End_Summary_Trials_Completed, End_Summary_Condition);

                            if (info.flag)
                            { }

                            else
                            {
                                // type a message 
                                ErrorMessage1 += info.ErrMsg;
                            }
                        }

                        break;
                    case "Progressive Ratio (PR)":
                        if (Analysis_Name.Trim().ToLower().Contains("pr"))
                        {
                            if (!(SessionName.Trim().ToLower().Contains("multiple responding training fr-1") ||
                            SessionName.Trim().ToLower().Contains("multiple responding training fr-1") ||
                            SessionName.Trim().ToLower().Contains("multiple responding training fr-2") ||
                            SessionName.Trim().ToLower().Contains("multiple responding training fr-3") ||
                            SessionName.Trim().ToLower().Contains("multiple responding training fr-5") ||
                            SessionName.Trim().ToLower().Contains("basic pr (pr4)") ||
                            SessionName.Trim().ToLower().Contains("baseline fr-5") ||
                            SessionName.Trim().ToLower().Contains("high demand pr (pr4)") ||
                            SessionName.Trim().ToLower().Contains("high demand pr (pr8)") ||
                            SessionName.Trim().ToLower().Contains("high demand pr (pr12)") ||
                            SessionName.Trim().ToLower().Contains("uncapped fr-5")
                            ))
                            {
                                ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                            }

                            (bool flag, string ErrMsg) info = Check_PR(Max_Number_Trials, Max_Schedule_Time, End_Summary_Schedule_Length, PR_End_Summary_Trials_Completed, SessionName);


                            if (info.flag)
                            { }

                            else
                            {
                                // type a message 
                                ErrorMessage1 += info.ErrMsg;
                            }


                        }

                        break;
                    case "Probabilistic reversal learning (PRL)":
                        if (Analysis_Name.Trim().ToLower().Contains("prl"))
                        {
                            if (!(SessionName.Trim().ToLower().Contains("deterministic reversal training") ||
                            SessionName.Trim().ToLower().Contains("probabilistic feedback")
                            ))
                            {
                                ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                            }

                            (bool flag, string ErrMsg) info = Check_PD(Max_Number_Trials, Max_Schedule_Time, Whole_Session_Analysis_Condition, Whole_Session_Analysis_Trials_Completed);


                            if (info.flag)
                            { }

                            else
                            {
                                // type a message 
                                ErrorMessage1 += info.ErrMsg;
                            }

                        }

                        break;
                    case "Image Continuous Performance Task (iCPT)":
                        // add iCPT QC codes
                        if (Analysis_Name.Trim().ToLower().Contains("icpt") || Analysis_Name.Trim().ToLower().Contains("cpt"))
                        {
                            if (!(SessionName.Trim().ToLower().Contains("stage 1 - stimulus touch") || SessionName.Trim().ToLower().Contains("stage 2 - target stimulus touch") ||
                                  SessionName.Trim().ToLower().Contains("stage 3 - one target and one non-target") || SessionName.Trim().ToLower().Contains("stage 4 - one target and four non-targets") ||
                                  SessionName.Trim().ToLower().Contains("probe-1 variable stimulus duration") || SessionName.Trim().ToLower().Contains("probe-2 variable contrast levels") ||
                                  SessionName.Trim().ToLower().Contains("probe-3 variable flanking distractor") ))
                            {
                                ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                            }
                            // We can use check_PD function for cpt but sending the parameters realted to CPT. The logic is the same.
                            //(bool flag, string ErrMsg) info = Check_PD(Max_Number_Trials, Max_Schedule_Time, End_Summary_Schedule_Length, End_Summary_Hits);

                            //if (info.flag)
                            //{ }

                            //else
                            //{
                            //    // type a message 
                            //    ErrorMessage1 += info.ErrMsg;
                            //}
                            
                        }
                            break;
                    case "Visuomotor Conditional Learning (VMCL)":
                        if (Analysis_Name.Trim().ToLower().Contains("vmcl"))
                        {
                            if (!(SessionName.Trim().ToLower().Contains("vmcl train") || SessionName.Trim().ToLower().Contains("vmcl test")))
                            {
                                ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                            }

                            (bool flag, string ErrMsg) info = Check_VMCL(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_Trials_Completed);

                            if (info.flag)
                            { }

                            else
                            {
                                // type a message 
                                ErrorMessage1 += info.ErrMsg;
                            }


                        }
                            break;

                    case "Autoshaping":
                        if (Analysis_Name.Trim().ToLower().Contains("autoshap"))
                        {
                            if (!(SessionName.Trim().ToLower().Contains("autoshape_acquisition") || SessionName.Trim().ToLower().Contains("autoshape_reversal")))
                            {
                                ErrorMessage1 += $"Analysis Name does not match with Schedule Name or Session Name. Analysis name is <b>{Analysis_Name}</b> and Session name is <b>{SessionName}</b>. <br/>";
                            }

                            // End_Summary_Trials_Completed does not exist in Autoshape anaylysis file, so we skip this rule now
                            //(bool flag, string ErrMsg) info = Check_PD(Max_Number_Trials, Max_Schedule_Time, End_Summary_Condition, End_Summary_Trials_Completed);

                            //if (info.flag)
                            //{ }

                            //else
                            //{
                            //    // type a message 
                            //    ErrorMessage1 += info.ErrMsg;
                            //}


                        }
                        break;

                }  // end of switch-case for TaskName
            } // End of else
              //****************************************************************************************************************************
              //****************************************************************************************************************************

            // set the value of IsQcPassed based on ErrorMessage    
            IsQcPassed1 = (string.IsNullOrEmpty(ErrorMessage1)) ? true : false;

            // Check if Animal ID of input file exist in table animal in Database
            List<Animal> lstAnimal = GetAnimalByUserAnimalID(Animal_ID, expID);

            // if animal exist in DB, use the existing animal information
            if (lstAnimal.Any())
            {
                //set the value of IsIdentifierPassed if IsQCPassed

                // new: loop through lstAnimal and check if any animalinfo of each element in the animal list is not null  (operator and i && i+1)
                IsIdentifierPassed1 = (!string.IsNullOrEmpty(lstAnimal[0].Sex) && lstAnimal[0].GID != null && lstAnimal[0].SID != null) ? true : false;


                if (IsIdentifierPassed1 == false)
                {
                    ErrorMessage1 = $"Missing Animal Information: No values have been provided for any of the following fields:" +
                                  $"<ul> <li>Sex</li> <li>Strain</li> <li>Genotype</li></ul>";
                }

                // Set the Value for Sys Animal ID if Animal already exist in DB
                SysAnimalID1 = lstAnimal[0].AnimalID;

            }
            else // Animal does not exist in DB
            {

                if (IsQcPassed1 == true)
                {
                    ErrorMessage1 = $"Missing Animal Information: No values have been provided for any of the following fields:" +
                                 $"<ul> <li>Sex</li> <li>Strain</li> <li>Genotype</li></ul>";

                }

                //Create an object from Animal Model and assign values to its attributes based on info in the uploaded xml file
                Animal animal = new Animal
                {
                    ExpID = expID,
                    UserAnimalID = Animal_ID,
                    Sex = null,
                    Genotype = null,
                    Strain = null,
                    GID = null,
                    SID = null,

                };

                SysAnimalID1 = InsertToAnimalTbl(animal);


            }

            return (IsQcPassed: IsQcPassed1, IsIdentifierPassed: IsIdentifierPassed1, FileUniqueID: FileUniqueID1, ErrorMessage: ErrorMessage1, WarningMessage: WarningMessage1,
                 InsertToTblUpload: InsertToTblUpload1, SysAnimalID: SysAnimalID1, UploadId: uploadId1, AnimalID: Animal_ID);


        }

        // *************End of QualityControl Function****************************************************************************************************************************
        //************************************************************************************************************************************************************************

        // Function Definition for extracting the required fields from XML Files ******************
        private string FeatureExtraction(string Tag1, string Tag2, string TagName, string TagValue, XDocument xdoc1)
        {

            string output = "";
            string xpath = "/LiEvent/" + Tag1 + "/" + Tag2 + "[Name='" + TagName + "']/" + TagValue;

            var value = xdoc1.XPathSelectElement(xpath);
            if (value != null)
            {
                output = (string)value;
            }

            return output;

        }

        // Function definition to check if a feature exist in xml file
        //public bool XMLFeatureExist(string Tag1, string Tag2, string TagName, XDocument xdoc1)
        //{
        //    bool flag = false;

        //    flag = xdoc1.Elements("LiEvent").Elements(Tag1).Elements(Tag2).Elements("Name").ToList().Exists(x => x.Value.ToLower() == TagName.ToLower());

        //    return flag;

        //}

        // Function definition for handleing 1 seconds delay to stop executing
        private bool IsNumberAcceptable(int exactNumber, string Machine_Var)
        {
            bool flag = false;
            if ((float.Parse(HandleNullStr(Machine_Var)) > exactNumber - 1 && float.Parse(HandleNullStr(Machine_Var)) < exactNumber + 1))
            {
                flag = true;
            }


            return flag;
        }

        // Function DEfinition for checking conditions of Habituation 1 for 5C, PAL, and PD
        private (bool flag, string ErrMsg) Check_Habit_1(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition)
        {
            bool Flag = false;
            string ErrMsg1 = "";
            if ((Int32.Parse(HandleNullStr(Max_Number_Trials)) == 0 || Max_Number_Trials == ""))

            {
                Flag = true;
            }
            else
            {
                { ErrMsg1 += $"Max_Number_Trials should be empty or 0, but this value is equal to <b> { Int32.Parse(HandleNullStr(Max_Number_Trials))}  </b> in the uploaded file <br />"; }


            }

            return (flag: Flag, ErrMsg: ErrMsg1);
        }

        // Function DEfinition for checking conditions of Habituation 2 for 5C, PAL, and PD
        private (bool flag, string ErrMsg) Check_Habit_2(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition)
        {
            bool Flag = false;
            string ErrMsg1 = "";
            if ((Int32.Parse(HandleNullStr(Max_Number_Trials)) == 0 || Max_Number_Trials == ""))

            {
                Flag = true;
            }
            else
            {
                { ErrMsg1 += $"Max_Number_Trials should be empty or 0, but this value is equal to <b>  { Int32.Parse(HandleNullStr(Max_Number_Trials))}  </b> in the uploaded file. <br />"; }


            }


            return (flag: Flag, ErrMsg: ErrMsg1);
        }

        // Function Definition for checking conditions of Initial Touch for 5C, PAL, PD and LD, PR, PRL
        private (bool flag, string ErrMsg) Check_Initial_Touch(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition, string End_Summary_No_Images)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }


            if (
                (float.Parse(HandleNullStr(End_Summary_Condition)) > 0) && (float.Parse(HandleNullStr(End_Summary_No_Images)) > 0)
                && (
                           (float.Parse(HandleNullStr(End_Summary_No_Images)) < Int32.Parse(HandleNullStr(Max_Number_Trials)) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                        || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_No_Images)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))
                    )
                )
            {
                Flag = true;
            }
            else
            {


                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Condition))} </b> in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(End_Summary_No_Images)) <= 0)
                { ErrMsg1 += $@"End_Summary_No_Images should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_No_Images))} </b> in the uploaded file. <br />"; }

                if (!(((float.Parse(HandleNullStr(End_Summary_No_Images)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_No_Images)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                {
                    ErrMsg1 += $@"End_Summary_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_No_Images is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  End_Summary_No_Images has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>End_Summary_Condition = <b>{float.Parse(HandleNullStr(End_Summary_Condition))}</b></li >
                  <li> End_Summary_No_Images = <b>{float.Parse(HandleNullStr(End_Summary_No_Images))}</b></li ></ul><br />";
                }



            }


            return (flag: Flag, ErrMsg: ErrMsg1);
        }

        // Function DEfinition for checking conditions of Must Touch for 5C, PAL, and PD
        private (bool flag, string ErrMsg) Check_Must_Touch(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition, string End_Summary_Corrects)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if (
                (float.Parse(HandleNullStr(End_Summary_Condition)) > 0)
                && (float.Parse(HandleNullStr(End_Summary_Corrects)) > 0)
                && (((float.Parse(HandleNullStr(End_Summary_Corrects)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Corrects)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                )
            {
                Flag = true;
            }
            else
            {

                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to  <b>  {float.Parse(HandleNullStr(End_Summary_Condition)) }  </b>   in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(End_Summary_Corrects)) <= 0)
                { ErrMsg1 += $@"End_Summary_Corrects should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Corrects))} </b> in the uploaded file. <br />"; }

                if (!(((float.Parse(HandleNullStr(End_Summary_Corrects)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Corrects)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                {
                    ErrMsg1 += $@"End_Summary_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_Corrects is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  End_Summary_Corrects has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>End_Summary_Condition = <b>{float.Parse(HandleNullStr(End_Summary_Condition))}</b></li >
                  <li> End_Summary_Corrects = <b>{float.Parse(HandleNullStr(End_Summary_Corrects))}</b></li ></ul><br />";

                }

            }


            return (flag: Flag, ErrMsg: ErrMsg1);
        }

        // Function DEfinition for checking conditions of Must Initiate for 5C, PAL, and PD
        private (bool flag, string ErrMsg) Check_Must_Initiate(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition, string End_Summary_Corrects)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if (
                (float.Parse(HandleNullStr(End_Summary_Condition)) > 0)
                && (float.Parse(HandleNullStr(End_Summary_Corrects)) > 0)
                && (((float.Parse(HandleNullStr(End_Summary_Corrects)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Corrects)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                )
            {
                Flag = true;
            }
            else
            {

                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to  <b>  {float.Parse(HandleNullStr(End_Summary_Condition)) }  </b>   in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(End_Summary_Corrects)) <= 0)
                { ErrMsg1 += $@"End_Summary_Corrects should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Corrects))} </b> in the uploaded file. <br />"; }

                if (!(((float.Parse(HandleNullStr(End_Summary_Corrects)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Corrects)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                {
                    ErrMsg1 += $@"End_Summary_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_Corrects is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  End_Summary_Corrects has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>End_Summary_Condition = <b>{float.Parse(HandleNullStr(End_Summary_Condition))}</b></li >
                  <li> End_Summary_Corrects = <b>{float.Parse(HandleNullStr(End_Summary_Corrects))}</b></li ></ul><br />";

                }


            }


            return (flag: Flag, ErrMsg: ErrMsg1);
        }

        // Function DEfinition for checking conditions of Punish Incorrect for 5C, PAL, and PD
        private (bool flag, string ErrMsg) Check_Punish_Incorrect(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition, string End_Summary_Trials_Completed)
        {
            bool Flag = false;
            string ErrMsg1 = "";


            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if (
                (float.Parse(HandleNullStr(End_Summary_Condition)) > 0)
                && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) > 0)
                && (((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                )
            {
                Flag = true;
            }

            else

            {

                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Condition))} </b> in the uploaded file.<br />"; }

                if (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) <= 0)
                { ErrMsg1 += $@"End_Summary_Trials_Completed should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Trials_Completed))} </b> in the uploaded file.<br />"; }

                if (!(((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) <= Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                {
                    ErrMsg1 += $@"End_Summary_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_Trials_Completed is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  End_Summary_Trials_Completed has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>End_Summary_Condition = <b>{End_Summary_Condition}</b></li >
                  <li> End_Summary_Trials_Completed = <b>{End_Summary_Trials_Completed}</b></li ></ul><br />";
                }
            }

            return (flag: Flag, ErrMsg: ErrMsg1);

        }

        // Function DEfinition for checking conditions of PAL training sessions
        private (bool flag, string ErrMsg) Check_PAL_Analysis_Acquisition(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition, string End_Summary_Trials_Completed)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file.  <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if (
                (float.Parse(HandleNullStr(End_Summary_Condition)) > 0)
                && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) > 0)
                && (((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                )
            {
                Flag = true;
            }

            else

            {

                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Condition))} </b> in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) <= 0)
                { ErrMsg1 += $@"End_Summary_Trials_Completed should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Trials_Completed))} </b> in the uploaded file. <br />"; }

                if (!(((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                {
                    ErrMsg1 += $@"End_Summary_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_Trials_Completed is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  End_Summary_Trials_Completed has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>End_Summary_Condition = <b>{End_Summary_Condition}</b></li >
                  <li> End_Summary_Trials_Completed = <b>{End_Summary_Trials_Completed}</b></li ></ul><br />";

                }
            }

            return (flag: Flag, ErrMsg: ErrMsg1);

        }

        // Function DEfinition for checking conditions of Sub-Analysis whcih are Specific to PD and PRL tasks
        private (bool flag, string ErrMsg) Check_PD(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition, string End_Summary_Trials_Completed)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if (
               (float.Parse(HandleNullStr(End_Summary_Condition)) > 0)
                && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) > 0)
                && (((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                )
            {
                Flag = true;
            }

            else

            {

                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Condition))} </b> in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) <= 0)
                { ErrMsg1 += $@"End_Summary_Trials_Completed should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Trials_Completed))} </b> in the uploaded file. <br />"; }

                if (!(((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                {
                    ErrMsg1 += $@"End_Summary_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_Trials_Completed is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  End_Summary_Trials_Completed has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>End_Summary_Condition = <b>{End_Summary_Condition}</b></li >
                  <li> End_Summary_Trials_Completed = <b>{End_Summary_Trials_Completed}</b></li ></ul><br />";
                }
            }


            return (flag: Flag, ErrMsg: ErrMsg1);

        }

        // Function Definition for checking conditions of Sub_Analysis which are Specific to 5C or Cam 5C
        private (bool flag, string ErrMsg) Check_Mouse5C_Touch_Var1(string Max_Number_Trials, string Max_Schedule_Time, string Threshold_Condition, string Threshold_Trials)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if (
                (float.Parse(HandleNullStr(Threshold_Condition)) > 0)
                && (float.Parse(HandleNullStr(Threshold_Trials)) > 0)
                && (((float.Parse(HandleNullStr(Threshold_Trials)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), Threshold_Condition)) ||
                ((float.Parse(HandleNullStr(Threshold_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(Threshold_Trials)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                )
            {
                Flag = true;
            }

            else
            {

                if (float.Parse(HandleNullStr(Threshold_Condition)) <= 0)
                { ErrMsg1 += $@"Threshold_Condition should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(Threshold_Condition))} </b> in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(Threshold_Trials)) <= 0)
                { ErrMsg1 += $@"Threshold_Trials should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(Threshold_Trials))} </b> in the uploaded file. <br />"; }

                if (!((((float.Parse(HandleNullStr(Threshold_Trials)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), Threshold_Condition)) ||
                ((float.Parse(HandleNullStr(Threshold_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(Threshold_Trials)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))))
                {
                    ErrMsg1 += $@"Threshold_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the Threshold_Trials is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  Threshold_Trials has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the Threshold_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>Threshold_Condition = <b>{Threshold_Condition}</b></li >
                  <li> Threshold_Trials = <b>{Threshold_Trials}</b></li ></ul><br />";
                }
            }

            return (flag: Flag, ErrMsg: ErrMsg1);
        }

        // Function Definition for checking the condtions of Sub-Analysis specific to "LD 1 choice" and "LD 1 choice reversal"
        private (bool flag, string ErrMsg) Check_LD1_Choice(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Trials_Completed, string End_Summary_Condition)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                {
                    ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                }

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if (
                (float.Parse(HandleNullStr(End_Summary_Condition)) > 0)
                && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) > 0)
                && (((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
            {
                Flag = true;
            }


            if (Flag != true)
            {

                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Condition))} </b> in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) <= 0)
                { ErrMsg1 += $@"End_Summary_Trials_Completed should be greater than 0, but this value is equal to <b> { float.Parse(HandleNullStr(End_Summary_Trials_Completed))} </b> in the uploaded file. <br />"; }

                if (!(((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Condition)))
                || ((float.Parse(HandleNullStr(End_Summary_Condition)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                {
                    ErrMsg1 += $@"End_Summary_Condition has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_Trials_Completed is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
                  End_Summary_Trials_Completed has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Condition is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
                  but such values are set as follows in the uploaded file: <br />
                  <ul><li>End_Summary_Condition = <b>{End_Summary_Condition}</b></li >
                  <li> End_Summary_Trials_Completed = <b>{End_Summary_Trials_Completed}</b></li ></ul><br />";
                }
            }

            return (flag: Flag, ErrMsg: ErrMsg1);

        }

        // Function definition for checking conditions of sessions belong to PR Task
        private (bool flag, string ErrMsg) Check_PR(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Schedule_Length, string End_Summary_Trials_Completed, string SessionName)
        {
            bool Flag = false;
            string ErrMsg1 = "";
            switch (SessionName)
            {
                case "Multiple Responding Training FR-1":
                case "Multiple Responding Training FR-2":
                case "Multiple Responding Training FR-3":
                case "Multiple Responding Training FR-5":
                case "Baseline FR-5":
                    // write code here

                    if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 && Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
                    {
                        Flag = true;
                    }
                    else
                    {
                        if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                        {
                            ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                        }

                        if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                        {
                            ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                        }

                        return (flag: Flag, ErrMsg: ErrMsg1);
                    }

                    if (
                       (float.Parse(HandleNullStr(End_Summary_Schedule_Length)) > 0)
                        && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) > 0)
                        && (((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Schedule_Length)))
                        || ((float.Parse(HandleNullStr(End_Summary_Schedule_Length)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                        )
                    {
                        Flag = true;
                    }

                    else

                    {

                        if (float.Parse(HandleNullStr(End_Summary_Schedule_Length)) <= 0)
                        { ErrMsg1 += $@"End_Summary_Schedule_Length should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Schedule_Length))} </b> in the uploaded file. <br />"; }

                        if (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) <= 0)
                        { ErrMsg1 += $@"End_Summary_Trials_Completed should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Trials_Completed))} </b> in the uploaded file. <br />"; }

                        if (!(((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Schedule_Length)))
                        || ((float.Parse(HandleNullStr(End_Summary_Schedule_Length)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials))))))
                        {
                            ErrMsg1 += $@"End_Summary_Schedule_Length has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> if the End_Summary_Trials_Completed is less than <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> OR
							  End_Summary_Trials_Completed has to be equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> if the End_Summary_Schedule_Length is less than <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b>,
							  but such values are set as follows in the uploaded file: <br />
							  <ul><li>End_Summary_Schedule_Length = <b>{End_Summary_Schedule_Length}</b></li >
							  <li> End_Summary_Trials_Completed = <b>{End_Summary_Trials_Completed}</b></li ></ul><br />";
                        }
                    }


                    return (flag: Flag, ErrMsg: ErrMsg1);
                    


                case "Basic PR (PR4)":
                case "High Demand PR (PR8)":
                case "High Demand PR (PR12)":

                    // write code here

                    if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
                    {
                        Flag = true;
                    }
                    else
                    {
                        if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                        {
                            ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file. <br />";
                        }

                        return (flag: Flag, ErrMsg: ErrMsg1);
                    }

                    if (
                       (float.Parse(HandleNullStr(End_Summary_Schedule_Length)) > 0)
                        && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) > 0)
                        && (((float.Parse(HandleNullStr(End_Summary_Trials_Completed)) < Int32.Parse(HandleNullStr(Max_Number_Trials))) && (IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Schedule_Length)))
                        || ((float.Parse(HandleNullStr(End_Summary_Schedule_Length)) < Int32.Parse(HandleNullStr(Max_Schedule_Time))) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) == Int32.Parse(HandleNullStr(Max_Number_Trials)))))
                        )
                    {
                        Flag = true;
                    }

                    else

                    {

                        if (float.Parse(HandleNullStr(End_Summary_Schedule_Length)) <= 0)
                        { ErrMsg1 += $@"End_Summary_Schedule_Length should be greater than 0, but this value is equal to <b> c </b> in the uploaded file. <br />"; }

                        if (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) <= 0)
                        { ErrMsg1 += $@"End_Summary_Trials_Completed should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Trials_Completed))} </b> in the uploaded file. <br />"; }
                    }

                    return (flag: Flag, ErrMsg: ErrMsg1);

                case "Uncapped FR-5":

                    if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) > 0)
                    {
                        Flag = true;
                    }
                    else
                    {
                        if (Int32.Parse(HandleNullStr(Max_Schedule_Time)) <= 0)
                        {
                            ErrMsg1 += $@"Max_Schedule_Time should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Schedule_Time))}</b> in the uploaded file.";
                        }
                    }

                    if ((IsNumberAcceptable(Int32.Parse(HandleNullStr(Max_Schedule_Time)), End_Summary_Schedule_Length)))
                    {
                        Flag = true;
                    }
                    else
                    {
                        ErrMsg1 += $@"End_Summary_Schedule_Length should be equal to Max_Schedule_Time, but this value is <b>{float.Parse(HandleNullStr(End_Summary_Trials_Completed))}</b> in the uploaded file and Max_Schedule_Time is equalt to {Int32.Parse(HandleNullStr(Max_Schedule_Time))}";
                    }

                    return (flag: Flag, ErrMsg: ErrMsg1);


                default:
                    return (flag: Flag, ErrMsg: ErrMsg1);

            }

                  
        } // end of PR function

        // Function definition for checking conditions of sessions belong to VMCL Task
        private (bool flag, string ErrMsg) Check_VMCL(string Max_Number_Trials, string Max_Schedule_Time, string End_Summary_Condition, string End_Summary_Trials_Completed)
        {
            bool Flag = false;
            string ErrMsg1 = "";

            if (Int32.Parse(HandleNullStr(Max_Number_Trials)) > 0 )
            {
                Flag = true;
            }
            else
            {
                if (Int32.Parse(HandleNullStr(Max_Number_Trials)) <= 0)
                {
                    ErrMsg1 += $@"Max_Number_Trials should be greater than 0, but this value is equal to <b>{Int32.Parse(HandleNullStr(Max_Number_Trials))}</b> in the uploaded file. <br />";
                }

                

                return (flag: Flag, ErrMsg: ErrMsg1);
            }

            if ((float.Parse(HandleNullStr(End_Summary_Condition)) > 0) && (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) > 0))

            {
                Flag = true;
            }

            else

            {

                if (float.Parse(HandleNullStr(End_Summary_Condition)) <= 0)
                { ErrMsg1 += $@"End_Summary_Condition should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Condition))} </b> in the uploaded file. <br />"; }

                if (float.Parse(HandleNullStr(End_Summary_Trials_Completed)) <= 0)
                { ErrMsg1 += $@"End_Summary_Trials_Completed should be greater than 0, but this value is equal to <b> {float.Parse(HandleNullStr(End_Summary_Trials_Completed))} </b> in the uploaded file. <br />"; }


            }


            return (flag: Flag, ErrMsg: ErrMsg1);

        }

        public string HandleNullStr(string varName1)
        {
            if (string.IsNullOrEmpty(varName1))
            {
                return "0";
            }
            return varName1;
        }


        //*******************************************************************************************************************************************************************************
        //************************************************************Extract Info From DataBase*****************************************************************************************


        // ************************Check if Animal ID of input file exist in tbl Animal in Database
        public List<Animal> GetAnimalByUserAnimalID(string Animal_ID, int expID)
        {
            List<Animal> lstAnimal = new List<Animal>();

            using (DataTable dt = Dal.GetDataTable($@"SELECT Animal.* From Animal
                                                         Where UserAnimalID = '{Animal_ID}' and ExpID = {expID}"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstAnimal.Add(new Animal
                    {

                        ExpID = Int32.Parse(dr["ExpID"].ToString()),
                        AnimalID = Int32.Parse(dr["AnimalID"].ToString()),
                        UserAnimalID = Convert.ToString(dr["UserAnimalID"].ToString()),
                        SID = HelperService.ConvertToNullableInt(dr["SID"].ToString()),
                        GID = HelperService.ConvertToNullableInt(dr["GID"].ToString()),
                        Sex = Convert.ToString(dr["Sex"].ToString()),


                    });
                }

            }

            return lstAnimal;

        }

        //***************************Insert to Table UploadErrorLog**************************
        public void insertToUploadErrorLog(UploadErrorLog errorLog)
        {
            string sql = $"insert into UploadErrorLog (ExpID, SubExpID, UserFileName, ErrorMessage, UploadDate) " +
            $"Values({errorLog.ExpID}, {errorLog.SubExpID}, '{errorLog.UserFileName}', '{errorLog.ErrorMessage}', '{errorLog.UploadDate}'); SELECT @@IDENTITY AS 'Identity'; ";

            Int32.Parse(Dal.ExecScalar(sql).ToString());
        }

        //*******************Check for FileUniqueID************************************
        public FileUploadResult GetFileUniqueIDIsQcPassed(string FileUniqueID1, int expID)
        {
            FileUploadResult FileUniqueIDIsQcPassed;

            using (DataTable dt = Dal.GetDataTable($@"select UploadID, ltrim(rtrim(FileUniqueID)) As FileUniqueID, IsQcPassed, IsIdentifierPassed from Upload
                                                        where ltrim(rtrim([FileUniqueID])) = '{FileUniqueID1.Trim()}' and  ExpID = {expID};"))
            {
                if (dt.Rows.Count == 0)
                {
                    return null;
                }

                FileUniqueIDIsQcPassed = new FileUploadResult
                {
                    FileUniqueID = Convert.ToString(dt.Rows[0]["FileUniqueID"].ToString()),
                    IsQcPassed = Convert.ToBoolean(dt.Rows[0]["IsQcPassed"].ToString()),
                    UploadID = Int32.Parse(dt.Rows[0]["UploadID"].ToString()),
                    //IsIdentifierPassed = Convert.ToBoolean(dr["IsIdentifierPassed"].ToString()),
                };

            }

            return FileUniqueIDIsQcPassed;

        }

        // ********************************Insert To Table Animal in DataBase****************************
        public int InsertToAnimalTbl(Animal animal)
        {
            string SID = animal.SID == null ? "null" : "";
            string GID = animal.GID == null ? "null" : "";

            string sql = $"insert into Animal (ExpID, UserAnimalID, SID, GID, Sex)" +
            $"Values('{animal.ExpID}', '{animal.UserAnimalID}', {SID}, {GID}, '{animal.Sex}'); SELECT @@IDENTITY AS 'Identity'; ";

            return Int32.Parse(Dal.ExecScalar(sql).ToString());

        }

    }


}
