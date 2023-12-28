using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class BuyTicketController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();

        // GET: BuyTicket
        public ActionResult SelectTicket()
        {
            return View();
        }
        public ActionResult SelectSeat()
        {
            return View();
        }
        public ActionResult SelectPayment()
        {
            return View();
        }
        public ActionResult ShowTicket()
        {
            return View();
        }
    }
}