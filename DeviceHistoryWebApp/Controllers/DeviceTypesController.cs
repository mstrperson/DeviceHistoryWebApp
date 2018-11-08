using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeviceHistoryWebApp;
using Csv;

namespace DeviceHistoryWebApp.Controllers
{
    public class DeviceTypesController : Controller
    {
        private DeviceHistoryEntities db = new DeviceHistoryEntities();

        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, bool update=true)
        {
            CSV csv = new CSV(file.InputStream);

            int nextId = DeviceType.NextAvailableId;

            foreach (Dictionary<string, string> row in csv)
            {
                string typeName = row["Name"];
                if (db.DeviceTypes.ToList().Where(t => t.Name.Equals(typeName)).Count() <= 0)
                {
                    DeviceType newType = new DeviceType()
                    {
                        Id = nextId++,
                        Name = typeName,
                        Model = row["Model"],
                        Category = row["Category"],
                        Notes = row["Notes"]
                    };

                    db.DeviceTypes.Add(newType);
                }
                else if(update)
                {
                    DeviceType type = db.DeviceTypes.ToList().Where(t => t.Name.Equals(typeName)).Single();
                    type.Model = row["Model"];
                    type.Category = row["Category"];
                    type.Notes = row["Notes"];
                }
            }

            db.SaveChanges();
            return Index();
        }


        // GET: DeviceTypes
        public ActionResult Index()
        {
            return View(db.DeviceTypes.ToList());
        }

        // GET: DeviceTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceType deviceType = db.DeviceTypes.Find(id);
            if (deviceType == null)
            {
                return HttpNotFound();
            }
            return View(deviceType);
        }

        // GET: DeviceTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeviceTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Model,Category,Notes")] DeviceType deviceType)
        {
            if (ModelState.IsValid)
            {
                deviceType.Id = DeviceType.NextAvailableId;

                if (deviceType.Notes == null)
                    deviceType.Notes = "";
                if (deviceType.Model == null)
                    deviceType.Model = "";

                db.DeviceTypes.Add(deviceType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deviceType);
        }

        // GET: DeviceTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceType deviceType = db.DeviceTypes.Find(id);
            if (deviceType == null)
            {
                return HttpNotFound();
            }
            return View(deviceType);
        }

        // POST: DeviceTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Model,Category,Notes")] DeviceType deviceType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deviceType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deviceType);
        }

        // GET: DeviceTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceType deviceType = db.DeviceTypes.Find(id);
            if (deviceType == null)
            {
                return HttpNotFound();
            }
            return View(deviceType);
        }

        // POST: DeviceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DeviceType deviceType = db.DeviceTypes.Find(id);
            db.DeviceTypes.Remove(deviceType);
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
