using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Lexnarro.Models
{
    public class Activity_Master_Metadata
    {
        [Required(ErrorMessage = "Activity is Required Field")]
        public string Name { get; set; }
    }

    public class Category_Master_Metadata
    {
        [Required(ErrorMessage = "Category is Required Field")]
        public string Name { get; set; }
    }

    public class Country_Master_Metadata
    {
        [Required(ErrorMessage = "Country Name is Required Field")]
        [RegularExpression("[a-zA-Z][a-zA-Z ]+[a-zA-Z]$", ErrorMessage = "Invalid Name")]
        public string Name { get; set; }
    }

    public class Page_Master_Metadata
    {
        [Required(ErrorMessage = "Required Field.")]
        public string page_name { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string page_group { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string menu_name { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string status { get; set; }
    }

    public class Rate_Card_Metadata
    {
        [Required(ErrorMessage = "Plan Name is Required Field")]
        public decimal Plan_Id { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        //[Required(ErrorMessage = "Required Field.")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
            ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Amount is Required Field")]
        public Nullable<decimal> Amount { get; set; }

        [Required(ErrorMessage = "Status is Required Field")]
        public string Status { get; set; }
    }

    public class Role_Master_Metadata
    {
        [Required(ErrorMessage = "Role Name is Required Field")]
        public string Name { get; set; }
    }

    public class Role_Page_Map_Metadata
    {
        [Required(ErrorMessage = "Role Id is Required Field")]
        public decimal role_id { get; set; }

        [Required(ErrorMessage = "Page Id is Required Field")]
        public decimal page_id { get; set; }

        [Required(ErrorMessage = "Status is Required Field")]
        public string status { get; set; }
    }

    public class Rule1_Master_Metadata
    {
        [Required(ErrorMessage = "Rule Name is Required Field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Minimum Value is Required Field")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Min { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Max { get; set; }
    }

    public class Rule2_Master_Metadata
    {
        [Required(ErrorMessage = "Rule Name is Required Field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Unit is Required Field")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Unit { get; set; }

        [Required(ErrorMessage = "Hours is Required Field")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public Nullable<decimal> Hours { get; set; }
    }

    public class State_Master_Metadata
    {
        [Required(ErrorMessage = "State Name is Required Field")]
        [RegularExpression("[a-zA-Z][a-zA-Z ]+[a-zA-Z]$", ErrorMessage = "Invalid Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Country is Required Field")]
        public decimal Country_ID { get; set; }
    }

    public class Sub_Activity_Master_Metadata
    {
        [Required(ErrorMessage = "Sub-activity is Required Field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Activity is Required Field")]
        public decimal Activity_ID { get; set; }

        [Required(ErrorMessage = "State Name is Required Field")]
        public decimal StateID { get; set; }
    }

    public class Total_Unit_Master_Metadata
    {
        [Required(ErrorMessage = "State Name is Required Field")]
        public decimal StateID { get; set; }

        [Required(ErrorMessage = "Total Units is Required Field")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal Total_Units { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
            ApplyFormatInEditMode = true)]
        public DateTime Start_Date { get; set; }


        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
            ApplyFormatInEditMode = true)]
        public DateTime End_Date { get; set; }
    }

    public class User_Profile_Metadata
    {
        [Required(ErrorMessage = "First Name is Required Field")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required Field")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Street Number is Required Field")]
        public string StreetNumber { get; set; }

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

        [Required(ErrorMessage = "Country Name is Required Field")]
        public Nullable<decimal> CountryID { get; set; }

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

        [Required(ErrorMessage = "Phone Number is Required Field")]
        //[DataType(DataType.PhoneNumber)]
        //[DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        //[Range(1000000000, 9999999999, ErrorMessage = "Phone number must be 10 digits long")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage ="Phone number must be 10 digits long and start with zero")]
        public string PhoneNumber { get; set; }

        //[Required(ErrorMessage = "Required Field")]
        //[Remote("UserAlreadyExistsAsync", "User_Profile", HttpMethod = "POST", ErrorMessage = "User with this UserName already exists")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required Field")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
            ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Date { get; set; }

        [Required(ErrorMessage = "Required Field")]
        public decimal Role_id { get; set; }
    }

    public class User_Role_Mapping_Metadata
    {
        [Required(ErrorMessage = "Required Field.")]
        public decimal user_ID { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public decimal role_id { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string login_id { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string password { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string confirm_password { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string description { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string status { get; set; }
    }

    public class User_Subscription_Metadata
    {
        [Required(ErrorMessage = "Required Field.")]
        public Nullable<decimal> UserID { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public Nullable<decimal> RateID { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public Nullable<decimal> Amount { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
            ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartDate { get; set; }

        //[Required(ErrorMessage = "Required Field.")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
            ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string PaymentStatus { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string TransectionID { get; set; }

        [Required(ErrorMessage = "Required Field.")]
        public string Status { get; set; }
    }


    public class Plan_Master_Metadata
    {
        [Required(ErrorMessage = "Plan Name is Required Field")]
        public string Plan { get; set; }
    }

    public class StateActivitySubActivityWithRule1_Metadata
    {
        [Required(ErrorMessage = "State Name is Required Field")]
        public decimal StateID { get; set; }

        [Required(ErrorMessage = "Activity is Required Field")]
        public decimal ActivityID { get; set; }

        //[Required(ErrorMessage = "Required Field")]
        public Nullable<decimal> SubActivityID { get; set; }

        [Required(ErrorMessage = "Rule Name is Required Field")]
        public decimal Rule1ID { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
          ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }

        //[Required(ErrorMessage = "Required Field.")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<DateTime> EndDate { get; set; }

        [Required(ErrorMessage = "Status is Required Field")]
        public string Status { get; set; }
    }

    public class StateActivity__with_Rule2_Metadata
    {
        [Required(ErrorMessage = "State Name is Required Field")]
        public decimal StateID { get; set; }

        [Required(ErrorMessage = "Activity is Required Field")]
        public decimal ActivityID { get; set; }

        [Required(ErrorMessage = "Rule Name is Required Field")]
        public decimal Rule2_ID { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
          ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }

        //[Required(ErrorMessage = "Required Field.")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<DateTime> EndDate { get; set; }

        [Required(ErrorMessage = "Status is Required Field")]
        public string Status { get; set; }
    }

    public class User_Transaction_Master_Metadata
    {
        [Required(ErrorMessage = "Rate is Required Field")]
        public decimal Rate_ID { get; set; }

        [Required(ErrorMessage = "User Name is Required Field")]
        public decimal User_ID { get; set; }

        [Required(ErrorMessage = "Plan Name is Required Field")]
        public string PlanID { get; set; }

        [Required(ErrorMessage = "Amounr is Required Field")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public System.DateTime Start_Date { get; set; }

        //[Required(ErrorMessage = "Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<DateTime> End_Date { get; set; }

        [Required(ErrorMessage = "Payment Status is Required Field")]
        public string Payment_Status { get; set; }

        //[Required(ErrorMessage = "Required Field")]
        //[DataType(DataType.PhoneNumber)]
        //[DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public string Transection_ID { get; set; }

        [Required(ErrorMessage = "Status is Required Field")]
        public string Status { get; set; }

        //[Required(ErrorMessage = "Payment Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Payment_Date { get; set; }

        //[Required(ErrorMessage = "Required Field")]
        //[DataType(DataType.PhoneNumber)]
        //[DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public Nullable<int> Invoice_No { get; set; }

        [Required(ErrorMessage = "Payment Method is Required Field")]
        public string Payment_Method { get; set; }
    }

    public class State_Rule3_Marriage_Metadata
    {
        [Required(ErrorMessage = "State Name is Required Field")]
        public decimal StateID { get; set; }

        [Required(ErrorMessage = "Rule Name is Required Field")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal Rule3_ID { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartDate { get; set; }

        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Required(ErrorMessage = "Status is Required Field")]
        public string Status { get; set; }

        //public virtual Rule3_Master Rule3_Master { get; set; }
        //public virtual State_Master State_Master { get; set; }
    }

    public class Rule3_Master_Metadata
    {
        [Required(ErrorMessage = "Carry Over Units is Required Field.")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public string CarryOverUnits { get; set; }
    }


    public class User_Training_Transaction_Metadata
    {
        [Required(ErrorMessage = "User Name Required Field")]
        public decimal User_Id { get; set; }

        [Required(ErrorMessage = "Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
          ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Date { get; set; }

        [Required(ErrorMessage = "State Name is Required Field")]
        public Nullable<decimal> State_Id { get; set; }


        [Required(ErrorMessage = "Category is Required Field")]
        public Nullable<decimal> Category_Id { get; set; }


        [Required(ErrorMessage = "Activity is Required Field")]
        public Nullable<decimal> Activity_Id { get; set; }
        

        [Required(ErrorMessage = "Hours is Required Field")]
        public decimal? Hours { get; set; }

        [Required(ErrorMessage = "Provider is Required Field")]
        public string Provider { get; set; }


        [Required(ErrorMessage = "Financial Year is Required Field")]
        public string Financial_Year { get; set; }
       
        public string Your_Role { get; set; }

        public string Forwardable { get; set; }

        public string Has_been_Forwarded { get; set; }

        //[Column(TypeName = "varchar(MAX)")]

        //public string Descrption { get; set; }
    }
    public class Rule4_Master_Metadata
    {
        [Required(ErrorMessage = "Name is Required Field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Minimum Units is Required Field")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public int MinUnits { get; set; }
    }

    public class State_Category_With_Rule4_Mapping_Meatdata
    {
        [Required(ErrorMessage = "State Name is Required Field")]
        public decimal StateID { get; set; }

        [Required(ErrorMessage = "Category is Required Field")]
        public decimal CategoryID { get; set; }

        [Required(ErrorMessage = "Rule Name is Required Field")]
        public decimal Rule4_ID { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field.")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Required(ErrorMessage = "Status is Required Field")]
        public string Status { get; set; }
    }

    public partial class State_Activity_Mapping_Metadata
    {
        [Required(ErrorMessage = "State is Required Field")]
        public decimal StateID { get; set; }

        [Required(ErrorMessage = "Activity is Required Field")]
        public decimal ActivityID { get; set; }
    }

}