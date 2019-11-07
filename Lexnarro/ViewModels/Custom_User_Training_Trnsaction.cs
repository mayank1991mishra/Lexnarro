using System;

namespace Lexnarro.Models
{
    public class Custom_User_Training_Trnsaction
    {
        public decimal User_Id { get; set; }
        
        public decimal State_Id { get; set; }
        
        //public decimal Category_Id { get; set; }
        
        public decimal Activity_Id { get; set; }

        public string Name { get; set; }

        public Nullable<decimal> Min { get; set; }
        
        public Nullable<decimal> Max { get; set; }

        public int TotalRecords { get; set; }
    }
}