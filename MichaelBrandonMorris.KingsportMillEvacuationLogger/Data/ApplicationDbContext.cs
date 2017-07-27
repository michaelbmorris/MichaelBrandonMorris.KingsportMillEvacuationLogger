using System.Diagnostics;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Data
{
    /// <summary>
    ///     Class ApplicationDbContext.
    /// </summary>
    /// <seealso
    ///     cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.User}" />
    /// TODO Edit XML Comment Template for ApplicationDbContext
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="ApplicationDbContext" /> class.
        /// </summary>
        /// <param name="options">
        ///     The options to be used by a
        ///     <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />
        ///     .
        /// </param>
        /// TODO Edit XML Comment Template for #ctor
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        /// TODO Edit XML Comment Template for User
        public DbSet<User> User
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// TODO Edit XML Comment Template for UpdateUser
        public void UpdateUser(UserViewModel model)
        {
            var user = Users.Find(model.Id);
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
        }

        public void UpdateUserStatus(UserEvacuationStatusViewModel model)
        {
            var user = Users.Find(model.Id);
            user.Status = model.Status;
        }
    }
}