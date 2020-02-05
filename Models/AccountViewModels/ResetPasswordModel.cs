using System.Collections.Generic;

namespace AngularSPAWebAPI.Models.AccountViewModels
{
    /// <summary>
    /// Class required to create a new user.
    /// </summary>
    public class ResetPasswordModel
    {
        public string email { get; set; }
        public string token { get; set; }
        public string password { get; set; }
    }
}
