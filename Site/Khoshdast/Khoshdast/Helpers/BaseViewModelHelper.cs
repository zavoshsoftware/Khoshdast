using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Models;
using ViewModels;

//using ViewModels;

namespace Helpers
{
    public class BaseViewModelHelper
    {
        private DatabaseContext db = new DatabaseContext();

        public List<MenuItems> GetMenuProductGroup()
        {
            List < MenuItems > menuItems=new List<MenuItems>();

            List<ProductGroup> productGroups = db.ProductGroups
                .Where(c => c.ParentId == null && c.IsDeleted == false && c.IsActive).OrderBy(c=>c.Order).ToList();

            foreach (ProductGroup productGroup in productGroups)
            {
                menuItems.Add(new MenuItems()
                {
                    ParentProductGroup = productGroup,
                    ChildProductGroups =
                        db.ProductGroups.Where(c => c.ParentId == productGroup.Id && c.IsDeleted == false && c.IsActive)
                            .OrderBy(c => c.Order).ToList()
                });
            }
            return menuItems;
        }

   

    }
}