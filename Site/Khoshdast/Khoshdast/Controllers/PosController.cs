using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ViewModels;
using System.Data.Entity;
using Helpers;

namespace Khoshdast.Controllers
{
    public class PosController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        // GET: Pos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups.Where(current => current.IsDeleted == false), "Id", "Title");
            ViewBag.ProductId = new SelectList(db.Products.Where(current => current.IsDeleted == false), "Id", "Title");
            //List<DropDownListViewModel> paymentTypeDropDowns = new List<DropDownListViewModel>()
            //{
            //    new DropDownListViewModel() {Text = "پرداخت آنلاین",Value = "online" },
            //    new DropDownListViewModel() {Text = "پرداخت در محل",Value = "recieve" },
            //    new DropDownListViewModel() {Text = "کارت به کارت",Value = "transfer" },
            //    //........................ and so on
            //};

            //ViewBag.PaymentTypeId =
            //    new SelectList(paymentTypeDropDowns, "Value",
            //        "Text");

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(current => current.IsDeleted == false), "Id", "Title");
            return View();
        }

        public ActionResult GetProductByProductGroup(Guid id)
        {
            List<DropDownListViewModel> result = new List<DropDownListViewModel>();
            Guid productGroupId = id;
            var products = GetProductListByProductGroupId(id);
            foreach (Product product in products.ToList())
            {
                result.Add(new DropDownListViewModel()
                {
                    Text = product.Title,
                    Value = product.Id.ToString(),
                });
            }
            //foreach (City city in cities)
            //{
            //    cityItems.Add(new CityItemViewModel()
            //    {
            //        Text = city.Title,
            //        Value = city.Id.ToString()
            //    });
            //}
            return Json(result.OrderBy(c => c.Text), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadProductBySelectProduct(Guid id)
        {
            var products = GetProductListBySelectId(id);

            ProductInputViewModel productInput = GetProductList(products);
            //string productJson = JsonConvert.SerializeObject(productInput);
            return Json(productInput, JsonRequestBehavior.AllowGet);
        }

        public List<Product> GetProductListBySelectId(Guid productId)
        {
            List<Product> products = new List<Product>();

            products = db.Products
                .Where(c => c.Id == productId && c.IsDeleted == false).ToList();


            foreach (Product product in products)
            {
                if (!products.Any(c => c.Id == product.Id))
                    products.Add(product);
            }


            return products.OrderByDescending(c => c.Stock).ThenByDescending(c => c.Amount).ToList();
        }

        [AllowAnonymous]
        public ActionResult LoadProductByGroupId(Guid id)
        {

            var products = GetProductListByProductGroupId(id);

            ProductInputViewModel productInput = GetProductList(products);
            //string productJson = JsonConvert.SerializeObject(productInput);
            return Json(productInput, JsonRequestBehavior.AllowGet);
        }

        public ProductInputViewModel GetProductList(List<Product> products)
        {

            ProductInputViewModel productInput =
                new ProductInputViewModel()
                {
                    Products = GetProductViewModel(products),
                };

            return productInput;
        }

        public List<ProductItemViewModel> GetProductViewModel(List<Product> products)
        {
            Guid? id = null;
            try
            {
                List<ProductItemViewModel> productItems = new List<ProductItemViewModel>();

                foreach (Product product in products)
                {
                    id = product.Id;
                    productItems.Add(new ProductItemViewModel()
                    {
                        Id = product.Id.ToString(),
                        Title = product.Title,
                        Amount = product.AmountStr,
                        ImageUrl = WebConfigurationManager.AppSettings["baseUrl"] + product.ImageUrl,
                    });
                }

                return productItems;
            }
            catch (Exception e)
            {
                Guid? asd = id;
                Console.WriteLine(e);
                throw;
            }
        }

        public List<Product> GetProductListByProductGroupId(Guid productGroupId)
        {
            List<Product> products = new List<Product>();

            List<ProductGroupRelProduct> productGroupRel = db.ProductGroupRelProducts
                .Where(c => (c.ProductGroupId == productGroupId || c.ProductGroup.ParentId == productGroupId
                             || c.ProductGroup.Parent.ParentId == productGroupId) && c.IsDeleted == false).ToList();


            foreach (ProductGroupRelProduct groupRelProduct in productGroupRel)
            {
                if (!products.Any(c => c.Id == groupRelProduct.ProductId))
                    products.Add(groupRelProduct.Product);
            }


            return products.OrderByDescending(c => c.Stock).ThenByDescending(c => c.Amount).ToList();
        }

        [AllowAnonymous]
        public ActionResult GetBasketInfoByCookie()
        {
            string[] coockieProducts = GetCookie();
            BasketViewModel basket = CalculateBasket(coockieProducts);
            return Json(basket, JsonRequestBehavior.AllowGet);


        }

        public string[] GetCookie()
        {
            if (Request.Cookies["basket"] != null)
            {
                string cookievalue = Request.Cookies["basket"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        public BasketViewModel CalculateBasket(string[] coockieProducts)
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            decimal totalAmount = 0;

            foreach (string oProduct in coockieProducts)
            {
                string[] productItems = oProduct.Split('^');

                string productId = productItems[0];


                if (!string.IsNullOrEmpty(productId))
                {

                    Guid id = new Guid(productId);

                    Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();

                    decimal amount = product.Amount;
                    decimal rowAmount;
                    if (productItems.Length == 1)
                        rowAmount = amount;
                    else
                        rowAmount = amount * Convert.ToInt32(productItems[1]);

                    totalAmount = Convert.ToDecimal(totalAmount + rowAmount);
                    
                    if (product != null)
                    {

                        //string parentTitle;

                        //if (product.ParentId != null)
                        //    parentTitle = UnitOfWork.ProductRepository.GetById(product.ParentId.Value).Title;
                        //else
                        //    parentTitle = product.Title;
                        string qty = "1";
                        if (productItems.Length != 1)

                            qty = productItems[1];


                        basketItems.Add(new BasketItemViewModel()
                        {
                            Id = productId,
                            Amount = (amount).ToString("n0"),
                            // ChildProducts = GetChildProducts(UnitOfWork.ProductColorRepository.Get(c => c.ProductId == product.Id).ToList(), selectedColor),
                            Title = product.Title,
                            Quantity = qty,
                            RowAmount = rowAmount.ToString("n0"),
                            Description = "",
                        });
                    }
                }
            }

            BasketViewModel basket = GetBasket(basketItems, totalAmount);

            return basket;
        }

        public BasketViewModel GetBasket(List<BasketItemViewModel> basketITems, decimal totalAmount)
        {
            BasketViewModel basket = new BasketViewModel()
            {
                Products = basketITems,
                Total = totalAmount.ToString("n0") + " تومان"
            };

            return basket;

        }

        [AllowAnonymous]
        public ActionResult RemoveFromBasket(string id)
        {
            string[] coockieProducts = GetCookie();

            foreach (string productId in coockieProducts)
            {
                if (!string.IsNullOrEmpty(productId))
                {
                    if (productId.Split('^')[0] == id)
                    {
                        coockieProducts = coockieProducts.Where(current => current != productId).ToArray();
                        break;
                    }
                }
            }

            SetCookie(coockieProducts);

            BasketViewModel basket = CalculateBasket(coockieProducts);

            return Json(basket, JsonRequestBehavior.AllowGet);
        }

        public void SetCookie(string[] basket)
        {
            string cookievalue = null;

            Deletecookie();

            foreach (string s in basket)
            {
                if (!string.IsNullOrEmpty(s))
                    cookievalue = cookievalue + s + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket")
            {
                Name = "basket",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }


        public void Deletecookie()
        {
            HttpCookie currentUserCookie = Request.Cookies["basket"];
            Response.Cookies.Remove("basket");
            if (currentUserCookie != null)
            {
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                Response.SetCookie(currentUserCookie);
            }
        }

        public ActionResult GetUserFullName(string cellNumber)
        {
            User user = db.Users.Where(current => current.CellNum == cellNumber && current.Role.Name == "customer").FirstOrDefault();

            if (user == null)
                return Json("invalid", JsonRequestBehavior.AllowGet);


            return Json(user.FullName, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostFinalize(string orderDate, string cellNumber, string fullName, string address, string addedAmount,
            string decreasedAmount, string desc, string paymentAmount, string paymentTypeId, string file, string subtotalAmount, string totalAmount)
        {
            try
            {
                OrderInsertViewModel orderDetails = GetProductIdByCookie();

                User user = GetCurrentUser(cellNumber, fullName);

                //DateTime dtOrderDete = DateTimeHelper.PostPersianDate(orderDate);
                DateTime dtOrderDete = DateTime.Now;
                Order order = InsertOrder(user, dtOrderDete, address,
                    addedAmount, decreasedAmount, desc, paymentAmount, paymentTypeId,
                    orderDetails.SubTotal, file);

                InsertToOrderDetail(orderDetails, order.Id);
                //InsertToAccount(order.SubAmount + order.AdditiveAmount - order.DiscountAmount, order.PaymentAmount,
                //    order.BranchId, order.UserId, order.Code);

                db.SaveChanges();

                return Json("true-" + order.Code, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public OrderInsertViewModel GetProductIdByCookie()
        {
            string[] coockieProducts = GetCookie();

            List<PosInsertViewModel> productList = new List<PosInsertViewModel>();

            decimal subTotal = 0;

            for (int i = 0; i < coockieProducts.Length - 1; i++)
            {
                string[] productFeatures = coockieProducts[i].Split('^');

                Guid id = new Guid(productFeatures[0]);


                Product oProduct = db.Products.Where(c => c.Id == id).FirstOrDefault();

                

                int qty = Convert.ToInt32(productFeatures[1]);

                if (oProduct != null)
                {
                    decimal amount = oProduct.Amount;


                    decimal rowAmount = qty * amount;

                    subTotal = subTotal + rowAmount;


                    PosInsertViewModel input = new PosInsertViewModel()
                    {
                        ProductId = oProduct.Id,
                        Quantity = qty,
                        RowAmount = rowAmount,
                        Amount = amount,
                    };

                    productList.Add(input);

                }
            }

            OrderInsertViewModel orderDetails = new OrderInsertViewModel()
            {
                OrderDetails = productList,
                SubTotal = subTotal
            };

            return orderDetails;
        }

        public User GetCurrentUser(string cellNumber, string fullName)
        {
            User user = db.Users.Where(current => current.CellNum == cellNumber && current.Role.Name == "customer").FirstOrDefault();

            if (user != null)
            {
                user.FullName = fullName;

                db.Entry(user).State = EntityState.Modified;

                return user;
            }

            user = CreateUser(cellNumber, fullName);

            return user;
        }

        public User CreateUser(string cellNumber, string fullName)
        {
            User user = db.Users.OrderByDescending(current => current.Code).FirstOrDefault();
            int? generateCode = 1;
            if (user != null)
            {
                generateCode = user.Code + 1;
            }
            user = new User()
            {
                CellNum = cellNumber,
                FullName = fullName,
                Code = generateCode,
                IsActive = true,
                Password = RandomCode().ToString(),
                RoleId = db.Roles.Where(current => current.Name == "customer").FirstOrDefault().Id,
                IsDeleted = false,
                CreationDate = DateTime.Now
            };
            db.Users.Add(user);
            db.SaveChanges();

            return user;
        }

        public int RandomCode()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000).ToString("D5");
            return Convert.ToInt32(r);
        }

        public Order InsertOrder(User user, DateTime orderDate, string address, string addedAmount,
            string decreasedAmount, string desc, string paymentAmount, string paymentTypeId, decimal subTotal, string fileUrl)
        {
            decimal additiveAmount = Convert.ToDecimal(addedAmount);
            decimal discountAmount = Convert.ToDecimal(decreasedAmount);
            decimal totalAmount = subTotal + additiveAmount - discountAmount;
            decimal paymentAmountDecimal = Convert.ToDecimal(paymentAmount);
            decimal remainAmountDecimal = totalAmount - paymentAmountDecimal;

            bool isPaid = totalAmount == paymentAmountDecimal;


            Guid orderStatusId = db.OrderStatuses.Where(current => current.Code == 1).FirstOrDefault().Id;


            Order order = new Order()
            {
                Code = ReturnCode(),
                UserId = user.Id,
                SubTotal = subTotal,
                AdditiveAmount = additiveAmount,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                PaymentAmount = paymentAmountDecimal,
                Address = address,
                PaymentDate = orderDate,
                IsPaid = isPaid,
                IsActive = true,
                Description = desc,
                RemainAmount = remainAmountDecimal,
                OrderStatusId = orderStatusId,
                IsPos = true,
                DecreaseAmount= discountAmount,
                DeliverCellNumber = user.CellNum,
                DeliverFullName = user.FullName,
                CreationDate = DateTime.Now,
                IsDeleted = false
            };

            db.Orders.Add(order);
            db.SaveChanges();

            if (paymentAmountDecimal > 0)
                InsertToPayment(paymentAmountDecimal, order.Id, totalAmount, paymentTypeId);

            return order;
        }

        public int ReturnCode()
        {
            int orderCode = 1;
            Order order = db.Orders.OrderByDescending(current => current.Code).FirstOrDefault();

            if (order != null)
            {
                orderCode = order.Code + 1;
            }
            return orderCode;
        }
        public void InsertToPayment(decimal payAmount, Guid orderId, decimal totalAmount, string paymentTypeId)
        {
            bool isDeposit = payAmount < totalAmount;

            Payment payment = new Payment()
            {
                Amount = payAmount,
                IsDeposit = isDeposit,
                PaymentTypeId = new Guid(paymentTypeId),
                OrderId = orderId,
                IsActive = true,
                PaymentDay = DateTime.Now,
                IsDeleted = false,
                CreationDate = DateTime.Now

            };

            db.Payments.Add(payment);
            db.SaveChanges();
        }
        public void InsertToOrderDetail(OrderInsertViewModel orderDetails, Guid orderId)
        {
            foreach (PosInsertViewModel detail in orderDetails.OrderDetails)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = orderId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    Amount = detail.Amount,
                    Price = detail.RowAmount,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.Now
                };

                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }
        }

    }
}