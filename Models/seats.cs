








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class seats
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public seats()
        {
            this.performance_seats = new HashSet<performance_seats>();
        }
    
        public long id { get; set; }
        public long venue_id { get; set; }
        public string seatmap_section { get; set; }
        public string row_label { get; set; }
        public string seat_number { get; set; }
        public Nullable<bool> is_wheelchair { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<performance_seats> performance_seats { get; set; }
        public virtual venues venues { get; set; }
    }
}


