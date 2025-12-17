using System;
using System.Collections.Generic;

namespace EventDeneme.Models
{
    public class CartItemViewModel
    {
        public long CartItemId { get; set; }
        public long PerformanceSeatId { get; set; }
        public long PerformanceId { get; set; }
        public long EventId { get; set; }
        public string EventTitle { get; set; }
        public string EventImageUrl { get; set; }
        public DateTime? EventDate { get; set; }
        public string VenueName { get; set; }
        public string CityName { get; set; }
        public string Section { get; set; }
        public string Row { get; set; }
        public string SeatNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CartViewModel
    {
        public long CartId { get; set; }
        public List<CartItemViewModel> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}



