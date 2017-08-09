using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Data
{
    /// <summary>
    ///     Class ApplicationDbContext.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{T}" />
    /// TODO Edit XML Comment Template for ApplicationDbContext
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
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
    }
}