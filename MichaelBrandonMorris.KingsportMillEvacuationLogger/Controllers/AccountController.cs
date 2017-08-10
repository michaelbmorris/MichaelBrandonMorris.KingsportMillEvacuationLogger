using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.
    AccountViewModels;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Controllers
{
    /// <summary>
    ///     Class AccountController.
    /// </summary>
    /// <seealso cref="Controller" />
    /// TODO Edit XML Comment Template for AccountController
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        ///     The email sender
        /// </summary>
        /// TODO Edit XML Comment Template for EmailSender
        private IEmailSender EmailSender
        {
            get;
        }

        /// <summary>
        ///     The external cookie scheme
        /// </summary>
        /// TODO Edit XML Comment Template for ExternalCookieScheme
        private string ExternalCookieScheme
        {
            get;
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
        ///     Initializes a new instance of the
        ///     <see cref="AccountController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="identityCookieOptions">
        ///     The identity cookie
        ///     options.
        /// </param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// TODO Edit XML Comment Template for #ctor
        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory)
        {
            UserManager = userManager;
            SignInManager = signInManager;

            ExternalCookieScheme = identityCookieOptions.Value
                .ExternalCookieAuthenticationScheme;

            EmailSender = emailSender;
            Logger = loggerFactory.CreateLogger<AccountController>();
        }

        /// <summary>
        ///     Accesses the denied.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        ///     Confirms the email.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The code.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(
            string userId,
            string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var result = await UserManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        ///     Forgots the password.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        ///     Forgots the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null
                || !await UserManager.IsEmailConfirmedAsync(user))
            {
                return View("ForgotPasswordConfirmation");
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                nameof(ResetPassword),
                "Account",
                new
                {
                    userId = user.Id,
                    code
                },
                HttpContext.Request.Scheme);

            await EmailSender.SendEmailAsync(
                string.Empty,
                model.Email,
                "Reset Password",
                $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.",
                $"Please reset your password by pasting the following link in your browser: {callbackUrl}");

            return View("ForgotPasswordConfirmation");
        }

        /// <summary>
        ///     Forgots the password confirmation.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        ///     Logins the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.Authentication
                .SignOutAsync(ExternalCookieScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        ///     Logins the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false);

            if (result.Succeeded)
            {
                Logger.LogInformation(1, "User logged in.");
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                Logger.LogWarning(2, "User account locked out.");
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        /// <summary>
        ///     Logouts this instance.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            Logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        ///     Resets the password.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        /// <summary>
        ///     Resets the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction(
                    nameof(ResetPasswordConfirmation),
                    "Account");
            }

            var result =
                await UserManager.ResetPasswordAsync(
                    user,
                    model.Code,
                    model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(
                    nameof(ResetPasswordConfirmation),
                    "Account");
            }

            AddErrors(result);
            return View();
        }

        /// <summary>
        ///     Resets the password confirmation.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #endregion
    }
}