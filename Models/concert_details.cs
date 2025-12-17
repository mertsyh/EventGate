








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class concert_details
    {
        public long event_id { get; set; }
        public Nullable<int> duration_min { get; set; }
        public string headliner { get; set; }
        public string opener { get; set; }
        public Nullable<long> music_genre_main_id { get; set; }
    
        public virtual events events { get; set; }
    }
}


