using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventDeneme.Models
{
    public class EventDetailViewModel
    {
        public long EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Venue { get; set; }
        public string City { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
    }

}