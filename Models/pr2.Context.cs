








namespace EventDeneme.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class pr2Entities1 : DbContext
    {
        public pr2Entities1()
            : base("name=pr2Entities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<admins> admins { get; set; }
        public virtual DbSet<cart_items> cart_items { get; set; }
        public virtual DbSet<carts> carts { get; set; }
        public virtual DbSet<categories> categories { get; set; }
        public virtual DbSet<cities> cities { get; set; }
        public virtual DbSet<concert_details> concert_details { get; set; }
        public virtual DbSet<events> events { get; set; }
        public virtual DbSet<moderation_events> moderation_events { get; set; }
        public virtual DbSet<movie_details> movie_details { get; set; }
        public virtual DbSet<movie_genres> movie_genres { get; set; }
        public virtual DbSet<music_genres> music_genres { get; set; }
        public virtual DbSet<order_items> order_items { get; set; }
        public virtual DbSet<orders> orders { get; set; }
        public virtual DbSet<organizer_applications> organizer_applications { get; set; }
        public virtual DbSet<organizer_documents> organizer_documents { get; set; }
        public virtual DbSet<organizer_users> organizer_users { get; set; }
        public virtual DbSet<organizers> organizers { get; set; }
        public virtual DbSet<payments> payments { get; set; }
        public virtual DbSet<performance_seats> performance_seats { get; set; }
        public virtual DbSet<performances> performances { get; set; }
        public virtual DbSet<price_tiers> price_tiers { get; set; }
        public virtual DbSet<refunds> refunds { get; set; }
        public virtual DbSet<search_queries> search_queries { get; set; }
        public virtual DbSet<seat_holds> seat_holds { get; set; }
        public virtual DbSet<seatmaps> seatmaps { get; set; }
        public virtual DbSet<seats> seats { get; set; }
        public virtual DbSet<sessions> sessions { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<theatre_details> theatre_details { get; set; }
        public virtual DbSet<theatre_genres> theatre_genres { get; set; }
        public virtual DbSet<tickets> tickets { get; set; }
        public virtual DbSet<user_payment_methods> user_payment_methods { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<venues> venues { get; set; }
    }
}


