using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuickFood.Areas.Admin.Models;
using QuickFood.Models.Business;
using QuickFood.Models.EF;

namespace QuickFood.Areas.Admin.Controllers
{
    public class OrderTableController : Controller
    {
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        // GET: Admin/Order
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            var or = new OrderBusiness();
            var model = db.Orders.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pagesize); //Phân trang 

            return View(model);
        }

        public JsonResult ChangeStatus(long ID, int Value)
        {
            var order = db.Orders.Find(ID);
            if(Value == 0)
            {
                order.Status = 1;
                order.ShipDate = DateTime.Now;
            }
            else if(Value == 2)
            {
                order.Status = 2;
                order.PaidDate = DateTime.Now;
            }               
            else if (Value == -2)
            {
                order.Status = -2;
                order.CancerDate = DateTime.Now;
            }

            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }


    }
}