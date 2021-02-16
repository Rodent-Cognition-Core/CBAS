using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{

    public class ExperimentService
    {
        public List<Experiment> GetAllExperiments(string userID)
        {
            List<Experiment> lstExperiment = new List<Experiment>();

            using (DataTable dt = Dal.GetDataTable($@"select Experiment.*, task.name as TaskName, Species.Species as Species, tt2.PISiteName, 

                                                        STUFF((SELECT ' <br/>' + se.SubExpName
                                                                FROM SubExperiment se
                                                                Where se.ExpID = Experiment.ExpID
                                                                order by se.SubExpName
                                                                FOR XML PATH(''), type
                                                        ).value('.', 'nvarchar(max)'),1,6,'') As SubExpNames

                                                        from Experiment 
                                                        inner join task on task.id = Experiment.taskID
                                                        inner join species on species.id = Experiment.SpeciesID
                                                        inner join 

                                                        (Select PUSID, PIUserSite.PSID, tt.PISiteName  From PIUserSite
                                                        inner join 
                                                        (Select PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) as PISiteName From PISite
                                                        inner join PI on PI.PID = PISite.PID 
                                                        inner join Site on Site.SiteID = PISite.SiteID) as tt on tt.PSID = PIUserSite.PSID) as tt2 on tt2.PUSID = Experiment.PUSID

                                                        WHERE Experiment.UserID = '{userID}'"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                   

                    lstExperiment.Add(new Experiment
                    {
                        ExpID = Int32.Parse(dr["ExpID"].ToString()),
                        PUSID = Int32.Parse(dr["PUSID"].ToString()),
                        ExpName = Convert.ToString(dr["ExpName"].ToString()),
                        SubExperimentNames = Convert.ToString(dr["SubExpNames"].ToString()),
                        StartExpDate = Convert.ToDateTime(dr["StartExpDate"].ToString()),
                        EndExpDate = Convert.ToDateTime(dr["EndExpDate"].ToString()),
                        TaskName = Convert.ToString(dr["TaskName"].ToString()),
                        TaskDescription = Convert.ToString(dr["TaskDescription"].ToString()),
                        //ErrorMessage = Convert.ToString(dr["ErrorMessage"].ToString()),
                        TaskID = Int32.Parse(dr["TaskID"].ToString()),
                        DOI = Convert.ToString(dr["DOI"].ToString()),
                        //IsPostProcessingPass = bool.Parse(dr["IsPostProcessingPass"].ToString()),
                        PISiteName = Convert.ToString(dr["PISiteName"].ToString()),
                        Status = Convert.ToBoolean(dr["Status"]),
                        SpeciesID = Int32.Parse(dr["SpeciesID"].ToString()),
                        Species = Convert.ToString(dr["Species"].ToString()),
                        TaskBattery = Convert.ToString(dr["TaskBattery"].ToString()),
                        MultipleSessions = Convert.ToBoolean(dr["MultipleSessions"].ToString()),
                    });
                }

            }

            return lstExperiment;

        }

        // Function Definition to get Imagepath from ImageID
        public static string GetImagePathFromImageIdCsv(string imageIdCsv)
        {
            // todo: this shouldn't get the values from db everytime we are calling it

            if (string.IsNullOrEmpty(imageIdCsv))
            {
                return "";
            }

            string sql = $@"Select * From Image Where Id in ({imageIdCsv}) ";

            List<string> imageList = new List<string>();

            using (DataTable dt = Dal.GetDataTable(sql))
            {
                foreach (DataRow dr in dt.Rows)
                {

                    imageList.Add(
                       $"<img src ='{Convert.ToString(dr["ImagePath"].ToString())}' width='30' height='30' style='margin-top:15px;' />"
                    );
                }

            }

            return string.Join(", ", imageList);
        }

        public bool DoesExperimentNameExist(string experimentName)
        {
            string sql = $"select count(*) from Experiment where ltrim(rtrim(ExpName)) = '{HelperService.EscapeSql(experimentName.Trim())}' ";

            int countResult = Int32.Parse(Dal.ExecScalar(sql).ToString());

            bool flag = (countResult == 0) ? false : true;
            return flag;
        }

        public bool DoesExperimentNameExistEdit(string experimentName, int expID)
        {
            string sql = $"select count(*) from Experiment where ltrim(rtrim(ExpName)) = '{HelperService.EscapeSql(experimentName.Trim())}' AND ExpID != {expID} ";

            int countResult = Int32.Parse(Dal.ExecScalar(sql).ToString());

            bool flag = (countResult == 0) ? false : true;
            return flag;
        }

        public int InsertExperiment(Experiment experiment, string userID, string Email)

        {
            
            string sql = $"insert into Experiment " +
              $"(UserID, PUSID, ExpName, StartExpDate, EndExpDate, TaskID, SpeciesID, TaskDescription, DOI, Status, TaskBattery, MultipleSessions) Values " +
              $"('{userID}', {experiment.PUSID}, '{HelperService.EscapeSql(experiment.ExpName.Trim())}', '{experiment.StartExpDate}', '{experiment.EndExpDate}', " +
              $"'{experiment.TaskID}', '{experiment.SpeciesID}', '{HelperService.EscapeSql(experiment.TaskDescription)}'," +
              $" '{HelperService.EscapeSql(experiment.DOI)}', {(experiment.Status ? 1 : 0)}, '{HelperService.EscapeSql(experiment.TaskBattery)}', {(experiment.MultipleSessions ? 1 : 0)} );" +
              $" SELECT @@IDENTITY AS 'Identity';";

            // Calling function to send an email to Admin that new Exp with public Status has been added to MouseBytes

            string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{Email}</b> has just created the experiment: <b>{HelperService.EscapeSql(experiment.ExpName.Trim())}</b>
                                    with public Status!  <br /><br /> Thanks <br /> MouseBytes";
            HelperService.SendEmail("", "", "New Experiment with pubic status added!", strBody);


            return Int32.Parse(Dal.ExecScalar(sql).ToString());
        }

        public void UpdateExp(Experiment experiment, string userID, string Email)
        {

            
            string sql = $@"UPDATE Experiment " +
                 $"SET PUSID = {experiment.PUSID}, ExpName = '{HelperService.EscapeSql(experiment.ExpName)}', StartExpDate = '{experiment.StartExpDate}'," +
                 $"EndExpDate = '{experiment.EndExpDate}', SpeciesID = {experiment.SpeciesID}, TaskDescription = '{HelperService.EscapeSql(experiment.TaskDescription)}'," +
                 $" DOI = '{HelperService.EscapeSql(experiment.DOI)}', TaskBattery = '{HelperService.EscapeSql(experiment.TaskBattery)}',  Status = {(experiment.Status ? 1 : 0)}, MultipleSessions = {(experiment.MultipleSessions ? 1 : 0)} " +
                 $" WHERE ExpID = {experiment.ExpID}  AND UserID = '{userID}';";

            if (experiment.Status)
            {
                string strBody = $@"Hi Admin: <br /><br /> User with Email address <b>{Email}</b> has just changed the status of the experiment: <b>{HelperService.EscapeSql(experiment.ExpName.Trim())}</b>
                                    to public!  <br /><br /> Thanks <br /> MouseBytes";
                HelperService.SendEmail("", "", "Status of experiment changed to public!", strBody);
            }

            Dal.ExecuteNonQuery(sql);

        }

        public void DeleteExpByExpID(int expID)
        {
            string sql = $@"Delete From RBT_TouchScreen_Features Where SessionID in (Select SessionID From SessionInfo Where ExpID = {expID});
                            Delete From rbt_data_cached Where SessionID in (Select SessionID From SessionInfo Where ExpID = {expID});
                            Delete From SessionInfo_Dynamic Where SessionID in (Select SessionID From SessionInfo Where ExpID = {expID});
                            Delete From SessionInfo Where ExpID = {expID};
                            Delete From Upload Where ExpID ={expID};
                            Delete From Animal Where ExpID ={expID};
                            Delete From UploadErrorLog Where ExpID ={expID};
                            Delete From SubExperiment Where ExpId = {expID};
                            Delete From Experiment Where ExpID={expID};";

            Dal.ExecuteNonQuery(sql);
        }

        public void DeleteExpByUploadID(int uploadID)
        {
            string sql = $@"Delete From RBT_TouchScreen_Features Where SessionID in (Select SessionID From SessionInfo Where UploadID = {uploadID});
                            Delete From rbt_data_cached Where SessionID in (Select SessionID From SessionInfo Where UploadID = {uploadID});
                            Delete From SessionInfo_Dynamic Where SessionID in (Select SessionID From SessionInfo Where UploadID = {uploadID});
                            Delete From SessionInfo Where UploadID = {uploadID};
                            Delete From Upload Where UploadID = {uploadID};";

            Dal.ExecuteNonQuery(sql);
        }

        // Function definition to get image list from DB for PAL and PD
        public List<Image> GetAllImagesList()
        {
            List<Image> lstImages = new List<Image>();

            using (DataTable dt = Dal.GetDataTable("select * from Image"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstImages.Add(new Image
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        ImagePath = Convert.ToString(dr["ImagePath"].ToString()),

                    });
                }

            }

            return lstImages;

        }

        // Function definition to get image list from DB for PAL and PD
        public List<Species_> GetAllSpeciesList()
        {
            List<Species_> lstSpecies = new List<Species_>();

            using (DataTable dt = Dal.GetDataTable("select * from Species"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstSpecies.Add(new Species_
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Species = Convert.ToString(dr["Species"].ToString()),

                    });
                }

            }

            return lstSpecies;

        }





    }
}
