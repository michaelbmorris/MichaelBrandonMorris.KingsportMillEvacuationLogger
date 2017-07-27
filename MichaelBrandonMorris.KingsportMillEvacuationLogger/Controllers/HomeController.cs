using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Data;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using Microsoft.AspNetCore.Mvc;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ApplicationDbContext db)
        {
            Db = db;
        }

        private ApplicationDbContext Db
        {
            get;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = Db.Users.ToList()
                .Select(user => new UserEvacuationStatusViewModel(user))
                .Where(user => user.Status != UserEvacuationStatus.Inactive)
                .OrderBy(user => user.Status)
                .ThenBy(user => user.Department)
                .ThenBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            IList<UserEvacuationStatusViewModel> model)
        {
            foreach (var item in model)
            {
                Debug.WriteLine(item.Id);
                Debug.WriteLine(item.Status);
                Db.UpdateUserStatus(item);
            }

            await Db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}