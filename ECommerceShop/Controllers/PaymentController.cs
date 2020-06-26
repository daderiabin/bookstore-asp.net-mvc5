using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceShop.Controllers
{
    public class PaymentController : Controller
    {

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {                    
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Payment/PaymentWithPaypal?";                    
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

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
            
            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };

            this.payment = new Payment() { id = paymentId };

            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            int subTotal = 0;
            var itemList = new ItemList() { items = new List<Item>() };

            if (Session["cart"] != null)
            {
                List<Models.Home.Item> cart = (List<Models.Home.Item>)Session["cart"];

                foreach (var item in cart)
                {
                    int productTotal = Convert.ToInt32(item.Quantity * item.Product.Price);
                    subTotal = Convert.ToInt32(subTotal + productTotal);

                    itemList.items.Add(new Item()
                    {
                        name = item.Product.ProductName,
                        currency = "CAD",
                        price = item.Product.Price.ToString(),
                        quantity = item.Quantity.ToString(),
                        sku = "sku"
                    });
                }
            }

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = ((double)subTotal * 0.13).ToString(),
                shipping = ((double)subTotal * 0.05).ToString(),
                subtotal = ((double)subTotal).ToString()
            };

            var amount = new Amount()
            {
                currency = "CAD",
                total = ((double)subTotal + ((double)subTotal * 0.13) + ((double)subTotal * 0.05)).ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = "#1010101",
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

            return this.payment.Create(apiContext);
        }
    }
}