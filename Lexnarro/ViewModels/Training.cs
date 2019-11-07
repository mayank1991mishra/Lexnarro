using System;

namespace Lexnarro.ViewModels
{
    public class Training
    {
        //public decimal Id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }

        public decimal? Hours { get; set; }

        public string Provider { get; set; }

        public string Financial_Year { get; set; }

        public string Forwardable { get; set; }

        public string Has_been_Forwarded { get; set; }

        public string Descrption { get; set; }

        public string Received_By_Forwarding { get; set; }

        public Nullable<decimal> Units_Done { get; set; }

        public Nullable<decimal> Min_Required_Category_Units { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public Nullable<decimal> PostCode { get; set; }
        public string Suburb { get; set; }
        public Nullable<decimal> LawSocietyNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public string Your_Role { get; set; }

        public string CountryName { get; set; }

        public string StateName { get; set; }

        public string StateShortName { get; set; }

        public string ActivityName { get; set; }

        public string ActivityShortName { get; set; }

        public string CategoryName { get; set; }

        public string CategoryShortName { get; set; }

        public string SubactivityName { get; set; }

        public string SubactivityShortName { get; set; }

        public string Firm { get; set; }

        public string UploadedFileName { get; set; }
    }
}