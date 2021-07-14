using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ViewModels;

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
            List<DropDownListViewModel> paymentTypeDropDowns = new List<DropDownListViewModel>()
            {
                new DropDownListViewModel() {Text = "پرداخت آنلاین",Value = "online" },
                new DropDownListViewModel() {Text = "پرداخت در محل",Value = "recieve" },
                new DropDownListViewModel() {Text = "کارت به کارت",Value = "transfer" },
                //........................ and so on
            };

            ViewBag.PaymentTypeId =
                new SelectList(paymentTypeDropDowns, "Value",
                    "Text");
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
    }
}