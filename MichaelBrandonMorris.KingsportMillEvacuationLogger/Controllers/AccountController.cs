using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models.
    AccountViewModels;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        /// TODO Edit XML Comment Template for _emailSender
        private readonly IEmailSender _emailSender;

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
        ///     <see cref="AccountController" /> class.
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
        public AccountController(
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

            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
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
            if (userId == null
                || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        ///     Externals the login.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(
            string provider,
            string returnUrl = null)
        {
            var redirectUrl = Url.Action(
                nameof(ExternalLoginCallback),
                "Account",
                new
                {
                    ReturnUrl = returnUrl
                });

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(
                    provider,
                    redirectUrl);

            return Challenge(properties, provider);
        }

        /// <summary>
        ///     Externals the login callback.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="remoteError">The remote error.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(
            string returnUrl = null,
            string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                false);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    5,
                    "User logged in with {Name} provider.",
                    info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(
                    nameof(SendCode),
                    new
                    {
                        ReturnUrl = returnUrl
                    });
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            return View(
                "ExternalLoginConfirmation",
                new ExternalLoginConfirmationViewModel
                {
                    Email = email
                });
        }

        /// <summary>
        ///     Externals the login confirmation.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(
            ExternalLoginConfirmationViewModel model,
            string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        _logger.LogInformation(
                            6,
                            "User created an account using {Name} provider.",
                            info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
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

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null
                || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return View("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                nameof(ResetPassword),
                "Account",
                new
                {
                    userId = user.Id,
                    code
                },
                HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
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
                .SignOutAsync(_externalCookieScheme);

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

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false);
            if (result.Succeeded)
            {
                _logger.LogInformation(1, "User logged in.");
                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(
                    nameof(SendCode),
                    new
                    {
                        ReturnUrl = returnUrl,
                        model.RememberMe
                    });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, "User account locked out.");
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
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        ///     Registers the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        ///     Registers the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Department = model.Department,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var code = await _userManager
                    .GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Account",
                    new
                    {
                        userId = user.Id,
                        code
                    },
                    HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Confirm your account",
                    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                _logger.LogInformation(
                    3,
                    "User created a new account with password.");

                return RedirectToLocal(returnUrl);
            }

            AddErrors(result);

            return View(model);
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

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction(
                    nameof(ResetPasswordConfirmation),
                    "Account");
            }

            var result =
                await _userManager.ResetPasswordAsync(
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

        /// <summary>
        ///     Sends the code.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="rememberMe">
        ///     if set to <c>true</c> [remember
        ///     me].
        /// </param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(
            string returnUrl = null,
            bool rememberMe = false)
        {
            var user =
                await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var userFactors =
                await _userManager.GetValidTwoFactorProvidersAsync(user);

            var factorOptions = userFactors.Select(
                    purpose => new SelectListItem
                    {
                        Text = purpose,
                        Value = purpose
                    })
                .ToList();

            return View(
                new SendCodeViewModel
                {
                    Providers = factorOptions,
                    ReturnUrl = returnUrl,
                    RememberMe = rememberMe
                });
        }

        /// <summary>
        ///     Sends the code.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// TODO Edit XML Comment Template for SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user =
                await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var code =
                await _userManager.GenerateTwoFactorTokenAsync(
                    user,
                    model.SelectedProvider);

            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;

            switch (model.SelectedProvider)
            {
                case "Email":
                    await _emailSender.SendEmailAsync(
                        await _userManager.GetEmailAsync(user),
                        "Security Code",
                        message);
                    break;
                case "Phone":
                    await _smsSender.SendSmsAsync(
                        await _userManager.GetPhoneNumberAsync(user),
                        message);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return RedirectToAction(
                nameof(VerifyCode),
                new
                {
                    Provider = model.SelectedProvider,
                    model.ReturnUrl,
                    model.RememberMe
                });
        }

        /// <summary>
        ///     Verifies the code.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="rememberMe">
        ///     if set to <c>true</c> [remember
        ///     me].
        /// </param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(
            string provider,
            bool rememberMe,
            string returnUrl = null)
        {
            var user =
                await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            return View(
                new VerifyCodeViewModel
                {
                    Provider = provider,
                    ReturnUrl = returnUrl,
                    RememberMe = rememberMe
                });
        }

        /// <summary>
        ///     Verifies the code.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.TwoFactorSignInAsync(
                model.Provider,
                model.Code,
                model.RememberMe,
                model.RememberBrowser);

            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid code.");
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