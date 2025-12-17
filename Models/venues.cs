








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class venues
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public venues()
        {
            this.performances = new HashSet<performances>();
            this.seatmaps = new HashSet<seatmaps>();
            this.seats = new HashSet<seats>();
        }
    
        public long id { get; set; }
        public long city_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Nullable<decimal> latitude { get; set; }
        public Nullable<decimal> longitude { get; set; }
        public bool has_seating { get; set; }
    
        public virtual cities cities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<performances> performances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<seatmaps> seatmaps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<seats> seats { get; set; }
    }
}


