using System.ComponentModel.DataAnnotations;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel()
        {
        }

        public ChangePasswordViewModel(User user)
        {
            Email = user.Email;
            Id = user.Id;
        }

        public string Email
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare(
            "NewPassword",
            ErrorMessage =
                "The new password and confirmation password do not match.")]
        public string ConfirmPassword
        {
            get;
            set;
        }

        [Required]
        [StringLength(
            100,
            ErrorMessage =
                "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword
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