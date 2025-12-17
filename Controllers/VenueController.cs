using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class VenueController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        
        public ActionResult Index()
        {
            var venues = db.venues.ToList();
            return View(venues);
        }

        
        public ActionResult Details(int id)
        {
            var venue = db.venues.Find(id);
            if (venue == null) return HttpNotFound();
            return View(venue);
        }
    }
}



