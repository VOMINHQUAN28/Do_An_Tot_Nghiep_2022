using PagedList;
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
    public class HomeController : Controller
    {
        private const string OrderSesstion = "ordersesstion";
        private const string BookFoodSesstion = "BookFood";
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        // GET: Home
        public ActionResult Index(int page = 1, int pagesize = 18)
        {
            var sv = new FoodBusiness();

            ViewBag.Favorite = db.Favorites.ToList();

            //Danh sách món ăn
            var model = sv.PageListFood().ToPagedList(page, pagesize);

            ViewBag.lstCategory = sv.GetCategories();
            ViewBag.lstFood = db.Foods.ToList();

            return View(model);
        }

        
        //Tìm kiếm món ăn
        public ActionResult Search(string keyword, string type = null, string order = null, int page = 1, int pagesize = 12)
        {
            //string[] key = keyword.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var lstfood = new List<Food>();
            if (type != null && order != null)
            {
                if (type == "name")
                {
                    if (order == "a-to-z")
                    {
                        lstfood = db.Foods.Where(x => x.Name.Contains(keyword)).OrderBy(x => x.Name).ToList();
                    }
                    else
                    {
                        lstfood = db.Foods.Where(x => x.Name.Contains(keyword)).OrderByDescending(x => x.Name).ToList();
                    }
                }
                else
                {
                    if (order == "desc")
                    {
                        lstfood = db.Foods.Where(x => x.Name.Contains(keyword)).OrderByDescending(x => x.Price).ToList();
                    }
                    else
                    {
                        lstfood = db.Foods.Where(x => x.Name.Contains(keyword)).OrderBy(x => x.Price).ToList();
                    }
                }

                ViewBag.Type = type;
                ViewBag.Order = order;
            }
            else
            {
                foreach (var item in db.Foods.ToList())
                {
                    if (item.Name == keyword)
                    {
                        lstfood.Insert(0, item);
                        new CookiesManage().SetFood_intoCookie(lstfood, true);
                    }
                    else if (item.Name.Contains(keyword))
                    {
                        lstfood.Add(item);
                        new CookiesManage().SetFood_intoCookie(lstfood, false);
                    }
                }
                //Thêm cookie
                new CookiesManage().SetFood_intoCookie(lstfood, false);
            }

            ViewBag.Favorite = db.Favorites.ToList();
            ViewBag.KeyWord = keyword;
            return View(lstfood.ToPagedList(page, pagesize));
        }

        //Thuộc tính autocomplete
        public JsonResult ListName(string q)
        {
            var data = new FoodBusiness().searchFood(q);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }


        [ChildActionOnly]
        public PartialViewResult MainMenu()
        {
            ViewBag.menu = db.Main_Menu.Where(x => x.Status == true).OrderBy(x => x.DisplayOrder).ToList();
            ViewBag.Food_Category = db.Food_Category.ToList();
            return PartialView();
        }


        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}