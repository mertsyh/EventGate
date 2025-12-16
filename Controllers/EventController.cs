using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class EventController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        // -------------------- HOME / ALL EVENTS --------------------
        public ActionResult Index()
        {
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();

            DateTime now = DateTime.Now;

            var events = db.events
                .Where(e => e.status != "deleted")
                .ToList()
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = GetNextOrFirstPerformanceDate(e.performances, now),
                    Venue = e.performances.FirstOrDefault()?.venues.name,
                    City = e.performances.FirstOrDefault()?.venues.cities.name,
                    Price = e.performances.SelectMany(p => p.price_tiers).OrderBy(t => t.price).FirstOrDefault()?.price,
                    ImageUrl = e.poster_url
                })
                .ToList();

            return View(events);
        }

        // -------------------- CINEMA --------------------
        public ActionResult Cinema()
        {
            int category = 2;

            ViewBag.CategoryId = category;
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();

            DateTime now = DateTime.Now;

            var movies = db.events
                .Where(e => e.category_id == category && e.status != "deleted")
                .ToList()   // First get from DB, then map
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = GetNextOrFirstPerformanceDate(e.performances, now),
                    Venue = e.performances.FirstOrDefault()?.venues.name,
                    City = e.performances.FirstOrDefault()?.venues.cities.name,
                    Price = e.performances.SelectMany(p => p.price_tiers).OrderBy(t => t.price).FirstOrDefault()?.price,
                    ImageUrl = e.poster_url
                })
                .ToList();

            return View(movies);
        }

        // -------------------- MUSIC --------------------
        public ActionResult Music()
        {
            int category = 1;

            ViewBag.CategoryId = category;
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();

            DateTime now = DateTime.Now;

            var concerts = db.events
                .Where(e => e.category_id == category && e.status != "deleted")
                .ToList()
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = GetNextOrFirstPerformanceDate(e.performances, now),
                    Venue = e.performances.FirstOrDefault()?.venues.name,
                    City = e.performances.FirstOrDefault()?.venues.cities.name,
                    Price = e.performances.SelectMany(p => p.price_tiers).OrderBy(t => t.price).FirstOrDefault()?.price,
                    ImageUrl = e.poster_url
                })
                .ToList();

            return View(concerts);
        }

        // -------------------- DETAILS --------------------
        public ActionResult Details(int id)
        {
            var eventRaw = db.events
                .Where(e => e.id == id)
                .FirstOrDefault();

            if (eventRaw == null)
                return HttpNotFound();

            // Deleted event sayfasına sadece admin veya etkinlik sahibi organizer erişebilsin
            if (eventRaw.status == "deleted")
            {
                bool isAdmin = Session["AdminID"] != null;
                long? organizerId = null;
                if (Session["OrganizerID"] != null)
                {
                    organizerId = Convert.ToInt64(Session["OrganizerID"]);
                }

                bool isOwner = organizerId.HasValue && eventRaw.organizer_id == organizerId.Value;

                if (!isAdmin && !isOwner)
                {
                    return HttpNotFound();
                }
            }

            DateTime now = DateTime.Now;
            var nextPerf = eventRaw.performances
                .Where(p => p.start_datetime >= now && p.status != "cancelled")
                .OrderBy(p => p.start_datetime)
                .FirstOrDefault();

            bool hasUpcoming = nextPerf != null;
            if (!hasUpcoming)
            {
                nextPerf = eventRaw.performances
                    .OrderBy(p => p.start_datetime)
                    .FirstOrDefault();
            }

            var evt = new EventDetailViewModel
            {
                EventId = eventRaw.id,
                Title = eventRaw.title,
                Description = eventRaw.description,
                ImageUrl = eventRaw.poster_url,
                Venue = eventRaw.performances.FirstOrDefault()?.venues.name,
                City = eventRaw.performances.FirstOrDefault()?.venues.cities.name,
                Date = nextPerf?.start_datetime,
                Price = eventRaw.performances.SelectMany(p => p.price_tiers).OrderBy(t => t.price).FirstOrDefault()?.price
            };

            ViewBag.IsPastEvent = !hasUpcoming;

            return View(evt);
        }

        // -------------------- AJAX FILTER --------------------
        public ActionResult Filter(int? cityId, int? venueId, string dateFilter, int? categoryId)
        {
            // Load data into memory to avoid EF translation issues
            var eventsQuery = db.events
                .Where(e => e.status != "deleted" && (!categoryId.HasValue || e.category_id == categoryId))
                .ToList();

            // City
            if (cityId.HasValue)
                eventsQuery = eventsQuery
                    .Where(e => e.performances.Any(p => p.venues.city_id == cityId))
                    .ToList();

            // Venue
            if (venueId.HasValue)
                eventsQuery = eventsQuery
                    .Where(e => e.performances.Any(p => p.venue_id == venueId))
                    .ToList();

            // Date filter (now works in memory, not in EF)
            DateTime now = DateTime.Now;

            if (dateFilter == "today")
            {
                eventsQuery = eventsQuery
                    .Where(e => e.performances.Any(p => p.start_datetime.Date == now.Date))
                    .ToList();
            }
            else if (dateFilter == "week")
            {
                DateTime week = now.AddDays(7);

                eventsQuery = eventsQuery
                    .Where(e => e.performances.Any(p => p.start_datetime >= now &&
                                                        p.start_datetime <= week))
                    .ToList();
            }
            else if (dateFilter == "month")
            {
                DateTime month = now.AddMonths(1);

                eventsQuery = eventsQuery
                    .Where(e => e.performances.Any(p => p.start_datetime >= now &&
                                                        p.start_datetime <= month))
                    .ToList();
            }

            // Result — Entity to ViewModel
            var result = eventsQuery
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = GetNextOrFirstPerformanceDate(e.performances, now),
                    Venue = e.performances.FirstOrDefault()?.venues.name,
                    City = e.performances.FirstOrDefault()?.venues.cities.name,
                    Price = e.performances.SelectMany(p => p.price_tiers).OrderBy(t => t.price).FirstOrDefault()?.price,
                    ImageUrl = e.poster_url 
                })
                .ToList();

            return PartialView("_EventCards", result);
        }

        // Helper: choose next upcoming performance date if exists, otherwise first performance
        private static System.DateTime? GetNextOrFirstPerformanceDate(
            System.Collections.Generic.ICollection<performances> perfs,
            System.DateTime now)
        {
            if (perfs == null || !perfs.Any()) return null;

            var upcoming = perfs
                .Where(p => p.start_datetime >= now && p.status != "cancelled")
                .OrderBy(p => p.start_datetime)
                .FirstOrDefault();

            if (upcoming != null) return upcoming.start_datetime;

            return perfs
                .OrderBy(p => p.start_datetime)
                .FirstOrDefault()
                ?.start_datetime;
        }

        public ActionResult Theater()
        {
            ViewBag.CategoryId = 3;
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();
            return View();
        }
        public ActionResult Search(int? cityId, int? categoryId, int? venueId, string date)
        {
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();
            ViewBag.categories = db.categories.ToList();

            ViewBag.CityId = cityId;
            ViewBag.CategoryId = categoryId;
            ViewBag.VenueId = venueId;
            ViewBag.Date = date;

            return View(); // Eventpages layout + rightcontent var
        }


        [HttpPost]
        public JsonResult SearchEvents(string name)
        {
            var events = db.events
                .Where(e => e.title.Contains(name))
                .Select(e => new
                {
                    e.id,
                    e.title,
                    e.poster_url,
                    e.description
                })
                .ToList();

            return Json(events, JsonRequestBehavior.AllowGet);
        }

    }
}
