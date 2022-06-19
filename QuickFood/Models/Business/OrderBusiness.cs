using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuickFood.Models.EF;

namespace QuickFood.Models.Business
{
    public class OrderBusiness
    {
        Order_Restaurant_Db db = new Order_Restaurant_Db();

        //Lưu thông tin khách hàng đặt bàn
        public bool Insert(Order or)
        {
            try
            {
                or.CreatedDate = DateTime.Now;
                or.Status = 0;
                db.Orders.Add(or);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }


        //Lấy ID vừa mới thêm vào csdl
        public long FindIDNew()
        {
            return db.Orders.Max(x => x.ID);
        }

        public IEnumerable<Order> ListAll()
        {
            return db.Orders.OrderByDescending(x => x.CreatedDate).ToList();
        }

        public Order FindID(long id)
        {
            return db.Orders.Find(id);
        }

        public bool EditOrder(Order or)
        {
            try
            {
                var order = db.Orders.Find(or.ID);
                order.Full_Name = or.Full_Name;
                order.Phone = or.Phone;
                order.Note = or.Note;
                order.Address = or.Address;
               
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void Add_OrderDetail(Order_Detail detail, List<ToppingDTO> entity)
        {
            db.Order_Detail.Add(detail);
            db.SaveChanges();

            if(entity != null)
            {
                var OrderDetail_ID = db.Order_Detail.Max(x => x.ID);
                foreach (var item in entity.Where(x => x.Topping.Food_ID == detail.Food_ID))
                {
                    var topping = new Topping_Order();
                    topping.Topping_ID = item.Topping.ID;
                    topping.OrderDetail_ID = OrderDetail_ID;
                    topping.Quantity = item.count;
                    topping.Price = item.count * item.Topping.Price;

                    db.Topping_Order.Add(topping);
                    db.SaveChanges();
                }
            }
            
        }

        public void AddOrder_Paypal(User user, int TotalQuantity ,decimal TotalMoney, List<OrderFood> orderFoods, List<ToppingDTO> toppingDTOs)
        {
            var or = new Order();
            or.Full_Name = user.Full_Name;
            or.Phone = user.Phone;
            or.Address = user.Address;
            or.PaidDate = DateTime.Now;
            or.User_ID = user.ID;
            or.CreatedDate = DateTime.Now;
            or.TotalQuantity = TotalQuantity;
            or.TotalMoney = TotalMoney;
            or.Status = 2;
            db.Orders.Add(or);
            db.SaveChanges();

            var bookFood = new Order_Detail();
            var idOrder = new OrderBusiness().FindIDNew();
            foreach (var item in orderFoods)
            {
                bookFood.Food_ID = item.food.ID;
                bookFood.Count = item.quantity;
                bookFood.Price = Convert.ToDecimal(item.food.Price);
                bookFood.Order_ID = idOrder;
                bookFood.Amount = Convert.ToDecimal(item.food.Price * item.quantity);

                new OrderBusiness().Add_OrderDetail(bookFood, toppingDTOs);

            }
        }

        //public bool DeleteOrder(long id)
        //{
        //    try
        //    {
        //        var or = db.OrderTables.Find(id);
        //        db.OrderTables.Remove(or);
        //        db.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}

        //public OrderTable Find_FullName(long order_id)
        //{
        //    return db.OrderTables.SingleOrDefault(x => x.ID == order_id);
        //}
    }
}