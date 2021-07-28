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

    public class SubExperimentService
    {

        public List<SubExperiment> GetAllSubExperimentsByExpID (int ExpID)
        {
            List<SubExperiment> SubExpList = new List<SubExperiment>();

            using (DataTable dt = Dal.GetDataTable($@"Select se.SubExpID, se.ExpID, se.AgeID, se.SubExpName, se.IsPostProcessingPass, se.ErrorMessage, Age.AgeInMonth,
                                                        se.IsIntervention, se.IsDrug, se.DrugName, se.DrugUnit, se.DrugQuantity, se.InterventionDescription, se.ImageIds, se.ImageDescription,
                                                        se.Housing, se.LightCycle
                                                        From SubExperiment se Inner Join Age On Age.ID = se.AgeID Where ExpID = {ExpID} order by Age.ID"))
            {
                foreach(DataRow dr in dt.Rows)
                {
                    var imageIds = Convert.ToString(dr["ImageIds"].ToString());

                    SubExpList.Add(new SubExperiment
                    {
                        SubExpID = Int32.Parse(dr["SubExpID"].ToString()),
                        ExpID = Int32.Parse(dr["ExpID"].ToString()),
                        SubExpName = Convert.ToString(dr["SubExpName"].ToString()),
                        AgeID = Int32.Parse(dr["AgeID"].ToString()),
                        AgeInMonth = Convert.ToString(dr["AgeInMonth"].ToString()),
                        IsPostProcessingPass = bool.Parse(dr["IsPostProcessingPass"].ToString()),
                        ErrorMessage = Convert.ToString(dr["ErrorMessage"].ToString()),
                        IsIntervention = bool.Parse(dr["IsIntervention"].ToString()),
                        IsDrug = bool.Parse(dr["IsDrug"].ToString()),
                        DrugName = Convert.ToString(dr["DrugName"].ToString()),
                        DrugQuantity = Convert.ToString(dr["DrugQuantity"].ToString()),
                        DrugUnit = Convert.ToString(dr["DrugUnit"].ToString()),
                        InterventionDescription = Convert.ToString(dr["InterventionDescription"].ToString()),
                        ImageIds = string.IsNullOrEmpty(imageIds) ? null : imageIds.Split(',').Select(s => Int32.Parse(s)).ToArray(),
                        ImageInfo = $@"{GetImagePathFromImageIdCsv(Convert.ToString(dr["ImageIds"]))}",
                        ImageDescription = Convert.ToString(dr["ImageDescription"].ToString()),
                        Housing = Convert.ToString(dr["Housing"].ToString()),
                        LightCycle = Convert.ToString(dr["LightCycle"].ToString()),

                    });
                }
            }

                return SubExpList;

        }

        // Function Definition to get Imagepath from ImageIDCsv
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

        public List<Age> GetAgeList()
        {

            List<Age> AgeList = new List<Age>();
            using (DataTable dt = Dal.GetDataTable($@"select * from age"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AgeList.Add(new Age
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        AgeInMonth = Convert.ToString(dr["AgeInMonth"].ToString()),
                       
                    });
                }
            }

            return AgeList;

        }



        public (bool flagSubExp, bool flagAge) DoesSubExperimentNameExist(SubExperiment subexpObj, int subExpId)
        {
            string sqlsubexp = $"select count(*) from SubExperiment where ltrim(rtrim(SubExpName)) = '{HelperService.EscapeSql(subexpObj.SubExpName.Trim())}' {(subExpId == 0 ? "" : " AND SubExpID != " + subexpObj.SubExpID)} ";
            string sqlage = $@"select count(*) from SubExperiment where AgeID={subexpObj.AgeID} and Isintervention={(subexpObj.IsIntervention ? 1 : 0)} and
                            IsDrug = {(subexpObj.IsDrug ? 1 : 0)} and
                            ltrim(rtrim(DrugName)) = '{HelperService.EscapeSql(subexpObj.DrugName.Trim())}' and
                            DrugUnit = '{HelperService.EscapeSql(subexpObj.DrugUnit.Trim())}' and
                            DrugQuantity = '{HelperService.EscapeSql(subexpObj.DrugQuantity.Trim())}' and
                            CONVERT(VARCHAR, InterventionDescription) = '{HelperService.EscapeSql(subexpObj.InterventionDescription.Trim())}' and
                            Housing = '{HelperService.EscapeSql(subexpObj.Housing.Trim())}' and
                            LightCycle = '{HelperService.EscapeSql(subexpObj.LightCycle.Trim())}' and
                            ExpID = {subexpObj.ExpID} {(subExpId == 0 ? "" : " AND SubExpID != " + subexpObj.SubExpID)} ";

            int countResultSubExpName = Int32.Parse(Dal.ExecScalar(sqlsubexp).ToString());
            int countResultAge = Int32.Parse(Dal.ExecScalar(sqlage).ToString());
                        
            bool flagSubExp1 = (countResultSubExpName == 0) ? false : true;
            bool flagAge1 = (countResultAge == 0) ? false : true;

            return (flagSubExp: flagSubExp1, flagAge: flagAge1);
        }

        public int InsertSubExperiment(SubExperiment subexperiment)
        {
            string ImageIDcsv = "";

            if (subexperiment.ImageIds != null && subexperiment.ImageIds.Length != 0)
            {

                ImageIDcsv = String.Join(",", subexperiment.ImageIds.Select(x => x.ToString()).ToArray());

            }

            string sql = $@"Insert into SubExperiment  
                        (ExpID, AgeID, SubExpName, IsIntervention, IsDrug, DrugName, DrugUnit, DrugQuantity, InterventionDescription, ImageIds, ImageDescription, Housing, LightCycle) Values  
                ({subexperiment.ExpID}, {subexperiment.AgeID}, '{HelperService.EscapeSql(subexperiment.SubExpName.Trim())}',
                {(subexperiment.IsIntervention ? 1 : 0)}, {(subexperiment.IsDrug ? 1 : 0)}, '{HelperService.EscapeSql(subexperiment.DrugName)}',
                '{HelperService.EscapeSql(subexperiment.DrugUnit)}', '{HelperService.EscapeSql(subexperiment.DrugQuantity)}',
                '{HelperService.EscapeSql(subexperiment.InterventionDescription)}',
                '{ImageIDcsv}', '{HelperService.EscapeSql(subexperiment.ImageDescription)}',
                '{HelperService.EscapeSql(subexperiment.Housing)}', '{HelperService.EscapeSql(subexperiment.LightCycle)}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalar(sql).ToString());
        }

        public void UpdateSubExperiment(SubExperiment subexperiment)
        {

            string ImageIDcsv = "";

            if (subexperiment.ImageIds != null && subexperiment.ImageIds.Length != 0)
            {

                ImageIDcsv = String.Join(",", subexperiment.ImageIds.Select(x => x.ToString()).ToArray());

            }

            

            string sql = $@"UPDATE SubExperiment 
                        SET SubExpName = '{HelperService.EscapeSql(subexperiment.SubExpName.Trim())}', AgeID = {subexperiment.AgeID},
                        IsIntervention = {(subexperiment.IsIntervention ? 1 : 0)}, IsDrug = {(subexperiment.IsDrug ? 1 : 0)}, DrugName = '{HelperService.EscapeSql(subexperiment.DrugName)}',
                        DrugUnit = '{HelperService.EscapeSql(subexperiment.DrugUnit)}', DrugQuantity= '{HelperService.EscapeSql(subexperiment.DrugQuantity)}',
                        InterventionDescription = '{HelperService.EscapeSql(subexperiment.InterventionDescription)}',
                        ImageIds = '{ImageIDcsv}', ImageDescription = '{subexperiment.ImageDescription}',
                        Housing = '{subexperiment.Housing}', LightCycle = '{subexperiment.LightCycle}'
                        WHERE SubExpID = {subexperiment.SubExpID} ;";

            Dal.ExecuteNonQuery(sql);
        }


        public void DeleteSubExpBySubExpID(int subExpID)
        {
            string sql = $@"Delete From RBT_TouchScreen_Features Where SessionID in
                         (Select SessionID From SessionInfo Where UploadID in (Select UploadID From Upload Where SubExpID = {subExpID} ) );

                         Delete From SessionInfo_Dynamic Where SessionID in
                         (Select SessionID From SessionInfo Where UploadID in (Select UploadID From Upload Where SubExpID = {subExpID} ) );

                        Delete From rbt_data_cached_avg Where SessionID in (Select SessionID From SessionInfo Where UploadID in (Select UploadID From Upload Where SubExpID = {subExpID} );
                        Delete From rbt_data_cached_std Where SessionId2 in (Select SessionID From SessionInfo Where UploadID in (Select UploadID From Upload Where SubExpID = {subExpID} );
                        Delete From rbt_data_cached_cnt Where SessionId3 in (Select SessionID From SessionInfo Where UploadID in (Select UploadID From Upload Where SubExpID = {subExpID} );
                        Delete From rbt_data_cached_sum Where SessionId4 in (Select SessionID From SessionInfo Where UploadID in (Select UploadID From Upload Where SubExpID = {subExpID} );

                        Delete From SessionInfo Where UploadID in (Select UploadID From Upload Where SubExpID = {subExpID} );
                        Delete From Upload Where SubExpID = {subExpID};
                        Delete From UploadErrorLog Where SubExpID = {subExpID};
                        Delete From SubExperiment Where SubExpID={subExpID};";

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


    }
}
