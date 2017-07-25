namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    public enum UserEvacuationStatus
    {
        Missing,
        Present,
        Away
    }

    public class UserEvacuationStatusViewModel
    {
        public UserEvacuationStatusViewModel()
        {
        }

        public UserEvacuationStatusViewModel(ApplicationUser user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Status = user.Status;
            UserId = user.Id;
        }

        public UserEvacuationStatus Status
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

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

        public string UserId
        {
            get;
            set;
        }
    }
}