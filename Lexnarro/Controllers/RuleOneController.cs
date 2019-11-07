using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Lexnarro.Models;
using System;
using Lexnarro.HelperClasses;
using System.Data.Entity.Validation;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RuleOneController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();


        // GET: Rule1_Master
        public ActionResult Index()
        {           
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        public ActionResult GetRules()
        {
            var rule_1_master = from s in db.Rule1_Master select s;

            var rules = rule_1_master.ToList().Select(S => new
            {
                S.Id,
                S.Name,
                S.Min,
                S.Max
            });

            return Json(new { data = rules }, JsonRequestBehavior.AllowGet);
        }


        // GET: Rule1_Master/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Rule1_Master rule1_Master = db.Rule1_Master.Find(id);

            if (rule1_Master == null)
                return HttpNotFound();

            return View(rule1_Master);
        }


        // GET: Rule1_Master/Create
        public ActionResult Create()
        {            
            return View();
        }


        // POST: Rule1_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Min,Max")] Rule1_Master rule1_Master)
        {            
            try
            {
                //if (ModelState.IsValid)
                //{
                    var getName = db.Rule1_Master.Where(u => u.Name == rule1_Master.Name).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Rule1_Master.Add(rule1_Master);
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


        // GET: Rule1_Master/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Rule1_Master rule1_Master = db.Rule1_Master.Find(id);

            if (rule1_Master == null)
                return HttpNotFound();

            return View(rule1_Master);
        }


        // POST: Rule1_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Min,Max")] Rule1_Master rule1_Master)
        {            
            try
            {
                //if (ModelState.IsValid)
                //{
                    var getName = db.Rule1_Master.Where(u => u.Name == rule1_Master.Name && u.Id != rule1_Master.Id).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Entry(rule1_Master).State = EntityState.Modified;
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

        // GET: Rule1_Master/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Rule1_Master rule1_Master = db.Rule1_Master.Find(id);

            if (rule1_Master == null)
                return HttpNotFound();

            return View(rule1_Master);
        }


        // POST: Rule1_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            try
            {
                Rule1_Master rule1_master = db.Rule1_Master.Find(id);
                db.Rule1_Master.Remove(rule1_master);
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
