using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Helpers;
using ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Khoshdast.Controllers
{
    public class AccountsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        [Route("LoginRegister")]
        public ActionResult LoginRegister(string returnUrl)
        {
            LoginRegisterViewModel model = new LoginRegisterViewModel();
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginRegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                User oUser = db.Users.Include(u => u.Role)
                    .FirstOrDefault(a =>
                        a.CellNum == model.LoginCellNumber
                        && a.Password == model.LoginPassword
                        && a.IsActive
                        && a.IsDeleted == false);

                if (oUser != null)
                {
                    LoginTask(oUser);
                    return RedirectToLocal(model.ReturnUrl, oUser.Role.Name);
                }
                else
                {
                    // invalid username or password
                    TempData["WrongPass"] = "شماره موبایل و یا کلمه عبور وارد شده صحیح نمی باشد.";
                }
            }

            return RedirectToAction("LoginRegister", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(LoginRegisterViewModel model)
        {
            model.RegisterCellNumber = model.RegisterCellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");

            bool isValidMobile = Regex.IsMatch(model.RegisterCellNumber, @"(^(09|9)[0-9][0-9]\d{7}$)|(^(09|9)[3][12456]\d{7}$)", RegexOptions.IgnoreCase);

            if (!isValidMobile)
            {
                TempData["WrongRegisterCellnumber"] = "شماره موبایل وارد شده صحیح نمی باشد.";
                return RedirectToAction("LoginRegister", model);
            }

            if (!string.IsNullOrEmpty(model.RegisterEmail))
            {
                bool isEmail = Regex.IsMatch(model.RegisterEmail,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase);

                if (!isEmail)
                {
                    TempData["WrongRegisterEmail"] = "ایمیل وارد شده صحیح نمی باشد.";
                    return RedirectToAction("LoginRegister", model);
                }
            }

            User user = db.Users.FirstOrDefault(c => c.CellNum == model.RegisterCellNumber);
            if (user != null)
            {
                TempData["WrongRegisterDuplicate"] = "این شماره موبایل قبلا در سایت ثبت شده است. با شماره موبایل دیگری ثبت نام کنید.";
                return RedirectToAction("LoginRegister", model);
            }

            CodeGenerator codeGenerator = new CodeGenerator();

            User oUser = new User()
            {
                Id = Guid.NewGuid(),
                CellNum = model.RegisterCellNumber,
                Email = model.RegisterEmail,
                Password = model.RegisterPassword,
                FullName = model.RegisterFullName,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.Now,
                Code = codeGenerator.ReturnUserCode(),
                RemainCredit = 0,
                RoleId = db.Roles.FirstOrDefault(c => c.Name == "customer").Id
            };

            db.Users.Add(oUser);
            db.SaveChanges();
            LoginTask(oUser);
            return RedirectToLocal(model.ReturnUrl, oUser.Role.Name);
        }

        public void LoginTask(User oUser)
        {
            var ident = new ClaimsIdentity(
                new[] { 
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier, oUser.CellNum),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                    new Claim(ClaimTypes.Name,oUser.Id.ToString()),

                    // optionally you could add roles if any
                    new Claim(ClaimTypes.Role, oUser.Role.Name),
                    new Claim(ClaimTypes.Surname, oUser.FullName),

                },
                DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.GetOwinContext().Authentication.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(600),

                },
                ident);
        }

        private ActionResult RedirectToLocal(string returnUrl, string role)
        {

            if (role.ToLower().Contains("admin"))
                return RedirectToAction("dashboard", "Home");
            if (role.ToLower().Contains("content"))
                return RedirectToAction("Index", "Blogs");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl == "checkout")
                    return Redirect("/checkout");
                return Redirect(returnUrl);
            }
            if (role.ToLower().Contains("customer"))
                return RedirectToAction("Index", "Home");


            return Redirect("/");
        }
        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
            }
            return Redirect("/");
        }

    }
}