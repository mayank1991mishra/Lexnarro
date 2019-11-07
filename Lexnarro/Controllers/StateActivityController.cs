using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Lexnarro.HelperClasses;
using Lexnarro.Models;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StateActivityController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();


        // GET: State_Activity_Mapping
        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        public ActionResult GetStateActivities()
        {
            var state_Activity_Mapping = from s in db.State_Activity_Mapping.Include(s => s.Activity_Master).Include(s => s.State_Master)
                                         select s;

            var subactivities = state_Activity_Mapping.ToList().Select(S => new
            {
                S.Id,
                State = S.State_Master.Name,
                Activity = S.Activity_Master.Name
            });

            return Json(new { data = subactivities }, JsonRequestBehavior.AllowGet);
        }


        // GET: State_Activity_Mapping/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Activity_Mapping state_Activity_Mapping = db.State_Activity_Mapping.Find(id);

            if (state_Activity_Mapping == null)
            {
                return HttpNotFound();
            }

            return View(state_Activity_Mapping);
        }


        // GET: State_Activity_Mapping/Create
        public ActionResult Create()
        {
            ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m=>m.Name), "ID", "Name");
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            return View();
        }


        // POST: State_Activity_Mapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StateID,ActivityID")] State_Activity_Mapping state_Activity_Mapping)
        {
            try
            {
                ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", state_Activity_Mapping.ActivityID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Activity_Mapping.StateID);

                //if (ModelState.IsValid)
                //{
                    var getName = db.State_Activity_Mapping.Where(u => u.ActivityID == state_Activity_Mapping.ActivityID
                    && u.StateID == state_Activity_Mapping.StateID).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.State_Activity_Mapping.Add(state_Activity_Mapping);
                        db.SaveChanges();
                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
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


        // GET: State_Activity_Mapping/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Activity_Mapping state_Activity_Mapping = db.State_Activity_Mapping.Find(id);

            if (state_Activity_Mapping == null)
            {
                return HttpNotFound();
            }

            ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", state_Activity_Mapping.ActivityID);
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Activity_Mapping.StateID);

            return View(state_Activity_Mapping);
        }


        // POST: State_Activity_Mapping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StateID,ActivityID")] State_Activity_Mapping state_Activity_Mapping)
        {
            try
            {
                ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", state_Activity_Mapping.ActivityID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Activity_Mapping.StateID);

                //if (ModelState.IsValid)
                //{
                    var getName = db.State_Activity_Mapping.Where(u => u.ActivityID == state_Activity_Mapping.ActivityID
                    && u.StateID == state_Activity_Mapping.StateID && u.Id != state_Activity_Mapping.Id).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Entry(state_Activity_Mapping).State = EntityState.Modified;
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


        // GET: State_Activity_Mapping/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Activity_Mapping state_Activity_Mapping = db.State_Activity_Mapping.Find(id);

            if (state_Activity_Mapping == null)
            {
                return HttpNotFound();
            }

            return View(state_Activity_Mapping);
        }


        // POST: State_Activity_Mapping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                State_Activity_Mapping state_Activity_Mapping = db.State_Activity_Mapping.Find(id);
                db.State_Activity_Mapping.Remove(state_Activity_Mapping);
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
