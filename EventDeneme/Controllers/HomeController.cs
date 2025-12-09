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

            var events = db.events
                .ToList()
                .Select(e => new EventCardViewModel
                {
                    EventId = e.id,
                    Title = e.title,
                    StartDate = e.performances
                                    .OrderBy(p => p.start_datetime)
                                    .FirstOrDefault()?.start_datetime,
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
            DateTime now = DateTime.Now;
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

    }
}
