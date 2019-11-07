using Lexnarro.HelperClasses;
using Lexnarro.Models;
using PayPal.Api;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {

        private LaxNarroEntities db = null;
        private readonly ToasterMessage toastNotifications = null;


        public SubscriptionController()
        {
            db = new LaxNarroEntities();
            toastNotifications = new ToasterMessage();
        }


        // GET: Plan_Subscription
        public ActionResult Index()
        {
            List<UserSubscribedPlan> userSubscribedPlanList = new List<UserSubscribedPlan>();

            List<Plan_Master> plan_master = (from s in db.Plan_Master
                                             where s.Plan != "Demo"
                                             select s).ToList();

            decimal userid = Convert.ToDecimal(UserHelper.GetUserId());

            var userSubscribedPlan = (from s in db.User_Transaction_Master
                                      join c in db.Plan_Master on s.PlanID equals c.Plan_ID
                                      where s.User_ID == userid
                                      select new { s.Id, s.Plan_Master.Plan, s.Start_Date, s.End_Date, s.Transaction_Status }).ToList();

            ViewBag.PM = plan_master;

            foreach (var item in userSubscribedPlan)
            {
                userSubscribedPlanList.Add(new UserSubscribedPlan()
                {
                    Id = item.Id,
                    PlanName = item.Plan,
                    StartDate = item.Start_Date,
                    EndDate = item.End_Date.ToString(),
                    TransactionStatus = item.Transaction_Status
                });
            }


            ViewBag.SubscribedPlan = userSubscribedPlanList;

            List<SelectListItem> plans = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = plan_master[0].Plan, Value = Convert.ToString(plan_master[0].Plan_ID)
                }
            };

            ViewBag.Plans = plans;

            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }



        //[ValidateAntiForgeryToken]
        public ActionResult PayNow(decimal planId)
        {
            try
            {
                decimal userId = Convert.ToDecimal(UserHelper.GetUserId());

                IEnumerable<Rate_Card> plan_master = from s in db.Rate_Card join b in db.Plan_Master
                                                       on s.Plan_Id equals b.Plan_ID
                                                       where b.Plan != "Demo"
                                                       select s;

                ViewBag.PM = plan_master;

                string amount = string.Empty;

                string planName = plan_master.Where(x => x.Plan_Id == planId).Select(x => x.Plan_Master.Plan).First();

                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    using (var transaction = ctx.Database.BeginTransaction())
                    {
                        var userTransaction = ctx.User_Transaction_Master.FirstOrDefault(x => x.User_ID == userId);

                        string desc = string.Empty;

                        if (planName == "Yearly")
                        {
                            desc = "Lex Narro Yearly Plan Subscription.";
                            //amount = "13.99";
                            amount = plan_master.Select(x=>x.Amount).FirstOrDefault().ToString();
                        }
                        else
                        {
                            desc = "Lex Narro 2 Yearly Plan Subscription.";
                            //amount = "24.99";
                            amount = plan_master.Select(x => x.Amount).FirstOrDefault().ToString();
                        }

                        var apiContext = GetAPIContext();

                        var payment = new Payment
                        {
                            //For Sandbox testing
                            //experience_profile_id = "XP-3FDL-P42J-GUEG-25ZK",

                            //For live paypal account
                            experience_profile_id = "XP-R4X7-FQ49-PL4T-HLNR",

                            intent = "sale",

                            payer = new Payer
                            {
                                payment_method = "paypal"
                            },
                            transactions = new List<Transaction>
                            {
                                new Transaction
                                {
                                    description = desc,
                                    amount = new Amount
                                    {
                                       currency="AUD",
                                       total = amount,
                                    },
                                    item_list = new ItemList
                                    {
                                        items = new List<Item>()
                                        {
                                            new Item()
                                            {
                                                description = "Lex Narro " + planName + " plan subscription.",
                                                currency = "AUD",
                                                quantity = "1",
                                                price = amount
                                            }
                                        }
                                    }
                                }
                            },
                            redirect_urls = new RedirectUrls
                            {
                                return_url = Url.Action("Success", "Subscription", new { planId = planId }, Request.Url.Scheme),
                                cancel_url = Url.Action("Cancel", "Subscription", new { planId = planId }, Request.Url.Scheme)
                            }
                        };

                        Payment createdPayment = payment.Create(apiContext);

                        var approvalUrl = createdPayment.links
                            .FirstOrDefault(x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase));

                        return Redirect(approvalUrl.href);
                    }
                }
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

                        TempData["message"] = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception rr)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.error, "Something went wrong " + rr.ToString());
            }

            return RedirectToAction("Index");
        }


        public ActionResult Success(decimal planId, string payerId, string paymentId)
        {
            try
            {
                decimal userId = Convert.ToDecimal(UserHelper.GetUserId());

                //-----For sending Invoice mail------
                decimal recordId = 0;

                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    using (var transaction = ctx.Database.BeginTransaction())
                    {
                        int? maxInvoice = ctx.User_Transaction_Master.Max(u => u.Invoice_No);

                        Rate_Card rate_card = ctx.Rate_Card.Include(x => x.Plan_Master)
                            .Where(m => m.Plan_Id == planId && m.Status == "Active").FirstOrDefault();

                        User_Transaction_Master userTransaction = new User_Transaction_Master();

                        IEnumerable<User_Transaction_Master> userAllTransaction = ctx.User_Transaction_Master
                            .Where(x => x.User_ID == userId);

                        // Set the payer for the payment
                        var paymentExecution = new PaymentExecution()
                        {
                            payer_id = payerId
                        };

                        // Identify the payment to execute
                        var payment = new Payment()
                        {
                            id = paymentId
                        };

                        var apiContext = GetAPIContext();

                        // Execute the Payment
                        var executedPayment = payment.Execute(apiContext, paymentExecution);

                        int? max = userAllTransaction.Max(u => (int?)u.Invoice_No);

                        User_Transaction_Master maxedInvoicedTransaction = userAllTransaction
                            .FirstOrDefault(x => x.User_ID == userId && x.Invoice_No == max);

                        int? maxedInvoiceNo = ctx.User_Transaction_Master.Max(u => (int?)u.Invoice_No);

                        if (executedPayment.state == "approved")
                        {
                            //---------For new payment record (second time payment and so on)-------
                            if (maxedInvoicedTransaction.Payment_Status != "N/A")
                            {
                                userTransaction.Amount = Convert.ToDecimal(rate_card.Amount);
                                
                                DateTime? endDate = maxedInvoicedTransaction.End_Date;
                                string strEndDate = string.Empty;
                                if (endDate != null)
                                    strEndDate = endDate.ToString();

                                userTransaction.Start_Date = Convert.ToDateTime(strEndDate);

                                if (rate_card.Plan_Master.Plan == "Yearly")
                                    userTransaction.End_Date = userTransaction.Start_Date.AddYears(1);
                                else
                                    userTransaction.End_Date = userTransaction.Start_Date.AddYears(2);

                                if (maxInvoice == null)
                                    userTransaction.Invoice_No = 1;
                                else
                                    userTransaction.Invoice_No = maxInvoice + 1;

                                userTransaction.Payment_Date = DateTime.Today;

                                userTransaction.Payment_Method = "Online";
                                userTransaction.Payment_Status = "Paid";
                                userTransaction.Paypal_PayerId = payerId;
                                userTransaction.PlanID = rate_card.Plan_Id;
                                userTransaction.Rate_ID = rate_card.Rate_Id;                                

                                userTransaction.Status = "Active";
                                userTransaction.Transaction_Status = executedPayment.state;
                                userTransaction.Transection_ID = executedPayment.id;
                                userTransaction.User_ID = userId;

                                ctx.User_Transaction_Master.Add(userTransaction);
                                ctx.SaveChanges();

                                recordId = userTransaction.Id;
                            }

                            //---------For updating the demo record (First time payment)-------
                            else
                            {
                                maxedInvoicedTransaction.Amount = Convert.ToDecimal(rate_card.Amount);

                                if (rate_card.Plan_Master.Plan == "Yearly")
                                    maxedInvoicedTransaction.End_Date = DateTime.Today.AddYears(1);
                                else
                                    maxedInvoicedTransaction.End_Date = DateTime.Today.AddYears(2);

                                if (maxInvoice == null)
                                    maxedInvoicedTransaction.Invoice_No = 1;
                                else
                                    maxedInvoicedTransaction.Invoice_No = maxInvoice + 1;

                                maxedInvoicedTransaction.Payment_Date = DateTime.Today;

                                maxedInvoicedTransaction.Payment_Method = "Online";
                                maxedInvoicedTransaction.Payment_Status = "Paid";
                                maxedInvoicedTransaction.Paypal_PayerId = payerId;
                                maxedInvoicedTransaction.PlanID = rate_card.Plan_Id;
                                maxedInvoicedTransaction.Rate_ID = rate_card.Rate_Id;

                                //DateTime? endDate = maxedInvoicedTransaction.End_Date;
                                //string strEndDate = string.Empty;
                                //if (endDate != null)
                                //    strEndDate = endDate.ToString();
                                maxedInvoicedTransaction.Start_Date = DateTime.Today;

                                maxedInvoicedTransaction.Status = "Active";
                                maxedInvoicedTransaction.Transaction_Status = executedPayment.state;
                                maxedInvoicedTransaction.Transection_ID = executedPayment.id;
                                maxedInvoicedTransaction.User_ID = userId;

                                ctx.Entry(maxedInvoicedTransaction).State = EntityState.Modified;
                                ctx.SaveChanges();

                                recordId = maxedInvoicedTransaction.Id;
                            }

                            PaypalResponse paypalResponse = new PaypalResponse
                            {
                                user_id = userId,
                                Paypal_ReferenceId = executedPayment.id,
                                intent = executedPayment.intent,
                                payer = Convert.ToString(executedPayment.payer),
                                payee = Convert.ToString(executedPayment.payee),
                                note_to_payer = executedPayment.note_to_payer,
                                update_time = executedPayment.update_time,
                                create_time = executedPayment.create_time,
                                response_state = executedPayment.state,
                                failure_reason = executedPayment.failure_reason
                            };

                            ctx.PaypalResponses.Add(paypalResponse);
                            ctx.SaveChanges();

                            transaction.Commit();

                            MailLog user = ctx.MailLogs.FirstOrDefault(x => x.UserID == userId);
                            user.LastReminderStatus = "Deactive";
                            ctx.SaveChanges();
                        }

                        Thread thread = new Thread(() => SendInvoice(recordId));
                        thread.Start();                        
                        
                        User_Profile userMail = ctx.User_Profile.Where(p => p.ID == userId).FirstOrDefault();

                        Thread thread2 = new Thread(() => SendMail(userMail.FirstName, userMail.LastName, userMail.EmailAddress, "",
                            "Lexnarro - Subscription Successful", "~/EmailTemplate/SubscriptionMail.html"));
                        thread2.Start();

                        //SendMail(userMail.FirstName, userMail.LastName, userMail.EmailAddress, "",
                        //    "Lexnarro - Subscription Successful", "~/EmailTemplate/SubscriptionMail.html");

                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Subscription successful. Thank you.");
                    }
                }
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

                        TempData["message"] = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                    }
                }
            }

            catch (Exception r)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.error, "Something went wrong");
            }
            return RedirectToAction("Index");
        }


        public ActionResult SendInvoice(decimal recordId)
        {
            try
            {
                decimal id = Convert.ToDecimal(UserHelper.GetUserId());
                using (MailMessage mailMessage = new MailMessage())
                {
                    List<User_Transaction_Master> user_Transaction_Master = db.User_Transaction_Master
                        .Include(u => u.User_Profile)
                        .Where(u => u.User_ID == id).ToList();

                    mailMessage.From = new MailAddress("mail@lexnarro.com.au", "Lexnarro");
                    mailMessage.To.Add(new MailAddress(user_Transaction_Master[0].User_Profile.EmailAddress));
                    mailMessage.Subject = "Lexnarro Payment Invoice";
                    mailMessage.Body = "<p>Lexnarro Payment Invoice</p>";


                    HtmlToPdf converter = new HtmlToPdf();
                    string pdf_page_size = "A4";
                    PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                        pdf_page_size, true);
                    string pdf_orientation = "Portrait";
                    PdfPageOrientation pdfOrientation =
                        (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                        pdf_orientation, true);
                    int webPageWidth = 800;
                    int webPageHeight = 0;

                    converter.Options.PdfPageSize = pageSize;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;

                    string myurl = Request.Url.Scheme + "://"
                        + Request.Url.Authority + "/Invoice/Index?v=" + id + "&r=" + recordId;

                    PdfDocument doc = converter.ConvertUrl(myurl);
                    doc.Save(Server.MapPath("~/Reports/Invoice_" + id + ".pdf"));
                    doc.Close();
                    doc.DetachStream();

                    mailMessage.Attachments.Add(new Attachment(Server.MapPath("~/Reports/Invoice_" + id + ".pdf")));

                    mailMessage.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();

                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "mail.lexnarro.com.au",
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Port = 25,
                        //smtp.EnableSsl = true;
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential("mail@lexnarro.com.au", "lexnarro@123")
                    };
                    smtp.Send(mailMessage);

                    //TempData["message"] = ToasterMessage.Message(ToastType.success, "Invoice emailed.");
                    var categories = (new
                    {
                        status = "success"
                    });

                    return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception dd)
            {
                var categories = (new
                {
                    status = dd.ToString()
                });
                return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult PrintInvoice(decimal recordId)
        {
            decimal id = Convert.ToDecimal(UserHelper.GetUserId());
            string myurl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Invoice/Index?v=" + id + "&r=" + recordId;
            Response.Redirect(myurl);
            return RedirectToAction("Index");
        }



        public APIContext GetAPIContext()
        {
            Dictionary<string, string> config = ConfigManager.Instance.GetProperties();
            string accessToken = new OAuthTokenCredential(config).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);

            return apiContext;
        }



        public ActionResult Cancel(string payerId, string paymentId)
        {
            try
            {
                //using (LaxNarroEntities ctx = new LaxNarroEntities())
                //{
                //    using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                //    {
                //        User_Transaction_Master dfd = ctx.User_Transaction_Master.Where(x => x.Transection_ID == paymentId).First();

                //        PaypalResponse pr = ctx.PaypalResponses.Where(x => x.Paypal_ReferenceId == paymentId).First();

                //        ctx.PaypalResponses.Remove(pr);
                //        ctx.SaveChanges();

                //        ctx.User_Transaction_Master.Remove(dfd);
                //        ctx.SaveChanges();

                //        transaction.Commit();

                        TempData["message"] = ToasterMessage.Message(ToastType.info, "Transaction cancelled by user.");
                //    }
                //}
            }
            catch (Exception tt)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.error, "Something went wrong.");
            }
            return RedirectToAction("Index");
        }



        private string SendMail(string firstName, string lastName, string emailID, string activationLink, string subject, string emailTemplate)
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

                MailMessage mail = new MailMessage();
                mail.To.Add(emailID);
                mail.From = new MailAddress("mail@lexnarro.com.au");
                mail.Subject = subject;

                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.lexnarro.com.au";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = 25;
                //smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
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
    }

    public class UserSubscribedPlan
    {
        public decimal Id { get; set; }
        public string PlanName { get; set; }
        public DateTime StartDate { get; set; }
        public string EndDate { get; set; }

        public string TransactionStatus { get; set; }
    }
}