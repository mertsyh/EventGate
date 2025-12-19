using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class RegisterController : Controller
    {
       
        private pr2Entities1 db= new pr2Entities1();

     
        public ActionResult Signin(string returnUrl)
        {
            // Eğer kullanıcı zaten giriş yapmışsa ana sayfaya veya returnUrl'e yönlendir
            if (Session["UserID"] != null)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            
            // Cache control - giriş yapmış kullanıcılar login sayfasına geri dönemesin
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

    
        [HttpPost]
        public ActionResult Signin(string Email, string Password, string returnUrl)
        {
            string hashedPassword = HashPassword(Password);

            var user = db.users.FirstOrDefault(x =>
                x.email == Email && x.password_hash == hashedPassword);

            if (user == null)
            {
                ViewBag.Error = "Email or password is incorrect!";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            Session["UserID"] = user.id;
            Session["UserName"] = user.name;
            Session["UserEmail"] = user.email;

            // Guest sepetini kullanıcı sepetine aktar
            MergeGuestCartToUser(user.id);

            // Başarılı login sonrası login sayfasını geçmişten kaldırmak için flag
            TempData["JustLoggedIn"] = true;

            // ReturnUrl varsa oraya, yoksa ana sayfaya yönlendir
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

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

        
        private void MergeGuestCartToUser(long userId)
        {
            try
            {
                
                var guestCart = db.carts
                    .FirstOrDefault(c => c.user_id == null && c.status == "active");

                if (guestCart == null)
                {
                    return;
                }

                
                var userCart = db.carts
                    .FirstOrDefault(c => c.user_id == userId && c.status == "active");

                if (userCart == null)
                {
                    
                    guestCart.user_id = userId;
                    guestCart.updated_at = DateTime.Now;
                    db.SaveChanges();
                    return;
                }

                
                var guestItems = db.cart_items
                    .Where(ci => ci.cart_id == guestCart.id && ci.status == "active")
                    .ToList();

                foreach (var guestItem in guestItems)
                {
                    
                    var existingItem = db.cart_items
                        .FirstOrDefault(ci => ci.cart_id == userCart.id &&
                                             ci.performance_id == guestItem.performance_id &&
                                             ci.seat_id == guestItem.seat_id &&
                                             ci.status == "active");

                    if (existingItem != null)
                    {
                        
                        existingItem.quantity += guestItem.quantity;
                        guestItem.status = "merged";
                    }
                    else
                    {
                        
                        guestItem.cart_id = userCart.id;
                    }
                }

                
                guestCart.status = "merged";
                userCart.updated_at = DateTime.Now;
                db.SaveChanges();
            }
            catch
            {
                
            }
        }
    }
}


