using Microsoft.AspNetCore.Identity;

namespace AngularSPAWebAPI.Models
{
    public class Users
    {

        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PISiteName { get; set; }

        
    }
    public class UsersLoginInfo
    {
        public bool IsUserApproved { get; set; }
        public bool IsUserLocked { get; set; }

    }
}
