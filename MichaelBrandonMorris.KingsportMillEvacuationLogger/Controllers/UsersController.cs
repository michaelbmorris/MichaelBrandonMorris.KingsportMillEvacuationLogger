using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using MichaelBrandonMorris.Core.Extensions.Collection;
using MichaelBrandonMorris.Core.Extensions.List;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Controllers
{
    /// <summary>
    ///     Class UsersController.
    /// </summary>
    /// <seealso cref="Controller" />
    /// TODO Edit XML Comment Template for UsersController
    [Authorize(Roles = "Owner, Administrator")]
    public class UsersController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="UsersController" /> class.
        /// </summary>
        /// <param name="activeDirectoryColumnMapping">
        ///     The active directory column mapping.
        /// </param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="roleManager"></param>
        /// TODO Edit XML Comment Template for #ctor
        public UsersController(
            IOptions<ActiveDirectoryColumnMapping> activeDirectoryColumnMapping,
            UserManager<User> userManager,
            IEmailSender emailSender,
            RoleManager<Role> roleManager)
        {
            ActiveDirectoryColumnMapping = activeDirectoryColumnMapping.Value;
            UserManager = userManager;
            EmailSender = emailSender;
            RoleManager = roleManager;
        }

        /// <summary>
        ///     Gets the active directory column mapping.
        /// </summary>
        /// <value>The active directory column mapping.</value>
        /// TODO Edit XML Comment Template for ActiveDirectoryColumnMapping
        private ActiveDirectoryColumnMapping ActiveDirectoryColumnMapping
        {
            get;
        }

        private IEmailSender EmailSender
        {
            get;
        }

        private RoleManager<Role> RoleManager
        {
            get;
        }

        /// <summary>
        ///     Gets the user manager.
        /// </summary>
        /// <value>The user manager.</value>
        /// TODO Edit XML Comment Template for UserManager
        private UserManager<User> UserManager
        {
            get;
        }

        /// <summary>
        ///     Changes the password.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ChangePassword
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            var model = new ChangePasswordViewModel(user);
            return View(model);
        }

        /// <summary>
        ///     Changes the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);

            user.PasswordHash =
                new PasswordHasher<User>().HashPassword(
                    user,
                    model.NewPassword);

            await UserManager.UpdateAsync(user);

            await EmailSender.SendEmailAsync(
                $"{user.FirstName} {user.LastName}",
                user.Email,
                "Password Reset",
                $"Your password has been changed for you by an administrator.<br />User name: {user.Email}<br />Password: {model.NewPassword}",
                $"Your password has been changed for you by an administrator.\nUser name: {user.Email}\nPassword: {model.NewPassword}");

            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Changes the role.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ChangeRole
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            var roleNames = RoleManager.Roles.OrderBy(role => role.Index)
                .Select(role => role.Name);
            var model = new ChangeRoleViewModel(user, roleNames);
            return View(model);
        }

        /// <summary>
        ///     Changes the role.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ChangeRole
        [HttpPost]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.UserId);

            foreach (var userRole in user.Roles)
            {
                var role = await RoleManager.FindByIdAsync(userRole.RoleId);

                if (role.Name != model.RoleName)
                {
                    await UserManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            await UserManager.AddToRoleAsync(user, model.RoleName);
            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Deletes this instance.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Delete
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var users = await UserManager.Users.Include(user => user.Roles).ToListAsync();
            IList<UserViewModel> model = new List<UserViewModel>();
            
            foreach(var user in users)
            {
                var roleId = user.Roles.IsNullOrEmpty()
                    ? null
                    : user.Roles.Single().RoleId;

                var role = await RoleManager.FindByIdAsync(roleId);

                if(role == null || role.Name == "User")
                {
                    model.Add(new UserViewModel(user, string.Empty));
                }
            }

            model = model.OrderBy(user => user.LastName, user => user.FirstName);
            return View(model);
        }

        /// <summary>
        ///     Deletes the specified user ids.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Delete
        [HttpPost]
        public async Task<IActionResult> Delete(string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await UserManager.FindByIdAsync(userId);
                await UserManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await UserManager.Users.Include(Roles).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel(user, string.Empty);
            return View(model);
        }

        private Func<User, string, bool> IdEquals => (user, id) => user.Id == id;

        /// <summary>
        ///     Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = new User(model);
                await UserManager.UpdateAsync(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(model.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Indexes this instance.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Index
        public async Task<IActionResult> Index()
        {
            var users = await UserManager.Users.Include(Roles).ToListAsync();
            var model = users.Select(user => new UserViewModel(user, string.Empty));
            return View(model);
        }

        private Expression<Func<User, ICollection<IdentityUserRole<string>>>> Roles => user => user.Roles;

        /// <summary>
        ///     Uploads this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for Upload
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        /// <summary>
        ///     Uploads the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null)
            {
                ViewData["Error"] = "Select a file.";
                return View();
            }

            string json;

            using (var stream = file.OpenReadStream())
            using (var streamReader = new StreamReader(stream))
            {
                json = streamReader.ReadToEnd();
            }

            var data = JsonConvert
                .DeserializeObject<List<Dictionary<string, string>>>(json);

            foreach (var item in data)
            {
                var email = item[ActiveDirectoryColumnMapping.Email];

                if (email == null)
                {
                    continue;
                }

                var department = item[ActiveDirectoryColumnMapping.Department]
                    .Replace(
                        ActiveDirectoryColumnMapping.DepartmentPrefix,
                        string.Empty);

                var firstName = item[ActiveDirectoryColumnMapping.FirstName];

                var isActive = item[ActiveDirectoryColumnMapping.IsActive]
                    .Equals("True", StringComparison.OrdinalIgnoreCase);

                var lastName = item[ActiveDirectoryColumnMapping.LastName];

                var phoneNumber = item[ActiveDirectoryColumnMapping
                    .PhoneNumber];

                var user = await UserManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // The user does not exist.
                    if (!isActive)
                    {
                        // The user does not exist and is not active.
                        // TODO Log user not created.
                        continue;
                    }

                    // The user does not exist and is active.
                    await UserManager.CreateAsync(
                        new User
                        {
                            Department = department,
                            Email = email,
                            FirstName = firstName,
                            IsActive = true,
                            IsImported = true,
                            LastName = lastName,
                            PhoneNumber = phoneNumber,
                            UserName = email
                        });
                }
                else
                {
                    // The user exists.
                    if (!isActive
                        && !user.IsActive)
                    {
                        // The user exists and is not active in either the 
                        // database or Active Directory.
                        if (!user.Department.Equals(department)
                            || !user.FirstName.Equals(firstName)
                            || !user.LastName.Equals(lastName)
                            || !user.PhoneNumber.Equals(phoneNumber))
                        {
                            // The user exists and is not active, but there 
                            // have been changes made in Active Directory.
                            // TODO Log user not updated.
                        }

                        // The user exists, is not active, and no changes have 
                        // been made.
                        continue;
                    }

                    // The user exists and changed have been made.
                    user.Department = department;
                    user.FirstName = firstName;
                    user.IsActive = isActive;
                    user.LastName = lastName;
                    user.PhoneNumber = phoneNumber;
                    await UserManager.UpdateAsync(user);
                }
            }

            return RedirectToAction("Index");
        }

        private bool UserExists(string id)
        {
            return UserManager.FindByIdAsync(id) != null;
        }
    }
}