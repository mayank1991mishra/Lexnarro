using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lexnarro.ViewModels
{
    public class CarryOverTraining
    {
        public decimal TrainingStatusId { get; set; }
        public decimal TrainingTransactionId { get; set; }
        public decimal User_Id { get; set; }
        public DateTime? Date { get; set; }
        public decimal? State_Id { get; set; }
        public decimal? Category_Id { get; set; }
        public decimal? Activity_Id { get; set; }
        public decimal? SubActivity_Id { get; set; }
        public decimal? Hours { get; set; }
        public string Provider { get; set; }
        public string Financial_Year { get; set; }
        public string Your_Role { get; set; }
        public string Forwardable { get; set; }
        public string Has_been_Forwarded { get; set; }
        public string Descrption { get; set; }
        public byte[] UploadedFile { get; set; }
        public string UploadedFileName { get; set; }
        public string Received_By_Forwarding { get; set; }
        public decimal? Units_Done { get; set; }
        public int? Min_Required_Category_Units { get; set; }
    }
}