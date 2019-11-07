using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly LaxNarroEntities db = null;

        public AdminController()
        {
            db = new LaxNarroEntities();
        }


        // GET: Admin
        public new ActionResult Profile()
        {
            return View();
        }



        public ActionResult GetAdminProfile()
        {
            var adminProfile = db.User_Profile.Include(u => u.Role_Master).Where(u => u.Role_Master.Name == "Admin").ToList();

            var admin = adminProfile.Select(S => new
            {
                S.ID,
                S.FirstName,
                S.LastName,
                S.OtherName,
                S.State_Master.Name,
                StateEnrolled = S.State_Master1.Name,
                S.Password,
                S.PostCode,
                S.StreetName,
                S.StreetNumber,
                S.Suburb,
                S.EmailAddress,
                S.Address,
                Country = S.Country_Master.Name
            });

            return Json(new { data = admin }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Details(decimal? id)
        {
            User_Profile user_Profile = null;

            if (id != Convert.ToDecimal(UserHelper.GetUserId()))
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized Access");
                return RedirectToAction("Index");
            }

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            user_Profile = db.User_Profile.Find(id);

            if (user_Profile == null)
                return HttpNotFound();

            return View(user_Profile);
        }



        public ActionResult Edit(decimal? id)
        {
            if (id != Convert.ToDecimal(UserHelper.GetUserId()))
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized Access");
                return RedirectToAction("Index");
            }

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User_Profile user_Profile = db.User_Profile.Find(id);

            if (user_Profile == null)
                return HttpNotFound();

            ViewBag.CountryID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name", user_Profile.CountryID);

            ViewBag.Role_id = new SelectList(db.Role_Master, "Id", "Name", user_Profile.Role_id);

            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", user_Profile.StateID);

            ViewBag.StateEnrolled = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", user_Profile.StateEnrolled);

            return View(user_Profile);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User_Profile userProfileEdit)
        {
            if (userProfileEdit.ID != Convert.ToDecimal(UserHelper.GetUserId()))
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized Access");
                return RedirectToAction("Index");
            }

            User_Profile user_Profile = db.User_Profile.Find(userProfileEdit.ID);

            try
            {
                ViewBag.CountryID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name", user_Profile.CountryID);

                ViewBag.Role_id = new SelectList(db.Role_Master, "Id", "Name", user_Profile.Role_id);

                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", user_Profile.StateID);

                ViewBag.StateEnrolled = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", user_Profile.StateEnrolled);

                user_Profile.UserName = userProfileEdit.EmailAddress;
                user_Profile.FirstName = userProfileEdit.FirstName;
                user_Profile.LastName = userProfileEdit.LastName;
                user_Profile.OtherName = userProfileEdit.OtherName;
                user_Profile.StateID = userProfileEdit.StateID;
                user_Profile.StreetName = userProfileEdit.StreetName;
                user_Profile.StreetNumber = userProfileEdit.StreetNumber;
                user_Profile.PostCode = userProfileEdit.PostCode;
                user_Profile.Suburb = userProfileEdit.Suburb;
                user_Profile.LawSocietyNumber = userProfileEdit.LawSocietyNumber;
                user_Profile.PhoneNumber = userProfileEdit.PhoneNumber;

                user_Profile.Password = userProfileEdit.Password;

                user_Profile.Date = userProfileEdit.Date;
                user_Profile.Address = userProfileEdit.Address;
                db.Entry(user_Profile).State = EntityState.Modified;
                db.SaveChanges();

                TempData["message"] = ToasterMessage.Message(ToastType.success, "Updated Successfully");
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

           // return RedirectToAction("Index");
            return RedirectToAction("Profile");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}