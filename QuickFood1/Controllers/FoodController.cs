using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuickFood.Models.Business;
using QuickFood.Models.EF;
using System.Web.Script.Serialization;
using QuickFood.Models;

namespace QuickFood.Controllers
{
    public class FoodController : Controller
    {
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        // GET: Food
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(long id)
        {
            var food = db.Foods.Find(id);
            ViewBag.foodDetail = food;

            //Topping
            ViewBag.Topping = db.Toppings.Where(x => x.Food_ID == id).ToList();

            //thêm cookei
            var lstfood = new List<Food>();
            lstfood.Insert(0, food);
            new CookiesManage().SetFood_intoCookie(lstfood, true);

            //Món ăn cùng danh mục
            ViewBag.lstFoodSameCategory = db.Foods.Where(x => x.Food_CategoryID == food.Food_CategoryID).ToList();

            ViewBag.lstCmt = db.Comments.Where(x => x.Food_ID == id).OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.lstReply = db.ReplyCmts.OrderByDescending(x => x.CreatedDate).ToList();

            //Lưu món ăn đã xem
            if (Session["RecentFood"] != null)
            {
                var recentnews = Session["RecentFood"] as List<Food>;
                if (!recentnews.Exists(x => x.ID == id))
                {
                    recentnews.Add(food);
                    Session["RecentFood"] = recentnews;
                }
            }
            else
            {
                var recentnews = new List<Food>();
                recentnews.Add(food);
                Session["RecentFood"] = recentnews;
            }
            bool check = false;
            if (Session["BookFood"] != null)
            {
                var list = Session["BookFood"] as List<OrderFood>;
                if (list.Exists(x => x.food.ID == id))
                    check = true;
            }
            ViewBag.Check = check;

            
            return View();

        }

        public JsonResult AddFavorite(long User_ID, long Food_ID, bool isLike)
        {
            if (isLike)
            {
                //thêm cookei
                var lstfood = new List<Food>();
                var food = db.Foods.Find(Food_ID);
                lstfood.Insert(0, food);
                new CookiesManage().SetFood_intoCookie(lstfood, true);

                var fav = new Favorite();
                fav.Food_ID = Food_ID;
                fav.User_ID = User_ID;
                db.Favorites.Add(fav);
            }
            else
            {
                var fav = db.Favorites.FirstOrDefault(x => x.User_ID == User_ID && x.Food_ID == Food_ID);
                db.Favorites.Remove(fav);
            }

            db.SaveChanges();
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Menu(string type = null, string order = null, int page = 1, int pagesize = 18)
        {
            if (type != null && order != null)
            {
                var food = new List<Food>();
                if (type == "name")
                {
                    if (order == "a-to-z")
                    {
                        food = db.Foods.OrderBy(x => x.Name).ToList();
                    }
                    else
                    {
                        food = db.Foods.OrderByDescending(x => x.Name).ToList();
                    }
                }
                else
                {
                    if (order == "desc")
                    {
                        food = db.Foods.OrderByDescending(x => x.Price).ToList();
                    }
                    else
                    {
                        food = db.Foods.OrderBy(x => x.Price).ToList();
                    }
                }

                ViewBag.Type = type;
                ViewBag.Order = order;
                ViewBag.Favorite = db.Favorites.ToList();
                return View(food.ToPagedList(page, pagesize));
            }
            else
            {
                var model = db.Foods.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pagesize);
                ViewBag.Favorite = db.Favorites.ToList();
                return View(model);
            }
            
        }

        public ActionResult FoodCategory(string Link, long ID, string type = null, string order = null, int page = 1, int pagesize = 12)
        {
            ViewBag.FoodCategory = db.Food_Category.Find(ID);
            
            if (type != null && order != null)
            {
                var food = new List<Food>();
                if(type == "name")
                {
                    if(order == "a-to-z")
                    {
                        food = db.Foods.Where(x => x.Food_CategoryID == ID).OrderBy(x => x.Name).ToList();
                    }
                    else
                    {
                        food = db.Foods.Where(x => x.Food_CategoryID == ID).OrderByDescending(x => x.Name).ToList();
                    }
                }
                else
                {
                    if (order == "desc")
                    {
                        food = db.Foods.Where(x => x.Food_CategoryID == ID).OrderByDescending(x => x.Price).ToList();
                    }
                    else
                    {
                        food = db.Foods.Where(x => x.Food_CategoryID == ID).OrderBy(x => x.Price).ToList();
                    }
                }

                ViewBag.Type = type;
                ViewBag.Order = order;
                ViewBag.Favorite = db.Favorites.ToList();
                return View(food.ToPagedList(page, pagesize));
            }
            else
            {
                var food = db.Foods.Where(x => x.Food_CategoryID == ID).ToList();
                //Thêm cookei
                new CookiesManage().SetFood_intoCookie(food, false);

                var model = food.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pagesize);
                ViewBag.Favorite = db.Favorites.ToList();
                return View(model);
            }
            
        }

        public JsonResult addComment(string json_review)
        {
            var JsonReview = new JavaScriptSerializer().Deserialize<List<Comment>>(json_review);
            var review = new Comment();
            foreach (var item in JsonReview)
            {
                review.Content = item.Content;
                review.Rating = item.Rating;
                review.CreatedDate = DateTime.Now;
                review.User_ID = item.User_ID;
                review.Food_ID = item.Food_ID;
                review.Status = true;
            }

            db.Comments.Add(review);
            db.SaveChanges();

            return Json(new
            {
                status = true
            });

        }

        public JsonResult addReply(string json_review)
        {
            var JsonReview = new JavaScriptSerializer().Deserialize<List<ReplyCmt>>(json_review);
            var review = new ReplyCmt();
            foreach (var item in JsonReview)
            {
                review.Content = item.Content;
                review.CreatedDate = DateTime.Now;
                review.User_ID = item.User_ID;
                review.Comment_ID = item.Comment_ID;
            }

            db.ReplyCmts.Add(review);
            db.SaveChanges();

            return Json(new
            {
                status = true
            });

        }

    }
}