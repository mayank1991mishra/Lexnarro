using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lexnarro.ViewModels
{
    public class Categories
    {
        public decimal? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalUnits { get; set; }
    }
}