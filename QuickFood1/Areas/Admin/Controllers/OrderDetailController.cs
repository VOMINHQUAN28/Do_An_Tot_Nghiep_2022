using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using QuickFood.Areas.Admin.Models;
using QuickFood.Models.Business;
using QuickFood.Models.EF;

namespace QuickFood.Areas.Admin.Controllers
{
    public class OrderDetailController : Controller
    {
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        // GET: Admin/BookFood
        public ActionResult Index(long order_id)
        {
            var model = db.Order_Detail.Where(x => x.Order_ID == order_id).ToList();
           
            ViewBag.OrderTable =db.Orders.Find(order_id);
            ViewBag.ToppingOrder = db.Topping_Order.Where(x => x.Order_Detail.Order_ID == order_id).ToList();
            return View(model);
        }


    }
}