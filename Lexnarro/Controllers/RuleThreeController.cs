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
    public class RuleThreeController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: Rule3_Master
        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        public ActionResult GetRules()
        {
            var rule3_Master = from s in db.Rule3_Master select s;

            var rules = rule3_Master.ToList().Select(S => new
            {
                S.ID,
                S.CarryOverUnits
            });

            return Json(new { data = rules }, JsonRequestBehavior.AllowGet);
        }


        // GET: Rule3_Master/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Rule3_Master rule3_Master = db.Rule3_Master.Find(id);

            if (rule3_Master == null)
            {
                return HttpNotFound();
            }

            return View(rule3_Master);
        }


        // GET: Rule3_Master/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Rule3_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CarryOverUnits")] Rule3_Master rule3_Master)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    var getName = db.Rule3_Master.Where(u => u.CarryOverUnits == rule3_Master.CarryOverUnits).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Rule3_Master.Add(rule3_Master);
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


        // GET: Rule3_Master/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rule3_Master rule3_Master = db.Rule3_Master.Find(id);

            if (rule3_Master == null)
            {
                return HttpNotFound();
            }

            return View(rule3_Master);
        }


        // POST: Rule3_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CarryOverUnits")] Rule3_Master rule3_Master)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    var getName = db.Rule3_Master.Where(u => u.CarryOverUnits == rule3_Master.CarryOverUnits && u.ID != rule3_Master.ID).ToList();
                    
                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Entry(rule3_Master).State = EntityState.Modified;
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

        // GET: Rule3_Master/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rule3_Master rule3_Master = db.Rule3_Master.Find(id);

            if (rule3_Master == null)
            {
                return HttpNotFound();
            }

            return View(rule3_Master);
        }


        // POST: Rule3_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                Rule3_Master rule3_Master = db.Rule3_Master.Find(id);
                db.Rule3_Master.Remove(rule3_Master);
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
