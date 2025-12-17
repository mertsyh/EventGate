








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class refunds
    {
        public long id { get; set; }
        public long payment_id { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public string provider_refund_id { get; set; }
        public Nullable<System.DateTime> processed_at { get; set; }
    
        public virtual payments payments { get; set; }
    }
}


