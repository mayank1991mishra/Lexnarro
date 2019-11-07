using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;

/// <summary>
/// Summary description for ForgotPassword
/// </summary>
[WebService(Namespace = "http://www.lexnarro.com.au/services/ForgotPassword.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service acts as a Forgot Password in from app.</b></font>")]

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class ForgotPassword : System.Web.Services.WebService
{
    SqlConnection con = null;
    SqlCommand cmd;
    public ForgotPassword()
    {
        con = new SqlConnection(ConfigurationManager.ConnectionStrings["LaxNarroConnectionString"].ConnectionString);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    public Data_return mailPassword(string emailId)
    {
        SqlDataAdapter adap = new SqlDataAdapter();
        DataTable data = new DataTable();
        Data_return dr = new Data_return();
        try
        {
            if(emailId == null || emailId == "")
            {
                dr.Status = "Failure";
                dr.Message = "Email Id is Required.";
                dr.Requestkey = "mailPassword";
                return dr;
            }

            cmd = new SqlCommand();
            string query = @"SELECT EmailAddress,Password,FirstName,LastName FROM User_Profile WHERE EmailAddress=@email";
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@email", emailId);

            adap.SelectCommand = cmd;
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
            adap.Fill(data);

            if (data.Rows.Count > 0)
            {
                string email = data.Rows[0]["EmailAddress"].ToString();
                string password = data.Rows[0]["Password"].ToString();

                string fname = data.Rows[0]["FirstName"].ToString();
                string lname = data.Rows[0]["LastName"].ToString();

                string subject = "Password Recovery";                
                string body = "Hello "+fname+" "+lname+ ", <br /><br />Your Email Id : '" + emailId + "'<br />Your Password : '" + password + "'<br />Thanks.";

                bool result = SendEmail(email, subject, body);

                if(result==true)
                {
                    dr.Status = "SUCCESS";
                    dr.Message = "Password is sent to the Registered Email Id.";
                    dr.Requestkey = "mailPassword";
                }
                else
                {
                    dr.Status = "FAILURE";
                    dr.Message = "SERVER ERROR";
                    dr.Requestkey = "mailPassword";
                }
            }
            else
            {
                dr.Status = "Failure";
                dr.Message = "Email Id is does not Exist.";
                dr.Requestkey = "mailPassword";
            }
        }

        catch(SoapException ex)
        {
            dr.Status = "FAILURE";
            dr.Message = "SERVER ERROR";
            dr.Requestkey = "mailPassword";
        }
        catch (Exception ex)
        {
            dr.Status = "FAILURE";
            dr.Message = "SERVER ERROR";
            dr.Requestkey = "mailPassword";
        }
        finally
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }
        return dr;
    }

    public class Data_return
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Requestkey { get; set; }
    }

    private bool SendEmail(string recipient, string subject, string body)
    {
        MailMessage mail = new MailMessage();
        mail.To.Add(recipient);
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
        return true;
    }

}
