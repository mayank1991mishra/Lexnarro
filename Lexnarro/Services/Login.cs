using System;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;

using System.Data.Entity;
using Lexnarro.Models;

/// <summary>
/// Summary description for Login
/// </summary>
[WebService(Namespace = "http://www.lexnarro.com.au/services/Login.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service acts as a sign in in from app.</b></font>")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1, Name = "TripStatusBinding")]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]

public class Login : System.Web.Services.WebService
{
    SqlConnection con = null;
    Random rand = new Random();
    public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private LaxNarroEntities db = new LaxNarroEntities();

    public Login()
    {
        con = new SqlConnection(ConfigurationManager.ConnectionStrings["LaxNarroConnectionString"].ConnectionString);
    }

    [WebMethod]
    public Data_return UserLogin(string User_Name, string Password, string Device_Imei, string Device_Token, string Device_Type)
    {
        Data_return dr = new Data_return();
        SqlCommand cmd = new SqlCommand();
        try
        {
            if (User_Name != "")
            {
                if (Password != "")
                {
                    //DataTable dt = new DataTable();
                    //SqlDataAdapter adap = new SqlDataAdapter();

                    //string sql = @"SELECT User_Profile.*, State_Master.ShortName, State_Master.Name
                    //                FROM            User_Profile LEFT OUTER JOIN
                    //                State_Master ON User_Profile.StateID = State_Master.ID 
                    //                Where User_Profile.EmailAddress=@User_Name and User_Profile.password=@Password";
                    //cmd.CommandType = CommandType.Text;
                    //cmd.Parameters.AddWithValue("@User_Name", User_Name);
                    //cmd.Parameters.AddWithValue("@Password", Password);
                    //cmd.CommandText = sql;
                    //cmd.Connection = con;
                    //dt.Clear();
                    //adap.SelectCommand = cmd;
                    //if (con.State == System.Data.ConnectionState.Closed)
                    //{
                    //    con.Open();
                    //}
                    //adap.Fill(dt);

                    var userProfile = db.User_Profile.Include(u=>u.Role_Master).Include(u=>u.State_Master1).Where(a => a.UserName == User_Name && a.Password == Password
                        && a.IsDeleted != "Deleted").ToList();

                    if (userProfile.Count > 0)
                    {
                        if (userProfile[0].Role_Master.Name == "Admin")
                        {
                            dr.Status = "FAILURE";
                            dr.Message = "Admin login not allowed";
                            dr.Requestkey = "UserLogin";
                            return dr;
                        }     
                                           
                        if (userProfile[0].AccountConfirmed == "No")
                        {
                            dr.Status = "FAILURE";
                            dr.Message = "Account not activated. Check your email for activation link.";
                            dr.Requestkey = "UserLogin";

                            SendMail(userProfile[0].FirstName, userProfile[0].LastName, userProfile[0].EmailAddress,
                            "https://www.lexnarro.com.au/User/ConfirmAccount/" + userProfile[0].ActivationCode,
                            "~/EmailTemplate/SignupConfirmation.html");
                        }
                        else
                        {

                            cmd.Parameters.Clear();

                            string update = @"update User_Profile set Device_Imei=@Device_Imei, Device_Token=@Device_Token, Device_Type=@Device_Type where EmailAddress=@User_Name AND  Password=@Password";
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Device_Imei", Device_Imei);
                            cmd.Parameters.AddWithValue("@Device_Token", Device_Token);
                            cmd.Parameters.AddWithValue("@Device_Type", Device_Type);
                            cmd.Parameters.AddWithValue("@User_Name", User_Name);
                            cmd.Parameters.AddWithValue("@Password", Password);
                            cmd.CommandText = update;
                            cmd.Connection = con;

                            if (con.State == System.Data.ConnectionState.Closed)
                            {
                                con.Open();
                            }

                            int i = cmd.ExecuteNonQuery();

                            if (i > 0)
                            {
                                UserProfile uf = new UserProfile();
                                uf.ID = userProfile[0].ID;
                                uf.FirstName = userProfile[0].FirstName;
                                uf.LastName = userProfile[0].LastName;
                                uf.OtherName = userProfile[0].OtherName;
                                uf.StreetName = userProfile[0].StreetName;
                                uf.StreetNumber = userProfile[0].StreetNumber;
                                uf.PostCode = userProfile[0].PostCode;
                                uf.Suburb = userProfile[0].Suburb;
                                uf.StateID = userProfile[0].StateID;
                                uf.StateName = userProfile[0].State_Master.Name;
                                uf.CountryID = userProfile[0].CountryID;
                                uf.CountryName = userProfile[0].Country_Master.Name;
                                uf.StateEnrolled = userProfile[0].StateEnrolled;
                                uf.StateEnrolledName = userProfile[0].State_Master1.Name;
                                uf.StateEnrolledShortName = userProfile[0].State_Master1.ShortName;
                                uf.LawSocietyNumber = userProfile[0].LawSocietyNumber;
                                uf.EmailAddress = userProfile[0].EmailAddress;
                                uf.PhoneNumber = userProfile[0].PhoneNumber;
                                uf.Date = userProfile[0].Date;
                                uf.Address = userProfile[0].Address;
                                uf.Device_Imei = userProfile[0].Device_Imei;
                                uf.Device_Token = userProfile[0].Device_Token;
                                uf.Device_Type = userProfile[0].Device_Type;
                                uf.IsDeleted = userProfile[0].IsDeleted;
                                uf.AccountConfirmed = userProfile[0].AccountConfirmed;
                                uf.ActivationCode = userProfile[0].ActivationCode;
                                uf.MailUnsubscribed = userProfile[0].MailUnsubscribed;
                                uf.Firm = userProfile[0].Firm;

                                dr.Profile = uf;
                                dr.Status = "SUCCESS";
                                dr.Message = "Login Success.";
                                dr.Requestkey = "UserLogin";
                                dr.Token = GenerateString(15);
                            }
                            else
                            {
                                dr.Status = "FAILURE";
                                dr.Message = "Server Error";
                                dr.Requestkey = "UserLogin";
                            }
                        }
                    }
                    else
                    {
                        dr.Status = "FAILURE";
                        dr.Message = "User ID is not valid.";
                        dr.Requestkey = "UserLogin";
                    }
                }
                else
                {
                    dr.Status = "FAILURE";
                    dr.Message = "Password is not valid.";
                    dr.Requestkey = "UserLogin";
                }
            }
            else
            {
                dr.Status = "FAILURE";
                dr.Message = "User ID is required.";
                dr.Requestkey = "UserLogin";
            }
        }
        catch (Exception)
        {
            dr.Status = "FAILURE";
            dr.Message = "Something went wrong..";
            dr.Requestkey = "UserLogin";
        }
        return dr;
    }


    private string SendMail(string firstName, string lastName, string emailAddress, string activationLink, string emailTemplate)
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
            mail.From = new MailAddress("mail@lexnarro.com.au");
            mail.Subject = "Lexnarro - Account Information";

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

    public string GenerateString(int size)
    {
        char[] chars = new char[size];
        for (int i = 0; i < size; i++)
        {
            chars[i] = Alphabet[rand.Next(Alphabet.Length)];
        }
        return new string(chars);
    }

    public class Data_return
    {
        public UserProfile Profile { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Requestkey { get; set; }
        public string Token { get; set; }
    }

    public class UserProfile
    {
        public decimal ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public Nullable<decimal> PostCode { get; set; }
        public string Suburb { get; set; }
        public Nullable<decimal> StateID { get; set; }
        public string StateName { get; set; }
        public Nullable<decimal> CountryID { get; set; }
        public string CountryName { get; set; }
        public Nullable<decimal> StateEnrolled { get; set; }
        public string StateEnrolledName { get; set; }
        public string StateEnrolledShortName { get; set; }
        public Nullable<decimal> LawSocietyNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Address { get; set; }
        public string Device_Imei { get; set; }
        public string Device_Token { get; set; }
        public string Device_Type { get; set; }
        public string IsDeleted { get; set; }
        public string AccountConfirmed { get; set; }
        public Nullable<System.Guid> ActivationCode { get; set; }
        public string MailUnsubscribed { get; set; }
        public string Firm { get; set; }
    }
}
