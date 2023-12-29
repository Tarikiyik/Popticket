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

        [HttpPost]
        public ActionResult ConfirmTicketSelection(ConfirmTicketSelectionViewModel model)
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var user = Session["User"] as User;
            var booking = new Bookings
            {
                userID = user.UserID,
                showtimeID = model.ShowtimeId,
                bookingTime = DateTime.Now
            };

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges(); // Save the booking to generate the bookingID

                    // Add booking details and reserve seats
                    AddBookingDetailsAndReserveSeats(model, booking);

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    // Handle errors, log them, and return an appropriate view/message
                    return View("Error");
                }
            }

            return RedirectToAction("SelectSeat", new { bookingId = booking.bookingID });
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
