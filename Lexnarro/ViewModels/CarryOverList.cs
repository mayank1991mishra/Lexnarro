﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lexnarro.ViewModels
{
    public class CarryOverList
    {
        //User_Traning_Transaction
        //public decimal Id { get; set; }
        public decimal User_Id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> State_Id { get; set; }
        public Nullable<decimal> Category_Id { get; set; }
        public Nullable<decimal> Activity_Id { get; set; }
        public Nullable<decimal> SubActivity_Id { get; set; }
        public string Hours { get; set; }
        public string Provider { get; set; }
        public string Financial_Year { get; set; }
        public string Your_Role { get; set; }
        public string Forwardable { get; set; }
        public string Has_been_Forwarded { get; set; }
        public string Descrption { get; set; }
        public byte[] UploadedFile { get; set; }
        public string UploadedFileName { get; set; }

        //User_Traning_Status
        //public decimal Id { get; set; }
        //public decimal User_Id { get; set; }
        //public string Financial_Year { get; set; }
        //public Nullable<decimal> Category_Id { get; set; }
        public Nullable<int> Min_Required_Category_Units { get; set; }
        //public Nullable<decimal> Activity_Id { get; set; }
        //public Nullable<decimal> SubActivity_Id { get; set; }
        public Nullable<decimal> Units_Done { get; set; }
        public string Received_By_Forwarding { get; set; }
        public decimal Training_Transaction_ID { get; set; }
    }
}