using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.ViewModels;
using WebProject.Models;


namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();
        public ActionResult Homepage()
        {
            // Fetch the movies from the database; adjust the logic as needed to fetch the appropriate movies
            var moviesOnTheaters = db.Movie.Where(m => m.releaseDate <= DateTime.Now).Take(5).ToList();
            var upcomingMovies = db.Movie.Where(m => m.releaseDate > DateTime.Now).Take(5).ToList();
            var featuredMovies = db.Movie.Where(m => m.isFeatured == true).Take(3).ToList();

            var viewModel = new Homepage
            {
                OnTheaters = moviesOnTheaters,
                Upcoming = upcomingMovies,
                Featured = featuredMovies
            };

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}