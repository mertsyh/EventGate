using EventDeneme.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Linq;
using System;


namespace ProjeAdi.Controllers
{
    public class HomeController : Controller
    {
        string conStr = ConfigurationManager
            .ConnectionStrings["DefaultConnection"]
            .ConnectionString;

        pr2Entities1 db = new pr2Entities1();

        public ActionResult Index()
        {
            ViewBag.cities = db.cities.ToList();
            ViewBag.categories = db.categories.ToList();
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
                    Venue = e.performances
                                .FirstOrDefault()?.venues.name,
                    City = e.performances
                                .FirstOrDefault()?.venues.cities.name,
                    Price = e.performances
                                .SelectMany(p => p.price_tiers)
                                .OrderBy(t => t.price)
                                .FirstOrDefault()?.price,
                    ImageUrl = e.poster_url
                })
                .ToList();
            DateTime nextWeek = now.AddDays(7);

            var lastWeekEvents = events
                .Where(e => e.StartDate.HasValue &&
                            e.StartDate.Value >= now &&
                            e.StartDate.Value <= nextWeek)
                .OrderBy(e => e.StartDate) 
                .ToList();

            ViewBag.LastWeekEvents = lastWeekEvents;

            return View(events);
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        } public ActionResult Privacy_policy()
        {
            return View();
        }
        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
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

    }
}


