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
    public class StateCategoryRuleFourController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: State_Category_With_Rule4_Mapping
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
            var state_Category_With_Rule4_Mapping = db.State_Category_With_Rule4_Mapping.Include(s => s.Category_Master)
                .Include(s => s.Rule4_Master).Include(s => s.State_Master).Where(s => s.Status == "Active");

            var subData = state_Category_With_Rule4_Mapping.ToList().Select(S => new
            {
                S.Id,
                State = S.State_Master.Name,
                Category = S.Category_Master.Name,
                Rule = S.Rule4_Master.Name,
                StartDate = S.StartDate.ToShortDateString(),
                EndDate = S.EndDate.ToString().Split(' ')[0],
                S.Status
            });

            return Json(new { data = subData }, JsonRequestBehavior.AllowGet);
        }


        // GET: State_Category_With_Rule4_Mapping/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Category_With_Rule4_Mapping state_Category_With_Rule4_Mapping = db.State_Category_With_Rule4_Mapping.Find(id);

            if (state_Category_With_Rule4_Mapping == null)
            {
                return HttpNotFound();
            }

            return View(state_Category_With_Rule4_Mapping);
        }


        // GET: State_Category_With_Rule4_Mapping/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Category_Master.OrderBy(m => m.Name), "ID", "Name");
            ViewBag.Rule4_ID = new SelectList(db.Rule4_Master.OrderBy(m => m.Name), "Id", "Name");
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            return View();
        }


        // POST: State_Category_With_Rule4_Mapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StateID,CategoryID,Rule4_ID,StartDate,EndDate,Status")] State_Category_With_Rule4_Mapping state_Category_With_Rule4_Mapping)
        {
            try
            {
                ViewBag.CategoryID = new SelectList(db.Category_Master.OrderBy(m => m.Name), "ID", "Name", state_Category_With_Rule4_Mapping.CategoryID);
                ViewBag.Rule4_ID = new SelectList(db.Rule4_Master.OrderBy(m => m.Name), "Id", "Name", state_Category_With_Rule4_Mapping.Rule4_ID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Category_With_Rule4_Mapping.StateID);

                //if (ModelState.IsValid)
                //{
                if (state_Category_With_Rule4_Mapping.StartDate > state_Category_With_Rule4_Mapping.EndDate)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                    return View();
                }
                else
                {
                    System.Collections.Generic.List<State_Category_With_Rule4_Mapping> getName = db.State_Category_With_Rule4_Mapping
                    .Where(u => u.StateID == state_Category_With_Rule4_Mapping.StateID
                    && u.CategoryID == state_Category_With_Rule4_Mapping.CategoryID
                    && u.Rule4_ID == state_Category_With_Rule4_Mapping.Rule4_ID
                    && u.StartDate == state_Category_With_Rule4_Mapping.StartDate
                    && u.EndDate == state_Category_With_Rule4_Mapping.EndDate
                    && u.Status == state_Category_With_Rule4_Mapping.Status).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.State_Category_With_Rule4_Mapping.Add(state_Category_With_Rule4_Mapping);
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


        // GET: State_Category_With_Rule4_Mapping/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Category_With_Rule4_Mapping state_Category_With_Rule4_Mapping = db.State_Category_With_Rule4_Mapping.Find(id);

            if (state_Category_With_Rule4_Mapping == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(db.Category_Master.OrderBy(m => m.Name), "ID", "Name", state_Category_With_Rule4_Mapping.CategoryID);
            ViewBag.Rule4_ID = new SelectList(db.Rule4_Master.OrderBy(m => m.Name), "Id", "Name", state_Category_With_Rule4_Mapping.Rule4_ID);
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Category_With_Rule4_Mapping.StateID);
            return View(state_Category_With_Rule4_Mapping);
        }


        // POST: State_Category_With_Rule4_Mapping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StateID,CategoryID,Rule4_ID,StartDate,EndDate,Status")] State_Category_With_Rule4_Mapping state_Category_With_Rule4_Mapping)
        {
            try
            {
                ViewBag.CategoryID = new SelectList(db.Category_Master.OrderBy(m => m.Name), "ID", "Name", state_Category_With_Rule4_Mapping.CategoryID);
                ViewBag.Rule4_ID = new SelectList(db.Rule4_Master.OrderBy(m => m.Name), "Id", "Name", state_Category_With_Rule4_Mapping.Rule4_ID);
                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", state_Category_With_Rule4_Mapping.StateID);

                //if (ModelState.IsValid)
                //{
                    System.Collections.Generic.List<State_Category_With_Rule4_Mapping> getName = db.State_Category_With_Rule4_Mapping
                    .Where(u => u.StateID == state_Category_With_Rule4_Mapping.StateID
                    && u.CategoryID == state_Category_With_Rule4_Mapping.CategoryID
                    && u.Rule4_ID == state_Category_With_Rule4_Mapping.Rule4_ID
                    && u.StartDate == state_Category_With_Rule4_Mapping.StartDate
                    && u.EndDate == state_Category_With_Rule4_Mapping.EndDate
                    && u.Status == state_Category_With_Rule4_Mapping.Status && u.Id != state_Category_With_Rule4_Mapping.Id).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Entry(state_Category_With_Rule4_Mapping).State = EntityState.Modified;
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


        // GET: State_Category_With_Rule4_Mapping/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State_Category_With_Rule4_Mapping state_Category_With_Rule4_Mapping = db.State_Category_With_Rule4_Mapping.Find(id);

            if (state_Category_With_Rule4_Mapping == null)
            {
                return HttpNotFound();
            }
            return View(state_Category_With_Rule4_Mapping);
        }


        // POST: State_Category_With_Rule4_Mapping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                State_Category_With_Rule4_Mapping state_Category_With_Rule4_Mapping = db.State_Category_With_Rule4_Mapping.Find(id);
                db.State_Category_With_Rule4_Mapping.Remove(state_Category_With_Rule4_Mapping);
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
            var ruleList = from s in db.Rule4_Master select new { s.Id, Name = string.Concat(s.Name, " Min Units - ", s.MinUnits) };

            return Json(new SelectList(ruleList.ToArray().Distinct().OrderBy(m => m.Name), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }
    }
}
