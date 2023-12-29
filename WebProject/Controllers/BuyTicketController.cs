using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using WebProject.ViewModels;
using System.Data.Entity;

namespace WebProject.Controllers
{
    public class BuyTicketController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();

        // GET: BuyTicket

        public ActionResult SelectTicket(int movieId)
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var movie = db.Movie.Find(movieId);
            if (movie == null)
            {
                return HttpNotFound(); // Ensure the movie exists
            }

            var theaters = db.Theater.ToList();
            var showtimes = db.Showtime.Where(s => s.movieID == movieId).ToList();
            var ticketTypes = db.TicketTypes.ToList();

            var viewModel = new SelectTicketViewModel
            {
                Movie = movie,
                Theaters = theaters,
                Showtimes = showtimes,
                TicketTypes = ticketTypes
            };

            return View(viewModel);
        }

        public ActionResult GetTimesForDate(int movieId, DateTime date)
        {
            var times = db.Showtime
                          .Where(s => s.movieID == movieId && DbFunctions.TruncateTime(s.date) == date.Date)
                          .Select(s => new { s.time }) // Retrieve the time as-is
                          .AsEnumerable() // Switch to LINQ to Objects
                          .Select(s => s.time.ToString("HH:mm")) // Now we can format
                          .ToList();

            return Json(times, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmTicketSelection(ConfirmTicketSelectionViewModel model)
        {
            if (Session["User"] == null)
                return Json(new { success = false, error = "User not logged in." });

            try
            {
                var user = Session["User"] as User;
                var booking = new Bookings
                {
                    userID = user.UserID,
                    showtimeID = model.ShowtimeId,
                    bookingTime = DateTime.Now
                };

                db.Bookings.Add(booking);
                db.SaveChanges();

                // Add logic to handle booking details and seat reservations
                foreach (var ticketSelection in model.TicketSelections)
                {
                    var bookingDetail = new BookingDetails
                    {
                        bookingID = booking.bookingID,
                        ticketTypeID = ticketSelection.TicketTypeId,
                        quantity = ticketSelection.Quantity
                    };
                    db.BookingDetails.Add(bookingDetail);
                    // Repeat for seat reservations if needed
                }
                db.SaveChanges();

                return Json(new { success = true, redirectUrl = Url.Action("SelectSeat", new { bookingId = booking.bookingID }) });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, error = ex.Message });
            }
        }
        private void AddBookingDetailsAndReserveSeats(ConfirmTicketSelectionViewModel model, Bookings booking)
        {
            foreach (var detail in model.TicketSelections)
            {
                var bookingDetail = new BookingDetails
                {
                    bookingID = booking.bookingID,
                    ticketTypeID = detail.TicketTypeId,
                    quantity = detail.Quantity
                };
                db.BookingDetails.Add(bookingDetail);

                var seatReservation = new seatReservations
                {
                    bookingID = booking.bookingID,
                    seatID = detail.SeatId,
                    showtimeID = booking.showtimeID
                };
                db.seatReservations.Add(seatReservation);
            }
        }
        public ActionResult SelectSeat(int bookingId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = db.Bookings.Find(bookingId);
            if (booking == null)
            {
                return HttpNotFound();
            }

            var showtime = db.Showtime.Find(booking.showtimeID);
            var theater = db.Theater.Find(showtime.theaterID);
            var seats = db.Seats.Where(s => s.theaterID == theater.theaterID).ToList();

            ViewBag.Booking = booking;
            ViewBag.Theater = theater;
            ViewBag.Seats = seats;

            return View();
        }

        public ActionResult SelectPayment(int bookingId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = db.Bookings.Find(bookingId);
            if (booking == null)
            {
                return HttpNotFound();
            }

            var bookingDetails = db.BookingDetails.Where(bd => bd.bookingID == bookingId).ToList();
            var totalPrice = bookingDetails.Sum(bd => bd.quantity * db.TicketTypes.Find(bd.ticketTypeID).price);

            var viewModel = new PaymentViewModel
            {
                BookingId = bookingId,
                TotalPrice = totalPrice
            };

            return View(viewModel);
        }

        public ActionResult ShowTicket(int bookingId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = db.Bookings.Find(bookingId);
            if (booking == null)
            {
                return HttpNotFound();
            }

            var showtime = db.Showtime.Find(booking.showtimeID);
            var movie = db.Movie.Find(showtime.movieID);
            var theater = db.Theater.Find(showtime.theaterID);
            var seatReservations = db.seatReservations.Where(sr => sr.bookingID == bookingId).ToList();
            var seats = seatReservations.Select(sr => db.Seats.Find(sr.seatID)).ToList();

            var viewModel = new ShowTicketViewModel
            {
                MovieName = movie.title,
                TheaterName = theater.name,
                ShowtimeDate = showtime.date,
                ShowtimeTime = showtime.time,
                Seats = seats.Select(s => $"{s.seatRow}{s.seatNumber}").ToList(),
                TotalPrice = seatReservations.Count * db.TicketTypes.Sum(tt => tt.price) // Assuming one ticket type for simplicity
            };

            return View(viewModel);
        }
    }
}
