using System;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for Registration
/// </summary>
[WebService(Namespace = "http://www.lexnarro.com.au/services/Registration.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service acts as a Registration from app.</b></font>")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
//[System.Web.Script.Services.ScriptService]
public class Registration : System.Web.Services.WebService
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LaxNarroConnectionString"].ConnectionString);
    //SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["LaxNarroConnectionString"]);
    Random rand = new Random();
    SqlCommand cmd = new SqlCommand();
    public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string[] specialCharacterList = {"!", "~", "@", "`", "#", "$", "%", "^", "&",
        "*", "(", ")", "_", "-", "|", "\\", "{", "}", "[", "]", ":", ";", "'", "\"", "<", ">", ",", ".", "?", "/"}; 
    public Registration()
    {

    }

    [WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    public Data_return UserRegistration(string FirstName, string LastName, string OtherName,
        string StreetNumber, string StreetName, string PostCode, string Suburb,
        string stateName, string countryName, string StateEnrolledName, string Firm,
        string LawSocietyNumber, string EmailAddress, string PhoneNumber,
        string Password, string Address, string Device_Imei, string Device_Token, string Device_Type)
    {
        Data_return dr = new Data_return();
        SqlTransaction tran = null;
        try
        {
            if (FirstName == "")
            {
                dr.Message = "Please Enter Your First Name";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (LastName == "")
            {
                dr.Message = "Please Enter Your Last Name";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (StreetNumber == "")
            {
                dr.Message = "Please Enter Your Street Number";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (StreetName == "")
            {
                dr.Message = "Please Enter Your Street Name";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (PostCode.ToString() == "")
            {
                dr.Message = "Please Enter Your Post Code";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }
            else
            {
                if (PostCode.ToString().Length <= 0 || PostCode.ToString().Length > 4)
                {
                    dr.Message = "Post Code Should be 4 Digits long";
                    dr.Status = "Failure";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }                

                if (NewClass.ContainsAny(PostCode.ToString(), specialCharacterList))
                {
                    dr.Message = "Invalid Post Code";
                    dr.Status = "Failure";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }
            }

            if (stateName == "")
            {
                dr.Message = "Please Enter Your State";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }
            if (countryName == "")
            {
                dr.Message = "Please Enter Your Country";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (StateEnrolledName == "")
            {
                dr.Message = "Please Enter Your State Enrolled";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (LawSocietyNumber.ToString() == "")
            {
                dr.Message = "Please Enter Your LawSociety Number";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }
            else
            {
                if (LawSocietyNumber.ToString().Length < 5 || LawSocietyNumber.ToString().Length > 10)
                {
                    dr.Message = "LawSociety Number Should be 5 to 10 Digits long";
                    dr.Status = "Failure";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }

                if (NewClass.ContainsAny(LawSocietyNumber.ToString(), specialCharacterList))
                {
                    dr.Message = "Invalid LawSociety Number";
                    dr.Status = "Failure";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }
            }

            if (PhoneNumber.ToString() == "")
            {
                dr.Message = "Please Enter Your Phone Number";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }
            else
            {
                if (PhoneNumber.ToString().Length < 10 || PhoneNumber.ToString().Length > 10)
                {
                    dr.Message = "Phone Number Should be 10 Digits Long And Must Start With Zero(0).";
                    dr.Status = "Failure";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }
                else
                {
                    Regex regex = new Regex(@"^0\d{9}$");
                    Match match = regex.Match(PhoneNumber);
                    if (match.Success)
                    {
                        //dr.Message = "Please Enter Your Phone Number";
                        //dr.Status = "Failure";
                        //dr.Requestkey = "UserRegistration";
                        //return dr;
                    }
                    else
                    {
                        dr.Message = "Phone Number Should be 10 Digits Long And Must Start With Zero(0).";
                        dr.Status = "Failure";
                        dr.Requestkey = "UserRegistration";
                        return dr;
                    }

                }
            }

            if (Password == "")
            {
                dr.Message = "Please Enter Your Password";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (Device_Imei == "")
            {
                dr.Message = "Please Enter Your Device_Imei";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (Device_Token == "")
            {
                dr.Message = "Please Enter Your Device_Token";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (Device_Type == "")
            {
                dr.Message = "Please Enter Your Device_Type";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }

            if (EmailAddress == "")
            {
                dr.Message = "Please Enter Your Email Address";
                dr.Status = "Failure";
                dr.Requestkey = "UserRegistration";
                return dr;
            }
            else
            {
                Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                Match match = regex.Match(EmailAddress);
                if (match.Success)
                {
                    //dr.Message = "Please Enter Your Phone Number";
                    //dr.Status = "Failure";
                    //dr.Requestkey = "UserRegistration";
                    //return dr;
                }
                else
                {
                    dr.Message = "Please Enter Valid Email Address";
                    dr.Status = "Failure";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }


                string[] emailCode = CheckExistingEmail(EmailAddress).Split('-');
                if (emailCode[0] == "RegisteredAndActivated")
                {
                    dr.Message = "Email already registered. Check your email for Id and password.";
                    dr.Status = "Success";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }
                if (emailCode[0] == "RegisteredNotActivated")
                {
                    dr.Message = "Email already registered. Account not activated, Please check your email for activation link.";
                    dr.Status = "Success";
                    dr.Requestkey = "UserRegistration";
                    return dr;
                }
            }

            

            //if (Date == "")
            //{
            //    dr.Message = "Please Enter Your Date";
            //    dr.Status = "Failure";
            //    dr.Requestkey = "UserRegistration";
            //    return dr;
            //}

            //if (Role_id.ToString() == "")
            //{
            //    dr.Message = "Please Enter Your Role_id";
            //    dr.Status = "Failure";
            //    dr.Requestkey = "UserRegistration";
            //    return dr;
            //}

            
            
            decimal countryID = Convert.ToDecimal(findCountry(countryName));
            decimal stateID = Convert.ToDecimal(findState(stateName));
            decimal stateEnrolledID = Convert.ToDecimal(findState(StateEnrolledName));

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            tran = con.BeginTransaction(IsolationLevel.ReadCommitted);
            using (tran)
            {
                cmd.Connection = con;
                cmd.CommandText = "usp_insert";
                cmd.Transaction = tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@FirstName", FirstName);
                cmd.Parameters.AddWithValue("@LastName", LastName);
                cmd.Parameters.AddWithValue("@OtherName", OtherName);
                cmd.Parameters.AddWithValue("@StreetNumber", StreetNumber);
                cmd.Parameters.AddWithValue("@StreetName", StreetName);
                cmd.Parameters.AddWithValue("@PostCode", Convert.ToDecimal(PostCode));
                cmd.Parameters.AddWithValue("@Suburb", Suburb);
                cmd.Parameters.AddWithValue("@StateID", stateID);
                cmd.Parameters.AddWithValue("@CountryID", countryID);
                cmd.Parameters.AddWithValue("@StateEnrolled", stateEnrolledID);
                cmd.Parameters.AddWithValue("@LawSocietyNumber", Convert.ToDecimal(LawSocietyNumber));
                cmd.Parameters.AddWithValue("@EmailAddress", EmailAddress);
                cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                cmd.Parameters.AddWithValue("@UserName", EmailAddress);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@Date", DateTime.Today);
                cmd.Parameters.AddWithValue("@Firm", !string.IsNullOrEmpty(Firm) ? Firm : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Role_id", 1);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.Parameters.AddWithValue("@Device_Imei", Device_Imei);
                cmd.Parameters.AddWithValue("@Device_Token", Device_Token);
                cmd.Parameters.AddWithValue("@Device_Type", Device_Type);
                cmd.Parameters.AddWithValue("@IsDeleted", DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountConfirmed", "No");

                Guid activationCode = Guid.NewGuid();
                cmd.Parameters.AddWithValue("@ActivationCode", activationCode);
                
                cmd.Parameters.Add("@id", SqlDbType.Decimal).Direction = ParameterDirection.Output;

                int r = cmd.ExecuteNonQuery();
                decimal userId = Convert.ToDecimal(cmd.Parameters["@id"].Value);
                if (r > 0)
                {
                    DataTable dst = GetRateCard();

                    if (dst.Rows.Count > 0)
                    {
                        string insertIntoTransactionTable = @"insert into User_Transaction_Master values
                        (@Rate_ID, @User_ID, @Plan_ID, @Amount, @Start_Date, @End_Date, 'N/A',
                         0, 'Active', @Payment_Date, 0, 'N/A', NULL, NULL)";

                        cmd.Parameters.Clear();
                        cmd.Connection = con;
                        cmd.CommandText = insertIntoTransactionTable;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Rate_ID", Convert.ToDecimal(dst.Rows[0]["Rate_Id"]));
                        cmd.Parameters.AddWithValue("@User_ID", userId);
                        cmd.Parameters.AddWithValue("@Plan_ID", Convert.ToDecimal(dst.Rows[0]["Plan_ID"]));
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dst.Rows[0]["Amount"]));
                        cmd.Parameters.AddWithValue("@Start_Date", DateTime.Today);
                        cmd.Parameters.AddWithValue("@End_Date", DateTime.Today.AddMonths(3));
                        cmd.Parameters.AddWithValue("@Payment_Date", Convert.ToDateTime("01/01/1990"));

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            string setMailReminderDates = @"insert into MailLog values
                            (@UserID, @ReminderOne, @ReminderTwo, @ReminderThree, @ReminderFour,
                             @ReminderFive, @ReminderSix, @LastReminderStatus)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@UserID", userId);
                            cmd.Parameters.AddWithValue("@ReminderOne", DateTime.Today.AddDays(2));
                            cmd.Parameters.AddWithValue("@ReminderTwo", DateTime.Today.AddDays(7));
                            cmd.Parameters.AddWithValue("@ReminderThree", DateTime.Today.AddDays(14));
                            cmd.Parameters.AddWithValue("@ReminderFour", DateTime.Today.AddDays(30));
                            cmd.Parameters.AddWithValue("@ReminderFive", DateTime.Today.AddDays(60));
                            cmd.Parameters.AddWithValue("@ReminderSix", DateTime.Today.AddDays(90));
                            cmd.Parameters.AddWithValue("@LastReminderStatus", DBNull.Value);

                            cmd.Connection = con;
                            cmd.CommandText = setMailReminderDates;
                            cmd.CommandType = CommandType.Text;
                            int res = cmd.ExecuteNonQuery();

                            if (res > 0)
                            {
                                tran.Commit();

                                string activationLink = "https://www.lexnarro.com.au/User/ConfirmAccount/" + activationCode;
                                string mailStatus = SendMail(FirstName, LastName, EmailAddress,
                                activationLink, "~/EmailTemplate/SignupConfirmation.html");

                                if (mailStatus == "success")
                                {
                                    dr.Status = "Success";
                                    dr.mailStatus = "Email sent.";
                                    dr.Message = "Registration Successfull, check email for account activation.";
                                    dr.Requestkey = "UserRegistration";
                                    dr.Token = GenerateString(15);
                                }
                                else
                                {
                                    dr.Status = "Success";
                                    dr.mailStatus = "Email sending failed.";
                                    dr.Message = "Registration Successfull.";
                                    dr.Requestkey = "UserRegistration";
                                    dr.Token = GenerateString(15);
                                }
                            }
                            else
                            {
                                tran.Rollback();
                                dr.Status = "Failure";
                                dr.Message = "Registration Failure";
                                dr.Requestkey = "UserRegistration";
                            }
                        }
                        else
                        {
                            tran.Rollback();
                            dr.Status = "Failure";
                            dr.Message = "Registration Failure";
                            dr.Requestkey = "UserRegistration";
                        }
                    }
                    else
                    {
                        tran.Rollback();
                        dr.Status = "Failure";
                        dr.Message = "Registration Failure";
                        dr.Requestkey = "UserRegistration";
                    }
                }

                else
                {
                    tran.Rollback();
                    dr.Status = "Failure";
                    dr.Message = "Registration Failure";
                    dr.Requestkey = "UserRegistration";
                }
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        catch (System.Web.Services.Protocols.SoapException ex)
        {
            tran.Rollback();
            dr.Status = "Failure";
            dr.Message = "Registration Failure" + ex.ToString();
            dr.Requestkey = "UserRegistration";
        }
        catch (Exception ex)
        {
            tran.Rollback();
            dr.Status = "Failure" + ex.ToString();
            dr.Message = "Registration Failure" + ex.ToString();
            dr.Requestkey = "UserRegistration";
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
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

            body = body.Replace("{password}", activationLink);

            MailMessage mail = new MailMessage();
            mail.To.Add(emailAddress);
            mail.From = new MailAddress("mail@lexnarro.com.au", "Lex Narro");
            mail.Subject = "Lex Narro - Verify email to activate account";

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

    private DataTable GetRateCard()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        try
        {
            string getRateCard = @"SELECT Plan_Master.Plan_ID, Plan_Master.[Plan], Rate_Card.Rate_Id, 
            Rate_Card.Status, Rate_Card.Amount FROM Plan_Master INNER JOIN
            Rate_Card ON Plan_Master.Plan_ID = Rate_Card.Plan_Id
            WHERE (Plan_Master.[Plan] = 'Demo') AND (Rate_Card.Status = 'Active')";

            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter();

            cmd.CommandText = getRateCard;
            cmd.CommandType = CommandType.Text;
            sda.SelectCommand = cmd;

            sda.Fill(ds, "RateAndPlanInfo");
        }
        catch (Exception ed)
        {
            return dt;
        }

        return dt = ds.Tables["RateAndPlanInfo"];
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
        internal string mailStatus;

        public string Status { get; set; }
        public string Message { get; set; }
        public string Requestkey { get; set; }
        public string Token { get; set; }
    }

    private string findState(string stateName)
    {
        string stateID = "";
        try
        {
            string sql = @"SELECT ID From State_Master WHERE Name='" + stateName + "'";
            DataTable data = new DataTable();

            data = search_general(sql);
            if (data.Rows.Count > 0)
            {
                stateID = data.Rows[0]["ID"].ToString();
            }
        }
        catch (Exception ex)
        {
            //throw;
        }

        return stateID;
    }

    private string findCountry(string countryName)
    {
        string stateID = "";
        try
        {
            string sql = @"SELECT ID From Country_Master WHERE Name='" + countryName + "'";
            DataTable data = new DataTable();

            data = search_general(sql);
            if (data.Rows.Count > 0)
            {
                stateID = data.Rows[0]["ID"].ToString();
            }
        }
        catch (Exception ex)
        {
            //throw;
        }

        return stateID;
    }

    public DataTable search_general(string sql)
    {
        DataTable dt = null;
        SqlCommand cmd = null;

        try
        {

            cmd = new SqlCommand();
            SqlDataAdapter adap = new SqlDataAdapter();
            DataSet ds = new DataSet();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.Connection = con;
            ds.Clear();
            adap.SelectCommand = cmd;
            adap.Fill(ds, "data");

            dt = ds.Tables["data"];
        }
        catch (Exception e)
        {
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return dt;
    }

    public string CheckExistingEmail(string email)
    {
        DataTable dt = null;
        SqlCommand cmd = null;
        SqlDataAdapter adap = null;
        DataSet ds = null;
        string emailExist = string.Empty;
        try
        {
            cmd = new SqlCommand();
            adap = new SqlDataAdapter();
            ds = new DataSet();
            dt = new DataTable();
            cmd.CommandText = "Select * from User_Profile where EmailAddress = @email and accountconfirmed = 'No'";
            cmd.Parameters.AddWithValue("@email", email);
            cmd.CommandType = CommandType.Text;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.Connection = con;
            ds.Clear();
            adap.SelectCommand = cmd;
            adap.Fill(ds, "data");

            dt = ds.Tables["data"];

            if (dt.Rows.Count > 0)
            {
                emailExist = "RegisteredNotActivated";

                SendMail(dt.Rows[0]["FirstName"].ToString(), dt.Rows[0]["LastName"].ToString(), email,
                    "https://www.lexnarro.com.au/User/ConfirmAccount/" + dt.Rows[0]["ActivationCode"].ToString(),
                    "~/EmailTemplate/SignupConfirmation.html");
            }

            else
            {
                cmd.Dispose();
                adap.Dispose();
                ds.Dispose();
                dt.Dispose();
                cmd = new SqlCommand();
                adap = new SqlDataAdapter();
                ds = new DataSet();
                dt = new DataTable();
                cmd.CommandText = "Select * from User_Profile where EmailAddress = @email and accountconfirmed = 'Yes'";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.CommandType = CommandType.Text;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                cmd.Connection = con;
                ds.Clear();
                adap.SelectCommand = cmd;
                adap.Fill(ds, "data");

                dt = ds.Tables["data"];

                if (dt.Rows.Count > 0)
                {
                    emailExist = "RegisteredAndActivated";

                    SendMail(dt.Rows[0]["FirstName"].ToString(), dt.Rows[0]["LastName"].ToString(), email,
                    dt.Rows[0]["Password"].ToString(), "~/EmailTemplate/RegistrationMail.html");
                }
                else
                {
                    emailExist = "NotRegistered";
                }
            }
        }
        catch (Exception e)
        {
            
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return emailExist;
    }
}

public static class NewClass
{
    public static bool ContainsAny(this string haystack, params string[] needles)
    {
        return needles.Any(haystack.Contains);
    }
}
