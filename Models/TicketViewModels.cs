using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventDeneme.Models
{
    public class SeatSelectionViewModel
    {
        public long EventId { get; set; }
        public long PerformanceId { get; set; }
        public string EventTitle { get; set; }
        public string VenueName { get; set; }
        public DateTime StartDate { get; set; }
        public List<SeatViewModel> AvailableSeats { get; set; }
    }

    public class SeatViewModel
    {
        public long PerformanceSeatId { get; set; }
        public string Section { get; set; }
        public string Row { get; set; }
        public string Number { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } // available, sold, etc.
    }

    public class CheckoutViewModel
    {
        public long PerformanceId { get; set; }
        public string SelectedSeatIds { get; set; } // Comma separated IDs
        public decimal TotalAmount { get; set; }
        public string EventTitle { get; set; } // Display purpose
        public int SeatCount { get; set; } // Display purpose

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        [Display(Name = "Expiry Date (MM/YY)")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [Display(Name = "CVV")]
        public string CVV { get; set; }
    }

    public class UserTicketViewModel
    {
        public long TicketId { get; set; }
        public string EventTitle { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public string SeatLabel { get; set; }
        public string TicketCode { get; set; }
        public string QrUrl { get; set; }
        public string HolderName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }

    public class UserRefundViewModel
    {
        public long RefundId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}

