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
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
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
    }
}