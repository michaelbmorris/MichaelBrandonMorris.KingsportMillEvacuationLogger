using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.ManageViewModels
    ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChangePasswordViewModel =
    MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.ManageViewModels.
    ChangePasswordViewModel;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Controllers
{
    /// <summary>
    ///     Class ManageController.
    /// </summary>
    /// <seealso cref="Controller" />
    /// TODO Edit XML Comment Template for ManageController
    [Authorize]
    public class ManageController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="ManageController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// TODO Edit XML Comment Template for #ctor
        public ManageController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILoggerFactory loggerFactory)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Logger = loggerFactory.CreateLogger<ManageController>();
        }

        /// <summary>
        ///     The logger
        /// </summary>
        /// TODO Edit XML Comment Template for Logger
        private ILogger Logger
        {
            get;
        }

        /// <summary>
        ///     The sign in manager
        /// </summary>
        /// TODO Edit XML Comment Template for SignInManager
        private SignInManager<User> SignInManager
        {
            get;
        }

        /// <summary>
        ///     The user manager
        /// </summary>
        /// TODO Edit XML Comment Template for UserManager
        private UserManager<User> UserManager
        {
            get;
        }

        /// <summary>
        ///     Changes the password.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        ///     Changes the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        Message = ManageMessageId.Error
                    });
            }

            var result = await UserManager.ChangePasswordAsync(
                user,
                model.OldPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, false);

                Logger.LogInformation(
                    3,
                    "User changed their password successfully.");

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        Message = ManageMessageId.ChangePasswordSuccess
                    });
            }

            AddErrors(result);
            return View(model);
        }

        /// <summary>
        ///     Indexes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess
                    ? "Your password has been changed."
                    : message == ManageMessageId.SetPasswordSuccess
                        ? "Your password has been set."
                        : message == ManageMessageId.Error
                            ? "An error has occurred."
                            : "";

            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var model = new IndexViewModel
            {
                HasPassword = await UserManager.HasPasswordAsync(user),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(user),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(user),
                Logins = await UserManager.GetLoginsAsync(user),
                BrowserRemembered =
                    await SignInManager.IsTwoFactorClientRememberedAsync(user)
            };

            return View(model);
        }

        /// <summary>
        ///     Sets the password.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        /// <summary>
        ///     Sets the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        Message = ManageMessageId.Error
                    });
            }

            var result =
                await UserManager.AddPasswordAsync(user, model.NewPassword);

            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, false);

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        Message = ManageMessageId.SetPasswordSuccess
                    });
            }

            AddErrors(result);
            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        /// <summary>
        ///     Enum ManageMessageId
        /// </summary>
        /// TODO Edit XML Comment Template for ManageMessageId
        public enum ManageMessageId
        {
            /// <summary>
            ///     The change password success
            /// </summary>
            /// TODO Edit XML Comment Template for ChangePasswordSuccess
            ChangePasswordSuccess,

            /// <summary>
            ///     The set password success
            /// </summary>
            /// TODO Edit XML Comment Template for SetPasswordSuccess
            SetPasswordSuccess,

            /// <summary>
            ///     The error
            /// </summary>
            /// TODO Edit XML Comment Template for Error
            Error
        }

        /// <summary>
        ///     Gets the current user asynchronous.
        /// </summary>
        /// <returns>Task&lt;User&gt;.</returns>
        /// TODO Edit XML Comment Template for GetCurrentUserAsync
        private Task<User> GetCurrentUserAsync()
        {
            return UserManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}