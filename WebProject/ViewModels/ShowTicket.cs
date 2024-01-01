using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.ViewModels
{
    public class ShowTicket
    { 
        public string MovieName { get; set; }
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public int ShowtimeId { get; set; }
        public List<string> SelectedSeatIds { get; set; }
        public List<int> TicketTypeIds { get; set; }
        public List<int> TicketQuantities { get; set; }
        public decimal TotalPrice { get; set; }
    }
}