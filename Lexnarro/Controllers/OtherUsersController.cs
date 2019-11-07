using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OtherUsersController : Controller
    {

        private LaxNarroEntities db = null;
        private readonly ToasterMessage toastNotifications = null;


        public OtherUsersController()
        {
            db = new LaxNarroEntities();
            toastNotifications = new ToasterMessage();
        }



        // GET: OtherUsers
        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }



        public ActionResult GetUsers()
        {
            DateTime dt = DateTime.Today;
            var user_Profile = db.User_Transaction_Master.Include(u => u.User_Profile).Include(z => z.User_Profile.Country_Master)
               .Include(z => z.User_Profile.State_Master).Include(z => z.User_Profile.State_Master1).Where(z => z.End_Date < dt
               || z.Payment_Status == "N/A" && z.User_Profile.Role_Master.Name != "Admin").ToList();

            var users = user_Profile.Select(S => new
            {
                S.Id,
                S.User_ID,
                S.User_Profile.FirstName,
                S.User_Profile.LastName,
                Country = S.User_Profile.Country_Master.Name,
                State = S.User_Profile.State_Master.Name,
                StateEnrolled = S.User_Profile.State_Master1.Name,
                S.User_Profile.EmailAddress
            });

            return Json(new { data = users }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Details(decimal? id)
        {
            User_Profile user_Profile = null;

            if (UserHelper.GetUserRole() == "Admin")
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                user_Profile = db.User_Profile.Find(id);

                if (user_Profile == null)
                    return HttpNotFound();

            }
            else
            {
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
            }

            return View(user_Profile);
        }



        #region OTHER USERS AND TRANSACTION RECORDS

        public ActionResult Payment(decimal? id)
        {
            ViewBag.id = id;
            return View();
        }


        public ActionResult GetTransactions(decimal id)
        {
            IQueryable<User_Transaction_Master> userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u => u.Rate_Card).Include(u => u.Plan_Master).Where(u => u.User_ID == id);

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
                PaymentDate = item.Payment_Date.ToString().Split(' ')[0],
                item.Payment_Status,
                item.Transection_ID,
                item.Status,
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



        #region OTHER USERS AND CPD RECORDS

        public ActionResult Training(decimal? id)
        {
            ViewBag.id = id;
            return View();
        }


        public ActionResult GetTraining(decimal id)
        {
            var userTraining = db.User_Training_Transaction.Include(x => x.User_Profile).Include(x => x.Category_Master)
                 .Include(x => x.Activity_Master).Include(x => x.Sub_Activity_Master).Include(x => x.State_Master).Where(x => x.User_Id == id).ToList();

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
                SubActivity = (S.SubActivity_Id == null) ? "" : S.Sub_Activity_Master.Name,
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