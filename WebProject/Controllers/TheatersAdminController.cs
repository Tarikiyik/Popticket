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
    public class TheatersAdminController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();

        // GET: TheatersAdmin
        public ActionResult Index()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            var theater = db.Theater.Include(t => t.Cities);
            return View(theater.ToList());
        }

        // GET: TheatersAdmin/Details/5
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
            Theater theater = db.Theater.Find(id);
            if (theater == null)
            {
                return HttpNotFound();
            }
            return View(theater);
        }

        // GET: TheatersAdmin/Create
        public ActionResult Create()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName");
            return View();
        }

        // POST: TheatersAdmin/Create
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "theaterID,cityID,name,address,contactNumber,contactMail")] Theater theater)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Theater.Add(theater);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", theater.cityID);
            return View(theater);
        }

        // GET: TheatersAdmin/Edit/5
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
            Theater theater = db.Theater.Find(id);
            if (theater == null)
            {
                return HttpNotFound();
            }
            ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", theater.cityID);
            return View(theater);
        }

        // POST: TheatersAdmin/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "theaterID,cityID,name,address,contactNumber,contactMail")] Theater theater)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(theater).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", theater.cityID);
            return View(theater);
        }

        // GET: TheatersAdmin/Delete/5
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
            Theater theater = db.Theater.Find(id);
            if (theater == null)
            {
                return HttpNotFound();
            }
            return View(theater);
        }

        // POST: TheatersAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            Theater theater = db.Theater.Find(id);
            db.Theater.Remove(theater);
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
