using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Services;
using System.ComponentModel.DataAnnotations;
using PayPal.Api;
using System.Threading;
using System.Net.Mail;
using SelectPdf;
using System.IO;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for UserTransaction
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/UserTransaction.asmx",
          Description = "<font color='#a31515' size='3'><b>Service to save user transaction data</b></font>")]

    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UserTransaction : System.Web.Services.WebService
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        [WebMethod]
        public ReturnData GetTransactions(string email)
        {
            ReturnData rd = new ReturnData();

            try
            {
                var user_Transaction_Master = (from s in db.User_Transaction_Master.Include(u => u.Rate_Card)
                                          .Include(u => u.User_Profile).Where(u => u.User_Profile.EmailAddress == email)
                                          .Where(s => s.Status == "Active")
                                               select s).ToList();

                if (user_Transaction_Master.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "No Transaction Found";
                    rd.Requestkey = "GetTransactions";
                    return rd;
                }

                List<UserTransactions> transactionList = new List<UserTransactions>();

                for (int i = 0; i < user_Transaction_Master.Count; i++)
                {
                    UserTransactions cm = new UserTransactions();
                    cm.Id = Convert.ToDecimal(user_Transaction_Master[i].Id);
                    cm.FirstName = user_Transaction_Master[i].User_Profile.FirstName;
                    cm.LastName = user_Transaction_Master[i].User_Profile.LastName;
                    cm.Rate_ID = user_Transaction_Master[i].Rate_ID;
                    cm.PlanID = user_Transaction_Master[i].PlanID;
                    cm.PlanName = user_Transaction_Master[i].Plan_Master.Plan;
                    cm.User_ID = user_Transaction_Master[i].User_ID;
                    cm.Amount = user_Transaction_Master[i].Amount;
                    cm.Start_Date = user_Transaction_Master[i].Start_Date;
                    cm.End_Date = user_Transaction_Master[i].End_Date;
                    cm.Payment_Status = user_Transaction_Master[i].Payment_Status;
                    cm.Transection_ID = user_Transaction_Master[i].Transection_ID;
                    cm.Status = user_Transaction_Master[i].Status;
                    cm.Payment_Date = user_Transaction_Master[i].Payment_Date;
                    cm.Invoice_No = user_Transaction_Master[i].Invoice_No;
                    cm.Payment_Method = user_Transaction_Master[i].Payment_Method;
                    transactionList.Add(cm);
                    rd.Transactions = transactionList;
                }

                rd.Status = "Success";
                rd.Message = "Transaction List";
                rd.Requestkey = "GetTransactions";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetTransactions";
                return rd;
            }
        }

        [WebMethod]
        public ReturnData PostTransaction(string email, string Plan_ID, string payerId, string Rate_Id,
            string paymentId, string Amount, string Transection_ID)
        {
            ReturnData rd = new ReturnData();

            if (email == string.Empty || Plan_ID == string.Empty || payerId == string.Empty || paymentId == string.Empty
                || Rate_Id == string.Empty || Amount == string.Empty || Transection_ID == string.Empty)
            {
                rd.Status = "Failure";
                rd.Message = "Missing Parameters";
                rd.Requestkey = "PostTransactions";
                return rd;
            }

            try
            {
                decimal userId = db.User_Profile.Where(x => x.EmailAddress == email).Select(x => x.ID).First();

                //-----For sending Invoice mail------
                decimal recordId = 0;

                decimal planId = Convert.ToDecimal(Plan_ID);

                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    using (var transaction = ctx.Database.BeginTransaction())
                    {
                        int? maxInvoice = ctx.User_Transaction_Master.Max(u => u.Invoice_No);

                        Rate_Card rate_card = ctx.Rate_Card.Include(x => x.Plan_Master)
                            .Where(m => m.Plan_Id == planId && m.Status == "Active" && m.Plan_Master.Plan != "Demo").FirstOrDefault();

                        if (rate_card == null)
                        {
                            rd.Status = "Failure";
                            rd.Message = "Invalid Plan (Demo)";
                            rd.Requestkey = "PostTransactions";
                            return rd;
                        }


                        //if(rate_card.Amount !=Convert.ToDecimal(Amount))
                        //{
                        //    PaypalResponse paypalResponse1 = new PaypalResponse
                        //    {
                        //        user_id = userId,
                        //        Paypal_ReferenceId = paymentId,
                        //        intent = "sale",
                        //        payer = "Android",
                        //        payee = string.Empty,
                        //        note_to_payer = "From Android",
                        //        update_time = Convert.ToString(DateTime.Today),
                        //        create_time = Convert.ToString(DateTime.Today),
                        //        response_state = "created",
                        //        failure_reason = "Invalid Amount"
                        //    };

                        //    ctx.PaypalResponses.Add(paypalResponse1);
                        //    ctx.SaveChanges();

                        //    transaction.Commit();


                        //    rd.Status = "Failure";
                        //    rd.Message = "Invalid Amount";
                        //    rd.Requestkey = "PostTransactions";
                        //    return rd;


                        //}



                        User_Transaction_Master userTransaction = new User_Transaction_Master();

                        List<User_Transaction_Master> userAllTransaction = ctx.User_Transaction_Master
                            .Where(x => x.User_ID == userId).ToList();

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

                        //var apiContext = GetAPIContext();

                        // Execute the Payment
                        //var executedPayment = payment.Execute(apiContext, paymentExecution);

                        int? max = userAllTransaction.Max(u => (int?)u.Invoice_No);

                        User_Transaction_Master maxedInvoicedTransaction = userAllTransaction
                            .FirstOrDefault(x => x.User_ID == userId && x.Invoice_No == max);

                        int? maxedInvoiceNo = ctx.User_Transaction_Master.Max(u => (int?)u.Invoice_No);

                        //if (executedPayment.state == "approved")
                        //{
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
                            userTransaction.Transaction_Status = "approved";
                            userTransaction.Transection_ID = Transection_ID;
                            userTransaction.User_ID = userId;

                            ctx.User_Transaction_Master.Add(userTransaction);
                            ctx.SaveChanges();

                            recordId = userTransaction.Id;

                            rd.Status = "Success";
                            rd.Message = "Transaction Saved";
                            rd.Requestkey = "PostTransactions";
                            //return rd;
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
                            maxedInvoicedTransaction.Transaction_Status = "approved";
                            maxedInvoicedTransaction.Transection_ID = Transection_ID;
                            maxedInvoicedTransaction.User_ID = userId;

                            ctx.Entry(maxedInvoicedTransaction).State = EntityState.Modified;
                            ctx.SaveChanges();

                            recordId = maxedInvoicedTransaction.Id;

                            rd.Status = "Success";
                            rd.Message = "Transaction Saved";
                            rd.Requestkey = "PostTransactions";
                            //return rd;
                        }

                        PaypalResponse paypalResponse = new PaypalResponse
                        {
                            user_id = userId,
                            Paypal_ReferenceId = paymentId,
                            intent = "sale",
                            payer = "Android",
                            payee = string.Empty,
                            note_to_payer = "From Android",
                            update_time = Convert.ToString(DateTime.Today),
                            create_time = Convert.ToString(DateTime.Today),
                            response_state = "approved",
                            failure_reason = string.Empty
                        };

                        ctx.PaypalResponses.Add(paypalResponse);
                        ctx.SaveChanges();

                        MailLog user = ctx.MailLogs.FirstOrDefault(x => x.UserID == userId);
                        if (user != null)
                        {
                            user.LastReminderStatus = "Deactive";
                            ctx.SaveChanges();
                        }


                        transaction.Commit();
                    }

                    Thread thread = new Thread(() => SendInvoice(recordId, userId));
                    thread.Start();

                    User_Profile userMail = ctx.User_Profile.Where(p => p.ID == userId).FirstOrDefault();

                    Thread thread2 = new Thread(() => SendMail(userMail.FirstName, userMail.LastName, userMail.EmailAddress, "",
                        "Lexnarro - Subscription Successful", "~/EmailTemplate/SubscriptionMail.html"));
                    thread2.Start();

                    return rd;
                }
            }

            //catch (DbEntityValidationException e)
            //{
            //    string errorMessage = string.Empty;

            //    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);

            //        foreach (DbValidationError ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //            errorMessage = ve.ErrorMessage;

            //            //TempData["message"] = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
            //        }
            //    }
            //}

            catch (Exception r)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please contact administrator";
                rd.Requestkey = "PostTransactions";
                return rd;
            }
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

                if (activationLink != "")
                {
                    string link = "<br /><a href = '" + activationLink + "'>Click here to activate your account.</a>";

                    body = body.Replace("{link}", link);
                }
                else
                {
                    body = body.Replace("{link}", "");
                }

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


        public void SendInvoice(decimal recordId, decimal userId)
        {
            try
            {
                decimal id = userId;
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

                    string myurl = "https://www.lexnarro.com.au/Invoice/Index?v=" + id + "&r=" + recordId;

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
                    //var categories = (new
                    //{
                    //    status = "success"
                    //});

                    //return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception dd)
            {
                //var categories = (new
                //{
                //    status = dd.ToString()
                //});
                //return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
            }
        }


        public APIContext GetAPIContext()
        {
            Dictionary<string, string> config = ConfigManager.Instance.GetProperties();
            string accessToken = new OAuthTokenCredential(config).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);

            return apiContext;
        }


        public class ReturnData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public List<UserTransactions> Transactions { get; set; }
        }

        public class UserTransactions
        {
            public decimal Id { get; set; }

            [Required(ErrorMessage = "Required Field")]
            public decimal Rate_ID { get; set; }

            [Required(ErrorMessage = "Required Field")]
            public decimal User_ID { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }

            [Required(ErrorMessage = "Required Field")]
            public decimal? PlanID { get; set; }

            public string PlanName { get; set; }

            [Required(ErrorMessage = "Required Field")]
            public decimal Amount { get; set; }

            [Required(ErrorMessage = "Required Field")]
            [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
               ApplyFormatInEditMode = true)]
            public System.DateTime Start_Date { get; set; }

            [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
               ApplyFormatInEditMode = true)]
            public Nullable<DateTime> End_Date { get; set; }

            [Required(ErrorMessage = "Required Field")]
            public string Payment_Status { get; set; }

            public string Transection_ID { get; set; }

            [Required(ErrorMessage = "Required Field")]
            public string Status { get; set; }

            [Required(ErrorMessage = "Required Field")]
            [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
               ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> Payment_Date { get; set; }

            public int? Invoice_No { get; set; }

            [Required(ErrorMessage = "Required Field")]
            public string Payment_Method { get; set; }
        }
    }
}
