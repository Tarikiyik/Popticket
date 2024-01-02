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
    public class SearchController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();

        [HttpGet]
        public ActionResult SearchResults(string query)
        {
            var searchResults = db.Movie
                .Where(m => m.title.Contains(query))
                .ToList();

            // Assume you have a SearchResults ViewModel
            var viewModel = new SearchResults
            {
                Query = query,
                Movies = searchResults
            };

            return View(viewModel);
        }

        public ActionResult AjaxSearchResults(string query)
        {
            var searchResults = db.Movie
                .Where(m => m.title.Contains(query))
                .Select(m => new { m.title, m.description, m.movieImg, m.imdb, m.duration })
                .ToList();

            return Json(searchResults, JsonRequestBehavior.AllowGet);
        }
    }
}