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
    public class ManageUserService
    {
        public async Task<UsersLoginInfo> IsUserEmailApprovedAsync(string username)
        {
            bool isUserApproved = false;
            bool isUserLocked = false;

            try
            {
                const string sql = @"
                    SELECT 
                        EmailConfirmed, 
                        LockoutEnd 
                    FROM AspNetUsers 
                    WHERE UserName = @UserName";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@UserName", username)
                };

                using (var dt = await Dal.GetDataTableAsync(sql, parameters))
                {
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        isUserApproved = row["EmailConfirmed"] != DBNull.Value && Convert.ToBoolean(row["EmailConfirmed"]);
                        isUserLocked = row["LockoutEnd"] != DBNull.Value && !string.IsNullOrEmpty(row["LockoutEnd"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking if user email is approved for user {Username}", username);
                throw;
            }

            return new UsersLoginInfo
            {
                IsUserApproved = isUserApproved,
                IsUserLocked = isUserLocked
            };
        }

        public async Task<List<Users>> GetAllUserListAsync()
        {
            var lstUser = new List<Users>();

            try
            {
                const string sql = @"
                    SELECT AspNetUsers.GivenName, AspNetUsers.FamilyName, AspNetUsers.Email, EmailConfirmed,
                    STUFF(( 
                        SELECT DISTINCT ' <br/>' + CONCAT(PName, ' - ', Institution) AS PISiteName
                        FROM PISite
                        INNER JOIN PI ON PI.PID = PISite.PID
                        INNER JOIN Site ON Site.SiteID = PISite.SiteID
                        INNER JOIN PIUserSite ON PISite.PSID = PIUserSite.PSID
                        WHERE PIUserSite.UserID = AspNetUsers.id
                        FOR XML PATH(''), TYPE
                    ).value('.', 'nvarchar(max)'), 1, 6, '') AS PISiteName
                    FROM AspNetUsers";

                using (var dt = await Dal.GetDataTableAsync(sql))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstUser.Add(new Users
                        {
                            GivenName = Convert.ToString(dr["GivenName"]),
                            FamilyName = Convert.ToString(dr["FamilyName"]),
                            Email = Convert.ToString(dr["Email"]),
                            EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]),
                            PISiteName = Convert.ToString(dr["PISiteName"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving all user list");
                throw;
            }

            return lstUser;
        }

        public async Task ApproveSelectedUserAsync(string email)
        {
            try
            {
                const string sql = "UPDATE AspNetUsers SET EmailConfirmed = 'true' WHERE Email = @Email";
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Email", email)
                };

                await Dal.ExecuteNonQueryAsync(sql, parameters.ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error approving selected user with email {Email}", email);
                throw;
            }
        }

        public async Task LockSelectedUserAsync(string email)
        {
            try
            {
                const string sql = "UPDATE AspNetUsers SET EmailConfirmed = 'false' WHERE Email = @Email";
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Email", email)
                };

                await Dal.ExecuteNonQueryAsync(sql, parameters.ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error locking selected user with email {Email}", email);
                throw;
            }
        }



    }
}
