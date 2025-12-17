using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventDeneme.Models;

namespace EventDeneme.Controllers
{
    public class TicketController : Controller
    {
        pr2Entities1 db = new pr2Entities1();

        // GET: Ticket/Buy/5
        public ActionResult Buy(int id)
        {
            var eventItem = db.events.FirstOrDefault(e => e.id == id);
            if (eventItem == null) return HttpNotFound();

            // Check if event is deleted
            if (eventItem.status == "deleted")
            {
                ViewBag.Message = "This event is not available for sale.";
                return View("Error");
            }

            DateTime now = DateTime.Now;

            // Performance selection - get next upcoming performance that is available for sale
            var performance = eventItem.performances
                .Where(p => 
                    p.status != "cancelled" &&
                    // Check if performance hasn't ended
                    (p.end_datetime.HasValue ? p.end_datetime.Value > now : p.start_datetime > now) &&
                    // Check sales dates if they exist
                    (!p.sales_start.HasValue || p.sales_start.Value <= now) &&
                    (!p.sales_end.HasValue || p.sales_end.Value > now)
                )
                .OrderBy(p => p.start_datetime)
                .FirstOrDefault();
            
            if (performance == null)
            {
                ViewBag.Message = "This event is not available for sale.";
                return View("Error");
            }

            // AUTO-SEED: If there are no seats in DB, create demo seats.
            // This part should be removed in production.
            if (!db.performance_seats.Any(ps => ps.performance_id == performance.id))
            {
                SeedSeats(performance.id, performance.venue_id);
            }

            // Load price tiers
            var priceTiers = db.price_tiers.Where(pt => pt.performance_id == performance.id).ToList();

            // Load seats
            var perfSeats = db.performance_seats
                .Where(ps => ps.performance_id == performance.id && ps.status == "available")
                .ToList();

            var seatViewModels = new List<SeatViewModel>();

            foreach (var ps in perfSeats)
            {
                // Find price
                decimal price = 0;
                var tier = priceTiers.FirstOrDefault(pt => pt.id == ps.price_tier_id);
                if (tier != null)
                {
                    price = tier.price;
                }
                else
                {
                    // ID ile bulunamazsa Section ile dene
                    var tierBySection = priceTiers.FirstOrDefault(pt => pt.seatmap_section == ps.seats.seatmap_section);
                    if (tierBySection != null) price = tierBySection.price;
                }

                seatViewModels.Add(new SeatViewModel
                {
                    PerformanceSeatId = ps.id,
                    Section = ps.seats.seatmap_section,
                    Row = ps.seats.row_label,
                    Number = ps.seats.seat_number,
                    Price = price,
                    Status = ps.status
                });
            }

            // If there are no seats or all are sold
            if (!seatViewModels.Any())
            {
                // For demo purposes, we could create fake seats or return empty.
                // For now, it returns empty.
            }

            var model = new SeatSelectionViewModel
            {
                EventId = eventItem.id,
                PerformanceId = performance.id,
                EventTitle = eventItem.title,
                VenueName = performance.venues != null ? performance.venues.name : "",
                StartDate = performance.start_datetime,
                AvailableSeats = seatViewModels.OrderBy(s => s.Section).ThenBy(s => s.Row).ThenBy(s => s.Number).ToList()
            };

            return View(model);
        }

        // GET: Ticket/Checkout
        // Comes here from Cart checkout
        public ActionResult Checkout(long performanceId, string selectedSeats)
        {
            if (string.IsNullOrEmpty(selectedSeats))
            {
                // Redirect back (we need EventId from performance)
                var perf = db.performances.Find(performanceId);
                if (perf == null) return HttpNotFound();
                return RedirectToAction("Buy", new { id = perf.event_id });
            }

            var seatIds = selectedSeats.Split(',').Select(long.Parse).ToList();
            
            // Fiyat hesapla
            decimal totalAmount = 0;
            var seatsToBuy = db.performance_seats.Where(ps => seatIds.Contains(ps.id)).ToList();
            var priceTiers = db.price_tiers.Where(pt => pt.performance_id == performanceId).ToList();

            foreach (var seat in seatsToBuy)
            {
                 var tier = priceTiers.FirstOrDefault(pt => pt.id == seat.price_tier_id);
                 if (tier != null) totalAmount += tier.price;
                 else 
                 {
                     var tierBySection = priceTiers.FirstOrDefault(pt => pt.seatmap_section == seat.seats.seatmap_section);
                     if (tierBySection != null) totalAmount += tierBySection.price;
                 }
            }

            var perfInfo = db.performances.Find(performanceId);
            if (perfInfo == null) return HttpNotFound();
            var eventInfo = perfInfo.events;
            if (eventInfo == null) return HttpNotFound();

            var model = new CheckoutViewModel
            {
                PerformanceId = performanceId,
                SelectedSeatIds = selectedSeats,
                TotalAmount = totalAmount,
                EventTitle = eventInfo.title,
                SeatCount = seatIds.Count,
                EventImageUrl = eventInfo.poster_url,
                EventDate = perfInfo.start_datetime,
                VenueName = perfInfo.venues != null ? perfInfo.venues.name : "",
                CityName = perfInfo.venues != null && perfInfo.venues.cities != null ? perfInfo.venues.cities.name : "",
                CategoryName = eventInfo.categories != null ? eventInfo.categories.display_name : ""
            };

            return View(model);
        }

        // POST: Ticket/Checkout
        // Comes here when user selects seats and continues
        [HttpPost]
        public ActionResult Checkout(long performanceId, string selectedSeats)
        {
            if (string.IsNullOrEmpty(selectedSeats))
            {
                // Redirect back (we need EventId from performance)
                var perf = db.performances.Find(performanceId);
                return RedirectToAction("Buy", new { id = perf.event_id });
            }

            var seatIds = selectedSeats.Split(',').Select(long.Parse).ToList();
            
            // Fiyat hesapla
            decimal totalAmount = 0;
            var seatsToBuy = db.performance_seats.Where(ps => seatIds.Contains(ps.id)).ToList();
            var priceTiers = db.price_tiers.Where(pt => pt.performance_id == performanceId).ToList();

            foreach (var seat in seatsToBuy)
            {
                 var tier = priceTiers.FirstOrDefault(pt => pt.id == seat.price_tier_id);
                 if (tier != null) totalAmount += tier.price;
                 else 
                 {
                     var tierBySection = priceTiers.FirstOrDefault(pt => pt.seatmap_section == seat.seats.seatmap_section);
                     if (tierBySection != null) totalAmount += tierBySection.price;
                 }
            }

            var perfInfo = db.performances.Find(performanceId);
            var eventInfo = perfInfo.events;

            var model = new CheckoutViewModel
            {
                PerformanceId = performanceId,
                SelectedSeatIds = selectedSeats,
                TotalAmount = totalAmount,
                EventTitle = eventInfo.title,
                SeatCount = seatIds.Count,
                EventImageUrl = eventInfo.poster_url,
                EventDate = perfInfo.start_datetime,
                VenueName = perfInfo.venues != null ? perfInfo.venues.name : "",
                CityName = perfInfo.venues != null && perfInfo.venues.cities != null ? perfInfo.venues.cities.name : "",
                CategoryName = eventInfo.categories != null ? eventInfo.categories.display_name : ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessPayment(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            try
            {
                // 1. Payment process (simulation)
                bool paymentSuccess = true; 
                if (!paymentSuccess) return RedirectToAction("Failed");

                // 2. Create order
                long? userId = null;
                if (Session["UserID"] != null)
                {
                    userId = Convert.ToInt64(Session["UserID"]);
                }

                var order = new orders
                {
                    user_id = userId,
                    email = model.Email,
                    phone = model.Phone,
                    total_amount = model.TotalAmount,
                    currency = "TRY",
                    status = "completed",
                    created_at = DateTime.Now
                };
                db.orders.Add(order);
                db.SaveChanges(); // Save to get ID

                // 2b. Create payment record for this order (for refunds)
                var payment = new payments
                {
                    order_id = order.id,
                    provider = "manual",
                    provider_payment_id = Guid.NewGuid().ToString(),
                    amount = model.TotalAmount,
                    currency = "TRY",
                    status = "captured",
                    captured_at = DateTime.Now
                };
                db.payments.Add(payment);
                db.SaveChanges();

                // 3. Update seats and create order items
                var seatIds = model.SelectedSeatIds.Split(',').Select(long.Parse).ToList();
                var seatsToUpdate = db.performance_seats.Where(ps => seatIds.Contains(ps.id)).ToList();
                var priceTiers = db.price_tiers.Where(pt => pt.performance_id == model.PerformanceId).ToList();

                foreach (var seat in seatsToUpdate)
                {
                    // Ideally check if seat already sold (concurrency)
                    if (seat.status != "available")
                    {
                        // Error: seat taken by someone else
                        return RedirectToAction("Failed"); 
                    }

                    seat.status = "sold";
                    
                    // Order Item ekle
                    // Find price and tier ID
                    decimal unitPrice = 0;
                    long priceTierId = 0;

                    var tier = priceTiers.FirstOrDefault(pt => pt.id == seat.price_tier_id);
                    if (tier != null)
                    {
                        unitPrice = tier.price;
                        priceTierId = tier.id;
                    }
                    else
                    {
                        var tierBySection = priceTiers.FirstOrDefault(pt => pt.seatmap_section == seat.seats.seatmap_section);
                        if (tierBySection != null)
                        {
                            unitPrice = tierBySection.price;
                            priceTierId = tierBySection.id;
                        }
                    }
                    
                    var item = new order_items
                    {
                        order_id = order.id,
                        performance_id = seat.performance_id,
                        seat_id = seat.seat_id,
                        price_tier_id = priceTierId,
                        unit_price = unitPrice
                    };
                    db.order_items.Add(item);
                    db.SaveChanges(); // Item ID needed for Ticket

                    // 4. Create ticket (for reporting)
                    var ticket = new tickets
                    {
                        order_item_id = item.id,
                        ticket_code = Guid.NewGuid().ToString().ToUpper().Substring(0, 8),
                        qr_code_url = "https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=" + item.id,
                        holder_name = model.FullName,
                        status = "active",
                        delivered_to_email = model.Email,
                        issued_at = DateTime.Now
                    };
                    db.tickets.Add(ticket);
                }

                db.SaveChanges();

                return RedirectToAction("Success");
            }
            catch (Exception)
            {
                // Log ex
                return RedirectToAction("Failed");
            }
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Failed()
        {
            return View();
        }

        // GET: Ticket/Details/5 (ticket details for logged-in user)
        public ActionResult Details(long id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Register");

            long userId = Convert.ToInt64(Session["UserID"]);
            var user = db.users.FirstOrDefault(u => u.id == userId);
            if (user == null)
                return RedirectToAction("Login", "Register");

            var ticket = db.tickets.Find(id);
            if (ticket == null) return HttpNotFound();

            var oi = ticket.order_items;
            if (oi == null) return HttpNotFound();
            var order = oi.orders;
            if (order == null) return HttpNotFound();

            // Ownership check: order belongs to logged-in user or guest order with same email
            if (!(order.user_id == userId || (order.user_id == null && order.email == user.email)))
                return new HttpUnauthorizedResult();

            var perf = db.performances.FirstOrDefault(p => p.id == oi.performance_id);
            var ev = perf != null ? db.events.FirstOrDefault(e => e.id == perf.event_id) : null;
            var venue = perf != null ? db.venues.FirstOrDefault(v => v.id == perf.venue_id) : null;
            var city = venue != null ? db.cities.FirstOrDefault(c => c.id == venue.city_id) : null;
            var seat = oi.seat_id.HasValue ? db.seats.FirstOrDefault(s => s.id == oi.seat_id.Value) : null;

            var model = new TicketDetailsViewModel
            {
                TicketId = ticket.id,
                TicketCode = ticket.ticket_code,
                QR = ticket.qr_code_url,
                PurchasedAt = order.created_at,
                EventTitle = ev?.title,
                EventPoster = ev?.poster_url,
                StartDate = perf?.start_datetime,
                EndDate = perf?.end_datetime,
                VenueName = venue?.name,
                CityName = city?.name,
                Price = oi.unit_price,
                SeatRow = seat?.row_label,
                SeatNumber = seat?.seat_number
            };

            return View(model);
        }

        // POST: Ticket/RequestRefund
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestRefund(long ticketId)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Register");

            long userId = Convert.ToInt64(Session["UserID"]);
            var user = db.users.FirstOrDefault(u => u.id == userId);
            if (user == null)
                return RedirectToAction("Login", "Register");

            var ticket = db.tickets.Find(ticketId);
            if (ticket == null) return HttpNotFound();

            var oi = ticket.order_items;
            if (oi == null) return HttpNotFound();
            var order = oi.orders;
            if (order == null) return HttpNotFound();

            if (!(order.user_id == userId || (order.user_id == null && order.email == user.email)))
                return new HttpUnauthorizedResult();

            var payment = db.payments.FirstOrDefault(p => p.order_id == order.id);
            if (payment == null)
            {
                // Create a synthetic payment if missing (for older orders)
                payment = new payments
                {
                    order_id = order.id,
                    provider = "manual",
                    provider_payment_id = Guid.NewGuid().ToString(),
                    amount = order.total_amount,
                    currency = order.currency,
                    status = "captured",
                    captured_at = order.created_at ?? DateTime.Now
                };
                db.payments.Add(payment);
                db.SaveChanges();
            }

            // Prevent duplicate pending/approved refunds for same payment
            bool hasActiveRefund = db.refunds.Any(r => r.payment_id == payment.id && r.status != "rejected");
            if (!hasActiveRefund)
            {
                var refund = new refunds
                {
                    payment_id = payment.id,
                    amount = payment.amount,
                    status = "pending",
                    provider_refund_id = null,
                    processed_at = null
                };
                db.refunds.Add(refund);
                db.SaveChanges();
                TempData["Success"] = "Your refund request has been submitted and is pending review.";
            }
            else
            {
                TempData["Success"] = "A refund request already exists for this order.";
            }

            return RedirectToAction("Details", new { id = ticketId });
        }

        // Demo Data Seeder Helper
        private void SeedSeats(long performanceId, long venueId)
        {
            try
            {
                // 1. Ensure Seats exist for the Venue
                var existingSeats = db.seats.Where(s => s.venue_id == venueId).ToList();
                if (!existingSeats.Any())
                {
                    string[] rows = { "A", "B", "C" };
                    for (int i = 0; i < rows.Length; i++)
                    {
                        for (int num = 1; num <= 8; num++)
                        {
                            var seat = new seats
                            {
                                venue_id = venueId,
                                seatmap_section = "Main Hall",
                                row_label = rows[i],
                                seat_number = num.ToString(),
                                is_wheelchair = false
                            };
                            db.seats.Add(seat);
                        }
                    }
                    db.SaveChanges();
                    existingSeats = db.seats.Where(s => s.venue_id == venueId).ToList();
                }

                // 2. Ensure Price Tiers exist
                if (!db.price_tiers.Any(pt => pt.performance_id == performanceId))
                {
                    var tier = new price_tiers
                    {
                        performance_id = performanceId,
                        name = "Standard",
                        price = 150, // Default price
                        currency = "TRY",
                        seatmap_section = "Main Hall"
                    };
                    db.price_tiers.Add(tier);
                    db.SaveChanges();
                }

                // 3. Create Performance Seats (Available)
                foreach (var seat in existingSeats)
                {
                    var perfSeat = new performance_seats
                    {
                        performance_id = performanceId,
                        seat_id = seat.id,
                        status = "available",
                        price_tier_id = db.price_tiers.FirstOrDefault(pt => pt.performance_id == performanceId)?.id
                    };
                    db.performance_seats.Add(perfSeat);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                // Silent fail if seeding has issues
            }
        }
    }
}

