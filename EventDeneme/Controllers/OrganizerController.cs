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

