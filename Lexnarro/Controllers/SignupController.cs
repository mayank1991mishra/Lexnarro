using System;
using System.Linq;
using System.Web.Mvc;
using Lexnarro.Models;
using System.IO;
using System.Net.Mail;
using System.Data.Entity;

namespace Lexnarro.Controllers
{
    public class SignupController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();


        // GET: Signup/Create
        public ActionResult Create()
        {
            ViewBag.CountryID = new SelectList(db.Country_Master.OrderBy(z => z.Name), "ID", "Name");
            ViewBag.Role_id = new SelectList(db.Role_Master.OrderBy(z => z.Name), "Id", "Name");
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(z => z.Name), "ID", "Name");
            ViewBag.StateEnrolled = new SelectList(db.State_Master.OrderBy(z => z.Name), "ID", "Name");
            return View();
        }


        // POST: Signup/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,OtherName,StreetNumber,StreetName,PostCode,Suburb,StateID,CountryID,Address,StateEnrolled,LawSocietyNumber,EmailAddress,PhoneNumber,UserName,Password,Date,Role_id")] User_Profile user_Profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var getName = db.User_Profile.Where(u => u.EmailAddress == user_Profile.EmailAddress).ToList();
                    if (getName.Count > 0)
                        ViewBag.message = @"<script type='text/javascript' language='javascript'>alert('User already registered.')
                                         </script>";
                    else
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            user_Profile.Date = DateTime.Today;
                            user_Profile.Device_Imei = null;
                            user_Profile.Device_Token = null;
                            user_Profile.Device_Type = null;
                            user_Profile.UserName = user_Profile.EmailAddress;
                            user_Profile.Role_id = 1;

                            user_Profile.AccountConfirmed = "No";
                            Guid activationCode = Guid.NewGuid();
                            user_Profile.ActivationCode = activationCode;

                            //----For server----
                            string activationLink = Request.Url.Scheme + "://" + Request.Url.Authority + "/Lexnarro/Signup/ConfirmAccount/" + activationCode;

                            //----For Local----
                            //string activationLink = Request.Url.Scheme + "://" + Request.Url.Authority + "/Signup/ConfirmAccount/" + activationCode;


                            db.User_Profile.Add(user_Profile);
                            db.SaveChanges();

                            decimal id = user_Profile.ID;

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

                            var mailLog = new MailLog();
                            mailLog.UserID = id;
                            mailLog.ReminderOne = DateTime.Today.AddDays(2);
                            mailLog.ReminderTwo = DateTime.Today.AddDays(7);
                            mailLog.ReminderThree = DateTime.Today.AddDays(14);
                            mailLog.ReminderFour = DateTime.Today.AddDays(30);
                            mailLog.ReminderFive = DateTime.Today.AddDays(60);
                            mailLog.ReminderSix = DateTime.Today.AddDays(90);

                            db.MailLogs.Add(mailLog);
                            db.SaveChanges();
                            transaction.Commit();

                            SendMail(user_Profile.FirstName, user_Profile.LastName, user_Profile.EmailAddress,
                                activationLink, "~/EmailTemplate/SignupConfirmation.html");

                            ViewBag.message = @"<script type='text/javascript' language='javascript'>alert(""Registration successful. Check your email for account activation."")
                                         window.location.href = ""~/../../Home/Login"";</script>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = @"<script type='text/javascript' language='javascript'>alert(""Something went Wrong..!!"")</script>";
            }
            ViewBag.CountryID = new SelectList(db.Country_Master.OrderBy(z => z.Name), "ID", "Name", user_Profile.CountryID);
            ViewBag.Role_id = new SelectList(db.Role_Master.OrderBy(z => z.Name), "Id", "Name", user_Profile.Role_id);
            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(z => z.Name), "ID", "Name", user_Profile.StateID);
            ViewBag.StateEnrolled = new SelectList(db.State_Master.OrderBy(z => z.Name), "ID", "Name", user_Profile.StateID);
            return View();
        }

        public JsonResult StateList(int CountryID)
        {
            var state = from s in db.State_Master
                        where s.Country_ID == CountryID
                        select s;
            return Json(new SelectList(state.ToArray(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ConfirmAccount()
        {
            ViewBag.message = @"<script type='text/javascript' language='javascript'>alert(""Invalid activation code"")</script>";
            if (RouteData.Values["RoleID"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["RoleID"].ToString());
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

                        SendMail(userActivation.FirstName, userActivation.LastName,
                            userActivation.EmailAddress, "", "~/EmailTemplate/AccountValidated.html");

                        ViewBag.Status = "Thank you for confirming your account. Now you can Login and start using our services.";
                    }
                }
            }

            return View();
        }


        public ActionResult Unsubscribe()
        {
            if (RouteData.Values["RoleID"] != null)
            {
                decimal id = Convert.ToDecimal(RouteData.Values["RoleID"]);
                User_Profile unsubscribeUser = db.User_Profile.Where(p => p.ID == id).FirstOrDefault();
                if (unsubscribeUser != null)
                {
                    unsubscribeUser.MailUnsubscribed = "Yes";

                    db.Entry(unsubscribeUser).State = EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.Status = unsubscribeUser.EmailAddress + " has been unsubscribed from our e-mailing services.";
                }
            }

            return View();
        }


        private string SendMail(string firstName, string lastName, string emailID, string activationLink, string emailTemplate)
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

                string link = "<br /><a href = '" + activationLink + "'>Click here to activate your account.</a>";

                body = body.Replace("{link}", link);

                //if (days != null && expirationDate != null)
                //{
                //    body = body.Replace("{expirationDate}", Convert.ToString(expirationDate));
                //    if (days > 0)
                //    {
                //        body = body.Replace("{days}", Convert.ToString("will expire in " + days));
                //        body = body.Replace("{ExtraInfo}", Convert.ToString("Your account will be locked after 7 days of expiration date."));
                //    }
                //    else
                //    {
                //        body = body.Replace("{days}", Convert.ToString("has expired on " + days));
                //        body = body.Replace("{ExtraInfo}", Convert.ToString(""));
                //    }
                //}


                MailMessage mail = new MailMessage();
                mail.To.Add(emailID);
                mail.From = new MailAddress("mail@lexnarro.com.au");
                mail.Subject = "Lexnarro - Activate account";

                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.lexnarro.com.au";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = 587;
                //smtp.EnableSsl = true;
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
