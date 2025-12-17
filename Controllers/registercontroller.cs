using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class RegisterController : Controller
    {
       
        private pr2Entities1 db= new pr2Entities1();

     
        public ActionResult Signin()
        {
            return View();
        }

    
        [HttpPost]
        public ActionResult Signin(string Email, string Password)
        {
            string hashedPassword = HashPassword(Password);

            var user = db.users.FirstOrDefault(x =>
                x.email == Email && x.password_hash == hashedPassword);

            if (user == null)
            {
                ViewBag.Error = "Email or password is incorrect!";
                return View();
            }

            Session["UserID"] = user.id;
            Session["UserName"] = user.name;
            Session["UserEmail"] = user.email;

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            Session.Clear();    
            Session.Abandon();  

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Signup()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            
            var user = db.users.FirstOrDefault(x => x.email == Email);
            if (user == null)
            {
                ViewBag.Error = "User not found.";
                return View();
            }

            
            ViewBag.Message = "Password reset link has been sent to your email.";
            return View();
        }

        [HttpPost]
        public ActionResult Signup(string Name, string Surname, string Email, string Phone, string Password)
        {
            var existingUser = db.users.FirstOrDefault(x => x.email == Email);

            if (existingUser != null)
            {
                ViewBag.Error = "This email has already been used";
                return View();
            }

            users newUser = new users
            {
                name = Name,
                surname = Surname,
                email = Email,
                phone = Phone,
                password_hash = HashPassword(Password),
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            db.users.Add(newUser);
            db.SaveChanges();

            return RedirectToAction("Signin");
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
    }
}


