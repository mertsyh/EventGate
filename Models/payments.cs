








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class payments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public payments()
        {
            this.refunds = new HashSet<refunds>();
        }
    
        public long id { get; set; }
        public long order_id { get; set; }
        public string provider { get; set; }
        public string provider_payment_id { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> captured_at { get; set; }
        public string failure_reason { get; set; }
    
        public virtual orders orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<refunds> refunds { get; set; }
    }
}


