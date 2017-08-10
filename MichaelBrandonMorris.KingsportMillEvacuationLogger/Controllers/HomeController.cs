using System.Linq;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Controllers
{
    /// <summary>
    ///     Class HomeController.
    /// </summary>
    /// <seealso cref="Controller" />
    [Authorize(Roles = "Owner, Administrator, Security, Commander")]
    public class HomeController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// TODO Edit XML Comment Template for #ctor
        public HomeController(UserManager<User> userManager)
        {
            UserManager = userManager;
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
        ///     Admins this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        /// TODO Edit XML Comment Template for Admin
        public ActionResult Admin()
        {
            return View();
        }

        /// <summary>
        ///     Errors this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for Error
        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        ///     Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        /// TODO Edit XML Comment Template for Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = (await UserManager.Users.ToListAsync())
                .Where(user => user.IsActive)
                .Select(user => new UserViewModel(user, string.Empty));

            model = model
                .OrderBy(user => UserEvacuationStatusMap.Map[user.Status])
                .ThenBy(user => user.Department)
                .ThenBy(user => user.LastName)
                .ThenBy(user => user.FirstName);

            return View(model.ToList());
        }

        /// <summary>
        ///     Indexes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Index
        [HttpPost]
        public async Task<IActionResult> Index(UserViewModel[] model)
        {
            foreach (var item in model)
            {
                if (item.Status == 0)
                {
                    continue;
                }

                var user = await UserManager.FindByIdAsync(item.Id);

                if (user.Status == item.Status)
                {
                    continue;
                }

                user.Status = item.Status;
                await UserManager.UpdateAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Owner, Administrator, Security")]
        [HttpGet]
        public ActionResult Reset()
        {
            return View();
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Reset
        [Authorize(Roles = "Owner, Administrator, Security")]
        [ActionName("Reset")]
        [HttpPost]
        public async Task<ActionResult> ResetConfirmed()
        {
            foreach (var user in await UserManager.Users.ToListAsync())
            {
                if (user.Status == 0)
                {
                    continue;
                }

                user.Status = 0;
                await UserManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Updates the status.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="status">The status.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for UpdateStatus
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(
            string id,
            UserEvacuationStatus status)
        {
            var user = await UserManager.FindByIdAsync(id);
            user.Status = status;
            var result = await UserManager.UpdateAsync(user);

            return Json(
                new
                {
                    failure = !result.Succeeded
                });
        }
    }
}