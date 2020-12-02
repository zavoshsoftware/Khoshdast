using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Helpers
{
    public class CodeGenerator
    {
        private DatabaseContext db = new DatabaseContext();
        public int ReturnOrderCode()
        {

            Order order = db.Orders.Where(c => c.IsDeleted == false).OrderByDescending(current => current.Code).FirstOrDefault();

            if (order != null)
            {
                return order.Code + 1;
            }
            return 100000;
        }


        public int ReturnProductCode()
        {
            Product product = db.Products.Where(c => c.IsDeleted == false).OrderByDescending(current => current.Code).FirstOrDefault();

            if (product != null)
            {
                return Convert.ToInt32(product.Code) + 1;
            }
            return 100;
        }

        public int ReturnProductGroupCode()
        {
            ProductGroup productGroup = db.ProductGroups.Where(c => c.IsDeleted == false).OrderByDescending(current => current.Code).FirstOrDefault();

            if (productGroup != null)
            {
                if (productGroup.Code != null)
                    return (productGroup.Code.Value) + 1;
            }
            return 1;
        }

        public int ReturnUserCode()
        {
            User user = db.Users.Where(c => c.IsDeleted == false).OrderByDescending(current => current.Code).FirstOrDefault();

            if (user != null)
            {
                return Convert.ToInt32(user.Code) + 1;
            }
            return 1;
        }


    }
}