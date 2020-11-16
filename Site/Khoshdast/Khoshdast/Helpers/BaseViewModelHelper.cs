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
            List<MenuItems> menuItems = new List<MenuItems>();

            List<ProductGroup> productGroups = db.ProductGroups
                .Where(c => c.ParentId == null && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();

            foreach (ProductGroup productGroup in productGroups)
            {
                List<ProductGroup> simpleGroups = new List<ProductGroup>();

                if (!db.ProductGroups.Any(c =>
                    c.Parent.ParentId == productGroup.Id && c.IsDeleted == false && c.IsActive))

                    simpleGroups = db.ProductGroups
                        .Where(c => c.ParentId == productGroup.Id && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();

                menuItems.Add(new MenuItems()
                {
                    ParentProductGroup = productGroup,
                    ChildProductGroups = GetChildren(productGroup.Id),
                    SimpleProductGroups = simpleGroups,
                    NonChildProductGroups = GetNonChildChildren(productGroup.Id),
                });
            }
            return menuItems;
        }

        public List<ChildMenuItem> GetChildren(Guid parentId)
        {
            List<ChildMenuItem> children = new List<ChildMenuItem>();

            List<ProductGroup> productGroups = db.ProductGroups
                .Where(c => c.ParentId == parentId && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();


            foreach (ProductGroup productGroup in productGroups)
            {
                if (HasChild(productGroup.Id))
                {
                    List<ProductGroup> childProductGroups = db.ProductGroups
                        .Where(c => c.ParentId == productGroup.Id && c.IsDeleted == false && c.IsActive)
                        .OrderBy(c => c.Order).ToList();

                    children.Add(new ChildMenuItem()
                    {
                        ParentProductGroup = productGroup,
                        ChildProductGroups = childProductGroups,

                    });
                }
            }

            return children;
        }

        public List<ProductGroup> GetNonChildChildren(Guid parentId)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            List<ProductGroup> productGroups = db.ProductGroups
                .Where(c => c.ParentId == parentId && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();

            foreach (ProductGroup productGroup in productGroups)
            {
                if (!HasChild(productGroup.Id))
                {
                    list.Add(productGroup);
                }
            }

            return list;
        }

        public bool HasChild(Guid producGroupId)
        {
            if (db.ProductGroups.Any(c => c.ParentId == producGroupId && c.IsDeleted == false && c.IsActive))
                return true;

            return false;
        }

        public string GetTextItemByName(string name)
        {
            TextItem textItem = db.TextItems.FirstOrDefault(c => c.Name == name);
            if (textItem != null)
                return textItem.Summery;

            return string.Empty;
        }

    }
}