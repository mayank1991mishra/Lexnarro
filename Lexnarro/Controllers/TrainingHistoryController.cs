using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainingHistoryController : Controller
    {
        private LaxNarroEntities db = null;
        private readonly ToasterMessage toastNotifications = null;

        public TrainingHistoryController()
        {
            db = new LaxNarroEntities();
            toastNotifications = new ToasterMessage();
        }

        // GET: TransactionHistory
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetData()
        {
            var userTraining = db.User_Training_Transaction.Include(x => x.User_Profile).Include(x => x.Category_Master)
                 .Include(x => x.Activity_Master).Include(x => x.Sub_Activity_Master).Include(x => x.State_Master).ToList();

            var training = userTraining.Select(S => new
            {
                S.Id,
                S.User_Id,
                S.Date,
                Name = S.User_Profile.FirstName + " " + S.User_Profile.LastName,
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


        // GET: TransactionHistory/Details/5
        public ActionResult Details(decimal? id)
        {           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User_Training_Transaction user_Training_Transaction = db.User_Training_Transaction.Find(id);

            ViewBag.SearchedUserID = user_Training_Transaction.User_Id;

            if (user_Training_Transaction == null)
            {
                return HttpNotFound();
            }

            return View(user_Training_Transaction);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}