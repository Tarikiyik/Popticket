using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class SelectSeat
    {
        public int ShowtimeId { get; set; }
        public int TheaterId { get; set; }
        public TheaterLayouts TheaterLayout { get; set; }
        public List<string> OccupiedSeats { get; set; } // This will store seat identifiers like "A1", "A2", etc.
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public List<int> TicketTypeIds { get; set; }
        public List<int> TicketQuantities { get; set; }

    }
}