








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class organizer_applications
    {
        public long id { get; set; }
        public Nullable<long> organizer_id { get; set; }
        public string org_name { get; set; }
        public string contact_email { get; set; }
        public Nullable<System.DateTime> submitted_at { get; set; }
        public string status { get; set; }
        public Nullable<long> reviewed_by_admin_id { get; set; }
        public string review_notes { get; set; }
        public Nullable<System.DateTime> decided_at { get; set; }
    
        public virtual admins admins { get; set; }
        public virtual organizers organizers { get; set; }
    }
}


