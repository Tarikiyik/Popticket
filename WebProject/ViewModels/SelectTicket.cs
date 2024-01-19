using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models; 

namespace WebProject.ViewModels
{
    public class SelectTicket
    {
        public Movie Movie { get; set; }
        public List<Theater> Theaters { get; set; }
        public List<Showtime> Showtimes { get; set; }
        public List<TicketTypes> TicketTypes { get; set; }
        public List<Cities> Cities { get; set; }
    }
}