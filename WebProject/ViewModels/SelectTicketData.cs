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
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}