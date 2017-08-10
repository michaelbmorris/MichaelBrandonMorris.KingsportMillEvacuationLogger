using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    /// <summary>
    ///     Class ChangeRoleViewModel.
    /// </summary>
    /// TODO Edit XML Comment Template for ChangeRoleViewModel
    public class ChangeRoleViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="ChangeRoleViewModel" /> class.
        /// </summary>
        /// TODO Edit XML Comment Template for #ctor
        public ChangeRoleViewModel()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="ChangeRoleViewModel" /> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="roleNames">The role names.</param>
        /// TODO Edit XML Comment Template for #ctor
        public ChangeRoleViewModel(User user, IEnumerable<string> roleNames)
        {
            Email = user.Email;
            UserId = user.Id;
            RoleNames = roleNames;
        }

        /// <summary>
        ///     Gets the role name select list items.
        /// </summary>
        /// <value>The role name select list items.</value>
        /// TODO Edit XML Comment Template for RoleNameSelectListItems
        public IEnumerable<SelectListItem> RoleNameSelectListItems => RoleNames
            ?.Select(
                roleName => new SelectListItem
                {
                    Text = roleName,
                    Value = roleName
                });

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
        ///     Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        /// TODO Edit XML Comment Template for RoleName
        [Display(Name = "Role")]
        public string RoleName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the role names.
        /// </summary>
        /// <value>The role names.</value>
        /// TODO Edit XML Comment Template for RoleNames
        public IEnumerable<string> RoleNames
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        /// TODO Edit XML Comment Template for UserId
        public string UserId
        {
            get;
            set;
        }
    }
}