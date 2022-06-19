using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PayPal.Api;
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
        public ActionResult BookTable(Models.EF.Order entity)
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

        /*
            Tài khoản: superjunior25251325-buyer@gmail.com
            Mật khẩu: TestPayPal
         */
        //Thanh toán paypal
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PayPalModel.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Order/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.
            Session[BookFoodSesstion] = null;
            Session["Topping"] = null;
            return View("Success");
        }
        private PayPal.Api.Payment payment;


        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }


        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            var Listcart = (List<OrderFood>)Session[BookFoodSesstion];
            var lstTopping = Session["Topping"] as List<ToppingDTO>;
            var kh = CookiesManage.GetUser();
            decimal tong = 0;
            decimal tongtien = 0;
            int soluong = 0;
            foreach (var item in Listcart)
            {
                var vnd = Convert.ToDecimal((item.quantity * item.food.Price) / 23060);
                var money = Math.Round(Convert.ToDecimal((item.quantity * item.food.Price) / 23060), 2);
                itemList.items.Add(new Item()
                {
                    name = Str_ProductName(item.food.Name),
                    currency = "USD",
                    price = money.ToString(),
                    quantity = item.quantity.ToString(),
                    sku = "sku"
                });

                tong += vnd;
                tongtien += (decimal)(item.quantity * item.food.Price);
                soluong += item.quantity;
            }

            if(lstTopping != null)
            {
                foreach (var item in lstTopping)
                {
                    var vnd = Convert.ToDecimal((item.count * item.Topping.Price) / 23060);
                    var money = Math.Round(Convert.ToDecimal((item.count * item.Topping.Price) / 23060), 2);
                    itemList.items.Add(new Item()
                    {
                        name = Str_ProductName(item.Topping.Name),
                        currency = "USD",
                        price = money.ToString(),
                        quantity = item.count.ToString(),
                        sku = "sku"
                    });

                    tong += vnd;
                    tongtien += (decimal)(item.count * item.Topping.Price);
                    soluong += item.count;
                }
            }



            //Lưu đơn hàng và chi tiết đơn hàng

            new OrderBusiness().AddOrder_Paypal(kh, soluong, tongtien, Listcart, lstTopping);
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = Math.Round(tong, 2).ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }


        public ActionResult FailureView()
        {
            return View();
        }

        public string Str_ProductName(string str)
        {
            string[] VietNamChar = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ:"
            };
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            string str1 = str.ToLower();
            string[] name = str1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string meta = null;
            //Thêm dấu '-'
            foreach (var item in name)
            {
                meta = meta + item + " ";
            }
            return meta;
        }

    }
}