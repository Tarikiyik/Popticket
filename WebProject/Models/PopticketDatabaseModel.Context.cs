﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebProject.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PopTicketEntities : DbContext
    {
        public PopTicketEntities()
            : base("name=PopTicketEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BookingDetails> BookingDetails { get; set; }
        public virtual DbSet<Bookings> Bookings { get; set; }
        public virtual DbSet<CardDetails> CardDetails { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Movie> Movie { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<seatReservations> seatReservations { get; set; }
        public virtual DbSet<Seats> Seats { get; set; }
        public virtual DbSet<Showtime> Showtime { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Theater> Theater { get; set; }
        public virtual DbSet<TicketTypes> TicketTypes { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
