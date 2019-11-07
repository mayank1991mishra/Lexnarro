using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ActivityController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: Activity_Master
        public ActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }


        public ActionResult GetActivities()
        {
            IQueryable<Activity_Master> activity_master = from s in db.Activity_Master select s;

            var categories = activity_master.ToList().Select(S => new
            {
                S.ID,
                S.Name,
                S.ShortName,
                S.Description
            });

            return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
        }


        // GET: Activity_Master/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Activity_Master activity_Master = db.Activity_Master.Find(id);

            if (activity_Master == null)
            {
                return HttpNotFound();
            }

            return View(activity_Master);
        }


        // GET: Activity_Master/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Activity_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,ShortName")] Activity_Master activity_Master)
        {
            try
            {
                System.Collections.Generic.List<Activity_Master> getName = db.Activity_Master.Where(u => u.Name == activity_Master.Name).ToList();

                if (getName.Count > 0)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                    return View();
                }
                else
                {
                    db.Activity_Master.Add(activity_Master);
                    db.SaveChanges();
                    TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                    return RedirectToAction("Index");
                }
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


        // GET: Activity_Master/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Activity_Master activity_Master = db.Activity_Master.Find(id);

            if (activity_Master == null)
            {
                return HttpNotFound();
            }

            return View(activity_Master);
        }


        // POST: Activity_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,ShortName")] Activity_Master activity_Master)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                System.Collections.Generic.List<Activity_Master> getName = db.Activity_Master.Where(u => u.Name == activity_Master.Name && u.ID != activity_Master.ID).ToList();

                if (getName.Count > 0)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                    return View();
                }
                else
                {
                    db.Entry(activity_Master).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = ToasterMessage.Message(ToastType.success, "Updated Successfully");
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
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
            }
            return View();

        }


        // GET: Activity_Master/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Activity_Master activity_Master = db.Activity_Master.Find(id);

            if (activity_Master == null)
            {
                return HttpNotFound();
            }

            return View(activity_Master);
        }


        // POST: Activity_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Activity_Master activity_Master = db.Activity_Master.Find(id);
                db.Activity_Master.Remove(activity_Master);
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
