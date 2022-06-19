using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuickFood.Models.EF;

namespace QuickFood.Models
{
    public class BookFood
    {
        public long Food_ID { get; set; }
        public string Food_Name { get; set; }
        public int Count { get; set; }
        public Nullable<decimal> TotalMoney { get; set; }
        public string Price { get; set; }
        public Order Order { get; set; }

        public string Food_Category_Name { get; set; }

    }
}