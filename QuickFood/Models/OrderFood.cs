using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuickFood.Models.EF;

namespace QuickFood.Models
{
    public class OrderFood
    {
        public Food food { get; set; }
        public int quantity { get; set; }
        public List<ToppingDTO> toppings { get; set; }

    }

    public class ToppingDTO
    {
        public Topping Topping { get; set; }
        public int count { get; set; }
    }
}