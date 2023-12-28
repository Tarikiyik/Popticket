using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;
namespace WebProject.ViewModels
{
    public class ConfirmTicketSelectionViewModel
    {
        public int ShowtimeId { get; set; }
        public List<TicketSelection> TicketSelections { get; set; }
    }

    public class TicketSelection
    {
        public int SeatId { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}