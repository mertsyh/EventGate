








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tickets
    {
        public long id { get; set; }
        public long order_item_id { get; set; }
        public string ticket_code { get; set; }
        public string qr_code_url { get; set; }
        public string holder_name { get; set; }
        public string status { get; set; }
        public string delivered_to_email { get; set; }
        public Nullable<System.DateTime> issued_at { get; set; }
    
        public virtual order_items order_items { get; set; }
    }
}


