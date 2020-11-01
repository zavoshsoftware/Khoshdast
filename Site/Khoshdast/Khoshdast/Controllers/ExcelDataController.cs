using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using Helpers;
using Models;
using ViewModels;

namespace Khoshdast.Controllers
{
    public class ExcelDataController : Controller
    {
        public ActionResult Import()
        {
            UploadFile uploadFile = new UploadFile();
            return View(uploadFile);
        }

        [HttpPost]
        public ActionResult Import(UploadFile UploadFile)
        {
            if (ModelState.IsValid)
            {
                if (UploadFile.ExcelFile.ContentLength > 0)
                {
                    if (UploadFile.ExcelFile.FileName.EndsWith(".xlsx") || UploadFile.ExcelFile.FileName.EndsWith(".xls"))
                    {
                        XLWorkbook Workbook;
                        try
                        {
                            Workbook = new XLWorkbook(UploadFile.ExcelFile.InputStream);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(String.Empty, $"Check your file. {ex.Message}");
                            return View();
                        }
                        IXLWorksheet WorkSheet = null;
                        try
                        {
                            WorkSheet = Workbook.Worksheet("Sheet1");

                        }
                        catch
                        {
                            ModelState.AddModelError(String.Empty, "sheet not found!");
                            return View();
                        }
                           WorkSheet.FirstRow().Delete();//if you want to remove ist row
                        int newCode = 0;
                        int i = 1;

                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            BrandCheck(row.Cell(4).Value.ToString());
                        }
                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            UpdateRow(row.Cell(1).Value.ToString(), row.Cell(2).Value.ToString(),
                                Convert.ToInt32(row.Cell(3).Value.ToString()),
                                row.Cell(4).Value.ToString(),
                                Convert.ToDecimal(row.Cell(5).Value.ToString()),
                                Convert.ToInt32(row.Cell(6).Value.ToString()));
                            i++;
                        }

                        db.SaveChanges();

                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Only .xlsx and .xls files are allowed");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Not a valid file");
                    return View();
                }
            }
            return View();
        }

        public bool ConvertMattresVal(string hasMattres)
        {
            if (hasMattres.ToLower() == "y")
                return true;

            return false;
        }
        public decimal DecimalConvertor(string amount)
        {
            try
            {
                if (string.IsNullOrEmpty(amount))
                    return 0;

                return Convert.ToDecimal(amount);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void UpdateRow(string barcode, string title, int productGroupCode, string brandTitle, decimal amount,
            int qty)
        {
            CodeGenerator codeGenerator = new CodeGenerator();

            Product product = db.Products.FirstOrDefault(c => c.Barcode == barcode && c.IsDeleted == false);

            if (product == null)
            {
                bool isAvailable = qty > 0;
                Product oProduct = new Product()
                {
                    Title = title,
                    PageTitle = title,
                    Barcode = barcode,
                    BrandId = GetBrandIdByTitle(brandTitle),
                    Amount = amount,
                    Stock = qty,
                    SeedStock = qty,
                    Id = Guid.NewGuid(),
                    Code = codeGenerator.ReturnProductCode().ToString(),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    IsAvailable = isAvailable,
                    ImageUrl = "/Uploads/Product/" +barcode + ".jpg",
                    Order = 1,
                    Visit = 0,
                    SellNumber = 0,

                };

                db.Products.Add(oProduct);

                InsertToProductGroupRel(oProduct.Id, productGroupCode);
            }
            else
            {
                product.Title = title;
                product.BrandId = GetBrandIdByTitle(brandTitle);
                product.Amount = amount;
                product.Stock = qty;
                product.LastModifiedDate = DateTime.Now;

                InsertToProductGroupRel(product.Id, productGroupCode);
            }
        }

        public void InsertToProductGroupRel(Guid productId, int productGroupCode)
        {
            ProductGroup pg = db.ProductGroups.FirstOrDefault(c => c.Code == productGroupCode);

            if (pg != null)
            {
                if (!db.ProductGroupRelProducts.Any(c => c.ProductGroupId == pg.Id && c.ProductId == productId && c.IsDeleted == false))
                {
                    ProductGroupRelProduct productGroupRelProduct = new ProductGroupRelProduct()
                    {
                        ProductId = productId,
                        ProductGroupId = pg.Id,
                        Id = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        IsDeleted = false,
                        IsActive = true,
                    };

                    db.ProductGroupRelProducts.Add(productGroupRelProduct);
                }
            }
        }

        private DatabaseContext db = new DatabaseContext();

        public Guid GetBrandIdByTitle(string brandTitle)
        {
            Brand brand = db.Brands.FirstOrDefault(c => c.Title == brandTitle && c.IsDeleted == false);

            if (brand == null)
            {
                Brand oBrand = new Brand()
                {
                    Id = Guid.NewGuid(),
                    Title = brandTitle,
                    UrlParam = GetUrlParam(brandTitle),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    IsInHome = false,
                    Order = 1
                };

                db.Brands.Add(oBrand);

                return oBrand.Id;
            }

            return brand.Id;
        }

        public void BrandCheck(string brandTitle)
        {
            Brand brand = db.Brands.FirstOrDefault(c => c.Title == brandTitle && c.IsDeleted == false);

            if (brand == null)
            {
                Brand oBrand = new Brand()
                {
                    Id = Guid.NewGuid(),
                    Title = brandTitle,
                    UrlParam = GetUrlParam(brandTitle),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    IsInHome = false,
                    Order = 1
                };

                db.Brands.Add(oBrand);
                db.SaveChanges();
            }
        }
        public string GetUrlParam(string title)
        {
            title = ReplaceCharachter(title, '@');
            title = ReplaceCharachter(title, '#');
            title = ReplaceCharachter(title, '$');
            title = ReplaceCharachter(title, '&');
            title = ReplaceCharachter(title, '^');
            title = ReplaceCharachter(title, '/');
            title = ReplaceCharachter(title, ']');
            title = ReplaceCharachter(title, '[');
            title = ReplaceCharachter(title, '%');
            title = ReplaceCharachter(title, '?');
            title = ReplaceCharachter(title, '؟');
            title = ReplaceCharachter(title, '!');

            return title.Replace(' ', '-');
        }

        public string ReplaceCharachter(string title, char charachter)
        {
            if (title.Contains(charachter))
                return title.Replace(charachter, '-');
            return title;
        }
        //public void UpdateRow(string parentCode, string childTitle, decimal customerAmount, decimal storeAmount, decimal factoryAmount, decimal colorAdditiveAmount, decimal colorAdditiveStoreeAmount, decimal colorAdditiveFactoryAmount,bool hasMattress)
        //{
        //    Product product = UnitOfWork.ProductRepository.Get(current => current.Code == parentCode).FirstOrDefault();


        //    if (product != null)
        //    {
        //        if (string.IsNullOrEmpty(childTitle))
        //        {
        //            product.Amount = customerAmount;
        //            product.StoreAmount = storeAmount;
        //            product.FactoryAmount = factoryAmount;
        //            product.HasMattress = hasMattress;

        //            UnitOfWork.ProductRepository.Update(product);
        //            UnitOfWork.Save();

        //        }

        //        else
        //        {
        //            product.StoreAmount = storeAmount;
        //            product.FactoryAmount = factoryAmount;
        //            product.Amount = customerAmount;
        //            product.HasMattress = hasMattress;
        //            UnitOfWork.ProductRepository.Update(product);

        //            ProductColor productColor = UnitOfWork.ProductColorRepository
        //                .Get(current => current.ProductId == product.Id && current.Title == childTitle)
        //                .FirstOrDefault();

        //            if (productColor != null)
        //            {
        //                productColor.Amount = colorAdditiveAmount;
        //                productColor.FactoryAmount = colorAdditiveFactoryAmount;
        //                productColor.StoreAmount = colorAdditiveStoreeAmount;

        //                UnitOfWork.ProductColorRepository.Update(productColor);
        //            }
        //            else
        //            {
        //                ProductColor newChildProduct = new ProductColor
        //                {
        //                    Id = Guid.NewGuid(),
        //                    ProductId = product.Id,
        //                    Title = childTitle,
        //                    Amount = colorAdditiveAmount,
        //                    IsDeleted = false,
        //                    IsActive = true,
        //                    FactoryAmount = colorAdditiveFactoryAmount,
        //                    StoreAmount = colorAdditiveStoreeAmount,
        //                };

        //                UnitOfWork.ProductColorRepository.Insert(newChildProduct);

        //            }

        //            UnitOfWork.Save();
        //        }
        //    }
        //    //else
        //    //{
        //    //    Product oProduct=new Product()
        //    //    {
        //    //        Title = 
        //    //    };
        //    //}
        //}

        public string updateProductsImages()
        {
            List<Product> products = db.Products.ToList();

            foreach (Product product in products)
            {
                product.ImageUrl = "/Uploads/Product/" + product.ImageUrl;
            }

            db.SaveChanges();
            return "";
        }
    }
}