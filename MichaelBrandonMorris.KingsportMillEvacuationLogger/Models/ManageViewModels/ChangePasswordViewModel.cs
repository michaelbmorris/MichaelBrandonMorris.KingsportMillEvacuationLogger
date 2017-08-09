using System.ComponentModel.DataAnnotations;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.
    ManageViewModels
{
    /// <summary>
    ///     Class ChangePasswordViewModel.
    /// </summary>
    /// TODO Edit XML Comment Template for ChangePasswordViewModel
    public class ChangePasswordViewModel
    {
        /// <summary>
        ///     Gets or sets the confirm password.
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
        ///     Gets or sets the new password.
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
        ///     Gets or sets the old password.
        /// </summary>
        /// <value>The old password.</value>
        /// TODO Edit XML Comment Template for OldPassword
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword
        {
            get;
            set;
        }
    }
}