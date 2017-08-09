using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    /// <summary>
    ///     Class Role.
    /// </summary>
    /// <seealso cref="IdentityRole" />
    /// TODO Edit XML Comment Template for Role
    public class Role : IdentityRole
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Role" />
        ///     class.
        /// </summary>
        /// <remarks>
        ///     The Id property is initialized to from a new GUID
        ///     string value.
        /// </remarks>
        /// TODO Edit XML Comment Template for #ctor
        public Role()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Role" />
        ///     class.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="index">The index.</param>
        /// TODO Edit XML Comment Template for #ctor
        public Role(string roleName, int index)
            : base(roleName)
        {
            Index = index;
        }

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        /// TODO Edit XML Comment Template for Index
        public int Index
        {
            get;
            set;
        }
    }
}