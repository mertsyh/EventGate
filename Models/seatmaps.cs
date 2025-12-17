








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class seatmaps
    {
        public long id { get; set; }
        public long venue_id { get; set; }
        public string name { get; set; }
        public string layout_json { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        public virtual venues venues { get; set; }
    }
}


