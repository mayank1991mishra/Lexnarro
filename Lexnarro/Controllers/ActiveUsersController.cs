using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Lexnarro.ViewModels;
using System.Data.Entity.Validation;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ActiveUsersController : Controller
    {
        private LaxNarroEntities db = null;
        private readonly ToasterMessage toastNotifications = null;


        public ActiveUsersController()
        {
            db = new LaxNarroEntities();
            toastNotifications = new ToasterMessage();
        }


        // GET: ActiveUsers
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Details(decimal? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User_Profile user_Profile = db.User_Profile.Find(id);

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
        public ActionResult Edit(UserProfileEditViewModel userProfileEdit)
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

            return RedirectToAction("Index");
        }



        #region  ACTIVE USERS AND TRANSACTION DETAILS


        public ActionResult GetActiveUsers()
        {
            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

            IQueryable<User_Transaction_Master> active_User_Profile = db.User_Transaction_Master.Include(u => u.User_Profile).
                Where(z => z.Payment_Status == "Paid" && z.End_Date >= DateTime.Today).Distinct();

            var activeUsers = active_User_Profile.ToList().Select(item => new
            {
                item.Id,
                item.User_ID,
                item.User_Profile.FirstName,
                item.User_Profile.LastName,
                item.User_Profile.OtherName,
                item.User_Profile.EmailAddress,
                StartDate = item.Start_Date.ToShortDateString(),
                EndDate = item.End_Date.ToString().Split(' ')[0],
                StateEnrolled = item.User_Profile.State_Master1.Name,
                item.Payment_Status
            });

            return Json(new { data = activeUsers }, JsonRequestBehavior.AllowGet);
        }        


        public ActionResult Payment(decimal? id)
        {
            ViewBag.id = id;
            return View();
        }


        public ActionResult GetPaidTransactions(decimal id)
        {
            IQueryable<User_Transaction_Master> userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u => u.Rate_Card).Include(u => u.Plan_Master).Where(u => u.User_ID == id && u.Payment_Status == "Paid");

            var transactions = userTransaction.ToList().Select(item => new
            {
                item.Id,
                item.User_ID,
                item.User_Profile.FirstName,
                item.User_Profile.LastName,
                item.Plan_Master.Plan,
                item.Amount,
                StartDate = item.Start_Date.ToShortDateString(),
                EndDate = item.End_Date.ToString().Split(' ')[0],
                item.Payment_Status,
                item.Transection_ID,
                item.Status,
                PaymentDate = Convert.ToDateTime(item.Payment_Date).ToShortDateString(),//item.Payment_Date.ToString().Split(' ')[0],
                item.Invoice_No
            });

            return Json(new { data = transactions }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PaymentDetails(decimal id)
        {
            User_Transaction_Master userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u => u.Rate_Card).Include(u => u.Plan_Master).Where(u => u.Id == id).SingleOrDefault();

            if (userTransaction == null)
                return HttpNotFound();

            return View(userTransaction);
        }
        #endregion



        #region ACTIVE USERS AND CPD RECORDS

        public ActionResult Training(decimal? id)
        {
            ViewBag.id = id;
            return View();
        }


        public ActionResult GetTrainingList(decimal id)
        {
            IQueryable<User_Training_Transaction> userTraining = db.User_Training_Transaction.Include(x => x.User_Profile).Include(x => x.Category_Master)
                 .Include(x => x.Activity_Master).Include(x => x.Sub_Activity_Master).Include(x => x.State_Master).Where(x => x.User_Id == id);

            var training = userTraining.Select(S => new
            {
                S.Id,
                S.User_Id,
                S.Date,
                S.User_Profile.FirstName,
                S.User_Profile.LastName,
                Country = S.User_Profile.Country_Master.Name,
                StateEnrolled = S.State_Master.Name,
                Category = S.Category_Master.Name,
                Activity = S.Activity_Master.Name,
                SubActivity = S.Sub_Activity_Master.Name,
                S.Hours,
                S.Provider,
                S.Financial_Year,
                S.User_Profile.EmailAddress
            });

            return Json(new { data = training }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult TrainingDetails(decimal? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User_Training_Transaction userTraining = db.User_Training_Transaction.Include(u => u.User_Profile).
                Include(u => u.User_Training_Status).Where(u => u.Id == id).SingleOrDefault();

            if (userTraining == null)
                return HttpNotFound();

            return View(userTraining);
        }


        #endregion



        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}