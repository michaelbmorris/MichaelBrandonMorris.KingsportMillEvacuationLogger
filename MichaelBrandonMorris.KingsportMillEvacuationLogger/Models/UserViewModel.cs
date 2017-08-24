using System.ComponentModel.DataAnnotations;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    /// <summary>
    ///     Class UserViewModel.
    /// </summary>
    /// TODO Edit XML Comment Template for UserViewModel
    public class UserViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="UserViewModel" /> class.
        /// </summary>
        /// TODO Edit XML Comment Template for #ctor
        public UserViewModel()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="UserViewModel" /> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="roleName"></param>
        /// TODO Edit XML Comment Template for #ctor
        public UserViewModel(User user, string roleName)
        {
            Department = user.Department;
            Email = user.Email;
            FirstName = user.FirstName;
            Id = user.Id;
            IsActive = user.IsActive;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            RoleName = roleName;
            Status = user.Status;
        }

        /// <summary>
        ///     Gets or sets the department.
        /// </summary>
        /// <value>The department.</value>
        /// TODO Edit XML Comment Template for Department
        public string Department
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        /// TODO Edit XML Comment Template for Email
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        /// TODO Edit XML Comment Template for FirstName
        [Display(Name = "First Name")]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        /// TODO Edit XML Comment Template for Id
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance
        ///     is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is active; otherwise,
        ///     <c>false</c>.
        /// </value>
        /// TODO Edit XML Comment Template for IsActive
        [Display(Name = "Is Active?")]
        public bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance
        ///     is imported.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is imported; otherwise,
        ///     <c>false</c>.
        /// </value>
        /// TODO Edit XML Comment Template for IsImported
        [Display(Name = "Is Imported?")]
        public bool IsImported
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        /// TODO Edit XML Comment Template for LastName
        [Display(Name = "Last Name")]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        /// TODO Edit XML Comment Template for PhoneNumber
        [Display(Name = "Phone Number")]
        public string PhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        /// TODO Edit XML Comment Template for RoleName
        public string RoleName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        /// TODO Edit XML Comment Template for Status
        public UserEvacuationStatus Status
        {
            get;
            set;
        }
    }
}