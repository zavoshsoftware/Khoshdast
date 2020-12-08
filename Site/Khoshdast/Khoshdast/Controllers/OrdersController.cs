using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;

namespace Khoshdast.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Orders
        public ActionResult Index(Guid? id)
        {
            List<Order> orders = new List<Order>();

            if (id != null)
            {
                if (id == new Guid("11869F4D-D6D1-434C-A2F1-7945227CD3BB"))
                {
                    Guid status2 = new Guid("EC934A7E-0061-4B09-BD44-CA5120CF6200");
                    Guid status3 = new Guid("7DBF85F4-7835-4D21-8269-26695D0C7E0F");
                    orders = db.Orders.Include(o => o.City).Where(o =>
                            (o.OrderStatusId == id || o.OrderStatusId == status3 || o.OrderStatusId == status2) &&
                            o.IsDeleted == false)
                        .OrderByDescending(o => o.CreationDate).Include(o => o.DiscountCode).Include(o => o.OrderStatus)
                        .Include(o => o.User).ToList();
                }
                else
                {
                    orders = db.Orders.Include(o => o.City).Where(o =>
                           o.OrderStatusId == id  && o.IsDeleted == false)
                        .OrderByDescending(o => o.CreationDate).Include(o => o.DiscountCode).Include(o => o.OrderStatus)
                        .Include(o => o.User).ToList();
                }
            }
            else
            {

                orders = db.Orders.Include(o => o.City).Where(o => o.IsDeleted == false)
                    .OrderByDescending(o => o.CreationDate).Include(o => o.DiscountCode).Include(o => o.OrderStatus)
                    .Include(o => o.User).ToList();

            }
            return View(orders);
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            List<OrderDetail> orderDetails = db.OrderDetails.Include(current => current.Product).Where(current => current.OrderId == order.Id).ToList();

            OrderDetailViewModel orderDetailViewModel = new OrderDetailViewModel()
            {
                Order = order,
                OrderDetails = orderDetails
            };
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "Id", "Title", order.OrderStatusId);

            return View(orderDetailViewModel);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Title");
            ViewBag.DiscountCodeId = new SelectList(db.DiscountCodes, "Id", "Code");
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,UserId,Address,TotalAmount,OrderStatusId,CityId,SaleReferenceId,IsPaid,DiscountCodeId,ShippingAmount,SubTotal,DiscountAmount,DeliverFullName,DeliverCellNumber,PostalCode,PaymentDate,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.IsDeleted = false;
                order.CreationDate = DateTime.Now;

                order.Id = Guid.NewGuid();
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities, "Id", "Title", order.CityId);
            ViewBag.DiscountCodeId = new SelectList(db.DiscountCodes, "Id", "Code", order.DiscountCodeId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "Id", "Title", order.OrderStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Title", order.CityId);
            ViewBag.DiscountCodeId = new SelectList(db.DiscountCodes, "Id", "Code", order.DiscountCodeId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "Id", "Title", order.OrderStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,UserId,Address,TotalAmount,OrderStatusId,CityId,SaleReferenceId,IsPaid,DiscountCodeId,ShippingAmount,SubTotal,DiscountAmount,DeliverFullName,DeliverCellNumber,PostalCode,PaymentDate,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.IsDeleted = false;
                order.LastModifiedDate = DateTime.Now;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Title", order.CityId);
            ViewBag.DiscountCodeId = new SelectList(db.DiscountCodes, "Id", "Code", order.DiscountCodeId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "Id", "Title", order.OrderStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Order order = db.Orders.Find(id);
            order.IsDeleted = true;
            order.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        [AllowAnonymous]
        public ActionResult ChangeOrderStatus(string code, string statusId)
        {

            Guid id = new Guid(statusId);
            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                order.OrderStatusId = id;
                order.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ChangeOrderPaymentStatus(string code, string statusId, string paymentDesc)
        {

            bool isPay = Convert.ToBoolean(statusId);
            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                order.IsPaid = isPay;
                order.PaymentDesc = paymentDesc;
                order.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SendSmsToUser(string code, string userSms)
        {


            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                SendSms.SendCommonSms(order.User.CellNum, userSms);

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SubmitOrderDesc(string code, string desc)
        {


            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

            if (order != null)
            {
                order.Description = desc;
                order.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        //public string SerOrderStatus()
        //{
        //    Guid id = new Guid("7dbf85f4-7835-4d21-8269-26695d0c7e0f");
        //    List<Order> orders = db.Orders.Where(c => c.OrderStatusId == id).ToList();

        //    foreach (Order order in orders)
        //    {
        //        order.OrderStatusId = new Guid("11869f4d-d6d1-434c-a2f1-7945227cd3bb");

        //    }

        //    db.SaveChanges();
        //    return string.Empty;
        //}

        public void UpdateOrderCodes()
        {
            var orders = db.Orders.Where(c => c.IsDeleted == false).OrderBy(c => c.CreationDate).ToList();

            int i = 0;
            foreach (var order in orders)
            {
                order.Code = 100000 + i;
                i++;
            }
            db.SaveChanges();
        }
    }
}
