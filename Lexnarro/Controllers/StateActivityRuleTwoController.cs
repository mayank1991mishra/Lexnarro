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
    public class StateActivityRuleTwoController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: StateActivity__with_Rule2
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
            var stateActivity__with_Rule2 = db.StateActivity__with_Rule2.Include(s => s.Activity_Master)
               .Include(s => s.Rule2_Master).Include(s => s.State_Master).Where(s => s.Status == "Active");

            var subData = stateActivity__with_Rule2.ToList().Select(S => new
            {
                S.Id,
                State = S.State_Master.Name,
                Activity = S.Activity_Master.Name,
                Ruletwo = S.Rule2_Master.Name,
                StartDate = S.StartDate.ToShortDateString(),
                EndDate = S.EndDate.ToString().Split(' ')[0],
                S.Status
            });

            return Json(new { data = subData }, JsonRequestBehavior.AllowGet);
        }


        // GET: StateActivity__with_Rule2/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StateActivity__with_Rule2 stateActivity__with_Rule2 = db.StateActivity__with_Rule2.Find(id);

            if (stateActivity__with_Rule2 == null)
            {
                return HttpNotFound();
            }

            return View(stateActivity__with_Rule2);
        }


        // GET: StateActivity__with_Rule2/Create
        public ActionResult Create()
        {
            ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name");

            ViewBag.Rule2_ID = new SelectList(db.Rule2_Master.OrderBy(m => m.Name), "Id", "Name");

            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");

            return View();
        }


        // POST: StateActivity__with_Rule2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StateID,ActivityID,Rule2_ID,StartDate,EndDate,Status")] StateActivity__with_Rule2 stateActivity__with_Rule2)
        {
            try
            {
                ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", stateActivity__with_Rule2.ActivityID);
                ViewBag.Rule2_ID = new SelectList(db.Rule2_Master.OrderBy(m => m.Name), "Id", "Name", stateActivity__with_Rule2.Rule2_ID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", stateActivity__with_Rule2.StateID);

                //if (ModelState.IsValid)
                //{
                    if (stateActivity__with_Rule2.StartDate > stateActivity__with_Rule2.EndDate)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                        return View();
                    }
                    else
                    {
                        var getName = db.StateActivity__with_Rule2.Where(u => u.StateID == stateActivity__with_Rule2.StateID
                        && u.Rule2_ID == stateActivity__with_Rule2.Rule2_ID && u.StartDate == stateActivity__with_Rule2.StartDate
                        && u.EndDate == stateActivity__with_Rule2.EndDate && u.Status == stateActivity__with_Rule2.Status
                        && u.ActivityID == stateActivity__with_Rule2.ActivityID).ToList();

                        if (getName.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            db.StateActivity__with_Rule2.Add(stateActivity__with_Rule2);
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


        // GET: StateActivity__with_Rule2/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StateActivity__with_Rule2 stateActivity__with_Rule2 = db.StateActivity__with_Rule2.Find(id);

            if (stateActivity__with_Rule2 == null)
            {
                return HttpNotFound();
            }

            ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", stateActivity__with_Rule2.ActivityID);
            ViewBag.Rule2_ID = new SelectList(db.Rule2_Master.OrderBy(m => m.Name), "Id", "Name", stateActivity__with_Rule2.Rule2_ID);
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", stateActivity__with_Rule2.StateID);

            return View(stateActivity__with_Rule2);
        }


        // POST: StateActivity__with_Rule2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StateID,ActivityID,Rule2_ID,StartDate,EndDate,Status")] StateActivity__with_Rule2 stateActivity__with_Rule2)
        {
            try
            {
                ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", stateActivity__with_Rule2.ActivityID);
                ViewBag.Rule2_ID = new SelectList(db.Rule2_Master.OrderBy(m => m.Name), "Id", "Name", stateActivity__with_Rule2.Rule2_ID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", stateActivity__with_Rule2.StateID);

                if (stateActivity__with_Rule2.StartDate > stateActivity__with_Rule2.EndDate)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                    return View();
                }
                else
                {
                    List<StateActivity__with_Rule2> getName = db.StateActivity__with_Rule2.Where(u => u.StateID == stateActivity__with_Rule2.StateID
                    && u.Rule2_ID == stateActivity__with_Rule2.Rule2_ID && u.StartDate == stateActivity__with_Rule2.StartDate
                    && u.EndDate == stateActivity__with_Rule2.EndDate && u.Status == stateActivity__with_Rule2.Status
                    && u.ActivityID == stateActivity__with_Rule2.ActivityID && u.Id != stateActivity__with_Rule2.Id).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {

                        db.Entry(stateActivity__with_Rule2).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Updated Successfully");
                        return RedirectToAction("Index");
                    }
                }
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
                return View();
            }
            catch (Exception)
            {
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
            }
            return View();
        }


        // GET: StateActivity__with_Rule2/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StateActivity__with_Rule2 stateActivity__with_Rule2 = db.StateActivity__with_Rule2.Find(id);

            if (stateActivity__with_Rule2 == null)
            {
                return HttpNotFound();
            }

            return View(stateActivity__with_Rule2);
        }


        // POST: StateActivity__with_Rule2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal? id)
        {
            try
            {
                StateActivity__with_Rule2 stateActivity__with_Rule2 = db.StateActivity__with_Rule2.Find(id);
                db.StateActivity__with_Rule2.Remove(stateActivity__with_Rule2);
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
            var ruleList = from s in db.Rule2_Master select new { s.Id, Name = string.Concat(s.Name, ", Units - ", s.Unit, ", Hours - ", s.Hours) };

            return Json(new SelectList(ruleList.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ActivityList(int StateID)
        {
            var state = from s in db.State_Activity_Mapping
                        join c in db.Activity_Master on s.ActivityID equals c.ID
                        where s.StateID == StateID
                        select new { c.ID, c.Name };
            return Json(new SelectList(state.ToArray().Distinct().OrderBy(m => m.Name), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ActivityListEdit(int StateID)
        {
            var state = from s in db.State_Activity_Mapping
                        join c in db.Activity_Master on s.ActivityID equals c.ID
                        where s.StateID == StateID
                        select new { c.ID, c.Name };
            return Json(new SelectList(state.ToArray().Distinct().OrderBy(m => m.Name), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }
    }
}
