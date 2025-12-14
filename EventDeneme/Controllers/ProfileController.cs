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
            // ✅ Login kontrolü
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Register");
            }

            // ✅ HATA BURADAYDI → Artık %100 güvenli
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

            // ✅ Yeni şifreler uyuşuyor mu?
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Yeni şifreler uyuşmuyor!";
                return RedirectToAction("Index");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            var user = db.users.FirstOrDefault(x => x.id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }

            // ✅ Eski şifre hash kontrolü
            string oldHashed = HashPassword(OldPassword);

            if (user.password_hash != oldHashed)
            {
                ViewBag.Error = "Eski şifre yanlış!";
                return RedirectToAction("Index");
            }

            // ✅ Yeni şifreyi hashleyip DB’ye yaz
            user.password_hash = HashPassword(NewPassword);
            user.updated_at = DateTime.Now;

            db.SaveChanges();

            // ✅ Güvenlik için otomatik logout
            Session.Clear();
            Session.Abandon();

            TempData["Success"] = "Şifre başarıyla değiştirildi. Lütfen tekrar giriş yapın.";

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

            // ✅ Güncelleme
            user.name = Name;
            user.surname = Surname;
            user.updated_at = DateTime.Now;

            db.SaveChanges();

            TempData["ProfileSuccess"] = "Profil bilgileri başarıyla güncellendi.";

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
