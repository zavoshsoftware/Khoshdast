using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace Khoshdast.Controllers
{
    [Authorize(Roles = "customer")]
    public class DashboardController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {

            DashboardViewModel result = new DashboardViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                Guid userId = new Guid(id);

                User user = db.Users.Find(userId);

                List<Order> orders = db.Orders.Where(c => c.UserId == userId).ToList();

                if (user != null)
                {
                    result.CreationDateStr = user.CreationDateStr;
                    result.FullName = user.FullName;
                    result.UserCellNumber = user.CellNum;
                    result.UserEmail = user.Email;
                    result.TotalCompleteOrderCount = orders.Count(c => c.IsPaid);
                    result.TotalNotCompleteOrderCount = orders.Count(c => c.IsPaid == false);
                }
            }
            return View(result);
        }

        [Route("dashboard/order")]
        public ActionResult OrderList()
        {

            CustomerOrderListViewModel result = new CustomerOrderListViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                Guid userId = new Guid(id);

                User user = db.Users.Find(userId);

                List<Order> orders = db.Orders.Where(c => c.UserId == userId).OrderByDescending(c=>c.CreationDate).ToList();

                if (user != null)
                {
                    result.Orders = orders;
                    ViewBag.Title = "سفارشات";
                }
            }
            return View(result);
        }

        [Route("dashboard/order/details/{id:Guid}")]
        public ActionResult Details(Guid id)
        {

            CustomerOrderDetailViewModel result = new CustomerOrderDetailViewModel();

            if (User.Identity.IsAuthenticated)
            {
                Order order = db.Orders.Find(id);

                if (order != null)
                {
                    result.Order = order;
                    result.OrderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id).ToList();
                    ViewBag.Title = "جزییات سفارش کد: " + order.Code;
                }
            }
            return View(result);
        }

        [Route("dashboard/editProfile")]
        public ActionResult EditProfile()
        {


            DashboardViewModel result = new DashboardViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                Guid userId = new Guid(id);

                User user = db.Users.Find(userId);


                if (user != null)
                {
                    result.CreationDateStr = user.CreationDateStr;
                    result.FullName = user.FullName;
                    result.UserCellNumber = user.CellNum;
                    result.UserEmail = user.Email;
                    result.Id = user.Id;
                }
            }
            return View(result);
        }

        [HttpPost]
        [Route("dashboard/editProfile")]
        public ActionResult EditProfile(DashboardViewModel dashboard)
        {


            DashboardViewModel result = new DashboardViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                Guid userId = new Guid(id);

                User user = db.Users.Find(userId);

        
                if (user != null)
                {
                    user.Email = dashboard.UserEmail;
                    user.CellNum = dashboard.UserCellNumber;
                    user.FullName = dashboard.FullName;
                    user.LastModifiedDate = DateTime.Now;

                    db.SaveChanges();

                    TempData["successPost"] = "تغییرات با موفقیت انجام شد.";

                    result.CreationDateStr = user.CreationDateStr;
                    result.FullName = user.FullName;
                    result.UserCellNumber = user.CellNum;
                    result.UserEmail = user.Email;
                    result.Id = user.Id;
                }
            }
            return View(result);
        }

    }
}