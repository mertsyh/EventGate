using System;
using System.Linq;
using System.Collections.Generic;
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

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Register");
            }

         
            int userId = Convert.ToInt32(Session["UserID"]);

            var user = db.users.FirstOrDefault(x => x.id == userId);

            return View(user);
        }
        public ActionResult MyTickets()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Register");

            long userId = Convert.ToInt64(Session["UserID"]);
            var user = db.users.FirstOrDefault(x => x.id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }

            var tickets = (from t in db.tickets
                           join oi in db.order_items on t.order_item_id equals oi.id
                           join o in db.orders on oi.order_id equals o.id
                           join perf in db.performances on oi.performance_id equals perf.id
                           join ev in db.events on perf.event_id equals ev.id
                           join seat in db.seats on oi.seat_id equals seat.id
                           join v in db.venues on perf.venue_id equals v.id
                           where (o.user_id == userId) ||
                                 (o.user_id == null && o.email == user.email)
                           orderby perf.start_datetime descending
                           select new UserTicketViewModel
                           {
                               TicketId = t.id,
                               EventTitle = ev.title,
                               Date = perf.start_datetime,
                               Venue = v.name,
                               SeatLabel = seat.seatmap_section + " " + seat.row_label + seat.seat_number,
                               TicketCode = t.ticket_code,
                               QrUrl = t.qr_code_url,
                               HolderName = t.holder_name,
                               Price = oi.unit_price,
                               Status = t.status
                           }).ToList();

            ViewBag.MyTickets = tickets;
            ViewBag.DefaultTab = "MyTickets";
            return View("Index", user);
        } 
        public ActionResult Refunds()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Register");

            long userId = Convert.ToInt64(Session["UserID"]);
            var user = db.users.FirstOrDefault(x => x.id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }

            var refunds = (from r in db.refunds
                           join p in db.payments on r.payment_id equals p.id
                           join o in db.orders on p.order_id equals o.id
                           where (o.user_id == userId) ||
                                 (o.user_id == null && o.email == user.email)
                           orderby r.id descending
                           select new UserRefundViewModel
                           {
                               RefundId = r.id,
                               Amount = r.amount,
                               Status = r.status,
                               RequestedAt = p.captured_at,
                               ProcessedAt = r.processed_at
                           }).ToList();

            ViewBag.MyRefunds = refunds;
            ViewBag.DefaultTab = "Refunds";
            return View("Index", user);
        }
        public ActionResult ChangePassword()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Register");

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.users.FirstOrDefault(x => x.id == userId);
            ViewBag.DefaultTab = "ChangePassword";
            return View("Index", user);
        }


        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Register");
            }

       
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "The new passwords don't match.!";
                return RedirectToAction("Index");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            var user = db.users.FirstOrDefault(x => x.id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }

       
            string oldHashed = HashPassword(OldPassword);

            if (user.password_hash != oldHashed)
            {
                ViewBag.Error = "Old password is wrong!";
                return RedirectToAction("Index");
            }

 
            user.password_hash = HashPassword(NewPassword);
            user.updated_at = DateTime.Now;

            db.SaveChanges();

      
            Session.Clear();
            Session.Abandon();

            TempData["Success"] = "Password successfully changed, please log in again.";

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

            user.name = Name;
            user.surname = Surname;
            user.updated_at = DateTime.Now;

            db.SaveChanges();

            TempData["ProfileSuccess"] = "Profil informations updated succesfully!";

            return RedirectToAction("Index");
        }

    }

}