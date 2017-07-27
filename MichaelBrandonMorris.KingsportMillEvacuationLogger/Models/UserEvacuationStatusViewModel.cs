﻿namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    public enum UserEvacuationStatus
    {
        Missing,
        Here,
        Away,
        Inactive
    }

    public class UserEvacuationStatusViewModel
    {
        public UserEvacuationStatusViewModel()
        {
        }

        public UserEvacuationStatusViewModel(User user)
        {
            if (user == null)
            {
                return;
            }

            Department = user.Department;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Status = user.Status;
            Id = user.Id;
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

        public string PhoneNumber
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

        public UserEvacuationStatus Status
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }
    }
}