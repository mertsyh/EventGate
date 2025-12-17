








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class search_queries
    {
        public long id { get; set; }
        public long session_id { get; set; }
        public Nullable<long> user_id { get; set; }
        public Nullable<long> city_id { get; set; }
        public Nullable<int> category_id { get; set; }
        public Nullable<long> venue_id { get; set; }
        public Nullable<System.DateTime> date_from { get; set; }
        public Nullable<System.DateTime> date_to { get; set; }
        public string text_query { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        public virtual sessions sessions { get; set; }
    }
}


