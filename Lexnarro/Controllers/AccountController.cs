using Lexnarro.Models;
using Lexnarro.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Lexnarro.HelperClasses;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Threading;

namespace Lexnarro.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Private Properties    
        /// <summary>  
        /// Database Store property.    
        /// </summary>  
        private LaxNarroEntities db = null;

        public static string role = string.Empty;
        #endregion


        public AccountController()
        {
            db = new LaxNarroEntities();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.message = TempData["message"].ToString();

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(string name, string email, string message)
        {
            //string result = SendMail(name, email, message, "~/EmailTemplate/Message.html");

            //if (result == "success")
            //{
            //    TempData["message"] = ToasterMessage.Message(ToastType.success, "Message sent. We will contact you shortly.");                
            //}
            //else
            //{
            //    TempData["message"] = ToasterMessage.Message(ToastType.error, "Message not sent. Please try again after some time.");
            //}

            return View();
        }


        [AllowAnonymous]
        public void SendMail(string name, string email, string message)
        {
            try
            {
                string body = string.Empty;

                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate/Message.html")))
                    body = reader.ReadToEnd();

                body = body.Replace("{name}", name);
                body = body.Replace("{Email}", email);

                body = body.Replace("{message}", message);

                MailMessage mail = new MailMessage();
                mail.To.Add("mail@lexnarro.com.au");
                mail.From = new MailAddress("mail@lexnarro.com.au");
                mail.Subject = "Lexnarro - Contact Page Message.";

                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.lexnarro.com.au";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = 25;
                smtp.UseDefaultCredentials = false;
                //smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("mail@lexnarro.com.au", "lexnarro@123");
                smtp.Send(mail);

                //result = "success";
            }
            catch (Exception d)
            {
                //result = "Error";
            }
            //return result;
        }


        #region Login methods    
        /// <summary>  
        /// GET: /Account/Login    
        /// </summary>  
        /// <param name="returnUrl">Return URL parameter</param>  
        /// <returns>Return login view</returns>  
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if (TempData["message"] != null)
                    ViewBag.messageInfo = TempData["message"].ToString();

                // Verification.    
                if (Request.IsAuthenticated)
                {
                    // Info.    
                    //return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                Console.Write(ex);
            }
            // Info.    
            return View();
        }


        /// <summary>  
        /// POST: /Account/Login    
        /// </summary>  
        /// <param name="model">Model parameter</param>  
        /// <param name="returnUrl">Return URL parameter</param>  
        /// <returns>Return login view</returns>  
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                Thread thDelete = new Thread(DeleteFiles);
                thDelete.Start();

                // Verification.    
                if (ModelState.IsValid)
                {
                    // Initialization. 

                    User_Profile loginInfo = db.User_Profile.Include(d => d.Role_Master).Include(d => d.State_Master)
                        .Where(a => a.UserName.Equals(model.UserName)
                        && a.Password.Equals(model.Password) && a.IsDeleted != "Deleted" && a.AccountConfirmed == "Yes").FirstOrDefault();
                    // Verification.    
                    if (loginInfo != null)
                    {
                        ////WebClient webClient = new WebClient();
                        //await Task.Run(() => DeleteFiles());

                        

                        // Login In.    
                        SignInUser(loginInfo.ID, loginInfo.Role_Master.Name, loginInfo.UserName, loginInfo.FirstName,
                            loginInfo.StateEnrolled, loginInfo.State_Master1.Name, loginInfo.State_Master1.ShortName, false);


                        //Thread th = new Thread(getAllExpiredPlanUsers);
                        var checkTodaysRecord = db.ExpiredPlanUserMailLogs.
                                              Where(x => x.RecordOperationDate == DateTime.Today
                                                    && x.ReminderType == "N/A")
                                              .ToList();

                        if (checkTodaysRecord.Count == 0)
                            await Task.Run(() => getAllExpiredPlanUsers()); // Get All Expired Plan Users and Insert in LOG Table.


                        Thread th1 = new Thread(CheckSubscriptionStatusAndDoStuff);
                        th1.Start(); // Send Reminders for all Expired Plan users.

                        //await Task.Run(() => CheckSubscriptionStatusAndDoStuff());
                        ////Thread th2 = new Thread(DeleteUnsubscribedUser());
                        ////th2.Start(loginInfo.UserName);

                        Thread thread = new Thread(() => DeleteUnsubscribedUser(loginInfo.UserName));
                        thread.Start();

                        //await Task.Run(() => DeleteUnsubscribedUser(loginInfo.UserName));

                        //CheckSubscriptionStatusAndDoStuff(userProfile.ID);


                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        // Setting.    
                        ModelState.AddModelError(string.Empty, "Invalid Username/Password or Profile deleted/not confirmed.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info    
                Console.Write(ex);
            }
            // If we got this far, something failed, redisplay form    
            return View(model);
        }
        #endregion


        private void DeleteFiles()
        {
            try
            {
                foreach (string f in Directory.EnumerateFiles(Server.MapPath("~/Reports/"), "TrainingReport_*.pdf"))
                {
                    System.IO.File.Delete(f);
                }

                foreach (string f in Directory.EnumerateFiles(Server.MapPath("~/Reports/"), "Invoice_*.pdf"))
                {
                    System.IO.File.Delete(f);
                }

                foreach (string f in Directory.EnumerateFiles(Server.MapPath("~/Reports/"), "*.jpeg"))
                {
                    System.IO.File.Delete(f);
                }

                foreach (string f in Directory.EnumerateFiles(Server.MapPath("~/Reports/"), "*.jpg"))
                {
                    System.IO.File.Delete(f);
                }

                foreach (string f in Directory.EnumerateFiles(Server.MapPath("~/Reports/"), "*.png"))
                {
                    System.IO.File.Delete(f);
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }


        private void getAllExpiredPlanUsers()
        {
            try
            {
                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    ExpiredPlanUserMailLog expiredPlanUserMailLog = null;
                    DateTime dt = DateTime.Today;

                    var expiredPlanUsers = (from s in db.User_Transaction_Master
                                            where s.End_Date <= dt
                                            select s).ToList();

                    for (int i = 0; i < expiredPlanUsers.Count; i++)
                    {
                        decimal userId = Convert.ToDecimal(expiredPlanUsers[i].User_ID);

                        var user = ctx.User_Transaction_Master.
                                   Where(u => u.User_ID == userId)
                                   .OrderByDescending(u => u.Id).ToList();

                        var checkExisting = ctx.ExpiredPlanUserMailLogs.Where(x => x.UserID == userId).ToList();

                        if (checkExisting.Count == 0)
                        {
                            expiredPlanUserMailLog = new ExpiredPlanUserMailLog();

                            expiredPlanUserMailLog.UserID = user[0].User_ID;
                            expiredPlanUserMailLog.ReminderType = "N/A";
                            expiredPlanUserMailLog.ReminderDate = null;
                            expiredPlanUserMailLog.ReminderStatus = null;

                            DateTime nextDueDate = Convert.ToDateTime(user[0].End_Date).AddMonths(6);
                            expiredPlanUserMailLog.NextReminderDueDate = nextDueDate;
                            expiredPlanUserMailLog.RecordOperationDate = dt;

                            db.ExpiredPlanUserMailLogs.Add(expiredPlanUserMailLog);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception r)
            {
                //throw;
            }
        }


        private void CheckSubscriptionStatusAndDoStuff()
        {
            try
            {
                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    DateTime dt = DateTime.Today;

                    var expiredPlanUsers = (from s in db.ExpiredPlanUserMailLogs
                                            where s.NextReminderDueDate <= dt
                                            select s).ToList();

                    User_Transaction_Master userTransaction = db.User_Transaction_Master.Include(u => u.User_Profile).
                Include(u => u.Rate_Card).Include(u => u.Plan_Master).Where(u => u.Id == expiredPlanUsers[0].UserID).SingleOrDefault();

                    for (int i = 0; i < expiredPlanUsers.Count; i++)
                    {
                        switch (expiredPlanUsers[i].ReminderType)
                        {
                            case "N/A":

                                // Write Code to Sent Reminder One.

                                //________________________________

                                expiredPlanUsers[i].UserID = expiredPlanUsers[i].UserID;
                                expiredPlanUsers[i].ReminderType = "First";
                                expiredPlanUsers[i].ReminderDate = dt;
                                expiredPlanUsers[i].ReminderStatus = "Sent";
                                expiredPlanUsers[i].NextReminderDueDate = Convert.ToDateTime(expiredPlanUsers[i].NextReminderDueDate).AddMonths(5);
                                expiredPlanUsers[i].RecordOperationDate = dt;

                                db.Entry(expiredPlanUsers[i]).State = EntityState.Modified;
                                db.SaveChanges();

                                SendResubMail(userTransaction.User_Profile.FirstName, userTransaction.User_Profile.LastName,
                                    userTransaction.User_Profile.EmailAddress, userTransaction.User_ID, userTransaction.End_Date, "~/EmailTemplate/ResubscriptionMail.html");
                                break;


                            case "First":

                                // Write Code to Sent Reminder Two.

                                //________________________________

                                expiredPlanUsers[i].UserID = expiredPlanUsers[i].UserID;
                                expiredPlanUsers[i].ReminderType = "Second";
                                expiredPlanUsers[i].ReminderDate = dt;
                                expiredPlanUsers[i].ReminderStatus = "Sent";
                                expiredPlanUsers[i].NextReminderDueDate = Convert.ToDateTime(expiredPlanUsers[i].NextReminderDueDate).AddMonths(1).AddDays(-1);
                                expiredPlanUsers[i].RecordOperationDate = dt;

                                db.Entry(expiredPlanUsers[i]).State = EntityState.Modified;
                                db.SaveChanges();

                                SendResubMail(userTransaction.User_Profile.FirstName, userTransaction.User_Profile.LastName,
                                    userTransaction.User_Profile.EmailAddress, userTransaction.User_ID, userTransaction.End_Date, "~/EmailTemplate/ResubscriptionMail.html");
                                break;
                        }
                    }
                }
            }
            catch (Exception r)
            {
                //throw;
            }
        }


        //private void CheckSubscriptionStatusAndDoStuff(decimal userID)
        //{
        //    try
        //    {
        //        using (LaxNarroEntities ctx = new LaxNarroEntities())
        //        {
        //            DateTime dt = DateTime.Today;
        //            string mailResult = string.Empty;

        //            var plan_id = db.Plan_Master.FirstOrDefault(x => x.Plan == "Demo");

        //            decimal planID = Convert.ToDecimal(plan_id.Plan_ID);

        //            var SubscriptionEndDate = (from s in db.User_Transaction_Master
        //                                       where s.End_Date >= dt
        //                                       select s).ToList();

        //            var user = ctx.User_Transaction_Master.Include(s => s.User_Profile).LastOrDefault(s => s.User_Profile.ID == userID);
        //        }
        //    }
        //    catch (Exception r)
        //    {
        //        //throw;
        //    }
        //}


        private string SendResubMail(string firstName, string lastName, string emailID, decimal id,
            DateTime? expirationDate, string emailTemplate)
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
                body = body.Replace("{Email}", emailID);
                
                body = body.Replace("{expirationDate}", Convert.ToString(expirationDate));
                body = body.Replace("{days}", Convert.ToString("has expired on " + expirationDate));
                body = body.Replace("{ExtraInfo}", Convert.ToString(@"Your account will be deleted with all your data after one year of 
                                            subscription end date."));
  
                MailMessage mail = new MailMessage();
                mail.To.Add(emailID);
                mail.From = new MailAddress("mail@lexnarro.com.au");
                mail.Subject = "Lexnarro - Reminder";

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


        private void DeleteUnsubscribedUser(string userName)
        {
            try
            {
                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    DateTime dt = DateTime.Today;
                    var plan_id = db.Plan_Master.FirstOrDefault(x => x.Plan == "Demo");
                    decimal planID = Convert.ToDecimal(plan_id.Plan_ID);
                    var totalUserList = (from s in db.User_Transaction_Master
                                         join c in db.User_Profile on s.User_ID equals c.ID
                                         where s.PlanID == planID && s.End_Date <= dt && c.UserName == userName
                                         && c.IsDeleted != "Deleted"
                                         select s.User_ID).ToList();

                    if (totalUserList.Count > 0)
                    {
                        var user = ctx.User_Profile.FirstOrDefault(x => x.UserName == userName);
                        user.IsDeleted = "Deleted";
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
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
            {
                FinYear = CurYear + "-" + NexYear;
            }
            else
            {
                FinYear = PreYear + "-" + CurYear;
            }

            return FinYear;
        }


        #region Log Out method.    
        /// <summary>  
        /// POST: /Account/LogOff    
        /// </summary>  
        /// <returns>Return log off action</returns> 
        /// 

        [Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                // Setting.    
                Microsoft.Owin.IOwinContext ctx = Request.GetOwinContext();
                IAuthenticationManager authenticationManager = ctx.Authentication;
                // Sign Out.    
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return RedirectToAction("Index", "Account");
        }
        #endregion


        #region Helpers    
        #region Sign In method.    
        /// <summary>  
        /// Sign In User method.    
        /// </summary>  
        /// <param name="username">Username parameter.</param>  
        /// <param name="isPersistent">Is persistent parameter.</param>  
        private void SignInUser(decimal id, string role, string userName, 
            string name, decimal? stateEnrolled, string stateName, string stateShortName, bool isPersistent)
        {
            // Initialization.    
            List<Claim> claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, name));
                claims.Add(new Claim(ClaimTypes.Email, userName));
                claims.Add(new Claim("StateEnrolled", stateName));
                claims.Add(new Claim("StateEnrolledId", Convert.ToString(stateEnrolled)));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, Convert.ToString(id)));
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("StateShortName", stateShortName));

                ClaimsIdentity claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                Microsoft.Owin.IOwinContext ctx = Request.GetOwinContext();
                IAuthenticationManager authenticationManager = ctx.Authentication;
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);                
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }

            //Thread th = new Thread(CarryOver);
            //th.Start();
        }
        #endregion


        #region Redirect to local method.    
        /// <summary>  
        /// Redirect to local method.    
        /// </summary>  
        /// <param name="returnUrl">Return URL parameter.</param>  
        /// <returns>Return redirection action</returns>  
        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.    
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.    
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.

            return RedirectToAction("Index", "Account");
        }
        #endregion
        #endregion


        public ActionResult ErrorPage()
        {
            // Setting.    
            Microsoft.Owin.IOwinContext ctx = Request.GetOwinContext();
            IAuthenticationManager authenticationManager = ctx.Authentication;
            // Sign Out.    
            authenticationManager.SignOut();

            return View();
        }


        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }



        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(User_Profile user_profile)
        {
            try
            {
                using (LaxNarroEntities db = new LaxNarroEntities())
                {
                    var account = db.User_Profile.Where(a => a.EmailAddress == user_profile.EmailAddress).ToList();
                    //var GetUserData = (db.User_Profile.Where(a => a.EmailAddress == email).ToList());
                    string mailResult = "";
                    if (account != null)
                    {
                        mailResult = SendMail(account[0].EmailAddress, "~/EmailTemplate/ForgotPassword.html", account[0].FirstName,
                            account[0].LastName, account[0].Password);
                    }
                    else
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.error, "Invalid email"); 
                    }
                    if (mailResult == "success")
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.success, "Password sent, check your email");
                    }
                }

            }
            catch (Exception)
            {

            }
            return View();
        }



        private string SendMail(string emailAddress, string emailTemplate, string firstName, string lastName, string password)
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
                body = body.Replace("{Password}", password);

                MailMessage mail = new MailMessage();
                mail.To.Add(emailAddress);
                mail.From = new MailAddress("mail@lexnarro.com.au");
                mail.Subject = "Lexnarro - Forgot Password";

                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.lexnarro.com.au";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = 25;
                smtp.UseDefaultCredentials = false;
                //smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("mail@lexnarro.com.au", "lexnarro@123");
                smtp.Send(mail);

                result = "success";
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public ActionResult privacyPolicy()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult TermAndCondition()
        {
            return View();
        }



    }
}