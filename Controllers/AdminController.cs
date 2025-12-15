using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class AdminController : Controller
    {
        private pr2Entities1 db = new pr2Entities1();

        // Admin Auth Helper
        private bool IsAdminLoggedIn()
        {
            return Session["AdminID"] != null;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // 1. Admin Login (admin-login.html)
        public ActionResult Login()
        {
            if (IsAdminLoggedIn()) return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            var admin = db.admins.FirstOrDefault(a => a.username == username && a.password_hash == hashedPassword);

            if (admin != null)
            {
                Session["AdminID"] = admin.id;
                Session["AdminRole"] = admin.role;
                Session["AdminName"] = admin.username;
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("AdminID");
            Session.Remove("AdminRole");
            Session.Remove("AdminName");
            return RedirectToAction("Login");
        }

        // 2. Dashboard (admin-dashboard.html)
        public ActionResult Dashboard()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            ViewBag.TotalEvents = db.events.Count();
            ViewBag.TotalUsers = db.users.Count();
            ViewBag.TotalOrganizers = db.organizers.Count();
            // Add more stats as needed
            return View();
        }

        // Run this once to create a default admin, then remove or comment it out.
        public ActionResult CreateDefaultAdmin()
        {
            string password = "admin123";
            string hashedPassword = HashPassword(password); // use existing HashPassword method

            var admin = new admins
            {
                username = "admin",
                password_hash = hashedPassword,
                role = "SuperAdmin",
                created_at = DateTime.Now
            };

            db.admins.Add(admin);
            db.SaveChanges();

            return Content("Default admin created. Username: admin, Password: admin123");
        }

        // 3. Events Management (admin-events.html)
        public ActionResult Events()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.events.ToList());
        }

        // 4. Venues Management (admin-venues.html)
        public ActionResult Venues()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.venues.ToList());
        }

        // 5. Users Management (admin-users.html)
        public ActionResult Users()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.users.ToList());
        }

        // 6. Tickets Management (admin-tickets.html)
        public ActionResult Tickets()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.tickets.ToList());
        }

        // 7. Refunds Management (admin-refunds.html)
        public ActionResult Refunds()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.refunds.ToList());
        }

        // 8. Organizers Management (admin-organizers.html)
        public ActionResult Organizers()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.organizers.ToList());
        }
    }
}

