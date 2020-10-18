using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Helpers;
using Models;
 
using ViewModels;

namespace Khoshdast.Controllers
{
    public class BasketController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        BaseViewModelHelper baseViewModel = new BaseViewModelHelper();
        ZarinPalHelper zp = new ZarinPalHelper();

        [Route("cart")]
        [HttpPost]
        public ActionResult AddToCart(string code, string qty)
        {
            //code=CheckCodeParent(code);
            SetCookie(code, qty);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

     

        [Route("Basket")]
        public ActionResult Basket()
        {
            CartViewModel cart = new CartViewModel();

        

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            cart.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            cart.SubTotal = subTotal.ToString("n0") + " تومان";

            decimal discountAmount = GetDiscount();

            cart.DiscountAmount = discountAmount.ToString("n0") + " تومان";

            cart.Total = (subTotal - discountAmount).ToString("n0");

            return View(cart);
        }

        [Route("Basket/remove/{code}")]
        public ActionResult RemoveFromBasket(string code)
        {
            string[] coockieItems = GetCookie();


            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                string[] coockieItem = coockieItems[i].Split('^');

                if (coockieItem[0] == code)
                {
                    string removeArray = coockieItem[0] + "^" + coockieItem[1];
                    coockieItems = coockieItems.Where(current => current != removeArray).ToArray();
                    break;
                }
            }

            string cookievalue = null;
            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                cookievalue = cookievalue + coockieItems[i] + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket-khoshdast")
            {
                Name = "basket-khoshdast",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });

            return RedirectToAction("basket");
        }

        [AllowAnonymous]
        public ActionResult DiscountRequestPost(string coupon)
        {
            DiscountCode discount = db.DiscountCodes.FirstOrDefault(current => current.Code == coupon);

            string result = CheckCouponValidation(discount);

            if (result != "true")
                return Json(result, JsonRequestBehavior.AllowGet);

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();
            decimal subTotal = GetSubtotal(productInCarts);

            decimal total = subTotal;

            DiscountHelper helper = new DiscountHelper();

            decimal discountAmount = helper.CalculateDiscountAmount(discount, total);

            SetDiscountCookie(discountAmount.ToString(), coupon);

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public decimal GetSubtotal(List<ProductInCart> orderDetails)
        {
            decimal subTotal = 0;

            foreach (ProductInCart orderDetail in orderDetails)
            {
                decimal amount = orderDetail.Product.Amount;

                if (orderDetail.Product.IsInPromotion)
                {
                    if (orderDetail.Product.DiscountAmount != null)
                        amount = orderDetail.Product.DiscountAmount.Value;
                }

                subTotal = subTotal + (amount * orderDetail.Quantity);
            }

            return subTotal;
        }
        public List<ProductInCart> GetProductInBasketByCoockie()
        {
            List<ProductInCart> productInCarts = new List<ProductInCart>();

            string[] basketItems = GetCookie();

            if (basketItems != null)
            {
                for (int i = 0; i < basketItems.Length - 1; i++)
                {
                    string[] productItem = basketItems[i].Split('^');

                    string productCode = productItem[0];

                    Product product =
                        db.Products.FirstOrDefault(current =>
                            current.IsDeleted == false && current.Code == productCode);

                    productInCarts.Add(new ProductInCart()
                    {
                        Product = product,
                        Quantity = Convert.ToInt32(productItem[1]),

                    });
                }
            }

            return productInCarts;
        }
        public void SetCookie(string code, string quantity)
        {
            string cookievalue = null;

            if (Request.Cookies["basket-khoshdast"] != null)
            {
                bool changeCurrentItem = false;

                cookievalue = Request.Cookies["basket-khoshdast"].Value;

                string[] coockieItems = cookievalue.Split('/');

                for (int i = 0; i < coockieItems.Length - 1; i++)
                {
                    string[] coockieItem = coockieItems[i].Split('^');

                    if (coockieItem[0] == code)
                    {
                        coockieItem[1] = (Convert.ToInt32(coockieItem[1]) + 1).ToString();
                        changeCurrentItem = true;
                        coockieItems[i] = coockieItem[0] + "^" + coockieItem[1];
                        break;
                    }
                }

                if (changeCurrentItem)
                {
                    cookievalue = null;
                    for (int i = 0; i < coockieItems.Length - 1; i++)
                    {
                        cookievalue = cookievalue + coockieItems[i] + "/";
                    }

                }
                else
                    cookievalue = cookievalue + code + "^" + quantity + "/";

            }
            else
                cookievalue = code + "^" + quantity + "/";

            HttpContext.Response.Cookies.Set(new HttpCookie("basket-khoshdast")
            {
                Name = "basket-khoshdast",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }



        public string[] GetCookie()
        {
            if (Request.Cookies["basket-khoshdast"] != null)
            {
                string cookievalue = Request.Cookies["basket-khoshdast"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        [AllowAnonymous]
        public string CheckCouponValidation(DiscountCode discount)
        {
            if (discount == null)
                return "Invald";

            if (!discount.IsMultiUsing)
            {
                if (db.Orders.Any(current => current.DiscountCodeId == discount.Id))
                    return "Used";
            }

            if (discount.ExpireDate < DateTime.Today)
                return "Expired";

            return "true";
        }


        public void SetDiscountCookie(string discountAmount, string discountCode)
        {
            HttpContext.Response.Cookies.Set(new HttpCookie("discount")
            {
                Name = "discount",
                Value = discountAmount + "/" + discountCode,
                Expires = DateTime.Now.AddDays(1)
            });
        }
        public decimal GetDiscount()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');
                    return Convert.ToDecimal(basketItems[0]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }


        [Route("checkout")]
        public ActionResult CheckOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                CheckOutViewModel checkOut = new CheckOutViewModel();
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string role = identity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                Guid userId = new Guid(id);

                Models.User user = db.Users.Find(userId);

                if (user != null)
                {
                    UserInformation userInformation=new UserInformation()
                    {
                        FullName = user.FullName,
                        CellNumber = user.CellNum
                    };

                    checkOut.UserInformation = userInformation;
                }

                if (role != "customer")
                {
                    return Redirect("/login?ReturnUrl=checkout");
                }

               

                List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                checkOut.Products = productInCarts;

                decimal subTotal = GetSubtotal(productInCarts);

                checkOut.SubTotal = subTotal.ToString("n0") + " تومان";


                decimal discountAmount = GetDiscount();

                checkOut.DiscountAmount = discountAmount.ToString("n0") + " تومان";

                checkOut.Total = (subTotal - discountAmount).ToString("n0") + " تومان";


                checkOut.Provinces = db.Provinces.OrderBy(current => current.Title).ToList();
                

                ViewBag.CityId = new SelectList(db.Cities, "Id", "Title");

                return View(checkOut);
            }

            return Redirect("/loginregister?ReturnUrl=checkout");

        }

        public ActionResult FillCities(string id)
        {
            Guid provinceId = new Guid(id);
            //   ViewBag.cityId = ReturnCities(provinceId);
            var cities = db.Cities.Where(c => c.ProvinceId == provinceId).OrderBy(current => current.Title).ToList();
            List<CityItemViewModel> cityItems = new List<CityItemViewModel>();
            foreach (Models.City city in cities)
            {
                cityItems.Add(new CityItemViewModel()
                {
                    Text = city.Title,
                    Value = city.Id.ToString()
                });
            }
            return Json(cityItems, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Finalize(string notes, string cellnumber, string postal,
                                    string address, string city, string fullname, string paymentType)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;

                    string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                    Guid userId = new Guid(name);

                    List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                    Order order = ConvertCoockieToOrder(productInCarts);

                    if (order != null)
                    {
                        order.UserId = userId;
                        order.DeliverFullName = fullname;
                        order.DeliverCellNumber = cellnumber;
                        order.Address = address;
                        order.PostalCode = postal;
                        order.Description = notes;
                        order.CityId = new Guid(city);

                       

                        order.TotalAmount = GetTotalAmount(order.SubTotal, order.DiscountAmount, order.ShippingAmount);

                        OrderStatus orderStatus = db.OrderStatuses.FirstOrDefault(current => current.Code == 2);
                        if (orderStatus != null)
                            order.OrderStatusId = orderStatus.Id;

                        db.SaveChanges();
                        RemoveCookie();

                        string res = "/PaymentafterShippment?code="+order.Code;

                        if (paymentType == "online")
                        {
                            decimal onlineAmount = Convert.ToDecimal(order.TotalAmount * 10);
                            //PaymentUniqeCode paymentUniqeCode =
                            //    CreateUniqeCode(onlineAmount, order.Id);

                            string url ="/payment";

                            res = url;
                        }

                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("/login?ReturnUrl=checkout", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);

            }
        }


        #region Finalize
      
      


        public decimal GetTotalAmount(decimal? subtotal, decimal? discount, decimal? shippment)
        {
            decimal discountAmount = 0;
            if (discount != null)
                discountAmount = (decimal)discount;

            decimal shipmentAmount = 0;
            if (shippment != null)
                shipmentAmount = (decimal)shippment;

            if (subtotal == null)
                subtotal = 0;

            return (decimal)subtotal - discountAmount + shipmentAmount;
        }
        public Order ConvertCoockieToOrder(List<ProductInCart> products)
        {
            try
            {
                CodeGenerator codeCreator = new CodeGenerator();

                Order order = new Order();

                order.Id = Guid.NewGuid();
                order.IsActive = true;
                order.IsDeleted = false;
                order.IsPaid = false;
                order.CreationDate = DateTime.Now;
                order.LastModifiedDate = DateTime.Now;
                order.Code = codeCreator.ReturnOrderCode();
                order.OrderStatusId = db.OrderStatuses.FirstOrDefault(current => current.Code == 1).Id;
                order.SubTotal = GetSubtotal(products);


                order.DiscountAmount = GetDiscount();
                order.DiscountCodeId = GetDiscountId();

                order.TotalAmount = Convert.ToDecimal(order.SubTotal + order.ShippingAmount - order.DiscountAmount);


                db.Orders.Add(order);

                foreach (ProductInCart product in products)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductId = product.Product.Id,
                        Quantity = product.Quantity,
                        Amount = product.Product.Amount * product.Quantity,
                        IsDeleted = false,
                        IsActive = true,
                        CreationDate = DateTime.Now,
                        OrderId = order.Id,
                        Price = product.Product.Amount
                    };

                    db.OrderDetails.Add(orderDetail);
                }

                return order;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Guid? GetDiscountId()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');

                    DiscountCode discountCode =
                        db.DiscountCodes.FirstOrDefault(current => current.Code == basketItems[1]);

                    return discountCode?.Id;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
        public User CreateUser(string fullName, string cellNumber)
        {
            CodeGenerator codeCreator = new CodeGenerator();

            Random rnd = new Random();

            User oUser =
                db.Users.FirstOrDefault(current => current.CellNum == cellNumber && current.IsDeleted == false);

            if (oUser == null)
            {
                User user = new User()
                {
                    CellNum = cellNumber,
                    FullName = fullName,
                    Password = rnd.Next(1000, 9999).ToString(),
                    Code = 1000,
                    IsDeleted = false,
                    IsActive = true,
                    CreationDate = DateTime.Now,
                    RemainCredit = 0,
                    RoleId = db.Roles.FirstOrDefault(current => current.Name.ToLower() == "customer").Id,

                };

                db.Users.Add(user);
                db.SaveChanges();
                return user;
            }
            return oUser;
        }


        public void RemoveCookie()
        {
            if (Request.Cookies["basket-khoshdast"] != null)
            {
                Response.Cookies["basket-khoshdast"].Expires = DateTime.Now.AddDays(-1);
            }
        }
        #endregion



        //[HttpPost]
        //[Route("callback")]
        //public ActionResult CallBack(PaymentCallbackModel model)
        //{
        //    BaseViewModelHelper baseViewModel = new BaseViewModelHelper();

        //    CallBackViewModel callback = new CallBackViewModel()
        //    {
        //        Brands = baseViewModel.GetMenu(),
        //        OrderCode = model.OrderId.ToString(),
        //    };


        //    if (model.status == Constants.ParsianPaymentGateway.Successful && (model.Token ?? 0L) > 0L)
        //    {
        //        //ایجاد یک نمونه از سرویس تایید پرداخت
        //        using (var confirmSvc = new ConfirmService())
        //        {
        //            confirmSvc.Url = ConfigHelper.ParsianPGWConfirmServiceUrl;

        //            //ایجاد یک نمونه از نوع پارامتر سرویس تایید پرداخت
        //            var confirmRequestData = new ClientConfirmRequestData();

        //            //شناسه پذیرندگی باید در فراخوانی سرویس تایید تراکنش پرداخت ارائه شود
        //            confirmRequestData.LoginAccount = ConfigHelper.LoginAccount;

        //            //توکن باید ارائه شود
        //            confirmRequestData.Token = model.Token ?? -1;

        //            //فراخوانی سرویس و دریافت نتیجه فراخوانی
        //            var confirmResponse = confirmSvc.ConfirmPayment(confirmRequestData);
        //            //  callBackViewModel.ConfirmResponseStatus = confirmResponse.Status;

        //            //کنترل کد وضعیت نتیجه فراخوانی
        //            //درصورتی که موفق باشد، باید خدمات یا کالا به کاربر پرداخت کننده ارائه شود
        //            if (confirmResponse.Status == Constants.ParsianPaymentGateway.Successful)
        //            {
        //                PaymentUniqeCode paymentUniqeCode = db.PaymentUniqeCodes
        //                    .FirstOrDefault(current => current.Id == model.OrderId);

        //                if (paymentUniqeCode != null)
        //                {
        //                    paymentUniqeCode.Amount = Convert.ToDecimal(model.Amount);
        //                    paymentUniqeCode.HashCartNumber = model.HashCardNumber;
        //                    paymentUniqeCode.RNN = model.RRN.ToString();
        //                    paymentUniqeCode.Status = model.status;
        //                    paymentUniqeCode.TerminalNumber = model.TerminalNo;
        //                    paymentUniqeCode.Token = model.ToString();
        //                    paymentUniqeCode.IsSuccess = true;
        //                    db.SaveChanges();

        //                    callback.IsSuccess = true;
        //                    callback.RefrenceId = model.RRN.ToString();
        //                    return View("CallBack", callback);

        //                }
        //                callback.IsSuccess = false;
        //                return View("CallBack", callback);
        //            }

        //            callback.IsSuccess = false;
        //            return View("CallBack", callback);
        //        }




        //    }

        //    callback.IsSuccess = false;
        //    return View("CallBack", callback);
        //    //String Status = status;
        //    //CallBackViewModel callBack = new CallBackViewModel();
        //    //callBack.Brands = baseViewModel.GetMenu();
        //    //if (Status != "OK")
        //    //    callBack.IsSuccess = false;

        //    //else
        //    //{
        //    //    try
        //    //    {
        //    //        var zarinpal = ZarinPal.ZarinPal.Get();
        //    //        zarinpal.DisableSandboxMode();
        //    //        String Authority = authority;
        //    //        long Amount = zp.GetAmountByAuthority(Authority);

        //    //        var verificationRequest = new ZarinPal.PaymentVerification(zp.GetMerchantId(), Amount, Authority);
        //    //        var verificationResponse = zarinpal.InvokePaymentVerification(verificationRequest);
        //    //        if (verificationResponse.Status == 100)
        //    //        {
        //    //            Order order = zp.GetOrderByAuthority(authority);
        //    //            if (order != null)
        //    //            {
        //    //                order.IsPaid = true;
        //    //                order.OrderStatusId = db.OrderStatuses.FirstOrDefault(current => current.Code == 3).Id;
        //    //                order.PaymentDate = DateTime.Now;

        //    //                ChangeStockAfterPayment(order.Id);

        //    //                db.SaveChanges();
        //    //                callBack.IsSuccess = true;
        //    //                callBack.OrderCode = order.Code.ToString();
        //    //                callBack.RefrenceId = verificationResponse.RefID;

        //    //            }
        //    //            else
        //    //            {
        //    //                callBack.IsSuccess = false;
        //    //                callBack.RefrenceId = "سفارش پیدا نشد";
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            callBack.IsSuccess = false;
        //    //            callBack.RefrenceId = verificationResponse.Status.ToString();
        //    //        }
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        callBack.IsSuccess = false;
        //    //        callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
        //    //    }
        //    //}
        //   // return View(callBack);

        //}

        //[Route("PaymentafterShippment")]
        //public ActionResult CallBackFree(string code)
        //{
        //    CallBackViewModel callBack = new CallBackViewModel();
        //    callBack.Brands = baseViewModel.GetMenu();
          

         
        //        try
        //        {
        //        callBack.IsSuccess = true;
        //            callBack.OrderCode = code;
        //        }
        //        catch (Exception e)
        //        {
        //            callBack.IsSuccess = false;
        //            callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
        //        }
            
        //    return View(callBack);

        //}

        public void ChangeStockAfterPayment(Guid orderId)
        {
            List<OrderDetail> orderDetails = db.OrderDetails.Where(current =>
                current.OrderId == orderId && current.IsDeleted == false && current.IsActive == true).ToList();

            foreach (OrderDetail orderDetail in orderDetails)
            {
                Product product = db.Products.FirstOrDefault(current => current.Id == orderDetail.ProductId);

                if (product != null)
                {
                    product.Stock = product.Stock
                                    - orderDetail.Quantity;
                }
            }
        }
    }
}