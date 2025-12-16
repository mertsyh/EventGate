using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class CategoryController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        // GET: Category (categories.html)
        public ActionResult Index()
        {
            var categories = db.categories.ToList();
            return View(categories);
        }
    }
}

