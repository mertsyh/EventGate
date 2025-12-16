using System;
using System.ComponentModel.DataAnnotations;

namespace EventDeneme.Models
{
    public class VenueViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [Display(Name = "Venue Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please select a city.")]
        [Display(Name = "City")]
        public long CityId { get; set; }

        [Display(Name = "Latitude")]
        public decimal? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public decimal? Longitude { get; set; }

        [Display(Name = "Has Seating?")]
        public bool HasSeating { get; set; }
    }
}
