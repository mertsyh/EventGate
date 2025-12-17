








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class user_payment_methods
    {
        public long id { get; set; }
        public long user_id { get; set; }
        public string provider { get; set; }
        public string token { get; set; }
        public string brand { get; set; }
        public string last4 { get; set; }
        public Nullable<int> exp_month { get; set; }
        public Nullable<int> exp_year { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        public virtual users users { get; set; }
    }
}


