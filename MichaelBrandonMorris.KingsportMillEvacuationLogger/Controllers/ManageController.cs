using System.Linq;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.ManageViewModels
    ;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ChangePasswordViewModel = MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.ManageViewModels.ChangePasswordViewModel;

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
        ///     The external cookie scheme
        /// </summary>
        /// TODO Edit XML Comment Template for _externalCookieScheme
        private readonly string _externalCookieScheme;

        /// <summary>
        ///     The logger
        /// </summary>
        /// TODO Edit XML Comment Template for _logger
        private readonly ILogger _logger;

        /// <summary>
        ///     The sign in manager
        /// </summary>
        /// TODO Edit XML Comment Template for _signInManager
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        ///     The SMS sender
        /// </summary>
        /// TODO Edit XML Comment Template for _smsSender
        private readonly ISmsSender _smsSender;

        /// <summary>
        ///     The user manager
        /// </summary>
        /// TODO Edit XML Comment Template for _userManager
        private readonly UserManager<User> _userManager;

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="ManageController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="identityCookieOptions">
        ///     The identity cookie
        ///     options.
        /// </param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="smsSender">The SMS sender.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// TODO Edit XML Comment Template for #ctor
        public ManageController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value
                .ExternalCookieAuthenticationScheme;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        /// <summary>
        ///     Adds the phone number.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(
            AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var code =
                await _userManager.GenerateChangePhoneNumberTokenAsync(
                    user,
                    model.PhoneNumber);
            await _smsSender.SendSmsAsync(
                model.PhoneNumber,
                "Your security code is: " + code);
            return RedirectToAction(
                nameof(VerifyPhoneNumber),
                new
                {
                    model.PhoneNumber
                });
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

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

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.OldPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                _logger.LogInformation(
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(nameof(Index), "Manage");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _signInManager.SignInAsync(user, false);

            _logger.LogInformation(
                2,
                "User disabled two-factor authentication.");

            return RedirectToAction(nameof(Index), "Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(nameof(Index), "Manage");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            await _signInManager.SignInAsync(user, false);

            _logger.LogInformation(
                1,
                "User enabled two-factor authentication.");

            return RedirectToAction(nameof(Index), "Manage");
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
                        : message == ManageMessageId.SetTwoFactorSuccess
                            ? "Your two-factor authentication provider has been set."
                            : message == ManageMessageId.Error
                                ? "An error has occurred."
                                : message == ManageMessageId.AddPhoneSuccess
                                    ? "Your phone number was added."
                                    : message
                                      == ManageMessageId.RemovePhoneSuccess
                                        ? "Your phone number was removed."
                                        : "";

            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var model = new IndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
                Logins = await _userManager.GetLoginsAsync(user),
                BrowserRemembered =
                    await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };

            return View(model);
        }

        /// <summary>
        ///     Links the login.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            await HttpContext.Authentication
                .SignOutAsync(_externalCookieScheme);

            var redirectUrl = Url.Action(nameof(LinkLoginCallback), "Manage");

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(
                    provider,
                    redirectUrl,
                    _userManager.GetUserId(User));

            return Challenge(properties, provider);
        }

        /// <summary>
        ///     Links the login callback.
        /// </summary>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var info =
                await _signInManager.GetExternalLoginInfoAsync(
                    await _userManager.GetUserIdAsync(user));

            if (info == null)
            {
                return RedirectToAction(
                    nameof(ManageLogins),
                    new
                    {
                        Message = ManageMessageId.Error
                    });
            }

            var result = await _userManager.AddLoginAsync(user, info);
            var message = ManageMessageId.Error;

            if (!result.Succeeded)
            {
                return RedirectToAction(
                    nameof(ManageLogins),
                    new
                    {
                        Message = message
                    });
            }

            message = ManageMessageId.AddLoginSuccess;

            await HttpContext.Authentication
                .SignOutAsync(_externalCookieScheme);

            return RedirectToAction(
                nameof(ManageLogins),
                new
                {
                    Message = message
                });
        }


        /// <summary>
        ///     Manages the logins.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins(
            ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess
                    ? "The external login was removed."
                    : message == ManageMessageId.AddLoginSuccess
                        ? "The external login was added."
                        : message == ManageMessageId.Error
                            ? "An error has occurred."
                            : "";

            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var userLogins = await _userManager.GetLoginsAsync(user);

            var otherLogins = _signInManager.GetExternalAuthenticationSchemes()
                .Where(
                    auth => userLogins.All(
                        ul => auth.AuthenticationScheme != ul.LoginProvider))
                .ToList();

            ViewData["ShowRemoveButton"] =
                user.PasswordHash != null || userLogins.Count > 1;

            return View(
                new ManageLoginsViewModel
                {
                    CurrentLogins = userLogins,
                    OtherLogins = otherLogins
                });
        }

        /// <summary>
        ///     Removes the login.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(
            RemoveLoginViewModel account)
        {
            ManageMessageId? message = ManageMessageId.Error;
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return RedirectToAction(
                    nameof(ManageLogins),
                    new
                    {
                        Message = message
                    });
            }

            var result = await _userManager.RemoveLoginAsync(
                user,
                account.LoginProvider,
                account.ProviderKey);

            if (!result.Succeeded)
            {
                return RedirectToAction(
                    nameof(ManageLogins),
                    new
                    {
                        Message = message
                    });
            }

            await _signInManager.SignInAsync(user, false);
            message = ManageMessageId.RemoveLoginSuccess;

            return RedirectToAction(
                nameof(ManageLogins),
                new
                {
                    Message = message
                });
        }


        /// <summary>
        ///     Removes the phone number.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
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

            var result = await _userManager.SetPhoneNumberAsync(user, null);

            if (!result.Succeeded)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        Message = ManageMessageId.Error
                    });
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(
                nameof(Index),
                new
                {
                    Message = ManageMessageId.RemovePhoneSuccess
                });
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
                await _userManager.AddPasswordAsync(user, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

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

        /// <summary>
        ///     Verifies the phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            return phoneNumber == null
                ? View("Error")
                : View(
                    new VerifyPhoneNumberViewModel
                    {
                        PhoneNumber = phoneNumber
                    });
        }

        /// <summary>
        ///     Verifies the phone number.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(
            VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(
                    user,
                    model.PhoneNumber,
                    model.Code);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction(
                        nameof(Index),
                        new
                        {
                            Message = ManageMessageId.AddPhoneSuccess
                        });
                }
            }

            ModelState.AddModelError(
                string.Empty,
                "Failed to verify phone number");

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
            ///     The add phone success
            /// </summary>
            /// TODO Edit XML Comment Template for AddPhoneSuccess
            AddPhoneSuccess,

            /// <summary>
            ///     The add login success
            /// </summary>
            /// TODO Edit XML Comment Template for AddLoginSuccess
            AddLoginSuccess,

            /// <summary>
            ///     The change password success
            /// </summary>
            /// TODO Edit XML Comment Template for ChangePasswordSuccess
            ChangePasswordSuccess,

            /// <summary>
            ///     The set two factor success
            /// </summary>
            /// TODO Edit XML Comment Template for SetTwoFactorSuccess
            SetTwoFactorSuccess,

            /// <summary>
            ///     The set password success
            /// </summary>
            /// TODO Edit XML Comment Template for SetPasswordSuccess
            SetPasswordSuccess,

            /// <summary>
            ///     The remove login success
            /// </summary>
            /// TODO Edit XML Comment Template for RemoveLoginSuccess
            RemoveLoginSuccess,

            /// <summary>
            ///     The remove phone success
            /// </summary>
            /// TODO Edit XML Comment Template for RemovePhoneSuccess
            RemovePhoneSuccess,

            /// <summary>
            ///     The error
            /// </summary>
            /// TODO Edit XML Comment Template for Error
            Error
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}