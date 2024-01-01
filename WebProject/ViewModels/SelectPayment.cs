using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class SelectPayment
    {
        public List<int> TicketTypeIds { get; set; }
        public List<int> TicketQuantities { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}