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
    public class StateRuleThreeController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: State_Rule3_Marriage
        public ActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }


        public ActionResult GetData()
        {
            var state_Rule3_Marriage = from s in db.State_Rule3_Marriage.Include(s => s.Rule3_Master)
                                       .Include(s => s.State_Master).Where(s => s.Status == "Active")
                                                                    select s;

            var subData = state_Rule3_Marriage.ToList().Select(S => new
            {
                S.ID,
                State = S.State_Master.Name,
                S.Rule3_Master.CarryOverUnits,
                StartDate = S.StartDate.ToShortDateString(),
                EndDate = S.EndDate.ToString().Split(' ')[0],
                S.Status
            });

            return Json(new { data = subData }, JsonRequestBehavior.AllowGet);
        }


        // GET: State_Rule3_Marriage/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Rule3_Marriage state_Rule3_Marriage = db.State_Rule3_Marriage.Find(id);

            if (state_Rule3_Marriage == null)
            {
                return HttpNotFound();
            }

            return View(state_Rule3_Marriage);
        }


        // GET: State_Rule3_Marriage/Create
        public ActionResult Create()
        {
            ViewBag.Rule3_ID = new SelectList(db.Rule3_Master, "ID", "CarryOverUnits");
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            return View();
        }


        // POST: State_Rule3_Marriage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,StateID,Rule3_ID,StartDate,EndDate,Status")] State_Rule3_Marriage state_Rule3_Marriage)
        {
            try
            {
                ViewBag.Rule3_ID = new SelectList(db.Rule3_Master, "ID", "CarryOverUnits", state_Rule3_Marriage.Rule3_ID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Rule3_Marriage.StateID);

                //if (ModelState.IsValid)
                //{
                    if (state_Rule3_Marriage.StartDate > state_Rule3_Marriage.EndDate)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                        return View();
                    }
                    else
                    {
                        List<State_Rule3_Marriage> getName = db.State_Rule3_Marriage.Where(u => u.StateID == state_Rule3_Marriage.StateID
                        && u.Rule3_ID == state_Rule3_Marriage.Rule3_ID && u.StartDate == state_Rule3_Marriage.StartDate
                        && u.EndDate == state_Rule3_Marriage.EndDate && u.Status == state_Rule3_Marriage.Status).ToList();

                        if (getName.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            db.State_Rule3_Marriage.Add(state_Rule3_Marriage);
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
            }
            catch (Exception)
            {
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
            }

            return View();
        }


        // GET: State_Rule3_Marriage/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Rule3_Marriage state_Rule3_Marriage = db.State_Rule3_Marriage.Find(id);

            if (state_Rule3_Marriage == null)
            {
                return HttpNotFound();
            }

            ViewBag.Rule3_ID = new SelectList(db.Rule3_Master, "ID", "CarryOverUnits", state_Rule3_Marriage.Rule3_ID);
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Rule3_Marriage.StateID);
            return View(state_Rule3_Marriage);
        }


        // POST: State_Rule3_Marriage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StateID,Rule3_ID,StartDate,EndDate,Status")] State_Rule3_Marriage state_Rule3_Marriage)
        {
            try
            {
                ViewBag.Rule3_ID = new SelectList(db.Rule3_Master, "ID", "CarryOverUnits", state_Rule3_Marriage.Rule3_ID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Rule3_Marriage.StateID);

                //if (ModelState.IsValid)
                //{
                    if (state_Rule3_Marriage.StartDate > state_Rule3_Marriage.EndDate)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                        return View();
                    }
                    else
                    {
                        List<State_Rule3_Marriage> getName = db.State_Rule3_Marriage.Where(u => u.StateID == state_Rule3_Marriage.StateID
                        && u.Rule3_ID == state_Rule3_Marriage.Rule3_ID && u.StartDate == state_Rule3_Marriage.StartDate
                        && u.EndDate == state_Rule3_Marriage.EndDate && u.Status == state_Rule3_Marriage.Status && u.ID != state_Rule3_Marriage.ID).ToList();

                        if (getName.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            db.Entry(state_Rule3_Marriage).State = EntityState.Modified;
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
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        errorMessage = ve.ErrorMessage;

                        ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
            }

            return View();
        }


        // GET: State_Rule3_Marriage/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Rule3_Marriage state_Rule3_Marriage = db.State_Rule3_Marriage.Find(id);

            if (state_Rule3_Marriage == null)
            {
                return HttpNotFound();
            }

            return View(state_Rule3_Marriage);
        }


        // POST: State_Rule3_Marriage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                State_Rule3_Marriage state_Rule3_Marriage = db.State_Rule3_Marriage.Find(id);
                db.State_Rule3_Marriage.Remove(state_Rule3_Marriage);
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


        public JsonResult RuleDetails()
        {
            var ruleList = from s in db.Rule3_Master select new { s.ID, Name = string.Concat("Carry Over Units - ", s.CarryOverUnits) };

            return Json(new SelectList(ruleList.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }
    }
}
