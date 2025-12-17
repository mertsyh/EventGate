








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class price_tiers
    {
        public long id { get; set; }
        public long performance_id { get; set; }
        public string name { get; set; }
        public string allocation_type { get; set; }
        public Nullable<int> capacity { get; set; }
        public decimal price { get; set; }
        public string currency { get; set; }
        public string seatmap_section { get; set; }
    
        public virtual performances performances { get; set; }
    }
}


