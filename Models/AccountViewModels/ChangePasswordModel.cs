using System.Collections.Generic;

namespace AngularSPAWebAPI.Models.AccountViewModels
{
    /// <summary>
    /// Class required to create a new user.
    /// </summary>
    public class ChangePasswordModel
    {
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
    }
}
