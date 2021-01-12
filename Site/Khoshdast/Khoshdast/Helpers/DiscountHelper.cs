using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Helpers
{
    public class DiscountHelper
    {
        private DatabaseContext db = new DatabaseContext();
        public decimal CalculateDiscountAmount(Models.DiscountCode discountCode, decimal totalAmount)
        {
            if (discountCode.IsPercent)
                return totalAmount * discountCode.Amount / 100;
            else
                return discountCode.Amount;
        }

        public void CheckProductGroupDiscount()
        {
            DateTime today = DateTime.Today;

            List<ProductGroupDiscount> productGroupDiscounts = db.ProductGroupDiscounts
                .Where(c => c.IsDeleted == false && c.IsActive && c.ExpireDate < today).ToList();

            foreach (ProductGroupDiscount productGroupDiscount in productGroupDiscounts)
            {
                productGroupDiscount.IsActive = false;
                productGroupDiscount.LastModifiedDate=DateTime.Now;
                
                DisableDiscountForProducts(productGroupDiscount.ProductGroupId);

                db.SaveChanges();
            }
        }

        public void DisableDiscountForProducts(Guid id)
        {
            ProductGroup oProductGroup = db.ProductGroups.Find(id);

            if (oProductGroup != null)
            {
                List<ProductGroupRelProduct> productGroupRelProducts = db.ProductGroupRelProducts
                    .Where(c => c.ProductGroupId == oProductGroup.Id && c.IsDeleted == false).ToList();

                List<Product> products = new List<Product>();

                foreach (ProductGroupRelProduct productGroupRelProduct in productGroupRelProducts)
                {
                    if (!products.Contains(productGroupRelProduct.Product))
                        products.Add(productGroupRelProduct.Product);
                }

                foreach (Product product in products)
                {
                    product.DiscountAmount = 0;
                    product.IsInPromotion = false;
                    product.LastModifiedDate = DateTime.Now;
                    product.ProductGroupDiscountId = null;
                }
            }
        }
    }
}