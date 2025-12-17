








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class categories
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public categories()
        {
            this.events = new HashSet<events>();
        }
    
        public int id { get; set; }
        public string category_key { get; set; }
        public string display_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<events> events { get; set; }
    }
}


