using System;

namespace Lexnarro.ViewModels
{
    public class CarryOverRecordUserTrainingStatus
    {
        //User_Traning_Status
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
    }
}