using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Khoshdast.Controllers
{
    public class PaymentsController : Controller
    {

        private DatabaseContext db = new DatabaseContext();
        public ActionResult Index(Guid id)
        {
            List<Payment> payments = db.Payments.Where(current => current.OrderId == id).ToList();

            PutAmount(id);

            Order order = db.Orders.Find(id);

            if (order != null)
            {
                if (order.RemainAmount == 0)
                {
                    if (!order.IsPaid)
                    {
                        order.IsPaid = true;
                        order.LastModifiedDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }

            return View(payments);
        }

        public void PutAmount(Guid orderId)
        {
            Order order = db.Orders.Find(orderId);


            ViewBag.total = order.TotalAmountStr;
            ViewBag.payment = order.PaymentAmount;
            ViewBag.remain = order.RemainAmount;
            ViewBag.orderDate = order.OrderDateStr;
            ViewBag.code = order.Code;

        }
        public ActionResult Create(Guid id)
        {
            Payment depositePayment = db.Payments.Where(current => current.OrderId == id && current.IsDeposit).FirstOrDefault();

            if (depositePayment != null)
                ViewBag.hasDeposite = true;
            else
                ViewBag.hasDeposite = false;


            PutAmount(id);

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(c=>c.IsDeleted==false), "Id", "Title");

            ViewBag.id = id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Payment payment, Guid id, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                if (UpdateOrderPayment(id, payment.Amount))
                {
                    #region Upload and resize image if needed
                    string newFilenameUrl = string.Empty;
                    if (fileupload != null)
                    {
                        string filename = Path.GetFileName(fileupload.FileName);
                        string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                             + Path.GetExtension(filename);

                        newFilenameUrl = "/Uploads/paymentAttachment/" + newFilename;
                        string physicalFilename = Server.MapPath(newFilenameUrl);

                        fileupload.SaveAs(physicalFilename);

                        payment.FileAttched = newFilenameUrl;
                    }
                    #endregion


                    payment.OrderId = id;
                    payment.IsActive = true;
                    db.Payments.Add(payment);

                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = id });
                }
                ModelState.AddModelError("highAmount", "مبلغ وارد شده بیشتر از باقی مانده مبلغ سفارش می باشد");
            }
            PutAmount(id);

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(c => c.IsDeleted == false), "Id", "Title");
            return View(payment);
        }

        public bool UpdateOrderPayment(Guid orderId, decimal payment)
        {
            Order order =db.Orders.Find(orderId);

            if (payment > order.RemainAmount)
                return false;

            order.PaymentAmount = order.PaymentAmount + payment;

            order.RemainAmount = order.RemainAmount - payment;

            if (order.PaymentAmount == order.RemainAmount)
                order.IsPaid = true;

            

            return true;
        }


        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id.Value);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = payment.OrderId;

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Payment entity)
        {
            entity = db.Payments.Find(entity.Id);
            entity.IsDeleted = true;
            entity.DeletionDate=DateTime.Now;
            UpdateOrderPaymentOnDelete(entity.OrderId, entity.Amount);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = entity.OrderId });
        }


        public bool UpdateOrderPaymentOnDelete(Guid orderId, decimal payment)
        {
            Order order = db.Orders.Find(orderId);

            if (payment > order.RemainAmount)
                return false;

            order.PaymentAmount = order.PaymentAmount - payment;

            order.RemainAmount = order.RemainAmount + payment;

           

            return true;
        }

    }
}
