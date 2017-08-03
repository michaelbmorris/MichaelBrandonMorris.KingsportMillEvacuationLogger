using System.Linq;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Controllers
{
    /// <summary>
    ///     Class HomeController.
    /// </summary>
    /// <seealso cref="Controller" />
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
        /// Gets the user manager.
        /// </summary>
        /// <value>The user manager.</value>
        /// TODO Edit XML Comment Template for UserManager
        private UserManager<User> UserManager
        {
            get;
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
                .Select(user => new UserViewModel(user));

            model = model.OrderBy(user => user.Status)
                .ThenBy(user => user.Department)
                .ThenBy(user => user.LastName)
                .ThenBy(user => user.FirstName);

            return View(model.ToList());
        }

        [HttpGet]
        public ActionResult Reset()
        {
            return View();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Reset
        [ActionName("Reset")]
        [HttpPost]
        public async Task<ActionResult> ResetConfirmed()
        {
            foreach (var user in await UserManager.Users.ToListAsync())
            {
                user.Status = UserEvacuationStatus.Unknown;
                await UserManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Updates the status.
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