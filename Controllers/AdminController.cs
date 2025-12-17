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
            // Admin should see all events (approved, pending, rejected)
            var allEvents = db.events
      .Where(e => e.status != "deleted")
      .OrderByDescending(e => e.created_at)
      .ToList();

            return View(allEvents);
        }

        // GET: Admin/CreateEvent (direct admin-created event)
        public ActionResult CreateEvent()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            ViewBag.Categories = db.categories.ToList();
            ViewBag.Organizers = db.organizers.ToList();
            return View();
        }

        // POST: Admin/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEvent(int categoryId, long organizerId, string Title, string Description, string Language, string AgeLimit, string PosterUrl)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(Title))
            {
                ViewBag.Error = "Title is required.";
                ViewBag.Categories = db.categories.ToList();
                ViewBag.Organizers = db.organizers.ToList();
                return View();
            }

            var evt = new events
            {
                category_id = categoryId,
                organizer_id = organizerId,
                title = Title,
                description = Description,
                language = Language,
                age_limit = AgeLimit,
                poster_url = string.IsNullOrWhiteSpace(PosterUrl)  ? null : "~/images/" + PosterUrl.Trim(),

                status = "approved",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            db.events.Add(evt);
            db.SaveChanges();

            TempData["Success"] = "Event created successfully.";
            return RedirectToAction("Events");
        }

        // GET: Admin/EditEvent/5
        public ActionResult EditEvent(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var evt = db.events.Find(id);
            if (evt == null) return HttpNotFound();
            return View(evt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEvent(events model)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var evt = db.events.Find(model.id);
            if (evt == null) return HttpNotFound();

            evt.title = model.title;
            evt.description = model.description;
            evt.poster_url = string.IsNullOrWhiteSpace(model.poster_url)
     ? null
     : (model.poster_url.StartsWith("~/")
         ? model.poster_url
         : "~/images/" + model.poster_url.Trim());


            db.SaveChanges();
            TempData["Success"] = "Event updated successfully.";
            return RedirectToAction("Events");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        // 4. Venues Management (admin-venues.html)
        public ActionResult Venues()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.venues.ToList());
        }

        // GET: Admin/CreateVenue
        public ActionResult CreateVenue()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            ViewBag.Cities = db.cities.ToList();
            return View();
        }

        // POST: Admin/CreateVenue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVenue(long cityId, string Name, string Address, bool hasSeating = false)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(Name))
            {
                ViewBag.Error = "Name is required.";
                ViewBag.Cities = db.cities.ToList();
                return View();
            }

            var venue = new venues
            {
                city_id = cityId,
                name = Name,
                address = Address,
                has_seating = hasSeating
            };

            db.venues.Add(venue);
            db.SaveChanges();

            TempData["Success"] = "Venue created successfully.";
            return RedirectToAction("Venues");
        }

        // GET: Admin/EditVenue/5
        public ActionResult EditVenue(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var venue = db.venues.Find(id);
            if (venue == null) return HttpNotFound();
            ViewBag.Cities = db.cities.ToList();
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVenue(venues model)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var venue = db.venues.Find(model.id);
            if (venue == null) return HttpNotFound();

            venue.name = model.name;
            venue.address = model.address;
            venue.city_id = model.city_id;
            venue.has_seating = model.has_seating;

            db.SaveChanges();
            TempData["Success"] = "Venue updated successfully.";
            return RedirectToAction("Venues");
        }
 
        // 5. Users Management (admin-users.html)
        public ActionResult Users()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(db.users.ToList());
        }

        // GET: Admin/EditUser/5
        public ActionResult EditUser(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var user = db.users.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(users model)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var user = db.users.Find(model.id);
            if (user == null) return HttpNotFound();

            user.name = model.name;
            user.surname = model.surname;
            user.email = model.email;
            user.phone = model.phone;

            db.SaveChanges();
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction("Users");
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

        // 9. Event Requests (pending events from organizers)
        public ActionResult EventRequests()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var pending = db.moderation_events
                .Where(m => m.status == "pending")
                .OrderByDescending(m => m.id)
                .ToList();
            return View(pending);
        }

        [HttpPost]
        public ActionResult ApproveEventRequest(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var mod = db.moderation_events.Find(id);
            if (mod != null)
            {
                mod.status = "approved";
                mod.reviewed_by = Convert.ToInt64(Session["AdminID"]);
                mod.reviewed_at = DateTime.Now;

                var evt = db.events.Find(mod.event_id);
                if (evt != null)
                {
                    evt.status = "active";
                    evt.updated_at = DateTime.Now;

                    var perfs = db.performances.Where(p => p.event_id == evt.id).ToList();
                    foreach (var perf in perfs)
                    {
                        perf.status = "active";
                    }

                }
                db.SaveChanges();
                TempData["Success"] = "Event request approved.";
            }
            return RedirectToAction("EventRequests");
        }

        [HttpPost]
        public ActionResult RejectEventRequest(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var mod = db.moderation_events.Find(id);
            if (mod != null)
            {
                mod.status = "rejected";
                mod.reviewed_by = Convert.ToInt64(Session["AdminID"]);
                mod.reviewed_at = DateTime.Now;

                var evt = db.events.Find(mod.event_id);
                if (evt != null)
                {
                    evt.status = "rejected";
                    evt.updated_at = DateTime.Now;
                }
                db.SaveChanges();
                TempData["Success"] = "Event request rejected.";
            }
            return RedirectToAction("EventRequests");
        }

        // GET: Admin/CreateOrganizer
        public ActionResult CreateOrganizer()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View();
        }

        // POST: Admin/CreateOrganizer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrganizer(string LegalName, string BrandName, string ContactEmail, string ContactPhone, string Username, string Password)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(LegalName) ||
                string.IsNullOrWhiteSpace(BrandName) ||
                string.IsNullOrWhiteSpace(ContactEmail) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Please fill in all required fields.";
                return View();
            }

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

            var organizer = new organizers
            {
                legal_name = LegalName,
                brand_name = BrandName,
                contact_email = ContactEmail,
                contact_phone = ContactPhone,
                status = "approved",
                created_at = DateTime.Now
            };
            db.organizers.Add(organizer);
            db.SaveChanges();

            var app = new organizer_applications
            {
                organizer_id = organizer.id,
                org_name = BrandName,
                contact_email = ContactEmail,
                submitted_at = DateTime.Now,
                status = "approved",
                decided_at = DateTime.Now
            };
            db.organizer_applications.Add(app);
            db.SaveChanges();

            var orgUser = new organizer_users
            {
                organizer_id = organizer.id,
                username = Username,
                email = ContactEmail,
                password_hash = Password,
                role = "Owner",
                status = "active"
            };
            db.organizer_users.Add(orgUser);
            db.SaveChanges();

            TempData["Success"] = "Organizer created successfully.";
            return RedirectToAction("Organizers");
        }

        // ========== DELETE ACTIONS ==========

        [HttpPost]
        public ActionResult DeleteEvent(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var evt = db.events.Find(id);
            if (evt == null)
            {
                TempData["Success"] = "Event not found or already deleted.";
                return RedirectToAction("Events");
            }

            // Soft-delete: keep event and related data in DB for integrity (tickets, orders, etc.),
            // but mark it as deleted/cancelled so it is not used anymore.
            evt.status = "deleted";
            evt.updated_at = DateTime.Now;

            // Optionally, mark all related performances as cancelled as well
            var performances = db.performances.Where(p => p.event_id == evt.id).ToList();
            foreach (var perf in performances)
            {
                perf.status = "cancelled";
            }

            db.SaveChanges();
            TempData["Success"] = "Event marked as deleted successfully.";

            return RedirectToAction("Events");
        }

        [HttpPost]
        public ActionResult DeleteVenue(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var venue = db.venues.Find(id);
            if (venue != null)
            {
                db.venues.Remove(venue);
                db.SaveChanges();
                TempData["Success"] = "Venue deleted successfully.";
            }
            return RedirectToAction("Venues");
        }

        // ========== ORGANIZER ACTIONS ==========

        public ActionResult OrganizerDetails(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var organizer = db.organizers.Find(id);
            if (organizer == null) return HttpNotFound();
            return View(organizer);
        }

        [HttpPost]
        public ActionResult ApproveOrganizer(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var organizer = db.organizers.Find(id);
            if (organizer != null)
            {
                organizer.status = "approved";
                var orgUsers = db.organizer_users.Where(u => u.organizer_id == id).ToList();
                foreach (var user in orgUsers)
                {
                    user.status = "active";
                }
                db.SaveChanges();
                TempData["Success"] = "Organizer approved successfully.";
            }
            return RedirectToAction("Organizers");
        }

        [HttpPost]
        public ActionResult RejectOrganizer(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var organizer = db.organizers.Find(id);
            if (organizer != null)
            {
                organizer.status = "rejected";
                db.SaveChanges();
                TempData["Success"] = "Organizer rejected.";
            }
            return RedirectToAction("Organizers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrganizer(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var organizer = db.organizers.Find(id);
            if (organizer != null)
            {
                // Soft delete organizer: mark as deleted and deactivate related users and events
                organizer.status = "deleted";

                var orgUsers = db.organizer_users.Where(u => u.organizer_id == id).ToList();
                foreach (var user in orgUsers)
                {
                    user.status = "inactive";
                }

                var orgEvents = db.events.Where(e => e.organizer_id == id).ToList();
                foreach (var evt in orgEvents)
                {
                    evt.status = "deleted";
                    evt.updated_at = DateTime.Now;

                    var perfs = db.performances.Where(p => p.event_id == evt.id).ToList();
                    foreach (var perf in perfs)
                    {
                        perf.status = "cancelled";
                    }
                }

                db.SaveChanges();
                TempData["Success"] = "Organizer deleted and related users/events disabled.";
            }
            return RedirectToAction("Organizers");
        }

        // ========== REFUND ACTIONS ==========

        [HttpPost]
        public ActionResult ApproveRefund(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var refund = db.refunds.Find(id);
            if (refund != null)
            {
                refund.status = "approved";
                refund.processed_at = DateTime.Now;

                var payment = refund.payments;
                if (payment != null)
                {
                    payment.status = "refunded";
                    var order = payment.orders;
                    if (order != null)
                    {
                        order.status = "refunded";
                        var orderItems = db.order_items.Where(oi => oi.order_id == order.id).ToList();
                        foreach (var oi in orderItems)
                        {
                            foreach (var ticket in oi.tickets.ToList())
                            {
                                ticket.status = "refunded";
                            }
                        }
                    }
                }

                db.SaveChanges();
                TempData["Success"] = "Refund approved successfully.";
            }
            return RedirectToAction("Refunds");
        }

        [HttpPost]
        public ActionResult RejectRefund(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var refund = db.refunds.Find(id);
            if (refund != null)
            {
                refund.status = "rejected";
                refund.processed_at = DateTime.Now;
                db.SaveChanges();
                TempData["Success"] = "Refund rejected.";
            }
            return RedirectToAction("Refunds");
        }

        // ========== USER ACTIONS ==========

        [HttpPost]
        public ActionResult BanUser(long id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            // Note: users table might not have a status field, so this is a placeholder
            // You may need to add a status field or use a different approach
            TempData["Success"] = "User ban functionality - to be implemented with user status field.";
            return RedirectToAction("Users");
        }
    }
}

