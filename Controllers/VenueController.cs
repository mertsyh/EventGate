using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class VenueController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        // GET: Venue (venues.html)
        public ActionResult Index()
        {
            var venues = db.venues.ToList();
            return View(venues);
        }

        // GET: Venue/Details/5 (venue-details.html)
        public ActionResult Details(int id)
        {
            var venue = db.venues.Find(id);
            if (venue == null) return HttpNotFound();
            return View(venue);
        }
    }
}

