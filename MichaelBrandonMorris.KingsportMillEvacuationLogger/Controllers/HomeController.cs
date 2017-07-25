using System;
using System.Collections;
using System.Collections.Generic;
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

        [HttpGet]
        public IActionResult Index()
        {
            var model = Db.Users.Select(user => new UserEvacuationStatusViewModel(user)).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IList<UserEvacuationStatusViewModel> model)
        {
            foreach (var item in model)
            {
                Db.Update(item);
            }

            await Db.SaveChangesAsync();
            return RedirectToAction("Index");
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
    }
}
