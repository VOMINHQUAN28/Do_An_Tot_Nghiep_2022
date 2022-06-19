using QuickFood.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuickFood.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        Order_Restaurant_Db db = new Order_Restaurant_Db();
        // GET: Admin/Category
        public ActionResult Index()
        {
            ViewBag.lstCategory = db.Food_Category.OrderByDescending(x => x.ID).ToList();
            return View();
        }

        public JsonResult Delete(long ID)
        {

            try
            {
                db.Food_Category.Remove(db.Food_Category.Find(ID));
                db.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }

        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmAdd(Food_Category entity)
        {
            try
            {
                entity.Link = Str_Metatitle(entity.Name);
                db.Food_Category.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm danh mục món ăn thành công";
                return RedirectToAction("Index");

            }
            catch
            {
                TempData["message"] = "Thêm danh mục món ăn KHÔNG thành công";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmEdit(Food_Category entity)
        {
            try
            {
                var prv = db.Food_Category.Find(entity.ID);
                prv.Name = entity.Name.Trim();
                prv.Link = Str_Metatitle(entity.Name.Trim());
                db.SaveChanges();
                TempData["message"] = "Cập nhật danh mục món ăn thành công";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["message"] = "Cập nhật danh mục món ăn KHÔNG thành công";
                return RedirectToAction("Index");
            }
        }

        public JsonResult GetByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var category = db.Food_Category.Find(ID);
            return Json(category, JsonRequestBehavior.AllowGet);
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
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            string str1 = str.ToLower();
            string[] name = str1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string meta = null;
            //Thêm dấu '-'
            foreach (var item in name)
            {
                meta = meta + item + "-";
            }
            return meta;
        }
    }
}