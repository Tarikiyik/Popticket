using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models; // This should be the namespace where Movie, Theater, and Showtime are defined.

namespace WebProject.ViewModels
{
    public class SelectTicketViewModel
    {
        public Movie Movie { get; set; }
        public List<Theater> Theaters { get; set; }
        public List<Showtime> Showtimes { get; set; }
        public List<TicketTypes> TicketTypes { get; set; } // Add this property
    }
}