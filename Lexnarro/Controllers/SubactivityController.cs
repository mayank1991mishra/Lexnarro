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
    public class SubactivityController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: Sub_Activity_Master
        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        public ActionResult GetSubactivities()
        {
            var sub_Activity_Master = db.Sub_Activity_Master.Include(s => s.Activity_Master).Include(s => s.State_Master);

            var subactivities = sub_Activity_Master.ToList().Select(S => new
            {
                S.ID,
                S.Name,
                Activity = S.Activity_Master.Name,
                S.ShortName,
                State = S.State_Master.Name
            });

            return Json(new { data = subactivities }, JsonRequestBehavior.AllowGet);
        }



        // GET: Sub_Activity_Master/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sub_Activity_Master sub_Activity_Master = db.Sub_Activity_Master.Find(id);

            if (sub_Activity_Master == null)
            {
                return HttpNotFound();
            }

            return View(sub_Activity_Master);
        }


        // GET: Sub_Activity_Master/Create
        public ActionResult Create()
        {
            ViewBag.Activity_ID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name");

            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(s=>s.Name), "ID", "Name");

            return View();
        }

        // POST: Sub_Activity_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Activity_ID,ShortName,StateID")] Sub_Activity_Master sub_Activity_Master)
        {
            try
            {
                ViewBag.Activity_ID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", sub_Activity_Master.Activity_ID);

                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(s => s.Name), "ID", "Name", sub_Activity_Master.StateID);

                //if (ModelState.IsValid)
                //{
                    var getName = db.Sub_Activity_Master.Where(u => u.Name == sub_Activity_Master.Name && u.Activity_ID == sub_Activity_Master.Activity_ID 
                    && u.StateID == sub_Activity_Master.StateID).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Sub_Activity_Master.Add(sub_Activity_Master);
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


        // GET: Sub_Activity_Master/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sub_Activity_Master sub_Activity_Master = db.Sub_Activity_Master.Find(id);

            if (sub_Activity_Master == null)
            {
                return HttpNotFound();
            }

            ViewBag.Activity_ID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", sub_Activity_Master.Activity_ID);
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(s => s.Name), "ID", "Name", sub_Activity_Master.StateID);

            return View(sub_Activity_Master);
        }


        // POST: Sub_Activity_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Activity_ID,ShortName,StateID")] Sub_Activity_Master sub_Activity_Master)
        {
            try
            {
                ViewBag.Activity_ID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", sub_Activity_Master.Activity_ID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(s => s.Name), "ID", "Name", sub_Activity_Master.StateID);

                //if (ModelState.IsValid)
                //{
                    var getName = db.Sub_Activity_Master.Where(u => u.Name == sub_Activity_Master.Name && u.Activity_ID == sub_Activity_Master.Activity_ID
                    && u.StateID == sub_Activity_Master.StateID && u.ID != sub_Activity_Master.ID).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Entry(sub_Activity_Master).State = EntityState.Modified;
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


        // GET: Sub_Activity_Master/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sub_Activity_Master sub_Activity_Master = db.Sub_Activity_Master.Find(id);

            if (sub_Activity_Master == null)
            {
                return HttpNotFound();
            }

            return View(sub_Activity_Master);
        }


        // POST: Sub_Activity_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                Sub_Activity_Master sub_Activity_Master = db.Sub_Activity_Master.Find(id);
                db.Sub_Activity_Master.Remove(sub_Activity_Master);
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
