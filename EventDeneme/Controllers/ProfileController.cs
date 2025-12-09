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

    }

}
