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

        
        public ActionResult Index()
        {
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();

            DateTime now = DateTime.Now;

            var events = db.events
                .Where(e => e.status != "deleted")
                .ToList()
                .Where(e => HasUpcomingPerformances(e.performances, now))
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

        
        public ActionResult Cinema()
        {
            int category = 2;

            ViewBag.CategoryId = category;
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();

            DateTime now = DateTime.Now;

            var movies = db.events
                .Where(e => e.category_id == category && e.status != "deleted")
                .ToList()   
                .Where(e => HasUpcomingPerformances(e.performances, now))
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
                .Where(e => HasUpcomingPerformances(e.performances, now))
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

        
        public ActionResult Details(int id)
        {
            var eventRaw = db.events
                .Where(e => e.id == id)
                .FirstOrDefault();

            if (eventRaw == null)
                return HttpNotFound();

            
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

        
        public ActionResult Filter(int? cityId, int? venueId, string dateFilter, int? categoryId)
        {
            DateTime now = DateTime.Now;
            
            
            var eventsQuery = db.events
                .Where(e => e.status != "deleted" && (!categoryId.HasValue || e.category_id == categoryId))
                .ToList()
                .Where(e => HasUpcomingPerformances(e.performances, now))
                .ToList();

            
            if (cityId.HasValue)
                eventsQuery = eventsQuery
                    .Where(e => e.performances.Any(p => p.venues.city_id == cityId))
                    .ToList();

            
            if (venueId.HasValue)
                eventsQuery = eventsQuery
                    .Where(e => e.performances.Any(p => p.venue_id == venueId))
                    .ToList();

            
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

            
            var result = eventsQuery
                .Select(e =>
                {
                    var perf = e.performances
      .Where(p =>
          p.status != "cancelled" &&
          (p.end_datetime.HasValue ? p.end_datetime.Value > now : p.start_datetime > now) &&
          (dateFilter != "week" || (p.start_datetime >= now && p.start_datetime <= now.AddDays(7))) &&
          (dateFilter != "month" || (p.start_datetime >= now && p.start_datetime <= now.AddMonths(1))) &&
          (dateFilter != "today" || p.start_datetime.Date == now.Date)
      )
      .OrderBy(p => p.start_datetime)
      .FirstOrDefault();


                    return new EventCardViewModel
                    {
                        EventId = e.id,
                        Title = e.title,
                        StartDate = perf?.start_datetime,
                        Venue = perf?.venues.name,
                        City = perf?.venues.cities.name,
                        Price = perf?.price_tiers.OrderBy(t => t.price).FirstOrDefault()?.price,
                        ImageUrl = e.poster_url
                    };
                })
                .ToList();
            return PartialView("_EventCards", result);

        }

        
        private static bool HasUpcomingPerformances(
            System.Collections.Generic.ICollection<performances> perfs,
            System.DateTime now)
        {
            if (perfs == null || !perfs.Any()) return false;

            return perfs.Any(p =>
            {
                if (p.status == "cancelled") return false;
                
                
                if (p.end_datetime.HasValue)
                {
                    return p.end_datetime.Value > now;
                }
                
                
                return p.start_datetime > now;
            });
        }

        
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
            int category = 3;

            ViewBag.CategoryId = category;
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();

            DateTime now = DateTime.Now;

            var theaterEvents = db.events
                .Where(e => e.category_id == category && e.status != "deleted")
                .ToList()
                .Where(e => HasUpcomingPerformances(e.performances, now))
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

            return View(theaterEvents);
        }
        public ActionResult Search(
      string q,               
      int? cityId,
      int? categoryId,
      int? venueId,
      string date
  )
        {
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();
            ViewBag.categories = db.categories.ToList();

            DateTime now = DateTime.Now;

            var query = db.events.AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(e => e.title.Contains(q));

            if (categoryId.HasValue)
                query = query.Where(e => e.category_id == categoryId);

            if (cityId.HasValue)
                query = query.Where(e => e.performances.Any(p => p.venues.city_id == cityId));

            if (venueId.HasValue)
                query = query.Where(e => e.performances.Any(p => p.venue_id == venueId));

            if (!string.IsNullOrEmpty(date) &&
                DateTime.TryParse(date, out DateTime selectedDate))
            {
                query = query.Where(e =>
                    e.performances.Any(p => p.start_datetime.Date == selectedDate.Date));
            }

            var result = query
                .ToList()
                .Where(e => HasUpcomingPerformances(e.performances, now))
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = GetNextOrFirstPerformanceDate(e.performances, now),
                    Venue = e.performances.FirstOrDefault()?.venues.name,
                    City = e.performances.FirstOrDefault()?.venues.cities.name,
                    Price = e.performances.SelectMany(p => p.price_tiers)
                                          .OrderBy(t => t.price)
                                          .FirstOrDefault()?.price,
                    ImageUrl = e.poster_url
                })
                .ToList();

            return View(result);
        }




        [HttpPost]
        public JsonResult SearchEvents(string name)
        {
            DateTime now = DateTime.Now;
            
            var events = db.events
                .Where(e => e.title.Contains(name) && e.status != "deleted")
                .ToList()
                .Where(e => HasUpcomingPerformances(e.performances, now))
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


