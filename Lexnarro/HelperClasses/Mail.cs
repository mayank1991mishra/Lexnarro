using System;
using System.IO;
using System.Net.Mail;
using System.Web;

namespace Lexnarro.HelperClasses
{
    public class Mail
    {
        public Mail()
        {

        }

        public static string SendMail(string firstName, string lastName, string emailID, string activationLink, string emailTemplate)
        {
            string result = "";
            try
            {
                string body = string.Empty;

                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(emailTemplate)))
                    body = reader.ReadToEnd();

                body = body.Replace("{name}", firstName + " " + lastName);
                body = body.Replace("{Email}", emailID);

                string link = "<br /><a href = '" + activationLink + "'>Click here to activate your account.</a>";

                body = body.Replace("{link}", link);
                
                MailMessage mail = new MailMessage();
                mail.To.Add(emailID);
                mail.From = new MailAddress("mail@lexnarro.com.au");
                mail.Subject = "Lexnarro - Activate account";

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
            catch (Exception)
            {
                result = "";
            }

            return result;
        }
    }
}