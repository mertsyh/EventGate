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

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.users.FirstOrDefault(x => x.id == userId);
            ViewBag.DefaultTab = "MyTickets";
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
