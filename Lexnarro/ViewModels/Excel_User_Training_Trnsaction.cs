using System;
using System.ComponentModel.DataAnnotations;

namespace Lexnarro.Models
{
    public class Excel_User_Training_Trnsaction
    {
        public string Name { get; set; }

        public string State { get; set; }

        public string Category { get; set; }

        public string SubActivity { get; set; }

        public string Activity { get; set; }

        public decimal? Hours { get; set; }

        public string Provider { get; set; }

        [DisplayFormat(DataFormatString = @"{0:MMM\/dd\/yyyy}",
           ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Date { get; set; }

        public string Financial_Year { get; set; }

        public string Your_Role { get; set; }

        public string Forwardable { get; set; }

        public string Has_Been_Forwarded { get;set;}

        public string Description { get; set;}
    }
}