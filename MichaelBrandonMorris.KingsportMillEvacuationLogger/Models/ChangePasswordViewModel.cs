using System.ComponentModel.DataAnnotations;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    /// <summary>
    /// Class ChangePasswordViewModel.
    /// </summary>
    /// TODO Edit XML Comment Template for ChangePasswordViewModel
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordViewModel"/> class.
        /// </summary>
        /// TODO Edit XML Comment Template for #ctor
        public ChangePasswordViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordViewModel"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// TODO Edit XML Comment Template for #ctor
        public ChangePasswordViewModel(User user)
        {
            Email = user.Email;
            Id = user.Id;
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        /// TODO Edit XML Comment Template for Email
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        /// <value>The confirm password.</value>
        /// TODO Edit XML Comment Template for ConfirmPassword
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

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        /// <value>The new password.</value>
        /// TODO Edit XML Comment Template for NewPassword
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

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        /// TODO Edit XML Comment Template for Id
        public string Id
        {
            get;
            set;
        }
    }
}