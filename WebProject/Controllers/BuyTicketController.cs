using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Net;
using System.Transactions;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
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
                return RedirectToAction("Homepage", "Home");

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

            // Instantiate ShowTicketViewModel
            var showTicket = new ShowTicket
            {
                MovieName = movie.title,
                MovieId = movie.MovieID,
            };

            // Store the ShowTicket in TempData
            TempData["ShowTicket"] = showTicket;
            TempData.Keep("ShowTicket"); // Ensure TempData is kept after a refresh

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

                if (!data.TicketQuantities.Any())
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
                    TotalQuantity = data.TicketQuantities.Sum(),
                    TotalPrice = data.TotalPrice,
                    OccupiedSeats = occupiedSeatsList,
                    TicketTypeIds = data.TicketTypeIds,
                    TicketQuantities = data.TicketQuantities
                };

                // Store the viewModel in TempData
                TempData["SelectSeat"] = viewModel;

                var ShowTicket = TempData["ShowTicket"] as ShowTicket;
                if (ShowTicket != null)
                {
                    ShowTicket.TheaterId = data.TheaterId; // Save TheaterID for later lookup
                    ShowTicket.ShowtimeId = showtime.showtimeID;
                    ShowTicket.TotalPrice = data.TotalPrice;
                    ShowTicket.TicketQuantities = data.TicketQuantities;
                    ShowTicket.TicketTypeIds = data.TicketTypeIds;
                }
                TempData["ShowTicket"] = ShowTicket; // Re-save the ViewModel in TempData
                TempData.Keep("ShowTicket");

                // Return a JsonResult indicating success and the next action to redirect to
                return Json(new { success = true, redirectUrl = Url.Action("SelectSeat", "BuyTicket") });
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
                return RedirectToAction("Homepage", "Home");

            // Retrieve the ViewModel from TempData
            var viewModel = TempData["SelectSeat"] as SelectSeat;

            if (viewModel == null)
            {
                // Log the error here
                return RedirectToAction("Error", new { message = "Seat selection data is not available." });
            }

            // Render the SelectSeat view with the viewModel
            return View(viewModel);
        }

        private int ConvertAlphanumericToNumericSeatId(string alphanumericSeatId, int theaterLayoutId)
        {
            // This method should query your database to find the seat with the given alphanumeric ID and layout ID
            // and return the numeric seat ID
            var seat = db.Seats.FirstOrDefault(s => s.SeatRowLetter + s.SeatRowNumber.ToString() == alphanumericSeatId && s.TheaterLayoutID == theaterLayoutId);
            if (seat != null)
            {
                return seat.SeatID;
            }
            else
            {
                // Handle the case where the seat is not found, perhaps by logging and throwing an exception
                throw new Exception("Seat ID not found for given alphanumeric ID and theater layout ID.");
            }
        }

        [HttpPost]
        public JsonResult PrepareForPayment(SelectSeatData data)
        {
            try
            {
                if (Session["User"] == null)
                {
                    return Json(new { success = false, message = "User session has expired." });
                }

                var user = Session["User"] as User;
                int userId = user.UserID;

                // Convert alphanumeric seat IDs to numeric IDs based on the theater layout
                List<int> numericSeatIds = data.SelectedSeatIds.Select(alphanumericId => ConvertAlphanumericToNumericSeatId(alphanumericId, data.TheaterLayoutId)).ToList();

                // Start a database transaction
                using (var transaction = new TransactionScope())
                {
                    // Retrieve all bookings for this user
                    var userBookings = db.Bookings.Where(b => b.userID == userId).ToList();

                    // Find all pending reservations from these bookings
                    var pendingReservations = userBookings
                        .SelectMany(b => b.seatReservations)
                        .Where(sr => sr.status == "pending")
                        .ToList();

                    // Remove these pending reservations
                    db.seatReservations.RemoveRange(pendingReservations);

                    // Check if the selected seats are already reserved
                    var alreadyReservedSeats = db.seatReservations
                        .Where(sr => sr.showtimeID == data.ShowtimeId && data.SelectedSeatIds.Contains(sr.seatID.ToString()))
                        .ToList();

                    if (alreadyReservedSeats.Any(sr => sr.status == "reserved"))
                    {
                        return Json(new { success = false, message = "Some of the selected seats are already reserved." });
                    }

                    // Create temporary seat reservations with a status of "pending"
                    foreach (var seatId in numericSeatIds)
                    {
                        var tempReservation = new seatReservations
                        {
                            showtimeID = data.ShowtimeId,
                            userID = userId,
                            seatID = seatId,
                            status = "pending", // Mark the reservation as pending
                            createdTime = DateTime.Now
                        };

                        db.seatReservations.Add(tempReservation);
                    }

                    // Save the changes to the database
                    db.SaveChanges();

                    // Complete the transaction
                    transaction.Complete();
                }

                var selectPayment = new SelectPayment
                {
                    TotalQuantity = data.TicketQuantities.Sum(),
                    TicketQuantities = data.TicketQuantities,
                    TicketTypeIds = data.TicketTypeIds,
                    TotalPrice = data.TotalPrice,
                };

                // Store the payment data in TempData or Session
                TempData["SelectPayment"] = selectPayment;
                TempData.Keep("SelectPayment");

                var showTicket = TempData["ShowTicket"] as ShowTicket;
                if (showTicket != null)
                {
                    showTicket.SelectedSeatIds = data.SelectedSeatIds;
                }
                TempData["ShowTicket"] = showTicket;
                TempData.Keep("ShowTicket");

                // Return a JsonResult indicating success and the next action to redirect to
                string paymentPageUrl = Url.Action("SelectPayment");
                return Json(new { success = true, redirectUrl = paymentPageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public ActionResult SelectPayment()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Homepage", "Home");
            }
            var user = Session["User"] as User;
            int userId = user.UserID;

            // Retrieve the SelectPayment ViewModel from TempData
            var selectPayment = TempData["SelectPayment"] as SelectPayment;

            if (selectPayment == null)
            {
                // If the ViewModel is not found, redirect to an error page or a relevant action
                return RedirectToAction("Error", new { message = "Payment data is not available." });
            }

            CleanUpPendingReservations(userId);

            // Render the SelectPayment view with the ViewModel
            return View(selectPayment);
        }

        [HttpPost]
        public ActionResult ClearPendingReservations()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Homepage", "Home");
            }
            var user = Session["User"] as User;
            int userId = user.UserID;

            CleanUpPendingReservationsOnLeave(userId);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult HandleTimeout()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Homepage", "Home");
            }
            var user = Session["User"] as User;

            // Perform cleanup
            CleanUpPendingReservationsOnLeave(user.UserID);

            // You can return a simple success response, as the client-side will handle the redirection
            return Json(new { success = true });
        }

        private void CleanUpPendingReservations(int userId)
        {
            // Define the time limit for pending reservations
            int reservationTimeLimit = 5;

            // Find all pending reservations for the user that are older than the limit
            var outdatedReservations = db.seatReservations
                .Where(sr => sr.status == "pending" &&
                             sr.userID == userId &&
                             DbFunctions.AddMinutes(sr.createdTime, reservationTimeLimit) < DateTime.Now);

            // Remove the outdated reservations
            db.seatReservations.RemoveRange(outdatedReservations);
            db.SaveChanges();
        }

        private void CleanUpPendingReservationsOnLeave(int userId)
        {
            // Find all pending reservations for the user that are older than the limit
            var pendingReservations = db.seatReservations
                .Where(sr => sr.status == "pending" &&
                             sr.userID == userId);

            // Remove the outdated reservations
            db.seatReservations.RemoveRange(pendingReservations);
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult ConfirmPayment()
        {
            if (Session["User"] == null)
            {
                return Json(new { success = false, message = "User is not logged in." });
            }

            var user = Session["User"] as User;
            int userId = user.UserID;

            // Update the status of pending reservations to 'reserved'
            var pendingReservations = db.seatReservations
                .Where(sr => sr.userID == userId && sr.status == "pending")
                .ToList();

            foreach (var reservation in pendingReservations)
            {
                reservation.status = "reserved";
                reservation.createdTime = DateTime.Now; // Update the creation time to the current time
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Payment confirmed and reservations updated." });
        }

        public ActionResult ShowTicket()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Homepage", "Home");
            }
            var user = Session["User"] as User;

            var showTicketViewModel = TempData["ShowTicket"] as ShowTicket;

            if (showTicketViewModel == null)
            {
                // If the ViewModel is not found, redirect to an error page or a relevant action
                return RedirectToAction("Error", new { message = "ShowTicket data is not available." });
            }

            // Fetch additional details from the database
            var theater = db.Theater.Find(showTicketViewModel.TheaterId);
            var showtime = db.Showtime.Find(showTicketViewModel.ShowtimeId);

            if (theater == null || showtime == null)
            {
                // Handle missing theater or showtime data
                return RedirectToAction("Error", new { message = "Theater or Showtime information is missing." });
            }

            // Populate additional details in the ViewModel
            showTicketViewModel.TheaterName = theater.name;
            showTicketViewModel.ShowtimeDetails = $"{showtime.date.ToString("yyyy-MM-dd")} at {showtime.time}";

            var ticketDetails = showTicketViewModel.TicketTypeIds.Zip(showTicketViewModel.TicketQuantities, (id, quantity) =>
                    new { TicketTypeId = id, Quantity = quantity }).ToList();

            Bookings newBooking = new Bookings
            {
                userID = user.UserID,
                movieID = showTicketViewModel.MovieId,
                showtimeID = showTicketViewModel.ShowtimeId,
                theaterID = showTicketViewModel.TheaterId,
                selectedSeats = JsonConvert.SerializeObject(showTicketViewModel.SelectedSeatIds),
                ticketTypeQuantities = JsonConvert.SerializeObject(ticketDetails),
                totalPrice = showTicketViewModel.TotalPrice,
                bookingTime = DateTime.Now
            };

            db.Bookings.Add(newBooking);
            db.SaveChanges();


            // Render the ShowTicket view with the populated ViewModel
            return View(showTicketViewModel);
        }
    }
}
