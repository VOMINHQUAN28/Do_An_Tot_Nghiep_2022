using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using QuickFood.Models.Business;
using QuickFood.Models.EF;

namespace QuickFood.Areas.Admin.Controllers
{
    public class FooderController : Controller
    {
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        // GET: Admin/Fooder
        public ActionResult Index(long? Food_ID, int page = 1, int pagesize = 10)
        {
            var model = db.Foods.Where(x => x.Status == true).OrderByDescending(x => x.CreatedDate).ToPagedList(page, pagesize);
            var lstTopping = new List<Topping>();
            if(Food_ID != 0)
            {
                lstTopping = db.Toppings.Where(x => x.Food_ID == Food_ID).ToList();
            }
            else
            {
                lstTopping = db.Toppings.OrderByDescending(x => x.Food.CreatedDate).ToList();
            }

            ViewBag.LstTopping = lstTopping;
            return View(model);
        }



        // GET: Admin/Fooder/Create
        public ActionResult Create()
        {
            ViewBag.LstCategory = db.Food_Category.ToList();
            return View();
        }

        // POST: Admin/Fooder/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Food entity, HttpPostedFileBase Image)
        {
            try
            {
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/Client/img/food"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/Client/img/food"), filename);
                    Image.SaveAs(path);
                    entity.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    entity.Image = Image.FileName;
                }

                entity.CreatedDate = DateTime.Now;
                entity.MetaTitle = Str_Metatitle(entity.Name);
                entity.Status = true;
                db.Foods.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm mới món ăn thành công!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["message"] = "Thêm món ăn KHÔNG thành công";
                return RedirectToAction("Index");
            }

        }

        // GET: Admin/Fooder/Edit/5
        public ActionResult Edit(int ID)
        {
            ViewBag.Food = new FoodBusiness().FindID(ID);
            ViewBag.LstCategory = db.Food_Category.ToList();
            return View();
        }

        // POST: Admin/Fooder/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Food entity, HttpPostedFileBase Image)
        {
            var food = db.Foods.Find(entity.ID);

            try
            {
                if (Image != null && food.Image != Image.FileName)
                {
                    //Xóa file cũ
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/food"), food.Image));
                    //Thêm hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Assets/Client/img/food"), Image.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        string extensionName = Path.GetExtension(Image.FileName);
                        string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                        path = Path.Combine(Server.MapPath("~/Assets/Client/img/food"), filename);
                        Image.SaveAs(path);
                        food.Image = filename;
                    }
                    else
                    {
                        Image.SaveAs(path);
                        food.Image = Image.FileName;
                    }
                }

                food.Name = entity.Name;
                food.Price = entity.Price;
                food.Food_CategoryID = entity.Food_CategoryID;
                food.CreatedDate = DateTime.Now;
                food.MetaTitle = Str_Metatitle(entity.Name);

                db.SaveChanges();
                TempData["message"] = "Cập nhật món ăn thành công!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["message"] = "Cập nhật món ăn KHÔNG thành công!";
                return RedirectToAction("Index");
            }

        }



        // POST: Admin/Fooder/Delete/5
        [HttpPost]
        public JsonResult Delete(long ID)
        {
            var food = db.Foods.Find(ID);
            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/food"), food.Image));

            var res = new FoodBusiness().DeleteFood(ID);
            if (res)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }

        }

        [HttpPost]
        public ActionResult frmAdd_Topping(Topping entity, HttpPostedFileBase Image, int Page)
        {
            try
            {
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/Client/img/topping"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/Client/img/topping"), filename);
                    Image.SaveAs(path);
                    entity.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    entity.Image = Image.FileName;
                }

                db.Toppings.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm mới topping thành công!";
                return Redirect("/Admin/Fooder?Food_ID=" + entity.Food_ID + "&Page=" + Page);
            }
            catch
            {
                TempData["message"] = "Thêm topping KHÔNG thành công";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult frmEdit_Topping(Topping entity, HttpPostedFileBase Image, int Page)
        {
            var topping = db.Toppings.Find(entity.ID);

            try
            {
                if (Image != null && Image.FileName != topping.Image)
                {
                    //Xóa file cũ
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/topping"), topping.Image));
                    //Thêm hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Assets/Client/img/topping"), Image.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        string extensionName = Path.GetExtension(Image.FileName);
                        string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                        path = Path.Combine(Server.MapPath("~/Assets/Client/img/topping"), filename);
                        Image.SaveAs(path);
                        topping.Image = filename;
                    }
                    else
                    {
                        Image.SaveAs(path);
                        topping.Image = Image.FileName;
                    }
                }

                topping.Name = entity.Name;
                topping.Price = entity.Price;

                db.SaveChanges();
                TempData["message"] = "Cập nhật topping thành công!";
                return Redirect("/Admin/Fooder?Food_ID=" + topping.Food_ID + "&Page=" + Page);
            }
            catch
            {
                TempData["message"] = "Cập nhật topping KHÔNG thành công!";
                return RedirectToAction("Index");
            }
        }

        public JsonResult DeleteTopping(long ID)
        {
            var topping = db.Toppings.Find(ID);
            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/topping"), topping.Image));

            db.Toppings.Remove(topping);
            db.SaveChanges();
            return Json(new { status = true });
        }

        public JsonResult GetFoodById(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var food = db.Foods.Find(ID);
            return Json(food, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetToppingById(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var topping = db.Toppings.Find(ID);
            return Json(topping, JsonRequestBehavior.AllowGet);
        }

        public string Str_Metatitle(string str)
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
                "ÝỲỴỶỸ:/"
            };
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                {
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]).Replace("“", string.Empty).Replace("”", string.Empty);
                    str = str.Replace("\"", string.Empty).Replace("'", string.Empty).Replace("`", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty);
                    str = str.Replace(".", string.Empty).Replace(",", string.Empty).Replace(";", string.Empty).Replace(":", string.Empty);
                    str = str.Replace("?", string.Empty);
                }
            }
            string str1 = str.ToLower();
            string[] name = str1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string meta = null;
            //Thêm dấu '-'
            foreach (var item in name)
            {
                meta = meta + item + "-";
            }
            return meta.Trim();
        }

    }
}