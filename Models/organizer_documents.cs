








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class organizer_documents
    {
        public long id { get; set; }
        public long application_id { get; set; }
        public string file_url { get; set; }
        public string doc_type { get; set; }
        public Nullable<System.DateTime> uploaded_at { get; set; }
    }
}


