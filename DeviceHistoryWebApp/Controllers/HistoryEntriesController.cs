using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeviceHistoryWebApp;

namespace DeviceHistoryWebApp.Controllers
{
    public class HistoryEntriesController : Controller
    {
        private DeviceHistoryEntities db = new DeviceHistoryEntities();

        // GET: HistoryEntries
        public ActionResult Index()
        {
            var historyEntries = db.HistoryEntries.Include(h => h.Device).Include(h => h.EndUser).Include(h => h.Creator);
            return View(historyEntries.ToList());
        }

        // GET: HistoryEntries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoryEntry historyEntry = db.HistoryEntries.Find(id);
            if (historyEntry == null)
            {
                return HttpNotFound();
            }
            return View(historyEntry);
        }

        // GET: HistoryEntries/Create
        public ActionResult Create()
        {
            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Uid");
            ViewBag.EndUserId = new SelectList(db.Users, "Id", "Name");
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: HistoryEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EndUserId,CreatorId,Summary,Action,Result,DeviceId,AdditionalNotes")] HistoryEntry historyEntry, bool closed = false)
        {
            if (ModelState.IsValid)
            {
                historyEntry.Id = HistoryEntry.NextAvailableId;
                if (historyEntry.Summary == null) historyEntry.Summary = "";
                if (historyEntry.Action == null) historyEntry.Action = "";
                if (historyEntry.Result == null) historyEntry.Result = "";
                if (historyEntry.AdditionalNotes == null) historyEntry.AdditionalNotes = "";

                historyEntry.Submitted = DateTime.Today;
                historyEntry.LastUpdated = DateTime.Today;
                historyEntry.Closed = closed ? DateTime.Today : new DateTime();

                db.HistoryEntries.Add(historyEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Uid", historyEntry.DeviceId);
            ViewBag.EndUserId = new SelectList(db.Users, "Id", "Name", historyEntry.EndUserId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Name", historyEntry.CreatorId);
            return View(historyEntry);
        }

        // GET: HistoryEntries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoryEntry historyEntry = db.HistoryEntries.Find(id);
            if (historyEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Uid", historyEntry.DeviceId);
            ViewBag.EndUserId = new SelectList(db.Users, "Id", "Name", historyEntry.EndUserId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Name", historyEntry.CreatorId);
            return View(historyEntry);
        }

        // POST: HistoryEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EndUserId,CreatorId,Summary,Action,Result,DeviceId,AdditionalNotes,Submitted,LastUpdated,Closed")] HistoryEntry historyEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(historyEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Uid", historyEntry.DeviceId);
            ViewBag.EndUserId = new SelectList(db.Users, "Id", "Name", historyEntry.EndUserId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Name", historyEntry.CreatorId);
            return View(historyEntry);
        }

        // GET: HistoryEntries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoryEntry historyEntry = db.HistoryEntries.Find(id);
            if (historyEntry == null)
            {
                return HttpNotFound();
            }
            return View(historyEntry);
        }

        // POST: HistoryEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HistoryEntry historyEntry = db.HistoryEntries.Find(id);
            db.HistoryEntries.Remove(historyEntry);
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
