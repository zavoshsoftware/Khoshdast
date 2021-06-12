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
    [Authorize(Roles = "Administrator")]
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


                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            if (!string.IsNullOrEmpty(row.Cell(4).Value.ToString()))
                                BrandCheck(row.Cell(4).Value.ToString());
                        }

                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            UpdateRow(row.Cell(1).Value.ToString(), row.Cell(2).Value.ToString(),
                                 row.Cell(3).Value.ToString(),
                                row.Cell(4).Value.ToString(),
                                row.Cell(5).Value.ToString(),
                                row.Cell(6).Value.ToString());
                        }


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

        public void test()
        {
            decimal a = Convert.ToDecimal("");
            decimal ab = Convert.ToInt32("");
            decimal aaaDecimal = Convert.ToDecimal(null);
            decimal aaasdb = Convert.ToInt32(null);
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

        public void UpdateRow(string barcode, string title, string productGroupCode, string brandTitle, string amount,
            string qty)
        {
            CodeGenerator codeGenerator = new CodeGenerator();

            Product product = db.Products.FirstOrDefault(c => c.Barcode == barcode && c.IsDeleted == false);

            if (product == null)
            {
                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(productGroupCode) && !string.IsNullOrEmpty(qty) &&
                    !string.IsNullOrEmpty(brandTitle) && !string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(amount))
                {
                    int qtyInt = Convert.ToInt32(qty);
                    decimal amountDecimal = Convert.ToDecimal(amount) / 10;

                    int productGroupCodeInt = Convert.ToInt32(productGroupCode);

                    bool isAvailable = qtyInt > 0;
                    Product oProduct = new Product()
                    {
                        Title = title,
                        PageTitle = title,
                        Barcode = barcode,
                        BrandId = GetBrandIdByTitle(brandTitle),
                        Amount = amountDecimal,
                        Stock = qtyInt,
                        SeedStock = qtyInt,
                        Id = Guid.NewGuid(),
                        Code = codeGenerator.ReturnProductCode(),
                        CreationDate = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        IsAvailable = isAvailable,
                        ImageUrl = "/Uploads/Product/" + barcode + ".jpg",
                        Order = 1,
                        Visit = 0,
                        SellNumber = 0,

                    };

                    db.Products.Add(oProduct);

                    InsertToProductGroupRel(oProduct.Id, productGroupCodeInt);
                    db.SaveChanges();

                }

            }
            else
            {
                if (!string.IsNullOrEmpty(amount))
                {
                    decimal amountDecimal = Convert.ToDecimal(amount) / 10;
                    product.Amount = amountDecimal;
                    product.LastModifiedDate = DateTime.Now;
                    db.SaveChanges();

                }
                if (!string.IsNullOrEmpty(qty))
                {
                    int qtyInt = Convert.ToInt32(qty);
                    product.Stock = qtyInt;
                    product.LastModifiedDate = DateTime.Now;
                    db.SaveChanges();

                }


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

        public void RemoveOldProductGroupRel(Guid productId)
        {
            ProductGroupRelProduct productGroupRelProduct = db.ProductGroupRelProducts.FirstOrDefault
                                            (c => c.ProductId == productId && c.IsDeleted == false);


            if (productGroupRelProduct != null)
            {
                db.ProductGroupRelProducts.Remove(productGroupRelProduct);
                db.SaveChanges();
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

        public string updateProductsCodes()
        {
            List<Product> products = db.Products.ToList();

            int i = 100;
            foreach (Product product in products)
            {
                product.Code = i;
                i++;
            }

            db.SaveChanges();
            return "";
        }


        public ActionResult ImportNames()
        {
            UploadFile uploadFile = new UploadFile();
            return View(uploadFile);
        }

        [HttpPost]
        public ActionResult ImportNames(UploadFile UploadFile)
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


                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            if (!string.IsNullOrEmpty(row.Cell(4).Value.ToString()))
                                BrandCheck(row.Cell(4).Value.ToString());
                        }

                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            UpdateRowName(row.Cell(1).Value.ToString(), row.Cell(2).Value.ToString(), row.Cell(3).Value.ToString());
                        }


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


        public void UpdateRowName(string barcode, string title, string pgTitle)
        {
            CodeGenerator codeGenerator = new CodeGenerator();

            Product product = db.Products.FirstOrDefault(c => c.Barcode == barcode && c.IsDeleted == false);

            if (product != null)
            {
                if (!string.IsNullOrEmpty(title))
                {
                    product.Title = title;
                    db.SaveChanges();
                }
                if (!string.IsNullOrEmpty(pgTitle))
                {
                    int productGroupCodeInt = Convert.ToInt32(pgTitle);

                    RemoveOldProductGroupRel(product.Id);

                    InsertToProductGroupRel(product.Id, productGroupCodeInt);
                }
            }
        }
    }
}