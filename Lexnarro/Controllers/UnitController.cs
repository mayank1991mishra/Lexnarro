using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UnitController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: Total_Unit_Master
        public ActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }


        public ActionResult GetUnits()
        {
            IQueryable<Total_Unit_Master> total_Unit_Master = from s in db.Total_Unit_Master.Include(s => s.State_Master) select s;

            var units = total_Unit_Master.ToList().Select(S => new
            {
                S.Id,
                Start_Date = S.Start_Date.ToShortDateString(),
                End_Date = S.End_Date.ToString().Split(' ')[0],
                State = S.State_Master.Name,
                S.Total_Units
            });

            return Json(new { data = units }, JsonRequestBehavior.AllowGet);
        }


        // GET: Total_Unit_Master/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Total_Unit_Master total_Unit_Master = db.Total_Unit_Master.Find(id);

            if (total_Unit_Master == null)
            {
                return HttpNotFound();
            }

            return View(total_Unit_Master);
        }


        // GET: Total_Unit_Master/Create
        public ActionResult Create()
        {
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            return View();
        }


        // POST: Total_Unit_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StateID,Total_Units,Start_Date,End_Date")] Total_Unit_Master total_Unit_Master)
        {
            try
            {
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", total_Unit_Master.StateID);

                //if (ModelState.IsValid)
                //{
                    if (total_Unit_Master.Start_Date > total_Unit_Master.End_Date)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                        return View();
                    }
                    else
                    {
                        List<Total_Unit_Master> getName = db.Total_Unit_Master.Where(u => u.StateID == total_Unit_Master.StateID
                        && u.Total_Units == total_Unit_Master.Total_Units && u.Start_Date == total_Unit_Master.Start_Date
                        && u.End_Date == total_Unit_Master.End_Date).ToList();

                        if (getName.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            db.Total_Unit_Master.Add(total_Unit_Master);
                            db.SaveChanges();
                            TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                            return RedirectToAction("Index");
                        }
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


        // GET: Total_Unit_Master/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Total_Unit_Master total_Unit_Master = db.Total_Unit_Master.Find(id);

            if (total_Unit_Master == null)
            {
                return HttpNotFound();
            }

            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", total_Unit_Master.StateID);

            return View(total_Unit_Master);
        }


        // POST: Total_Unit_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StateID,Total_Units,Start_Date,End_Date")] Total_Unit_Master total_Unit_Master)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", total_Unit_Master.StateID);

                    if (total_Unit_Master.Start_Date > total_Unit_Master.End_Date)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                        return View();
                    }
                    else
                    {
                        List<Total_Unit_Master> getName = db.Total_Unit_Master.Where(u => u.StateID == total_Unit_Master.StateID
                        && u.Total_Units == total_Unit_Master.Total_Units && u.Start_Date == total_Unit_Master.Start_Date
                        && u.End_Date == total_Unit_Master.End_Date && u.Id != total_Unit_Master.Id).ToList();

                        if (getName.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            db.Entry(total_Unit_Master).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["message"] = ToasterMessage.Message(ToastType.success, "Updated Successfully");
                            return RedirectToAction("Index");
                        }
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


        // GET: Total_Unit_Master/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Total_Unit_Master total_Unit_Master = db.Total_Unit_Master.Find(id);

            if (total_Unit_Master == null)
            {
                return HttpNotFound();
            }

            return View(total_Unit_Master);
        }


        // POST: Total_Unit_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Total_Unit_Master total_Unit_Master = db.Total_Unit_Master.Find(id);
                db.Total_Unit_Master.Remove(total_Unit_Master);
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
