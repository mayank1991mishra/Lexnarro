using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TransactionHistoryController : Controller
    {
        private LaxNarroEntities db = null;
        private readonly ToasterMessage toastNotifications = null;


        public TransactionHistoryController()
        {
            db = new LaxNarroEntities();
            toastNotifications = new ToasterMessage();
        }


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetData()
        {
            var userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u => u.Rate_Card).Include(u => u.Plan_Master).ToList();

            var transactions = userTransaction.ToList().Select(item => new
            {
                item.Id,
                item.User_ID,
                Name = item.User_Profile.FirstName + " " + item.User_Profile.LastName,
                item.Plan_Master.Plan,
                item.Amount,
                item.User_Profile.EmailAddress,
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



        // GET: TransactionHistory
        public ActionResult Details(decimal id)
        {
            User_Transaction_Master userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u=>u.Rate_Card).Include(u=>u.Plan_Master).Where(u => u.Id == id).SingleOrDefault();


            if (userTransaction == null)
            {
                return HttpNotFound();
            }

            return View(userTransaction);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}