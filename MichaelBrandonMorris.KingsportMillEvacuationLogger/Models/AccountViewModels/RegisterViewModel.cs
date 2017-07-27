using System.ComponentModel.DataAnnotations;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.
    AccountViewModels
{
    public class RegisterViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(
            "Password",
            ErrorMessage =
                "The password and confirmation password do not match.")]
        public string ConfirmPassword
        {
            get;
            set;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get;
            set;
        }

        [Required]
        [StringLength(
            100,
            ErrorMessage =
                "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password
        {
            get;
            set;
        }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(
            @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            ErrorMessage = "Not a valid phone number.")]
        public string PhoneNumber
        {
            get;
            set;
        }

        [Required]
        public string Department
        {
            get;
            set;
        }
    }
}