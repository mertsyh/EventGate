using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class OrganizerController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        
        private bool IsOrganizerLoggedIn()
        {
            return Session["OrganizerUserID"] != null;
        }

        private long GetCurrentOrganizerId()
        {
            return Convert.ToInt64(Session["OrganizerID"]);
        }

        private bool IsOwnerOfEvent(long eventId)
        {
            long orgId = GetCurrentOrganizerId();
            var evt = db.events.FirstOrDefault(e => e.id == eventId && e.organizer_id == orgId && e.status != "deleted");
            return evt != null;
        }

        
        public ActionResult Login()
        {
            if (IsOrganizerLoggedIn()) return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            
            
            var orgUser = db.organizer_users.FirstOrDefault(u => u.email == email && u.password_hash == password);
            
            if (orgUser != null)
            {
                Session["OrganizerUserID"] = orgUser.id;
                Session["OrganizerID"] = orgUser.organizer_id;
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("OrganizerUserID");
            Session.Remove("OrganizerID");
            return RedirectToAction("Login");
        }

        
        public ActionResult Dashboard()
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            
            long orgId = GetCurrentOrganizerId();
            var events = db.events
                .Where(e => e.organizer_id == orgId && e.status != "deleted")
                .OrderByDescending(e => e.created_at)
                .ToList();
            
            
            var eventIds = events.Select(e => e.id).ToList();
            
            if (eventIds.Any())
            {
                
                var performanceIds = db.performances
                    .Where(p => eventIds.Contains(p.event_id))
                    .Select(p => p.id)
                    .ToList();
                
                if (performanceIds.Any())
                {
                    
                    var orderItemIds = db.order_items
                        .Where(oi => performanceIds.Contains(oi.performance_id))
                        .Select(oi => oi.id)
                        .ToList();
                    
                    
                    int totalTicketsSold = db.tickets
                        .Where(t => orderItemIds.Contains(t.order_item_id))
                        .Count();
                    
                    
                    
                    var orderIds = db.order_items
                        .Where(oi => performanceIds.Contains(oi.performance_id))
                        .Select(oi => oi.order_id)
                        .Distinct()
                        .ToList();
                    
                    decimal totalRevenue = db.orders
                        .Where(o => orderIds.Contains(o.id) && o.status == "completed")
                        .Sum(o => (decimal?)o.total_amount) ?? 0;
                    
                    ViewBag.TotalTicketsSold = totalTicketsSold;
                    ViewBag.TotalRevenue = totalRevenue;
                }
                else
                {
                    ViewBag.TotalTicketsSold = 0;
                    ViewBag.TotalRevenue = 0;
                }
            }
            else
            {
                ViewBag.TotalTicketsSold = 0;
                ViewBag.TotalRevenue = 0;
            }
            
            return View(events);
        }

        
        public ActionResult Applications()
        {
             if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
             long orgId = Convert.ToInt64(Session["OrganizerID"]);
             var apps = db.organizer_applications.Where(a => a.organizer_id == orgId).ToList();
             return View(apps);
        }

        
        public ActionResult CreateEvent()
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            ViewBag.Categories = db.categories.ToList();
            ViewBag.Venues = db.venues.ToList();
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEvent(
            int categoryId,
            string Title,
            string Description,
            string Language,
            string AgeLimit,
            string PosterUrl,
            long venueId,
            DateTime performanceStart,
            string tierName,
            string seatSection,
            int? capacity,
            decimal price,
            string currency)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");

            long orgId = GetCurrentOrganizerId();
            long submittedBy = Convert.ToInt64(Session["OrganizerUserID"]);

            var evt = new events
            {
                category_id = categoryId,
                organizer_id = orgId,
                title = Title,
                description = Description,
                language = Language,
                age_limit = AgeLimit,
                poster_url = string.IsNullOrWhiteSpace(PosterUrl)
                    ? null
                    : (PosterUrl.StartsWith("~/")
                        ? PosterUrl
                        : "~/images/" + PosterUrl.Trim()),
                status = "pending",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            db.events.Add(evt);
            db.SaveChanges();

            
            var performance = new performances
            {
                event_id = evt.id,
                venue_id = venueId,
                start_datetime = performanceStart,
                status = "pending"
            };
            db.performances.Add(performance);
            db.SaveChanges();

            
            var tier = new price_tiers
            {
                performance_id = performance.id,
                name = string.IsNullOrWhiteSpace(tierName) ? "Standard" : tierName,
                allocation_type = "general",
                capacity = capacity,
                price = price,
                currency = string.IsNullOrWhiteSpace(currency) ? "TRY" : currency,
                seatmap_section = string.IsNullOrWhiteSpace(seatSection) ? "General" : seatSection
            };
            db.price_tiers.Add(tier);
            db.SaveChanges();

            var moderation = new moderation_events
            {
                event_id = evt.id,
                submitted_by = submittedBy,
                status = "pending"
            };
            db.moderation_events.Add(moderation);
            db.SaveChanges();

            TempData["Success"] = "Your event request has been submitted for admin review.";
            return RedirectToAction("Dashboard");
        }

        
        public ActionResult EditEvent(long id)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            if (!IsOwnerOfEvent(id)) return new HttpUnauthorizedResult();

            var evt = db.events.Find(id);
            if (evt == null) return HttpNotFound();

            ViewBag.Categories = db.categories.ToList();
            ViewBag.Venues = db.venues.ToList();
            return View(evt);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEvent(
            long id,
            int categoryId,
            string Title,
            string Description,
            string Language,
            string AgeLimit,
            string PosterUrl)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            if (!IsOwnerOfEvent(id)) return new HttpUnauthorizedResult();

            var evt = db.events.Find(id);
            if (evt == null) return HttpNotFound();

            if (string.IsNullOrWhiteSpace(Title))
            {
                ViewBag.Error = "Title is required.";
                ViewBag.Categories = db.categories.ToList();
                ViewBag.Venues = db.venues.ToList();
                return View(evt);
            }

            evt.category_id = categoryId;
            evt.title = Title;
            evt.description = Description;
            evt.language = Language;
            evt.age_limit = AgeLimit;
            evt.poster_url = string.IsNullOrWhiteSpace(PosterUrl)
                ? null
                : (PosterUrl.StartsWith("~/")
                    ? PosterUrl
                    : "~/images/" + PosterUrl.Trim());
            evt.updated_at = DateTime.Now;

            db.SaveChanges();

            TempData["Success"] = "Event updated successfully.";
            return RedirectToAction("Dashboard");
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEvent(long id)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            if (!IsOwnerOfEvent(id)) return new HttpUnauthorizedResult();

            var evt = db.events.Find(id);
            if (evt == null) return HttpNotFound();

            evt.status = "deleted";
            evt.updated_at = DateTime.Now;

            
            var perfs = db.performances.Where(p => p.event_id == evt.id).ToList();
            foreach (var p in perfs)
            {
                p.status = "cancelled";
            }

            db.SaveChanges();
            TempData["Success"] = "Event deleted successfully.";
            return RedirectToAction("Dashboard");
        }

        
        public ActionResult ManagePerformances(long eventId)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            if (!IsOwnerOfEvent(eventId)) return new HttpUnauthorizedResult();

            var evt = db.events.Find(eventId);
            if (evt == null) return HttpNotFound();

            ViewBag.Event = evt;
            var perfs = db.performances
                .Where(p => p.event_id == eventId)
                .OrderBy(p => p.start_datetime)
                .ToList();
            ViewBag.Venues = db.venues.ToList();
            return View(perfs);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPerformance(long eventId, long venueId, DateTime startDateTime)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            if (!IsOwnerOfEvent(eventId)) return new HttpUnauthorizedResult();

            var perf = new performances
            {
                event_id = eventId,
                venue_id = venueId,
                start_datetime = startDateTime,
                status = "pending"
            };
            db.performances.Add(perf);
            db.SaveChanges();

            TempData["Success"] = "Performance added successfully.";
            return RedirectToAction("ManagePerformances", new { eventId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePerformance(long id)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");

            var perf = db.performances.Find(id);
            if (perf == null) return HttpNotFound();
            if (!IsOwnerOfEvent(perf.event_id)) return new HttpUnauthorizedResult();

            perf.status = "cancelled";
            db.SaveChanges();

            TempData["Success"] = "Performance cancelled.";
            return RedirectToAction("ManagePerformances", new { eventId = perf.event_id });
        }

        
        public ActionResult ManagePrices(long performanceId)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");

            var perf = db.performances.Find(performanceId);
            if (perf == null) return HttpNotFound();
            if (!IsOwnerOfEvent(perf.event_id)) return new HttpUnauthorizedResult();

            ViewBag.Performance = perf;
            var tiers = db.price_tiers.Where(t => t.performance_id == performanceId).ToList();
            return View(tiers);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPriceTier(
            long performanceId,
            string name,
            string seatmapSection,
            int? capacity,
            decimal price,
            string currency)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");

            var perf = db.performances.Find(performanceId);
            if (perf == null) return HttpNotFound();
            if (!IsOwnerOfEvent(perf.event_id)) return new HttpUnauthorizedResult();

            var tier = new price_tiers
            {
                performance_id = performanceId,
                name = string.IsNullOrWhiteSpace(name) ? "Standard" : name,
                allocation_type = "general",
                capacity = capacity,
                price = price,
                currency = string.IsNullOrWhiteSpace(currency) ? "TRY" : currency,
                seatmap_section = string.IsNullOrWhiteSpace(seatmapSection) ? "General" : seatmapSection
            };
            db.price_tiers.Add(tier);
            db.SaveChanges();

            TempData["Success"] = "Price tier added.";
            return RedirectToAction("ManagePrices", new { performanceId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePriceTier(long id)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");

            var tier = db.price_tiers.Find(id);
            if (tier == null) return HttpNotFound();

            var perf = db.performances.Find(tier.performance_id);
            if (perf == null) return HttpNotFound();
            if (!IsOwnerOfEvent(perf.event_id)) return new HttpUnauthorizedResult();

            db.price_tiers.Remove(tier);
            db.SaveChanges();

            TempData["Success"] = "Price tier deleted.";
            return RedirectToAction("ManagePrices", new { performanceId = perf.id });
        }
    }
}



