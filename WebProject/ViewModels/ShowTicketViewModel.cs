using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;
namespace WebProject.ViewModels
{
    public class ShowTicketViewModel
    {
        public string MovieName { get; set; }
        public string TheaterName { get; set; }
        public DateTime ShowtimeDate { get; set; }
        public TimeSpan ShowtimeTime { get; set; }
        public List<string> Seats { get; set; }
        public decimal TotalPrice { get; set; }
        // Other properties related to the show ticket
    }
}