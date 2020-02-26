using ECommerceShop.DAL;
using ECommerceShop.Models;
using ECommerceShop.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceShop.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        public List<SelectListItem> GetCategories()
        {
            List<SelectListItem> categories = new List<SelectListItem>();
            var category = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords();

            foreach (var item in category)
            {
                categories.Add(new SelectListItem { Value = item.CategoryId.ToString(), Text = item.CategoryName });
            }

            return categories;
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Categories()
        {
            List<Tbl_Category> allCategories = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecordsIQueryable().Where(i => i.IsDeleted == false).ToList();
            return View(allCategories);
        }

        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(tbl);
            return RedirectToAction("Categories");
        }

        public ActionResult EditCategory(int categoryId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstOrDefault(categoryId));
        }

        [HttpPost]
        public ActionResult EditCategory(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Update(tbl);
            return RedirectToAction("Categories");
        }

        public ActionResult DeleteCategory(int categoryId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstOrDefault(categoryId));
        }

        [HttpPost]
        public ActionResult DeleteCategory(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Remove(tbl);
            return RedirectToAction("Categories");
        }

        public ActionResult Products()
        {
            List<Tbl_Product> allProducts = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecordsIQueryable().Where(i => i.IsDeleted == false).ToList();
            return View(allProducts);
        }

        public ActionResult AddProduct()
        {
            ViewBag.CategoryList = GetCategories();
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Tbl_Product tbl, HttpPostedFileBase file)
        {
            string pic = null;

            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Products"), pic);
                // file is uploaded
                file.SaveAs(path);
            }

            tbl.ProductImage = pic;

            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(tbl);
            return RedirectToAction("Products");
        }

        public ActionResult EditProduct(int productId)
        {
            ViewBag.CategoryList = GetCategories();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstOrDefault(productId));
        }

        [HttpPost]
        public ActionResult EditProduct(Tbl_Product tbl, HttpPostedFileBase file)
        {
            string pic = null;

            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Products"), pic);
                // file is uploaded
                file.SaveAs(path);
            }

            tbl.ProductImage = pic;

            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(tbl);
            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(int productId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstOrDefault(productId));
        }

        [HttpPost]
        public ActionResult DeleteProduct(Tbl_Product tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Remove(tbl);
            return RedirectToAction("Products");
        }
    }
}
