








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class performance_seats
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public performance_seats()
        {
            this.seat_holds = new HashSet<seat_holds>();
        }
    
        public long id { get; set; }
        public long performance_id { get; set; }
        public long seat_id { get; set; }
        public Nullable<long> price_tier_id { get; set; }
        public string status { get; set; }
    
        public virtual performances performances { get; set; }
        public virtual seats seats { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<seat_holds> seat_holds { get; set; }
    }
}


