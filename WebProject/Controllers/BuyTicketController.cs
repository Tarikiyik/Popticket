using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Transactions;
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
        public JsonResult PrepareSelectSeats(SelectTicketData data)
        {
            try
            {
                DateTime selectedDate = DateTime.ParseExact(data.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var showtime = db.Showtime.FirstOrDefault(s => s.theaterID == data.TheaterId &&
                                                               DbFunctions.TruncateTime(s.date) == selectedDate &&
                                                               s.time == data.Time);

                if (showtime == null)
                    return Json(new { success = false, message = "Showtime not found." });

                var theaterLayout = db.TheaterLayouts.FirstOrDefault(tl => tl.TheaterID == data.TheaterId);
                if (theaterLayout == null)
                    return Json(new { success = false, message = "Theater layout not found." });

                if (data.TotalQuantity == 0)
                    return Json(new { success = false, message = "Ticket quantity not found." });

                if (data.TotalPrice == 0)
                    return Json(new { success = false, message = "Total price not found." });

                var occupiedSeatsQueryResults = db.seatReservations
                    .Where(sr => sr.showtimeID == showtime.showtimeID)
                    .ToList();

                List<string> occupiedSeatsList = occupiedSeatsQueryResults
                    .Select(sr => $"{sr.Seats.SeatRowLetter}{sr.Seats.SeatRowNumber}")
                    .ToList();

                // Prepare the ViewModel for the SelectSeat view
                SelectSeat viewModel = new SelectSeat
                {
                    ShowtimeId = showtime.showtimeID,
                    TheaterId = data.TheaterId,
                    TheaterLayout = theaterLayout,
                    TotalQuantity = data.TotalQuantity,
                    TotalPrice = data.TotalPrice,
                    OccupiedSeats = occupiedSeatsList
                };

                // Store the viewModel in TempData
                TempData["SelectSeatData"] = viewModel;

                // Return a JsonResult indicating success and the next action to redirect to
                return Json(new { success = true, redirectAction = Url.Action("SelectSeat") });
            }
            catch (Exception ex)
            {
                // Log the exception here
                return Json(new { success = false, message = "An error occurred. " + ex.Message });
            }
        }

        public ActionResult SelectSeat()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            // Retrieve the ViewModel from TempData
            var viewModel = TempData["SelectSeatData"] as SelectSeat;

            if (viewModel == null)
            {
                // Log the error here
                return RedirectToAction("Error", new { message = "Seat selection data is not available." });
            }

            // Render the SelectSeat view with the viewModel
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult PrepareForPayment(SelectSeatData data)
        {
            try
            {
                var user = Session["User"] as User;
                int userId = user.UserID; // Ensure the property name matches your User model

                // Start a database transaction
                using (var transaction = new TransactionScope())
                {
                    // Check if the selected seats are already reserved
                    var alreadyReservedSeats = db.seatReservations
                        .Where(sr => sr.showtimeID == data.ShowtimeId && data.SelectedSeatIds.Contains(sr.seatID.ToString()))
                        .ToList();

                    if (alreadyReservedSeats.Any(sr => sr.status == "reserved"))
                    {
                        return Json(new { success = false, message = "Some of the selected seats are already reserved." });
                    }

                    // Create temporary seat reservations with a status of "pending"
                    foreach (var seatId in data.SelectedSeatIds)
                    {
                        var tempReservation = new seatReservations
                        {
                            showtimeID = data.ShowtimeId,
                            seatID = int.Parse(seatId),
                            status = "pending" // Mark the reservation as pending
                        };

                        db.seatReservations.Add(tempReservation);
                    }

                    // Save the changes to the database
                    db.SaveChanges();

                    // Complete the transaction
                    transaction.Complete();
                }

                // Prepare the data to be passed to the payment page
                var paymentData = new
                {
                    ShowtimeId = data.ShowtimeId,
                    SelectedSeatIds = data.SelectedSeatIds,
                    TotalQuantity = data.TotalQuantity,
                    TotalPrice = data.TotalPrice
                };

                // Store the payment data in TempData or Session
                TempData["PaymentData"] = paymentData;

                // Return a JsonResult indicating success and the next action to redirect to
                string paymentPageUrl = Url.Action("Payment");
                return Json(new { success = true, redirectUrl = paymentPageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
    }
}
