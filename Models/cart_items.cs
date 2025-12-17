








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class cart_items
    {
        public long id { get; set; }
        public long cart_id { get; set; }
        public long performance_id { get; set; }
        public long price_tier_id { get; set; }
        public Nullable<long> seat_id { get; set; }
        public int quantity { get; set; }
        public decimal unit_price { get; set; }
        public string status { get; set; }
    
        public virtual carts carts { get; set; }
    }
}


