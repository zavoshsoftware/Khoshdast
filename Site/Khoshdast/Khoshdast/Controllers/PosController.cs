using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        //public ActionResult GetProductByProductGroup(Guid id)
        //{
        //    Guid productGroupId = id;
        //    var products = db.Products.Where(current => current.productg.IsDeleted == false), "Id", "ResumeFileUrl");
        //    //UnitOfWork.CityRepository.Get(c => c.ProvinceId == provinceId).OrderBy(current => current.Title).ToList();
        //    //List<CityItemViewModel> cityItems = new List<CityItemViewModel>();
        //    //foreach (City city in cities)
        //    //{
        //    //    cityItems.Add(new CityItemViewModel()
        //    //    {
        //    //        Text = city.Title,
        //    //        Value = city.Id.ToString()
        //    //    });
        //    //}
        //    return Json(cityItems, JsonRequestBehavior.AllowGet);
        //}
    }
}