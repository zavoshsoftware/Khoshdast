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
using System.Threading.Tasks;


namespace Khoshdast.Controllers
{
    public class AccountsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        [HttpGet]
        [Route("login")]
        public async Task<ActionResult> OtpLogin(string returnUrl)
        {
            ForgetPasswordViewModel model = new ForgetPasswordViewModel() { ReturnUrl = returnUrl };
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> OtpLogin(ForgetPasswordViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (model.UserCellNumber != null)
                {
                    string cellNumber = PersianToEnglish(model.UserCellNumber);

                    bool isValidMobile = Regex.IsMatch(cellNumber,
                        @"(^(09|9)[0123456789][0123456789]\d{7}$)|(^(09|9)[0123456789][0123456789]\d{7}$)",
                        RegexOptions.IgnoreCase);

                    if (isValidMobile)
                    {

                        var user = db.Users.FirstOrDefault(x => x.CellNum == model.UserCellNumber&&x.IsDeleted==false);

                        if (user != null)
                        {
                            user.Password = RandomCode();
                            user.LastModifiedDate = DateTime.Now;
                            db.SaveChanges();

                            SendSms.SendOtpSms(user.CellNum, user.Password, 35100);

                            return RedirectToAction("Activate", new { returnUrl = returnUrl, code = user.Code });
                        }
                        else
                        {
                            if (cellNumber.Length < 11)
                            {

                                TempData["WrongMobile"] = "شماره موبایل وارد شده صحیح نمی باشد";
                                return View(model);
                            }
                            User oUser = new User()
                            {
                                Id = Guid.NewGuid(),
                                FullName = cellNumber,
                                CellNum = cellNumber,
                                Password = RandomCode(),
                                IsDeleted = false,
                                IsActive = true,
                                CreationDate = DateTime.Now,
                                Code = codeGenerator.ReturnUserCode(),
                                RemainCredit = 0,
                                RoleId = db.Roles.FirstOrDefault(c => c.Name == "customer").Id
                            };

                            db.Users.Add(oUser);
                            db.SaveChanges();

                            SendSms.SendOtpSms(oUser.CellNum, oUser.Password, 35100);

                            return RedirectToAction("Activate", new { returnUrl = returnUrl, code = oUser.Code });
                        }
                    }
                    else
                    {
                        TempData["WrongMobile"] = "شماره موبایل وارد شده صحیح نمی باشد";
                        return View(model);
                    }
                }

            }
            else
            {
                TempData["WrongMobile"] = "شماره موبایل خود را وارد نمایید";
                return View(model);
            }
            return View();
        }


        [HttpGet]
        [Route("activate")]
        public async Task<ActionResult> Activate(int code, string returnUrl)
        {
            var user = db.Users.Where(c => c.Code == code).Select(c => new { c.CellNum, c.Password }).FirstOrDefault();
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.code = code;

            if (user != null)
            {
                ActivateAccountViewModel activateViewModel = new ActivateAccountViewModel()
                {
                    CellNumber = user.CellNum,
                };
                return View(activateViewModel);
            }

            return RedirectToAction("Login");
        }

        [Route("Activate")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(int code, ActivateAccountViewModel activateViewModel, string returnUrl)
        {
            if (!string.IsNullOrEmpty(activateViewModel.ActivationCode))
            {
                string activecode = PersianToEnglish(activateViewModel.ActivationCode);

                User user = db.Users.FirstOrDefault(c =>
                    c.Code == code && c.Password == activecode && c.IsDeleted == false);


                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        user.IsActive = true;
                        user.LastModifiedDate = DateTime.Now;

                        db.SaveChanges();
                    }

                    LoginTask(user);

                    if (returnUrl != null)
                        return Redirect(returnUrl);

                    return RedirectToAction("index", "home");
                }

                TempData["WrongActivationCode"] = "کد فعالسازی وارد شده صحیح نمی باشد.";
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.code = code;

                return View(activateViewModel);
            }


            TempData["WrongActivationCode"] = "کد فعالسازی را وارد کنید.";
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.code = code;

            return View(activateViewModel);
        }


        public string RandomCode()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000).ToString("D5");
            return (r);
        }


        public string PersianToEnglish(string persianStr)
        {

            Dictionary<string, string> LettersDictionary = new Dictionary<string, string>
            {
                ["۰"] = "0",
                ["۱"] = "1",
                ["۲"] = "2",
                ["۳"] = "3",
                ["۴"] = "4",
                ["۵"] = "5",
                ["۶"] = "6",
                ["۷"] = "7",
                ["۸"] = "8",
                ["۹"] = "9"
            };
            return LettersDictionary.Aggregate(persianStr, (current, item) =>
                current.Replace(item.Key, item.Value));
        }

        [Route("LoginRegister")]
        public ActionResult LoginRegister(string returnUrl)
        {
            LoginRegisterViewModel model = new LoginRegisterViewModel();
            model.ReturnUrl = returnUrl;
            return View(model);
        }
        /**********************( forget password )****************************/
        [HttpGet]
        [Route("forgetpassword")]
        public async Task<ActionResult> ForgetPassword()
        {
            ForgetPasswordViewModel model = new ForgetPasswordViewModel();
            return View(model);
        }
        [HttpPost]
        [Route("forgetpassword")]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserCellNumber != null)
                {
                    var user = db.Users.FirstOrDefault(x => x.CellNum == model.UserCellNumber);
                    if (user != null)
                    {
                        string nextLine = "\n";
                        TempData["Message"] = "رمز عبور از طریق پیامک برای شما ارسال شد";
                        SendSms.SendCommonSms(user.CellNum,
                            $"رمز عبور شما در وب سایت رنگ خوشدست :{nextLine} {user.Password}");
                        return RedirectToAction("LoginRegister");
                    }
                    else
                    {
                        TempData["WrongMobile"] = "شماره موبایل وارد شده در سایت ثبت نشده است";
                        return View(model);
                    }
                }

            }
            return View();
        }
        /**********************( change password )****************************/
        [HttpGet]
        [Route("changepassword")]
        public async Task<ActionResult> ChangePassword()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            return View(model);
        }
        [HttpPost]
        [Route("changepassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid && model.OldPassword != null &&
                model.NewPassword != null && model.ConfirmNewPassword != null)
            {

                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                var user = await db.Users.FirstOrDefaultAsync(x => x.Id == new Guid(id));
                if (user != null && (user.Password != model.OldPassword))
                {
                    TempData["WrongPassword"] = "کلمه عبور پیشین وارد شده صحیح نمی باشد";
                    return View(model);
                }
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    TempData["PasswordNotMatch"] = "کلمه عبور جدید با تکرار آن برار نیست";
                    return View(model);
                }
                user.Password = model.NewPassword;
                await db.SaveChangesAsync();
                TempData["Message"] = "کلمه عبور با موفقیت تغییر یافت";
                return RedirectToAction("Index", "Dashboard");

            }

            return View();
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

            LoginRegisterViewModel result = new LoginRegisterViewModel();
            result.ReturnUrl = model.ReturnUrl;
            return RedirectToAction("LoginRegister");
        }
        CodeGenerator codeGenerator = new CodeGenerator();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(LoginRegisterViewModel model)
        {
            if (!model.RegisterCellNumber.StartsWith("0") || !model.RegisterCellNumber.StartsWith("۰") &&
                model.RegisterCellNumber.Length < 11)
            {
                model.RegisterCellNumber = 0.ToString() + model.RegisterCellNumber;
            }
            model.RegisterCellNumber = model.RegisterCellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");

            bool isValidMobile = Regex.IsMatch(model.RegisterCellNumber, @"(^(09|9)[0-9][0-9]\d{7}$)|(^(09|9)[3][12456]\d{7}$)", RegexOptions.IgnoreCase);

            if (!isValidMobile)
            {
                TempData["WrongRegisterCellnumber"] = "شماره موبایل وارد شده صحیح نمی باشد.";
                //LoginRegisterViewModel result = new LoginRegisterViewModel();
                //result.ReturnUrl = model.ReturnUrl;
                return RedirectToAction("LoginRegister");
            }

            if (!string.IsNullOrEmpty(model.RegisterEmail))
            {
                bool isEmail = Regex.IsMatch(model.RegisterEmail,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase);

                if (!isEmail)
                {
                    TempData["WrongRegisterEmail"] = "ایمیل وارد شده صحیح نمی باشد.";
                    //LoginRegisterViewModel result = new LoginRegisterViewModel();
                    //result.ReturnUrl = model.ReturnUrl;
                    return RedirectToAction("LoginRegister");
                }
            }

            User user = db.Users.FirstOrDefault(c => c.CellNum == model.RegisterCellNumber);
            if (user != null)
            {
                TempData["WrongRegisterDuplicate"] = "این شماره موبایل قبلا در سایت ثبت شده است. با شماره موبایل دیگری ثبت نام کنید.";
                //LoginRegisterViewModel result = new LoginRegisterViewModel();
                //result.ReturnUrl = model.ReturnUrl;
                return RedirectToAction("LoginRegister");
            }


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