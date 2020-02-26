using ECommerceShop.Repository;
using ECommerceShop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using PagedList;
using PagedList.Mvc;

namespace ECommerceShop.Models.Home
{
    public class HomePageViewModel
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        BookStoreDbEntities db = new BookStoreDbEntities();
        public IPagedList<Tbl_Product> productList { get; set; }
        public HomePageViewModel CreateModel(string search, int pageSize, int? page)
        {
            IPagedList<Tbl_Product> data = db.Database.SqlQuery<Tbl_Product>("SELECT * from Tbl_Product P " +
                                                                             "left join Tbl_Category C on p.CategoryId=c.CategoryId " +
                                                                             "where " +
                                                                             "P.ProductName LIKE CASE WHEN @search is not null then '%'+@search+'%' else P.ProductName end " +
                                                                             "OR " +
                                                                             "C.CategoryName LIKE CASE WHEN @search is not null then '%'+@search+'%' else C.CategoryName end", 
                                                                             new SqlParameter("@search", search ?? (object)DBNull.Value)).ToList().ToPagedList(page ?? 1, pageSize);

            return new HomePageViewModel
            {
                productList = data
            };
        }
    }
}