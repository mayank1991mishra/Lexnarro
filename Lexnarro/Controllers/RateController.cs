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
    public class RateController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: Rate_Card
        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();            
        }


        public ActionResult GetRateList()
        {
            var rate_card = from s in db.Rate_Card.Include(s => s.Plan_Master).Where(s => s.Status == "Active")
                            select s;

            var rates = rate_card.ToList().Select(S => new
            {
                S.Rate_Id,
                StartDate = S.StartDate.ToShortDateString(),
                EndDate = S.EndDate.ToString().Split(' ')[0],
                S.Amount,
                S.Plan_Master.Plan,
                S.Status
            });

            return Json(new { data = rates }, JsonRequestBehavior.AllowGet);
        }


        // GET: Rate_Card/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Rate_Card rate_Card = db.Rate_Card.Find(id);

            if (rate_Card == null)
                return HttpNotFound();

            return View(rate_Card);
        }


        // GET: Rate_Card/Create
        public ActionResult Create()
        {
            ViewBag.Plan_Id = new SelectList(db.Plan_Master.OrderBy(m => m.Plan), "Plan_ID", "Plan");
            return View();
        }


        // POST: Rate_Card/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Rate_Id,Plan_Id,StartDate,EndDate,Amount,Status")] Rate_Card rate_Card)
        {
            ViewBag.Plan_Id = new SelectList(db.Plan_Master.OrderBy(m => m.Plan), "Plan_ID", "Plan");
            try
            {
                //if (ModelState.IsValid)
                //{
                    if (rate_Card.StartDate > rate_Card.EndDate)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                        return View();
                    }
                    else
                    {
                        var getName = db.Rate_Card.Where(u => u.Plan_Id == rate_Card.Plan_Id && u.StartDate == rate_Card.StartDate 
                        && u.EndDate == rate_Card.EndDate && u.Status == rate_Card.Status).ToList();
                        if (getName.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            db.Rate_Card.Add(rate_Card);
                            db.SaveChanges();

                            TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                            return RedirectToAction("Index");
                        }
                    //}
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
                return View();
            }

            //return View();
        }


        // GET: Rate_Card/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Rate_Card rate_Card = db.Rate_Card.Find(id);

            if (rate_Card == null)
                return HttpNotFound();

            ViewBag.Plan_Id = new SelectList(db.Plan_Master.OrderBy(m => m.Plan), "Plan_ID", "Plan", rate_Card.Plan_Id);

            return View(rate_Card);
        }


        // POST: Rate_Card/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Rate_Id,Plan_Id,StartDate,EndDate,Amount,Status")] Rate_Card rate_Card)
        {
            ViewBag.Plan_Id = new SelectList(db.Plan_Master.OrderBy(m => m.Plan), "Plan_ID", "Plan");

            try
            {
                //if (ModelState.IsValid)
                //{
                    if (rate_Card.StartDate > rate_Card.EndDate)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                        return View();
                    }
                    else
                    {
                        var getName = db.Rate_Card.Where(u => u.Plan_Id == rate_Card.Plan_Id && u.StartDate == rate_Card.StartDate
                        && u.EndDate == rate_Card.EndDate && u.Status == rate_Card.Status && u.Plan_Id != rate_Card.Plan_Id).ToList();
                        if (getName.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            db.Entry(rate_Card).State = EntityState.Modified;
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


        // GET: Rate_Card/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Rate_Card rate_Card = db.Rate_Card.Find(id);

            if (rate_Card == null)
                return HttpNotFound();

            return View(rate_Card);
        }


        // POST: Rate_Card/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal? id)
        {
            try
            {
                Rate_Card rate_Card = db.Rate_Card.Find(id);
                db.Rate_Card.Remove(rate_Card);
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
