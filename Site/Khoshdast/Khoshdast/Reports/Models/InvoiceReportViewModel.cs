using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reports.Models
{
    public class InvoiceReportViewModel
    {
        public string Date { get; set; }
    
        public string CustomerCellNumber{ get; set; }
        public string CustomerAddress { get; set; }
    
        public string CustomerName { get; set; }
      
        public string OrderCode { get; set; }
        public string PaymentType { get; set; }
     
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string Discount { get; set; }
        public string Shipping { get; set; }
        public string OrderStatus { get; set; }
        public string IsPay { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string RefId { get; set; }
        public string PaymentDesc { get; set; }
     
        public IList<InvoiceProductViewModel> Products { get; set; }
    }

    public class InvoiceProductViewModel
    {
        public string Barcode { get; set; }
        public string ProductTitle { get; set; }
        public string Amount { get; set; }
        public string Quantity { get; set; }
        public string RowAmount { get; set; }
    }
}