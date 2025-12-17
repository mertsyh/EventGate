








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class seat_holds
    {
        public long id { get; set; }
        public long performance_seat_id { get; set; }
        public long cart_id { get; set; }
        public System.DateTime expires_at { get; set; }
    
        public virtual carts carts { get; set; }
        public virtual performance_seats performance_seats { get; set; }
    }
}


