








namespace EventDeneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class events
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public events()
        {
            this.moderation_events = new HashSet<moderation_events>();
            this.performances = new HashSet<performances>();
            this.movie_genres = new HashSet<movie_genres>();
            this.music_genres = new HashSet<music_genres>();
            this.theatre_genres = new HashSet<theatre_genres>();
        }
    
        public long id { get; set; }
        public int category_id { get; set; }
        public long organizer_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string language { get; set; }
        public string age_limit { get; set; }
        public string poster_url { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    
        public virtual categories categories { get; set; }
        public virtual concert_details concert_details { get; set; }
        public virtual organizers organizers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<moderation_events> moderation_events { get; set; }
        public virtual movie_details movie_details { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<performances> performances { get; set; }
        public virtual theatre_details theatre_details { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<movie_genres> movie_genres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<music_genres> music_genres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<theatre_genres> theatre_genres { get; set; }
    }
}


