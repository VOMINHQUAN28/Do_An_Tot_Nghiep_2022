using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuickFood.Models;
using QuickFood.Models.EF;

namespace QuickFood.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        // GET: Admin/Home
        public ActionResult Index()
        {
            //Tính tổng doanh thu
            TempData["TongDoanhThu"] = db.Orders.Where(n => n.Status == 2).Sum(n => n.TotalMoney);

            //Đếm đơn đặt bàn mới
            TempData["DonDatMoi"] = db.Orders.Where(n => n.Status == 0).Count();

            //Đếm số món ăn
            TempData["MonAn"] = db.Foods.Count();

            //Doanh thu hôm nay
            TempData["DoanhThuHomNay"] = db.Orders.Where(n => n.Status == 2 &&
                                                               DbFunctions.TruncateTime(n.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now))
                                                               .Sum(x => x.TotalMoney);

            //Thống kê lượng món ăn bán chạy
            var lstproduct_id = db.Order_Detail.Select(x => x.Food_ID).Distinct();
            var listProduct_sell = new List<BookFood>();
            foreach (var item in lstproduct_id)
            {
                var food = db.Foods.Find(item);
                var productsell = new BookFood();
                productsell.Food_Name = food.Name;
                productsell.Count = 0;
                productsell.TotalMoney = 0;
                foreach (var jtem in db.Order_Detail.Where(x => x.Food_ID == item && x.Order.Status == 2))
                {
                    productsell.Count += (int)jtem.Count;
                    productsell.TotalMoney += jtem.Price;
                }
                listProduct_sell.Add(productsell);
            }
            ViewBag.product_sell = listProduct_sell.OrderByDescending(x => x.Count).Take(10).ToList();

            

            //Thống kê đơn đạt hàng hôm nay
            ViewBag.Order_today = db.Orders.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Thống kê user đã đăng ký
            ViewBag.user = db.Users.Where(x => x.Type == 0).Count();

            //Đơn đăt đã thanh toán
            ViewBag.OrderPait = db.Orders.Where(x => x.Status == 2).Count();

            //Tổng đơn hàng
            ViewBag.TongDH = db.Orders.Count();


            //Thống kê món ăn đã bán chạy trong tháng
            ViewBag.Month = DateTime.Now.Month;
            var lstsp_id = db.Order_Detail.Where(x => x.Order.CreatedDate.Value.Month == DateTime.Now.Month).Select(x => x.Food_ID).Distinct();
            var listsp_sell = new List<BookFood>();
            foreach (var item in lstsp_id)
            {
                var product = db.Foods.Find(item);
                var productsell = new BookFood();
                productsell.Food_Name = product.Name;
                productsell.Count = 0;
                productsell.TotalMoney = 0;
                foreach (var jtem in db.Order_Detail.Where(x => x.Food_ID == item && x.Order.CreatedDate.Value.Month == DateTime.Now.Month && x.Order.Status == 2))
                {
                    productsell.Count += (int)jtem.Count;
                    productsell.TotalMoney += jtem.Price * jtem.Count;
                }
                listsp_sell.Add(productsell);
            }
            ViewBag.sp_sell_month = listsp_sell.OrderByDescending(x => x.Count).Take(10).ToList();

            //Thống kê Sản phẩm đã bán theo danh mục
            var listSpByNSX_sell = new List<BookFood>();
            foreach (var item in db.Food_Category.ToList())
            {
                var pro = new BookFood();
                pro.Food_Category_Name = item.Name;
                pro.Count = 0;
                pro.TotalMoney = 0;
                foreach (var jtem in db.Order_Detail.Where(x => x.Food.Food_CategoryID == item.ID && x.Order.Status == 2))
                {
                    pro.Count += (int)jtem.Count;
                    pro.TotalMoney += jtem.Price * jtem.Count;
                }
                listSpByNSX_sell.Add(pro);
            }
            ViewBag.sp_sell_nsx = listSpByNSX_sell.OrderByDescending(x => x.Count).Take(10).ToList();
            return View();
        }

        public ActionResult TotalSale_Month()
        {
            int[] month = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var lstTotal = new List<ChartDTO>();
            for (int i = 1; i <= 12; i++)
            {
                var model = (from ct in db.Order_Detail
                             join dh in db.Orders on ct.Order_ID equals dh.ID
                             where dh.CreatedDate.Value.Month == i && dh.Status == 2
                             select new ChartDTO
                             {
                                 tong = dh.TotalMoney
                             }).Sum(x => x.tong);

                var totalsale = new ChartDTO();
                totalsale.thang = i;
                totalsale.tong = model;
              
                lstTotal.Add(totalsale);
            }
            return Json(lstTotal, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TotalSale_Brand()
        {
            var lstTotal = new List<ChartDTO>();
            var lstNSX = db.Food_Category.ToList();
            foreach (var item in lstNSX)
            {
                var model = (from ct in db.Order_Detail
                             join dh in db.Orders on ct.Order_ID equals dh.ID
                             where ct.Food.Food_CategoryID == item.ID && dh.Status == 2
                             select new ChartDTO
                             {
                                 tong = dh.TotalMoney
                             }).Sum(x => x.tong);
                var totalsale = new ChartDTO();
                totalsale.Food_Category_Name = item.Name;
                if (model != null)
                    totalsale.tong = model;
                else
                    totalsale.tong = 0;
                lstTotal.Add(totalsale);
            }
            return Json(lstTotal, JsonRequestBehavior.AllowGet);

        }
    }
}