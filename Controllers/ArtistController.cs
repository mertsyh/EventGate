using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class ArtistController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        
        
        
        public ActionResult Index()
        {
            var artists = db.concert_details
                .Select(c => c.headliner)
                .Distinct()
                .Where(h => h != null)
                .ToList();
                
            return View(artists);
        }

        
        public ActionResult Details(string name)
        {
            if (string.IsNullOrEmpty(name)) return RedirectToAction("Index");

            ViewBag.ArtistName = name;
            
            
            var events = db.events
                .Where(e => e.concert_details != null && e.concert_details.headliner == name)
                .ToList();

            return View(events);
        }
    }
}



