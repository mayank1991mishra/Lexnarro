//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lexnarro.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Training_Status
    {
        public decimal Id { get; set; }
        public decimal User_Id { get; set; }
        public string Financial_Year { get; set; }
        public Nullable<decimal> Category_Id { get; set; }
        public Nullable<int> Min_Required_Category_Units { get; set; }
        public Nullable<decimal> Activity_Id { get; set; }
        public Nullable<decimal> SubActivity_Id { get; set; }
        public Nullable<decimal> Units_Done { get; set; }
        public string Received_By_Forwarding { get; set; }
        public decimal Training_Transaction_ID { get; set; }
    
        public virtual Activity_Master Activity_Master { get; set; }
        public virtual Category_Master Category_Master { get; set; }
        public virtual Sub_Activity_Master Sub_Activity_Master { get; set; }
        public virtual User_Profile User_Profile { get; set; }
        public virtual User_Training_Transaction User_Training_Transaction { get; set; }
    }
}
