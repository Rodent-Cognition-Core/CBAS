using AngularSPAWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{

    public class TaskAnalysisService
    {
        public List<TaskAnalysis> GetAllTaskAnalysis()
        {
            List<TaskAnalysis> lstTaskAnalyses = new List<TaskAnalysis>();

            using (DataTable dt = Dal.GetDataTable("select * from Task Where ID not in (1,6)"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstTaskAnalyses.Add(new TaskAnalysis
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Name = Convert.ToString(dr["Name"].ToString()),
                        OriginalName = Convert.ToString(dr["OriginalName"].ToString()),
                    });
                }

            }

            return lstTaskAnalyses;

        }


        public string GetTaskAnalysisByID(int expid)
        {
            string sql = "";
            return Dal.ExecScalar(sql).ToString();
        }

        public int InsertTest(TaskAnalysis taskAnalysis)
        {
            string sql = $"insert into Test (Name, Last_name) Values ('{taskAnalysis.Name}', '{taskAnalysis.OriginalName}'); SELECT @@IDENTITY AS 'Identity';";
            //Dal.ExecuteNonQuery(sql);
            //Dal.ExecuteScalar(sql);
            return Int32.Parse(Dal.ExecScalar(sql).ToString());
        }
    }
}
