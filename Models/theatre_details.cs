








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class theatre_details
    {
        public long event_id { get; set; }
        public Nullable<int> duration_min { get; set; }
        public string playwright { get; set; }
        public string director { get; set; }
        public Nullable<int> intermission_count { get; set; }
    
        public virtual events events { get; set; }
    }
}


