using System;
using System.Linq;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class ProfileController : Controller
    {
        pr2Entities1 db = new pr2Entities1();
        private string HashPassword(string password)
        {
            using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public ActionResult Index()
        {
            // Login check
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Register");
            }

            // Fixed previous issue - now safe
            int userId = Convert.ToInt32(Session["UserID"]);

            var user = db.users.FirstOrDefault(x => x.id == userId);

            return View(user);
        }
        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Register");
            }

            // Check if new passwords match
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "New passwords do not match!";
                return RedirectToAction("Index");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            var user = db.users.FirstOrDefault(x => x.id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }

            // Old password hash validation
            string oldHashed = HashPassword(OldPassword);

            if (user.password_hash != oldHashed)
            {
                ViewBag.Error = "Old password is incorrect!";
                return RedirectToAction("Index");
            }

            // Hash new password and save to DB
            user.password_hash = HashPassword(NewPassword);
            user.updated_at = DateTime.Now;

            db.SaveChanges();

            // Automatic logout for security
            Session.Clear();
            Session.Abandon();

            TempData["Success"] = "Password changed successfully. Please log in again.";

            return RedirectToAction("Login", "Register");
        }
        [HttpPost]
        public ActionResult UpdateProfile(string Name, string Surname)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Register");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            var user = db.users.FirstOrDefault(x => x.id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }

            // Update
            user.name = Name;
            user.surname = Surname;
            user.updated_at = DateTime.Now;

            db.SaveChanges();

            TempData["ProfileSuccess"] = "Profile information has been updated successfully.";

            return RedirectToAction("Index");
        }

        public ActionResult MyTickets()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Register");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            var tickets = db.tickets
                .Where(t => t.order_items.orders.user_id == userId)
                .OrderByDescending(t => t.issued_at)
                .ToList();

            var model = new System.Collections.Generic.List<UserTicketViewModel>();
            foreach (var t in tickets)
            {
                var item = t.order_items;
                // Note: Assuming performance_id is correctly populated in order_items
                var perf = db.performances.Find(item.performance_id); 
                var evt = perf.events;

                string seatInfo = "General Admission";
                if (item.seat_id.HasValue)
                {
                    var seat = db.seats.Find(item.seat_id);
                    if (seat != null) seatInfo = $"{seat.seatmap_section} / Row {seat.row_label} - Seat {seat.seat_number}";
                }

                model.Add(new UserTicketViewModel
                {
                    EventTitle = evt.title,
                    Date = perf.start_datetime,
                    Venue = perf.venues != null ? perf.venues.name : "",
                    SeatLabel = seatInfo,
                    TicketCode = t.ticket_code,
                    QrUrl = t.qr_code_url,
                    HolderName = t.holder_name,
                    Price = item.unit_price,
                    Status = t.status
                });
            }

            return View(model);
        }

        public ActionResult Refunds()
        {
             if (Session["UserID"] == null) return RedirectToAction("Login", "Register");
             // Demo: Return view. In real app, fetch refunds from db.refunds
             return View();
        }

    }

}
