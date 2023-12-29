using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
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
            var cities = db.Cities.ToList();

            var viewModel = new SelectTicketViewModel
            {
                Movie = movie,
                Theaters = theaters,
                Showtimes = showtimes,
                TicketTypes = ticketTypes,
                Cities = cities
            };


            return View(viewModel);
        }

        public ActionResult GetTheatersByCity(int? cityId)
        {
            try
            {
                var theatersQuery = db.Theater.AsQueryable();

                if (cityId.HasValue)
                {
                    theatersQuery = theatersQuery.Where(t => t.cityID == cityId.Value);
                }

                var theaters = theatersQuery.ToList();
                return Json(new { success = true, theaters = theaters.Select(t => new { t.theaterID, t.name, t.address, t.cityID }) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDates(int theaterId, int movieId)
        {
            try
            {
                var availableDates = db.Showtime
                    .Where(s => s.theaterID == theaterId && s.movieID == movieId)
                    .Select(s => s.date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList()
                    .Select(d => d.ToString("yyyy-MM-dd")); // Format dates into a standard ISO format

                return Json(new { success = true, availableDates }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(new { success = false, error = "An error occurred while fetching dates." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTimes(int theaterId, int movieId, string date)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var availableTimes = db.Showtime
                    .Where(s => s.theaterID == theaterId && s.movieID == movieId && DbFunctions.TruncateTime(s.date) == parsedDate)
                    .Select(s => s.time)
                    .Distinct()
                    .OrderBy(t => t) // Times are already in "HH:mm" format
                    .ToList();

                return Json(new { success = true, availableTimes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                return Json(new { success = false, error = "An error occurred while fetching times." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
