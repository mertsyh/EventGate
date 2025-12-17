








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orders()
        {
            this.order_items = new HashSet<order_items>();
            this.payments = new HashSet<payments>();
        }
    
        public long id { get; set; }
        public Nullable<long> user_id { get; set; }
        public Nullable<long> session_id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public decimal total_amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public string payment_intent_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_items> order_items { get; set; }
        public virtual sessions sessions { get; set; }
        public virtual users users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<payments> payments { get; set; }
    }
}


