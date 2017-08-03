namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {
        }

        public UserViewModel(User user)
        {
            Department = user.Department;
            Email = user.Email;
            FirstName = user.FirstName;
            Id = user.Id;
            IsActive = user.IsActive;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Status = user.Status;
        }

        public string Department
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

        public string Id
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string PhoneNumber
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