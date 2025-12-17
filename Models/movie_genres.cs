








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class movie_genres
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public movie_genres()
        {
            this.events = new HashSet<events>();
        }
    
        public long id { get; set; }
        public string name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<events> events { get; set; }
    }
}


