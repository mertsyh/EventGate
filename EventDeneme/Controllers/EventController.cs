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

            var events = db.events
                .ToList()
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = e.performances.OrderBy(p => p.start_datetime).FirstOrDefault()?.start_datetime,
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

            var movies = db.events
                .Where(e => e.category_id == category)
                .ToList()   // ÖNCE DB'DEN AL → SONRA MAPLE
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = e.performances.OrderBy(p => p.start_datetime).FirstOrDefault()?.start_datetime,
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

            var concerts = db.events
                .Where(e => e.category_id == category)
                .ToList()
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = e.performances.OrderBy(p => p.start_datetime).FirstOrDefault()?.start_datetime,
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

            var evt = new EventDetailViewModel
            {
                EventId = eventRaw.id,
                Title = eventRaw.title,
                Description = eventRaw.description,
                ImageUrl = eventRaw.poster_url,
                Venue = eventRaw.performances.FirstOrDefault()?.venues.name,
                City = eventRaw.performances.FirstOrDefault()?.venues.cities.name,
                Date = eventRaw.performances.OrderBy(p => p.start_datetime).FirstOrDefault()?.start_datetime,
                Price = eventRaw.performances.SelectMany(p => p.price_tiers).OrderBy(t => t.price).FirstOrDefault()?.price
            };

            ViewBag.IsPastEvent = evt.Date < DateTime.Now;

            return View(evt);
        }

        // -------------------- AJAX FILTER --------------------
        public ActionResult Filter(int? cityId, int? venueId, string dateFilter, int? categoryId)
        {
            // Veri Çek — EF Hatası Olmasın Diye Tamamını Belleğe Alıyoruz
            var eventsQuery = db.events
                .Where(e => !categoryId.HasValue || e.category_id == categoryId)
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

            // Date filter (Artık RAM'de hesaplandığı için EF hata vermez)
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
                    StartDate = e.performances.OrderBy(p => p.start_datetime).FirstOrDefault()?.start_datetime,
                    Venue = e.performances.FirstOrDefault()?.venues.name,
                    City = e.performances.FirstOrDefault()?.venues.cities.name,
                    Price = e.performances.SelectMany(p => p.price_tiers).OrderBy(t => t.price).FirstOrDefault()?.price,
                    ImageUrl = e.poster_url 
                })
                .ToList();

            return PartialView("_EventCards", result);
        }

        public ActionResult Theater()
        {
            ViewBag.CategoryId = 3;
            ViewBag.cities = db.cities.ToList();
            ViewBag.venues = db.venues.ToList();
            return View();
        }
    }
}
