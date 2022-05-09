using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using QuickFood.Models;
using QuickFood.Models.Business;
using QuickFood.Models.EF;

namespace QuickFood.Controllers
{
    public class OrderController : Controller
    {
        private const string BookFoodSesstion = "BookFood";//session 
        // GET: Order
        public ActionResult Index()
        {
            var or = Session[BookFoodSesstion];
            var list = new List<OrderFood>();
            if (or != null)
            {
                list = (List<OrderFood>)or;
            }
            return View(list);
        }


        //Thêm món ăn vào thực đơn
        public JsonResult AddMenu(long foodId, int quantity)
        {
            var food = new FoodBusiness().FindID(foodId);
            //thêm cookei
            var lstfood = new List<Food>();
            lstfood.Insert(0, food);
            new CookiesManage().SetFood_intoCookie(lstfood, true);
            
            var or = Session[BookFoodSesstion];
            if (or != null)
            {
                var list = (List<OrderFood>)or;
                if (list.Exists(x => x.food.ID == foodId))
                {
                    foreach (var item in list)
                    {
                        if (item.food.ID == foodId)
                        {
                            item.quantity += quantity;
                        }
                    }
                }
                else
                {
                    var item = new OrderFood();
                    item.food = food;
                    item.quantity = quantity;
                    list.Add(item);
                }
            }
            else
            {
                var item = new OrderFood();
                var list = new List<OrderFood>();
                item.food = food;
                item.quantity = quantity;
                list.Add(item);
                Session[BookFoodSesstion] = list;
            }
            return Json(new
            {
                status = true
            });
        }

        //Xoá một món ăn trong thực đơn
        public JsonResult Delete(long id)
        {
            var sec = (List<OrderFood>)Session[BookFoodSesstion];
            sec.RemoveAll(x => x.food.ID == id);
            Session[BookFoodSesstion] = sec;
            return Json(new
            {
                status = true
            });
        }

        //Xóa thực đơn
        public JsonResult DeleteAll()
        {
            Session[BookFoodSesstion] = null;
            Session["Topping"] = null;
            return Json(new
            {
                status = true
            });
        }



        //Sửa số lượng món ăn trong thực đơn
        public JsonResult Edit(string EditFood)
        {
            var ed = new JavaScriptSerializer().Deserialize<List<OrderFood>>(EditFood);
            var orSec = (List<OrderFood>)Session[BookFoodSesstion];

            if (ed.Exists(x => x.quantity <= 0))
            {
                return Json(new
                {
                    status = true
                });
            }
            foreach (var item in orSec)
            {
                var foodid = ed.SingleOrDefault(x => x.food.ID == item.food.ID);
                if (foodid != null)
                {
                    item.quantity = foodid.quantity;
                }

            }

            Session[BookFoodSesstion] = orSec;
            return Json(new
            {
                status = true
            });
        }

        //Thêm món ăn vào thực đơn
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        public JsonResult AddTopping(long topping_Id, int quantity)
        {
            var topping = db.Toppings.Find(topping_Id);
            //thêm cookei

            var or = Session["Topping"];
            if (or != null)
            {
                var list = (List<ToppingDTO>)or;
                if (list.Exists(x => x.Topping.ID == topping_Id))
                {
                    foreach (var item in list)
                    {
                        if (item.Topping.ID == topping_Id)
                        {
                            item.count += quantity;
                        }
                    }
                }
                else
                {
                    var item = new ToppingDTO();
                    item.Topping = topping;
                    item.count = quantity;
                    list.Add(item);
                }
            }
            else
            {
                var item = new ToppingDTO();
                var list = new List<ToppingDTO>();
                item.Topping = topping;
                item.count = quantity;
                list.Add(item);
                Session["Topping"] = list;
            }
            return Json(new
            {
                status = true
            });
        }

        //Xoá topping
        public JsonResult DeleteTopping(long id)
        {
            var sec = (List<ToppingDTO>)Session["Topping"];
            sec.RemoveAll(x => x.Topping.ID == id);
            Session["Topping"] = sec;
            return Json(new
            {
                status = true
            });
        }


        //lấy giá trị ngày đặt bàn, giờ đặt bàn, số lượng khách
        public ActionResult PageOrder()
        {
            var or = Session[BookFoodSesstion];
            var FoodOr = new List<OrderFood>();
            if (or != null)
            {
                FoodOr = (List<OrderFood>)or;
            }
            return View(FoodOr);
        }


        //Đặt bàn không chọn thực đơn
        [HttpPost]
        public ActionResult BookTable(Order entity)
        {
            entity.CreatedDate = DateTime.Now;
            try
            {
                var ins = new OrderBusiness();
                ins.Insert(entity);

                if(Session[BookFoodSesstion] != null)
                {
                    var lstFood = Session[BookFoodSesstion] as List<OrderFood>;
                    var lstTopping = Session["Topping"] as List<ToppingDTO>;

                    var bookFood = new Order_Detail();
                    var idOrder = new OrderBusiness().FindIDNew();
                    foreach (var item in lstFood)
                    {
                        bookFood.Food_ID = item.food.ID;
                        bookFood.Count = item.quantity;
                        bookFood.Price = Convert.ToDecimal(item.food.Price);
                        bookFood.Order_ID = idOrder;
                        bookFood.Amount = Convert.ToDecimal(item.food.Price * item.quantity);

                        new OrderBusiness().Add_OrderDetail(bookFood, lstTopping);

                    }

                }
                Session[BookFoodSesstion] = null;
                Session["Topping"] = null;
                return RedirectToAction("Success");
            }
            catch (Exception e)
            {
                TempData["message"] = "Đặt món không thành công. Bạn vui lòng thử lại sau.";
                return Redirect("/dat-mon");
            }
        }


        public ActionResult Success()
        {
            return View();
        }

       
    }
}