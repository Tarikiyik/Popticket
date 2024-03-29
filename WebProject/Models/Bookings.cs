//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Bookings
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bookings()
        {
            this.seatReservations = new HashSet<seatReservations>();
        }
    
        public int bookingID { get; set; }
        public int userID { get; set; }
        public int movieID { get; set; }
        public int theaterID { get; set; }
        public int showtimeID { get; set; }
        public System.DateTime bookingTime { get; set; }
        public string selectedSeats { get; set; }
        public string ticketTypeQuantities { get; set; }
        public decimal totalPrice { get; set; }
    
        public virtual Movie Movie { get; set; }
        public virtual Showtime Showtime { get; set; }
        public virtual Theater Theater { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<seatReservations> seatReservations { get; set; }
    }
}
