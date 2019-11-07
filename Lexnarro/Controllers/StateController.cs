using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Lexnarro.Models;
using Lexnarro.HelperClasses;
using System.Data.Entity.Validation;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StateController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();
        

        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        public JsonResult GetStates()
        {
            var state_master = from s in db.State_Master select s;

            var states = state_master.Select(S => new
            {
                S.ID,
                S.Name,
                S.ShortName
            });

            return Json(new { data = states }, JsonRequestBehavior.AllowGet);
        }


        // GET: State_Master/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            State_Master state_Master = db.State_Master.Find(id);

            if (state_Master == null)
                return HttpNotFound();

            return View(state_Master);
        }


        // GET: State_Master/Create
        public ActionResult Create()
        {
            ViewBag.Country_ID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name");
            return View();
        }


        // POST: State_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ShortName,Country_ID")] State_Master state_Master)
        {
            try
            {
                ViewBag.Country_ID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name", state_Master.Country_ID);

                //if (ModelState.IsValid)
                //{
                    var getName = db.State_Master.Where(u => u.Name == state_Master.Name && u.Country_ID == state_Master.Country_ID).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exists.");
                        return View();
                    }

                    else
                    {
                        db.State_Master.Add(state_Master);
                        db.SaveChanges();
                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully.");
                        return RedirectToAction("Index");
                    }
                //}
            }
            catch (DbEntityValidationException e)
            {
                string errorMessage = string.Empty;

                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        errorMessage = ve.ErrorMessage;

                        ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong.");
            }

            return View();
        }


        // GET: State_Master/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            State_Master state_Master = db.State_Master.Find(id);

            if (state_Master == null)
                return HttpNotFound();

            ViewBag.Country_ID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name", state_Master.Country_ID);

            return View(state_Master);
        }


        // POST: State_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ShortName,Country_ID")] State_Master state_Master)
        {
            try
            {
                ViewBag.Country_ID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name", state_Master.Country_ID);

                //if (ModelState.IsValid)
                //{
                    var getName = db.State_Master.Where(u => u.Name == state_Master.Name && u.Country_ID == state_Master.Country_ID 
                    && u.ID != state_Master.ID).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exists.");
                        return View();
                    }

                    else
                    {
                        db.Entry(state_Master).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Updated successfully.");
                        return RedirectToAction("Index");
                    }
                //}
            }
            catch (DbEntityValidationException e)
            {
                string errorMessage = string.Empty;

                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        errorMessage = ve.ErrorMessage;

                        ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong.");
            }

            return View();
        }


        // GET: State_Master/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State_Master state_Master = db.State_Master.Find(id);
            if (state_Master == null)
            {
                return HttpNotFound();
            }
            return View(state_Master);
        }


        // POST: State_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                State_Master state_Master = db.State_Master.Find(id);
                db.State_Master.Remove(state_Master);
                db.SaveChanges();

                TempData["message"] = ToasterMessage.Message(ToastType.success, "Deleted successfully");
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException e)
            {
                string errorMessage = string.Empty;

                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        errorMessage = ve.ErrorMessage;

                        ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
            }

            return View();
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
