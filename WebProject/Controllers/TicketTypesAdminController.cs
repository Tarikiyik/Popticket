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
    public class TicketTypesAdminController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();

        // GET: TicketTypesAdmin
        public ActionResult Index()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            return View(db.TicketTypes.ToList());
        }

        // GET: TicketTypesAdmin/Details/5
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
            TicketTypes ticketTypes = db.TicketTypes.Find(id);
            if (ticketTypes == null)
            {
                return HttpNotFound();
            }
            return View(ticketTypes);
        }

        // GET: TicketTypesAdmin/Create
        public ActionResult Create()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            return View();
        }

        // POST: TicketTypesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ticketTypeID,typeName,price")] TicketTypes ticketTypes)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.TicketTypes.Add(ticketTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticketTypes);
        }

        // GET: TicketTypesAdmin/Edit/5
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
            TicketTypes ticketTypes = db.TicketTypes.Find(id);
            if (ticketTypes == null)
            {
                return HttpNotFound();
            }
            return View(ticketTypes);
        }

        // POST: TicketTypesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ticketTypeID,typeName,price")] TicketTypes ticketTypes)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(ticketTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticketTypes);
        }

        // GET: TicketTypesAdmin/Delete/5
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
            TicketTypes ticketTypes = db.TicketTypes.Find(id);
            if (ticketTypes == null)
            {
                return HttpNotFound();
            }
            return View(ticketTypes);
        }

        // POST: TicketTypesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Homepage", "Home");
            }
            TicketTypes ticketTypes = db.TicketTypes.Find(id);
            db.TicketTypes.Remove(ticketTypes);
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
