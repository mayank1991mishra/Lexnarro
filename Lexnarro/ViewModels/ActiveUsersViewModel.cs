using System;
using System.ComponentModel.DataAnnotations;

namespace Lexnarro.Models
{
    public class ActiveUsers
    {
        [Required(ErrorMessage = "Id is Required Field")]
        public decimal Id { get; set; }

        [Required(ErrorMessage = "User Name is Required Field")]
        public decimal User_ID { get; set; }

        [Required(ErrorMessage = "Start Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public System.DateTime Start_Date { get; set; }

        [Required(ErrorMessage = "End Date is Required Field")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> End_Date { get; set; }

        [Required(ErrorMessage = "Payment Status is Required Field")]
        public string Payment_Status { get; set; }

        [Required(ErrorMessage = "First Name is Required Field")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required Field")]
        public string LastName { get; set; }

        public string OtherName { get; set; }

        public virtual User_Profile User_Profile { get; set; }
    }
}