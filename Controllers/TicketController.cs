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

            // Performance selection (for now first performance)
            var performance = eventItem.performances.OrderBy(p => p.start_datetime).FirstOrDefault();
            if (performance == null)
            {
                ViewBag.Message = "No scheduled performance found for this event.";
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

            var model = new CheckoutViewModel
            {
                PerformanceId = performanceId,
                SelectedSeatIds = selectedSeats,
                TotalAmount = totalAmount,
                EventTitle = perfInfo.events.title,
                SeatCount = seatIds.Count
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

