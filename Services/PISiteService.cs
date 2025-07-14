using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        internal async Task InsertPiSiteIdsAsync(string userId, List<int> selectedPiSiteIds)
        {
            try
            {
                foreach (var piSiteID in selectedPiSiteIds)
                {
                    const string sql = "INSERT INTO PIUserSite (PSID, UserID) VALUES (@PSID, @UserID)";
                    var parameters = new List<SqlParameter>
                    {
                        new("@PSID", piSiteID),
                        new("@UserID", userId)
                    };

                    await Dal.ExecuteNonQueryAsync(sql, parameters.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error inserting PI Site IDs for user {UserId}", userId);
                throw;
            }
        }

        // Function Defintion: Extracting list of PI & Institutions for the login User (for exp creation)
        public async Task<List<PISite>> GetPISitebyUserIDAsync(string userId)
        {
            var lstPISite = new List<PISite>();

            try
            {
                const string sql = @"
                    SELECT PUSID, PIUserSite.PSID, tt.PISiteName
                    FROM PIUserSite
                    INNER JOIN (
                        SELECT PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) AS PISiteName
                        FROM PISite
                        INNER JOIN PI ON PI.PID = PISite.PID
                        INNER JOIN Site ON Site.SiteID = PISite.SiteID
                    ) AS tt ON tt.PSID = PIUserSite.PSID
                    WHERE PIUserSite.UserID = @UserID";

                var parameters = new List<SqlParameter>
                {
                    new("@UserID", userId)
                };

                using (var dt = await Dal.GetDataTableAsync(sql, parameters))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstPISite.Add(new PISite
                        {
                            PUSID = Convert.ToInt32(dr["PUSID"]),
                            PSID = Convert.ToInt32(dr["PSID"]),
                            PISiteName = Convert.ToString(dr["PISiteName"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving PI Sites for user {UserId}", userId);
                throw;
            }

            return lstPISite;
        }

        public void DeletePIUserSitebyPSID(string userID, int psid)
        {
            string sql = $@"DELETE FROM PIUserSite Where PUSID={psid} AND userID={userID};";
            Dal.ExecuteNonQuery(sql);
        }


    }
}
