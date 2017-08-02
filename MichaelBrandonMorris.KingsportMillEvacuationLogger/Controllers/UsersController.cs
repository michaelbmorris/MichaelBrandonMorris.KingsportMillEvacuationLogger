using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Data;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext Db
        {
            get;
        }

        private ActiveDirectoryColumnMapping ActiveDirectoryColumnMapping
        {
            get;
        }

        private UserManager<User> UserManager
        { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="activeDirectoryColumnMapping">The active directory column mapping.</param>
        /// <param name="userManager">The user manager.</param>
        /// TODO Edit XML Comment Template for #ctor
        public UsersController(ApplicationDbContext db, IOptions<ActiveDirectoryColumnMapping> activeDirectoryColumnMapping, UserManager<User> userManager)
        {
            Db = db;
            ActiveDirectoryColumnMapping = activeDirectoryColumnMapping.Value;

            Debug.WriteLine(ActiveDirectoryColumnMapping.Email);
            UserManager = userManager;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FirstName,LastName,Id,UserName,Email")] User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            Db.Add(user);
            await Db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// Uploads the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// TODO Edit XML Comment Template for Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null)
            {
                return View();
            }

            string json;

            using (var stream = file.OpenReadStream())
            using(var streamReader = new StreamReader(stream))
            {
                json = streamReader.ReadToEnd();
            }

            var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

            foreach(var item in data)
            {
                var user = new User
                {
                    Email = item[ActiveDirectoryColumnMapping.Email],
                    FirstName = item[ActiveDirectoryColumnMapping.FirstName],
                    LastName = item[ActiveDirectoryColumnMapping.LastName],
                    PhoneNumber = item[ActiveDirectoryColumnMapping.PhoneNumber],
                    Department = item[ActiveDirectoryColumnMapping.Department],
                    IsActive = item[ActiveDirectoryColumnMapping.IsActive].Equals("True", System.StringComparison.OrdinalIgnoreCase),
                    UserName = item[ActiveDirectoryColumnMapping.Email]
                };

                if(user.Email == null)
                {
                    continue;
                }

                var currentUser = await UserManager.FindByEmailAsync(user.Email);

                if(currentUser == null && user.IsActive)
                {
                    await UserManager.CreateAsync(user);
                }
                else if(currentUser != null)
                {
                    await UserManager.UpdateAsync(user);
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =
                await Db.User.SingleOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user =
                await Db.User.SingleOrDefaultAsync(m => m.Id == id);
            Db.User.Remove(user);
            await Db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =
                await Db.User.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await Db.User.SingleOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            [Bind("FirstName,LastName,Id,Email")]
            UserViewModel model)
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
                Db.UpdateUser(model);
                await Db.SaveChangesAsync();
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

        public async Task<IActionResult> Index()
        {
            return View(await Db.User.ToListAsync());
        }

        private bool UserExists(string id)
        {
            return Db.User.Any(e => e.Id == id);
        }
    }
}