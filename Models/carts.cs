








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class carts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public carts()
        {
            this.cart_items = new HashSet<cart_items>();
            this.seat_holds = new HashSet<seat_holds>();
        }
    
        public long id { get; set; }
        public Nullable<long> user_id { get; set; }
        public Nullable<long> session_id { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart_items> cart_items { get; set; }
        public virtual sessions sessions { get; set; }
        public virtual users users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<seat_holds> seat_holds { get; set; }
    }
}


