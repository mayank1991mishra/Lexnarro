using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Lexnarro.ViewModels
{
    public partial class UserProfileEditViewModel
    {        
        public decimal ID { get; set; }
        [Required(ErrorMessage = "First Name is Required Field")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required Field")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Street Number is Required Field")]
        public string StreetNumber { get; set; }

        public string OtherName { get; set; }

        [Required(ErrorMessage = "Street Name is Required Field")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "Postal Code is Required Field")]
        [Range(1000, 9999, ErrorMessage = "Postal code must be 4 characters long")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> PostCode { get; set; }

        public string Suburb { get; set; }

        [Required(ErrorMessage = "State Name is Required Field")]
        public Nullable<decimal> StateID { get; set; }

        [Required(ErrorMessage = "State Enrolled is Required Field")]
        public Nullable<decimal> StateEnrolled { get; set; }

        [Required(ErrorMessage = "Law Society Number is Required Field")]
        [Range(10000, 9999999999, ErrorMessage = "Law Society Number Must be between 5 to 10 digits")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> LawSocietyNumber { get; set; }

        [Required(ErrorMessage = "Email is Required Field")]
        [Remote("EmailAlreadyExistsAsync", "UserProfile", HttpMethod = "POST", ErrorMessage = "User with this email already exists")]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Invalid Email")]
        public string EmailAddress { get; set; }

        public decimal Role_id { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Phone Number is Required Field")]
        //[DataType(DataType.PhoneNumber)]
        //[DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        //[Range(1000000000, 9999999999, ErrorMessage = "Phone number must be 10 digits long")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits long and start with zero")]
        public string PhoneNumber { get; set; }

        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
            ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Date { get; set; }

        public string Address { get; set; }

        public string Firm { get; set; }

        public string Password { get; set; }
    }
}