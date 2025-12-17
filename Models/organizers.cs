








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class organizers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public organizers()
        {
            this.events = new HashSet<events>();
            this.organizer_applications = new HashSet<organizer_applications>();
            this.organizer_users = new HashSet<organizer_users>();
        }
    
        public long id { get; set; }
        public string legal_name { get; set; }
        public string brand_name { get; set; }
        public string tax_id { get; set; }
        public string contact_email { get; set; }
        public string contact_phone { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<events> events { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<organizer_applications> organizer_applications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<organizer_users> organizer_users { get; set; }
    }
}


