








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class movie_details
    {
        public long event_id { get; set; }
        public Nullable<int> duration_min { get; set; }
        public string imdb_id { get; set; }
        public string parental_rating { get; set; }
        public string distributor { get; set; }
    
        public virtual events events { get; set; }
    }
}


