using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class MoviesAdminController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();

        // GET: MoviesAdmin
        public ActionResult Index()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            return View(db.Movie.ToList());
        }

        // GET: MoviesAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: MoviesAdmin/Create
        public ActionResult Create()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            return View();
        }

        // POST: MoviesAdmin/Create
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MovieID,title,description,imdb,releaseDate,director,genre,duration,movieImg,movieBanner")] Movie movie)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Movie.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: MoviesAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: MoviesAdmin/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MovieID,title,description,imdb,releaseDate,director,genre,duration,movieImg,movieBanner")] Movie movie)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: MoviesAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: MoviesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            Movie movie = db.Movie.Find(id);
            db.Movie.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
