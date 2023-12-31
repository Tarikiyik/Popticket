using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class SelectSeatData
    {
        public List<string> SelectedSeatIds { get; set; }
        public int ShowtimeId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}