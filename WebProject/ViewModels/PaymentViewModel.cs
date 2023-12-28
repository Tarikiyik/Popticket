using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;
namespace WebProject.ViewModels
{
    public class PaymentViewModel
    {
        public int BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        // Other properties related to payment
    }
}