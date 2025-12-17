








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class moderation_events
    {
        public long id { get; set; }
        public long event_id { get; set; }
        public long submitted_by { get; set; }
        public string status { get; set; }
        public Nullable<long> reviewed_by { get; set; }
        public Nullable<System.DateTime> reviewed_at { get; set; }
        public string notes { get; set; }
    
        public virtual admins admins { get; set; }
        public virtual events events { get; set; }
        public virtual organizer_users organizer_users { get; set; }
    }
}


