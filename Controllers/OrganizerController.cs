using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class OrganizerController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        // Helper: Check login
        private bool IsOrganizerLoggedIn()
        {
            return Session["OrganizerUserID"] != null;
        }

        // GET: Organizer/Login (organizer-login.html)
        public ActionResult Login()
        {
            if (IsOrganizerLoggedIn()) return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // Note: In real app, password should be hashed.
            // Using organizer_users table for login
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

        // GET: Organizer/Dashboard (organizer-dashboard.html)
        public ActionResult Dashboard()
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            
            long orgId = Convert.ToInt64(Session["OrganizerID"]);
            var events = db.events
                .Where(e => e.organizer_id == orgId)
                .OrderByDescending(e => e.created_at)
                .ToList();
            
            return View(events);
        }

        // GET: Organizer/Applications (organizer-applications.html)
        public ActionResult Applications()
        {
             if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
             long orgId = Convert.ToInt64(Session["OrganizerID"]);
             var apps = db.organizer_applications.Where(a => a.organizer_id == orgId).ToList();
             return View(apps);
        }

        // GET: Organizer/Documents (organizer-documents.html)
        public ActionResult Documents()
        {
             if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
             long orgId = Convert.ToInt64(Session["OrganizerID"]);
             var docs = (from d in db.organizer_documents
                         join a in db.organizer_applications on d.application_id equals a.id
                         where a.organizer_id == orgId
                         select d).ToList();
             return View(docs);
        }

        // GET: Organizer/CreateEvent – submit event request for admin approval
        public ActionResult CreateEvent()
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            ViewBag.Categories = db.categories.ToList();
            return View();
        }

        // POST: Organizer/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEvent(int categoryId, string Title, string Description, string Language, string AgeLimit, string PosterUrl)
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");

            long orgId = Convert.ToInt64(Session["OrganizerID"]);
            long submittedBy = Convert.ToInt64(Session["OrganizerUserID"]);

            var evt = new events
            {
                category_id = categoryId,
                organizer_id = orgId,
                title = Title,
                description = Description,
                language = Language,
                age_limit = AgeLimit,
                poster_url = PosterUrl,
                status = "pending",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            db.events.Add(evt);
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
    }
}

