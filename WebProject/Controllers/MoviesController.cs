using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    

    public class MoviesController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();
        // GET: Movies
        public ActionResult OnTheaters()
        {
            var onTheaterMovies = db.Movie.Where(m => m.releaseDate <= DateTime.Now).ToList();
            return View(onTheaterMovies);
        }
        public ActionResult Upcoming()
        {
            var upcomingMovies = db.Movie.Where(m => m.releaseDate > DateTime.Now).ToList();
            return View(upcomingMovies);
        }
        public ActionResult Inspect(int id)
        {
            var movie = db.Movie.FirstOrDefault(m => m.MovieID == id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }
    }
}