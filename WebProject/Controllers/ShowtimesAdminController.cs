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
    public class ShowtimesAdminController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();

        // GET: ShowtimesAdmin
        public ActionResult Index()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            var showtime = db.Showtime.Include(s => s.Movie).Include(s => s.Theater);
            return View(showtime.ToList());
        }

        // GET: ShowtimesAdmin/Details/5
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
            Showtime showtime = db.Showtime.Find(id);
            if (showtime == null)
            {
                return HttpNotFound();
            }
            return View(showtime);
        }

        // GET: ShowtimesAdmin/Create
        public ActionResult Create()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            ViewBag.movieID = new SelectList(db.Movie, "MovieID", "title");
            ViewBag.theaterID = new SelectList(db.Theater, "theaterID", "name");
            return View();
        }

        // POST: ShowtimesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "showtimeID,movieID,theaterID,time,date")] Showtime showtime)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Showtime.Add(showtime);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.movieID = new SelectList(db.Movie, "MovieID", "title", showtime.movieID);
            ViewBag.theaterID = new SelectList(db.Theater, "theaterID", "name", showtime.theaterID);
            return View(showtime);
        }

        // GET: ShowtimesAdmin/Edit/5
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
            Showtime showtime = db.Showtime.Find(id);
            if (showtime == null)
            {
                return HttpNotFound();
            }
            ViewBag.movieID = new SelectList(db.Movie, "MovieID", "title", showtime.movieID);
            ViewBag.theaterID = new SelectList(db.Theater, "theaterID", "name", showtime.theaterID);
            return View(showtime);
        }

        // POST: ShowtimesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "showtimeID,movieID,theaterID,time,date")] Showtime showtime)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(showtime).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.movieID = new SelectList(db.Movie, "MovieID", "title", showtime.movieID);
            ViewBag.theaterID = new SelectList(db.Theater, "theaterID", "name", showtime.theaterID);
            return View(showtime);
        }

        // GET: ShowtimesAdmin/Delete/5
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
            Showtime showtime = db.Showtime.Find(id);
            if (showtime == null)
            {
                return HttpNotFound();
            }
            return View(showtime);
        }

        // POST: ShowtimesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            Showtime showtime = db.Showtime.Find(id);
            db.Showtime.Remove(showtime);
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
