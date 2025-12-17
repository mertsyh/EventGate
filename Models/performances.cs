








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class performances
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public performances()
        {
            this.performance_seats = new HashSet<performance_seats>();
            this.price_tiers = new HashSet<price_tiers>();
        }
    
        public long id { get; set; }
        public long event_id { get; set; }
        public long venue_id { get; set; }
        public System.DateTime start_datetime { get; set; }
        public Nullable<System.DateTime> end_datetime { get; set; }
        public Nullable<System.DateTime> sales_start { get; set; }
        public Nullable<System.DateTime> sales_end { get; set; }
        public string status { get; set; }
    
        public virtual events events { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<performance_seats> performance_seats { get; set; }
        public virtual venues venues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<price_tiers> price_tiers { get; set; }
    }
}


