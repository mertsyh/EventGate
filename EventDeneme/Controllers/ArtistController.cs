using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class ArtistController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        // GET: Artist (artists.html)
        // Since we don't have an Artists table, we'll list distinct 'headliners' from concert_details
        // and 'organizers' as a fallback or separate list.
        public ActionResult Index()
        {
            var artists = db.concert_details
                .Select(c => c.headliner)
                .Distinct()
                .Where(h => h != null)
                .ToList();
                
            return View(artists);
        }

        // GET: Artist/Details?name=ArtistName (artist-details.html)
        public ActionResult Details(string name)
        {
            if (string.IsNullOrEmpty(name)) return RedirectToAction("Index");

            ViewBag.ArtistName = name;
            
            // Find events featuring this artist
            var events = db.events
                .Where(e => e.concert_details != null && e.concert_details.headliner == name)
                .ToList();

            return View(events);
        }
    }
}

