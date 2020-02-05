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

    public class PISiteService
    {

        // Function Defintion: extracting list of PI & institution From PISite tbl in Database (For sign up)
        public List<PISite> GetPISiteList()
        {
            List<PISite> lstPISite = new List<PISite>();

            using (DataTable dt = Dal.GetDataTable($@"Select PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) as PISiteName From PISite
                                                        inner join PI on PI.PID = PISite.PID
                                                        inner join Site on Site.SiteID = PISite.SiteID"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstPISite.Add(new PISite
                    {
                        PSID = Int32.Parse(dr["PSID"].ToString()),
                        PName = Convert.ToString(dr["PName"].ToString()),
                        Institution = Convert.ToString(dr["Institution"].ToString()),
                        PISiteName = Convert.ToString(dr["PISiteName"].ToString()),
                      
                    });
                }

            }

            return lstPISite;

        }

        // Function Definition: Insert the PI and institution that user selected in "sign up" to tbl PIUSERSite  
        internal void InsertPiSiteIds(string userId, List<int> selectedPiSiteIds)
        {
            // add selectedPiSiteIds to this user
            foreach(var PiSiteID in selectedPiSiteIds)
            {
                string sql = $@"Insert into PIUserSite (PSID, UserID) Values ({PiSiteID}, '{userId}'); ";

                Dal.ExecuteNonQuery(sql);
            }

        }

        // Function Defintion: Extracting list of PI & Institutions for the login User (for exp creation)
        public List<PISite> GetPISitebyUserID(string UserID)
        {
            List<PISite> lstPISite = new List<PISite>();
            using (DataTable dt = Dal.GetDataTable($@"Select PUSID, PIUserSite.PSID, tt.PISiteName  From PIUserSite
                                                        inner join 
                                                        (Select PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) as PISiteName From PISite
                                                        inner join PI on PI.PID = PISite.PID
                                                        inner join Site on Site.SiteID = PISite.SiteID) as tt on tt.PSID = PIUserSite.PSID
                                                        Where PIUserSite.UserID = '{UserID}'"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstPISite.Add(new PISite
                    {
                        PUSID = Int32.Parse(dr["PUSID"].ToString()),
                        PSID = Int32.Parse(dr["PSID"].ToString()),
                        PISiteName = Convert.ToString(dr["PISiteName"].ToString()),

                    });
                }

            }

            return lstPISite;
        }


    }
}
