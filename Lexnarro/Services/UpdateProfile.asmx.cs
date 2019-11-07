using System;
using System.Linq;
using Lexnarro.Models;
using System.Data.Entity;
using System.Web.Services;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for UpdateProfile
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/UpdateProfile.asmx",
         Description = "<font color='#a31515' size='3'><b>This web service updates user profile.</b></font>")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UpdateProfile : System.Web.Services.WebService
    {
        private LaxNarroEntities db = new LaxNarroEntities();
        public string[] specialCharacterList = {"!", "~", "@", "`", "#", "$", "%", "^", "&",
        "*", "(", ")", "_", "-", "|", "\\", "{", "}", "[", "]", ":", ";", "'", "\"", "<", ">", ",", ".", "?", "/"};

        [WebMethod]
        public Return UpdateUserProfile(string id, string firstName, string lastName, string otherName, string streetName,
            string postCode, string suburb, string stateId, string countryId, string lawSocietyNumber, string streetNumber,
            string phoneNumber, string Password, string address, string firm)
        {
            Return rd = new Return();

            try
            {
                if (id == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "User id is required";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (true)
                {

                }

                decimal userId = Convert.ToDecimal(id);

                var user = db.User_Profile.Where(u => u.ID == userId).FirstOrDefault();

                if (user == null)
                {
                    rd.Status = "Failure";
                    rd.Message = "User not found";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (firstName != string.Empty)
                {
                    user.FirstName = firstName;
                }
                else
                {
                    rd.Message = "First name is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (lastName != string.Empty)
                {
                    user.LastName = lastName;
                }
                else
                {
                    rd.Message = "Last name is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (otherName != string.Empty)
                {
                    user.OtherName = otherName;
                }

                if (streetName != string.Empty)
                {
                    user.StreetName = streetName;
                }
                else
                {
                    rd.Message = "Street name is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (streetNumber != string.Empty)
                {
                    user.StreetNumber = streetNumber;
                }
                else
                {
                    rd.Message = "Street number is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (postCode != string.Empty)
                {
                    if (postCode.ToString().Length <= 0 || postCode.ToString().Length > 4)
                    {
                        rd.Message = "Invalid Post Code";
                        rd.Status = "Failure";
                        rd.Requestkey = "UpdateUserProfile";
                        return rd;
                    }

                    if (NewClass.ContainsAny(postCode.ToString(), specialCharacterList))
                    {
                        rd.Message = "Invalid Post Code";
                        rd.Status = "Failure";
                        rd.Requestkey = "UpdateUserProfile";
                        return rd;
                    }
                    user.PostCode = Convert.ToDecimal(postCode);
                }
                else
                {
                    rd.Message = "Post code is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (suburb != string.Empty)
                {
                    user.Suburb = suburb;
                }

                if (stateId != string.Empty)
                {
                    user.StateID = Convert.ToDecimal(stateId);
                }
                else
                {
                    rd.Message = "State name is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }
                
                if (countryId != string.Empty)
                {
                    user.CountryID = Convert.ToDecimal(countryId);
                }
                else
                {
                    rd.Message = "Country name is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (lawSocietyNumber != string.Empty)
                {
                    if (lawSocietyNumber.ToString().Length < 5 || lawSocietyNumber.ToString().Length > 10)
                    {
                        rd.Message = "Invalid LawSociety Number";
                        rd.Status = "Failure";
                        rd.Requestkey = "UpdateUserProfile";
                        return rd;
                    }

                    if (NewClass.ContainsAny(lawSocietyNumber.ToString(), specialCharacterList))
                    {
                        rd.Message = "Invalid LawSociety Number";
                        rd.Status = "Failure";
                        rd.Requestkey = "UpdateUserProfile";
                        return rd;
                    }

                    user.LawSocietyNumber = Convert.ToDecimal(lawSocietyNumber);
                }
                else
                {
                    rd.Message = "Law society number is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (phoneNumber != string.Empty)
                {
                    if (phoneNumber.ToString().Length < 10 || phoneNumber.ToString().Length > 10)
                    {
                        rd.Message = "Invalid Phone Number";
                        rd.Status = "Failure";
                        rd.Requestkey = "UpdateUserProfile";
                        return rd;
                    }

                    user.PhoneNumber = phoneNumber;
                }
                else
                {
                    rd.Message = "Phone number is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (Password != string.Empty)
                {
                    user.Password = Password;
                }
                else
                {
                    rd.Message = "Password is required.";
                    rd.Status = "Failure";
                    rd.Requestkey = "UpdateUserProfile";
                    return rd;
                }

                if (address != string.Empty)
                {
                    user.Address = address;
                }

                if (firm != string.Empty)
                {
                    user.Firm = firm;
                }
               
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                var userProfile = db.User_Profile.Where(u => u.ID == userId).FirstOrDefault();

                UserProfile up = new UserProfile();
                up.ID = userProfile.ID;
                up.FirstName = userProfile.FirstName;
                up.LastName = userProfile.LastName;
                up.OtherName = userProfile.OtherName;
                up.StreetNumber = userProfile.StreetNumber;
                up.StreetName = userProfile.StreetName;
                up.PostCode = userProfile.PostCode;
                up.Suburb = userProfile.Suburb;
                up.StateID = userProfile.StateID;
                up.StateName = userProfile.State_Master.Name;
                up.CountryID = userProfile.CountryID;
                up.CountryName = userProfile.Country_Master.Name;
                up.StateEnrolled = userProfile.StateEnrolled;
                up.StateEnrolledName = userProfile.State_Master1.Name;
                up.StateEnrolledShortName = userProfile.State_Master1.ShortName;
                up.LawSocietyNumber = userProfile.LawSocietyNumber;
                up.EmailAddress = userProfile.EmailAddress;
                up.Password = userProfile.Password;
                up.PhoneNumber = userProfile.PhoneNumber;
                up.Date = userProfile.Date;
                up.Address = userProfile.Address;
                up.Device_Imei = userProfile.Device_Imei;
                up.Device_Token = userProfile.Device_Token;
                up.Device_Type = userProfile.Device_Type;
                up.Firm = userProfile.Firm;

                rd.Status = "Success";
                rd.Message = "Profile updated successfully";
                rd.Requestkey = "UpdateUserProfile";
                rd.userProfile = up;
                return rd;
            }
            catch (DbEntityValidationException e)
            {
                string errorMessage = string.Empty;

                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    //    eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        //    ve.PropertyName, ve.ErrorMessage);
                        errorMessage = ve.ErrorMessage;
                        rd.Message = rd.Message + ", " + errorMessage;
                    }
                }

                rd.Status = "Failure";
                rd.Requestkey = "UpdateUserProfile";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "UpdateUserProfile";
                return rd;
            }
        }
    }

    public class Return
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Requestkey { get; set; }
        public UserProfile userProfile { get; set; }
    }

    public class UserProfile
    {
        public decimal ID { get; set; }

        [Required(ErrorMessage = "First name is required field")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required field")]
        public string LastName { get; set; }
        public string OtherName { get; set; }

        [Required(ErrorMessage = "Street number is required field")]
        public string StreetNumber { get; set; }

        [Required(ErrorMessage = "Street name is required field")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "post code is required field")]
        [Range(1000, 9999, ErrorMessage = "Postal code must be 4 characters long")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> PostCode { get; set; }
        public string Suburb { get; set; }

        [Required(ErrorMessage = "State name is required field")]
        public Nullable<decimal> StateID { get; set; }

        [Required(ErrorMessage = "State name is required field")]
        public string StateName { get; set; }

        [Required(ErrorMessage = "Country name is required field")]
        public Nullable<decimal> CountryID { get; set; }

        [Required(ErrorMessage = "Country name is required field")]
        public string CountryName { get; set; }

        [Required(ErrorMessage = "State name is required field")]
        public Nullable<decimal> StateEnrolled { get; set; }

        [Required(ErrorMessage = "State name is required field")]
        public string StateEnrolledName { get; set; }

        [Required(ErrorMessage = "State name is required field")]
        public string StateEnrolledShortName { get; set; }

        [Required(ErrorMessage = "Law society number is required field")]
        [Range(10000, 9999999999, ErrorMessage = "Must be between 5 to 10 digits long")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> LawSocietyNumber { get; set; }

        [Required(ErrorMessage = "Email is required field")]
        //[Remote("EmailAlreadyExistsAsync", "User_Profile", HttpMethod = "POST", ErrorMessage = "User with this Email already exists")]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Invalid Email")]
        public string EmailAddress { get; set; }


        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required field")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits long and start with zero(0).")]
        public string PhoneNumber { get; set; }

        public Nullable<System.DateTime> Date { get; set; }
        public string Address { get; set; }
        public string Device_Imei { get; set; }
        public string Device_Token { get; set; }
        public string Device_Type { get; set; }
        public string Firm { get; set; }
    }

    public static class NewClass
    {
        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            return needles.Any(haystack.Contains);
        }
    }
}
