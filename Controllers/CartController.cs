using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class CartController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        
        private carts GetOrCreateCart()
        {
            long? userId = null;
            if (Session["UserID"] != null)
            {
                userId = Convert.ToInt64(Session["UserID"]);
            }

            
            var cart = db.carts
                .FirstOrDefault(c => 
                    (userId.HasValue && c.user_id == userId.Value || !userId.HasValue && c.user_id == null) &&
                    c.status == "active");

            if (cart == null)
            {
                
                cart = new carts
                {
                    user_id = userId,
                    status = "active",
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                db.carts.Add(cart);
                db.SaveChanges();
            }

            return cart;
        }

        
        public ActionResult Index()
        {
            var cart = GetOrCreateCart();
            var cartItems = db.cart_items
                .Where(ci => ci.cart_id == cart.id && ci.status == "active")
                .ToList();

            var viewModel = new CartViewModel
            {
                CartId = cart.id,
                Items = new List<CartItemViewModel>(),
                TotalAmount = 0,
                TotalItems = 0
            };

            foreach (var item in cartItems)
            {
                var performance = db.performances.FirstOrDefault(p => p.id == item.performance_id);
                if (performance == null) continue;

                var eventInfo = performance.events;
                var seat = item.seat_id.HasValue ? db.seats.FirstOrDefault(s => s.id == item.seat_id.Value) : null;
                var priceTier = db.price_tiers.FirstOrDefault(pt => pt.id == item.price_tier_id);
                
                
                var perfSeat = seat != null ? 
                    db.performance_seats.FirstOrDefault(ps => ps.performance_id == performance.id && ps.seat_id == seat.id) : 
                    null;

                var cartItem = new CartItemViewModel
                {
                    CartItemId = item.id,
                    PerformanceSeatId = perfSeat != null ? perfSeat.id : 0,
                    PerformanceId = performance.id,
                    EventId = eventInfo.id,
                    EventTitle = eventInfo.title,
                    EventImageUrl = eventInfo.poster_url,
                    EventDate = performance.start_datetime,
                    VenueName = performance.venues != null ? performance.venues.name : "",
                    CityName = performance.venues != null && performance.venues.cities != null ? performance.venues.cities.name : "",
                    Section = seat != null ? seat.seatmap_section : "",
                    Row = seat != null ? seat.row_label : "",
                    SeatNumber = seat != null ? seat.seat_number : "",
                    UnitPrice = item.unit_price,
                    Quantity = item.quantity,
                    Subtotal = item.unit_price * item.quantity
                };

                viewModel.Items.Add(cartItem);
                viewModel.TotalAmount += cartItem.Subtotal;
                viewModel.TotalItems += cartItem.Quantity;
            }

            return View(viewModel);
        }

        
        [HttpPost]
        public JsonResult AddToCart(long performanceSeatId, int quantity = 1)
        {
            try
            {
                var cart = GetOrCreateCart();
                var perfSeat = db.performance_seats.FirstOrDefault(ps => ps.id == performanceSeatId);

                if (perfSeat == null)
                {
                    return Json(new { success = false, message = "Seat not found." });
                }

                if (perfSeat.status != "available")
                {
                    return Json(new { success = false, message = "Seat is not available." });
                }

                
                var priceTier = db.price_tiers.FirstOrDefault(pt => pt.id == perfSeat.price_tier_id);
                if (priceTier == null)
                {
                    
                    var seat = perfSeat.seats;
                    if (seat != null)
                    {
                        priceTier = db.price_tiers
                            .FirstOrDefault(pt => pt.performance_id == perfSeat.performance_id && 
                                                  pt.seatmap_section == seat.seatmap_section);
                    }
                }

                if (priceTier == null)
                {
                    return Json(new { success = false, message = "Price tier not found." });
                }

                
                var existingItem = db.cart_items
                    .FirstOrDefault(ci => ci.cart_id == cart.id && 
                                         ci.performance_id == perfSeat.performance_id &&
                                         ci.seat_id == perfSeat.seat_id &&
                                         ci.status == "active");

                if (existingItem != null)
                {
                    
                    existingItem.quantity += quantity;
                }
                else
                {
                    
                    var cartItem = new cart_items
                    {
                        cart_id = cart.id,
                        performance_id = perfSeat.performance_id,
                        price_tier_id = priceTier.id,
                        seat_id = perfSeat.seat_id,
                        quantity = quantity,
                        unit_price = priceTier.price,
                        status = "active"
                    };
                    db.cart_items.Add(cartItem);
                }

                cart.updated_at = DateTime.Now;
                db.SaveChanges();

                
                var itemCount = db.cart_items
                    .Where(ci => ci.cart_id == cart.id && ci.status == "active")
                    .Sum(ci => (int?)ci.quantity) ?? 0;

                return Json(new { success = true, message = "Item added to cart.", itemCount = itemCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        
        [HttpPost]
        public JsonResult RemoveFromCart(long cartItemId)
        {
            try
            {
                var cartItem = db.cart_items.FirstOrDefault(ci => ci.id == cartItemId);
                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Item not found." });
                }

                cartItem.status = "removed";
                db.SaveChanges();

                
                var cart = db.carts.Find(cartItem.cart_id);
                var itemCount = db.cart_items
                    .Where(ci => ci.cart_id == cart.id && ci.status == "active")
                    .Sum(ci => (int?)ci.quantity) ?? 0;

                return Json(new { success = true, message = "Item removed from cart.", itemCount = itemCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        
        [HttpPost]
        public JsonResult UpdateQuantity(long cartItemId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Quantity must be greater than 0." });
                }

                var cartItem = db.cart_items.FirstOrDefault(ci => ci.id == cartItemId);
                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Item not found." });
                }

                cartItem.quantity = quantity;
                db.SaveChanges();

                var subtotal = cartItem.unit_price * quantity;
                var cart = db.carts.Find(cartItem.cart_id);
                var totalAmount = db.cart_items
                    .Where(ci => ci.cart_id == cart.id && ci.status == "active")
                    .Sum(ci => (decimal?)(ci.unit_price * ci.quantity)) ?? 0;

                var itemCount = db.cart_items
                    .Where(ci => ci.cart_id == cart.id && ci.status == "active")
                    .Sum(ci => (int?)ci.quantity) ?? 0;

                return Json(new { 
                    success = true, 
                    message = "Quantity updated.", 
                    subtotal = subtotal,
                    totalAmount = totalAmount,
                    itemCount = itemCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        
        public JsonResult GetCartCount()
        {
            try
            {
                var cart = GetOrCreateCart();
                var itemCount = db.cart_items
                    .Where(ci => ci.cart_id == cart.id && ci.status == "active")
                    .Sum(ci => (int?)ci.quantity) ?? 0;

                return Json(new { itemCount = itemCount }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { itemCount = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public ActionResult Checkout()
        {
            var cart = GetOrCreateCart();
            var cartItems = db.cart_items
                .Where(ci => ci.cart_id == cart.id && ci.status == "active")
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            
            var performanceGroups = cartItems.GroupBy(ci => ci.performance_id);

            
            
            var firstGroup = performanceGroups.First();
            var firstItem = firstGroup.First();
            var performanceId = firstItem.performance_id;

            
            var seatIds = new List<string>();
            foreach (var item in cartItems.Where(ci => ci.performance_id == performanceId))
            {
                if (item.seat_id.HasValue)
                {
                    var perfSeat = db.performance_seats
                        .FirstOrDefault(ps => ps.performance_id == performanceId && 
                                             ps.seat_id == item.seat_id.Value);
                    if (perfSeat != null)
                    {
                        seatIds.Add(perfSeat.id.ToString());
                    }
                }
            }

            if (!seatIds.Any())
            {
                TempData["Error"] = "No seats found in cart.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Checkout", "Ticket", new { 
                performanceId = performanceId, 
                selectedSeats = string.Join(",", seatIds) 
            });
        }
    }
}



