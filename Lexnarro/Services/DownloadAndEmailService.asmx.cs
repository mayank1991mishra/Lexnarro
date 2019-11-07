using Lexnarro.Models;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Services;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for DownloadAndEmailService
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/DownloadAndEmailService.asmx",
          Description = "<font color='#a31515' size='3'><b>Service to download and email cpd records and payment invoice.</b></font>")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DownloadAndEmailService : System.Web.Services.WebService
    {
        private LaxNarroEntities db = null;

        public DownloadAndEmailService()
        {
            db = new LaxNarroEntities();
        }

        [WebMethod]
        public ReturnData EmailTrainingReport(string finYear, string userEmail, string user_Id, string stateShortName)
        {
            ReturnData rd = new ReturnData();
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    if (finYear == string.Empty || userEmail == string.Empty || user_Id == string.Empty
                        || stateShortName == string.Empty)
                    {
                        rd.Status = "Failure";
                        rd.Message = "Missing Parameters";
                        rd.Requestkey = "SendTrainingReport";
                        return rd;
                    }

                    decimal userId = Convert.ToDecimal(user_Id);
                    string body = string.Empty;

                    mailMessage.From = new MailAddress("mail@lexnarro.com.au", "Lex Narro");
                    mailMessage.To.Add(new MailAddress(userEmail));
                    mailMessage.Subject = "Lex Narro CPD Records for " + finYear;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate/CPDRecords.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    string name = db.User_Profile.Where(x=>x.ID == userId).Select(x=>x.FirstName).First();

                    body = body.Replace("{name}", name);

                    mailMessage.Body = body;

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
                    converter.Options.MarginBottom = 30;
                    converter.Options.MarginRight = 20;
                    converter.Options.MarginTop = 30;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;

                    //Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath

                    //string myurl = Request.Url.Scheme + "://" + Request.Url.Authority
                    //    + "/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    //string documentUrl = "https://www.lexnarro.com.au/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    //string documentUrl = "http://localhost:4031/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    string myurl = "https://lexnarro.com.au/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    PdfDocument doc = converter.ConvertUrl(myurl);
                    doc.Save(Server.MapPath("~/Reports/TrainingReport_" + userId + ".pdf"));
                    doc.Close();

                    mailMessage.Attachments.Add(new Attachment(Server.MapPath("~/Reports/TrainingReport_" + userId + ".pdf")));


                    //// training image Attachment

                    var image = from s in db.User_Training_Transaction
                                where s.User_Id == userId && s.Financial_Year == finYear
                                select new { s.UploadedFile, s.UploadedFileName };

                    /////
                    foreach (var array in image)
                    {
                        if (array.UploadedFile != null)
                        {
                            mailMessage.Attachments.Add(new Attachment(new MemoryStream(array.UploadedFile), array.UploadedFileName));
                        }
                    }

                    mailMessage.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.lexnarro.com.au";
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = 25;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("mail@lexnarro.com.au", "lexnarro@123");
                    smtp.Send(mailMessage);

                    var categories = (new
                    {
                        status = "success"
                    });

                    rd.Status = "Success";
                    rd.Message = "CPD Records Mailed";
                    rd.DocumentUrl = "https://lexnarro.com.au/Reports/TrainingReport_" + userId + ".pdf";
                    rd.Requestkey = "SendTrainingReport";
                    return rd;
                }
            }
            catch (Exception dd)
            {
                rd.Status = "Failure";
                rd.Message = dd.ToString();
                rd.Requestkey = "SendTrainingReport";
                return rd;
            }
        }



        [WebMethod]
        public ReturnData DownloadTrainingReport(string finYear, string userEmail, string user_Id, string stateShortName)
        {
            ReturnData rd = new ReturnData();
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    if (finYear == string.Empty || userEmail == string.Empty || user_Id == string.Empty
                        || stateShortName == string.Empty)
                    {
                        rd.Status = "Failure";
                        rd.Message = "Missing Parameters";
                        rd.Requestkey = "DownloadTrainingReport";
                        return rd;
                    }

                    decimal userId = Convert.ToDecimal(user_Id);

                    //mailMessage.From = new MailAddress("mail@lexnarro.com.au", "Lexnarro");
                    //mailMessage.To.Add(new MailAddress(userEmail));
                    //mailMessage.Subject = "Lexnarro Training Reports";
                    //mailMessage.Body = "<p>CPD Records.</p>";

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
                    converter.Options.MarginBottom = 30;
                    converter.Options.MarginRight = 20;
                    converter.Options.MarginTop = 30;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;

                    //Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath

                    //string myurl = Request.Url.Scheme + "://" + Request.Url.Authority
                    //    + "/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    //string documentUrl = "https://www.lexnarro.com.au/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    //string documentUrl = "http://localhost:4031/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    string myurl = "https://lexnarro.com.au/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    PdfDocument doc = converter.ConvertUrl(myurl);
                    doc.Save(Server.MapPath("~/Reports/TrainingReport_" + userId + ".pdf"));
                    doc.Close();

                    rd.Status = "Success";
                    rd.Message = "Document URL";
                    rd.DocumentUrl = "https://lexnarro.com.au/Reports/TrainingReport_" + userId + ".pdf";
                    rd.Requestkey = "DownloadTrainingReport";
                    return rd;
                }
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong.";
                rd.DocumentUrl = null;
                rd.Requestkey = "DownloadTrainingReport";
                return rd;
            }
        }



        [WebMethod]
        public ReturnData EmailInvoice(string invoiceId, string user_Id)
        {
            ReturnData rd = new ReturnData();
            try
            {
                if (user_Id == string.Empty || invoiceId == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "Missing Parameters";
                    rd.Requestkey = "EmailInvoice";
                    return rd;
                }

                decimal userId = Convert.ToDecimal(user_Id);

                decimal InvoiceId = Convert.ToDecimal(invoiceId);

                using (MailMessage mailMessage = new MailMessage())
                {
                    List<User_Transaction_Master> user_Transaction_Master = db.User_Transaction_Master
                        .Include(u => u.User_Profile)
                        .Where(u => u.User_ID == userId).ToList();

                    mailMessage.From = new MailAddress("mail@lexnarro.com.au", "Lex Narro");
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

                    //string myurl = Request.Url.Scheme + "://"
                    //    + Request.Url.Authority + "/Invoice/Index?v=" + id + "&r=" + recordId;

                    //string documentUrl = "http://localhost:4031/Invoice/Index?v=" + userId + "&r=" + invoiceId;

                    //string documentUrl = "https://lexnarro.com.au/Invoice/Index?v=" + userId + "&r=" + invoiceId;

                    string myurl = "https://lexnarro.com.au/Invoice/Index?v=" + userId + "&r=" + invoiceId;

                    PdfDocument doc = converter.ConvertUrl(myurl);
                    doc.Save(Server.MapPath("~/Reports/Invoice_" + invoiceId + ".pdf"));
                    doc.Close();
                    doc.DetachStream();

                    mailMessage.Attachments.Add(new Attachment(Server.MapPath("~/Reports/Invoice_" + invoiceId + ".pdf")));

                    mailMessage.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();

                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "mail.lexnarro.com.au",
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Port = 25,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("mail@lexnarro.com.au", "lexnarro@123")
                    };
                    smtp.Send(mailMessage);

                    rd.Status = "Success";
                    rd.Message = "Invoice Mailed";
                    rd.DocumentUrl = "https://lexnarro.com.au/Reports/Invoice_" + invoiceId + ".pdf";
                    rd.Requestkey = "EmailInvoice";
                    return rd;
                }
            }
            catch (Exception dd)
            {
                rd.Status = "Failure";
                rd.Message = dd.ToString();
                rd.Requestkey = "EmailInvoice";
                return rd;
            }
        }



        [WebMethod]
        public ReturnData DownloadInvoice(string invoiceId, string user_Id)
        {
            ReturnData rd = new ReturnData();
            try
            {
                if (user_Id == string.Empty || invoiceId == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "Missing Parameters";
                    rd.Requestkey = "DownloadInvoice";
                    return rd;
                }

                decimal userId = Convert.ToDecimal(user_Id);

                decimal InvoiceId = Convert.ToDecimal(invoiceId);

                using (MailMessage mailMessage = new MailMessage())
                {
                    List<User_Transaction_Master> user_Transaction_Master = db.User_Transaction_Master
                        .Include(u => u.User_Profile)
                        .Where(u => u.User_ID == userId).ToList();

                    //mailMessage.From = new MailAddress("mail@lexnarro.com.au", "Lexnarro");
                    //mailMessage.To.Add(new MailAddress(user_Transaction_Master[0].User_Profile.EmailAddress));
                    //mailMessage.Subject = "Lexnarro Payment Invoice";
                    //mailMessage.Body = "<p>Lexnarro Payment Invoice</p>";


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

                    //string myurl = Request.Url.Scheme + "://"
                    //    + Request.Url.Authority + "/Invoice/Index?v=" + id + "&r=" + recordId;

                    //string myurl = "http://localhost:4031/Invoice/Index?v=" + userId + "&r=" + invoiceId;

                    //string myurl = "https://lexnarro.com.au/Invoice/Index?v=" + userId + "&r=" + invoiceId;

                    string myurl = "https://lexnarro.com.au/Invoice/Index?v=" + userId + "&r=" + invoiceId;

                    PdfDocument doc = converter.ConvertUrl(myurl);
                    doc.Save(Server.MapPath("~/Reports/Invoice_" + invoiceId + ".pdf"));
                    doc.Close();

                    //mailMessage.Attachments.Add(new Attachment(Server.MapPath("~/Reports/Invoice_" + userId + ".pdf")));

                    //mailMessage.IsBodyHtml = true;
                    //SmtpClient client = new SmtpClient();

                    //SmtpClient smtp = new SmtpClient
                    //{
                    //    Host = "mail.lexnarro.com.au",
                    //    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //    Port = 25,
                    //    UseDefaultCredentials = false,
                    //    Credentials = new NetworkCredential("mail@lexnarro.com.au", "lexnarro@123")
                    //};
                    //smtp.Send(mailMessage);

                    rd.Status = "Success";
                    rd.Message = "Invoice URL";
                    //rd.DocumentUrl = "http://localhost:4031/Reports/Invoice_" + invoiceId + ".pdf"; 
                    rd.DocumentUrl = "https://lexnarro.com.au/Reports/Invoice_" + invoiceId + ".pdf";
                    rd.Requestkey = "DownloadInvoice";
                    return rd;
                }
            }
            catch (Exception dd)
            {
                rd.Status = "Failure";
                rd.Message = dd.ToString();
                rd.Requestkey = "DownloadInvoice";
                return rd;
            }
        }


        public class ReturnData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public string DocumentUrl { get; set; }
        }
    }
}
