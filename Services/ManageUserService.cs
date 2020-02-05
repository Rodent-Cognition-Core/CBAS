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
    public class ManageUserService
    {
        public UsersLoginInfo IsUserEmailApproved(string username)
        {
            bool IsUserApproved = false;
            bool IsUserLocked = false;

            string sql = $@"Select EmailConfirmed From AspNetUsers Where UserName = '{username}'";
            IsUserApproved = Convert.ToBoolean(Dal.ExecScalar(sql).ToString());

            string sql2 = $@"Select LockoutEnd From AspNetUsers Where UserName = '{username}'";
            IsUserLocked = string.IsNullOrEmpty(Dal.ExecScalar(sql2).ToString()) ? false : true ;

            return new UsersLoginInfo {
                IsUserApproved = IsUserApproved,
                IsUserLocked = IsUserLocked
            };
        }

        
        public List<Users> GetAllUserList()
        {
            List<Users> lstUser = new List<Users>();

            using (DataTable dt = Dal.GetDataTable($@"Select AspNetUsers.GivenName, AspNetUsers.FamilyName, AspNetUsers.Email, EmailConfirmed,

                                                        STUFF(( SELECT distinct ' <br/>' + CONCAT(PName, ' - ', Institution) as PISiteName
                                                        From PISite
                                                        inner join PI on PI.PID = PISite.PID
                                                        inner join Site on Site.SiteID = PISite.SiteID
                                                        inner join PIUserSite on PISite.PSID = PIUserSite.PSID
                                                        Where PIUserSite.UserID = AspNetUsers.id

                                                        FOR XML PATH(''), type
                                                        ).value('.', 'nvarchar(max)'),1,6,'') as PISiteName

                                                        From AspNetUsers"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstUser.Add(new Users
                    {
                        GivenName = Convert.ToString(dr["GivenName"].ToString()),
                        FamilyName = Convert.ToString(dr["FamilyName"].ToString()),
                        Email = Convert.ToString(dr["Email"].ToString()),
                        EmailConfirmed =Convert.ToBoolean(dr["EmailConfirmed"].ToString()),
                        PISiteName = Convert.ToString(dr["PISiteName"].ToString()),
                        
                    });
                }

            }

            return lstUser;
        }

        public void ApproveSelectedUser(string email)
        {
            string sql = $@"Update AspNetUsers set EmailConfirmed='true' Where Email = '{email}'";
            Dal.ExecuteNonQuery(sql);
        }

        public void LockSelectedUser(string email)
        {
            string sql = $@"Update AspNetUsers set EmailConfirmed='false' Where Email = '{email}'";
            Dal.ExecuteNonQuery(sql);
        }



    }
}
