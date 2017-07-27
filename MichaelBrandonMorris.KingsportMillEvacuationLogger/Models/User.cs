using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    public class User : IdentityUser
    {
        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }

        public UserEvacuationStatus Status
        {
            get;
            set;
        }
    }
}