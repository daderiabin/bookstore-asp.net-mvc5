using ECommerceShop.Models.Home;
using ECommerceShop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceShop.Controllers
{
    public class HomeController : Controller
    {
        BookStoreDbEntities db = new BookStoreDbEntities();

        public ActionResult Index(string search, int? page)
        {
            int pageSize = 8; //products per page

            if (search != null)
            {
                page = 1;
            }

            HomePageViewModel model = new HomePageViewModel();
            return View(model.CreateModel(search, pageSize, page));
        }

        public ActionResult AddToCart(int productId)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = db.Tbl_Product.Find(productId);

                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });

                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = db.Tbl_Product.Find(productId);

                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {
                        int currentQty = item.Quantity;

                        cart.Remove(item);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = currentQty + 1
                        });
                        break;
                    }
                    else
                    {
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = 1
                        });
                        break;
                    }
                }

                Session["cart"] = cart;
            }

            return Redirect("Index");
        }

        public ActionResult RemoveFromCart(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];

            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }

            if(cart.Count() > 0)
            {
                Session["cart"] = cart;
            }
            else
            {
                Session["cart"] = null;
            }
            
            return Redirect("Index");
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult Increase(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            var product = db.Tbl_Product.Find(productId);

            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    item.Quantity += 1;
                    break;
                }
            }

            return Redirect("Checkout");
        }

        public ActionResult Decrease(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];

            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    if (item.Quantity > 0)
                    {
                        item.Quantity -= 1;
                        if (item.Quantity == 0)
                        {
                            RemoveFromCart(productId);
                        }
                        break;
                    }
                    else
                    {
                        RemoveFromCart(productId);
                        break;
                    }
                }
            }

            return Redirect("Checkout");
        }

        public ActionResult CheckoutDetails()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}