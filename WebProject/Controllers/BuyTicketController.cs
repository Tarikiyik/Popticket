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

            var viewModel = new SelectTicket
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

        public ActionResult GetOccupiedSeats(int showtimeId)
        {
            // Perform the query and materialize the results with ToList()
            var occupiedSeatsQueryResults = db.seatReservations
                .Where(sr => sr.showtimeID == showtimeId)
                .ToList(); // Materialize query results

            // Now that you have the results in memory, you can use string interpolation
            List<string> occupiedSeatsList = occupiedSeatsQueryResults
                .Select(sr => $"{sr.Seats.SeatRowLetter}{sr.Seats.SeatRowNumber}")
                .ToList();

            return Json(new { occupiedSeats = occupiedSeatsList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SelectSeats(SelectTicketData data)
        {
            // Ensure date is in "yyyy-MM-dd" format and time is in "HH:mm" format
            DateTime selectedDate = DateTime.ParseExact(data.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // Retrieve the showtime ID based on the theaterId, date, and time
            var showtime = db.Showtime.FirstOrDefault(s => s.theaterID == data.TheaterId &&
                                                           DbFunctions.TruncateTime(s.date) == selectedDate &&
                                                           s.time == data.Time);

            if (showtime == null)
            {
                // Handle case where showtime is not found
                return RedirectToAction("Error", new { message = "Showtime not found." });
            }

            // Retrieve theater layout
            var theaterLayout = db.TheaterLayouts.FirstOrDefault(tl => tl.TheaterID == data.TheaterId);
            if (theaterLayout == null)
            {
                // Handle case where theater layout is not found
                return RedirectToAction("Error", new { message = "Theater layout not found." });
            }

            var totalQuantity = data.TotalQuantity;
            if (totalQuantity == 0)
            {
                // Handle case where theater layout is not found
                return RedirectToAction("Error", new { message = "Ticket quantity not found." });
            }

            var totalPrice = data.TotalPrice;
            if (totalPrice == 0)
            {
                // Handle case where theater layout is not found
                return RedirectToAction("Error", new { message = "Total price not found." });
            }

            var occupiedSeatsQueryResults = db.seatReservations
                .Where(sr => sr.showtimeID == showtime.showtimeID)
                .ToList(); // Materialize query results

            List<string> occupiedSeatsList;
            if (occupiedSeatsQueryResults.Any())
            {
                // Use string interpolation after materializing the query results
                occupiedSeatsList = occupiedSeatsQueryResults
                    .Select(sr => $"{sr.Seats.SeatRowLetter}{sr.Seats.SeatRowNumber}")
                    .ToList();
            }
            else
            {
                // If there are no occupied seats, initialize an empty list
                occupiedSeatsList = new List<string>();
            }


            // Prepare the ViewModel for the SelectSeat view
            SelectSeat viewModel = new SelectSeat
            {
                ShowtimeId = showtime.showtimeID,
                TheaterId = data.TheaterId,
                TheaterLayout = theaterLayout,
                TotalQuantity = totalQuantity,
                TotalPrice = totalPrice,
                OccupiedSeats = occupiedSeatsList
            };

            // Pass the ViewModel to the SelectSeat view
            return View("SelectSeat", viewModel);
        }


    }
}
