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
    public class CategoryController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();


        // GET: Category_Master
        public ViewResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        public ActionResult GetCategories()
        {
            IQueryable<Category_Master> Category_Master = from s in db.Category_Master select s;

            var categories = Category_Master.ToList().Select(S => new
            {
                S.ID,
                S.Name,
                S.ShortName
            });

            return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
        }


        // GET: Category_Master/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Category_Master category_Master = db.Category_Master.Find(id);

            if (category_Master == null)
                return HttpNotFound();
            
            return View(category_Master);
        }


        // GET: Category_Master/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Category_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ShortName")] Category_Master category_Master)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    var getName = db.Category_Master.Where(u => u.Name == category_Master.Name).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Category_Master.Add(category_Master);
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


        // GET: Category_Master/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Category_Master category_Master = db.Category_Master.Find(id);

            if (category_Master == null)
                return HttpNotFound();
            
            return View(category_Master);
        }


        // POST: Category_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ShortName")] Category_Master category_Master)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    var getName = db.Category_Master.Where(u => u.Name == category_Master.Name && u.ID != category_Master.ID).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Entry(category_Master).State = EntityState.Modified;
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


        // GET: Category_Master/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Category_Master category_Master = db.Category_Master.Find(id);

            if (category_Master == null)
                return HttpNotFound();
            
            return View(category_Master);
        }


        // POST: Category_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal? id)
        {            
            try
            {
                Category_Master category_Master = db.Category_Master.Find(id);
                db.Category_Master.Remove(category_Master);
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
