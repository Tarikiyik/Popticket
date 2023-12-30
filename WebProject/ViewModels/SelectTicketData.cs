using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.ViewModels
{
    public class SelectTicketData
    {
        public int TheaterId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public List<TicketSelection> Tickets { get; set; }
    }

    public class TicketSelection
    {
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}