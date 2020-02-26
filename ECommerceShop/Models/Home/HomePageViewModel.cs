using ECommerceShop.Repository;
using ECommerceShop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerceShop.Models.Home
{
    public class HomePageViewModel
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public IEnumerable<Tbl_Product> productList { get; set; }
        public HomePageViewModel CreateModel()
        {
            return new HomePageViewModel
            {
                productList = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecords()
            };
        }
    }
}