using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.ViewModels
{
    public class SelectSeat
    {
        public int ShowtimeId { get; set; }
        public int TheaterId { get; set; }
        public TheaterLayout TheaterLayout { get; set; }
        public Dictionary<int, int> TicketQuantities { get; set; } // TicketTypeId and Quantity
                                                                   // Add any other properties needed for seat selection
    }
}