using Lexnarro.HelperClasses;
using Lexnarro.Models;
using Lexnarro.ViewModels;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private LaxNarroEntities db = null;
        private readonly ToasterMessage toastNotifications = null;


        public UserController()
        {
            db = new LaxNarroEntities();
            toastNotifications = new ToasterMessage();
        }


        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        //Changes pending
        public ActionResult GetUsers()
        {
            string Role = UserHelper.GetUserRole();
            string User = UserHelper.GetUserId();

            decimal userID = Convert.ToDecimal(User);

            var user_Profile = db.User_Profile.Include(u => u.State_Master).Include(u => u.State_Master1)
                .Include(u => u.Country_Master).Where(u => u.ID == userID).ToList();

            var users = user_Profile.Select(S => new
            {
                S.ID,
                S.FirstName,
                S.LastName,
                Country = S.Country_Master.Name,
                State = S.State_Master.Name,
                StateEnrolled = S.State_Master1.Name,
                S.EmailAddress
            });

            return Json(new { data = users }, JsonRequestBehavior.AllowGet);
        }

        
        private string GetFinancialYear()
        {
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;

            return FinYear;
        }


        public ActionResult Payment()
        {
            return View();
        }


        public ActionResult GetPayments(decimal id)
        {
            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

            var userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u => u.Rate_Card).Include(u => u.Plan_Master).Where(u => u.User_ID == id).ToList();

            if (userTransaction[0].User_ID != userID)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                return RedirectToAction("Index");
            }

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
            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

            User_Transaction_Master userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u => u.Rate_Card).Include(u => u.Plan_Master).Where(u => u.Id == id).SingleOrDefault();

            if (userTransaction.User_ID != userID)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                return RedirectToAction("Index");
            }

            if (userTransaction == null)
                return HttpNotFound();

            return View(userTransaction);
        }


        // GET: User_Profile/Details/5
        [OutputCache(Duration = 30)]
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



        #region EDIT USER PROFILE
        // GET: User_Profile/Edit/5
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
                user_Profile.Firm = userProfileEdit.Firm;
                user_Profile.PhoneNumber = userProfileEdit.PhoneNumber;
                user_Profile.Date = userProfileEdit.Date;
                user_Profile.Address = userProfileEdit.Address;
                user_Profile.Password = userProfileEdit.Password;
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

        #endregion



        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }



        public JsonResult StateList(int CountryID)
        {
            IQueryable<State_Master> state = from s in db.State_Master
                                             where s.Country_ID == CountryID
                                             select s;
            return Json(new SelectList(state.ToArray().OrderBy(m => m.Name), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }



        #region USER SIGNUP AND ACCOUNT ACTIVATION

        [AllowAnonymous]
        public ActionResult Signup()
        {
            ViewBag.CountryID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name");
            ViewBag.Role_id = new SelectList(db.Role_Master, "Id", "Name");
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            ViewBag.StateEnrolled = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(User_Profile userProfileCreate)
        {
            try
            {
                var getName = db.User_Profile.Where(u => u.EmailAddress == userProfileCreate.EmailAddress).ToList();
                if (getName.Count > 0)
                    ViewBag.message = ToasterMessage.Message(ToastType.info, "User already exist");

                else
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        userProfileCreate.Date = DateTime.Today;
                        userProfileCreate.UserName = userProfileCreate.EmailAddress;
                        userProfileCreate.Role_id = 1;
                        userProfileCreate.Current_Financial_year = GetFinancialYear();

                        userProfileCreate.AccountConfirmed = "No";
                        Guid activationCode = Guid.NewGuid();
                        userProfileCreate.ActivationCode = activationCode;

                        string activationLink = Request.Url.Scheme + "://" + Request.Url.Authority + "/User/ConfirmAccount/" + activationCode;

                        db.User_Profile.Add(userProfileCreate);
                        db.SaveChanges();

                        decimal id = userProfileCreate.ID;

                        var user_Transaction_Master = new User_Transaction_Master();

                        var plan_id = db.Plan_Master.FirstOrDefault(x => x.Plan == "Demo");
                        decimal planID = Convert.ToDecimal(plan_id.Plan_ID);
                        var rate_id = db.Rate_Card.FirstOrDefault(x => x.Plan_Id == planID);
                        decimal rateID = Convert.ToDecimal(rate_id.Rate_Id);
                        user_Transaction_Master.Rate_ID = rateID; //********************
                        user_Transaction_Master.User_ID = id;
                        user_Transaction_Master.PlanID = planID;  //********************
                        user_Transaction_Master.Amount = 0;
                        user_Transaction_Master.Start_Date = DateTime.Today;
                        user_Transaction_Master.End_Date = DateTime.Today.AddMonths(3);
                        user_Transaction_Master.Payment_Status = "N/A";
                        user_Transaction_Master.Transection_ID = null;
                        user_Transaction_Master.Status = "Active";
                        user_Transaction_Master.Payment_Date = null;
                        user_Transaction_Master.Invoice_No = null;
                        user_Transaction_Master.Payment_Method = "N/A";


                        db.User_Transaction_Master.Add(user_Transaction_Master);
                        db.SaveChanges();

                        var mailLog = new MailLog
                        {
                            UserID = id,
                            ReminderOne = DateTime.Today.AddDays(2),
                            ReminderTwo = DateTime.Today.AddDays(7),
                            ReminderThree = DateTime.Today.AddDays(14),
                            ReminderFour = DateTime.Today.AddDays(30),
                            ReminderFive = DateTime.Today.AddDays(60),
                            ReminderSix = DateTime.Today.AddDays(90)
                        };

                        db.MailLogs.Add(mailLog);
                        db.SaveChanges();
                        transaction.Commit();


                        SendMail(userProfileCreate.FirstName, userProfileCreate.LastName, userProfileCreate.EmailAddress,
                        activationLink, "~/EmailTemplate/SignupConfirmation.html", "Lex Narro - Verify email to activate account");

                        //SendMail("sanjay", "kumar", "forever.parv@gmail.com",
                        //        "", "~/EmailTemplate/SignupConfirmation.html");

                        TempData["message"] = ToasterMessage.MessageCenter(ToastType.success, "Registration successful. Check your email for account activation.");
                        return RedirectToAction("Login", "Account");
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.info, "something went wrong. Please contact us from home page.");
            }

            ViewBag.CountryID = new SelectList(db.Country_Master.OrderBy(m => m.Name), "ID", "Name");
            ViewBag.Role_id = new SelectList(db.Role_Master, "Id", "Name");
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            ViewBag.StateEnrolled = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");
            return View();
        }


        [AllowAnonymous]
        public ActionResult ConfirmAccount()
        {
            //ViewBag.message = @"<script type='text/javascript' language='javascript'>alert(""Invalid activation code"")</script>";
            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                //LaxNarroEntities usersEntities = new LaxNarroEntities();
                User_Profile userActivation = db.User_Profile.Where(p => p.ActivationCode == activationCode).FirstOrDefault();
                if (userActivation != null)
                {
                    //usersEntities.User_Profile.Remove(userActivation);
                    if (userActivation.AccountConfirmed == "Yes")
                        ViewBag.Status = "This account has already been activated. If you have forgotton your " +
                            "password click on forget password on login page.";
                    else
                    {
                        userActivation.AccountConfirmed = "Yes";

                        db.Entry(userActivation).State = EntityState.Modified;

                        db.SaveChanges();

                        string result = SendMail(userActivation.FirstName, userActivation.LastName,
                            userActivation.EmailAddress, "", "~/EmailTemplate/AccountValidated.html", "Lex Narro - Account Activated");

                        if (result == "success")
                        {

                        }
                        ViewBag.Status = "Thank you for confirming your account. Now you can Login and start using our services.";
                    }
                }
                else
                    ViewBag.Status = "Invalid request";
            }

            return View();
        }



        private string SendMail(string firstName, string lastName, string emailAddress, string activationLink, string emailTemplate, string subject)
        {
            string result = "";
            try
            {
                string body = string.Empty;

                using (StreamReader reader = new StreamReader(Server.MapPath(emailTemplate)))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{name}", firstName + " " + lastName);
                body = body.Replace("{Email}", emailAddress);

                string link = "<br /><a href = '" + activationLink + "'>Click here to activate your account.</a>";

                body = body.Replace("{link}", link);

                MailMessage mail = new MailMessage();
                mail.To.Add(emailAddress);
                mail.From = new MailAddress("mail@lexnarro.com.au", "Lex Narro");
                mail.Subject = subject;

                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.lexnarro.com.au";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = 25;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("mail@lexnarro.com.au", "lexnarro@123");
                smtp.Send(mail);

                result = "success";
            }
            catch (Exception ee)
            {
                result = "";
            }

            return result;
        }



        [AllowAnonymous]
        public ActionResult Unsubscribe()
        {
            if (RouteData.Values["id"] != null)
            {
                decimal id = Convert.ToDecimal(RouteData.Values["id"]);
                User_Profile unsubscribeUser = db.User_Profile.Where(p => p.ID == id).FirstOrDefault();
                if (unsubscribeUser != null)
                {
                    unsubscribeUser.MailUnsubscribed = "Yes";

                    db.Entry(unsubscribeUser).State = EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.Status = unsubscribeUser.EmailAddress + " has been unsubscribed from our e-mailing services.";
                }
                else
                    ViewBag.Status = "Invalid Request.";
            }

            return View();
        }

        #endregion
    }
}
