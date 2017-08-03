using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    /// <summary>
    ///     Class User.
    /// </summary>
    /// <seealso cref="IdentityUser" />
    /// TODO Edit XML Comment Template for User
    public sealed class User : IdentityUser
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="User" />
        ///     class.
        /// </summary>
        /// <remarks>
        ///     The Id property is initialized to from a new GUID
        ///     string value.
        /// </remarks>
        /// TODO Edit XML Comment Template for #ctor
        public User()
        {
            Status = UserEvacuationStatus.Unknown;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="User" />
        ///     class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// TODO Edit XML Comment Template for #ctor
        public User(UserViewModel model)
        {
            Department = model.Department;
            Email = model.Email;
            FirstName = model.FirstName;
            Id = model.Id;
            LastName = model.LastName;
            Status = model.Status;
            UserName = model.Email;
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
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        /// TODO Edit XML Comment Template for FirstName
        public string FirstName
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
        public bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        /// TODO Edit XML Comment Template for LastName
        public string LastName
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