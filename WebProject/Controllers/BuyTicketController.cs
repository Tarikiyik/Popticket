using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers
{
    public class BuyTicketController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();
        
        public ActionResult SelectTicket(int movieId)
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var movie = db.Movie.Find(movieId);
            var theaters = db.Theater.ToList();
            var showtimes = db.Showtime.Where(s => s.movieID == movieId).ToList();
            var ticketTypes = db.TicketTypes.ToList();

            var viewModel = new SelectTicketViewModel
            {
                Movie = movie,
                Theaters = theaters,
                Showtimes = showtimes,
                TicketTypes = ticketTypes // Make sure to add this to your ViewModel
            };

            return View(viewModel);
        }

        public ActionResult GetDates(int theaterId, int movieId)
        {
            try
            {
                var availableDates = db.Showtime
                    .Where(s => s.theaterID == theaterId && s.movieID == movieId)
                    .Select(s => s.date)
                    .Distinct()
                    .ToList();

                return Json(availableDates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(new { error = "An error occurred while fetching dates." });
            }
        }

        public ActionResult GetTimes(int theaterId, int movieId, DateTime date)
        {
            try
            {
                var availableTimes = db.Showtime
                    .Where(s => s.theaterID == theaterId && s.movieID == movieId && DbFunctions.TruncateTime(s.date) == DbFunctions.TruncateTime(date))
                    .Select(s => s.time)
                    .Distinct()
                    .OrderBy(t => t) // Optional: To order the times
                    .ToList();

                return Json(availableTimes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(new { error = "An error occurred while fetching times." });
            }
        }
    }
}
