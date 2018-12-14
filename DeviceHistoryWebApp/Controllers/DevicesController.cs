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
    public class DevicesController : Controller
    {
        private DeviceHistoryEntities db = new DeviceHistoryEntities();

        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _Search()
        {
            return PartialView();
        }
        
        [HttpPost]
        public ActionResult _SearchContent(string search_input)
        {

            ContentResult result = new ContentResult() { ContentEncoding=System.Text.Encoding.Default, ContentType = "text/json" };
            search_input = search_input.ToLowerInvariant();

            List<Device> results = new List<Device>();

            foreach (Device dev in db.Devices.ToList())
            {
                if (dev.Uid.ToLowerInvariant().Contains(search_input) || dev.SerialNo.ToLowerInvariant().Contains(search_input))
                {
                    results.Add(dev);
                }
            }


            if (results.Count <= 0) result.Content = "";

            else if (results.Count == 1) result.Content = results[0].JsonData;

            else result.Content = Device.GetJsonData(results);

            return result;
        }


        [HttpPost]
        public ActionResult _Search(String search_input)
        {
            search_input = search_input.ToLowerInvariant();

            List<int> results = new List<int>();

            foreach(Device dev in db.Devices.ToList())
            {
                if(dev.Uid.ToLowerInvariant().Contains(search_input) || dev.SerialNo.ToLowerInvariant().Contains(search_input))
                {
                    results.Add(dev.Id);
                }
            }

            if (results.Count <= 0) return RedirectToAction("Index", "Home");

            if (results.Count == 1) return RedirectToAction("Details", new { @id = results[0] });

            string listString = "";
            for(int i = 0; i < results.Count; i++)
            {
                listString += Convert.ToString(results[i]);
                if (i + 1 < results.Count) listString += ",";
            }

            return RedirectToAction("List", new { @list = listString });
        }

        [HttpGet]
        public ActionResult _List(string list)
        {
            List<Device> devList = new List<Device>();

            string[] listparts = list.Split(',');

            foreach (string idstr in listparts)
                devList.Add(db.Devices.Find(Int32.Parse(idstr)));

            ViewBag.DeviceList = devList;

            return PartialView();
        }

        [HttpGet]
        public ActionResult _Select(int id)
        {
            Device dev = db.Devices.Find(id);
            return new ContentResult() { ContentEncoding = System.Text.Encoding.Default, ContentType = "text/json", Content = dev.JsonData };
        }

        [HttpGet]
        public ActionResult List(string list)
        {
            if (list == null) return RedirectToAction("Index");

            List<Device> devList = new List<Device>();
            string[] listparts = list.Split(',');

            foreach (string idstr in listparts)
                devList.Add(db.Devices.Find(Int32.Parse(idstr)));

            ViewBag.DeviceList = devList;

            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            CSV csv = new CSV(file.InputStream);
            int nextId = Device.NextAvailableId;

            foreach (Dictionary<string, string> row in csv)
            {
                string typeName = row["Type"];
                DeviceType type = db.DeviceTypes.ToList().Where(t => t.Name.Equals(typeName)).Single();

                string uid = row["Uid"];

                if (db.Devices.ToList().Where(dev => dev.Uid.Equals(uid)).Count() <= 0)
                {
                    Device newDevice = new Device()
                    {
                        Id = nextId++,
                        TypeId = type.Id,
                        Notes = row["Notes"],
                        Uid = row["Uid"],
                        SerialNo = row["SerialNo"]
                    };

                    db.Devices.Add(newDevice);
                }
                else
                {
                    Device toUpdate = db.Devices.ToList().Where(dev => dev.Uid.Equals(uid)).Single();
                    toUpdate.TypeId = type.Id;
                    toUpdate.Notes = row["Notes"];
                    toUpdate.SerialNo = row["SerialNo"];
                }
            }

            db.SaveChanges();
            return View("Index");
        }

        // GET: Devices
        public ActionResult Index()
        {
            var devices = db.Devices.Include(d => d.DeviceType);
            return View(devices.ToList());
        }

        // GET: Devices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // GET: Devices/Create
        public ActionResult Create()
        {
            ViewBag.TypeId = new SelectList(db.DeviceTypes, "Id", "Name");
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Uid,SerialNo,TypeId,Notes")] Device device)
        {
            if (ModelState.IsValid)
            {
                if (device.Notes == null) device.Notes = "";
                if (device.SerialNo == null) device.SerialNo = "";
                if (device.Uid == null) return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
                db.Devices.Add(device);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TypeId = new SelectList(db.DeviceTypes, "Id", "Name", device.TypeId);
            return View(device);
        }

        // GET: Devices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeId = new SelectList(db.DeviceTypes, "Id", "Name", device.TypeId);
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Uid,SerialNo,TypeId,Notes")] Device device)
        {
            if (ModelState.IsValid)
            {
                db.Entry(device).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeId = new SelectList(db.DeviceTypes, "Id", "Name", device.TypeId);
            return View(device);
        }

        // GET: Devices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Device device = db.Devices.Find(id);
            db.Devices.Remove(device);
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
