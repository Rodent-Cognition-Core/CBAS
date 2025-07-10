using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.AccountViewModels;
using CBAS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{

    public class ProfileService
    {

        public Users GetUserInfoByID(string userID)
        {
            var user= new Users();

            using (DataTable dt = Dal.GetDataTable($@"Select GivenName, FamilyName, Email From dbo.AspNetUsers Where Id = '{userID}'"))
            {
                    user = new Users
                    {
                        GivenName = Convert.ToString(dt.Rows[0]["GivenName"].ToString()),
                        FamilyName = Convert.ToString(dt.Rows[0]["FamilyName"].ToString()),
                        Email = Convert.ToString(dt.Rows[0]["Email"].ToString()),
                       
                    };

            }

            return user;

        }

       
        //Function Definition for updating the user profile
        public void UpdateProfile(CreateViewModel model, string userID)
        {
            // For Daniel  (UserAnimalID = '{animal.UserAnimalID}' should be removed from the sql query)
            string sql = $@"UPDATE dbo.AspNetUsers set GivenName = '{model.givenName}', FamilyName = '{model.familyName}' Where Id = '{userID}'";
            Dal.ExecuteNonQuery(sql);

            if (model.selectedPiSiteIds != null)
            foreach (var PiSiteID in model.selectedPiSiteIds)
            {
                string sql2 = $@"Insert into tsd.PIUserSite (PSID, UserID) Values ({PiSiteID}, '{userID}'); ";
                Dal.ExecuteNonQuery(sql2);
            }

        }





    }
}
