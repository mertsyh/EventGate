using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventDeneme.Models
{
    public class TicketDetailsViewModel
    {
        public long TicketId { get; set; }
        public string TicketCode { get; set; }
        public string QR { get; set; }

        public DateTime? PurchasedAt { get; set; }

        public string EventTitle { get; set; }
        public string EventPoster { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string VenueName { get; set; }
        public string CityName { get; set; }

        public decimal? Price { get; set; }

        public string SeatRow { get; set; }
        public string SeatNumber { get; set; }
    }
}
