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

        // GET: Organizer/Register – organizer self-registration (application)
        public ActionResult Register()
        {
            if (IsOrganizerLoggedIn()) return RedirectToAction("Dashboard");
            return View();
        }

        // POST: Organizer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string LegalName, string BrandName, string ContactEmail, string ContactPhone, string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(LegalName) ||
                string.IsNullOrWhiteSpace(BrandName) ||
                string.IsNullOrWhiteSpace(ContactEmail) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Please fill in all required fields.";
                return View();
            }

            // Basic duplicate checks
            if (db.organizers.Any(o => o.contact_email == ContactEmail))
            {
                ViewBag.Error = "An organizer with this email already exists.";
                return View();
            }

            if (db.organizer_users.Any(u => u.email == ContactEmail || u.username == Username))
            {
                ViewBag.Error = "An organizer user with this email or username already exists.";
                return View();
            }

            // 1) Create organizer record in pending status
            var organizer = new organizers
            {
                legal_name = LegalName,
                brand_name = BrandName,
                contact_email = ContactEmail,
                contact_phone = ContactPhone,
                status = "pending",
                created_at = DateTime.Now
            };
            db.organizers.Add(organizer);
            db.SaveChanges();

            // 2) Create organizer application for admin review
            var app = new organizer_applications
            {
                organizer_id = organizer.id,
                org_name = BrandName,
                contact_email = ContactEmail,
                submitted_at = DateTime.Now,
                status = "pending"
            };
            db.organizer_applications.Add(app);
            db.SaveChanges();

            // 3) Create organizer user (for now store plain password like existing login)
            var orgUser = new organizer_users
            {
                organizer_id = organizer.id,
                username = Username,
                email = ContactEmail,
                password_hash = Password,
                role = "Owner",
                status = "pending"
            };
            db.organizer_users.Add(orgUser);
            db.SaveChanges();

            TempData["Success"] = "Your organizer application has been submitted. An admin will review and activate your account.";
            return RedirectToAction("Login");
        }

        // GET: Organizer/Dashboard (organizer-dashboard.html)
        public ActionResult Dashboard()
        {
            if (!IsOrganizerLoggedIn()) return RedirectToAction("Login");
            
            long orgId = Convert.ToInt64(Session["OrganizerID"]);
            var events = db.events.Where(e => e.organizer_id == orgId).ToList();
            
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
    }
}

